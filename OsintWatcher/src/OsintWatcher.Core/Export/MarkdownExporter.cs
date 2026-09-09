using System.Text;
using OsintWatcher.Core.Models;

namespace OsintWatcher.Core.Export;

public sealed class MarkdownExporter : IReportExporter
{
    public string FileExtension => "md";

    public string DisplayName => "Markdown (readable report)";

    public byte[] Export(InvestigationReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# {Escape(report.Title)}");
        sb.AppendLine();
        sb.AppendLine($"Generated: {report.GeneratedAtUtc:u}");
        if (!string.IsNullOrWhiteSpace(report.InvestigatorNote))
            sb.AppendLine($"Note: {Escape(report.InvestigatorNote)}");
        sb.AppendLine();
        sb.AppendLine($"**Summary** — Modified: {report.ModifiedCount} · Unchanged: {report.UnchangedCount} " +
                       $"· Unreachable: {report.UnreachableCount} · Baseline: {report.BaselineCount}");
        sb.AppendLine();

        sb.AppendLine("## Modified pages");
        AppendSection(sb, report.Entries.Where(e => e.Latest?.Verdict == ScanVerdict.Modified));

        sb.AppendLine("## Unchanged pages");
        AppendSection(sb, report.Entries.Where(e => e.Latest?.Verdict == ScanVerdict.Unchanged));

        sb.AppendLine("## Unreachable pages");
        AppendSection(sb, report.Entries.Where(e => e.Latest?.Verdict == ScanVerdict.Unreachable));

        sb.AppendLine("## Baseline only (no prior snapshot to compare)");
        AppendSection(sb, report.Entries.Where(e => e.Latest?.Verdict == ScanVerdict.Baseline));

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    static void AppendSection(StringBuilder sb, IEnumerable<PageReportEntry> entries)
    {
        var list = entries.ToList();
        if (list.Count == 0) { sb.AppendLine("_None._"); sb.AppendLine(); return; }

        foreach (var e in list)
        {
            var r = e.Latest!;
            sb.AppendLine($"### {Escape(e.Page.Label.Length > 0 ? e.Page.Label : e.Page.Url)}");
            sb.AppendLine($"- URL: {e.Page.Url}");
            if (!string.IsNullOrWhiteSpace(e.Page.CaseReference)) sb.AppendLine($"- Case reference: {Escape(e.Page.CaseReference)}");
            sb.AppendLine($"- Checked: {r.TimestampUtc:u}");
            sb.AppendLine($"- HTTP status: {r.HttpStatusCode?.ToString() ?? "N/A"}");
            if (r.Fingerprint is not null)
            {
                sb.AppendLine($"- Visible-text SHA-256: `{r.Fingerprint.VisibleTextSha256}`");
                sb.AppendLine($"- Raw SHA-256: `{r.Fingerprint.RawSha256}`");
                if (r.Fingerprint.TlsCertificateSha256 is not null)
                    sb.AppendLine($"- TLS certificate SHA-256: `{r.Fingerprint.TlsCertificateSha256}`");
            }
            if (r.ErrorMessage is not null) sb.AppendLine($"- Error: {Escape(r.ErrorMessage)}");
            if (r.Diff is not null)
            {
                sb.AppendLine($"- Diff: +{r.Diff.LinesAdded} / -{r.Diff.LinesRemoved} lines ({r.Diff.ChangePercent:0.00}% changed)");
                foreach (var line in r.Diff.Sample.Take(20))
                    sb.AppendLine($"  - `{(line.Kind == DiffLineKind.Added ? "+" : "-")}` {Escape(Truncate(line.Text, 160))}");
            }
            sb.AppendLine();
        }
    }

    static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "…";

    static string Escape(string s) => s.Replace("|", "\\|");
}
