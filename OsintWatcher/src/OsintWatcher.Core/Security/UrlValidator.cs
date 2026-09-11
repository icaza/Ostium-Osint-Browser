using System.Net;
using System.Net.Sockets;

namespace OsintWatcher.Core.Security;

/// <summary>
/// Validates monitoring targets before any request is issued. Blocks the classic SSRF
/// surface (loopback, link-local, private ranges, non-http(s) schemes) by default, since a
/// monitoring tool that fetches investigator-supplied URLs is a textbook SSRF vector if left
/// unchecked. Private/internal targets can be explicitly allowed per page for investigators
/// who genuinely need to watch internal infrastructure.
/// </summary>
public static class UrlValidator
{
    public sealed record ValidationResult(bool IsValid, string? Reason, Uri? Normalized);

    public static async Task<ValidationResult> ValidateAsync(string rawUrl, bool allowPrivateTargets, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rawUrl))
            return new ValidationResult(false, "Empty URL.", null);

        if (!Uri.TryCreate(rawUrl.Trim(), UriKind.Absolute, out var uri))
            return new ValidationResult(false, "Not a well-formed absolute URL.", null);

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return new ValidationResult(false, "Only http:// and https:// are permitted.", null);

        if (!string.IsNullOrEmpty(uri.UserInfo))
            return new ValidationResult(false, "URLs carrying embedded credentials are rejected.", null);

        if (allowPrivateTargets)
            return new ValidationResult(true, null, uri);

        // Resolve and check every candidate IP — protects against DNS rebinding to a
        // private address as well as literal private/loopback hosts.
        IPAddress[] addresses;
        if (IPAddress.TryParse(uri.Host, out var literal))
        {
            addresses = new[] { literal };
        }
        else
        {
            try
            {
                addresses = await Dns.GetHostAddressesAsync(uri.Host, ct);
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, $"DNS resolution failed: {ex.Message}", null);
            }
        }

        if (addresses.Length == 0)
            return new ValidationResult(false, "Host did not resolve to any address.", null);

        foreach (var ip in addresses)
        {
            if (IsDisallowed(ip))
                return new ValidationResult(false,
                    $"Target resolves to a private/loopback/link-local address ({ip}). " +
                    "Enable 'Allow private targets' for this page if this is intentional.", null);
        }

        return new ValidationResult(true, null, uri);
    }

    static bool IsDisallowed(IPAddress ip)
    {
        if (IPAddress.IsLoopback(ip)) return true;

        var bytes = ip.GetAddressBytes();

        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            // 10.0.0.0/8
            if (bytes[0] == 10) return true;
            // 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] is >= 16 and <= 31) return true;
            // 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168) return true;
            // 169.254.0.0/16 (link-local / cloud metadata endpoint range)
            if (bytes[0] == 169 && bytes[1] == 254) return true;
            // 100.64.0.0/10 (carrier-grade NAT)
            if (bytes[0] == 100 && bytes[1] is >= 64 and <= 127) return true;
            // 0.0.0.0/8
            if (bytes[0] == 0) return true;
        }
        else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
        {
            if (ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal || ip.IsIPv6Multicast) return true;
            // fc00::/7 (unique local)
            if ((bytes[0] & 0xFE) == 0xFC) return true;
        }

        return false;
    }
}
