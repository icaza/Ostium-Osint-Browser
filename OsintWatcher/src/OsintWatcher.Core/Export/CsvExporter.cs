using System.Text;
using OsintWatcher.Core.Models;

namespace OsintWatcher.Core.Export;

/// <summary>Flat tabular summary — one row per monitored page — for spreadsheet triage.</summary>
public sealed class CsvExporter : IReportExporter
{
    public string FileExtension => "csv";
    public string DisplayName => "CSV (tabular summary)";

    public byte[] Export(InvestigationReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(',', new[]
        {
            "Label", "Url", "CaseReference", "Verdict", "LastCheckedUtc",
            "HttpStatus", "ChangePercent", "RawSha256", "VisibleTextSha256",
            "TlsCertificateSha256", "TotalChecks", "TotalModifications"
        }));

        foreach (var e in report.Entries)
        {
            var r = e.Latest;
            sb.AppendLine(string.Join(',', new[]
            {
                Csv(e.Page.Label),
                Csv(e.Page.Url),
                Csv(e.Page.CaseReference),
                Csv(r?.Verdict.ToString() ?? "N/A"),
                Csv(r?.TimestampUtc.ToString("u") ?? ""),
                Csv(r?.HttpStatusCode?.ToString() ?? ""),
                Csv(r?.Diff?.ChangePercent.ToString("0.00") ?? ""),
                Csv(r?.Fingerprint?.RawSha256 ?? ""),
                Csv(r?.Fingerprint?.VisibleTextSha256 ?? ""),
                Csv(r?.Fingerprint?.TlsCertificateSha256 ?? ""),
                Csv(e.TotalChecks.ToString()),
                Csv(e.TotalModifications.ToString())
            }));
        }

        return new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(sb.ToString());
    }

    static string Csv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        return value;
    }
}
