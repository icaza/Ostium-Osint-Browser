using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Icaza;

namespace Ostium
{
    public partial class OpenSource_Frm : Form
    {
        #region Var_

        [DllImport("kernel32.dll")]
        static extern bool Beep(int freq, int duration);

        readonly IcazaClass senderror = new IcazaClass();
        readonly string AppStart = Application.StartupPath + @"\";

        #endregion

        public OpenSource_Frm()
        {
            InitializeComponent();
        }

        void OpenSource_Frm_Load(object sender, EventArgs e)
        {
            try
            {
                string strName = Path.GetFileName(Class_Var.File_Open);

                if (File.Exists(Class_Var.File_Open))
                {
                    Sortie_Lst.Items.AddRange(File.ReadAllLines(Class_Var.File_Open));
                    Text = "File open: " + strName;
                }

                Count_Lbl.Text = string.Format(" Items Count [ {0} ]", Sortie_Lst.Items.Count);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenSource_Frm_Load: ", ex.Message, "OpenSource_Frm", AppStart);
            }
        }

        void Sortie_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Sortie_Lst.SelectedIndex != -1)
                {
                    string formatUrl = Sortie_Lst.SelectedItem.ToString().Replace("[[", "").Replace("]]", "");
                    Clipboard.SetData(DataFormats.Text, formatUrl);
                    Beep(300, 200);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Sortie_Lst_SelectedIndexChanged: ", ex.Message, "OpenSource_Frm", AppStart);
            }
        }

        void SaveData_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Sortie_Lst.Items.Count > 0)
                {
                    Stream isData;
                    SaveFileDialog saveFD = new SaveFileDialog
                    {
                        InitialDirectory = AppStart,
                        Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                        FilterIndex = 2,
                        RestoreDirectory = true
                    };

                    if (saveFD.ShowDialog() == DialogResult.OK)
                    {
                        if ((isData = saveFD.OpenFile()) != null)
                        {
                            var ListElements = isData;
                            using (StreamWriter SW = new StreamWriter(ListElements))
                            {
                                foreach (string itm in Sortie_Lst.Items)
                                {
                                    SW.WriteLine(itm);
                                }
                            }

                            isData.Close();

                            Beep(1200, 200);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               senderror.ErrorLog("Error! Sortie_Lst_SelectedIndexChanged: ", ex.Message, "OpenSource_Frm", AppStart);
            }
        }

        void TopNo_Btn_Click(object sender, EventArgs e)
        {
            if (TopNo_Btn.Text == "Top")
            {
                TopMost = true;
                TopNo_Btn.Text = "Not";
            }
            else
            {
                TopMost = false;
                TopNo_Btn.Text = "Top";
            }
        }

        void Sorted_Btn_Click(object sender, EventArgs e)
        {
            Sortie_Lst.Sorted = true;
        }
    }
}
