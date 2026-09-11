using OsintWatcher.Core.Models;

namespace OsintWatcher.Core.Export;

public interface IReportExporter
{
    string FileExtension { get; }
    string DisplayName { get; }
    byte[] Export(InvestigationReport report);
}
