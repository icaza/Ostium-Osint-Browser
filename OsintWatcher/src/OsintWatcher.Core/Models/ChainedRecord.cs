namespace OsintWatcher.Core.Models;

/// <summary>
/// Wraps a ScanResult in a simple hash chain (each record commits to the previous record's
/// chain hash) so that the local evidence log is tamper-evident: editing or deleting a past
/// entry breaks every chain hash that follows it, and StorageIntegrity can detect that.
/// </summary>
public sealed class ChainedRecord
{
    public required ScanResult Result { get; init; }
    public required string PrevChainHash { get; init; }
    public required string ChainHash { get; init; }
}
