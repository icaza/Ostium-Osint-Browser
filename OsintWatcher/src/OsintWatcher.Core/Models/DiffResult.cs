namespace OsintWatcher.Core.Models;

/// <summary>Line-level evidence of what changed between two visible-text snapshots.</summary>
public sealed class DiffResult
{
    public int LinesAdded { get; init; }
    public int LinesRemoved { get; init; }
    public int LinesUnchanged { get; init; }

    /// <summary>0-100, share of lines that differ. Used as a "how much changed" indicator.</summary>
    public double ChangePercent { get; init; }

    /// <summary>Bounded list of representative +/- line entries for the report (not the full diff,
    /// to keep exports readable — capped by DiffService).</summary>
    public List<DiffLine> Sample { get; init; } = new();
}

public sealed record DiffLine(DiffLineKind Kind, string Text);

public enum DiffLineKind { Added, Removed }
