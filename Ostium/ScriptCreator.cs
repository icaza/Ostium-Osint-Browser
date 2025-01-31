using Icaza;
using Microsoft.Ajax.Utilities;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Ostium
{
    public partial class ScriptCreator : Form
    {
        #region Var_

        [DllImport("kernel32.dll")]
        static extern bool Beep(int freq, int duration);
        readonly string Scripts = Application.StartupPath + @"\scripts\";

        readonly IcazaClass senderror = new IcazaClass();
        readonly IcazaClass openfile = new IcazaClass();

        readonly string AppStart = Application.StartupPath + @"\";

        #endregion

        ListBox List_Object;

        public ScriptCreator()
        {
            InitializeComponent();

            FormClosing += new FormClosingEventHandler(ScriptCreator_Closed);
        }

        void ScriptCreator_Load(object sender, EventArgs e)
        {
            if (File.Exists(Scripts + "scripturl.ost"))
            {
                ScriptUrl_Lst.Items.Clear();
                ScriptUrl_Lst.Items.AddRange(File.ReadAllLines(Scripts + "scripturl.ost"));
            }

            if (File.Exists(Scripts + "scripturlp.ost"))
            {
                ScriptUrlp_Lst.Items.Clear();
                ScriptUrlp_Lst.Items.AddRange(File.ReadAllLines(Scripts + "scripturlp.ost"));
            }
        }

        void ScriptCreator_Closed(object sender, EventArgs e)
        {
            Class_Var.SCRIPTCREATOR = "off";
        }

        #region Btn_

        void NewScript_Btn_Click(object sender, EventArgs e)
        {
            SiteUrl_Txt.Text = string.Empty;
            ScriptTxt_Txt.Text = string.Empty;
            ScriptMinify_Txt.Text = string.Empty;
            ScriptUrl_Lst.ClearSelected();
            ScriptUrlp_Lst.ClearSelected();

            if (@Class_Var.URL_URI != string.Empty)
                SiteUrl_Txt.Text = @Class_Var.URL_URI;
        }

        void OpnScript_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ScriptUrl_Lst.SelectedIndex != -1)
                {
                    List_Object = ScriptUrl_Lst;
                    OpenScript(Scripts);
                }
                else if (ScriptUrlp_Lst.SelectedIndex != -1)
                {
                    List_Object = ScriptUrlp_Lst;
                    OpenScript(Scripts + @"scriptpause\");
                }
                else
                {
                    ScriptUrl_Lst.BackColor = Color.Red;
                    ScriptUrlp_Lst.BackColor = Color.Red;
                    MessageBox.Show("Select script first!");
                    ScriptUrl_Lst.BackColor = Color.FromArgb(41, 44, 51);
                    ScriptUrlp_Lst.BackColor = Color.FromArgb(41, 44, 51);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnScript_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpenScript(string Dirscript)
        {
            try
            {
                SiteUrl_Txt.Text = List_Object.SelectedItem.ToString();
                string ScriptSelect = List_Object.SelectedItem.ToString();
                string scriptName = GenerateFileName(ScriptSelect);
                string filePath = Path.Combine(Dirscript, scriptName + ".js");

                if (File.Exists(filePath))
                {
                    ScriptTxt_Txt.Text = string.Empty;

                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        ScriptTxt_Txt.Text = sr.ReadToEnd();
                    }
                }

                ScriptMinify_Txt.Text = string.Empty;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenScript: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void SaveScript_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (SiteUrl_Txt.Text != string.Empty)
                {
                    if (ScriptUrlp_Lst.SelectedIndex != -1)
                    {
                        List_Object = ScriptUrlp_Lst;
                        SaveScript("scripturlp.ost", Scripts + @"scriptpause\");
                    }
                    else
                    {
                        List_Object = ScriptUrl_Lst;
                        SaveScript("scripturl.ost", Scripts);
                    }
                }
                else
                {
                    SiteUrl_Txt.BackColor = Color.Red;
                    MessageBox.Show("Insert value or select script exist!");
                    SiteUrl_Txt.BackColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! SaveScript_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void SaveScript(string FileUrl, string Dirscript)
        {
            try
            {
                string ScriptSelect = SiteUrl_Txt.Text;
                string scriptName = GenerateFileName(ScriptSelect);
                string filePath = Path.Combine(Dirscript, scriptName + ".js");

                if (!File.Exists(filePath))
                {
                    using (StreamWriter fc = File.AppendText(Scripts + FileUrl))
                    {
                        fc.WriteLine(SiteUrl_Txt.Text);
                    }
                }

                using (StreamWriter fc = new StreamWriter(filePath))
                {
                    fc.Write(ScriptTxt_Txt.Text);
                }

                List_Object.Items.Clear();
                List_Object.Items.AddRange(File.ReadAllLines(Scripts + FileUrl));

                MessageBox.Show("Script save.");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! SaveScript: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        string GenerateFileName(string url)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(url));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        void PauseScript_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ScriptUrl_Lst.SelectedIndex != -1)
                {
                    if (!Directory.Exists(Scripts + "scriptpause"))
                        Directory.CreateDirectory(Scripts + "scriptpause");

                    List_Object = ScriptUrl_Lst;
                    PauseScript("scripturl.ost", "scripturlp.ost", Scripts, Scripts + @"scriptpause\");

                    ScriptUrlp_Lst.Items.Clear();
                    ScriptUrlp_Lst.Items.AddRange(File.ReadAllLines(Scripts + "scripturlp.ost"));
                }
                else if (ScriptUrlp_Lst.SelectedIndex != -1)
                {
                    List_Object = ScriptUrlp_Lst;
                    PauseScript("scripturlp.ost", "scripturl.ost", Scripts + @"scriptpause\", Scripts);

                    ScriptUrl_Lst.Items.Clear();
                    ScriptUrl_Lst.Items.AddRange(File.ReadAllLines(Scripts + "scripturl.ost"));
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! PauseScript_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void PauseScript(string FileUrlA, string FileUrlB, string DirscriptA, string DirscriptB)
        {
            try
            {
                string tmpSiteUrl = List_Object.SelectedItem.ToString();
                string ScriptSelect = List_Object.SelectedItem.ToString();
                string scriptName = GenerateFileName(ScriptSelect);

                File.Copy(DirscriptA + scriptName + ".js", DirscriptB + scriptName + ".js");
                File.Delete(DirscriptA + scriptName + ".js");

                List_Object.Items.Remove(List_Object.SelectedItem);

                var filescript = Scripts + FileUrlA;
                using (StreamWriter SW = new StreamWriter(filescript, false))
                {
                    foreach (string itm in List_Object.Items)
                        SW.WriteLine(itm);
                }

                using (StreamWriter file_create = File.AppendText(Scripts + FileUrlB))
                {
                    file_create.WriteLine(tmpSiteUrl);
                }

                SiteUrl_Txt.Text = string.Empty;
                ScriptTxt_Txt.Text = string.Empty;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! PauseScript: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void DelScript_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ScriptUrl_Lst.SelectedIndex != -1)
                {
                    List_Object = ScriptUrl_Lst;
                    DeleteScript(Scripts + "scripturl.ost", Scripts);
                }
                else if (ScriptUrlp_Lst.SelectedIndex != -1)
                {
                    List_Object = ScriptUrlp_Lst;
                    DeleteScript(Scripts + "scripturlp.ost", Scripts + @"scriptpause\");
                }
                else
                {
                    ScriptUrl_Lst.BackColor = Color.Red;
                    ScriptUrlp_Lst.BackColor = Color.Red;
                    MessageBox.Show("Select script first!");
                    ScriptUrl_Lst.BackColor = Color.FromArgb(41, 44, 51);
                    ScriptUrlp_Lst.BackColor = Color.FromArgb(41, 44, 51);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DelScript_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void DeleteScript(string FileUrl, string Dirscript)
        {
            try
            {
                string message = "Delete script: " + List_Object.SelectedItem.ToString() + " ? ";
                string caption = string.Empty;
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string ScriptSelect = List_Object.SelectedItem.ToString();
                    string scriptName = GenerateFileName(ScriptSelect);
                    string filePath = Path.Combine(Dirscript, scriptName + ".js");

                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    List_Object.Items.Remove(List_Object.SelectedItem);

                    var filescript = FileUrl;
                    using (StreamWriter SW = new StreamWriter(filescript, false))
                    {
                        foreach (string itm in List_Object.Items)
                            SW.WriteLine(itm);
                    }

                    SiteUrl_Txt.Text = string.Empty;
                    ScriptTxt_Txt.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DeleteScript: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpnJSfile_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string fileopen = openfile.Fileselect("c:\\", "txt files (*.js)|*.js|All files (*.*)|*.*", 2);

                if (File.Exists(fileopen) == true)
                {
                    using (StreamReader sr = new StreamReader(fileopen))
                    {
                        ScriptTxt_Txt.Text = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnJSfile_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void MinifyScript_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ScriptTxt_Txt.Text != string.Empty)
                {
                    var minifer = new Minifier();

                    CodeSettings settings = new CodeSettings
                    {
                        EvalTreatment = EvalTreatment.MakeImmediateSafe,
                        PreserveImportantComments = false,
                        LocalRenaming = LocalRenaming.CrunchAll,
                        RemoveUnneededCode = true,
                        PreserveFunctionNames = true,
                        OutputMode = OutputMode.SingleLine
                    };

                    var content = ScriptTxt_Txt.Text;
                    var minified = minifer.MinifyJavaScript(content, settings);

                    ScriptMinify_Txt.Text = minified;
                }
            }
            catch (IOException)
            {
                senderror.ErrorLog("Error! MinifyScript_Btn_Click IO: the script could not be minified.", string.Empty, "ScriptCreator", AppStart);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! MinifyScript_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void CopyScriptMini_Btn_Click(object sender, EventArgs e)
        {
            if (ScriptMinify_Txt.Text != string.Empty)
            {
                Clipboard.SetData(DataFormats.Text, ScriptMinify_Txt.Text);
                Beep(300, 200);
            }
        }

        #endregion

        #region ContextMenu_

        void Cut_Tools_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^" + "x");
        }

        void Copy_Mnu_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^" + "c");
        }

        void Paste_Mnu_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^" + "v");
        }

        void Delete_Mnu_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{DEL}");
        }

        void SelectAll_Mnu_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^" + "a");
        }

        #endregion

        void ScriptUrl_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ScriptUrl_Lst.SelectedIndex != -1)
                ScriptUrlp_Lst.ClearSelected();
        }

        void ScriptUrlp_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ScriptUrlp_Lst.SelectedIndex != -1)
                ScriptUrl_Lst.ClearSelected();
        }
    }
}
