// ---------------------------------------------------------------------------
//  Ostium - ModernTabControl
//  TabControl dessine entierement (fond sombre, onglet actif surligne, survol).
//  Remplace System.Windows.Forms.TabControl dans Main_Frm.Designer.cs :
//  toutes les proprietes / evenements habituels restent disponibles.
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

        /// <summary>Hauteur de la bande contenant les onglets.</summary>
        private int StripHeight
        {
            get { return ItemSize.Height + 3; }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Tout est dessine dans OnPaint (evite le scintillement).
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Fond general (identique aux pages pour que le cadre de 4 px autour d'elles soit invisible)
            using (SolidBrush body = new SolidBrush(ModernPalette.Bg))
            {
                g.FillRectangle(body, ClientRectangle);
            }

            // Bande des onglets
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
