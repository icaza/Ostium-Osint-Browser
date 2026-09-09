using OsintWatcher.Core.Models;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace OsintWatcher.Core.Export;

/// <summary>Full-fidelity machine-readable export — every hash layer, header, and diff
/// sample is preserved, making this the format to hand off to other tooling or archive as
/// primary evidence.</summary>
public sealed class JsonExporter : IReportExporter
{
    public string FileExtension => "json";
    public string DisplayName => "JSON (full fidelity)";

    static readonly JsonSerializerOptions Opts = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public byte[] Export(InvestigationReport report) =>
        JsonSerializer.SerializeToUtf8Bytes(report, Opts);
}
