using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Icaza;
using LoadDirectory;

namespace Ostium
{
    public partial class Mdi_Frm : Form
    {
        #region Var_

        Webview_Frm webviewForm;
        int VerifPosChild = 1;

        readonly string AppStart = Application.StartupPath + @"\";
        readonly string FileDirAll = Application.StartupPath + @"\filesdir\grp-frm\";

        readonly IcazaClass senderror = new IcazaClass();
        readonly Loaddir loadfiledir = new Loaddir();

        readonly List<string> lstUrlDfltCnf = new List<string>();

        #endregion

        public Mdi_Frm()
        {
            InitializeComponent();
            URLlist_Cbx.SelectedIndexChanged += new EventHandler(URLlist_Cbx_SelectedIndexChanged);
        }

        void Mdi_Frm_Load(object sender, EventArgs e)
        {
            loadfiledir.LoadFileDirectory(FileDirAll, "txt", "cbxts", URLlist_Cbx);
            ///
            /// Loading default URLs into a List
            ///
            if (File.Exists(AppStart + "url_dflt_cnf.ost"))
            {
                lstUrlDfltCnf.Clear();
                lstUrlDfltCnf.AddRange(File.ReadAllLines(AppStart + "url_dflt_cnf.ost"));
            }
        }

        void NewFrm_Mnu_Click(object sender, EventArgs e)
        {
            if (@Class_Var.URL_HOME == "")
                @Class_Var.URL_HOME = lstUrlDfltCnf[1].ToString();

            OpnNewForm(@Class_Var.URL_HOME);
        }

        void URLlist_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ChargeListURL(FileDirAll + URLlist_Cbx.Text);

                for (int i = 0; i < UrlOpn_Lst.Items.Count; i++)
                    OpnNewForm(UrlOpn_Lst.Items[i].ToString());
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! URLlist_Cbx_SelectedIndexChanged: ", ex.Message, "Mdi_Frm", AppStart);
            }
        }

        void AddUrlGrp_Btn_Click(object sender, EventArgs e)
        {
            if (URLlist_Cbx.Text != "")
                OpnFileOpt(FileDirAll + URLlist_Cbx.Text);
        }

        void ChargeListURL(string fileselect)
        {
            UrlOpn_Lst.Items.Clear();

            try
            {
                if (File.Exists(fileselect) == true)
                {
                    UrlOpn_Lst.Items.AddRange(File.ReadAllLines(fileselect));
                }
                else
                {
                    using (StreamWriter file_create = new StreamWriter(fileselect))
                    {
                        file_create.Write("");
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ChargeListURL: ", ex.Message, "Mdi_Frm", AppStart);
            }
        }

        void OpnNewForm(string Url)
        {
            try
            {
                @Class_Var.URL_WEBVIEW = Url;

                webviewForm = new Webview_Frm
                {
                    MdiParent = this,
                    Width = 500,
                    Height = 380
                };
                webviewForm.Show();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnNewForm: ", ex.Message, "Mdi_Frm", AppStart);
            }
        }

        void OpnFileOpt(string dir_dir)
        {
            try
            {
                if (!File.Exists(Class_Var.DEFAULT_EDITOR))
                {
                    MessageBox.Show("Editor are not exist in directory, verify your config file!", "Error!");
                    return;
                }

                if (!File.Exists(dir_dir))
                    File_Write(dir_dir, "");

                OpenFile_Editor(dir_dir);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnFileOpt: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void File_Write(string fileName, string content)
        {
            try
            {
                using (StreamWriter outputFile = new StreamWriter(fileName))
                {
                    outputFile.Write(content);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! File_Write: ", ex.Message, "Mdi_Frm", AppStart);
            }
        }

        void OpenFile_Editor(string FileSelect)
        {
            try
            {
                if (!File.Exists(Class_Var.DEFAULT_EDITOR))
                {
                    MessageBox.Show("Editor are not exist in directory, verify your config file!", "Error!");
                    return;
                }

                string aArg = "";
                string strName = Path.GetFileName(Class_Var.DEFAULT_EDITOR);
                if (strName == "OstiumE.exe")
                    aArg = "/input=\"" + FileSelect + "\"";
                else
                    aArg = FileSelect;


                if (FileSelect != "")
                {
                    if (File.Exists(FileSelect))
                    {
                        using (Process Proc = new Process())
                        {
                            Proc.StartInfo.FileName = Class_Var.DEFAULT_EDITOR;
                            Proc.StartInfo.Arguments = aArg;
                            Proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                            Proc.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenFile_Editor: ", ex.Message, "Mdi_Frm", AppStart);
            }
        }

        #region Window_

        void Cascade_Btn_Click(object sender, EventArgs e)
        {
            VerifPosChild = 1;
            LayoutMdi(MdiLayout.Cascade);
        }

        void Vertical_Btn_Click(object sender, EventArgs e)
        {
            VerifPosChild = 2;
            LayoutMdi(MdiLayout.TileVertical);
        }

        void Horizontal_Btn_Click(object sender, EventArgs e)
        {
            VerifPosChild = 3;
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        void ReplaceChild_Btn_Click(object sender, EventArgs e)
        {
            ReplaceChild();
        }

        void ReplaceChild()
        {
            switch (VerifPosChild)
            {
                case 1:
                    LayoutMdi(MdiLayout.Cascade);
                    break;
                case 2:
                    LayoutMdi(MdiLayout.TileVertical);
                    break;
                case 3:
                    LayoutMdi(MdiLayout.TileHorizontal);
                    break;
                default:
                    LayoutMdi(MdiLayout.Cascade);
                    break;
            }
        }

        void CloseAllForm_Btn_Click(object sender, EventArgs e)
        {
            foreach (Form webviewForm in MdiChildren)
                webviewForm.Close();
        }

        #endregion
    }
}
