using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using OsintWatcher.Core.Models;
using OsintWatcher.Core.Security;

namespace OsintWatcher.Core.Services;

public sealed class FetcherOptions
{
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(20);
    public long MaxResponseBytes { get; init; } = 25 * 1024 * 1024; // 25 MB hard cap
    public bool AllowPrivateTargets { get; init; } = false;
    public string UserAgent { get; init; } =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0 Safari/537.36";
    /// <summary>Minimum, per-domain delay before this fetcher will issue another request —
    /// basic politeness / anti-flood safeguard, independent of any UI-level scheduling.</summary>
    public TimeSpan MinDelayPerDomain { get; init; } = TimeSpan.FromSeconds(2);
}

/// <summary>
/// Hardened HTTP(S) fetcher: enforces the SSRF allowlist via UrlValidator, pins TLS 1.2+,
/// captures the leaf certificate's thumbprint for infrastructure-level fingerprinting,
/// bounds response size to avoid memory-exhaustion from a malicious/huge response, and
/// never executes any script from the fetched page (plain HTTP GET, not a browser engine).
/// </summary>
public sealed class SecureHttpFetcher : IDisposable
{
    private readonly FetcherOptions _options;
    private readonly HttpClient _client;
    private readonly Dictionary<string, DateTimeOffset> _lastRequestPerDomain = new();
    private string? _lastCertThumbprint;

    public SecureHttpFetcher(FetcherOptions? options = null)
    {
        _options = options ?? new FetcherOptions();

        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 5,
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13,
            ServerCertificateCustomValidationCallback = ValidateCertificate,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
        };

        _client = new HttpClient(handler) { Timeout = _options.Timeout };
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(_options.UserAgent);
        _client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
    }

    private bool ValidateCertificate(HttpRequestMessage request, X509Certificate2? cert, X509Chain? chain, SslPolicyErrors errors)
    {
        if (cert is not null)
            _lastCertThumbprint = Convert.ToHexString(SHA256.HashData(cert.RawData));

        // Reject invalid/untrusted/expired certificates by default — do not silently
        // downgrade security to make a broken target "work".
        return errors == SslPolicyErrors.None;
    }

    public async Task<FetchResult> FetchAsync(string url, CancellationToken ct = default) =>
        await FetchAsync(url, _options.AllowPrivateTargets, ct);

    public async Task<FetchResult> FetchAsync(string url, bool allowPrivateTargets, CancellationToken ct = default)
    {
        var validation = await UrlValidator.ValidateAsync(url, allowPrivateTargets, ct);
        if (!validation.IsValid)
            return new FetchResult { Success = false, ErrorMessage = validation.Reason };

        var uri = validation.Normalized!;
        await RespectDomainDelayAsync(uri.Host, ct);

        _lastCertThumbprint = null;
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

            var headers = ExtractRelevantHeaders(response);

            if (!response.IsSuccessStatusCode)
            {
                return new FetchResult
                {
                    Success = false,
                    HttpStatusCode = (int)response.StatusCode,
                    ErrorMessage = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}",
                    RelevantHeaders = headers,
                    TlsCertificateSha256 = _lastCertThumbprint
                };
            }

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var bounded = new MemoryStream();
            var buffer = new byte[81920];
            long total = 0;
            int read;
            while ((read = await stream.ReadAsync(buffer, ct)) > 0)
            {
                total += read;
                if (total > _options.MaxResponseBytes)
                    return new FetchResult
                    {
                        Success = false,
                        ErrorMessage = $"Response exceeded the {_options.MaxResponseBytes / (1024 * 1024)} MB cap; aborted.",
                        HttpStatusCode = (int)response.StatusCode,
                        RelevantHeaders = headers,
                        TlsCertificateSha256 = _lastCertThumbprint
                    };
                bounded.Write(buffer, 0, read);
            }

            var rawBytes = bounded.ToArray();
            var html = Encoding.UTF8.GetString(rawBytes);

            return new FetchResult
            {
                Success = true,
                HttpStatusCode = (int)response.StatusCode,
                RawBytes = rawBytes,
                Html = html,
                RelevantHeaders = headers,
                TlsCertificateSha256 = _lastCertThumbprint
            };
        }
        catch (TaskCanceledException)
        {
            return new FetchResult { Success = false, ErrorMessage = $"Timed out after {_options.Timeout.TotalSeconds:0}s." };
        }
        catch (HttpRequestException ex)
        {
            return new FetchResult { Success = false, ErrorMessage = ex.Message, TlsCertificateSha256 = _lastCertThumbprint };
        }
    }

    private static Dictionary<string, string> ExtractRelevantHeaders(HttpResponseMessage response)
    {
        string[] wanted = { "Server", "Last-Modified", "ETag", "Content-Type", "Content-Security-Policy" };
        var result = new Dictionary<string, string>();
        foreach (var name in wanted)
        {
            if (response.Headers.TryGetValues(name, out var v1)) result[name] = string.Join(", ", v1);
            else if (response.Content.Headers.TryGetValues(name, out var v2)) result[name] = string.Join(", ", v2);
        }
        return result;
    }

    private async Task RespectDomainDelayAsync(string host, CancellationToken ct)
    {
        if (_lastRequestPerDomain.TryGetValue(host, out var last))
        {
            var elapsed = DateTimeOffset.UtcNow - last;
            var remaining = _options.MinDelayPerDomain - elapsed;
            if (remaining > TimeSpan.Zero)
                await Task.Delay(remaining, ct);
        }
        _lastRequestPerDomain[host] = DateTimeOffset.UtcNow;
    }

    public void Dispose() => _client.Dispose();
}
