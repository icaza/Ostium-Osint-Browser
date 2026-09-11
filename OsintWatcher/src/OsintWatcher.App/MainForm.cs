using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using OsintWatcher.App.Theme;
using OsintWatcher.Core.Export;
using OsintWatcher.Core.Models;
using OsintWatcher.Core.Services;
using OsintWatcher.Core.Storage;

namespace OsintWatcher.App;

public sealed class MainForm : Form
{
    #region "Var_"
    readonly string _workspaceRoot;
    readonly EncryptedStore _store;
    readonly SecureHttpFetcher _fetcher;
    readonly MonitoringEngine _engine;
    readonly List<MonitoredPage> _pages = [];
    readonly DataGridView _grid = new();
    readonly WebView2 _dashboard = new();
    readonly Label _statusLabel = new();
    readonly System.Windows.Forms.Timer _scheduler = new();
    readonly List<IReportExporter> _exporters =
    [
        new HtmlExporter(), new JsonExporter(), new CsvExporter(), new MarkdownExporter()
    ];
    bool _busy;
    #endregion

    public MainForm()
    {
        _workspaceRoot = Path.Combine(Application.StartupPath, "OsintWatcher");
        _store = new EncryptedStore(_workspaceRoot);
        _fetcher = new SecureHttpFetcher();
        _engine = new MonitoringEngine(_fetcher, _store);

        Text = "OSTIUM OSINT Watcher — Web page integrity monitor";
        Width = 1180;
        Height = 720;
        MinimumSize = new Size(900, 560);
        StartPosition = FormStartPosition.CenterScreen;
        DarkTheme.Apply(this);

        BuildLayout();
        WireEvents();

        _pages.AddRange(_store.LoadPages());
        RefreshGrid();
        _ = RefreshDashboardAsync(Get_dashboard());

        _scheduler.Interval = 30_000;
        _scheduler.Tick += async (_, _) => await RunDuePagesAsync();
        _scheduler.Start();
    }

    // ---------------------------------------------------------------- layout

    Button _btnAdd = null!, _btnEdit = null!, _btnRemove = null!, _btnRunSelected = null!, _btnRunAll = null!, _btnVerify = null!;
    ComboBox _exportFormatBox = null!;
    Button _btnExport = null!;

    void BuildLayout()
    {
        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 48,
            BackColor = DarkTheme.Panel,
            Padding = new Padding(3, 6, 0, 5),
            FlowDirection = FlowDirection.LeftToRight
        };

        _btnAdd = MakeButton("Add page", true);
        _btnEdit = MakeButton("Edit");
        _btnRemove = MakeButton("Remove");
        _btnRunSelected = MakeButton("Run selected", true);
        _btnRunAll = MakeButton("Run all", true);
        _btnVerify = MakeButton("Verify integrity");

        _exportFormatBox = new ComboBox { Width = 190, Margin = new Padding(16, 7, 14, 3) };
        DarkTheme.StyleComboBox(_exportFormatBox);
        foreach (var exp in _exporters) _exportFormatBox.Items.Add(exp.DisplayName);
        _exportFormatBox.Items.Add("PDF (dashboard render)");
        _exportFormatBox.SelectedIndex = 0;
        _btnExport = MakeButton("Export…");

        toolbar.Controls.AddRange(
        [
            _btnAdd, _btnEdit, _btnRemove, _btnRunSelected, _btnRunAll, _btnVerify,
            _exportFormatBox, _btnExport
        ]);

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 90,
            BackColor = DarkTheme.Hairline
        };

        DarkTheme.StyleGrid(_grid);
        _grid.Dock = DockStyle.Fill;
        _grid.Columns.Add("Label", "Label");
        _grid.Columns.Add("Url", "URL");
        _grid.Columns.Add("Mode", "Mode");
        _grid.Columns.Add("Verdict", "Verdict");
        _grid.Columns.Add("LastChecked", "Last checked (UTC)");
        _grid.Columns[0].FillWeight = 18;
        _grid.Columns[1].FillWeight = 34;
        _grid.Columns[2].FillWeight = 16;
        _grid.Columns[3].FillWeight = 14;
        _grid.Columns[4].FillWeight = 22;
        foreach (DataGridViewColumn c in _grid.Columns) { c.ReadOnly = true; c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }

        split.Panel1.Controls.Add(_grid);
        split.Panel2.Controls.Add(_dashboard);
        _dashboard.Dock = DockStyle.Fill;

        var statusBar = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = DarkTheme.Panel };
        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.ForeColor = DarkTheme.TextMuted;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        _statusLabel.Padding = new Padding(10, 0, 0, 0);
        _statusLabel.Text = "Ready.";
        statusBar.Controls.Add(_statusLabel);

        Controls.Add(split);
        Controls.Add(statusBar);
        Controls.Add(toolbar);
    }

    static Button MakeButton(string text, bool primary = false)
    {
        var b = new Button { Text = text, AutoSize = true, Margin = new Padding(0, 0, 5, 0), Height = 10 };
        DarkTheme.StyleButton(b, primary);
        return b;
    }

    // ---------------------------------------------------------------- events

    void WireEvents()
    {
        _btnAdd.Click += (_, _) => OnAddPage();
        _btnEdit.Click += (_, _) => OnEditPage();
        _btnRemove.Click += (_, _) => OnRemovePage();
        _btnRunSelected.Click += async (_, _) => await OnRunAsync(SelectedPages());
        _btnRunAll.Click += async (_, _) => await OnRunAsync([.. _pages.Where(p => p.Enabled)]);
        _btnVerify.Click += (_, _) => OnVerifyIntegrity();
        _btnExport.Click += async (_, _) => await OnExportAsync();
        FormClosing += (_, _) => { _store.SavePages(_pages); _fetcher.Dispose(); };
    }

    List<MonitoredPage> SelectedPages()
    {
        var ids = _grid.SelectedRows.Cast<DataGridViewRow>()
            .Select(r => (Guid)r.Tag!).ToHashSet();
        return [.. _pages.Where(p => ids.Contains(p.Id))];
    }

    void OnAddPage()
    {
        using var dlg = new AddPageForm();
        if (dlg.ShowDialog(this) != DialogResult.OK || dlg.Result is null) return;
        _pages.Add(dlg.Result);
        _store.SavePages(_pages);
        RefreshGrid();
    }

    void OnEditPage()
    {
        var selected = SelectedPages();
        if (selected.Count != 1) { SetStatus("Select exactly one page to edit."); return; }
        using var dlg = new AddPageForm(selected[0]);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        _store.SavePages(_pages);
        RefreshGrid();
    }

    void OnRemovePage()
    {
        var selected = SelectedPages();
        if (selected.Count == 0) return;
        var confirm = MessageBox.Show(this,
            $"Remove {selected.Count} page(s) and their entire local history? This cannot be undone.",
            "Confirm removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        foreach (var p in selected)
        {
            _pages.Remove(p);
            _store.DeleteHistory(p.Id);
        }
        _store.SavePages(_pages);
        RefreshGrid();
        _ = RefreshDashboardAsync(Get_dashboard());
    }

    void OnVerifyIntegrity()
    {
        var selected = SelectedPages();
        if (selected.Count == 0) { SetStatus("Select one or more pages to verify."); return; }

        var lines = new List<string>();
        foreach (var p in selected)
        {
            var ok = _store.VerifyChainIntegrity(p.Id, out var brokenAt);
            lines.Add(ok
                ? $"OK — {p.Label} ({p.Url}): evidence chain intact."
                : $"TAMPERED — {p.Label} ({p.Url}): chain breaks at entry #{brokenAt}.");
        }
        MessageBox.Show(this, string.Join(Environment.NewLine, lines), "Evidence chain integrity",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    async Task OnRunAsync(List<MonitoredPage> targets)
    {
        if (_busy || targets.Count == 0) return;
        _busy = true;
        SetBusyUi(true);
        try
        {
            int i = 0;
            foreach (var page in targets)
            {
                i++;
                SetStatus($"Checking {i}/{targets.Count}: {page.Url}");
                var outcome = await _engine.RunCheckAsync(page);
                UpdateGridRow(outcome.Page, outcome.Result);
            }
            _store.SavePages(_pages);
            await RefreshDashboardAsync(Get_dashboard());
            SetStatus($"Done — checked {targets.Count} page(s) at {DateTimeOffset.Now:HH:mm:ss}.");
        }
        finally
        {
            _busy = false;
            SetBusyUi(false);
        }
    }

    async Task RunDuePagesAsync()
    {
        if (_busy) return;
        var due = _pages.Where(p => p.Enabled && p.Mode == MonitoringMode.RandomizedInterval &&
                                     (p.NextDueUtc is null || p.NextDueUtc <= DateTimeOffset.UtcNow)).ToList();
        if (due.Count == 0) return;
        await OnRunAsync(due);
    }

    async Task OnExportAsync()
    {
        var index = _exportFormatBox.SelectedIndex;
        var report = ReportBuilder.Build(_store, _pages);

        if (index == _exporters.Count) // PDF is the last, synthetic entry
        {
            using var sfd = new SaveFileDialog { Filter = "PDF file|*.pdf", FileName = "osint-watcher-report.pdf" };
            if (sfd.ShowDialog(this) != DialogResult.OK) return;
            using var pdf = new PdfExportService(this);
            SetStatus("Rendering PDF…");
            try
            {
                await pdf.ExportAsync(report, sfd.FileName, Path.Combine(_workspaceRoot, "webview2-pdf"));
                SetStatus($"Exported PDF to {sfd.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"PDF export failed: {ex.Message}", "Export error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }

        var exporter = _exporters[index];
        using var save = new SaveFileDialog
        {
            Filter = $"{exporter.DisplayName}|*.{exporter.FileExtension}",
            FileName = $"osint-watcher-report.{exporter.FileExtension}"
        };
        if (save.ShowDialog(this) != DialogResult.OK) return;
        File.WriteAllBytes(save.FileName, exporter.Export(report));
        SetStatus($"Exported {exporter.DisplayName} to {save.FileName}");
    }

    // ---------------------------------------------------------------- rendering

    void RefreshGrid()
    {
        _grid.Rows.Clear();
        foreach (var page in _pages)
        {
            var latest = _store.GetLatest(page.Id);
            AddOrUpdateRow(page, latest);
        }
    }

    void UpdateGridRow(MonitoredPage page, ScanResult result) => AddOrUpdateRow(page, result);

    void AddOrUpdateRow(MonitoredPage page, ScanResult? latest)
    {
        var existingRow = _grid.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => (Guid)r.Tag! == page.Id);
        var verdict = latest?.Verdict.ToString() ?? "Not checked";
        var mode = page.Mode == MonitoringMode.Manual ? "Manual" : $"Auto (~{page.IntervalMinutes}m)";
        var lastChecked = page.LastCheckedUtc?.ToString("yyyy-MM-dd HH:mm") ?? "—";

        int rowIndex;
        if (existingRow is null)
            rowIndex = _grid.Rows.Add(page.Label.Length > 0 ? page.Label : "(untitled)", page.Url, mode, verdict, lastChecked);
        else
        {
            rowIndex = existingRow.Index;
            existingRow.Cells[0].Value = page.Label.Length > 0 ? page.Label : "(untitled)";
            existingRow.Cells[1].Value = page.Url;
            existingRow.Cells[2].Value = mode;
            existingRow.Cells[3].Value = verdict;
            existingRow.Cells[4].Value = lastChecked;
        }

        var row = _grid.Rows[rowIndex];
        row.Tag = page.Id;
        row.Cells[3].Style.ForeColor = DarkTheme.VerdictColor(verdict);
        row.Cells[3].Style.Font = new Font(DarkTheme.BodyFont, FontStyle.Bold);
    }

    WebView2 Get_dashboard()
    {
        return _dashboard;
    }

    async Task RefreshDashboardAsync(WebView2 _dashboard1)
    {
        var report = ReportBuilder.Build(_store, _pages);
        var html = HtmlExporter.Render(report);

        if (_dashboard.CoreWebView2 is null)
        {
            var env = await CoreWebView2Environment.CreateAsync(userDataFolder: Path.Combine(_workspaceRoot, "webview2-ui"));
            await _dashboard.EnsureCoreWebView2Async(env);
        }

        _dashboard1.CoreWebView2.NavigateToString(html);
    }

    void SetStatus(string text) => _statusLabel.Text = text;

    void SetBusyUi(bool busy)
    {
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        foreach (var b in new[] { _btnAdd, _btnEdit, _btnRemove, _btnRunSelected, _btnRunAll, _btnVerify, _btnExport })
            b.Enabled = !busy;
    }
}
