using FastColoredTextBoxNS;
using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using Icaza;

namespace OstiumE
{
    public partial class Main_Frm : Form
    {
        #region Var_

        readonly IcazaClass senderror = new IcazaClass();
        string FileOpen = "";
        string lang = "XML";
        readonly string AppStart = Application.StartupPath + @"\";
        //readonly string SetcolorFile = Application.StartupPath + @"\setcolor.txt";
        //string FirstOpen = "on";

        //styles
        //readonly TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        //readonly TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        //readonly TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        //readonly TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        //readonly TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        //readonly TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        //readonly TextStyle RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        //readonly TextStyle BurlyWoodStyle = new TextStyle(Brushes.BurlyWood, null, FontStyle.Regular);
        //readonly TextStyle CoralStyle = new TextStyle(Brushes.Coral, null, FontStyle.Regular);
        //readonly TextStyle DarkRedStyle = new TextStyle(Brushes.Aquamarine, null, FontStyle.Regular);
        //readonly TextStyle YellowStyle = new TextStyle(Brushes.Yellow, null, FontStyle.Regular);
        //readonly TextStyle LimeStyle = new TextStyle(Brushes.Lime, null, FontStyle.Regular);
        //readonly TextStyle MediumTurquoiseStyle = new TextStyle(Brushes.MediumTurquoise, null, FontStyle.Regular);
        //readonly TextStyle OrangeStyle = new TextStyle(Brushes.Orange, null, FontStyle.Regular);
        //readonly TextStyle FuchsiaStyle = new TextStyle(Brushes.Fuchsia, null, FontStyle.Regular);
        //readonly TextStyle DodgerBlueStyle = new TextStyle(Brushes.DodgerBlue, null, FontStyle.Regular);

        #endregion

        public Main_Frm()
        {
            InitializeComponent();
            //Output_Txt.TextChanged += new EventHandler<TextChangedEventArgs>(Output_Txt_TextChanged);
        }

        void Main_Frm_Load(object sender, EventArgs e)
        {
            try
            {
                Assembly thisAssem = typeof(Program).Assembly;
                AssemblyName thisAssemName = thisAssem.GetName();
                Version ver = thisAssemName.Version;
                Text = string.Format("{0} {1}", thisAssemName.Name, ver);

                string[] args = Environment.GetCommandLineArgs();
                if (args.Length < 2)
                    return;

                string inputArgument = "/input=";
                string inputName = "";

                foreach (string s in args)
                {
                    if (s.ToLower().StartsWith(inputArgument))
                        inputName = s.Remove(0, inputArgument.Length);
                }

                if (inputName != "")
                {
                    FileOpen = inputName;
                    FileDir_Sts.Text = inputName;

                    if (File.Exists(FileOpen))
                    {
                        Thread Thr_OpenFile = new Thread(() => OpenFile_Thr(FileOpen));
                        Thr_OpenFile.Start();

                        Output_Txt.Selection.Start = Place.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Main_Frm_Load: ", ex.Message, "Main_Frm", Application.StartupPath + @"\");
            }
        }

        #region Edit_

        void Find_Tools_Click(object sender, EventArgs e)
        {
            Output_Txt.ShowFindDialog();
        }

        void Replace_Tools_Click(object sender, EventArgs e)
        {
            Output_Txt.ShowReplaceDialog();
        }

        void Collapse_Tools_Click(object sender, EventArgs e)
        {
            if (!lang.StartsWith("CSharp")) return;
            for (int iLine = 0; iLine < Output_Txt.LinesCount; iLine++)
            {
                if (Output_Txt[iLine].FoldingStartMarker == @"#region\b")
                    Output_Txt.CollapseFoldingBlock(iLine);
            }
        }

        void Expand_Tools_Click(object sender, EventArgs e)
        {
            if (!lang.StartsWith("CSharp")) return;
            for (int iLine = 0; iLine < Output_Txt.LinesCount; iLine++)
            {
                if (Output_Txt[iLine].FoldingStartMarker == @"#region\b")
                    Output_Txt.ExpandFoldedBlock(iLine);
            }
        }

        #endregion

        #region Language_

        void LanguageSelect(object sender, EventArgs e)
        {
            lang = (sender as ToolStripMenuItem).Text;
            Output_Txt.ClearStylesBuffer();
            Output_Txt.Range.ClearStyle(StyleIndex.All);
            Lang_Sts.Text = lang;

            switch (lang)
            {
                case "CSharp": Output_Txt.Language = Language.CSharp; break;
                case "VB": Output_Txt.Language = Language.VB; break;
                case "HTML": Output_Txt.Language = Language.HTML; break;
                case "XML": Output_Txt.Language = Language.XML; break;
                case "SQL": Output_Txt.Language = Language.SQL; break;
                case "PHP": Output_Txt.Language = Language.PHP; break;
                case "JS": Output_Txt.Language = Language.JS; break;
                case "Lua": Output_Txt.Language = Language.Lua; break;
            }
            Output_Txt.OnSyntaxHighlight(new TextChangedEventArgs(Output_Txt.Range));
        }

        #endregion

        #region Files_

        void NewFile_Tools_Click(object sender, EventArgs e)
        {
            if (Output_Txt.Text != "")
            {
                string message = "Open in new window?";
                string caption = "File Open";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Process.Start(Application.StartupPath + @"\OstiumE.exe");
                }
                else
                {
                    Output_Txt.Text = "";
                    SavFileDialog();
                }
            }
            else
            {
                Output_Txt.Text = "";
                SavFileDialog();
            }
        }

        void NewWindow_Tools_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + @"\OstiumE.exe");
        }

        void OpenFile_Tools_Click(object sender, EventArgs e)
        {
            try
            {
                if (Output_Txt.Text != "")
                {
                    string message = "Open in new window?";
                    string caption = "File Open";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        IcazaClass openfile = new IcazaClass();
                        string fileopen = openfile.Fileselect("c:\\", "txt files (*.txt)|*.txt|All files (*.*)|*.*", 2);

                        if (fileopen != "")
                        {
                            using (Process objProcess = new Process())
                            {
                                objProcess.StartInfo.FileName = AppStart + "OstiumE.exe";
                                objProcess.StartInfo.Arguments = "/input=\"" + fileopen + "\"";
                                objProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                                objProcess.Start();
                            }
                        }
                    }
                    else
                    {
                        FileOPN();
                    }
                }
                else
                {
                    FileOPN();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenFile_Tools_Click: ", ex.Message, "Main_Frm", Application.StartupPath + @"\");
            }
        }

        void FileOPN()
        {
            try
            {
                IcazaClass openfile = new IcazaClass();
                string fileopen = openfile.Fileselect("c:\\", "txt files (*.txt)|*.txt|All files (*.*)|*.*", 2);

                if (fileopen != "")
                {
                    FileOpen = fileopen;
                    FileDir_Sts.Text = fileopen;

                    Thread Thr_OpenFile = new Thread(() => OpenFile_Thr(fileopen));
                    Thr_OpenFile.Start();

                    Output_Txt.Selection.Start = Place.Empty;

                    FileDir_Sts.ForeColor = Color.DimGray;
                    //FirstOpen = "off";
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! FileOPN: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void CloseFile_Tools_Click(object sender, EventArgs e)
        {
            Output_Txt.Clear();
            FileOpen = "";
            FileDir_Sts.Text = "";
            //FirstOpen = "on";
        }

        void Save_Tools_Click(object sender, EventArgs e)
        {
            try
            {
                if (FileOpen != "")
                {
                    if (File.Exists(FileOpen))
                    {
                        using (StreamWriter file_create = new StreamWriter(FileOpen))
                        {
                            file_create.Write(Output_Txt.Text);
                        }
                        FileDir_Sts.ForeColor = Color.DimGray;
                        //FirstOpen = "off";
                    }
                    else
                    {
                        SavFileDialog();
                    }
                }
                else
                {
                    SavFileDialog();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Save_Tools_Click: ", ex.Message, "Main_Frm", Application.StartupPath + @"\");
            }
        }

        void SaveAs_Tools_Click(object sender, EventArgs e)
        {
            SavFileDialog();
        }

        void Exit_Tools_Click(object sender, EventArgs e)
        {
            Close();
        }

        void SavFileDialog()
        {
            try
            {
                IcazaClass savefiledialog = new IcazaClass();
                string savefile = savefiledialog.Savefiledialog("All files (*.*)|*.*", 2);

                if (savefile != "")
                {
                    using (StreamWriter file_create = new StreamWriter(savefile))
                    {
                        file_create.Write(Output_Txt.Text);
                    }
                    FileOpen = savefile;
                    FileDir_Sts.Text = savefile;

                    FileDir_Sts.ForeColor = Color.DimGray;
                    //FirstOpen = "off";
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! SavFileDialog: ", ex.Message, "Main_Frm", Application.StartupPath + @"\");
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
            if (Output_Txt.SelectedText != "")
                Clipboard.SetDataObject(Output_Txt.SelectedText);
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

        void TopNo_Tools_Click(object sender, EventArgs e)
        {
            TopMost = !TopMost;

            if (TopNo_Tools.Text == "Top")
                TopNo_Tools.Text = "No Top";
            else
                TopNo_Tools.Text = "Top";
        }

        void About_Tools_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Text + "\n\nIs an add-on, editor lite \nby ICAZA MEDIA \n\nmailto: contact@veydunet.com \nweb: " +
                "veydunet.com\n\nCredits:\nFCTB by Pavel Torgashov");
        }

        void OpenFile_Thr(string value)
        {
            try
            {
                Invoke(new Action<string>(Filopentxt), "clear");
                string xtext = "";
                using (StreamReader sr = new StreamReader(value))
                {
                    xtext = sr.ReadToEnd();
                }

                Invoke(new Action<string>(Filopentxt), xtext);

                FileDir_Sts.ForeColor = Color.DimGray;
                //FirstOpen = "off";
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenFile_Thr: ", ex.Message, "Main_Frm", Application.StartupPath + @"\");
            }
        }

        void Filopentxt(string value)
        {          
            switch (value)
            {
                case "clear":
                    Output_Txt.Text = "";
                    break;
                default:
                    Output_Txt.AppendText(value);
                    break;
            }
        }
    }
}
