namespace OsintWatcher.App.Theme;

/// <summary>Colour tokens shared with the HTML export (see HtmlExporter.Css) so the native
/// shell and the report dashboard read as one product rather than two mismatched UIs.</summary>
public static class DarkTheme
{
    public static readonly Color Ink = ColorTranslator.FromHtml("#12161C");
    public static readonly Color Panel = ColorTranslator.FromHtml("#1A2029");
    public static readonly Color PanelAlt = ColorTranslator.FromHtml("#212836");
    public static readonly Color Hairline = ColorTranslator.FromHtml("#2B3542");
    public static readonly Color Text = ColorTranslator.FromHtml("#E7EAEE");
    public static readonly Color TextMuted = ColorTranslator.FromHtml("#8A96A3");
    public static readonly Color Amber = ColorTranslator.FromHtml("#E8A33D");
    public static readonly Color Teal = ColorTranslator.FromHtml("#4FB6A8");
    public static readonly Color Brick = ColorTranslator.FromHtml("#D96257");
    public static readonly Color Baseline = ColorTranslator.FromHtml("#6B7A99");

    public static readonly Font BodyFont = new("Verdana", 9.5f);
    public static readonly Font MonoFont = new("Consolas", 9.5f);
    public static readonly Font TitleFont = new("Georgia", 15f, FontStyle.Bold);

    public static void Apply(Form form)
    {
        form.BackColor = Ink;
        form.ForeColor = Text;
        form.Font = BodyFont;
    }

    public static void StyleButton(Button b, bool primary = false)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderColor = primary ? Amber : Hairline;
        b.FlatAppearance.BorderSize = 1;
        b.BackColor = primary ? PanelAlt : Panel;
        b.ForeColor = primary ? Amber : Text;
        b.Font = BodyFont;
        b.Cursor = Cursors.Hand;
        b.Padding = new Padding(10, 4, 10, 4);
        b.FlatAppearance.MouseOverBackColor = PanelAlt;
    }

    public static void StyleGrid(DataGridView g)
    {
        g.EnableHeadersVisualStyles = false;
        g.BackgroundColor = Ink;
        g.GridColor = Hairline;
        g.BorderStyle = BorderStyle.None;
        g.RowHeadersVisible = false;
        g.AllowUserToAddRows = false;
        g.AllowUserToDeleteRows = false;
        g.AllowUserToResizeRows = false;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.MultiSelect = true;
        g.ColumnHeadersDefaultCellStyle.BackColor = Panel;
        g.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted;
        g.ColumnHeadersDefaultCellStyle.Font = new Font(BodyFont, FontStyle.Bold);
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Panel;
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        g.DefaultCellStyle.BackColor = Panel;
        g.DefaultCellStyle.ForeColor = Text;
        g.DefaultCellStyle.SelectionBackColor = PanelAlt;
        g.DefaultCellStyle.SelectionForeColor = Amber;
        g.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#181D25");
        g.RowTemplate.Height = 28;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
    }

    public static void StyleTextBox(TextBox t)
    {
        t.BackColor = PanelAlt;
        t.ForeColor = Text;
        t.BorderStyle = BorderStyle.FixedSingle;
    }

    public static void StyleComboBox(ComboBox c)
    {
        c.BackColor = PanelAlt;
        c.ForeColor = Text;
        c.FlatStyle = FlatStyle.Flat;
        c.DropDownStyle = ComboBoxStyle.DropDownList;
    }

    public static Color VerdictColor(string verdict) => verdict switch
    {
        "Modified" => Amber,
        "Unchanged" => Teal,
        "Unreachable" => Brick,
        _ => Baseline
    };
}
