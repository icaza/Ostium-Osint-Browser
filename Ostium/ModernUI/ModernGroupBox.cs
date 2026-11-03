// ---------------------------------------------------------------------------
//  Ostium - ModernGroupBox
//  Dark GroupBox: features a thin, rounded border with a legend that interrupts the line.
//  Replaces System.Windows.Forms.GroupBox in Main_Frm.Designer.cs.
// ---------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ostium.ModernUI
{
    public class ModernGroupBox : GroupBox
    {
        private const TextFormatFlags TextFlags =
            TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.Left;

        public ModernGroupBox()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (SolidBrush back = new SolidBrush(BackColor))
            {
                g.FillRectangle(back, ClientRectangle);
            }

            Size textSize = Size.Empty;
            if (!string.IsNullOrEmpty(Text))
            {
                textSize = TextRenderer.MeasureText(g, Text, Font, new Size(int.MaxValue, int.MaxValue), TextFlags);
            }

            int top = textSize.Height / 2;
            Rectangle box = new Rectangle(0, top, Math.Max(1, Width - 1), Math.Max(1, Height - top - 1));

            SmoothingMode previous = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = RoundedRectangle(box, 6))
            using (Pen pen = new Pen(ModernPalette.Border))
            {
                g.DrawPath(pen, path);
            }
            g.SmoothingMode = previous;

            if (textSize.Width > 0)
            {
                // The legend "cuts" into the border: the background behind the text is repainted.
                Rectangle gap = new Rectangle(8, 0, textSize.Width + 8, textSize.Height);
                using (SolidBrush back = new SolidBrush(BackColor))
                {
                    g.FillRectangle(back, gap);
                }
                TextRenderer.DrawText(
                    g, Text, Font, new Point(12, 0),
                    Enabled ? ForeColor : ModernPalette.Disabled, TextFlags);
            }
        }

        private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            int d = Math.Max(1, Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height)));
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
