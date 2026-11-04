// ---------------------------------------------------------------------------
//  Ostium - Modern dark theme
//  Supplementary file for Main_Frm.Designer.cs (C# 7.3 / .NET Framework 4.8.1)
//
//  Contents:
//    - ModernPalette            : shared color palette
//    - ModernTheme              : runtime theme application (dark title bar,
//                                 dark scrollbars, modern ListBoxes, tooltips)
//    - ModernToolStripRenderer  : modern rendering (flat, rounded corners) for ToolStrips / menus
//    - ModernColorTable         : associated color table
// ---------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ostium.ModernUI
{
    /// <summary>Palette sombre (inspiree des interfaces Windows 11 / Fluent).</summary>
    internal static class ModernPalette
    {
        public static readonly Color Bg = Color.FromArgb(28, 30, 35);
        public static readonly Color Surface = Color.FromArgb(36, 39, 46);
        public static readonly Color Sidebar = Color.FromArgb(23, 25, 30);
        public static readonly Color Input = Color.FromArgb(20, 22, 26);
        public static readonly Color Popup = Color.FromArgb(32, 35, 42);
        public static readonly Color Button = Color.FromArgb(48, 52, 61);
        public static readonly Color ButtonHover = Color.FromArgb(64, 69, 81);
        public static readonly Color Border = Color.FromArgb(58, 62, 72);
        public static readonly Color Text = Color.FromArgb(232, 234, 238);
        public static readonly Color Muted = Color.FromArgb(148, 155, 168);
        public static readonly Color Disabled = Color.FromArgb(105, 111, 124);
        public static readonly Color Accent = Color.FromArgb(59, 130, 246);
        public static readonly Color AccentHover = Color.FromArgb(96, 165, 250);
        public static readonly Color AccentPressed = Color.FromArgb(37, 99, 235);
        public static readonly Color AccentSoft = Color.FromArgb(34, 52, 88);
        public static readonly Color ToolTipBack = Color.FromArgb(44, 48, 58);

        public static Color Blend(Color from, Color to, double amount)
        {
            if (amount < 0) amount = 0;
            if (amount > 1) amount = 1;
            return Color.FromArgb(
                (int)(from.R + (to.R - from.R) * amount),
                (int)(from.G + (to.G - from.G) * amount),
                (int)(from.B + (to.B - from.B) * amount));
        }
    }

    /// <summary>Apply the modern theme to a window and all its controls.</summary>
    public static class ModernTheme
    {
        private static readonly object Marker = new object();
        private static readonly ConditionalWeakTable<Control, object> Themed = new ConditionalWeakTable<Control, object>();
        private static bool _rendererInstalled;
        private static Font _toolTipFont;

        // -------------------------------------------------------------------
        //  Entry point
        // -------------------------------------------------------------------

        /// <summary>
        /// To be called only once (e.g., from the window's HandleCreated event). 
        /// Safe to call multiple times.
        /// </summary>
        public static void Apply(Form form)
        {
            if (form == null) return;

            InstallRenderer();
            ApplyTree(form);
        }

        private static void InstallRenderer()
        {
            if (_rendererInstalled) return;
            _rendererInstalled = true;
            ToolStripManager.Renderer = new ModernToolStripRenderer();
        }

        private static void ApplyTree(Control root)
        {
            Themify(root);
            foreach (Control child in root.Controls)
            {
                ApplyTree(child);
            }
        }

        private static void Themify(Control c)
        {
            object existing;
            if (Themed.TryGetValue(c, out existing)) return;
            Themed.Add(c, Marker);

            c.ControlAdded += Control_ControlAdded;
            c.HandleCreated += Control_HandleCreated;

            ListBox listBox = c as ListBox;
            if (listBox != null) StyleListBox(listBox);

            ToolStrip toolStrip = c as ToolStrip;
            if (toolStrip != null) ThemifyItems(toolStrip.Items);

            if (c.IsHandleCreated) ApplyNative(c);
        }

        /// <summary>
        /// Drop-down menus (ToolStripDropDown) are not children of the form:
        /// here, we assign them a dark background and text color, and apply a theme to the
        /// hosted controls (ToolStripComboBox, ToolStripTextBox, etc.).
        /// </summary>
        private static void ThemifyItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                ToolStripControlHost host = item as ToolStripControlHost;
                if (host != null && host.Control != null) ApplyTree(host.Control);

                ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;
                if (dropDownItem != null && dropDownItem.HasDropDownItems)
                {
                    dropDownItem.DropDown.BackColor = ModernPalette.Popup;
                    dropDownItem.DropDown.ForeColor = ModernPalette.Text;
                    ThemifyItems(dropDownItem.DropDownItems);
                }
            }
        }

        private static void Control_ControlAdded(object sender, ControlEventArgs e)
        {
            ApplyTree(e.Control);
        }

        private static void Control_HandleCreated(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if (c != null) ApplyNative(c);
        }

        // -------------------------------------------------------------------
        //  Native (Win32) settings: title bar, scroll bars
        // -------------------------------------------------------------------

        private static void ApplyNative(Control c)
        {
            try
            {
                Form form = c as Form;
                if (form != null)
                {
                    ApplyWindowChrome(form);
                    return;
                }

                ComboBox combo = c as ComboBox;
                if (combo != null)
                {
                    NativeMethods.SetWindowTheme(combo.Handle, "DarkMode_CFD", null);
                    NativeMethods.COMBOBOXINFO info = new NativeMethods.COMBOBOXINFO();
                    info.cbSize = Marshal.SizeOf(typeof(NativeMethods.COMBOBOXINFO));
                    if (NativeMethods.GetComboBoxInfo(combo.Handle, ref info) && info.hwndList != IntPtr.Zero)
                    {
                        NativeMethods.SetWindowTheme(info.hwndList, "DarkMode_Explorer", null);
                    }
                    return;
                }

                TextBoxBase textBox = c as TextBoxBase;
                if (textBox != null)
                {
                    if (textBox.Multiline) NativeMethods.SetWindowTheme(textBox.Handle, "DarkMode_Explorer", null);
                    return;
                }

                if (c is ListBox || c is TreeView || c is ListView)
                {
                    NativeMethods.SetWindowTheme(c.Handle, "DarkMode_Explorer", null);
                    return;
                }

                ScrollableControl scrollable = c as ScrollableControl;
                if (scrollable != null && (scrollable.AutoScroll || c.GetType().Name == "FastColoredTextBox"))
                {
                    NativeMethods.SetWindowTheme(c.Handle, "DarkMode_Explorer", null);
                }
            }
            catch (DllNotFoundException) { }
            catch (EntryPointNotFoundException) { }
            catch (Exception) { /* The theme is purely cosmetic: it should never block the application. */ }
        }

        private static void ApplyWindowChrome(Form form)
        {
            IntPtr handle = form.Handle;

            // Dark title bar (Windows 10 2004+: attribute 20; older builds) : 19)
            int dark = 1;
            if (NativeMethods.DwmSetWindowAttribute(handle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int)) != 0)
            {
                NativeMethods.DwmSetWindowAttribute(handle, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE_OLD, ref dark, sizeof(int));
            }

            // Windows 11 : title bar colors aligned with the interface (no effect on Windows 10)
            int caption = ToColorRef(ModernPalette.Sidebar);
            int border = ToColorRef(ModernPalette.Border);
            int text = ToColorRef(ModernPalette.Text);
            NativeMethods.DwmSetWindowAttribute(handle, NativeMethods.DWMWA_CAPTION_COLOR, ref caption, sizeof(int));
            NativeMethods.DwmSetWindowAttribute(handle, NativeMethods.DWMWA_BORDER_COLOR, ref border, sizeof(int));
            NativeMethods.DwmSetWindowAttribute(handle, NativeMethods.DWMWA_TEXT_COLOR, ref text, sizeof(int));
        }

        private static int ToColorRef(Color c)
        {
            return c.R | (c.G << 8) | (c.B << 16);
        }

        // -------------------------------------------------------------------
        //  ListBox : sleeker lines, modern selection
        // -------------------------------------------------------------------

        private static void StyleListBox(ListBox lb)
        {
            lb.IntegralHeight = false;
            lb.DrawMode = DrawMode.OwnerDrawFixed;
            lb.ItemHeight = Math.Max(24, lb.Font.Height + 8);
            lb.DrawItem += ListBox_DrawItem;
        }

        private static void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            Graphics g = e.Graphics;
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using (SolidBrush back = new SolidBrush(selected ? ModernPalette.AccentSoft : lb.BackColor))
            {
                g.FillRectangle(back, e.Bounds);
            }

            if (e.Index < 0 || e.Index >= lb.Items.Count) return;

            string text = lb.GetItemText(lb.Items[e.Index]);
            Rectangle textBounds = new Rectangle(e.Bounds.X + 10, e.Bounds.Y, Math.Max(0, e.Bounds.Width - 14), e.Bounds.Height);
            TextRenderer.DrawText(
                g, text, lb.Font, textBounds,
                selected ? ModernPalette.Text : lb.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine);

            if (selected)
            {
                using (SolidBrush bar = new SolidBrush(ModernPalette.Accent))
                {
                    g.FillRectangle(bar, e.Bounds.X, e.Bounds.Y + 3, 3, Math.Max(0, e.Bounds.Height - 6));
                }
            }
        }

        // -------------------------------------------------------------------
        //  Tooltips (ToolTip in OwnerDraw)
        // -------------------------------------------------------------------

        private static Font ToolTipFont
        {
            get
            {
                if (_toolTipFont == null) _toolTipFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
                return _toolTipFont;
            }
        }

        /// <summary>To be connected to ToolTip.Popup (ToolTip.OwnerDraw = true).</summary>
        public static void ToolTipPopup(ToolTip toolTip, PopupEventArgs e)
        {
            string text = e.AssociatedControl != null ? toolTip.GetToolTip(e.AssociatedControl) : string.Empty;
            Size size = TextRenderer.MeasureText(text ?? string.Empty, ToolTipFont);
            e.ToolTipSize = new Size(size.Width + 18, size.Height + 12);
        }

        /// <summary>To be connected to ToolTip.Draw (ToolTip.OwnerDraw = true).</summary>
        public static void ToolTipDraw(DrawToolTipEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = e.Bounds;
            using (SolidBrush back = new SolidBrush(ModernPalette.ToolTipBack))
            {
                g.FillRectangle(back, bounds);
            }
            using (Pen border = new Pen(ModernPalette.Border))
            {
                g.DrawRectangle(border, 0, 0, bounds.Width - 1, bounds.Height - 1);
            }
            TextRenderer.DrawText(
                g, e.ToolTipText, ToolTipFont, bounds, ModernPalette.Text,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine);
        }

        // -------------------------------------------------------------------
        //  Interop
        // -------------------------------------------------------------------

        private static class NativeMethods
        {
            public const int DWMWA_USE_IMMERSIVE_DARK_MODE_OLD = 19;
            public const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
            public const int DWMWA_BORDER_COLOR = 34;
            public const int DWMWA_CAPTION_COLOR = 35;
            public const int DWMWA_TEXT_COLOR = 36;

            [DllImport("dwmapi.dll")]
            public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

            [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
            public static extern int SetWindowTheme(IntPtr hwnd, string subAppName, string subIdList);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetComboBoxInfo(IntPtr hwndCombo, ref COMBOBOXINFO info);

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct COMBOBOXINFO
            {
                public int cbSize;
                public RECT rcItem;
                public RECT rcButton;
                public int stateButton;
                public IntPtr hwndCombo;
                public IntPtr hwndItem;
                public IntPtr hwndList;
            }
        }
    }

    // =======================================================================
    //  Rendering of ToolStrips / menus / status bar
    // =======================================================================

    internal sealed class ModernToolStripRenderer : ToolStripProfessionalRenderer
    {
        public ModernToolStripRenderer() : base(new ModernColorTable())
        {
            RoundedEdges = false;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            ToolStripDropDown dropDown = e.ToolStrip as ToolStripDropDown;
            if (dropDown != null && dropDown.ForeColor == Control.DefaultForeColor)
            {
                // Menu created dynamically after the theme is applied: light text on a dark background.
                dropDown.ForeColor = ModernPalette.Text;
            }

            Color color = dropDown != null ? ModernPalette.Popup : e.ToolStrip.BackColor;
            using (SolidBrush brush = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            Size size = e.ToolStrip.Size;
            using (Pen pen = new Pen(ModernPalette.Border))
            {
                if (e.ToolStrip is ToolStripDropDown)
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, size.Width - 1, size.Height - 1);
                }
                else if (e.ToolStrip is StatusStrip)
                {
                    e.Graphics.DrawLine(pen, 0, 0, size.Width, 0);
                }
                else
                {
                    e.Graphics.DrawLine(pen, 0, size.Height - 1, size.Width, size.Height - 1);
                }
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            //No separate column for icons: the menu background is uniform.
        }

        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        {
        }

        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripItem item = e.Item;
            if (!item.Enabled) return;

            Color fill = Color.Empty;
            if (item.Pressed && !item.IsOnDropDown) fill = ModernPalette.Button;
            else if (item.Selected || item.Pressed) fill = ModernPalette.ButtonHover;
            if (fill.IsEmpty) return;

            Rectangle bounds = item.IsOnDropDown
                ? new Rectangle(3, 1, item.Width - 6, item.Height - 2)
                : new Rectangle(1, 1, item.Width - 2, item.Height - 2);
            FillRounded(e.Graphics, fill, bounds, 4);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripButton button = e.Item as ToolStripButton;
            PaintToolItem(e.Graphics, e.Item, button != null && button.Checked);
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            PaintToolItem(e.Graphics, e.Item, false);
        }

        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            PaintToolItem(e.Graphics, e.Item, false);

            ToolStripSplitButton split = e.Item as ToolStripSplitButton;
            if (split == null) return;

            Rectangle drop = split.DropDownButtonBounds;
            if (split.Selected || split.Pressed)
            {
                using (Pen pen = new Pen(ModernPalette.Border))
                {
                    e.Graphics.DrawLine(pen, drop.Left, drop.Top + 4, drop.Left, drop.Bottom - 4);
                }
            }
            DrawTriangle(e.Graphics, drop, ArrowDirection.Down, split.Enabled ? ModernPalette.Muted : ModernPalette.Disabled);
        }

        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            PaintToolItem(e.Graphics, e.Item, false);
            Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
            DrawTriangle(e.Graphics, bounds, ArrowDirection.Down, ModernPalette.Muted);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            Color color = (e.Item != null && !e.Item.Enabled) ? ModernPalette.Disabled : ModernPalette.Muted;
            DrawTriangle(e.Graphics, e.ArrowRectangle, e.Direction, color);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Size size = e.Item.Size;
            using (Pen pen = new Pen(ModernPalette.Border))
            {
                if (e.Vertical)
                {
                    int x = size.Width / 2;
                    e.Graphics.DrawLine(pen, x, 4, x, Math.Max(4, size.Height - 4));
                }
                else
                {
                    int y = size.Height / 2;
                    e.Graphics.DrawLine(pen, 8, y, Math.Max(8, size.Width - 8), y);
                }
            }
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle r = e.ImageRectangle;
            r.Inflate(-1, -1);
            Graphics g = e.Graphics;
            SmoothingMode previous = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            FillRounded(g, ModernPalette.AccentSoft, r, 3);
            using (Pen pen = new Pen(ModernPalette.AccentHover, 1.8f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                Point[] check =
                {
                    new Point(r.Left + r.Width / 4, r.Top + r.Height / 2),
                    new Point(r.Left + r.Width / 2 - 1, r.Bottom - r.Height / 4 - 1),
                    new Point(r.Right - r.Width / 5, r.Top + r.Height / 4)
                };
                g.DrawLines(pen, check);
            }
            g.SmoothingMode = previous;
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item != null && !e.Item.Enabled)
            {
                // The default rendering forces a system gray that is unreadable against a dark background.
                TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, e.TextRectangle, ModernPalette.Disabled, e.TextFormat);
                return;
            }
            base.OnRenderItemText(e);
        }

        // -------------------------------------------------------------------
        //  Helpers
        // -------------------------------------------------------------------

        private static void PaintToolItem(Graphics g, ToolStripItem item, bool isChecked)
        {
            if (!item.Enabled) return;

            // Custom background (e.g., accent button): different from the bar color.
            Color custom = Color.Empty;
            if (item.Owner != null && item.BackColor != item.Owner.BackColor && !item.BackColor.IsEmpty)
            {
                custom = item.BackColor;
            }

            Color fill = Color.Empty;
            if (item.Pressed) fill = custom.IsEmpty ? ModernPalette.Button : ModernPalette.AccentPressed;
            else if (item.Selected) fill = custom.IsEmpty ? ModernPalette.ButtonHover : ModernPalette.Blend(custom, Color.White, 0.18);
            else if (isChecked) fill = ModernPalette.AccentSoft;
            else if (!custom.IsEmpty) fill = custom;
            if (fill.IsEmpty) return;

            FillRounded(g, fill, new Rectangle(1, 1, item.Width - 2, item.Height - 2), 4);
        }

        private static void DrawTriangle(Graphics g, Rectangle area, ArrowDirection direction, Color color)
        {
            int cx = area.Left + area.Width / 2;
            int cy = area.Top + area.Height / 2;
            Point[] points;
            switch (direction)
            {
                case ArrowDirection.Right:
                    points = new[] { new Point(cx - 2, cy - 4), new Point(cx - 2, cy + 4), new Point(cx + 2, cy) };
                    break;
                case ArrowDirection.Left:
                    points = new[] { new Point(cx + 2, cy - 4), new Point(cx + 2, cy + 4), new Point(cx - 2, cy) };
                    break;
                case ArrowDirection.Up:
                    points = new[] { new Point(cx - 4, cy + 2), new Point(cx + 4, cy + 2), new Point(cx, cy - 2) };
                    break;
                default:
                    points = new[] { new Point(cx - 4, cy - 2), new Point(cx + 4, cy - 2), new Point(cx, cy + 2) };
                    break;
            }

            SmoothingMode previous = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillPolygon(brush, points);
            }
            g.SmoothingMode = previous;
        }

        private static void FillRounded(Graphics g, Color color, Rectangle bounds, int radius)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            SmoothingMode previous = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int d = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
                path.AddArc(bounds.Right - d - 1, bounds.Top, d, d, 270, 90);
                path.AddArc(bounds.Right - d - 1, bounds.Bottom - d - 1, d, d, 0, 90);
                path.AddArc(bounds.Left, bounds.Bottom - d - 1, d, d, 90, 90);
                path.CloseFigure();
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillPath(brush, path);
                }
            }
            g.SmoothingMode = previous;
        }
    }

    internal sealed class ModernColorTable : ProfessionalColorTable
    {
        public ModernColorTable()
        {
            UseSystemColors = false;
        }

        public override Color ToolStripDropDownBackground { get { return ModernPalette.Popup; } }
        public override Color ToolStripBorder { get { return ModernPalette.Border; } }
        public override Color ToolStripGradientBegin { get { return ModernPalette.Surface; } }
        public override Color ToolStripGradientMiddle { get { return ModernPalette.Surface; } }
        public override Color ToolStripGradientEnd { get { return ModernPalette.Surface; } }
        public override Color ToolStripContentPanelGradientBegin { get { return ModernPalette.Bg; } }
        public override Color ToolStripContentPanelGradientEnd { get { return ModernPalette.Bg; } }
        public override Color ToolStripPanelGradientBegin { get { return ModernPalette.Surface; } }
        public override Color ToolStripPanelGradientEnd { get { return ModernPalette.Surface; } }
        public override Color MenuStripGradientBegin { get { return ModernPalette.Surface; } }
        public override Color MenuStripGradientEnd { get { return ModernPalette.Surface; } }
        public override Color StatusStripGradientBegin { get { return ModernPalette.Surface; } }
        public override Color StatusStripGradientEnd { get { return ModernPalette.Surface; } }
        public override Color RaftingContainerGradientBegin { get { return ModernPalette.Surface; } }
        public override Color RaftingContainerGradientEnd { get { return ModernPalette.Surface; } }

        public override Color MenuBorder { get { return ModernPalette.Border; } }
        public override Color MenuItemBorder { get { return ModernPalette.ButtonHover; } }
        public override Color MenuItemSelected { get { return ModernPalette.ButtonHover; } }
        public override Color MenuItemSelectedGradientBegin { get { return ModernPalette.ButtonHover; } }
        public override Color MenuItemSelectedGradientEnd { get { return ModernPalette.ButtonHover; } }
        public override Color MenuItemPressedGradientBegin { get { return ModernPalette.Button; } }
        public override Color MenuItemPressedGradientMiddle { get { return ModernPalette.Button; } }
        public override Color MenuItemPressedGradientEnd { get { return ModernPalette.Button; } }

        public override Color ImageMarginGradientBegin { get { return ModernPalette.Popup; } }
        public override Color ImageMarginGradientMiddle { get { return ModernPalette.Popup; } }
        public override Color ImageMarginGradientEnd { get { return ModernPalette.Popup; } }
        public override Color ImageMarginRevealedGradientBegin { get { return ModernPalette.Popup; } }
        public override Color ImageMarginRevealedGradientMiddle { get { return ModernPalette.Popup; } }
        public override Color ImageMarginRevealedGradientEnd { get { return ModernPalette.Popup; } }

        public override Color SeparatorDark { get { return ModernPalette.Border; } }
        public override Color SeparatorLight { get { return ModernPalette.Border; } }
        public override Color GripDark { get { return ModernPalette.Border; } }
        public override Color GripLight { get { return ModernPalette.Border; } }

        public override Color ButtonSelectedBorder { get { return ModernPalette.ButtonHover; } }
        public override Color ButtonSelectedHighlight { get { return ModernPalette.ButtonHover; } }
        public override Color ButtonSelectedHighlightBorder { get { return ModernPalette.ButtonHover; } }
        public override Color ButtonSelectedGradientBegin { get { return ModernPalette.ButtonHover; } }
        public override Color ButtonSelectedGradientMiddle { get { return ModernPalette.ButtonHover; } }
        public override Color ButtonSelectedGradientEnd { get { return ModernPalette.ButtonHover; } }
        public override Color ButtonPressedBorder { get { return ModernPalette.Button; } }
        public override Color ButtonPressedHighlight { get { return ModernPalette.Button; } }
        public override Color ButtonPressedHighlightBorder { get { return ModernPalette.Button; } }
        public override Color ButtonPressedGradientBegin { get { return ModernPalette.Button; } }
        public override Color ButtonPressedGradientMiddle { get { return ModernPalette.Button; } }
        public override Color ButtonPressedGradientEnd { get { return ModernPalette.Button; } }
        public override Color ButtonCheckedHighlight { get { return ModernPalette.AccentSoft; } }
        public override Color ButtonCheckedHighlightBorder { get { return ModernPalette.AccentSoft; } }
        public override Color ButtonCheckedGradientBegin { get { return ModernPalette.AccentSoft; } }
        public override Color ButtonCheckedGradientMiddle { get { return ModernPalette.AccentSoft; } }
        public override Color ButtonCheckedGradientEnd { get { return ModernPalette.AccentSoft; } }

        public override Color CheckBackground { get { return ModernPalette.AccentSoft; } }
        public override Color CheckPressedBackground { get { return ModernPalette.AccentSoft; } }
        public override Color CheckSelectedBackground { get { return ModernPalette.AccentSoft; } }

        public override Color OverflowButtonGradientBegin { get { return ModernPalette.Surface; } }
        public override Color OverflowButtonGradientMiddle { get { return ModernPalette.Surface; } }
        public override Color OverflowButtonGradientEnd { get { return ModernPalette.Surface; } }
    }
}
