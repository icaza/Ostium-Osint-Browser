using System;
using System.Windows.Forms;

namespace Ostium
{
    public partial class TemplateEditorForm : Form
    {
        #region Var_
        public string TemplateName => txtName.Text.Trim();
        public string TemplateContent => txtContent.Text.Trim();
        #endregion

        public TemplateEditorForm()
        {
            InitializeComponent();
        }

        void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a name for the template.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void IconTextSelect_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtName.Text = $"{IconTextSelect_Cbx.SelectedItem} {txtName.Text}";
        }
    }
}
