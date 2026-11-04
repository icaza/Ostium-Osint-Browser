// ---------------------------------------------------------------------------
//  Ostium - Main_Frm.Theme.cs
//  Supplementary part of Main_Frm: integration of the modern theme.
//  No existing code is modified; these methods are called exclusively
//  by the events declared in Main_Frm.Designer.cs.
// ---------------------------------------------------------------------------
using Ostium.ModernUI;
using System;
using System.Windows.Forms;

namespace Ostium
{
    partial class Main_Frm
    {
        /// <summary>Applies the theme (dark title bar, ListBox, scroll bars, etc.).</summary>
        private void ModernTheme_HandleCreated(object sender, EventArgs e)
        {
            ModernTheme.Apply(this);
        }

        private void ModernToolTip_Popup(object sender, PopupEventArgs e)
        {
            ModernTheme.ToolTipPopup((ToolTip)sender, e);
        }

        private void ModernToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            ModernTheme.ToolTipDraw(e);
        }
    }
}
