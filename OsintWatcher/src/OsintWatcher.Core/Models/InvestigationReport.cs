namespace OsintWatcher.Core.Models;

/// <summary>Single source of truth every exporter renders from, so the five export formats
/// never drift out of sync with each other.</summary>
public sealed class InvestigationReport
{
    public required string Title { get; init; }
    public required DateTimeOffset GeneratedAtUtc { get; init; }
    public string InvestigatorNote { get; init; } = string.Empty;
    public required List<PageReportEntry> Entries { get; init; }

    public int ModifiedCount => Entries.Count(e => e.Latest?.Verdict == ScanVerdict.Modified);
    public int UnchangedCount => Entries.Count(e => e.Latest?.Verdict == ScanVerdict.Unchanged);
    public int UnreachableCount => Entries.Count(e => e.Latest?.Verdict == ScanVerdict.Unreachable);
    public int BaselineCount => Entries.Count(e => e.Latest?.Verdict == ScanVerdict.Baseline);
}

public sealed class PageReportEntry
{
    public required MonitoredPage Page { get; init; }
    public required ScanResult? Latest { get; init; }
    public required int TotalChecks { get; init; }
    public required int TotalModifications { get; init; }
}
