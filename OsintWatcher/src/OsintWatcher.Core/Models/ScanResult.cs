namespace OsintWatcher.Core.Models;

public enum ScanVerdict
{
    /// <summary>First observation of this page; nothing to compare against yet.</summary>
    Baseline,
    Unchanged,
    Modified,
    /// <summary>Fetch failed (network, timeout, DNS, TLS, disallowed target, etc.).</summary>
    Unreachable
}

/// <summary>One point-in-time check of a MonitoredPage.</summary>
public sealed class ScanResult
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid PageId { get; init; }
    public required string Url { get; init; }
    public DateTimeOffset TimestampUtc { get; init; } = DateTimeOffset.UtcNow;

    public ScanVerdict Verdict { get; set; } = ScanVerdict.Baseline;

    public FingerprintSet? Fingerprint { get; set; }

    public int? HttpStatusCode { get; set; }
    public string? ErrorMessage { get; set; }

    /// <summary>Response headers considered evidentially relevant (Server, Last-Modified, ETag, ...).</summary>
    public Dictionary<string, string> RelevantHeaders { get; init; } = new();

    public DiffResult? Diff { get; set; }

    /// <summary>Compressed, encrypted-at-rest snapshot of the visible text content, kept for
    /// future re-diffing and export. Populated by the storage layer, not the fetcher.</summary>
    public byte[]? SnapshotTextGz { get; set; }
}
