// ---------------------------------------------------------------------------
//  Ostium - Main_Frm.Theme.cs
//  Partie complementaire de Main_Frm : branchement du theme moderne.
//  Aucun code existant n'est modifie : ces methodes sont appelees uniquement
//  par les evenements declares dans Main_Frm.Designer.cs.
// ---------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using Ostium.ModernUI;

namespace Ostium
{
    partial class Main_Frm
    {
        /// <summary>Applique le theme (barre de titre sombre, ListBox, barres de defilement...).</summary>
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
