namespace OsintWatcher.Core.Models;

/// <summary>
/// Layered cryptographic fingerprint of one fetch. Several independent hashes are kept
/// instead of a single digest so that a verdict can distinguish real content edits from
/// incidental noise (ads, timestamps, whitespace), and so a change can be authenticated
/// at the layer where it actually occurred.
/// </summary>
public sealed class FingerprintSet
{
    /// <summary>SHA-256 of the exact bytes received on the wire. Strongest evidentiary value,
    /// but flips on any byte-level change including dynamic noise.</summary>
    public required string RawSha256 { get; init; }

    /// <summary>SHA-256 of the response after normalizing whitespace and removing
    /// comments/script/style blocks. Filters most markup noise while staying content-aware.</summary>
    public required string NormalizedSha256 { get; init; }

    /// <summary>SHA-256 of the extracted, tag-stripped visible text only. Most robust against
    /// markup/attribute churn; best proxy for "did the human-visible content change".</summary>
    public required string VisibleTextSha256 { get; init; }

    /// <summary>SHA-256 of the DOM tag skeleton (tag names/order, text removed). Flags
    /// structural/layout changes independent of copy edits.</summary>
    public required string StructuralSha256 { get; init; }

    /// <summary>SHA-256 thumbprint of the TLS leaf certificate presented, if HTTPS.
    /// Infrastructure-level signal (e.g. certificate reissued/replaced).</summary>
    public string? TlsCertificateSha256 { get; init; }

    public long ContentLengthBytes { get; init; }
}
