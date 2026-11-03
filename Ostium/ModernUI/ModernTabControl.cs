// ---------------------------------------------------------------------------
//  Ostium - ModernTabControl
//  A fully custom-drawn TabControl (dark background, highlighted active tab, hover effects).
//  Replaces System.Windows.Forms.TabControl in Main_Frm.Designer.cs:
//  all standard properties and events remain available.
// ---------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ostium.ModernUI
{
    public class ModernTabControl : TabControl
    {
        private int _hoverIndex = -1;

        public ModernTabControl()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);

            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(112, 34);
            Padding = new Point(14, 6);
            DoubleBuffered = true;
        }

        /// <summary>Height of the strip containing the tabs.</summary>
        private int StripHeight
        {
            get { return ItemSize.Height + 3; }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Everything is drawn within OnPaint (this avoids flickering).
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // General background (identical to the pages so that the 4 px frame around them is invisible)
            using (SolidBrush body = new SolidBrush(ModernPalette.Bg))
            {
                g.FillRectangle(body, ClientRectangle);
            }

            // Tab bar
            Rectangle strip = new Rectangle(0, 0, Width, StripHeight);
            using (SolidBrush stripBrush = new SolidBrush(ModernPalette.Sidebar))
            {
                g.FillRectangle(stripBrush, strip);
            }
            using (Pen line = new Pen(ModernPalette.Border))
            {
                g.DrawLine(line, 0, strip.Bottom - 1, Width, strip.Bottom - 1);
            }

            for (int i = 0; i < TabCount; i++)
            {
                DrawTab(g, i);
            }
        }

        private void DrawTab(Graphics g, int index)
        {
            Rectangle rect = GetTabRect(index);
            bool selected = index == SelectedIndex;
            bool hover = index == _hoverIndex && !selected;

            Rectangle tabRect = new Rectangle(rect.X, 0, rect.Width, StripHeight);

            if (selected)
            {
                using (SolidBrush back = new SolidBrush(ModernPalette.Bg))
                {
                    g.FillRectangle(back, tabRect);
                }
                using (SolidBrush accent = new SolidBrush(ModernPalette.Accent))
                {
                    g.FillRectangle(accent, tabRect.X, 0, tabRect.Width, 2);
                }
            }
            else if (hover)
            {
                using (SolidBrush back = new SolidBrush(ModernPalette.Surface))
                {
                    g.FillRectangle(back, tabRect);
                }
            }

            TabPage page = TabPages[index];
            Color textColor = selected || hover ? ModernPalette.Text : ModernPalette.Muted;
            Rectangle textRect = new Rectangle(tabRect.X + 6, tabRect.Y + 2, tabRect.Width - 12, tabRect.Height - 3);
            TextRenderer.DrawText(
                g, page.Text, Font, textRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int index = -1;
            for (int i = 0; i < TabCount; i++)
            {
                if (GetTabRect(i).Contains(e.Location))
                {
                    index = i;
                    break;
                }
            }

            if (index != _hoverIndex)
            {
                _hoverIndex = index;
                Invalidate(new Rectangle(0, 0, Width, StripHeight));
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoverIndex != -1)
            {
                _hoverIndex = -1;
                Invalidate(new Rectangle(0, 0, Width, StripHeight));
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Invalidate(new Rectangle(0, 0, Width, StripHeight));
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Invalidate();
        }
    }
}
