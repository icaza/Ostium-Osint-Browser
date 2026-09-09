namespace OsintWatcher.Core.Models;

/// <summary>Raw outcome of one HTTP fetch, before fingerprinting.</summary>
public sealed class FetchResult
{
    public required bool Success { get; init; }
    public string? ErrorMessage { get; init; }

    public int? HttpStatusCode { get; init; }
    public byte[] RawBytes { get; init; } = Array.Empty<byte>();
    public string Html { get; init; } = string.Empty;
    public string? TlsCertificateSha256 { get; init; }
    public Dictionary<string, string> RelevantHeaders { get; init; } = new();
}
