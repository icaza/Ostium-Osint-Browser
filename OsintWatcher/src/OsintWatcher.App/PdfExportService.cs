using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using OsintWatcher.Core.Export;
using OsintWatcher.Core.Models;

namespace OsintWatcher.App;

/// <summary>
/// Renders the report to PDF by driving a hidden WebView2 instance through the exact same
/// HTML the dashboard and the .html export use (HtmlExporter.Render), then calling Edge's
/// native PrintToPdfAsync. This avoids adding any third-party PDF library — the PDF is a
/// faithful, pixel-accurate render of the same report the investigator already reviewed.
/// </summary>
public sealed class PdfExportService : IDisposable
{
    readonly WebView2 _hidden;
    bool _initialized;

    /// <param name="owner">Form that will host the hidden render surface. WebView2 needs a
    /// real native window handle to initialize, so the control is parented off-screen rather
    /// than left detached.</param>
    public PdfExportService(Form owner)
    {
        _hidden = new WebView2 { Width = 2, Height = 2, Visible = false, Location = new System.Drawing.Point(-2000, -2000) };
        owner.Controls.Add(_hidden);
    }

    async Task EnsureReadyAsync(string userDataFolder)
    {
        if (_initialized) return;
        var env = await CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);
        await _hidden.EnsureCoreWebView2Async(env);
        _initialized = true;
    }

    public async Task ExportAsync(InvestigationReport report, string outputPath, string userDataFolder)
    {
        await EnsureReadyAsync(userDataFolder);

        var html = HtmlExporter.Render(report);
        var tcs = new TaskCompletionSource();

        void OnLoaded(object? s, CoreWebView2NavigationCompletedEventArgs e)
        {
            _hidden.CoreWebView2.NavigationCompleted -= OnLoaded;
            if (e.IsSuccess) tcs.TrySetResult();
            else tcs.TrySetException(new IOException("WebView2 failed to render the report for PDF export."));
        }

        _hidden.CoreWebView2.NavigationCompleted += OnLoaded;
        _hidden.CoreWebView2.NavigateToString(html);
        await tcs.Task;

        var settings = _hidden.CoreWebView2.Environment.CreatePrintSettings();
        settings.Orientation = CoreWebView2PrintOrientation.Portrait;
        settings.ShouldPrintBackgrounds = true;
        settings.ShouldPrintHeaderAndFooter = false;
        settings.MarginTop = 0.3; settings.MarginBottom = 0.3;
        settings.MarginLeft = 0.3; settings.MarginRight = 0.3;

        var ok = await _hidden.CoreWebView2.PrintToPdfAsync(outputPath, settings);
        if (!ok) throw new IOException("PrintToPdfAsync reported failure.");
    }

    public void Dispose() => _hidden.Dispose();
}
