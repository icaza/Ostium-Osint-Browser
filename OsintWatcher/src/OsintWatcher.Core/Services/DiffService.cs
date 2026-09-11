using OsintWatcher.Core.Models;

namespace OsintWatcher.Core.Services;

/// <summary>
/// Produces line-level evidence of what changed between two visible-text snapshots.
/// Uses a standard LCS (longest common subsequence) diff; for very large pages this is
/// capped to keep the check fast, degrading gracefully to a coarser added/removed set
/// beyond the cap rather than hanging.
/// </summary>
public static class DiffService
{
    const int MaxLinesForExactDiff = 4000;
    const int MaxSampleLines = 200;

    public static DiffResult Compare(string previousText, string currentText)
    {
        var before = SplitLines(previousText);
        var after = SplitLines(currentText);

        if (before.Length <= MaxLinesForExactDiff && after.Length <= MaxLinesForExactDiff)
            return ExactLcsDiff(before, after);

        return CoarseSetDiff(before, after);
    }

    static string[] SplitLines(string text) =>
        text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

    static DiffResult ExactLcsDiff(string[] before, string[] after)
    {
        int n = before.Length, m = after.Length;
        var lcs = new int[n + 1, m + 1];
        for (int i = n - 1; i >= 0; i--)
            for (int j = m - 1; j >= 0; j--)
                lcs[i, j] = before[i] == after[j] ? lcs[i + 1, j + 1] + 1 : Math.Max(lcs[i + 1, j], lcs[i, j + 1]);

        var sample = new List<DiffLine>();
        int added = 0, removed = 0, unchanged = 0;
        int a = 0, b = 0;
        while (a < n && b < m)
        {
            if (before[a] == after[b]) { unchanged++; a++; b++; }
            else if (lcs[a + 1, b] >= lcs[a, b + 1])
            {
                removed++;
                if (sample.Count < MaxSampleLines) sample.Add(new DiffLine(DiffLineKind.Removed, before[a]));
                a++;
            }
            else
            {
                added++;
                if (sample.Count < MaxSampleLines) sample.Add(new DiffLine(DiffLineKind.Added, after[b]));
                b++;
            }
        }
        while (a < n) { removed++; if (sample.Count < MaxSampleLines) sample.Add(new DiffLine(DiffLineKind.Removed, before[a])); a++; }
        while (b < m) { added++; if (sample.Count < MaxSampleLines) sample.Add(new DiffLine(DiffLineKind.Added, after[b])); b++; }

        double changePercent = Math.Round(100.0 * (added + removed) / Math.Max(1, unchanged + added + removed), 2);

        return new DiffResult
        {
            LinesAdded = added,
            LinesRemoved = removed,
            LinesUnchanged = unchanged,
            ChangePercent = changePercent,
            Sample = sample
        };
    }

    /// <summary>Fallback for very large documents: set-based added/removed without ordering,
    /// still gives a usable magnitude-of-change figure.</summary>
    static DiffResult CoarseSetDiff(string[] before, string[] after)
    {
        var beforeSet = new HashSet<string>(before);
        var afterSet = new HashSet<string>(after);

        var added = afterSet.Except(beforeSet).ToList();
        var removed = beforeSet.Except(afterSet).ToList();
        var unchanged = Math.Max(0, Math.Min(before.Length, after.Length) - Math.Max(added.Count, removed.Count));

        var sample = new List<DiffLine>();
        sample.AddRange(removed.Take(MaxSampleLines / 2).Select(l => new DiffLine(DiffLineKind.Removed, l)));
        sample.AddRange(added.Take(MaxSampleLines / 2).Select(l => new DiffLine(DiffLineKind.Added, l)));

        double changePercent = Math.Round(100.0 * (added.Count + removed.Count) / Math.Max(1, unchanged + added.Count + removed.Count), 2);

        return new DiffResult
        {
            LinesAdded = added.Count,
            LinesRemoved = removed.Count,
            LinesUnchanged = unchanged,
            ChangePercent = changePercent,
            Sample = sample
        };
    }
}
