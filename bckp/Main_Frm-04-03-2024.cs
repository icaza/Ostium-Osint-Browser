using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Data.SQLite;
using System.Xml;
using Microsoft.VisualBasic;
using System.ServiceModel.Syndication;
using Ostium.Properties;
using Icaza;
using LoadDirectory;
using Dirsize;
using System.Speech.Synthesis;
using System.Linq;
using Newtonsoft.Json;

namespace Ostium
{
    public partial class Main_Frm : Form
    {
        #region Var_Function

        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);

        readonly SpeechSynthesizer synth = new SpeechSynthesizer();

        readonly string AppStart = Application.StartupPath + @"\";
        readonly string Plugins = Application.StartupPath + @"\add-on\";
        readonly string DBdirectory = Application.StartupPath + @"\database\";
        readonly string FeedDir = Application.StartupPath + @"\feed\";
        readonly string FileDir = Application.StartupPath + @"\filesdir\";
        readonly string URLconstructor = Application.StartupPath + @"\filesdir\url-constructor\";
        readonly string Pictures = Application.StartupPath + @"\pictures\";
        readonly string Scripts = Application.StartupPath + @"\scripts\";
        readonly string Workflow = Application.StartupPath + @"\workflow\";
        readonly string WorkflowModel = Application.StartupPath + @"\workflow\model\";
        readonly string DiagramDir = Application.StartupPath + @"\diagram\";

        public string D4ta = "default_database_name";

        public Webview_Frm webviewForm;
        public HtmlText_Frm HtmlTextFrm;
        public Mdi_Frm mdiFrm;
        public Doc_Frm docForm;
        public OpenSource_Frm openSourceForm;
        public ScriptCreator scriptCreatorFrm;

        public ListBox List_Object;
        public ToolStripComboBox Cbx_Object;
        public ListBox List_Wf;
        public ComboBox Workflow_Cbx;
        public ListBox Workflow_Lst;
        public Label SizeAll_Lbl;

        public string SoftVersion = "";
        public string ClearOnOff = "on";
        public string NameUriDB = "";
        public string UnshortURLval = "";
        string UsenameAleatoire = "";
        public int CMDconsSwitch;
        public string TableOpen = "";
        public string Tables_Lst_Opt = "add";
        public string DataAddSelectUri = "";
        public string tlsi = ""; // Table List selected Item
        public string DBadmin = "off";
        public string ManageFeed = "off";
        public string TmpTitleWBrowse = "";
        public string TmpTitleWBrowsefeed = "";
        public int VerifLangOpn = 0;
        public string TitleFeed;
        public string AddTitleItem;
        public string AddLinkItem;
        public string UserAgentOnOff = "off";
        public string UserAgentSelect = "";

        readonly IcazaClass senderror = new IcazaClass();
        readonly Loaddir loadfiledir = new Loaddir();
        readonly IcazaClass selectdir = new IcazaClass();
        readonly IcazaClass openfile = new IcazaClass();
        readonly ReturnSize sizedireturn = new ReturnSize();

        readonly string MessageStartDiagram = "When this window closes, the diagram creation process begins, be patient the time depends on the file size " +
            "and structure. In case of blockage! use Debug in the menu to kill the javaw process. Feel free to join the Discord channel for help.";
        
        public string ThemeDiag;
        public string FileDiag;

        #endregion

        #region Frm_

        public Main_Frm()
        {
            InitializeComponent();
            WBrowse_EventHandlers(WBrowse);
            WBrowsefeed_EventHandlers(WBrowsefeed);
            Form_EventHandler();

            Assembly thisAssem = typeof(Program).Assembly;
            AssemblyName thisAssemName = thisAssem.GetName();
            Version ver = thisAssemName.Version;
            SoftVersion = string.Format("{0} {1}", thisAssemName.Name, ver);
        }

        private void Main_Frm_Load(object sender, EventArgs e)
        {
            try
            {
                BeginInvoke((MethodInvoker)delegate {
                    CreateDirectory();

                    if (File.Exists(AppStart + "config.xml"))
                    {
                        Config_Ini(AppStart + "config.xml");
                    }
                    else
                    {
                        CreateConfigFile(0);
                    }

                    WBrowse.Source = new Uri(@Class_Var.URL_HOME);
                    WBrowsefeed.Source = new Uri(@Class_Var.URL_HOME);

                    Tools_TAB_0.Visible = true;
                });
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Main_Frm_Load: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Main_Frm_FormClosed(object sender, EventArgs e)
        {
            try
            {
                if (ClearOnOff == "on")
                {
                    var result = MessageBox.Show("Delete history? (Start purge.bat after Ostium close, for complete cleaning)", "Delete history", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Process.Start(AppStart);
                        if (Directory.Exists(AppStart + "Ostium.exe.WebView2"))
                            Directory.Delete(AppStart + "Ostium.exe.WebView2", true);
                    }
                }
            }
            catch { }
        }

        #endregion

        private void Form_EventHandler()
        {
            FormClosing += new FormClosingEventHandler(Main_Frm_FormClosed);
            CategorieFeed_Cbx.SelectedIndexChanged += new EventHandler(CategorieFeed_Cbx_SelectedIndexChanged);
            CountBlockSite_Lbl.MouseEnter += new EventHandler(CountBlockSite_Lbl_MouseEnter);
            CountBlockSite_Lbl.MouseLeave += new EventHandler(CountBlockSite_Lbl_Leave);
        }

        private void CreateConfigFile(int val)
        {
            if (val == 0)
            {
                DefaultEditor_Opt_Txt.Text = AppStart + "OstiumE.exe";
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                CheckCharacters = true
            };

            using (XmlWriter writer = XmlWriter.Create(AppStart + "config.xml", settings))
            {
                writer.WriteStartElement("Xwparsingxml");
                writer.WriteStartElement("Xwparsingnode");
                writer.WriteElementString("DB_USE_DEFAULT", DB_Default_Opt_Txt.Text);
                writer.WriteElementString("URL_HOME_VAR", UrlHome_Opt_Txt.Text);
                writer.WriteElementString("URL_TRAD_WEBPAGE_VAR", UrlTradWebPage_Opt_Txt.Text);
                writer.WriteElementString("URL_TRAD_WEBTXT_VAR", "https://translate.google.fr/?hl=fr&sl=auto&tl=fr&text=VVVV0VVVV&op=translate");
                writer.WriteElementString("URL_DEFAUT_WSEARCH_VAR", SearchEngine_Opt_Txt.Text);
                writer.WriteElementString("URL_USER_AGENT_VAR", UserAgent_Opt_Txt.Text);
                writer.WriteElementString("URL_USER_AGENT_SRC_PAGE_VAR", UserAgentHttp_Opt_Txt.Text);
                writer.WriteElementString("URL_GOOGLEBOT_VAR", GoogBot_Opt_Txt.Text);
                writer.WriteElementString("DEFAULT_EDITOR_VAR", DefaultEditor_Opt_Txt.Text);
                writer.WriteElementString("VOLUME_TRACK_VAR", Convert.ToString(VolumeVal_Track.Value));
                writer.WriteElementString("RATE_TRACK_VAR", Convert.ToString(RateVal_Track.Value));
                writer.WriteEndElement();
                writer.Flush();
            }

            Config_Ini(AppStart + "config.xml");
        }

        public void CreateDirectory()
        {
            try
            {
                DirectoryCreate(Plugins);
                DirectoryCreate(DBdirectory);
                DirectoryCreate(FeedDir);
                DirectoryCreate(FileDir);
                DirectoryCreate(FileDir + "url-constructor");
                DirectoryCreate(FileDir + "grp-frm");
                DirectoryCreate(URLconstructor);
                DirectoryCreate(Pictures);
                DirectoryCreate(Scripts);
                DirectoryCreate(Workflow);
                DirectoryCreate(Workflow + "model");
                DirectoryCreate(DiagramDir);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateDirectory: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void DirectoryCreate(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public void Config_Ini(string ConfigFile)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(ConfigFile))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name.ToString())
                            {
                                case "DB_USE_DEFAULT":
                                    {
                                        DB_Default_Txt.Text = Convert.ToString(reader.ReadString());
                                        DB_Default_Opt_Txt.Text = DB_Default_Txt.Text;
                                        break;
                                    }
                                case "URL_HOME_VAR":
                                    {
                                        Class_Var.URL_HOME = reader.ReadString();
                                        UrlHome_Opt_Txt.Text = Class_Var.URL_HOME;
                                        break;
                                    }
                                case "URL_TRAD_WEBPAGE_VAR":
                                    {
                                        Class_Var.URL_TRAD_WEBPAGE = reader.ReadString();
                                        UrlTradWebPage_Opt_Txt.Text = Class_Var.URL_TRAD_WEBPAGE;
                                        break;
                                    }
                                case "URL_TRAD_WEBTXT_VAR":
                                    {
                                        Class_Var.URL_TRAD_WEBTXT = reader.ReadString();
                                        break;
                                    }
                                case "URL_DEFAUT_WSEARCH_VAR":
                                    {
                                        Class_Var.URL_DEFAUT_WSEARCH = reader.ReadString();
                                        SearchEngine_Opt_Txt.Text = Class_Var.URL_DEFAUT_WSEARCH;
                                        break;
                                    }
                                case "URL_USER_AGENT_VAR":
                                    {
                                        Class_Var.URL_USER_AGENT = reader.ReadString();
                                        UserAgent_Opt_Txt.Text = Class_Var.URL_USER_AGENT;
                                        break;
                                    }
                                case "URL_USER_AGENT_SRC_PAGE_VAR":
                                    {
                                        Class_Var.URL_USER_AGENT_SRC_PAGE = reader.ReadString();
                                        UserAgentHttp_Opt_Txt.Text = Class_Var.URL_USER_AGENT_SRC_PAGE;
                                        break;
                                    }
                                case "URL_GOOGLEBOT_VAR":
                                    {
                                        Class_Var.URL_GOOGLEBOT = reader.ReadString();
                                        GoogBot_Opt_Txt.Text = Class_Var.URL_GOOGLEBOT;
                                        break;
                                    }
                                case "DEFAULT_EDITOR_VAR":
                                    {
                                        Class_Var.DEFAULT_EDITOR = reader.ReadString();
                                        DefaultEditor_Opt_Txt.Text = Class_Var.DEFAULT_EDITOR;
                                        break;
                                    }
                                case "VOLUME_TRACK_VAR":
                                    {
                                        Class_Var.VOLUME_TRACK = Convert.ToInt32(reader.ReadString());
                                        VolumeVal_Track.Value = Class_Var.VOLUME_TRACK;
                                        break;
                                    }
                                case "RATE_TRACK_VAR":
                                    {
                                        Class_Var.RATE_TRACK = Convert.ToInt32(reader.ReadString());
                                        RateVal_Track.Value = Class_Var.RATE_TRACK;
                                        break;
                                    }
                            }
                        }
                    }
                }

                if (DB_Default_Txt.Text != "0x0")
                {
                    D4ta = DBdirectory + DB_Default_Txt.Text;
                }
                else
                {
                    string message, title;
                    object ValueInput;

                    message = "First use. \n\nChoose a name for the database or leave empty.";
                    title = "Database default name";

                    ValueInput = Interaction.InputBox(message, title);
                    string ValueOutput = Convert.ToString(ValueInput);
                    ValueOutput = Regex.Replace(ValueOutput, "[^a-zA-Z0-9]", "");
                    ValueOutput += ".db";

                    if (ValueOutput != "" && ValueOutput != ".db")
                    {
                        if (!File.Exists(DBdirectory + ValueOutput))
                        {
                            SQLiteConnection.CreateFile(DBdirectory + ValueOutput);
                        }

                        DB_Default_Txt.Text = ValueOutput;
                        D4ta = DBdirectory + DB_Default_Txt.Text;                        

                        ChangeDBdefault(DB_Default_Txt.Text);
                    }
                    else
                    {
                        DB_Default_Txt.Text = "D4taB.db";

                        if (!File.Exists(DBdirectory + "D4taB.db"))
                        {
                            SQLiteConnection.CreateFile(DBdirectory + "D4taB.db");
                        }

                        D4ta = DBdirectory + DB_Default_Txt.Text;

                        ChangeDBdefault(DB_Default_Txt.Text);
                    }
                    DB_Default_Opt_Txt.Text = DB_Default_Txt.Text;                    
                }

                if (File.Exists(FileDir + "url.txt"))
                {
                    URL_URL_Cbx.Items.Clear();
                    URL_URL_Cbx.Items.AddRange(File.ReadAllLines(FileDir + "url.txt"));
                }

                if (File.Exists(FileDir + @"url-constructor\construct_url.txt"))
                {
                    ConstructURL_Lst.Items.Clear();
                    ConstructURL_Lst.Items.AddRange(File.ReadAllLines(FileDir + @"url-constructor\construct_url.txt"));
                }

                if (File.Exists(Scripts + "scripturl.ost"))
                {
                    ScriptUrl_Lst.Items.Clear();
                    ScriptUrl_Lst.Items.AddRange(File.ReadAllLines(Scripts + "scripturl.ost"));
                }

                loadfiledir.LoadFileDirectory(Plugins, "exe", "cbxts", AddOn_Cbx);
                loadfiledir.LoadFileDirectory(FileDir + "url-constructor", "txt", "cbxts", Construct_URL_Cbx);
                loadfiledir.LoadFileDirectory(FeedDir, "*", "cbxts", CategorieFeed_Cbx);
                loadfiledir.LoadFileDirectory(Workflow, "xml", "lst", ProjectOpn_Lst);
                loadfiledir.LoadFileDirectory(WorkflowModel, "txt", "lst", ModelList_Lst);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Config_Ini: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private async void DireSizeCalc(string directoryname, object objectsend)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directoryname);
                long dirSize = await Task.Run(() => dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));

                var calcsize = sizedireturn.GetSizeName(long.Parse(Convert.ToString(dirSize)));

                SizeAll_Lbl = (Label)objectsend;
                SizeAll_Lbl.Text = calcsize;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DireSizeCalc: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #region Browser_Event Handler

        public void WBrowse_UpdtTitleEvent(string message)
        {
            string currentDocumentTitle = WBrowse?.CoreWebView2?.DocumentTitle ?? "Uninitialized";
            Text = currentDocumentTitle + " [" + message + "]";
            TmpTitleWBrowse = Text;
        }

        public void WBrowse_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var settings = WBrowse.CoreWebView2.Settings;

            if (UserAgentOnOff == "on")
            {
                settings.UserAgent = UserAgentSelect;
            }

            WBrowse_UpdtTitleEvent("Navigation Starting");
        }

        public void WBrowse_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            GetCookie(WBrowse.Source.AbsoluteUri);
            WBrowse_UpdtTitleEvent("Navigation Completed");

            ScriptInject();
        }

        public void WBrowse_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (SetCookie_Chk.Checked)
            {
                CoreWebView2Cookie cookie = WBrowse.CoreWebView2.CookieManager.CreateCookie(CookieName_Txt.Text, CookieValue_Txt.Text, CookieDomain_Txt.Text, "/");
                WBrowse.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);
            }
        }

        public void WBrowse_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            URLtxt_txt.Text = WBrowse.Source.AbsoluteUri;
            WBrowse_UpdtTitleEvent("Source Changed");
        }

        public void WBrowse_HistoryChanged(object sender, object e)
        {
            Back_Btn.Enabled = WBrowse.CoreWebView2.CanGoBack;
            Forward_Btn.Enabled = WBrowse.CoreWebView2.CanGoForward;
            WBrowse_UpdtTitleEvent("History Changed");
        }

        public void WBrowse_DocumentTitleChanged(object sender, object e)
        {
            Text = WBrowse.CoreWebView2.DocumentTitle;
            NameUriDB = WBrowse.CoreWebView2.DocumentTitle;

            WBrowse_UpdtTitleEvent("DocumentTitleChanged");
            Download_Source_Page();
        }

        public void WBrowse_InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                MessageBox.Show($"WBrowse creation failed with exception = {e.InitializationException}");
                WBrowse_UpdtTitleEvent("Initialization Completed failed");
                return;
            }

            WBrowse.CoreWebView2.SourceChanged += WBrowse_SourceChanged;
            WBrowse.CoreWebView2.HistoryChanged += WBrowse_HistoryChanged;
            WBrowse.CoreWebView2.DocumentTitleChanged += WBrowse_DocumentTitleChanged;
            WBrowse.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Image);
            WBrowse.CoreWebView2.WebResourceRequested += WBrowse_WebResourceRequested;

            WBrowse_UpdtTitleEvent("Initialization Completed succeeded");
        }

        void WBrowse_EventHandlers(Microsoft.Web.WebView2.WinForms.WebView2 control)
        {
            control.CoreWebView2InitializationCompleted += WBrowse_InitializationCompleted;
            control.NavigationStarting += WBrowse_NavigationStarting;
            control.NavigationCompleted += WBrowse_NavigationCompleted;
            control.SourceChanged += WBrowse_SourceChanged;
        }

        // Wbrowsefeed

        public void WBrowsefeed_UpdtTitleEvent(string message)
        {
            string currentDocumentTitle = WBrowsefeed?.CoreWebView2?.DocumentTitle ?? "Uninitialized";
            Text = currentDocumentTitle + " [" + message + "]";
            TmpTitleWBrowsefeed = Text;
        }

        public void WBrowsefeed_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            WBrowsefeed_UpdtTitleEvent("Navigation Starting");
        }

        public void WBrowsefeed_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            GetCookie(WBrowsefeed.Source.AbsoluteUri);
            WBrowsefeed_UpdtTitleEvent("Navigation Completed");
        }

        public void WBrowsefeed_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            URLtxt_txt.Text = WBrowsefeed.Source.AbsoluteUri;
            WBrowsefeed_UpdtTitleEvent("Source Changed");
        }

        public void WBrowsefeed_HistoryChanged(object sender, object e)
        {
            WBrowsefeed_UpdtTitleEvent("History Changed");
        }

        public void WBrowsefeed_DocumentTitleChanged(object sender, object e)
        {
            Text = WBrowsefeed.CoreWebView2.DocumentTitle;

            WBrowsefeed_UpdtTitleEvent("DocumentTitleChanged");
            //Download_Source_Page();
        }

        public void WBrowsefeed_InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                MessageBox.Show($"WBrowsefeed creation failed with exception = {e.InitializationException}");
                WBrowsefeed_UpdtTitleEvent("Initialization Completed failed");
                return;
            }

            WBrowsefeed.CoreWebView2.SourceChanged += WBrowsefeed_SourceChanged;
            WBrowsefeed.CoreWebView2.HistoryChanged += WBrowsefeed_HistoryChanged;
            WBrowsefeed.CoreWebView2.DocumentTitleChanged += WBrowsefeed_DocumentTitleChanged;
            WBrowsefeed.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Image);

            WBrowsefeed_UpdtTitleEvent("Initialization Completed succeeded");
        }

        void WBrowsefeed_EventHandlers(Microsoft.Web.WebView2.WinForms.WebView2 control)
        {
            control.CoreWebView2InitializationCompleted += WBrowsefeed_InitializationCompleted;
            control.NavigationStarting += WBrowsefeed_NavigationStarting;
            control.NavigationCompleted += WBrowsefeed_NavigationCompleted;
            control.SourceChanged += WBrowsefeed_SourceChanged;
        }

        #endregion

        private async void ScriptInject()
        {
            try
            {
                if (ScriptUrl_Lst.Items.Count > 0)
                {
                    string ScriptSelect = WBrowse.Source.AbsoluteUri;

                    for (int i = 0; i < ScriptUrl_Lst.Items.Count; i++)
                    {
                        if (WBrowse.Source.AbsoluteUri.Contains(ScriptUrl_Lst.Items[i].ToString()))
                        {
                            ScriptSelect = Regex.Replace(ScriptSelect, "[^a-zA-Z]", "");

                            if (File.Exists(Scripts + ScriptSelect + ".js"))
                            {
                                string text = File.ReadAllText(Scripts + ScriptSelect + ".js");
                                await WBrowse.CoreWebView2.ExecuteScriptAsync(text);
                            }
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ScriptInject: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #region Control_Browser

        public void Go_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser(0);
        }

        public void GoWebwiev_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser(1);
        }

        public void GoBrowser(int WebviewRedirect)
        {
            try
            {
                var rawUrl = URLbrowse_Cbx.Text;
                Uri uri;

                if (rawUrl.Contains("file:///"))
                {
                    uri = new Uri(rawUrl);
                }
                else
                {
                    if (Uri.IsWellFormedUriString(rawUrl, UriKind.Absolute))
                    {
                        uri = new Uri(rawUrl);
                    }
                    else if (!rawUrl.Contains(" ") && rawUrl.Contains("."))
                    {
                        uri = new Uri("https://" + rawUrl);
                    }
                    else
                    {
                        uri = new Uri(Class_Var.URL_DEFAUT_WSEARCH +
                            string.Join("+", Uri.EscapeDataString(rawUrl).Split(new string[] { "%20" }, StringSplitOptions.RemoveEmptyEntries)));
                    }
                }

                if (WebviewRedirect == 0)
                {
                    WBrowse.Source = uri;
                }
                else
                {
                    Class_Var.URL_WEBVIEW = Convert.ToString(uri);
                    GoNewtab();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! GoBrowser: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void GoNewtab()
        {
            try
            {
                webviewForm = new Webview_Frm();
                webviewForm.Show();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! GoNewtab: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Back_Btn_Click(object sender, EventArgs e)
        {
            WBrowse.GoBack();
        }

        public void Forward_Btn_Click(object sender, EventArgs e)
        {
            WBrowse.GoForward();
        }

        public void Refresh_Btn_Click(object sender, EventArgs e)
        {
            WBrowse.Reload();
        }

        public void Home_Btn_Click(object sender, EventArgs e)
        {
            WBrowse.Source = new Uri(@Class_Var.URL_HOME);
        }

        public void OnKey_URLbrowse(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                GoBrowser(0);
            }
        }

        public void URLbrowse_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (Control_Tab.SelectedIndex == 0)
            //{
            //    WBrowse.Source = new Uri(@URLbrowse_Cbx.Text);
            //}
        }

        public void URL_URL_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                WBrowse.Source = new Uri(@URL_URL_Cbx.Text);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! URL_URL_Cbx_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void CleanSearch_Btn_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Text = "";
        }

        #endregion

        #region Tools_Tab_0

        private void UserAgentChange_Btn_Click(object sender, EventArgs e)
        {
            if (UserAgentChange_Btn.Text == "Change User Agent Off")
            {
                UserAgentChange_Btn.Text = "Change User Agent On";
                UserAgentSelect = Class_Var.URL_USER_AGENT;
                UserAgentOnOff = "on";
            }
            else
            {
                ClearOnOff = "off";
                Process.Start(AppStart + "Ostium.exe");
                Close();
            }
        }

        private void Googlebot_Btn_Click(object sender, EventArgs e)
        {
            if (Googlebot_Btn.Text == "Googlebot Off")
            {
                Googlebot_Btn.Text = "Googlebot On";
                UserAgentSelect = Class_Var.URL_GOOGLEBOT;
                UserAgentOnOff = "on";
            }
            else
            {
                ClearOnOff = "off";
                Process.Start(AppStart + "Ostium.exe");
                Close();
            }
        }

        public void Word_Construct_URL_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Word_Construct_URL_Txt.Text != "")
                {
                    Word_Construct_URL_Txt.Text = Word_Construct_URL_Txt.Text.Replace(" ", "%20");
                    Construct_URL(Word_Construct_URL_Txt.Text);
                    Beep(800, 200);
                }
                else
                {
                    Word_Construct_URL_Txt.BackColor = Color.Red;
                    MessageBox.Show("First insert Name, ID or Word!");
                    Word_Construct_URL_Txt.BackColor = Color.FromArgb(41, 44, 51);
                    return;
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Word_Construct_URL_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Console_Btn_Click(object sender, EventArgs e)
        {
            if (Console_Cmd_Txt.Visible != true)
            {
                Console_Cmd_Txt.Visible = true;
            }
            else
            {
                Console_Cmd_Txt.Visible = false;
            }
        }

        private void Mute_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                WBrowse.CoreWebView2.IsMuted = !WBrowse.CoreWebView2.IsMuted;

                bool isDocumentPlayingAudio = WBrowse.CoreWebView2.IsDocumentPlayingAudio;
                bool isMuted = WBrowse.CoreWebView2.IsMuted;

                if (isDocumentPlayingAudio)
                {
                    if (isMuted)
                    {
                        Mute_Btn.Image = (Bitmap)Resources.ResourceManager.GetObject("Mute");
                    }
                    else
                    {
                        Mute_Btn.Image = (Bitmap)Resources.ResourceManager.GetObject("Unmute");
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Mute_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void CopyURL_Mnu_Click(object sender, EventArgs e)
        {
            try
            {
                string textData = WBrowse.Source.AbsoluteUri;
                Clipboard.SetData(DataFormats.Text, textData);
                Beep(1500, 400);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CopyURL_Mnu_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void TraductPage_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string formatURI = Regex.Replace(Class_Var.URL_TRAD_WEBPAGE, "VVVV0VVVV", WBrowse.Source.AbsoluteUri);
                WBrowse.Source = new Uri(@formatURI);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! TraductPage_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void UnshortUrl_Btn_Click(object sender, EventArgs e)
        {
            if (URLbrowse_Cbx.Text != "")
            {
                StartUnshortUrl(URLbrowse_Cbx.Text);
            }
            else
            {
                URLbrowse_Cbx.BackColor = Color.Red;
                MessageBox.Show("Insert URL!");
                URLbrowse_Cbx.BackColor = Color.FromArgb(41, 44, 51);
            }
        }

        public void StartUnshortUrl(string Uri)
        {
            UnshortURLval = Uri;

            Thread Thr_UnshortUrl = new Thread(new ThreadStart(UnshortUrl));
            Thr_UnshortUrl.Start();
        }

        public void UnshortUrl()
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(UnshortURLval);
                req.AllowAutoRedirect = false;
                var resp = req.GetResponse();
                string realUrl = resp.Headers["Location"];

                Invoke(new Action<string>(URLbrowseCbxText), realUrl);
            }
            catch (WebException ex)
            {
                senderror.ErrorLog("Error! => UnshortUrl() WebException: ", ex.Message, "Main_Frm", AppStart);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! => UnshortUrl() Exception: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void AddOn_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = Plugins + AddOn_Cbx.Text;
                    proc.StartInfo.Arguments = "";
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.WorkingDirectory = Plugins;
                    proc.StartInfo.RedirectStandardOutput = false;
                    proc.Start();
                }                
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddOn_Cbx_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Construct_URL_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ConstructURL_Lst.Items.Clear();
                ConstructURL_Lst.Items.AddRange(File.ReadAllLines(FileDir + @"url-constructor\" + Construct_URL_Cbx.Text));
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Construct_URL_Cbx_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void GoogleDork_Btn_Click(object sender, EventArgs e)
        {
            if (File.Exists(FileDir + "gh.txt"))
                Open_Source_Frm(FileDir + "gh.txt");
        }

        public async void WebpageToPng_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                var img = await TakeWebScreenshot();
                CreateNameAleat();
                img.Save(Pictures + UsenameAleatoire + ".png", System.Drawing.Imaging.ImageFormat.Png);
                Beep(800, 200);

                Process.Start(Pictures + UsenameAleatoire + ".png");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! WebpageToPng_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void HTMLtxt_Btn_Click(object sender, EventArgs e)
        {
            HtmlTextFrm = new HtmlText_Frm();
            HtmlTextFrm.Show();
        }

        public async Task<Image> TakeWebScreenshot(bool currentControlClipOnly = false)
        {
            dynamic scl = null;
            Size siz;

            if (!currentControlClipOnly)
            {
                var res = await WBrowse.CoreWebView2.ExecuteScriptAsync(@"var v = {""w"":document.body.scrollWidth, ""h"":document.body.scrollHeight}; v;");
                try { scl = JObject.Parse(res); } catch { }
            }
            siz = scl != null ?
                        new Size((int)scl.w > WBrowse.Width ? (int)scl.w : WBrowse.Width,
                                    (int)scl.h > WBrowse.Height ? (int)scl.h : WBrowse.Height)
                        :
                        WBrowse.Size;

            var img = await GetWebBrowserBitmap(siz);
            return img;
        }

        public async Task<Bitmap> GetWebBrowserBitmap(Size clipSize)
        {
            dynamic clip = new JObject();
            clip.x = 0;
            clip.y = 0;
            clip.width = clipSize.Width;
            clip.height = clipSize.Height;
            clip.scale = 1;

            dynamic settings = new JObject();
            settings.format = "png";
            settings.clip = clip;
            settings.fromSurface = true;
            settings.captureBeyondViewport = true;

            var p = settings.ToString(Newtonsoft.Json.Formatting.None);

            var devData = await WBrowse.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", p);
            var imgData = (string)JObject.Parse(devData).data;
            var ms = new MemoryStream(Convert.FromBase64String(imgData));
            return (Bitmap)Image.FromStream(ms);
        }

        public void Cookie_Btn_Click(object sender, EventArgs e)
        {
            OpenFile_Editor(AppStart + "cookie.txt");
        }

        private void SetCookie_Btn_Click(object sender, EventArgs e)
        {
            if (Cookie_Pnl.Visible)
            {
                Cookie_Pnl.Visible = false;
            }
            else
            {
                Cookie_Pnl.Visible = true;
            }
        }

        public void OpnFilOnEditor_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string fileopen = openfile.Fileselect(AppStart, "txt files (*.txt)|*.txt|All files (*.*)|*.*", 2);

                if (fileopen != "")
                {
                    string filePath = fileopen;
                    OpenFile_Editor(filePath);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnFilOnEditor_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void OpenListLink_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = AppStart;
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        Open_Source_Frm(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenListLink_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void OpnGroupFrm_Btn_Click(object sender, EventArgs e)
        {
            mdiFrm = new Mdi_Frm();
            mdiFrm.Show();
        }

        public void Memo_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(AppStart + "memo.txt");
        }

        private void Editor_Btn_Click(object sender, EventArgs e)
        {
            if (File.Exists(Class_Var.DEFAULT_EDITOR))
            {
                Process.Start(Class_Var.DEFAULT_EDITOR);
            }
            else
            {
                MessageBox.Show("File Editor are not exist in directory, verify your config file!", "Error!");
            }
        }

        private void OpnDirectory_Btn_Click(object sender, EventArgs e)
        {
            Process.Start(AppStart);
        }

        public void IndexDir_Btn_Click(object sender, EventArgs e)
        {
            WBrowse.Source = new Uri(@AppStart);
        }

        private async void InjectScript_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string message, title;
                object ScriptSelect;

                message = "Enter some JavaScript to be executed in the context of this page.";
                title = "Inject Script";

                ScriptSelect = Interaction.InputBox(message, title);
                string ScriptInject = Convert.ToString(ScriptSelect);

                if (ScriptInject != "")
                {
                    try
                    {
                        await WBrowse.ExecuteScriptAsync(ScriptInject);
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(this, ex.Message, "Execute Script Fails!");
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! InjectScript_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void OpenScriptEdit_Btn_Click(object sender, EventArgs e)
        {
            scriptCreatorFrm = new ScriptCreator();
            scriptCreatorFrm.Show();
        }

        private void RegexCmd_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string message, title;
                object RegexSelect;

                message = "Enter some Regex to be executed in the context of this page.";
                title = "Regular Expression";

                RegexSelect = Interaction.InputBox(message, title);
                string ScriptInject = Convert.ToString(RegexSelect);

                if (ScriptInject != "")
                {
                    Thread Thr_CMDConsoleExec = new Thread(() => CMD_Console_Exec(4, ScriptInject));
                    Thr_CMDConsoleExec.Start();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! RegexCmd_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void ReloadConfig_Btn_Click(object sender, EventArgs e)
        {
            if (File.Exists(AppStart + "config.xml"))
            {
                Config_Ini(AppStart + "config.xml");
                MessageBox.Show("Config reload Ok.");
            }
            else
            {
                MessageBox.Show("Config file missing!");
            }
        }

        private void JavaEnableDisable_Btn_Click(object sender, EventArgs e)
        {
            var settings = WBrowse.CoreWebView2.Settings;
            if (JavaEnableDisable_Btn.Text == "Javascript Enable")
            {
                JavaDisable_Lbl.Visible = true;
                JavaEnableDisable_Btn.Text = "Javascript Disable";
                settings.IsScriptEnabled = false;
            }
            else
            {
                JavaDisable_Lbl.Visible = false;
                JavaEnableDisable_Btn.Text = "Javascript Enable";
                settings.IsScriptEnabled = true;
            }
        }

        private void ArchiveDirectory_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter file_create = new StreamWriter(AppStart + "Archive-DB-FILES-FEED.bat"))
                {
                    file_create.WriteLine("@echo off");
                    file_create.WriteLine("echo ".PadRight(39, '-'));
                    file_create.WriteLine("echo          Ostium by ICAZA MEDIA");
                    file_create.WriteLine("echo ".PadRight(39, '-'));
                    file_create.WriteLine("@echo.");
                    file_create.WriteLine("echo Backup: DATABASE - FEED - FILES");
                    file_create.WriteLine("@echo.");
                    file_create.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + DBdirectory + " -mx9 -mtc=on");
                    file_create.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + FeedDir + " -mx9 -mtc=on");
                    file_create.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + FileDir + " -mx9 -mtc=on");
                    file_create.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + Workflow + " -mx9 -mtc=on");
                    file_create.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + Scripts + " -mx9 -mtc=on");
                    file_create.WriteLine("pause");
                }
                Process.Start(AppStart + "Archive-DB-FILES-FEED.bat");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ArchiveDirectory_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #endregion

        #region Tools_Tab_1

        private void OpnFileCategory_Btn_Click(object sender, EventArgs e)
        {
            if (CategorieFeed_Cbx.Text != "")
            {
                OpenFile_Editor(FeedDir + CategorieFeed_Cbx.Text);
            }
            else
            {
                CategorieFeed_Cbx.BackColor = Color.Red;
                MessageBox.Show("Select category first!");
                CategorieFeed_Cbx.BackColor = Color.FromArgb(41, 44, 51);
            }
        }

        private void CopyURLfeed_Mnu_Click(object sender, EventArgs e)
        {
            string textData = WBrowsefeed.Source.AbsoluteUri;
            Clipboard.SetData(DataFormats.Text, textData);
            Beep(1500, 400);
        }

        private void TraductPageFeed_Btn_Click(object sender, EventArgs e)
        {
            string formatURI = Regex.Replace(Class_Var.URL_TRAD_WEBPAGE, "VVVV0VVVV", WBrowsefeed.Source.AbsoluteUri);
            WBrowsefeed.Source = new Uri(@formatURI);
        }

        private void JavaEnableDisableFeed_Btn_Click(object sender, EventArgs e)
        {
            var settings = WBrowsefeed.CoreWebView2.Settings;
            if (JavaEnableDisableFeed_Btn.Text == "Javascript Enable")
            {
                JavaDisableFeed_Lbl.Visible = true;
                JavaEnableDisableFeed_Btn.Text = "Javascript Disable";
                settings.IsScriptEnabled = false;
            }
            else
            {
                JavaDisableFeed_Lbl.Visible = false;
                JavaEnableDisableFeed_Btn.Text = "Javascript Enable";
                settings.IsScriptEnabled = true;
            }
        }

        #endregion

        #region Tools_Tab_3

        private void NewProject_Tls_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void DeleteProject_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text == "")
                return;

            string message = "Do you want to delete the project? " + NameProjectwf_Txt.Text;
            string caption = "Delete Projet";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (File.Exists(Workflow + NameProjectwf_Txt.Text + ".xml"))
                    File.Delete(Workflow + NameProjectwf_Txt.Text + ".xml");

                if (File.Exists(Workflow + NameProjectwf_Txt.Text + ".ost"))
                    File.Delete(Workflow + NameProjectwf_Txt.Text + ".ost");

                Reset();

                loadfiledir.LoadFileDirectory(Workflow, "xml", "lst", ProjectOpn_Lst);
            }
        }

        private void ViewXml_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text == "")
                return;

            if (File.Exists(Workflow + NameProjectwf_Txt.Text + ".xml"))
            {
                WBrowse.Source = new Uri(Workflow + NameProjectwf_Txt.Text + ".xml");

                Tools_TAB_0.Visible = true;
                Tools_TAB_3.Visible = false;

                Control_Tab.SelectedIndex = 0;
            }                
        }

        private void EditXml_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text == "")
                return;

            if (File.Exists(Workflow + NameProjectwf_Txt.Text + ".xml"))
            {
                OpenFile_Editor(Workflow + NameProjectwf_Txt.Text + ".xml");
            }            
        }

        private void ExportXml_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                if (NameProjectwf_Txt.Text == "")
                    return;

                string dirselect = selectdir.Dirselect();

                if (dirselect != "")
                {
                    if (File.Exists(dirselect + @"\" + NameProjectwf_Txt.Text + ".xml"))
                    {
                        string message = "File exist continue?";
                        string caption = "File exist";
                        var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            goto Export;
                        }
                        else
                            return;
                    }
                Export:
                    File.Copy(Workflow + NameProjectwf_Txt.Text + ".xml", dirselect + @"\" + NameProjectwf_Txt.Text + ".xml", true);
                    Beep(1000, 400);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ExportXml_Tls_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void ExportJson_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                if (NameProjectwf_Txt.Text == "")
                    return;

                string dirselect = selectdir.Dirselect();

                if (dirselect != "")
                {
                    if (File.Exists(dirselect + @"\" + NameProjectwf_Txt.Text + ".json"))
                    {
                        string message = "File exist continue?";
                        string caption = "File exist";
                        var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            goto Export;
                        }
                        else
                            return;
                    }
                Export:
                    ConvertJson(Workflow + NameProjectwf_Txt.Text + ".xml", dirselect + @"\" + NameProjectwf_Txt.Text + ".json", 0);
                    Beep(1000, 400);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ExportJson_Tls_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void Diagram_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text == "")
                return;

            if (!File.Exists(DiagramDir + "plantuml.jar"))
            {
                MessageBox.Show("Sorry, PlantUML is not install, go to Discord channel Ostium for fix and help.");
                return;
            }                

            if (File.Exists(DiagramDir + NameProjectwf_Txt.Text + ".txt"))
                File.Delete(DiagramDir + NameProjectwf_Txt.Text + ".txt");

            MessageBox.Show(MessageStartDiagram);
            Timo.Enabled = true;

            ConvertJson(Workflow + NameProjectwf_Txt.Text + ".xml", DiagramDir + NameProjectwf_Txt.Text + ".txt", 1);

            FileDiag = NameProjectwf_Txt.Text + ".txt";

            Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd(NameProjectwf_Txt.Text + ".txt"));
            CreateDiagram.Start();
        }

        private void DiagramMindMap_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text == "")
                return;

            if (!File.Exists(DiagramDir + "plantuml.jar"))
            {
                MessageBox.Show("Sorry, PlantUML is not install, go to Discord channel Ostium for fix and help.");
                return;
            }

            MessageBox.Show(MessageStartDiagram);
            Timo.Enabled = true;

            ThemeDiag = ThemDiag_Cbx.Text;
            if (ThemeDiag == "")
                ThemeDiag = "cloudscape-design";

            Thread CreateDiagramMinMapFile = new Thread(() => CreateDiagramMinMapFile_Thrd(1));
            CreateDiagramMinMapFile.Start();
        }

        private void DiagramMindMap2_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text == "")
                return;

            if (!File.Exists(DiagramDir + "plantuml.jar"))
            {
                MessageBox.Show("Sorry, PlantUML is not install, go to Discord channel Ostium for fix and help.");
                return;
            }

            MessageBox.Show(MessageStartDiagram);
            Timo.Enabled = true;

            ThemeDiag = ThemDiag_Cbx.Text;
            if (ThemeDiag == "")
                ThemeDiag = "cloudscape-design";

            Thread CreateDiagramMinMapFile = new Thread(() => CreateDiagramMinMapFile_Thrd(0));
            CreateDiagramMinMapFile.Start();
        }

        private void CreateDiagram_Thrd(string fileselect)
        {
            try
            {
                string argumentsIs = "java -jar plantuml.jar " + DiagramDir + fileselect + " -tsvg -charset UTF-8";

                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = DiagramDir + "plantuml.jar";
                    proc.StartInfo.Arguments = argumentsIs;
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                    proc.Close();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateDiagram_Thrd: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void ConvertJson(string xmlFileConvert, string dirFileConvert, int value)
        {
            try
            {
                string ThemeDiag;
                ThemeDiag = ThemDiag_Cbx.Text;
                if (ThemeDiag == "")
                    ThemeDiag = "cloudscape-design";

                var xmlFile = xmlFileConvert;
                var doc = new XmlDocument();
                doc.Load(xmlFile);

                string json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);

                json = json.Replace("@", "");
                json = Regex.Replace(json, @"/\*.+?\*/", string.Empty);

                using (StreamWriter file_create = new StreamWriter(dirFileConvert))
                {
                    if (value == 1)
                    {
                        file_create.WriteLine("@startjson");
                        file_create.WriteLine("!theme " + ThemeDiag);
                    }

                    file_create.WriteLine(json);

                    if (value == 1)
                    {
                        file_create.Write("@endjson");
                    }

                    file_create.Close();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ConvertJson: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void CreateDiagramMinMapFile_Thrd(int value)
        {
            try
            {
                if (File.Exists(DiagramDir + NameProjectwf_Txt.Text + ".txt"))
                    File.Delete(DiagramDir + NameProjectwf_Txt.Text + ".txt");

                string element;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Workflow + NameProjectwf_Txt.Text + ".xml");

                using (StreamWriter file_create = File.AppendText(DiagramDir + NameProjectwf_Txt.Text + ".txt"))
                {
                    file_create.WriteLine("@startmindmap");
                    file_create.WriteLine("!theme " + ThemeDiag);

                    if (value == 0)
                    {
                        file_create.WriteLine("+ " + NameProjectwf_Txt.Text);
                    }
                    else if (value == 1)
                    {
                        file_create.WriteLine("* " + NameProjectwf_Txt.Text);
                    }
                }

                for (int i = 0; i < WorkflowItem_Lst.Items.Count; i++)
                {
                    if (WorkflowItem_Lst.Items[i].ToString() != "")
                    {
                        element = WorkflowItem_Lst.Items[i].ToString();
                        element += "_" + element;

                        using (StreamWriter file_create = File.AppendText(DiagramDir + NameProjectwf_Txt.Text + ".txt"))
                        {
                            if (value == 0)
                            {
                                file_create.WriteLine("++ " + WorkflowItem_Lst.Items[i].ToString());
                            }
                            else if (value == 1)
                            {
                                file_create.WriteLine("** " + WorkflowItem_Lst.Items[i].ToString());
                            }
                        }

                        XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Table/" + WorkflowItem_Lst.Items[i].ToString() + "/" + element);

                        for (int e = 0; e < nodeList.Count; e++)
                        {
                            string strvalue = string.Format("{0}", nodeList[e].ChildNodes.Item(0).InnerText);
                            string strnote = string.Format("{0}", nodeList[e].Attributes.Item(3).InnerText);

                            using (StreamWriter file_create = File.AppendText(DiagramDir + NameProjectwf_Txt.Text + ".txt"))
                            {
                                if (value == 0)
                                {
                                    file_create.WriteLine("+++ " + strvalue);
                                }
                                else if (value == 1)
                                {
                                    file_create.WriteLine("***:" + strnote);
                                    file_create.WriteLine("<code>");
                                    file_create.WriteLine(strvalue);
                                    file_create.WriteLine("</code>");
                                    file_create.WriteLine(";");
                                }
                            }
                        }
                    }
                }

                using (StreamWriter file_create = File.AppendText(DiagramDir + NameProjectwf_Txt.Text + ".txt"))
                {
                    file_create.Write("@endmindmap");
                }

                FileDiag = NameProjectwf_Txt.Text + ".txt";

                Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd(NameProjectwf_Txt.Text + ".txt"));
                CreateDiagram.Start();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateDiagramMinMapFile: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void OpnJsonFile_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(DiagramDir + "plantuml.jar"))
                {
                    MessageBox.Show("Sorry, PlantUML is not install, go to Discord channel Ostium for fix and help.");
                    return;
                }

                TmpFile_Txt.Text = "";

                string ThemeDiag;
                ThemeDiag = ThemDiag_Cbx.Text;
                if (ThemeDiag == "")
                    ThemeDiag = "cloudscape-design";

                string fileopen = openfile.Fileselect(AppStart, "json files (*.json)|*.json", 2);

                if (fileopen != "")
                {
                    MessageBox.Show(MessageStartDiagram);
                    Timo.Enabled = true;

                    string filePath = fileopen;

                    if (File.Exists(DiagramDir + "temp_file.txt"))
                        File.Delete(DiagramDir + "temp_file.txt");

                    StreamReader sr = new StreamReader(filePath);
                    TmpFile_Txt.Text = sr.ReadToEnd();
                    sr.Close();

                    using (StreamWriter file_create = new StreamWriter(DiagramDir + "temp_file.txt"))
                    {
                        file_create.WriteLine("@startjson");
                        file_create.WriteLine("!theme " + ThemeDiag);
                        file_create.WriteLine(TmpFile_Txt.Text);
                        file_create.Write("@endjson");
                    }

                    FileDiag = "temp_file.txt";

                    Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd("temp_file.txt"));
                    CreateDiagram.Start();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnJsonFile_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void OpnXMLFile_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(DiagramDir + "plantuml.jar"))
                {
                    MessageBox.Show("Sorry, PlantUML is not install, go to Discord channel Ostium for fix and help.");
                    return;
                }

                string fileopen = openfile.Fileselect(AppStart, "xml files (*.xml)|*.xml", 2);

                if (fileopen != "")
                {
                    MessageBox.Show(MessageStartDiagram);
                    Timo.Enabled = true;

                    if (File.Exists(DiagramDir + "temp_file.txt"))
                        File.Delete(DiagramDir + "temp_file.txt");

                    string filePath = fileopen;
                    ConvertJson(filePath, DiagramDir + "temp_file.txt", 1);

                    FileDiag = "temp_file.txt";

                    Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd("temp_file.txt"));
                    CreateDiagram.Start();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnXMLFile_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #endregion

        public void Control_Tab_Click(object sender, EventArgs e)
        {
            switch (Control_Tab.SelectedIndex)
            {
                case 0:
                    {
                        Tools_TAB_0.Visible = true;
                        Tools_TAB_1.Visible = false;
                        Tools_TAB_3.Visible = false;
                        Text = TmpTitleWBrowse;
                        URLtxt_txt.Text = WBrowse.Source.AbsoluteUri;
                        TableOpn_Lbl.Visible = true;
                        CountFeed_Lbl.Visible = false;
                        DBSelectOpen_Lbl.Visible = false;
                        TableCount_Lbl.Visible = false;
                        TableOpen_Lbl.Visible = false;
                        RecordsCount_Lbl.Visible = false;

                        if (JavaEnableDisable_Btn.Text == "Javascript Disable")
                        {
                            JavaDisable_Lbl.Visible = true;
                        }
                        
                        JavaDisableFeed_Lbl.Visible = false;
                        break;
                    }
                case 1:
                    {
                        Tools_TAB_0.Visible = false;
                        Tools_TAB_1.Visible = true;
                        Tools_TAB_3.Visible = false;
                        Text = TmpTitleWBrowsefeed;
                        URLtxt_txt.Text = WBrowsefeed.Source.AbsoluteUri;
                        TableOpn_Lbl.Visible = false;
                        CountFeed_Lbl.Visible = true;
                        DBSelectOpen_Lbl.Visible = false;
                        TableCount_Lbl.Visible = false;
                        TableOpen_Lbl.Visible = false;
                        RecordsCount_Lbl.Visible = false;

                        if (JavaEnableDisableFeed_Btn.Text == "Javascript Disable")
                        {
                            JavaDisableFeed_Lbl.Visible = true;
                        }

                        JavaDisable_Lbl.Visible = false;

                        NewCategory_Txt.ForeColor = Color.DimGray;
                        NewCategory_Txt.Text = "new category";
                        NewFeed_Txt.ForeColor = Color.DimGray;
                        NewFeed_Txt.Text = "new feed";                        
                        break;
                    }
                case 2:
                    {
                        Tools_TAB_0.Visible = false;
                        Tools_TAB_1.Visible = false;
                        Tools_TAB_3.Visible = false;
                        URLtxt_txt.Text = "";
                        Text = "DataBase Url";
                        TableOpn_Lbl.Visible = false;
                        CountFeed_Lbl.Visible = false;
                        JavaDisable_Lbl.Visible = false;
                        JavaDisableFeed_Lbl.Visible = false;
                        DBSelectOpen_Lbl.Visible = true;
                        TableCount_Lbl.Visible = true;
                        TableOpen_Lbl.Visible = true;
                        RecordsCount_Lbl.Visible = true;

                        ValueChange_Txt.ForeColor = Color.DimGray;
                        ValueChange_Txt.Text = "update URL and Name here";

                        loadfiledir.LoadFileDirectory(DBdirectory, "*", "lst", DataBaze_Lst);
                        break;
                    }
                case 3:
                    {
                        Tools_TAB_0.Visible = false;
                        Tools_TAB_1.Visible = false;
                        Tools_TAB_3.Visible = true;
                        URLtxt_txt.Text = "";
                        Text = "Workflow";
                        TableOpn_Lbl.Visible = false;
                        CountFeed_Lbl.Visible = false;
                        JavaDisable_Lbl.Visible = false;
                        JavaDisableFeed_Lbl.Visible = false;
                        DBSelectOpen_Lbl.Visible = false;
                        TableCount_Lbl.Visible = false;
                        TableOpen_Lbl.Visible = false;
                        RecordsCount_Lbl.Visible = false;
                        break;                        
                    }
                case 4:
                    Tools_TAB_0.Visible = false;
                    Tools_TAB_1.Visible = false;
                    Tools_TAB_3.Visible = false;
                    URLtxt_txt.Text = "";
                    Text = "Options";
                    TableOpn_Lbl.Visible = false;
                    CountFeed_Lbl.Visible = false;
                    JavaDisable_Lbl.Visible = false;
                    JavaDisableFeed_Lbl.Visible = false;
                    DBSelectOpen_Lbl.Visible = false;
                    TableCount_Lbl.Visible = false;
                    TableOpen_Lbl.Visible = false;
                    RecordsCount_Lbl.Visible = false;

                    DireSizeCalc(AppStart, OstiumDir_Lbl);
                    DireSizeCalc(Plugins, AddOnDir_Lbl);
                    DireSizeCalc(DBdirectory, DatabseDir_Lbl);
                    DireSizeCalc(FeedDir, FeedDir_Lbl);
                    DireSizeCalc(Scripts, ScriptDir_Lbl);
                    DireSizeCalc(Workflow, WorkFlowDir_Lbl);
                    DireSizeCalc(WorkflowModel, WorkFlowModelDir_Lbl);
                    DireSizeCalc(Pictures, PictureDir_Lbl);
                    break;
            }
        }

        public void Construct_URL(string URLc)
        {
            URLbrowse_Cbx.Items.Clear();

            for (int i = 0; i < ConstructURL_Lst.Items.Count; i++)
            {
                string formatURI = Regex.Replace(ConstructURL_Lst.Items[i].ToString(), "VVVV0VVVV", URLc);
                URLbrowse_Cbx.Items.Add(formatURI);
            }

            URLbrowse_Cbx.SelectedIndex = 0;
        }

        public async void Download_Source_Page()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd(@Class_Var.URL_USER_AGENT_SRC_PAGE);
                var response = await client.GetAsync(WBrowse.Source.AbsoluteUri);
                string pageContents = await response.Content.ReadAsStringAsync();

                File_Write(AppStart + "sourcepage", pageContents);
            }
            catch
            { }
        }

        public void CreateNameAleat()
        {
            UsenameAleatoire = DateTime.Now.ToString("d").Replace("/", "_") + "_" + DateTime.Now.ToString("HH:mm:ss").Replace(":", "_");
        }

        #region Console_

        public void OnKeyConsole_Cmd_Txt(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CMD_Console(Console_Cmd_Txt.Text);
                Console_Cmd_Txt.Text = "";
                Console_Cmd_Txt.Text = "> ";
                Console_Cmd_Txt.Select(Console_Cmd_Txt.Text.Length, 0);
            }
        }

        public void CMD_Console(string Cmd)
        {
            int yn = 0;
            int cmdSwitch = 0;
            string regxCmd = "";

            string s = Cmd;
            s = s.Replace(">", ""); s = s.Replace(" ", "");

            switch (s)
            {
                case "version":
                    {
                        MessageBox.Show(SoftVersion);
                        break;
                    }
                case "links":
                    {
                        Console_Cmd_Txt.Enabled = false;
                        cmdSwitch = 0;
                        yn = 1;
                        break;
                    }
                case "word":
                    {
                        Console_Cmd_Txt.Enabled = false;
                        cmdSwitch = 1;
                        yn = 1;
                        break;
                    }
                case "wordwd":
                    {
                        Console_Cmd_Txt.Enabled = false;
                        cmdSwitch = 3;
                        yn = 1;
                        break;
                    }
                case "help":
                    {
                        Open_Doc_Frm(FileDir + "cmdc.txt");
                        break;
                    }
                case "textlink":
                    {
                        Console_Cmd_Txt.Enabled = false;
                        cmdSwitch = 2;
                        yn = 1;
                        break;
                    }
                case "config":
                    {
                        OpenFile_Editor(AppStart + "config.xml");
                        break;
                    }
                case "id":
                    var browserInfo = WBrowse.CoreWebView2.BrowserProcessId;
                    MessageBox.Show(this, "Browser ID: " + browserInfo.ToString(), "Process ID");
                    break;
                case "exit":
                    Console_Cmd_Txt.Visible = false;
                    break;
                default:
                    {
                        MessageBox.Show("Command not recognized! Type help for more information.", "Error!");
                        return;
                    }
            }          

            if (yn == 1)
            {
                Thread Thr_CMDConsoleExec = new Thread(() => CMD_Console_Exec(cmdSwitch, regxCmd));
                Thr_CMDConsoleExec.Start();
            }                
        }

        public void CMD_Console_Exec(int cmdSwitch, string regxCmd)
        {
            StreamReader sr = new StreamReader(AppStart + "sourcepage");
            Invoke(new Action<string>(SRCpageAdd), "listclear");

            switch (cmdSwitch)
            {
                case 0: // links
                    {
                        Source_Page_Lst.Sorted = false;

                        foreach (Match match in Regex.Matches(sr.ReadToEnd(), @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*"))
                        {
                            Invoke(new Action<string>(SRCpageAdd), match.Value);
                        }
                        break;
                    }
                case 1: // word
                    {
                        Source_Page_Lst.Sorted = true;

                        foreach (Match match in Regex.Matches(sr.ReadToEnd(), @"\b[a-zA-ZÀ-ž]\w+"))
                        {
                            Invoke(new Action<string>(SRCpageAdd), match.Value);
                        }
                        break;
                    }
                case 2: // text link
                    {
                        Source_Page_Lst.Sorted = false;

                        string ex = @"<\s*a[^>]*>(?<valeur>([^<]*))</a>";
                        Regex regex = new Regex(ex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        MatchCollection resultats = regex.Matches(sr.ReadToEnd());
                        foreach (Match resultat in resultats)
                        {
                            Invoke(new Action<string>(SRCpageAdd), resultat.Groups["valeur"].Value);
                        }
                        break;
                    }
                case 3: // word without duplicate
                    Source_Page_Lst.Sorted = true;

                    foreach (Match match in Regex.Matches(sr.ReadToEnd(), @"\b[a-zA-ZÀ-ž]\w+"))
                    {
                        int x;
                        x = Source_Page_Lst.FindStringExact(match.Value);
                        if (x == -1)
                        {
                            Invoke(new Action<string>(SRCpageAdd), match.Value);
                        }
                    }
                    break;
                case 4: // regex
                    {
                        Source_Page_Lst.Sorted = true;

                        foreach (Match match in Regex.Matches(sr.ReadToEnd(), regxCmd))
                        {
                            Invoke(new Action<string>(SRCpageAdd), match.Value);
                        }
                        break;
                    }
                default:
                    {
                        MessageBox.Show("Command not recognized! Type help for more information.");
                        break;
                    }
            }

            Invoke(new Action<string>(SRCpageAdd), "listcreate");

            sr.Close();
        }

        #endregion 

        #region File_List_Create

        public void File_Write(string fileName, string content)
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
                senderror.ErrorLog("Error! File_Write: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void List_Create(string nameFile, string opnOrNo)
        {
            try
            {
                if (File.Exists(nameFile))
                {
                    File.Delete(nameFile);
                }

                var ListElements = nameFile;
                using (StreamWriter SW = new StreamWriter(ListElements, true))
                {
                    foreach (string itm in List_Object.Items)
                    {
                        SW.WriteLine(itm);
                    }
                }

                if (opnOrNo == "yes")
                {
                    Open_Source_Frm(nameFile);
                }
                else
                {
                    Beep(1200, 200);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! List_Create: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void CreateData(string fileName, string fileValue)
        {
            try
            {
                using (StreamWriter file_create = File.AppendText(fileName))
                {
                    file_create.WriteLine(fileValue);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateData: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void OpnFileOpt(string dir_dir)
        {
            if (!File.Exists(AppStart + "OstiumE.exe"))
            {
                MessageBox.Show("The OstiumE editor is missing!", "Missing editor");
                return;
            }

            if (!File.Exists(dir_dir))
            {
                File_Write(dir_dir, "");
            }
            OpenFile_Editor(dir_dir);
        }

        #endregion

        public void OpenFile_Editor(string FileSelect)
        {
            try
            {
                if (!File.Exists(AppStart + "OstiumE.exe"))
                {
                    MessageBox.Show("The OstiumE editor is missing!", "Missing editor");
                    return;
                }

                if (FileSelect != "")
                {
                    if (File.Exists(FileSelect))
                    {
                        using (Process objProcess = new Process())
                        {
                            objProcess.StartInfo.FileName = AppStart + "OstiumE.exe";
                            objProcess.StartInfo.Arguments = "/input=\"" + FileSelect + "\"";
                            objProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                            objProcess.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenFile_Editor: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Open_Doc_Frm(string fileNameSelect)
        {
            Class_Var.File_Open = fileNameSelect;
            docForm = new Doc_Frm();
            docForm.Show();
        }

        public void Open_Source_Frm(string fileNameSelect)
        {
            Class_Var.File_Open = fileNameSelect;
            openSourceForm = new OpenSource_Frm();
            openSourceForm.Show();
        }

        #region Database_OpenAdd

        public void URL_SAVE_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Cbx_Object = URL_SAVE_Cbx;
                string usct = URL_SAVE_Cbx.Text;

                Sqlite_ReadUri("SELECT * FROM " + TableOpen + " WHERE url_name = '" + usct + "'", "url_adress");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! URL_SAVE_Cbx_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void AddURLink_Btn_Click(object sender, EventArgs e)
        {
            Tables_Lst_Opt = "add";
            DataAddSelectUri = WBrowse.Source.AbsoluteUri;
            DatabasePnl();
            URLadd_Lbl.Visible = true;
            label8.Visible = true;
            TableName_Txt.Visible = true;
            AddTable_Btn.Visible = true;
            label7.Visible = true;
            UrlName_Txt.Visible = true;
        }

        public void OpnURL_Btn_Click(object sender, EventArgs e)
        {
            Tables_Lst_Opt = "open";
            DatabasePnl();
            label8.Visible = false;
            TableName_Txt.Visible = false;
            AddTable_Btn.Visible = false;
            label7.Visible = false;
            UrlName_Txt.Visible = false;
            URLadd_Lbl.Visible = false;
        }

        public void DatabasePnl()
        {
            try
            {
                if (DB_Pnl.Visible == false)
                {
                    DB_Pnl.BringToFront();                    
                    int PtX = Width - 370;
                    int PtY = 3;
                    DB_Pnl.Location = new Point(PtX, PtY);
                    DB_Pnl.Visible = true;

                    UrlName_Txt.Text = NameUriDB;
                    URLadd_Lbl.Text = WBrowse.Source.AbsoluteUri;

                    OpnAllTable();
                }
                else
                {
                    DB_Pnl.Visible = false;
                    DB_Pnl.SendToBack();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DatabasePnl: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void AddTable_Btn_Click(object sender, EventArgs e)
        {
            TableName_Txt.Text = Regex.Replace(TableName_Txt.Text, "[^a-zA-Z]", "");

            if (TableName_Txt.Text != "")
            {
                Sqlite_Cmd("CREATE TABLE IF NOT EXISTS " + TableName_Txt.Text + " (url_date TEXT, url_name TEXT, url_adress TEXT)");
                TableName_Txt.Text = "";
            }
            else
            {
                MessageBox.Show("Insert name Table first!", "");
            }
        }

        public void Tables_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Tables_Lst.SelectedIndex != -1)
            {
                tlsi = Tables_Lst.SelectedItem.ToString();

                Cbx_Object = URL_SAVE_Cbx;
                Cbx_Object.Text = "";
                Cbx_Object.Items.Clear();

                if (Tables_Lst_Opt == "add")
                {
                    UrlName_Txt.Text = Regex.Replace(UrlName_Txt.Text, "[^\\w\\\x20_-]", "");

                    if (UrlName_Txt.Text != "")
                    {
                        Sqlite_Read("SELECT * FROM " + tlsi + "", "url_name", "cbx");

                        int x;
                        x = Cbx_Object.FindStringExact(UrlName_Txt.Text);
                        if (x == -1)
                        {
                            Sqlite_Cmd("INSERT INTO " + tlsi + " (url_date, url_name, url_adress) VALUES ('" + DateTime.Now.ToString("d") + "', '" + UrlName_Txt.Text + "', '" + DataAddSelectUri + "')");

                            UrlName_Txt.Text = "";
                            Cbx_Object.Text = "";
                            Cbx_Object.Items.Clear();

                            Sqlite_Read("SELECT * FROM " + tlsi + "", "url_name", "cbx");

                            DB_Pnl.Visible = false;
                        }
                        else
                        {
                            UrlName_Txt.BackColor = Color.Red;
                            MessageBox.Show("Name Url exist!");
                            UrlName_Txt.BackColor = Color.FromArgb(28, 28, 28);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Insert name URi first!", "");
                    }
                }
                else
                {
                    Sqlite_Read("SELECT * FROM " + tlsi + "", "url_name", "cbx");
                    DB_Pnl.Visible = false;
                }

                TableOpn_Lbl.Text = string.Format("Table open: {0}", tlsi);
                TableOpen = tlsi;
            }
        }

        public void OpnAllTable()
        {
            Tables_Lst.Items.Clear();
            List_Object = Tables_Lst;

            Sqlite_Read("SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1", "name", "lst");
        }

        public void Sqlite_Cmd(string execSql)
        {
            try
            {
                SQLiteConnection myDB;
                myDB = new SQLiteConnection("Data Source=" + D4ta + ";Version=3;");
                myDB.Open();

                string sql = execSql;
                SQLiteCommand commande = new SQLiteCommand(sql, myDB);
                commande.ExecuteNonQuery();

                myDB.Close();

                if (DBadmin == "off")
                {
                    OpnAllTable();

                    Beep(1000, 400);
                }
                DBadmin = "off";
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Sqlite_Cmd: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Sqlite_Read(string execSql, string valueDB, string ObjLstCbx)
        {
            try
            {
                SQLiteConnection myDB;
                myDB = new SQLiteConnection("Data Source=" + D4ta + ";Version=3;");
                myDB.Open();

                string sql = execSql;
                SQLiteCommand commande = new SQLiteCommand(sql, myDB);

                SQLiteDataReader reader = commande.ExecuteReader();

                while (reader.Read())
                {
                    if (ObjLstCbx == "lst")
                    {
                        List_Object.Items.Add(reader[valueDB]);
                    }
                    else
                    {
                        Cbx_Object.Items.Add(reader[valueDB]);
                    }
                }

                myDB.Close();

                if (List_Object == DataTable_Lst)
                    TableCount_Lbl.Text = "table " + Convert.ToString(List_Object.Items.Count);

                if (List_Object == DataValue_Lst)
                    RecordsCount_Lbl.Text = "records " + Convert.ToString(List_Object.Items.Count);

                DBadmin = "off";
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Sqlite_Read: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Sqlite_ReadUri(string execSql, string valueDB)
        {
            try
            {
                SQLiteConnection myDB;
                myDB = new SQLiteConnection("Data Source=" + D4ta + ";Version=3;");
                myDB.Open();

                string sql = execSql;
                SQLiteCommand commande = new SQLiteCommand(sql, myDB);

                SQLiteDataReader reader = commande.ExecuteReader();

                while (reader.Read())
                {
                    if (DBadmin == "off")
                    {
                        WBrowse.Source = new Uri((string)reader[valueDB]);
                    }
                    else
                    {
                        DataValue_Opn.Text = (string)reader[valueDB];
                    }
                }

                myDB.Close();

                DBadmin = "off";
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Sqlite_ReadUri: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #endregion

        #region Database_Organiz

        public void ChangeDefDB_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaze_Opn.Text != "")
                {
                    if (File.Exists(DBdirectory + DataBaze_Opn.Text))
                    {
                        const string message = "Change default Database?";
                        const string caption = "Database default";
                        var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            ChangeDBdefault(DataBaze_Opn.Text);
                            D4ta = DBdirectory + DataBaze_Opn.Text;
                            DB_Default_Txt.Text = DataBaze_Opn.Text;
                        }
                    }
                    else
                    {
                        const string message = "The Database not exist, create first!";
                        const string caption = "Database default";
                        var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            DataBaze_Opn.Text = Regex.Replace(DataBaze_Opn.Text, "[^a-zA-Z0-9]", "");
                            DataBaze_Opn.Text += ".db";

                            SQLiteConnection.CreateFile(DBdirectory + DataBaze_Opn.Text);
                            ChangeDBdefault(DataBaze_Opn.Text);
                            D4ta = DBdirectory + DataBaze_Opn.Text;
                            DB_Default_Txt.Text = DataBaze_Opn.Text;

                            loadfiledir.LoadFileDirectory(DBdirectory, "*", "lst", DataBaze_Lst);
                        }
                    }
                }
                else
                {
                    DataBaze_Opn.BackColor = Color.Red;
                    MessageBox.Show("Enter name first!");
                    DataBaze_Opn.BackColor = Color.FromArgb(28, 28, 28);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ChangeDefDB_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void ExecuteCMDsql_Btn_Click(object sender, EventArgs e)
        {
            DataValue_Lst.Items.Clear();
            List_Object = DataValue_Lst;

            Sqlite_Read(ExecuteCMDsql_Txt.Text, ValueCMDsql_Txt.Text, "lst");
        }

        public void Db_Delete_Table_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataTable_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    tlsi = DataTable_Opn.Text;

                    string message = "Are you sure that you would like to delete the Database? " + tlsi;
                    const string caption = "Database delete";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Sqlite_Cmd("DROP TABLE [" + tlsi + "];");

                        DataTable_Lst.Items.Clear();
                        DataValue_Lst.Items.Clear();
                        List_Object = DataTable_Lst;

                        Sqlite_Read("SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1", "name", "lst");

                        DataTable_Opn.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("No data select!");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Db_Delete_Table_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Db_Delete_Table_Value_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataValue_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    string message = "Are you sure that you would like to delete the entry? " + DataValue_Lst.Text;
                    const string caption = "Database delete";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        tlsi = DataTable_Opn.Text;
                        string usct = DataValue_Lst.Text;

                        Sqlite_ReadUri("DELETE FROM " + tlsi + " WHERE url_name = '" + usct + "'", "url_adress");

                        DataValue_Lst.Items.Clear();
                        List_Object = DataValue_Lst;
                        tlsi = DataTable_Opn.Text;

                        Sqlite_Read("SELECT * FROM " + tlsi + "", "url_name", "lst");

                        DataValue_Opn.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("No data select!");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Db_Delete_Table_Value_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Db_Delete_Table_AllValue_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataTable_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    tlsi = DataTable_Opn.Text;

                    string message = "Are you sure that you would like to delete all entry? " + tlsi;
                    const string caption = "Database delete";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Sqlite_Cmd("DELETE FROM [" + tlsi + "];");

                        DataValue_Lst.Items.Clear();
                        DataValue_Opn.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("No data select!");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Db_Delete_Table_AllValue_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Db_Update_Name_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                ValueChange_Txt.Text = Regex.Replace(ValueChange_Txt.Text, "[^\\w\\\x20_-]", "");

                if (DataValue_Lst.SelectedIndex != -1)
                {
                    if (ValueChange_Txt.Text != "" && ValueChange_Txt.Text != "update URL and Name here")
                    {
                        DBadmin = "on";

                        string usct = DataValue_Lst.Text;
                        tlsi = DataTable_Opn.Text;

                        string message = "Are you sure that you update entry? \r\n" + DataValue_Lst.Text + " <=> " + ValueChange_Txt.Text;
                        const string caption = "Modify update";
                        var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            int x;
                            x = DataValue_Lst.FindStringExact(ValueChange_Txt.Text);
                            if (x == -1)
                            {
                                Sqlite_Cmd("UPDATE " + tlsi + " SET url_name = '" + ValueChange_Txt.Text + "' WHERE url_name = '" + usct + "';");

                                DataValue_Lst.Items.Clear();
                                List_Object = DataValue_Lst;
                                tlsi = DataTable_Lst.SelectedItem.ToString();

                                DataTable_Opn.Text = DataTable_Lst.SelectedItem.ToString();

                                Sqlite_Read("SELECT * FROM " + tlsi + "", "url_name", "lst");

                                ValueChange_Txt.ForeColor = Color.DimGray;
                                ValueChange_Txt.Text = "update URL and Name here";
                            }
                            else
                            {
                                ValueChange_Txt.BackColor = Color.Red;
                                MessageBox.Show("Name Url exist!");
                                ValueChange_Txt.BackColor = Color.FromArgb(28, 28, 28);
                            }
                        }
                    }
                    else
                    {
                        ValueChange_Txt.BackColor = Color.Red;
                        MessageBox.Show("Select Name first!");
                        ValueChange_Txt.BackColor = Color.FromArgb(28, 28, 28);
                    }
                }
                else
                {
                    MessageBox.Show("No data select!");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Db_Update_Name_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void Db_Update_Value_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataValue_Lst.SelectedIndex != -1)
                {
                    if (ValueChange_Txt.Text != "" && ValueChange_Txt.Text != "update URL and Name here")
                    {
                        DBadmin = "on";

                        string usct = DataValue_Opn.Text;
                        tlsi = DataTable_Opn.Text;

                        string message = "Are you sure that you modify entry? \r\n" + DataValue_Opn.Text + " <=> " + ValueChange_Txt.Text;
                        const string caption = "Modify entry";
                        var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            Sqlite_Cmd("UPDATE " + tlsi + " SET url_adress = '" + ValueChange_Txt.Text + "' WHERE url_adress = '" + usct + "';");

                            ValueChange_Txt.ForeColor = Color.DimGray;
                            ValueChange_Txt.Text = "update URL and Name here";
                        }
                    }
                    else
                    {
                        ValueChange_Txt.BackColor = Color.Red;
                        MessageBox.Show("URL is not valid!");
                        ValueChange_Txt.BackColor = Color.FromArgb(28, 28, 28);
                    }
                }
                else
                {
                    MessageBox.Show("No data select!");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Db_Update_Value_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void DataBaze_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataBaze_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    DataTable_Opn.Text = "";
                    DataValue_Name.Text = "";
                    DataValue_Opn.Text = "";
                    ValueChange_Txt.ForeColor = Color.DimGray;
                    ValueChange_Txt.Text = "update URL and Name here";

                    DataValue_Lst.Items.Clear();
                    DataTable_Lst.Items.Clear();
                    List_Object = DataTable_Lst;

                    DataBaze_Opn.Text = DataBaze_Lst.SelectedItem.ToString();
                    DBSelectOpen_Lbl.Text = "DB open: " + DataBaze_Opn.Text;

                    D4ta = DBdirectory + DataBaze_Opn.Text;

                    Sqlite_Read("SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1", "name", "lst");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DataBaze_Lst_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void DataTable_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataTable_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    DataValue_Name.Text = "";
                    DataValue_Opn.Text = "";
                    ValueChange_Txt.ForeColor = Color.DimGray;
                    ValueChange_Txt.Text = "update URL and Name here";

                    DataValue_Lst.Items.Clear();
                    List_Object = DataValue_Lst;
                    tlsi = DataTable_Lst.SelectedItem.ToString();

                    DataTable_Opn.Text = DataTable_Lst.SelectedItem.ToString();
                    TableOpen_Lbl.Text = "Table open: " + DataTable_Opn.Text;

                    Sqlite_Read("SELECT * FROM " + tlsi + "", "url_name", "lst");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DataTable_Lst_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void DataValue_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataValue_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    tlsi = DataTable_Opn.Text;
                    string usct = DataValue_Lst.Text;
                    DataValue_Name.Text = DataValue_Lst.Text;

                    Sqlite_ReadUri("SELECT * FROM " + tlsi + " WHERE url_name = '" + usct + "'", "url_adress");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DataValue_Lst_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void ChangeDBdefault(string NameDB)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(AppStart + "config.xml");
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Xwparsingxml/Xwparsingnode/DB_USE_DEFAULT") is XmlElement node1)
                {
                    node1.InnerText = NameDB;
                }

                xmlReader.Close();
                doc.Save(AppStart + "config.xml");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ChangeDBdefault: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void ValueChange_Txt_Click(object sender, EventArgs e)
        {
            ValueChange_Txt.ForeColor = Color.White;
            ValueChange_Txt.Text = "";
        }

        #endregion

        #region Feed_

        private void Title_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Title_Lst.SelectedIndex != -1)
            {               
                WBrowsefeed.Source = new Uri(Link_Lst.Items[Title_Lst.SelectedIndex].ToString());
            }
        }

        public void CategorieFeed_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ManageFeed == "on")
            {
                ListFeed_Lst.Items.Clear();
                ListFeed_Lst.Items.AddRange(File.ReadAllLines(FeedDir + CategorieFeed_Cbx.Text));
                return;
            }

            if (File.Exists(FeedDir + CategorieFeed_Cbx.Text))
            {
                CategorieFeed_Cbx.Enabled = false;
                CreatCategorie_Btn.Enabled = false;
                AddFeed_Btn.Enabled = false;
                CollapseTitleFeed_Btn.Enabled = false;
                HomeFeed_Btn.Enabled = false;
                ManageFeed_Btn.Enabled = false;
                ToolsFeed_Mnu.Enabled = false;
                LangSelect_Lst.Enabled = false;
                ReadTitle_Btn.Enabled = false;
                PauseSpeak_Btn.Enabled = false;
                StopSpeak_Btn.Enabled = false;
                GoFeed_Btn.Enabled = false;
                GoFeed_Txt.Enabled = false;

                ListFeed_Lst.Items.Clear();
                ListFeed_Lst.Items.AddRange(File.ReadAllLines(FeedDir + CategorieFeed_Cbx.Text));

                Title_Lst.Items.Clear();
                Link_Lst.Items.Clear();

                MSenter();

                Thread Thr_RssFeed = new Thread(() => LoadFeed(0));
                Thr_RssFeed.Start();

                splitContain_Rss.Panel1Collapsed = false;
                CollapseTitleFeed_Btn.Text = "Collapse On";
            }
        }

        private void CountBlockSite_Lbl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CountBlockSite_Lbl.SelectedIndex != -1)
            {
                CountBlockFeed_Lbl.Items[CountBlockSite_Lbl.SelectedIndex].ToString();
                int value = Convert.ToInt32(CountBlockFeed_Lbl.Items[CountBlockSite_Lbl.SelectedIndex].ToString());

                Title_Lst.SetSelected(value, true);
            }
        }

        private void LoadFeed(int value)
        {
            string URL = "";
            int i = value;

            if (i == 0)
            {
                Invoke(new Action<string>(CountBlockSite_Invk), "Clear");
            }

            try
            {
                Invoke(new Action<string>(CountBlockSite_Invk), "Disable");

                for (i = value; i < ListFeed_Lst.Items.Count; i++)
                {
                    URL = ListFeed_Lst.Items[i].ToString();
                    using (XmlReader reader = XmlReader.Create(ListFeed_Lst.Items[i].ToString()))
                    {
                        SyndicationFeed feed = SyndicationFeed.Load(reader);

                        TitleFeed = feed.Title.Text;
                        Invoke(new Action<string>(CountBlockSite_Invk), "FeedTitle");
                        Invoke(new Action<string>(CountBlockSite_Invk), "ListCount");

                        foreach (SyndicationItem item in feed.Items)
                        {
                            AddTitleItem = item.Title.Text;
                            Invoke(new Action<string>(CountBlockSite_Invk), "ItemTitleAdd");
                            AddLinkItem = Convert.ToString(item.Links[0].Uri);
                            Invoke(new Action<string>(CountBlockSite_Invk), "ItemLinkAdd");
                        }
                    }
                }

                Invoke(new Action<string>(CountBlockSite_Invk), "CountFeed");
                Invoke(new Action<string>(CountBlockSite_Invk), "AllTrue");
                Invoke(new Action<string>(CountBlockSite_Invk), "Msleave");
            }
            catch
            {
                MessageBox.Show("Feed error loading! " + URL);
                LoadFeed(i += 1);
            }
        }

        private void CreatCategorie_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (NewCategory_Txt.Text != "" && NewCategory_Txt.Text != "new category")
                {
                    if (!File.Exists(FeedDir + NewCategory_Txt.Text))
                    {
                        File_Write(FeedDir + NewCategory_Txt.Text, "");

                        CategorieFeed_Cbx.Items.Clear();
                        loadfiledir.LoadFileDirectory(FeedDir, "*", "cbxts", CategorieFeed_Cbx);

                        CategorieFeed_Cbx.Text = "";
                        NewCategory_Txt.ForeColor = Color.DimGray;
                        NewCategory_Txt.Text = "new category";

                        MessageBox.Show("Category create.");
                    }
                    else
                    {
                        MessageBox.Show("Category exists!");
                    }
                }
                else
                {
                    NewCategory_Txt.BackColor = Color.Red;
                    MessageBox.Show("Insert category name!");
                    NewCategory_Txt.BackColor = Color.FromArgb(41, 44, 51);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreatCategorie_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void NewCategory_Txt_Click(object sender, EventArgs e)
        {
            NewCategory_Txt.ForeColor = Color.White;
            NewCategory_Txt.Text = "";
        }

        private void NewFeed_Txt_Click(object sender, EventArgs e)
        {
            NewFeed_Txt.ForeColor = Color.White;
            NewFeed_Txt.Text = "";
        }

        private void AddFeed_Btn_Click(object sender, EventArgs e)
        {
            if (NewFeed_Txt.Text != "" && NewFeed_Txt.Text != "new feed")
            {
                if (CategorieFeed_Cbx.Text != "")
                {
                    using (StreamWriter file_create = File.AppendText(FeedDir + CategorieFeed_Cbx.Text))
                    {
                        file_create.WriteLine(NewFeed_Txt.Text);
                    }

                    CategorieFeed_Cbx.Items.Clear();
                    loadfiledir.LoadFileDirectory(FeedDir, "*", "cbxts", CategorieFeed_Cbx);

                    CategorieFeed_Cbx.Text = "";
                    NewFeed_Txt.ForeColor = Color.DimGray;
                    NewFeed_Txt.Text = "new feed";
                    MessageBox.Show("Feed create.");
                }
                else
                {
                    CategorieFeed_Cbx.BackColor = Color.Red;
                    MessageBox.Show("Select category first!");
                    CategorieFeed_Cbx.BackColor = Color.FromArgb(41, 44, 51);
                }
            }
            else
            {
                NewFeed_Txt.BackColor = Color.Red;
                MessageBox.Show("Insert new feed!");
                NewFeed_Txt.BackColor = Color.FromArgb(41, 44, 51);
            }
        }

        private void ManageFeed_Btn_Click(object sender, EventArgs e)
        {
            if (ManageFeed == "off")
            {
                ManageFeed = "on";
                ManageFeed_Btn.Text = "Manage end";

                splitContain_Rss.Visible = false;
                CreatCategorie_Btn.Enabled = false;
                AddFeed_Btn.Enabled = false;
                CollapseTitleFeed_Btn.Enabled = false;
                HomeFeed_Btn.Enabled = false;
                ToolsFeed_Mnu.Enabled = false;

                ListFeed_Lst.Visible = true;
                ListFeed_Lst.Dock = DockStyle.Fill;
                DeleteURLfeed_Btn.Enabled = true;
                DeleteCatfeed_Btn.Enabled = true;
                DeleteURLfeed_Btn.Visible = true;
                DeleteCatfeed_Btn.Visible = true;
                Separator4.Visible = true;
                Separator5.Visible = true;
            }
            else
            {
                ManageFeed = "off";
                ManageFeed_Btn.Text = "Manage feed";

                splitContain_Rss.Visible = true;
                CreatCategorie_Btn.Enabled = true;
                AddFeed_Btn.Enabled = true;
                CollapseTitleFeed_Btn.Enabled = true;
                HomeFeed_Btn.Enabled = true;
                ToolsFeed_Mnu.Enabled = true;

                ListFeed_Lst.Visible = false;
                ListFeed_Lst.Dock = DockStyle.None;
                DeleteURLfeed_Btn.Enabled = false;
                DeleteCatfeed_Btn.Enabled = false;
                DeleteURLfeed_Btn.Visible = false;
                DeleteCatfeed_Btn.Visible = false;
                Separator4.Visible = false;
                Separator5.Visible = false;
            }
        }

        private void DeleteCatfeed_Btn_Click(object sender, EventArgs e)
        {
            try 
            {
                if (CategorieFeed_Cbx.Text != "")
                {
                    string message = "Are you sure that you would like to delete the category and all content? " + CategorieFeed_Cbx.Text;
                    const string caption = "Delete Category";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (File.Exists(FeedDir + CategorieFeed_Cbx.Text))
                        {
                            File.Delete(FeedDir + CategorieFeed_Cbx.Text);

                            CategorieFeed_Cbx.Text = "";
                            CategorieFeed_Cbx.Items.Clear();
                            loadfiledir.LoadFileDirectory(FeedDir, "*", "cbxts", CategorieFeed_Cbx);
                        }
                    }
                }
                else
                {
                    CategorieFeed_Cbx.BackColor = Color.Red;
                    MessageBox.Show("Select category first!");
                    CategorieFeed_Cbx.BackColor = Color.FromArgb(41, 44, 51);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DeleteCatfeed_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void DeleteURLfeed_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ListFeed_Lst.SelectedIndex != -1)
                {
                    string message = "Are you sure that you would like to delete the URL? => " + ListFeed_Lst.SelectedItem.ToString();
                    const string caption = "Suppress URL";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        ListFeed_Lst.Items.Remove(ListFeed_Lst.SelectedItem);
                        File.Delete(FeedDir + CategorieFeed_Cbx.Text);

                        var CategoryFile = FeedDir + CategorieFeed_Cbx.Text;
                        using (StreamWriter SW = new StreamWriter(CategoryFile, true))
                        {
                            foreach (string itm in ListFeed_Lst.Items)
                                SW.WriteLine(itm);
                        }

                        CategorieFeed_Cbx.Items.Clear();
                        loadfiledir.LoadFileDirectory(FeedDir, "*", "cbxts", CategorieFeed_Cbx);
                    }
                }
                else
                {
                    MessageBox.Show("Select URL first!");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DeleteURLfeed_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void CollapseTitleFeed_Btn_Click(object sender, EventArgs e)
        {
            if (splitContain_Rss.Panel1Collapsed == false)
            {
                CollapseTitleFeed_Btn.Text = "Collapse Off";
                splitContain_Rss.Panel1Collapsed = true;
            }
            else
            {
                CollapseTitleFeed_Btn.Text = "Collapse On";
                splitContain_Rss.Panel1Collapsed = false;
            }
        }

        private void GoFeed_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Title_Lst.Items.Count > 0)
                {
                    if (GoFeed_Txt.Text != "" && GoFeed_Txt.Text != "0")
                    {
                        int icr = Convert.ToInt32(GoFeed_Txt.Text);

                        if (icr <= Title_Lst.Items.Count)
                        {
                            Title_Lst.SetSelected(icr - 1, true);
                        }
                        else
                        {
                            MessageBox.Show("Max count title = " + Title_Lst.Items.Count);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! GoFeed_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }          
        }

        private void GoFeed_Txt_Click(object sender, EventArgs e)
        {
            GoFeed_Txt.Text = "";
        }

        private void HomeFeed_Btn_Click(object sender, EventArgs e)
        {
            WBrowsefeed.Source = new Uri(@Class_Var.URL_HOME);
            CountFeed_Lbl.Text = "";
        }

        #endregion

        #region FeedSpeak

        private void SpeakOpenPnl_Btn_Click(object sender, EventArgs e)
        {
            if (!Speak_Pnl.Visible)
            {
                Speak_Pnl.Visible = true;
                
                if (VerifLangOpn == 0)
                {
                    LoadLang();
                }

                VolumeValue_Lbl.Text = "Volume: " + Convert.ToString(VolumeVal_Track.Value);
                RateValue_Lbl.Text = "Rate: " + Convert.ToString(RateVal_Track.Value);
            }
            else
            {
                Speak_Pnl.Visible = false;
            }
        }

        private void ReadTitle_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (CategorieFeed_Cbx.Text != "")
                {
                    synth.SetOutputToDefaultAudioDevice();
                    synth.Volume = Class_Var.VOLUME_TRACK;
                    synth.Rate = Class_Var.RATE_TRACK;                    
                    synth.SelectVoice(LangSelect_Lst.SelectedItem.ToString());

                    for (int i = 0; i < Link_Lst.Items.Count; i++)
                    {
                        synth.SpeakAsync("Title " + Convert.ToString(i+1) + ": " + Title_Lst.Items[i].ToString());
                    }
                }
            }
            catch
            {}
        }

        private void ReadClipB_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (CategorieFeed_Cbx.Text != "")
                {
                    synth.SetOutputToDefaultAudioDevice();
                    synth.Volume = Class_Var.VOLUME_TRACK;
                    synth.Rate = Class_Var.RATE_TRACK;
                    synth.SelectVoice(LangSelect_Lst.SelectedItem.ToString());
                    synth.SpeakAsync(Clipboard.GetText(TextDataFormat.Text));
                }
            }
            catch
            { }
        }

        private void PauseSpeak_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (PauseSpeak_Btn.Text == "Pause")
                {
                    PauseSpeak_Btn.Text = "Resume";
                    synth.Pause();
                }
                else
                {
                    PauseSpeak_Btn.Text = "Pause";
                    synth.Volume = Class_Var.VOLUME_TRACK;
                    synth.Rate = Class_Var.RATE_TRACK;
                    synth.Resume();
                }
            }
            catch
            { }
        }

        private void StopSpeak_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                synth.SpeakAsyncCancelAll();
                PauseSpeak_Btn.Text = "Pause";
            }
            catch
            { }
        }

        private void VolumeVal_Track_Scroll(object sender, EventArgs e)
        {
            Class_Var.VOLUME_TRACK = VolumeVal_Track.Value;
            VolumeValue_Lbl.Text = "Volume: " + Convert.ToString(VolumeVal_Track.Value);
        }

        private void RateVal_Track_Scroll(object sender, EventArgs e)
        {
            Class_Var.RATE_TRACK = RateVal_Track.Value;
            RateValue_Lbl.Text = "Rate: " + Convert.ToString(RateVal_Track.Value);
        }

        private void SaveVolRat_Btn_Click(object sender, EventArgs e)
        {
            SaveVolumeRate("VOLUME_TRACK_VAR", Class_Var.VOLUME_TRACK);
            SaveVolumeRate("RATE_TRACK_VAR", Class_Var.RATE_TRACK);

            Beep(800, 200);
        }

        private void LoadLang()
        {
            try
            {
                VerifLangOpn = 1;
                synth.SetOutputToDefaultAudioDevice();
                var VoiceInstall = synth.GetInstalledVoices();

                foreach (InstalledVoice v in VoiceInstall)
                    LangSelect_Lst.Items.Add(v.VoiceInfo.Name);

                LangSelect_Lst.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadLang: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void SaveVolumeRate(string nodeselect, int value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(AppStart + "config.xml");
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Xwparsingxml/Xwparsingnode/" + nodeselect) is XmlElement node1)
                {
                    node1.InnerText = Convert.ToString(value);
                }

                xmlReader.Close();
                doc.Save(AppStart + "config.xml");                
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ChangeDBdefault: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #endregion

        private void CountBlockSite_Lbl_MouseEnter(object sender, EventArgs e)
        {
            MSenter();
        }

        private void CountBlockSite_Lbl_Leave(object sender, EventArgs e)
        {
            MSleave();
        }

        private void MSenter()
        {
            CountBlockSite_Lbl.BringToFront();
            CountBlockSite_Lbl.Width = 250;
            CountBlockSite_Lbl.Visible = true;
        }

        private void MSleave()
        {
            CountBlockSite_Lbl.SendToBack();
            CountBlockSite_Lbl.Width = 50;
        }

        #region Param_

        private void Download_Param_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Text = "edge://downloads";
            GoBrowser(0);
        }

        private void History_Param_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Text = "edge://history";
            GoBrowser(0);
        }

        private async void DevTools_Param_Click(object sender, EventArgs e)
        {
            await WBrowse.EnsureCoreWebView2Async();
            WBrowse.CoreWebView2.OpenDevToolsWindow();
        }

        private void EdgeURL_Param_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Text = "edge://edge-urls";
            GoBrowser(0);
        }

        private void AdvancedOption_Param_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Text = "about:flags";
            GoBrowser(0);
        }

        private void SiteEngament_Param_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Text = "edge://site-engagement";
            GoBrowser(0);
        }

        private void System_Param_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Text = "edge://system";
            GoBrowser(0);
        }

        #endregion

        public async void GetCookie(string URLs)
        {
            try
            {
                List<CoreWebView2Cookie> cookieList = await WBrowse.CoreWebView2.CookieManager.GetCookiesAsync(URLs);
                StringBuilder cookieResult = new StringBuilder(cookieList.Count + " cookie(s) received from " + URLs + " [DATE] " + DateTime.Now.ToString("F"));

                for (int i = 0; i < cookieList.Count; ++i)
                {
                    CoreWebView2Cookie cookie = WBrowse.CoreWebView2.CookieManager.CreateCookieWithSystemNetCookie(cookieList[i].ToSystemNetCookie());
                    cookieResult.Append($"\n\r[NAME] {cookie.Name} [DOMAIN] {cookie.Domain} [IsSecure] {cookie.IsSecure} [VALUE] {cookie.Value} {(cookie.IsSession ? "[session cookie]" : cookie.Expires.ToString("G"))}");
                }

                CreateData(AppStart + "cookie.txt", cookieResult.ToString());
                CreateData(AppStart + "cookie.txt", "\r\n+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+\r\n");
            }
            catch
            {}
        }

        private void SetCookie_Chk_CheckedChanged(object sender, EventArgs e)
        {
            if (SetCookie_Chk.Checked)
            {
                if (CookieName_Txt.Text == "" || CookieValue_Txt.Text == "" || CookieDomain_Txt.Text == "")
                {
                    SetCookie_Chk.Checked = false;
                    MessageBox.Show("Enter all values!");
                }
            }
        }

        #region Workflow

        private void CreateXMLwf_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (AddItemswf_Txt.Text == "")
                    return;

                List_Wf = new ListBox();

                AddItemswf_Txt.Text = Regex.Replace(AddItemswf_Txt.Text, @" ", "");
                string[] str = AddItemswf_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                AddItemswf_Txt.Text = "";
                foreach (string s in str)
                {
                    if (s.Trim().Length > 0)
                    {
                        AddItemswf_Txt.Text += s + "\r\n";
                    }
                }

                File_Write(AppStart + "tempItemAdd.txt", AddItemswf_Txt.Text);

                List_Wf.Items.AddRange(File.ReadAllLines(AppStart + "tempItemAdd.txt"));

                XmlTextWriter writer = new XmlTextWriter(Workflow + NameProjectwf_Txt.Text + ".xml", Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.Indentation = 2;

                writer.WriteStartElement("Table");
                writer.WriteStartElement("KeywordItemCollect");

                for (int i = 0; i < List_Wf.Items.Count; i++)
                {
                    if (List_Wf.Items[i].ToString() != "")
                    {
                        writer.WriteElementString("Item", List_Wf.Items[i].ToString());
                    }
                }

                writer.WriteEndElement();

                for (int i = 0; i < List_Wf.Items.Count; i++)
                {
                    if (List_Wf.Items[i].ToString() != "")
                    {
                        writer.WriteStartElement(List_Wf.Items[i].ToString());
                        CreateNode(List_Wf.Items[i].ToString(), writer); // Markup
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

                loadfiledir.LoadFileDirectory(Workflow, "xml", "lst", ProjectOpn_Lst);
                AddItemswf_Txt.Text = "";
                NameProjectwf_Txt.Text = "";

                if (File.Exists(AppStart + "tempItemAdd.txt"))
                    File.Delete(AppStart + "tempItemAdd.txt");

                ModelList_Lst.Enabled = false;
                ModelDelete_Btn.Enabled = false;
                ModelList_Lst.ClearSelected();

                Beep(1000, 400);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateXMLwf_btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void CreateNode(string pValue, XmlTextWriter writer)
        {
            writer.WriteComment(pValue);
            writer.WriteEndElement();
        }

        private void AddItemwf_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (AddSingleItemswf_Txt.Text == "")
                    return;

                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(Workflow + NameProjectwf_Txt.Text + ".xml");
                doc.Load(xmlReader);

                string Markup = "<!--" + AddSingleItemswf_Txt.Text + "-->";

                if (doc.SelectSingleNode("/Table") is XmlElement node1)
                {
                    int x;
                    x = Itemwf_Cbx.FindStringExact(AddSingleItemswf_Txt.Text);
                    if (x == -1)
                    {
                        XmlElement elem = doc.CreateElement(AddSingleItemswf_Txt.Text);
                        elem.InnerXml = Markup; // Markup
                        node1.AppendChild(elem);
                    }
                }

                xmlReader.Close();
                doc.Save(Workflow + NameProjectwf_Txt.Text + ".xml");

                int y;
                y = Itemwf_Cbx.FindStringExact(AddSingleItemswf_Txt.Text);
                if (y == -1)
                {
                    AddDataWorkflow("KeywordItemCollect", "Item", AddSingleItemswf_Txt.Text, "no");
                }

                Itemwf_Cbx.Items.Clear();
                WorkflowItem_Lst.Items.Clear();
                AddItemswf_Txt.Text = "";

                Workflow_Cbx = Itemwf_Cbx;
                Workflow_Lst = WorkflowItem_Lst;
                Thread LoadAllXML = new Thread(() => LoadItemKeyword_Thr("KeywordItemCollect", "Item", "y"));
                LoadAllXML.Start();

                AddSingleItemswf_Txt.Text = "";
                Beep(1000, 400);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddItemwf_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void Reset()
        {
            Itemwf_Cbx.Items.Clear();
            WorkflowItem_Lst.Items.Clear();
            StatWorkflow_Lst.Items.Clear();
            ProjectOpn_Lst.ClearSelected();
            ModelList_Lst.ClearSelected();
            Timeline_Lst.ClearSelected();

            CreateXMLwf_btn.Enabled = true;
            AddItemswf_Txt.Enabled = true;
            AddItemswf_Txt.Text = "";
            AddSingleItemswf_Txt.Enabled = false;
            AddItemwf_Btn.Enabled = false;
            NameProjectwf_Txt.Enabled = true;
            NameProjectwf_Txt.Text = "";
            ModelList_Lst.Enabled = true;
            ModelDelete_Btn.Enabled = true;
        }

        private void ProjectOpn_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ProjectOpn_Lst.SelectedIndex != -1)
                {
                    Timeline_Lst.Items.Clear();
                    string tml = ProjectOpn_Lst.SelectedItem.ToString().Replace(".xml", ".ost");
                    
                    if (File.Exists(Workflow + tml))
                    {
                        Timeline_Lst.Items.AddRange(File.ReadAllLines(Workflow + tml));
                    }

                    if (File.Exists(Workflow + ProjectOpn_Lst.SelectedItem.ToString()))
                    {
                        Itemwf_Cbx.Items.Clear();
                        WorkflowItem_Lst.Items.Clear();
                        AddItemswf_Txt.Text = "";
                        CreateXMLwf_btn.Enabled = false;
                        AddSingleItemswf_Txt.Enabled = true;
                        AddItemwf_Btn.Enabled = true;
                        ModelList_Lst.Enabled = false;
                        ModelDelete_Btn.Enabled = false;
                        ModelList_Lst.ClearSelected();

                        string s = ProjectOpn_Lst.SelectedItem.ToString().Replace(".xml", "");
                        NameProjectwf_Txt.Text = s;
                        NameProjectwf_Txt.Enabled = false;
                        WFProjectOpn_Lbl.Text = s;
                        AddItemswf_Txt.Enabled = false;

                        Workflow_Cbx = Itemwf_Cbx;
                        Workflow_Lst = WorkflowItem_Lst;
                        Thread LoadItemKeyword = new Thread(() => LoadItemKeyword_Thr("KeywordItemCollect", "Item", "y"));
                        LoadItemKeyword.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ProjectOpn_Lst_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void WorkflowItem_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WorkflowItem_Lst.SelectedIndex != -1)
            {
                if (AddTxtWorkflow_Txt.Text != "")
                {
                    FormatValue();
                }
                else
                {
                    AddTxtWorkflow_Txt.Text = Clipboard.GetText(TextDataFormat.Text);
                    FormatValue();
                }
            }
        }

        private void FormatValue()
        {
            try
            {
                string value;

                string element = WorkflowItem_Lst.SelectedItem.ToString();
                element += "_" + element;

                string[] str = AddTxtWorkflow_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                AddTxtWorkflow_Txt.Text = "";
                foreach (string s in str)
                {
                    if (s.Trim().Length > 0)
                    {
                        AddTxtWorkflow_Txt.Text += s + " ";
                    }
                }

                string header = AddTxtWorkflow_Txt.Text;
                AddTxtWorkflow_Txt.Text = header.Trim(new char[] { ' ' });

                value = AddTxtWorkflow_Txt.Text;

                string message = "Add => " + value + " => in " + WorkflowItem_Lst.SelectedItem.ToString() + " ?";
                string caption = "";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Timeline_Lst.Items.Add(WorkflowItem_Lst.SelectedItem.ToString() + ": " + value);
                    using (StreamWriter SW = new StreamWriter(Workflow + NameProjectwf_Txt.Text + ".ost", false))
                    {
                        foreach (string itm in Timeline_Lst.Items)
                            SW.WriteLine(itm);
                    }

                    var rawItem = value;

                    if (rawItem.Contains("https://"))
                        value = "[[" + value + "]]";

                    if (rawItem.Contains("http://"))
                        value = "[[" + value + "]]";

                    AddDataWorkflow(WorkflowItem_Lst.SelectedItem.ToString(), element, value, "yes");
                }
                else
                {
                    WorkflowItem_Lst.ClearSelected();
                    AddTxtWorkflow_Txt.Text = "";
                    AddNoteWorkflow_Txt.Text = "";
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! FormatValue: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void AddDataWorkflow(string nodeselect, string elementselect, string value, string attrib)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(Workflow + NameProjectwf_Txt.Text + ".xml");
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Table/" + nodeselect) is XmlElement node1)
                {
                    XmlElement elem = doc.CreateElement(elementselect);

                    if (attrib != "no")
                    {
                        if (AddNoteWorkflow_Txt.Text != "")
                        {
                            string[] str = AddNoteWorkflow_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            AddNoteWorkflow_Txt.Text = "";
                            foreach (string s in str)
                            {
                                if (s.Trim().Length > 0)
                                {
                                    AddNoteWorkflow_Txt.Text += s + " ";
                                }
                            }

                            string header = AddNoteWorkflow_Txt.Text;
                            AddNoteWorkflow_Txt.Text = header.Trim(new char[] { ' ' });
                        }

                        elem.SetAttribute("author", Author_Txt.Text);
                        elem.SetAttribute("date", DateTime.Now.ToString("d"));
                        elem.SetAttribute("time", DateTime.Now.ToString("HH:mm:ss"));
                        elem.SetAttribute("note", AddNoteWorkflow_Txt.Text);
                    }

                    elem.InnerText = value;
                    node1.AppendChild(elem);
                }

                xmlReader.Close();
                doc.Save(Workflow + NameProjectwf_Txt.Text + ".xml");

                AddTxtWorkflow_Txt.Text = "";
                AddNoteWorkflow_Txt.Text = "";
                WorkflowItem_Lst.ClearSelected();

                LoadStatWorkflow();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddDataWorkflow: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void OpnWorkflowTools_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text != "")
            {
                if (Panel_Workflow.Visible)
                {
                    Panel_Workflow.Visible = false;
                }
                else
                {
                    Panel_Workflow.Visible = true;
                }
            }
            else
            {
                if (Panel_Workflow.Visible)
                {
                    Panel_Workflow.Visible = false;
                }
                else
                {
                    MessageBox.Show("Open Workflow project first!");
                }
            }
        }

        private void StatWorkflow_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (StatWorkflow_Lst.SelectedIndex != -1)
                {
                    string KeywordItem = StatWorkflow_Lst.SelectedItem.ToString();
                    KeywordItem = Regex.Replace(KeywordItem, "[^a-zA-Z]", "");

                    string element = KeywordItem;
                    element += "_" + element;

                    Thread LoadItemKeyword = new Thread(() => LoadItemKeyword_Thr(KeywordItem, element, "n"));
                    LoadItemKeyword.Start();

                    StatWorkflow_Lst.ClearSelected();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! StatWorkflow_Lst_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void LoadItemKeyword_Thr(string nodeselect, string elementselect, string LoadStat)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Workflow + NameProjectwf_Txt.Text + ".xml");
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Table/" + nodeselect + "/" + elementselect);

                if (LoadStat == "n")
                    Invoke(new Action<string>(SRCpageAdd), "listclear");

                for (int i = 0; i < nodeList.Count; i++)
                {
                    string strvalue = string.Format("{0}", nodeList[i].ChildNodes.Item(0).InnerText);

                    if (LoadStat == "y")
                    {
                        Invoke(new Action<string>(LoadItemKeyword_Invk), strvalue);
                    }
                    else
                    {
                        Invoke(new Action<string>(SRCpageAdd), strvalue);                        
                    }
                }

                if (LoadStat == "n")
                    Invoke(new Action<string>(SRCpageAdd), "listcreate");

                if (LoadStat == "y")
                    Invoke(new Action(LoadStatWorkflow_Invk));
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadItemKeyword_Thr: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void LoadStatWorkflow()
        {
            try
            {
                List_Wf = new ListBox();
                StatWorkflow_Lst.Items.Clear();

                string element;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Workflow + NameProjectwf_Txt.Text + ".xml");

                for (int i = 0; i < WorkflowItem_Lst.Items.Count; i++)
                {
                    if (WorkflowItem_Lst.Items[i].ToString() != "")
                    {
                        element = WorkflowItem_Lst.Items[i].ToString();
                        element += "_" + element;

                        XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Table/" + WorkflowItem_Lst.Items[i].ToString() + "/" + element);

                        for (int e = 0; e < nodeList.Count; e++)
                        {
                            string strvalue = string.Format("{0}", nodeList[e].ChildNodes.Item(0).InnerText);

                            List_Wf.Items.Add(strvalue);
                        }
                    }

                    StatWorkflow_Lst.Items.Add(WorkflowItem_Lst.Items[i].ToString() + "    " + List_Wf.Items.Count);
                    List_Wf.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadStatWorkflow: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void ModelCreate_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ModelName_Txt.Text != "" && ModelItem_Txt.Text != "")
                {
                    ModelItem_Txt.Text = Regex.Replace(ModelItem_Txt.Text, @" ", "");
                    string[] str = ModelItem_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    ModelItem_Txt.Text = "";
                    foreach (string s in str)
                    {
                        if (s.Trim().Length > 0)
                        {
                            ModelItem_Txt.Text += s + "\r\n";
                        }
                    }

                    if (File.Exists(WorkflowModel + ModelName_Txt.Text + ".txt"))
                    {
                        string message = "File " + ModelName_Txt.Text + " exist, continue?";
                        string caption = "File exist";
                        var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }

                    File_Write(WorkflowModel + ModelName_Txt.Text + ".txt", ModelItem_Txt.Text);

                    ModelItem_Txt.Text = "";
                    ModelName_Txt.Text = "";

                    loadfiledir.LoadFileDirectory(WorkflowModel, "txt", "lst", ModelList_Lst);

                    Beep(1000, 400);
                }
                else
                {
                    ModelItem_Txt.BackColor = Color.Red;
                    ModelName_Txt.BackColor = Color.Red;
                    MessageBox.Show("Add Model Name and Items!");
                    ModelItem_Txt.BackColor = Color.White;
                    ModelName_Txt.BackColor = Color.FromArgb(28, 28, 28);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ModelCreate_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void ModelList_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ModelList_Lst.SelectedIndex != -1)
                {
                    if (File.Exists(WorkflowModel + ModelList_Lst.SelectedItem.ToString()))
                    {
                        AddItemswf_Txt.Text = "";
                        StreamReader sr = new StreamReader(WorkflowModel + ModelList_Lst.SelectedItem.ToString());
                        AddItemswf_Txt.Text = sr.ReadToEnd();
                        sr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ModelList_Lst_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void ModelDelete_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ModelList_Lst.SelectedIndex != -1)
                {
                    string message = "Do you want to delete the model " + ModelList_Lst.SelectedItem.ToString() + "?";
                    string caption = "Delete Model";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (File.Exists(WorkflowModel + ModelList_Lst.SelectedItem.ToString()))
                        {
                            File.Delete(WorkflowModel + ModelList_Lst.SelectedItem.ToString());
                            AddItemswf_Txt.Text = "";

                            loadfiledir.LoadFileDirectory(WorkflowModel, "txt", "lst", ModelList_Lst);
                            Beep(1000, 400);
                        }
                        else
                        {
                            MessageBox.Show("File not exist!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ModelDelete_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void Timeline_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Timeline_Lst.SelectedIndex != -1)
            {
                Class_Var.Text_Load = Timeline_Lst.SelectedItem.ToString();
                Open_Doc_Frm("textload");
            }
        }

        private void ResetTimeline_Btn_Click(object sender, EventArgs e)
        {
            if (Timeline_Lst.Items.Count > 0)
            {
                string message = "Do you want to reset the timeline??";
                string caption = "Timeline reset";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Timeline_Lst.Items.Clear();

                    if (File.Exists(Workflow + NameProjectwf_Txt.Text + ".ost"))
                        File.Delete(Workflow + NameProjectwf_Txt.Text + ".ost");
                }
            }
        }

        #endregion

        #region Options_

        private void SaveConfig_Opt_Btn_Click(object sender, EventArgs e)
        {
            CreateConfigFile(1);
            MessageBox.Show("Config save.");
        }

        private void URLdirect_Opt_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(FileDir + "url.txt");
        }

        private void URLconstruct_Opt_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(FileDir + @"url-constructor\construct_url.txt");
        }

        private void URLconstructDir_Opt_Btn_Click(object sender, EventArgs e)
        {
            Process.Start(FileDir + "url-constructor");
        }

        private void AddOntools_Opt_Btn_Click(object sender, EventArgs e)
        {
            Process.Start(Plugins);
        }

        private void MultipleWin_Opt_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(FileDir + @"grp-frm\grp_frm_url_opn.txt");
        }

        private void MultipleWinDir_Opt_Btn_Click(object sender, EventArgs e)
        {
            Process.Start(FileDir + "grp-frm");
        }

        private void GoogleDork_Opt_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(FileDir + "gh.txt"); 
        }

        private void OstiumDir_Opn_Click(object sender, EventArgs e)
        {
            Process.Start(AppStart);
        }

        private void AddOnDir_Opn_Click(object sender, EventArgs e)
        {
            Process.Start(Plugins);
        }

        private void DatabseDir_Opn_Click(object sender, EventArgs e)
        {
            Process.Start(DBdirectory);
        }

        private void FeedDir_Opn_Click(object sender, EventArgs e)
        {
            Process.Start(FeedDir);
        }

        private void ScriptDir_Opn_Click(object sender, EventArgs e)
        {
            Process.Start(Scripts);
        }

        private void WorkFlowDir_Opn_Click(object sender, EventArgs e)
        {
            Process.Start(Workflow);
        }

        private void WorkFlowModelDir_Opn_Click(object sender, EventArgs e)
        {
            Process.Start(WorkflowModel);
        }

        private void PictureDir_Opn_Click(object sender, EventArgs e)
        {
            Process.Start(Pictures);
        }

        #endregion

        #region Process_

        private void VerifProcessRun_Btn_Click(object sender, EventArgs e)
        {
            VerifyProcess("javaw");
        }

        private void KillProcess_Btn_Click(object sender, EventArgs e)
        {
            Process[] localByName = Process.GetProcessesByName("javaw");
            if (localByName.Length > 0)
            {
                string message = "Are you sure to Kill Process?";
                string caption = "Process Kill";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    KillProcessMSEDGE("javaw");
                    MessageBox.Show("The Process: javaw was successfully stopped!", "Kill process");
                }
            }
            else
            {
                MessageBox.Show("Process not running.", "Process");
            }

        }

        public void VerifyProcess(string ProcessVerif)
        {
            try
            {
                Process[] localByName = Process.GetProcessesByName(ProcessVerif);
                if (localByName.Length > 0)
                {
                    MessageBox.Show("Process is True.", "Process");
                }
                else
                {
                    MessageBox.Show("Process is False.", "Process");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! VerifyProcess: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void KillProcessMSEDGE(string ProcessKill)
        {
            try
            {
                Process[] sProcess;
                Process[] localByName = Process.GetProcessesByName(ProcessKill);
                if (localByName.Length > 0)
                {
                    sProcess = Process.GetProcessesByName(ProcessKill);
                    for (var i = 0; i <= sProcess.Length - 1; i++)
                        sProcess[i].Kill();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! KillProcessMSEDGE: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #endregion

        private void Timo_Tick(object sender, EventArgs e)
        {
            Process[] localByName = Process.GetProcessesByName("javaw");
            if (localByName.Length > 0)
            {}
            else
            {
                Timo.Enabled = false;
                Thread.Sleep(1000);
                CreateDiagram_Invk(FileDiag); // Process False
            }
        }

        #region Invoke_

        public void URLbrowseCbxText(string value)
        {
            URLbrowse_Cbx.Text = value;
            URLtxt_txt.Text = string.Format("Unshorten URL => {0}", value);
        }

        public void SRCpageAdd(string value)
        {
            if (value == "listclear")
            {
                Source_Page_Lst.Items.Clear();
            }
            else if (value == "listcreate")
            {
                List_Object = Source_Page_Lst;
                List_Create("sourcepagelst", "yes");
                Console_Cmd_Txt.Enabled = true;
            }
            else
            {
                Source_Page_Lst.Items.Add(value);
            }
        }

        #endregion

        #region Invoke_Workflow

        private void LoadItemKeyword_Invk(string value)
        {
            Workflow_Cbx.Items.Add(value);
            Workflow_Lst.Items.Add(value);
            AddItemswf_Txt.AppendText(value + "\r\n");            
        }

        private void LoadStatWorkflow_Invk()
        {
            LoadStatWorkflow();
        }

        #endregion

        #region Invoke_Feed

        private void CountBlockSite_Invk(string xswitch)
        {
            switch (xswitch)
            {
                case "Clear":
                    CountBlockSite_Lbl.Items.Clear();
                    CountBlockFeed_Lbl.Items.Clear();
                    break;
                case "Disable":
                    CountBlockSite_Lbl.Enabled = false;
                    break;
                case "AllTrue":
                    CategorieFeed_Cbx.Enabled = true;
                    CountBlockSite_Lbl.Enabled = true;
                    CreatCategorie_Btn.Enabled = true;
                    AddFeed_Btn.Enabled = true;
                    CollapseTitleFeed_Btn.Enabled = true;
                    HomeFeed_Btn.Enabled = true;
                    ManageFeed_Btn.Enabled = true;
                    ToolsFeed_Mnu.Enabled = true;
                    LangSelect_Lst.Enabled = true;
                    ReadTitle_Btn.Enabled = true;
                    PauseSpeak_Btn.Enabled = true;
                    StopSpeak_Btn.Enabled = true;
                    GoFeed_Btn.Enabled = true;
                    GoFeed_Txt.Enabled = true;
                    break;
                case "FeedTitle":
                    CountFeed_Lbl.Text = TitleFeed;
                    CountBlockSite_Lbl.Items.Add(TitleFeed);
                    break;
                case "ListCount":
                    CountBlockFeed_Lbl.Items.Add(Title_Lst.Items.Count);
                    break;
                case "ItemTitleAdd":
                    Title_Lst.Items.Add(AddTitleItem);
                    break;
                case "ItemLinkAdd":
                    Link_Lst.Items.Add(AddLinkItem);
                    break;
                case "CountFeed":
                    CountFeed_Lbl.Text = "Count Feed: " + Title_Lst.Items.Count;
                    break;
                case "Msleave":
                    CountBlockSite_Lbl.SendToBack();
                    CountBlockSite_Lbl.Width = 50;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Invoke_Diagram

        private void CreateDiagram_Invk(string value)
        {
            string delExt = value.Replace(".txt", ""); ;

            URLbrowse_Cbx.Text = "file:///" + DiagramDir + delExt + ".svg";
            GoBrowser(1);

            URLbrowse_Cbx.Text = "";
        }

        #endregion
    }
}
