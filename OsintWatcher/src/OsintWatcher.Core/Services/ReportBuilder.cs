using OsintWatcher.Core.Models;
using OsintWatcher.Core.Storage;

namespace OsintWatcher.Core.Services;

/// <summary>Assembles the InvestigationReport (the single model every exporter renders from)
/// from the current page list and each page's stored history.</summary>
public static class ReportBuilder
{
    public static InvestigationReport Build(EncryptedStore store, IEnumerable<MonitoredPage> pages,
        string title = "OSINT Watcher — Surveillance report", string investigatorNote = "")
    {
        var entries = pages.Select(page =>
        {
            var history = store.GetHistory(page.Id);
            return new PageReportEntry
            {
                Page = page,
                Latest = history.LastOrDefault(),
                TotalChecks = history.Count,
                TotalModifications = history.Count(h => h.Verdict == ScanVerdict.Modified)
            };
        }).ToList();

        return new InvestigationReport
        {
            Title = title,
            GeneratedAtUtc = DateTimeOffset.UtcNow,
            InvestigatorNote = investigatorNote,
            Entries = entries
        };
    }
}
