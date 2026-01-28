using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ostium
{
    public partial class PromptSend_Frm : Form
    {
        #region Var_
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        readonly string FilePromptSave = Path.Combine(Application.StartupPath, "OOBai", "SendPrompt.txt");
        public string PromptSendFrm => PromptSendForm_Txt.Text.Trim();
        public string LocalCloud;

        readonly HashSet<string> itemsPrompt = new HashSet<string>();
        #endregion

        public PromptSend_Frm()
        {
            InitializeComponent();

            Local_Chk.Click += new EventHandler(Local_Chk_Click);
            Cloud_Chk.Click += new EventHandler(Cloud_Chk_Click);
        }

        void PromptSend_Frm_Load(object sender, EventArgs e)
        {
            LoadPrompt();
            MouseDown += new MouseEventHandler(Main_Frm_MouseDown);
        }

        void Main_Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        void LoadPrompt()
        {
            Prompt_Lst.Items.Clear();

            if (File.Exists(FilePromptSave))
            {
                Prompt_Lst.Items.AddRange(File.ReadAllLines(FilePromptSave));
            }

            foreach (var item in Prompt_Lst.Items)
            {
                itemsPrompt.Add(item.ToString());
            }
        }

        void Ok_Btn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("You must meet all the requirements!", "Requirements", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        void SavePrompt_Btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PromptSendForm_Txt.Text))
            {
                MessageBox.Show("You must write a prompt before saving!", "No prompt", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!itemsPrompt.Contains(PromptSendForm_Txt.Text))
            {
                using (StreamWriter fw = File.AppendText(FilePromptSave))
                {
                    fw.WriteLine(PromptSendForm_Txt.Text);
                }
            }
            else
            {
                MessageBox.Show("The prompt already exists!", "Prompt exist", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            LoadPrompt();
        }

        void Clear_Btn_Click(object sender, EventArgs e)
        {
            PromptSendForm_Txt.Text = string.Empty;
        }

        void Cancel_Btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void RemovePromptList_Btn_Click(object sender, EventArgs e)
        {
            if (Prompt_Lst.SelectedIndex != -1)
            {
                string message = "Are you sure you want to delete this prompt?";
                string caption = "Delete prompt";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Prompt_Lst.Items.Remove(Prompt_Lst.SelectedItem);

                    using (StreamWriter SW = new StreamWriter(FilePromptSave, false))
                    {
                        foreach (string itm in Prompt_Lst.Items)
                            SW.WriteLine(itm);
                    }

                    PromptSendForm_Txt.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("First, select a prompt to delete it!", "No prompt", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        bool ValidateInputs()
        {
            if (!Local_Chk.Checked && !Cloud_Chk.Checked)
            {
                return false;
            }

            if (string.IsNullOrEmpty(PromptSendForm_Txt.Text))
            {
                return false;
            }

            return true;
        }

        void Local_Chk_Click(object sender, EventArgs e)
        {
            if (Local_Chk.Checked)
            {
                LocalCloud = "local";
                Cloud_Chk.Checked = false;
            }
        }

        void Cloud_Chk_Click(object sender, EventArgs e)
        {
            if (Cloud_Chk.Checked)
            {
                LocalCloud = "cloud";
                Local_Chk.Checked = false;
            }
        }

        void Prompt_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Prompt_Lst.SelectedIndex != -1)
                PromptSendForm_Txt.Text = Prompt_Lst.SelectedItem.ToString();
        }
    }
}
