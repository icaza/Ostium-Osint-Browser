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

                //if (File.Exists(SetcolorFile))
                //{
                //    SetColor_Lbl.Items.AddRange(File.ReadAllLines(SetcolorFile));
                //}

                string[] args = Environment.GetCommandLineArgs();
                if (args.Length < 2)
                {
                    return;
                }

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
                //case "Custom":
                //    Output_Txt.Language = Language.Custom;
                //    Output_Txt.CommentPrefix = "//";
                //    Output_Txt.OnTextChanged();
                //    break;
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

        #region Config_

        void SetColors_Tools_Click(object sender, EventArgs e)
        {
            //if (File.Exists(SetcolorFile) == true)
            //{
            //    StreamReader sr = new StreamReader(SetcolorFile);
            //    Output_Txt.Text = sr.ReadToEnd();
            //    sr.Close();

            //    FileOpen = SetcolorFile;
            //    FileDir_Sts.Text = SetcolorFile;
            //    Output_Txt.Selection.Start = Place.Empty;

            //    FirstOpen = "off";
            //}
        }

        void Reload_Tools_Click(object sender, EventArgs e)
        {
            //SetColor_Lbl.Items.Clear();

            //if (File.Exists(SetcolorFile))
            //{
            //    SetColor_Lbl.Items.AddRange(File.ReadAllLines(SetcolorFile));

            //    Output_Txt.ClearStylesBuffer();
            //    Output_Txt.Range.ClearStyle(StyleIndex.All);

            //    Output_Txt.Language = Language.Custom;
            //    Output_Txt.CommentPrefix = "//";
            //    Output_Txt.OnTextChanged();

            //    Output_Txt.OnSyntaxHighlight(new TextChangedEventArgs(Output_Txt.Range));
            //}
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
            {
                Clipboard.SetDataObject(Output_Txt.SelectedText);
            }
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
            if (TopNo_Tools.Text == "Top")
            {
                TopNo_Tools.Text = "No Top";
                TopMost = true;
            }
            else
            {
                TopNo_Tools.Text = "Top";
                TopMost = false;
            }
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

        //void Output_Txt_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    Output_Txt.OnSyntaxHighlight(new TextChangedEventArgs(Output_Txt.Range));

        //    if (lang.StartsWith("Custom"))
        //    {
        //        e.ChangedRange.SetStyle(BlueStyle, SetColor_Lbl.Items[0].ToString());
        //        e.ChangedRange.SetStyle(BoldStyle, SetColor_Lbl.Items[1].ToString());
        //        e.ChangedRange.SetStyle(RedStyle, SetColor_Lbl.Items[2].ToString());
        //        e.ChangedRange.SetStyle(MagentaStyle, SetColor_Lbl.Items[3].ToString());
        //        e.ChangedRange.SetStyle(GreenStyle, SetColor_Lbl.Items[4].ToString());
        //        e.ChangedRange.SetStyle(BrownStyle, SetColor_Lbl.Items[5].ToString());
        //        e.ChangedRange.SetStyle(MaroonStyle, SetColor_Lbl.Items[6].ToString());
        //        e.ChangedRange.SetStyle(RedStyle, SetColor_Lbl.Items[7].ToString());
        //        e.ChangedRange.SetStyle(BurlyWoodStyle, SetColor_Lbl.Items[8].ToString());
        //        e.ChangedRange.SetStyle(CoralStyle, SetColor_Lbl.Items[9].ToString());
        //        e.ChangedRange.SetStyle(DarkRedStyle, SetColor_Lbl.Items[10].ToString());
        //        e.ChangedRange.SetStyle(YellowStyle, SetColor_Lbl.Items[11].ToString());
        //        e.ChangedRange.SetStyle(DodgerBlueStyle, SetColor_Lbl.Items[12].ToString());
        //        e.ChangedRange.SetStyle(LimeStyle, SetColor_Lbl.Items[13].ToString());
        //        e.ChangedRange.SetStyle(MediumTurquoiseStyle, SetColor_Lbl.Items[14].ToString());
        //        e.ChangedRange.SetStyle(OrangeStyle, SetColor_Lbl.Items[15].ToString());
        //        e.ChangedRange.SetStyle(FuchsiaStyle, SetColor_Lbl.Items[16].ToString());
        //    }
        //    if (FirstOpen == "off")
        //        FileDir_Sts.ForeColor = Color.Red;
        //}
    }
}
