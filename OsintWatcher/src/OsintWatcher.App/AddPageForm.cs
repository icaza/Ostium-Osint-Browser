using OsintWatcher.App.Theme;
using OsintWatcher.Core.Models;

namespace OsintWatcher.App;

/// <summary>Modal dialog for adding or editing one MonitoredPage.</summary>
public sealed class AddPageForm : Form
{
    readonly TextBox _urlBox = new();
    readonly TextBox _labelBox = new();
    readonly TextBox _caseBox = new();
    readonly ComboBox _modeBox = new();
    readonly NumericUpDown _intervalBox = new();
    readonly NumericUpDown _jitterBox = new();
    readonly CheckBox _allowPrivateBox = new();

    public MonitoredPage? Result { get; private set; }

    public AddPageForm(MonitoredPage? existing = null)
    {
        Text = existing is null ? "Add monitored page" : "Edit monitored page";
        Width = 460;
        Height = 440;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        DarkTheme.Apply(this);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(16),
            AutoSize = true
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        DarkTheme.StyleTextBox(_urlBox);
        DarkTheme.StyleTextBox(_labelBox);
        DarkTheme.StyleTextBox(_caseBox);
        DarkTheme.StyleComboBox(_modeBox);
        _modeBox.Items.AddRange(new object[] { "Manual", "Randomized interval" });
        _modeBox.SelectedIndex = 0;
        _intervalBox.Minimum = 1; _intervalBox.Maximum = 10080; _intervalBox.Value = 60;
        _jitterBox.Minimum = 0; _jitterBox.Maximum = 90; _jitterBox.Value = 30;
        _allowPrivateBox.Text = "Allow private / internal targets (disables SSRF guard)";
        _allowPrivateBox.ForeColor = DarkTheme.Brick;
        _allowPrivateBox.AutoSize = true;

        AddRow(layout, "URL", _urlBox);
        AddRow(layout, "Label", _labelBox);
        AddRow(layout, "Case reference", _caseBox);
        AddRow(layout, "Mode", _modeBox);
        AddRow(layout, "Interval (min)", _intervalBox);
        AddRow(layout, "Jitter (%)", _jitterBox);
        layout.Controls.Add(_allowPrivateBox, 1, layout.RowCount);
        layout.RowCount++;

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(16), Height = 52 };
        var ok = new Button { Text = existing is null ? "Add" : "Save", DialogResult = DialogResult.OK, Width = 90 };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
        DarkTheme.StyleButton(ok, primary: true);
        DarkTheme.StyleButton(cancel);
        buttonPanel.Controls.Add(ok);
        buttonPanel.Controls.Add(cancel);
        AcceptButton = ok;
        CancelButton = cancel;

        Controls.Add(layout);
        Controls.Add(buttonPanel);

        if (existing is not null)
        {
            _urlBox.Text = existing.Url;
            _labelBox.Text = existing.Label;
            _caseBox.Text = existing.CaseReference;
            _modeBox.SelectedIndex = existing.Mode == MonitoringMode.Manual ? 0 : 1;
            _intervalBox.Value = Math.Clamp(existing.IntervalMinutes, 1, 10080);
            _jitterBox.Value = Math.Clamp(existing.JitterPercent, 0, 90);
            _allowPrivateBox.Checked = existing.AllowPrivateTargetsFlag;
        }

        ok.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_urlBox.Text))
            {
                MessageBox.Show(this, "A URL is required.", "Missing URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            Result = existing ?? new MonitoredPage { Url = _urlBox.Text.Trim() };
            Result.Url = _urlBox.Text.Trim();
            Result.Label = _labelBox.Text.Trim();
            Result.CaseReference = _caseBox.Text.Trim();
            Result.Mode = _modeBox.SelectedIndex == 0 ? MonitoringMode.Manual : MonitoringMode.RandomizedInterval;
            Result.IntervalMinutes = (int)_intervalBox.Value;
            Result.JitterPercent = (int)_jitterBox.Value;
            Result.AllowPrivateTargetsFlag = _allowPrivateBox.Checked;
        };
    }

    static void AddRow(TableLayoutPanel layout, string label, Control input)
    {
        var lbl = new Label { Text = label, ForeColor = DarkTheme.TextMuted, AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 6, 0, 0) };
        input.Dock = DockStyle.Fill;
        layout.Controls.Add(lbl, 0, layout.RowCount);
        layout.Controls.Add(input, 1, layout.RowCount);
        layout.RowCount++;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
    }
}
