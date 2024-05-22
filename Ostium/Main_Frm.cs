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
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET;
using System.Globalization;
using System.Text.Json.Nodes;
using FastColoredTextBoxNS;

namespace Ostium
{
    public partial class Main_Frm : Form
    {
        #region Var_
        ///
        /// <summary>
        /// Emits a beep at the end of certain actions
        /// </summary>
        /// <param name="freq">Beep tone frequency</param>
        /// <param name="duration">Beep emission duration</param>
        /// <returns></returns>
        /// 
        [DllImport("kernel32.dll")]
        static extern bool Beep(int freq, int duration);
        ///
        /// <summary>
        /// Initialization of the voice for Reading Feed Titles
        /// </summary>
        ///
        readonly SpeechSynthesizer synth = new SpeechSynthesizer();
        ///
        /// <summary>
        /// List of default configuration URLs from the "config.xml" file, load from the "url_dflt_cnf.ost" file
        /// </summary>
        ///
        readonly List<string> lstUrlDfltCnf = new List<string>();
        ///
        /// <summary>
        /// Directories of the different usage files and the Database
        /// </summary>
        /// 
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
        readonly string WebView2Dir = Application.StartupPath + @"\Ostium.exe.WebView2\";
        readonly string Setirps = Application.StartupPath + @"\setirps\";
        readonly string BkmkltDir = Application.StartupPath + @"\scripts\bookmarklet\";
        readonly string MapDir = Application.StartupPath + @"\map\";
        readonly string JsonDir = Application.StartupPath + @"\json-files\";
        readonly string JsonDirTable = Application.StartupPath + @"\json-files\table\";
        public string D4ta = "default_database_name";
        ///
        /// <summary>
        /// Objects
        /// </summary>
        /// 
        Webview_Frm webviewForm;
        HtmlText_Frm HtmlTextFrm;
        Mdi_Frm mdiFrm;
        Doc_Frm docForm;
        OpenSource_Frm openSourceForm;
        ScriptCreator scriptCreatorFrm;
        Bookmarklets_Frm bookmarkletsFrm;
        ListBox List_Object;
        ToolStripComboBox Cbx_Object;
        ListBox List_Wf;
        ComboBox Workflow_Cbx;
        ListBox Workflow_Lst;
        Label SizeAll_Lbl;
        RichTextBox OutJson;
        RichTextBox OutParse;
        ///
        /// <summary>
        /// Variables
        /// </summary>
        /// 
        readonly string SoftVersion = "";
        string ClearOnOff = "on";
        string NameUriDB = "";
        string UnshortURLval = "";
        string Una = "";
        //readonly int CMDconsSwitch;
        string TableOpen = "";
        string Tables_Lst_Opt = "add";
        string DataAddSelectUri = "";
        string tlsi = ""; // Table List selected Item
        string DBadmin = "off";
        string ManageFeed = "off";
        string TmpTitleWBrowse = "";
        string TmpTitleWBrowsefeed = "";
        int VerifLangOpn = 0;
        string TitleFeed;
        string AddTitleItem;
        string AddLinkItem;
        string UserAgentOnOff = "off";
        string UserAgentSelect = "";
        string ThemeDiag;
        string FileDiag = "";
        string MinifyScr = "";
        string MapXmlOpn = "";
        string CrossCenter = "on";
        int MapZoom = 1;
        ///
        /// <summary>
        /// Map variables
        /// </summary>
        /// 
        string VerifMapOpn = "off";
        readonly GMapOverlay overlayOne = new GMapOverlay("OverlayOne");
        double LatT = 48.8589507;
        double LonGt = 2.2775175;
        GMarkerGoogleType Mkmarker = GMarkerGoogleType.red_dot; // by default
        ///
        int Commut = 0;
        ///
        /// <summary>
        /// DLL => "icaza.dll"
        /// </summary>
        /// 
        readonly IcazaClass senderror = new IcazaClass();
        readonly Loaddir loadfiledir = new Loaddir();
        readonly IcazaClass selectdir = new IcazaClass();
        readonly IcazaClass openfile = new IcazaClass();
        readonly ReturnSize sizedireturn = new ReturnSize();
        /// <summary>
        /// Message displayed when starting the creation of a Diagram
        /// </summary>
        readonly string MessageStartDiagram = "When this window closes, the diagram creation process begins, be patient the time depends on the file size " +
            "and structure. In case of blockage! use Debug in the menu to kill the javaw process. Feel free to join the Discord channel for help.";
        readonly string MessageStartGeoloc = "When this window closes, the creation process begins. Please be patient, as the time depends on the number " +
            "of points to be added to the file. ";
        ///
        /// <summary>
        /// Variable for checking updates
        /// <param name="versionNow">Current version of the application to compare with the Http request = > "updt_ostium.html"</param>
        /// </summary>
        /// 
        readonly string upftOnlineFile = "https://veydunet.com/2x24/sft/updt/updt_ostium.html";
        readonly string WebPageUpdate = "http://veydunet.com/ostium/update.html";
        readonly string versionNow = "7";

        readonly string HomeUrlRSS = "https://veydunet.com/ostium/rss.html";

        #endregion

        #region Frm_
        ///
        /// <param name="SoftVersion">Version and Assembly checking</param>
        ///
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

        void Main_Frm_Load(object sender, EventArgs e)
        {
            try
            {
                BeginInvoke((MethodInvoker)delegate {
                    CreateDirectory();
                    ///
                    /// Loading default URLs into a List
                    ///
                    if (File.Exists(AppStart + "url_dflt_cnf.ost"))
                    {
                        lstUrlDfltCnf.Clear();
                        lstUrlDfltCnf.AddRange(File.ReadAllLines(AppStart + "url_dflt_cnf.ost"));
                    }
                    ///
                    /// Loading configuration
                    /// <param name="CreateConfigFile"></param>
                    /// <param value="0">Resetting the config, loading the default URLs from the url_dflt_cnf.ost file</param>
                    /// <param value="1">Save the chosen configuration and Reload</param>
                    /// 
                    if (File.Exists(AppStart + "config.xml"))
                    {
                        Config_Ini(AppStart + "config.xml");
                    }
                    else
                    {
                        CreateConfigFile(0);
                    }
                    ///
                    /// Web URL Home page wBrowser Tab => index and wBrowser Tab => feed
                    /// If empty loading from default URL file
                    ///
                    if (@Class_Var.URL_HOME == "")
                    {
                        @Class_Var.URL_HOME = lstUrlDfltCnf[1].ToString();
                    }

                    WBrowse.Source = new Uri(@Class_Var.URL_HOME);
                    WBrowsefeed.Source = new Uri(HomeUrlRSS);

                    Tools_TAB_0.Visible = true;
                    ///
                    /// Checking auto updates
                    /// <param value="0">Message only if update available</param>
                    /// <param value="1">Message False or True update</param>
                    ///
                    VerifyUPDT("Ostium", 0);
                });
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Main_Frm_Load: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Cleanup request when closing the application
        /// </summary>
        /// <param name="ClearOnOff"></param>
        /// <param name="on">Cleaning True</param>
        /// <param name="off">No cleanup requested when auto restart of the application to modify the default user-agent</param>
        ///
        void Main_Frm_FormClosed(object sender, EventArgs e)
        {
            try
            {
                if (ClearOnOff == "on")
                {
                    var result = MessageBox.Show("Delete all history? (Run purge.bat after closing Ostium, for complete deletion of the WebView2 usage directory)", "Delete all history", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        ClearData(1);
                    }
                }
            }
            catch { }
        }

        #endregion

        void Form_EventHandler()
        {
            FormClosing += new FormClosingEventHandler(Main_Frm_FormClosed);
            CategorieFeed_Cbx.SelectedIndexChanged += new EventHandler(CategorieFeed_Cbx_SelectedIndexChanged);
            CountBlockSite_Lbl.MouseEnter += new EventHandler(CountBlockSite_Lbl_MouseEnter);
            CountBlockSite_Lbl.MouseLeave += new EventHandler(CountBlockSite_Lbl_Leave);
            ThemDiag_Cbx.SelectedIndexChanged += new EventHandler(ThemDiag_Cbx_SelectedIndexChanged);
            GMap_Ctrl.OnPositionChanged += new PositionChanged(GMap_Ctrl_OnPositionChanged);
            GMap_Ctrl.OnMapZoomChanged += new MapZoomChanged(GMap_Ctrl_OnMapZoomChanged);
            GMap_Ctrl.MouseDoubleClick += new MouseEventHandler(GMap_Ctrl_MouseDoubleClick);
            GMap_Ctrl.KeyDown += new KeyEventHandler(Main_Frm_KeyUp);
            OutJsonA_Chk.Click += new EventHandler(OutJsonA_Chk_Click);
            OutJsonB_Chk.Click += new EventHandler(OutJsonB_Chk_Click);
        }
        ///
        /// <summary>
        /// Creation of the "config.xml" configuration file
        /// </summary>
        /// <param name="val"></param>
        /// <param value="0">Loading default URLs from the "url_dflt_cnf.ost" file</param>
        /// <param value="1">Backup with parameters chosen by the user</param>
        /// 
        void CreateConfigFile(int val)
        {
            string dbDflt = DB_Default_Opt_Txt.Text;
            string urlHom = UrlHome_Opt_Txt.Text;
            string urlTra = UrlTradWebPage_Opt_Txt.Text;
            string Search = SearchEngine_Opt_Txt.Text;
            string UsrAgt = UserAgent_Opt_Txt.Text;
            string UsrHtt = UserAgentHttp_Opt_Txt.Text;
            string GoogBo = GoogBot_Opt_Txt.Text;

            if (val == 0)
            {                
                dbDflt = lstUrlDfltCnf[0].ToString();
                urlHom = lstUrlDfltCnf[1].ToString();
                urlTra = lstUrlDfltCnf[2].ToString();
                Search = lstUrlDfltCnf[3].ToString();
                UsrAgt = lstUrlDfltCnf[4].ToString();
                UsrHtt = lstUrlDfltCnf[5].ToString();
                GoogBo = lstUrlDfltCnf[6].ToString();
                DefaultEditor_Opt_Txt.Text = AppStart + "OstiumE.exe";
            }
            ///
            /// Create XML file "config.xml"
            ///
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                CheckCharacters = true
            };

            using (XmlWriter writer = XmlWriter.Create(AppStart + "config.xml", settings))
            {
                writer.WriteStartElement("Xwparsingxml");
                writer.WriteStartElement("Xwparsingnode");
                writer.WriteElementString("DB_USE_DEFAULT", dbDflt);
                writer.WriteElementString("URL_HOME_VAR", urlHom);
                writer.WriteElementString("URL_TRAD_WEBPAGE_VAR", urlTra);
                writer.WriteElementString("URL_TRAD_WEBTXT_VAR", "https://translate.google.fr/?hl=fr&sl=auto&tl=fr&text=replace_query&op=translate"); // Not implemented
                writer.WriteElementString("URL_DEFAUT_WSEARCH_VAR", Search);
                writer.WriteElementString("URL_USER_AGENT_VAR", UsrAgt);
                writer.WriteElementString("URL_USER_AGENT_SRC_PAGE_VAR", UsrHtt);
                writer.WriteElementString("URL_GOOGLEBOT_VAR", GoogBo);
                writer.WriteElementString("DEFAULT_EDITOR_VAR", DefaultEditor_Opt_Txt.Text);
                writer.WriteElementString("VOLUME_TRACK_VAR", Convert.ToString(VolumeVal_Track.Value));
                writer.WriteElementString("RATE_TRACK_VAR", Convert.ToString(RateVal_Track.Value));
                writer.WriteEndElement();
                writer.Flush();
            }
            ///
            /// Loading the configuration from the "config.xml" file
            /// 
            Config_Ini(AppStart + "config.xml");
        }
        ///
        /// <summary>
        /// Creation of the different directories of the application => DirectoryCreate
        /// </summary>
        /// 
        void CreateDirectory()
        {
            try
            {
                var CreateDir = new List<string>()
                    {
                        Plugins,
                        DBdirectory,
                        FeedDir,
                        FileDir,
                        FileDir + "url-constructor",
                        FileDir + "grp-frm",
                        Pictures,
                        Scripts,
                        Workflow,
                        Workflow + "model",
                        DiagramDir,
                        BkmkltDir,
                        Setirps,
                        MapDir,
                        JsonDir,
                        JsonDir + "table"
                    };

                for (int i = 0; i < CreateDir.Count; i++)
                {
                    DirectoryCreate(CreateDir[i].ToString());
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateDirectory: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Creation of different directories if Not exist!
        /// </summary>
        /// <param name="dir">Directory being created</param>
        /// 
        void DirectoryCreate(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        ///
        /// <summary>
        /// Loading the configuration from the "config.xml" file
        /// Class_var.cs variable registrations
        /// </summary>
        /// <param name="ConfigFile"></param>
        /// 
        void Config_Ini(string ConfigFile)
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
                                        JsonUsrAgt_Txt.Text = Class_Var.URL_USER_AGENT_SRC_PAGE;
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
                ///
                /// If DataBase exist True loading
                /// If DataBase exist False create
                /// 
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
                ///
                /// Loading JS scripts from "script url.ost" file for injection.
                /// 
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

                Class_Var.COOKIES_SAVE = 0; /// Save all cookies in the cookie.txt file at the root if SaveCookies_Chk checked = True, default = False
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Config_Ini: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Calculate and display the size of application directories
        /// </summary>
        /// <param name="directoryname">Directory address</param>
        /// <param name="objectsend">Loading the size into the corresponding Label object</param>
        /// 
        async void DireSizeCalc(string directoryname, object objectsend)
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
        ///
        /// <summary>
        /// Adding an item to the wBrowse Context Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="newItem0">Search on the WayBackMachine</param>
        /// <param name="newItem1">Search for video on Youtube</param>
        /// <param name="newItem2">Embed Youtube video</param>
        /// 
        void WBrowse_ContextMenuRequested(object sender, CoreWebView2ContextMenuRequestedEventArgs args)  // ContextMenu
        {
            string UriYoutube = "";
            string C = WBrowse.Source.AbsoluteUri;
            UriYoutube += C.Substring(0, 32);

            IList<CoreWebView2ContextMenuItem> menuList = args.MenuItems;

            CoreWebView2ContextMenuItem newItem0 = WBrowse.CoreWebView2.Environment.CreateContextMenuItem("Search on WayBackMachine", null, CoreWebView2ContextMenuItemKind.Command);
            newItem0.CustomItemSelected += delegate (object send, object ex)
            {
                string pageUri = args.ContextMenuTarget.PageUri; ;

                SynchronizationContext.Current.Post((_) =>
                {
                    GoBrowser("http://web.archive.org/web/*/" + pageUri, 1);
                }, null);
            };

            CoreWebView2ContextMenuItem newItem1 = WBrowse.CoreWebView2.Environment.CreateContextMenuItem("Search on Youtube", null, CoreWebView2ContextMenuItemKind.Command);
            newItem1.CustomItemSelected += delegate (object send, object ex)
            {
                string pageUri = "";

                if (Clipboard.ContainsText(TextDataFormat.Text))
                    pageUri = Clipboard.GetText(TextDataFormat.Text);

                SynchronizationContext.Current.Post((_) =>
                {
                    GoBrowser("https://www.youtube.com/results?search_query=" + pageUri, 1);
                }, null);
            };

            CoreWebView2ContextMenuItem newItem2 = WBrowse.CoreWebView2.Environment.CreateContextMenuItem("Youtube embed", null, CoreWebView2ContextMenuItemKind.Command);
            newItem2.CustomItemSelected += delegate (object send, object ex)
            {
                SynchronizationContext.Current.Post((_) =>
                {
                    if (UriYoutube == "https://www.youtube.com/watch?v=")
                    {
                        string pageUri = WBrowse.Source.AbsoluteUri;
                        pageUri = pageUri.Replace(UriYoutube, "https://www.youtube.com/embed/");
                        File_Write(AppStart + "tmpytb.html", "<iframe width=100% height=100% src=\"" + pageUri + "\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" referrerpolicy=\"strict-origin-when-cross-origin\" allowfullscreen></iframe>");
                        GoBrowser("file:///" + AppStart + "tmpytb.html", 1);
                    }
                    else
                    {
                        MessageBox.Show("You are not on Youtube!");
                    }
                }, null);
            };

            menuList.Insert(menuList.Count, newItem0);
            menuList.Insert(menuList.Count, newItem1);
            menuList.Insert(menuList.Count, newItem2);
        }
        ///
        /// <summary>
        /// Updated the wBrowse TAB index title bar
        /// </summary>
        /// <param name="message">Title of the current web page</param>
        /// <param name="TmpTitleWBrowse">Application Title variable when TAB change</param>
        /// 
        void WBrowse_UpdtTitleEvent(string message)
        {
            string currentDocumentTitle = WBrowse?.CoreWebView2?.DocumentTitle ?? "Uninitialized";
            Text = currentDocumentTitle + " [" + message + "]";
            TmpTitleWBrowse = Text;
        }
        ///
        /// <summary>
        /// Navigation starting => check if user-agent modification is enabled
        /// </summary>
        /// <param name="UserAgentOnOff"></param>
        /// <param name="on">If enabled user-agent modification</param>
        /// 
        void WBrowse_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var settings = WBrowse.CoreWebView2.Settings;

            if (UserAgentOnOff == "on")
            {
                settings.UserAgent = UserAgentSelect;
            }

            WBrowse_UpdtTitleEvent("Navigation Starting");
        }
        ///
        /// <summary>
        /// Script injection and cookie registration and Title update
        /// </summary>
        /// <param name="ScriptInject()">checking if a script is registered and executed for the current URL</param>
        /// <param name="GetCookie">Save all cookies in the cookie.txt file at the root if SaveCookies_Chk checked = True</param>
        /// 
        void WBrowse_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            @Class_Var.URL_URI = WBrowse.Source.AbsoluteUri;

            if (SaveCookies_Chk.Checked)
                GetCookie(WBrowse.Source.AbsoluteUri);

            WBrowse_UpdtTitleEvent("Navigation Completed");

            ScriptInject();
        }
        ///
        /// <summary>
        /// Checks if cookie injection is enabled
        /// </summary>
        /// <param name="SetCookie_Chk.Checked">If True creation of the configured cookie</param>
        /// 
        void WBrowse_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (SetCookie_Chk.Checked)
            {
                CoreWebView2Cookie cookie = WBrowse.CoreWebView2.CookieManager.CreateCookie(CookieName_Txt.Text, CookieValue_Txt.Text, CookieDomain_Txt.Text, "/");
                WBrowse.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);
            }
        }
        ///
        /// <param name="URLtxt_txt">Saving current URL in Textbox for reuse</param>
        ///
        void WBrowse_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            URLtxt_txt.Text = WBrowse.Source.AbsoluteUri;
            WBrowse_UpdtTitleEvent("Source Changed");
        }

        void WBrowse_HistoryChanged(object sender, object e)
        {
            Back_Btn.Enabled = WBrowse.CoreWebView2.CanGoBack;
            Forward_Btn.Enabled = WBrowse.CoreWebView2.CanGoForward;
            WBrowse_UpdtTitleEvent("History Changed");
        }
        ///
        /// <param name="NameUriDB">URL Title variable for addition to the DataBase</param>
        /// <param name="Download_Source_Page()">Download and save the web page source to the sourcepage file for reuse</param>
        /// 
        void WBrowse_DocumentTitleChanged(object sender, object e)
        {
            Text = WBrowse.CoreWebView2.DocumentTitle;
            NameUriDB = WBrowse.CoreWebView2.DocumentTitle;

            WBrowse_UpdtTitleEvent("DocumentTitleChanged");
            Download_Source_Page();
        }

        void WBrowse_InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
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
            WBrowse.CoreWebView2.ContextMenuRequested += WBrowse_ContextMenuRequested; // ContextMenu

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

        void WBrowsefeed_UpdtTitleEvent(string message)
        {
            string currentDocumentTitle = WBrowsefeed?.CoreWebView2?.DocumentTitle ?? "Uninitialized";
            Text = currentDocumentTitle + " [" + message + "]";
            TmpTitleWBrowsefeed = Text;
        }

        void WBrowsefeed_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            WBrowsefeed_UpdtTitleEvent("Navigation Starting");
        }
        ///
        /// <param name="GetCookie">Save all cookies in the cookie.txt file at the root if SaveCookies_Chk checked = True</param>
        ///
        void WBrowsefeed_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (SaveCookies_Chk.Checked)
                GetCookie(WBrowse.Source.AbsoluteUri);

            WBrowsefeed_UpdtTitleEvent("Navigation Completed");
        }
        ///
        /// <param name="URLtxt_txt">Saving current URL in Textbox for reuse</param>
        /// 
        void WBrowsefeed_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            URLtxt_txt.Text = WBrowsefeed.Source.AbsoluteUri;
            WBrowsefeed_UpdtTitleEvent("Source Changed");
        }

        void WBrowsefeed_HistoryChanged(object sender, object e)
        {
            WBrowsefeed_UpdtTitleEvent("History Changed");
        }

        void WBrowsefeed_DocumentTitleChanged(object sender, object e)
        {
            Text = WBrowsefeed.CoreWebView2.DocumentTitle;

            WBrowsefeed_UpdtTitleEvent("DocumentTitleChanged");
        }

        void WBrowsefeed_InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
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
        ///
        /// <summary>
        /// Script injection
        /// </summary>
        /// <param name="WBrowse.Source.AbsoluteUri">URL of the current site, removing special characters from the URL and adding the ".js" extension
        /// to check if a ".js" script name of the same name exists, if True injecting the script contained in the file on the current web page</param>
        /// 
        async void ScriptInject()
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
        ///
        /// <summary>
        /// Clear all Data history
        /// </summary>
        /// <param name="val"></param>
        /// <param value="0">No opening application directory to start full purge.bat cleanup</param>
        /// <param value="1">Opening application directory to start full purge.bat cleanup</param>
        /// 
        private void ClearData(int val)
        {
            CoreWebView2Profile profile;
            if (WBrowse.CoreWebView2 != null)
            {
                profile = WBrowse.CoreWebView2.Profile;
                CoreWebView2BrowsingDataKinds dataKinds =
                                         CoreWebView2BrowsingDataKinds.GeneralAutofill |
                                      CoreWebView2BrowsingDataKinds.PasswordAutosave |
                                      CoreWebView2BrowsingDataKinds.AllDomStorage |
                                      CoreWebView2BrowsingDataKinds.DownloadHistory |
                                      CoreWebView2BrowsingDataKinds.Cookies |
                                      CoreWebView2BrowsingDataKinds.CacheStorage |
                                      CoreWebView2BrowsingDataKinds.IndexedDb |
                                      CoreWebView2BrowsingDataKinds.LocalStorage |
                                      CoreWebView2BrowsingDataKinds.AllSite |
                                      CoreWebView2BrowsingDataKinds.BrowsingHistory;
                profile.ClearBrowsingDataAsync(dataKinds);
            }
            if (val == 1)
                Process.Start(AppStart);
        }

        #region Control_Browser
        ///
        /// <summary>
        /// Opening the web page
        /// </summary>
        /// <param name="GoBrowser"></param>
        /// <param value="0">Open parent window</param>
        /// <param value="1">Opens in a new window</param>
        ///
        void Go_Btn_Click(object sender, EventArgs e)
        {
            int x;
            x = URLbrowse_Cbx.FindStringExact(URLbrowse_Cbx.Text);
            if (x == -1)
            {
                URLbrowse_Cbx.Items.Add(URLbrowse_Cbx.Text);
            }       
            GoBrowser(URLbrowse_Cbx.Text, 0);
        }

        void GoWebwiev_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser(URLbrowse_Cbx.Text, 1);
        }
        ///
        /// <summary>
        /// Checking the URL and reformatting then opening the web page or searching the web
        /// </summary>
        /// <param name="WebviewRedirect"></param>
        /// <param value="0">Opening or Searching in parent window</param>
        /// <param value="1">Open or Search in a new window</param>
        /// <param value="file:///">Local file opening</param>
        /// <param name="URIopn">URL open in wBrowser "TAB BROWSx"</param>
        /// 
        void GoBrowser(string URIopn, int WebviewRedirect)
        {
            try
            {
                var rawUrl = URIopn;
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
                        if (Class_Var.URL_DEFAUT_WSEARCH == "")
                        {
                            Class_Var.URL_DEFAUT_WSEARCH = lstUrlDfltCnf[3].ToString();
                        }

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
        ///
        /// <summary>
        /// <param name="New Tab">Webview_Frm</param>
        /// </summary>
        /// 
        void GoNewtab()
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

        void Back_Btn_Click(object sender, EventArgs e)
        {
            WBrowse.GoBack();
        }

        void Forward_Btn_Click(object sender, EventArgs e)
        {
            WBrowse.GoForward();
        }

        void Refresh_Btn_Click(object sender, EventArgs e)
        {
            WBrowse.Reload();
        }
        ///
        /// <summary>
        /// Opening the Home Page if configured "@Class_Var.URL_HOME" or opening the default page of the file "url_dflt_cnf.ost"
        /// </summary>
        /// 
        void Home_Btn_Click(object sender, EventArgs e)
        {
            if (@Class_Var.URL_HOME == "")
            {
                @Class_Var.URL_HOME = lstUrlDfltCnf[1].ToString();
            }

            WBrowse.Source = new Uri(@Class_Var.URL_HOME);
        }

        void OnKey_URLbrowse(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                int x;
                x = URLbrowse_Cbx.FindStringExact(URLbrowse_Cbx.Text);
                if (x == -1)
                {
                    URLbrowse_Cbx.Items.Add(URLbrowse_Cbx.Text);
                }
                GoBrowser(URLbrowse_Cbx.Text, 0);
            }
        }

        void URLbrowse_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (Control_Tab.SelectedIndex == 0)
            //{
            //    WBrowse.Source = new Uri(@URLbrowse_Cbx.Text);
            //}
        }
        ///
        /// <summary>
        /// Opening URL of the “filesdir/url.txt” file loaded in CBX
        /// </summary>
        /// 
        void URL_URL_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Tools_TAB_0.Focus();
                WBrowse.Source = new Uri(@URL_URL_Cbx.Text);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! URL_URL_Cbx_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void CleanSearch_Btn_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Items.Clear();
            URLbrowse_Cbx.Text = "";
        }

        #endregion

        #region Tools_Tab_0
        ///
        /// <summary>
        /// Modification of the user-agent by the replacement agent if configured "Class_Var.URL_USER_AGENT" or by the default agent in the file
        /// "url_dflt_cnf.ost" and/or restart the application to return to the default wBrowser agent
        /// </summary>
        /// <param name="ClearOnOff"></param>
        /// <param name="on"></param>
        /// Clean request notification True
        /// <param name="off"></param>
        /// No notification of cleaning request when auto restart of the application to return the default user-agent
        /// 
        void UserAgentChange_Btn_Click(object sender, EventArgs e)
        {
            if (Class_Var.URL_USER_AGENT == "")
                Class_Var.URL_USER_AGENT = lstUrlDfltCnf[4].ToString();

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
        ///
        /// <summary>
        /// Modification of the user-agent by a "GoogleBot" type agent if configured in "Class_Var.URL_USER_AGENT" or by the default agent in the file
        /// "url_dflt_cnf.ost" and/or restarting the application
        /// </summary>
        /// <param name="ClearOnOff"></param>
        /// <param name="on"></param>
        /// Clean request notification True
        /// <param name="off"></param>
        /// No notification of cleaning request when auto restart of the application to return the default user-agent
        /// 
        void Googlebot_Btn_Click(object sender, EventArgs e)
        {
            if (Class_Var.URL_GOOGLEBOT == "")
                Class_Var.URL_GOOGLEBOT = lstUrlDfltCnf[6].ToString();

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
        ///
        /// <summary>
        /// Reformatting of the nickname/word entered in "Word_Construct_URL_Txt" before sending to => Construct_URL for URL construction
        /// </summary>
        /// <param name="Construct_URL"></param>
        /// Creation of URL with the nickname or search word
        /// 
        void Word_Construct_URL_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Opening. Closing prompt
        /// </summary>
        ///
        void Console_Btn_Click(object sender, EventArgs e)
        {
            Console_Cmd_Txt.Visible = !Console_Cmd_Txt.Visible;
        }

        void Mute_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                WBrowse.CoreWebView2.IsMuted = !WBrowse.CoreWebView2.IsMuted;

                bool isDocumentPlayingAudio = WBrowse.CoreWebView2.IsDocumentPlayingAudio;
                bool isMuted = WBrowse.CoreWebView2.IsMuted;

                if (isDocumentPlayingAudio)
                {
                    if (isMuted)
                        Mute_Btn.Image = (Bitmap)Resources.ResourceManager.GetObject("Mute");
                    else
                        Mute_Btn.Image = (Bitmap)Resources.ResourceManager.GetObject("Unmute");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Mute_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Copy the URL of the current web page to the clipboard
        /// </summary>
        /// 
        void CopyURL_Mnu_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Translation of the web page via the default translation site of the file "url_dflt_cnf.ost"
        /// or the one chosen by the user "Class_Var.URL_TRAD_WEBPAGE"
        /// </summary>
        /// <param name="replace_query"></param>
        /// Replacement characters for the URL to translate and open in wBrowser
        /// 
        void TraductPage_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Class_Var.URL_TRAD_WEBPAGE == "")
                    Class_Var.URL_TRAD_WEBPAGE = lstUrlDfltCnf[2].ToString();

                string formatURI = Regex.Replace(Class_Var.URL_TRAD_WEBPAGE, "replace_query", WBrowse.Source.AbsoluteUri);
                WBrowse.Source = new Uri(@formatURI);

            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! TraductPage_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Unshort URL
        /// </summary>
        /// <param name="StartUnshortUrl"></param>
        /// 
        void UnshortUrl_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Unshort URL
        /// </summary>
        /// <param name="Uri"></param>
        /// URL to check
        /// 
        void StartUnshortUrl(string Uri)
        {
            UnshortURLval = Uri;

            Thread Thr_UnshortUrl = new Thread(new ThreadStart(UnshortUrl));
            Thr_UnshortUrl.Start();
        }
        ///
        /// <summary>
        /// Unshort URL Http web request Header
        /// </summary>
        /// 
        void UnshortUrl()
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
        ///
        /// <summary>
        /// Execution Add-on of the add-on directory
        /// </summary>
        /// 
        void AddOn_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Tools_TAB_0.Focus();

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
        ///
        /// <summary>
        /// Loading the selected URL constructor file located in the “filesdir/url-constructor” directory
        /// </summary>
        ///
        void Construct_URL_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Tools_TAB_0.Focus();
                ConstructURL_Lst.Items.Clear();
                ConstructURL_Lst.Items.AddRange(File.ReadAllLines(FileDir + @"url-constructor\" + Construct_URL_Cbx.Text));
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Construct_URL_Cbx_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Opening in the "OpenSource_Frm" window of the file "filesdir/gh.txt" (Google Dork) if file exists True
        /// </summary>
        /// <param name="Open_Source_Frm">Opening the window</param>
        ///
        void GoogleDork_Btn_Click(object sender, EventArgs e)
        {
            if (File.Exists(FileDir + "gdork.txt"))
                Open_Source_Frm(FileDir + "gdork.txt");
        }

        async void WebpageToPng_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                var img = await TakeWebScreenshot();
                CreateNameAleat();
                img.Save(Pictures + Una + ".png", System.Drawing.Imaging.ImageFormat.Png);
                Beep(800, 200);

                Process.Start(Pictures + Una + ".png");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! WebpageToPng_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Opening the “Html Text Form” Text Browser window
        /// </summary>
        /// 
        public void HTMLtxt_Btn_Click(object sender, EventArgs e)
        {
            HtmlTextFrm = new HtmlText_Frm();
            HtmlTextFrm.Show();
        }
        ///
        /// <summary>
        /// Web page to Png
        /// </summary>
        /// <param name="currentControlClipOnly"></param>
        /// <returns></returns>
        /// wBrowser capture
        ///
        async Task<Image> TakeWebScreenshot(bool currentControlClipOnly = false)
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
        ///
        /// <summary>
        /// Web page to Png
        /// </summary>
        /// <param name="clipSize"></param>
        /// <returns></returns>
        /// 
        async Task<Bitmap> GetWebBrowserBitmap(Size clipSize)
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
        ///
        /// <summary>
        /// Opening the cookie file in the editor
        /// </summary>
        /// <param name="OpenFile_Editor"></param>
        /// <param value="filesdir/memo.txt"></param>
        ///
        void Cookie_Btn_Click(object sender, EventArgs e)
        {
            OpenFile_Editor(AppStart + "cookie.txt");
        }
        ///
        /// <summary>
        /// Opening the cookie injection element
        /// </summary>
        ///
        void SetCookie_Btn_Click(object sender, EventArgs e)
        {
            Cookie_Pnl.Visible = !Cookie_Pnl.Visible;
        }
        ///
        /// <summary>
        /// <param name="fileopen">Selection and Opening of file in the editor</param>
        /// </summary>
        /// <param name="OpenFile_Editor"></param>
        /// <param value="filePath">Selected file</param>
        ///
        void OpnFilOnEditor_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(AppStart + "OstiumE.exe"))
                {
                    MessageBox.Show("The OstiumE editor is missing!", "Missing editor");
                    return;
                }

                string fileopen = openfile.Fileselect(AppStart, "txt files (*.txt)|*.txt|All files (*.*)|*.*", 2);

                if (fileopen != "")
                    OpenFile_Editor(fileopen);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnFilOnEditor_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Opening URL List in the "OpenSource_Frm" window
        /// </summary>
        /// <param name="Open_Source_Frm"></param>
        /// <param value="filePath"></param>
        /// Selected file
        /// 
        void OpenListLink_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string fileopen = openfile.Fileselect(AppStart, "txt files (*.txt)|*.txt|All files (*.*)|*.*", 2);

                if (fileopen != "")
                    Open_Source_Frm(fileopen);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenListLink_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// MDI Form open
        /// </summary>
        /// 
        void OpnGroupFrm_Btn_Click(object sender, EventArgs e)
        {
            mdiFrm = new Mdi_Frm();
            mdiFrm.Show();
        }
        ///
        /// <summary>
        /// Opening the memo file in the editor
        /// </summary>
        /// <param name="OpnFileOpt"></param>
        /// <param value="filesdir/memo.txt"></param>
        /// 
        public void Memo_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(AppStart + "memo.txt");
        }
        ///
        /// <summary>
        /// Checking the existence and opening of the default editor "Class_Var.DEFAULT_EDITOR"
        /// </summary>
        /// 
        void Editor_Btn_Click(object sender, EventArgs e)
        {
            if (File.Exists(Class_Var.DEFAULT_EDITOR))
                Process.Start(Class_Var.DEFAULT_EDITOR);
            else
                MessageBox.Show("File Editor are not exist in directory, verify your config file!", "Error!");
        }

        void OpnDirectory_Btn_Click(object sender, EventArgs e)
        {
            Process.Start(AppStart);
        }

        void IndexDir_Btn_Click(object sender, EventArgs e)
        {
            WBrowse.Source = new Uri(@AppStart);
        }
        ///
        /// <summary>
        /// JS script injection into web page
        /// </summary>
        /// <param name="InputBox"></param>
        /// Enter the script
        /// <param value="ScriptInject"></param>
        /// Script to inject
        /// 
        async void InjectScript_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string message, title;
                object ScriptSelect;

                message = "Enter some JavaScript to be executed in the context of this page. Don't use scripts you don't understand!";
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

        void OpenScriptEdit_Btn_Click(object sender, EventArgs e)
        {
            scriptCreatorFrm = new ScriptCreator();
            scriptCreatorFrm.Show();
        }
        ///
        /// <summary>
        /// RegEX injection in web page
        /// </summary>
        /// <param name="InputBox">Enter the script</param>
        /// <param value="ScriptInject">Script to inject</param>
        ///
        void RegexCmd_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Reset default configuration
        /// </summary>
        /// <param name="CreateConfigFile"></param>
        /// <param value="0">Loading default URLs from the "url_dflt_cnf.ost" file</param>
        /// <param value="1">Save with the parameters chosen by the user</param>
        /// 
        void ResetConfig_Btn_Click(object sender, EventArgs e)
        {
            string message = "Reset Config?";
            string caption = "Config";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;
            CreateConfigFile(0);
        }

        void JavaEnableDisable_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Backup/Compression of selected directories with 7zip creation of a ".bat" file, creation of the "Archives.zip" archive
        /// </summary>
        /// <param name="7zip"></param>
        /// <param name="Archive-DB-FILES-FEED.bat"></param>
        ///
        void ArchiveDirectory_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter instxt = new StreamWriter(AppStart + "Archive-DB-FILES-FEED.bat"))
                {
                    instxt.WriteLine("@echo off");
                    instxt.WriteLine("echo ".PadRight(39, '-'));
                    instxt.WriteLine("echo          Ostium by ICAZA MEDIA");
                    instxt.WriteLine("echo ".PadRight(39, '-'));
                    instxt.WriteLine("@echo.");
                    instxt.WriteLine("echo Backup: DATABASE - FEED - FILES");
                    instxt.WriteLine("@echo.");
                    instxt.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + DBdirectory + " -mx9 -mtc=on");
                    instxt.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + FeedDir + " -mx9 -mtc=on");
                    instxt.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + FileDir + " -mx9 -mtc=on");
                    instxt.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + Workflow + " -mx9 -mtc=on");
                    instxt.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + Scripts + " -mx9 -mtc=on");
                    instxt.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + Setirps + " -mx9 -mtc=on");
                    instxt.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + MapDir + " -mx9 -mtc=on");
                    instxt.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + JsonDir + " -mx9 -mtc=on");
                    instxt.WriteLine("pause");
                }
                Process.Start(AppStart + "Archive-DB-FILES-FEED.bat");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ArchiveDirectory_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// Checking for updates (manual)
        /// <param value="0">Message only if update available</param>
        /// <param value="1">False or True update message</param>
        ///
        void OstUpdt_Btn_Click(object sender, EventArgs e)
        {
            VerifyUPDT("Ostium", 1);
        }
        ///
        /// <param name="GoBrowser"></param>
        /// <param name="WebviewRedirect"></param>
        /// <param value="0">Open parent window</param>
        /// <param value="1">Opens in a new window</param>
        ///
        void HomePage_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser("https://veydunet.com/ostium/home.html", 0);
        }

        void Credit_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(AppStart + "credits.txt");
        }

        void CreateBokmark_Btn_Click(object sender, EventArgs e)
        {
            bookmarkletsFrm = new Bookmarklets_Frm();
            bookmarkletsFrm.Show();
        }

        #endregion

        #region Tools_Tab_1
        ///
        /// <summary>
        /// Opening RSS feed category in the editor
        /// </summary>
        /// <param name="OpenFile_Editor"></param>
        /// <param name="CategorieFeed_Cbx">Selection of the category to open</param>
        /// 
        void OpnFileCategory_Btn_Click(object sender, EventArgs e)
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

        void CopyURLfeed_Mnu_Click(object sender, EventArgs e)
        {
            string textData = WBrowsefeed.Source.AbsoluteUri;
            Clipboard.SetData(DataFormats.Text, textData);
            Beep(1500, 400);
        }
        ///
        /// <summary>
        /// Translation and display of the web page via the default translation site of the "url_dflt_cnf.ost" file or the one chosen by
        /// the user "Class_Var.URL_TRAD_WEBPAGE"
        /// </summary>
        /// <param name="replace_query">Replacement value with URL</param>
        /// 
        void TraductPageFeed_Btn_Click(object sender, EventArgs e)
        {
            if (Class_Var.URL_TRAD_WEBPAGE == "")
                Class_Var.URL_TRAD_WEBPAGE = lstUrlDfltCnf[2].ToString();

            string formatURI = Regex.Replace(Class_Var.URL_TRAD_WEBPAGE, "replace_query", WBrowsefeed.Source.AbsoluteUri);
            WBrowsefeed.Source = new Uri(@formatURI);
        }

        void JavaEnableDisableFeed_Btn_Click(object sender, EventArgs e)
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

        void NewProject_Tls_Click(object sender, EventArgs e)
        {
            Reset();
        }
        ///
        /// <summary>
        /// Delete project WorkFlow 
        /// </summary>
        /// 
        void DeleteProject_Tls_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Displaying the WorkFlow project XML file in wBrowser
        /// </summary>
        ///
        void ViewXml_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text == "")
                return;

            if (File.Exists(Workflow + NameProjectwf_Txt.Text + ".xml"))
            {
                WBrowse.Source = new Uri(Workflow + NameProjectwf_Txt.Text + ".xml");

                CtrlTabBrowsx();
                Control_Tab.SelectedIndex = 0;
            }                
        }
        ///
        /// <summary>
        /// Opening the WorkFlow project XML file in the editor
        /// </summary>
        /// <param name="OpenFile_Editor"></param>
        /// <param value="NameProjectwf_Txt">Selected WorkFlow project</param>
        /// 
        void EditXml_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text == "")
                return;

            if (File.Exists(Workflow + NameProjectwf_Txt.Text + ".xml"))
            {
                OpenFile_Editor(Workflow + NameProjectwf_Txt.Text + ".xml");
            }            
        }
        ///
        /// <summary>
        /// Export of the WorkFlow project in XML format
        /// </summary>
        ///
        void ExportXml_Tls_Click(object sender, EventArgs e)
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

                        if (result != DialogResult.Yes)
                            return;
                        else
                            goto Export;
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
        ///
        /// <summary>
        /// Export of the WorkFlow project in Json format
        /// </summary>
        /// 
        void ExportJson_Tls_Click(object sender, EventArgs e)
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

                        if (result != DialogResult.Yes)
                            return;
                        else
                            goto Export;
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
        ///
        /// <summary>
        /// Creation of a Json data diagram of a WorkFlow XML project with plantUML.jar (#Model 1)
        /// </summary>
        /// <param name="ConvertJson">Converting WorkFlow XML project to Json format</param>
        /// <param value="1">Add the script header "@startjson" "!theme" then end "@endjson" for plantUML in the file to transform it into SVG</param>
        /// <param value="0">Simple conversion of XML file to Json format no header</param>
        /// <param name="Commut"></param>
        /// <param value="0">The diagram creation files are in the diagram directory these are the XML files of the WorkFlow projects</param>
        /// <param value="1">The diagramming files are in a random directory they are plantUML files</param>
        /// <param name="CreateDiagram_Thrd"></param>
        /// <param value="0">The diagram creation file is in the diagram directory</param>
        /// <param value="1">The diagram creation file is in a random directory</param>
        ///
        void Diagram_Tls_Click(object sender, EventArgs e)
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

            if (File.Exists(DiagramDir + NameProjectwf_Txt.Text + ".svg"))
                File.Delete(DiagramDir + NameProjectwf_Txt.Text + ".svg");

            MessageBox.Show(MessageStartDiagram);
            Timo.Enabled = true;

            ConvertJson(Workflow + NameProjectwf_Txt.Text + ".xml", DiagramDir + NameProjectwf_Txt.Text + ".txt", 1);

            FileDiag = NameProjectwf_Txt.Text + ".svg";
            Commut = 0;

            Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd(NameProjectwf_Txt.Text + ".txt", 0));
            CreateDiagram.Start();
        }
        ///
        /// <summary>
        /// Converting the WorkFlow project XML file to Json format or converting the XML file for conversion to an SVG diagram
        /// </summary>
        /// <param name="xmlFileConvert">XML file to convert to Json format</param>
        /// <param name="dirFileConvert">Dir for saving the final file</param>
        /// <param name="value"></param>
        /// <param value="1">Adds the script header "@startjson" "!theme" then end "@endjson" for plantUML in the created file to transform it into SVG</param>
        /// <param value="0">Simple conversion of XML file to Json format no header</param>
        /// 
        void ConvertJson(string xmlFileConvert, string dirFileConvert, int value)
        {
            try
            {
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
                        file_create.Write("@endjson");

                    file_create.Close();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ConvertJson: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Creation of Json data diagram with plantUML.jar (#Model 1)
        /// </summary>
        /// <param name="fileselect">File to create</param>
        /// <param name="value"></param>
        /// <param value="0">The diagram creation files are in the diagram directory these are the XML files of the WorkFlow projects</param>
        /// <param value="1">The diagramming files are in a random directory they are plantUML files</param>
        ///
        void CreateDiagram_Thrd(string fileselect, int value)
        {
            try
            {
                string argumentsIs = "";

                if (value == 0)
                {
                    argumentsIs = "java -jar plantuml.jar " + DiagramDir + fileselect + " -tsvg -charset UTF-8";
                }
                else if (value == 1)
                {
                    argumentsIs = "java -jar plantuml.jar " + fileselect + " -tsvg -charset UTF-8";
                }              

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
        ///
        /// <summary>
        /// Creation of a MindMap diagram of a WorkFlow XML project with plantUML.jar (#Model 2)
        /// </summary>
        /// <param name="CreateDiagramMinMapFile_Thrd"></param>
        /// <param value="0">"+" formatting character for a non-detailed MindMap Diagram (#Model 3)</param>
        /// <param value="1">"*" formatting character for a detailed MindMap Diagram (#Model 2)</param>
        /// 
        void DiagramMindMap_Tls_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Creation of a MindMap diagram of a WorkFlow XML project with plantUML.jar (#Model 3)
        /// </summary>
        /// <param name="CreateDiagramMinMapFile_Thrd"></param>
        /// <param value="0">"+" formatting character for a non-detailed MindMap Diagram (#Model 3)</param>
        /// <param value="1">"*" formatting character for a detailed MindMap Diagram (#Model 2)</param>
        ///
        void DiagramMindMap2_Tls_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Creation of a MindMap diagram of a WorkFlow XML project with plantUML.jar (#Model 2) or (#Model 3)
        /// </summary>
        /// <param name="value"></param>
        /// <param value="0">"+" formatting character for a non-detailed MindMap Diagram (#Model 3)</param>
        /// <param value="1">"*" formatting character for a detailed MindMap Diagram (#Model 2)</param>
        /// <param name="Commut"></param>
        /// <param value="0">The diagram creation files are in the diagram directory these are the XML files of the WorkFlow projects</param>
        /// <param value="1">The diagramming files are in a random directory they are plantUML files</param>
        /// <param name="CreateDiagram_Thrd"></param>
        /// <param value="0">The diagram creation files are in the diagram directory these are the XML files of the WorkFlow projects</param>
        /// <param value="1">The diagramming files are in a random directory they are plantUML files</param>
        /// 
        void CreateDiagramMinMapFile_Thrd(int value)
        {
            try
            {
                if (File.Exists(DiagramDir + NameProjectwf_Txt.Text + ".txt"))
                    File.Delete(DiagramDir + NameProjectwf_Txt.Text + ".txt");

                if (File.Exists(DiagramDir + NameProjectwf_Txt.Text + ".svg"))
                    File.Delete(DiagramDir + NameProjectwf_Txt.Text + ".svg");

                string element;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Workflow + NameProjectwf_Txt.Text + ".xml");

                using (StreamWriter file_create = File.AppendText(DiagramDir + NameProjectwf_Txt.Text + ".txt"))
                {
                    file_create.WriteLine("@startmindmap");
                    file_create.WriteLine("skinparam titleBorderRoundCorner 15");
                    file_create.WriteLine("skinparam titleBorderThickness 2");
                    file_create.WriteLine("skinparam titleBorderColor red");
                    file_create.WriteLine("skinparam titleBackgroundColor Aqua-CadetBlue");
                    file_create.WriteLine("!theme " + ThemeDiag);
                    file_create.WriteLine("footer https://veydunet.com/ostium");
                    file_create.WriteLine("caption Ostium Osint Browser");
                    file_create.WriteLine("title " + NameProjectwf_Txt.Text);

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
                            string strtext = string.Format("{0}", nodeList[e].ChildNodes.Item(0).InnerText);
                            string strauth = string.Format("{0}", nodeList[e].Attributes.Item(0).InnerText);
                            string strdate = string.Format("{0}", nodeList[e].Attributes.Item(1).InnerText);
                            string strtime = string.Format("{0}", nodeList[e].Attributes.Item(2).InnerText);
                            string strnote = string.Format("{0}", nodeList[e].Attributes.Item(3).InnerText);
                            string strurl = string.Format("{0}", nodeList[e].Attributes.Item(4).InnerText);

                            using (StreamWriter file_create = File.AppendText(DiagramDir + NameProjectwf_Txt.Text + ".txt"))
                            {
                                if (value == 0)
                                {
                                    file_create.WriteLine("+++ " + strtext);
                                }
                                else if (value == 1)
                                {
                                    file_create.WriteLine("***:" + strnote);
                                    file_create.WriteLine("<code>");
                                    file_create.WriteLine(strtext);
                                    file_create.WriteLine(strurl);
                                    file_create.WriteLine("-----------");
                                    file_create.WriteLine(string.Format("{0} {1} {2}", strauth, strdate, strtime));
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

                FileDiag = NameProjectwf_Txt.Text + ".svg";
                Commut = 0;

                Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd(NameProjectwf_Txt.Text + ".txt", 0));
                CreateDiagram.Start();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateDiagramMinMapFile: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Creation of a Json diagram from a Json file with plantUML.jar
        /// Opening the file selected in "TmpFile_Txt.Text"
        /// </summary>
        /// <param name="Commut"></param>
        /// <param value="0">The diagram creation files are in the diagram directory these are the XML files of the WorkFlow projects</param>
        /// <param value="1">The diagram creation files are in a random directory they are TXT files in plantUML format</param>
        /// <param name="CreateDiagram_Thrd"></param>
        /// <param value="0">The diagram creation files are in the diagram directory these are the XML files of the WorkFlow projects</param>
        /// <param value="1">The diagram creation files are in a random directory they are TXT files in plantUML format</param>
        /// 
        void OpnJsonFile_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(DiagramDir + "plantuml.jar"))
                {
                    MessageBox.Show("Sorry, PlantUML is not install, go to Discord channel Ostium for fix and help.");
                    return;
                }

                TmpFile_Txt.Text = "";

                ThemeDiag = ThemDiag_Cbx.Text;
                if (ThemeDiag == "")
                    ThemeDiag = "cloudscape-design";

                string fileopen = openfile.Fileselect(AppStart, "json files (*.json)|*.json", 2);

                if (fileopen != "")
                {
                    MessageBox.Show(MessageStartDiagram);
                    Timo.Enabled = true;

                    if (File.Exists(DiagramDir + "temp_file.txt"))
                        File.Delete(DiagramDir + "temp_file.txt");

                    if (File.Exists(DiagramDir + "temp_file.svg"))
                        File.Delete(DiagramDir + "temp_file.svg");

                    using (StreamReader sr = new StreamReader(fileopen))
                    {
                        TmpFile_Txt.Text = sr.ReadToEnd();
                    }

                    using (StreamWriter file_create = new StreamWriter(DiagramDir + "temp_file.txt"))
                    {
                        file_create.WriteLine("@startjson");
                        file_create.WriteLine("!theme " + ThemeDiag);
                        file_create.WriteLine(TmpFile_Txt.Text);
                        file_create.Write("@endjson");
                    }

                    FileDiag = "temp_file.svg";
                    Commut = 0;

                    Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd("temp_file.txt", 0));
                    CreateDiagram.Start();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnJsonFile_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Creation of a Json diagram from an XML file with plantUML.jar
        /// Opening the file and converting to Json format
        /// </summary>
        /// <param name="ConvertJson">Converting WorkFlow XML project to Json format</param>
        /// <param value="1">Adds the script header "@startjson" "!theme" then end "@endjson" for plantUML in the created file to transform it into SVG</param>
        /// <param value="0">Simple conversion of XML file to Json format no header</param>
        /// <param name="Commut"></param>
        /// <param value="0">The diagram creation files are in the diagram directory these are the XML files of the WorkFlow projects</param>
        /// <param value="1">The diagram creation files are in a random directory they are TXT files in plantUML format</param>
        /// <param name="CreateDiagram_Thrd"></param>
        /// <param value="0">The diagram creation file is in the diagram directory</param>
        /// <param value="1">The diagram creation file is in a random directory</param>
        /// 
        void OpnXMLFile_Btn_Click(object sender, EventArgs e)
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

                    if (File.Exists(DiagramDir + "temp_file.svg"))
                        File.Delete(DiagramDir + "temp_file.svg");

                    ConvertJson(fileopen, DiagramDir + "temp_file.txt", 1);

                    FileDiag = "temp_file.svg";
                    Commut = 0;

                    Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd("temp_file.txt", 0));
                    CreateDiagram.Start();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnXMLFile_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Creation of a Json diagram from a plantUML format file with plantUML.jar
        /// </summary>
        /// <param name="Commut"></param>
        /// <param value="0">The diagram creation files are in the diagram directory these are the XML files of the WorkFlow projects</param>
        /// <param value="1">The diagram creation files are in a random directory they are TXT files in plantUML format</param>
        /// <param name="CreateDiagram_Thrd"></param>
        /// <param value="0">The diagram creation files are in the diagram directory these are the XML files of the WorkFlow projects</param>
        /// <param value="1">The diagram creation files are in a random directory they are TXT files in plantUML format</param>
        ///
        void OpnPlantUMLFile_Btn_Click(object sender, EventArgs e)
        {
            if (!File.Exists(DiagramDir + "plantuml.jar"))
            {
                MessageBox.Show("Sorry, PlantUML is not install, go to Discord channel Ostium for fix and help.");
                return;
            }

            string fileopen = openfile.Fileselect(AppStart, "txt files (*.txt)|*.txt", 2);

            if (fileopen != "")
            {
                MessageBox.Show(MessageStartDiagram);
                Timo.Enabled = true;

                string strName = Path.GetFileNameWithoutExtension(fileopen);
                string strDir = Path.GetDirectoryName(fileopen);

                FileDiag = strDir + @"\" + strName + ".svg";
                Commut = 1;

                Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd(fileopen, 1));
                CreateDiagram.Start();
            }
        }
        ///
        /// <summary>
        /// Export of the created diagram
        /// </summary>
        /// <param name="Commut"></param>
        /// <param value="0">output file name => UsenameRandom + "_" + FileDiag</param>
        /// <param value="1">output file name => UsenameRandom</param>
        /// 
        void ExportDiag_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (FileDiag == "")
                {
                    MessageBox.Show("SVG file not exist!");
                    return;
                }

                CreateNameAleat();
                string nameSVG = Una + "_" + FileDiag;
                string nameSVGb = Una;

                IcazaClass selectdir = new IcazaClass();
                string dirselect = selectdir.Dirselect();

                if (Commut == 0)
                {
                    File.Copy(DiagramDir + FileDiag, dirselect + @"\" + nameSVG);
                    MessageBox.Show("File [" + nameSVG + "] export.");
                }
                else if (Commut == 1)
                {
                    File.Copy(FileDiag, dirselect + @"\" + nameSVGb + ".svg");
                    MessageBox.Show("File [" + nameSVGb + ".svg] export.");
                }                                
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ExportDiag_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void ThemDiag_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tools_TAB_3.Focus();
        }

        void OpnSprites_Btn_Click(object sender, EventArgs e)
        {
            if (!File.Exists(AppStart + "setirps.exe"))
            {
                MessageBox.Show("Setirps is missing!", "Missing editor");
                return;
            }

            Process.Start(AppStart + "setirps.exe");
        }
        
        #endregion

        void Control_Tab_Click(object sender, EventArgs e)
        {
            switch (Control_Tab.SelectedIndex)
            {
                case 0:
                    {
                        CtrlTabBrowsx();
                        break;
                    }
                case 1:
                    {
                        Tools_TAB_0.Visible = false;
                        Tools_TAB_1.Visible = true;
                        Tools_TAB_3.Visible = false;
                        Tools_TAB_4.Visible = false;
                        Text = TmpTitleWBrowsefeed;
                        URLtxt_txt.Text = WBrowsefeed.Source.AbsoluteUri;
                        TableOpn_Lbl.Visible = false;
                        CountFeed_Lbl.Visible = true;
                        DBSelectOpen_Lbl.Visible = false;
                        TableCount_Lbl.Visible = false;
                        TableOpen_Lbl.Visible = false;
                        RecordsCount_Lbl.Visible = false;
                        LatTCurrent_Lbl.Visible = false;
                        Separator.Visible = false;
                        LonGtCurrent_Lbl.Visible = false;
                        ProjectMapOpn_Lbl.Visible = false;
                        TtsButton_Sts.Visible = false;

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
                        Tools_TAB_4.Visible = false;
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
                        LatTCurrent_Lbl.Visible = false;
                        Separator.Visible = false;
                        LonGtCurrent_Lbl.Visible = false;
                        ProjectMapOpn_Lbl.Visible = false;
                        TtsButton_Sts.Visible = false;

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
                        Tools_TAB_4.Visible = false;
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
                        LatTCurrent_Lbl.Visible = false;
                        Separator.Visible = false;
                        LonGtCurrent_Lbl.Visible = false;
                        ProjectMapOpn_Lbl.Visible = false;
                        TtsButton_Sts.Visible = false;
                        break;                        
                    }
                case 4:
                    Tools_TAB_0.Visible = false;
                    Tools_TAB_1.Visible = false;
                    Tools_TAB_3.Visible = false;
                    Tools_TAB_4.Visible = true;
                    URLtxt_txt.Text = "";
                    Text = "Map";
                    TableOpn_Lbl.Visible = false;
                    CountFeed_Lbl.Visible = false;
                    JavaDisable_Lbl.Visible = false;
                    JavaDisableFeed_Lbl.Visible = false;
                    DBSelectOpen_Lbl.Visible = false;
                    TableCount_Lbl.Visible = false;
                    TableOpen_Lbl.Visible = false;
                    RecordsCount_Lbl.Visible = false;
                    LatTCurrent_Lbl.Visible = true;
                    Separator.Visible = true;
                    LonGtCurrent_Lbl.Visible = true;
                    ProjectMapOpn_Lbl.Visible = true;
                    TtsButton_Sts.Visible = false;

                    if (VerifMapOpn == "off")
                    {
                        Mkmarker = GMarkerGoogleType.red_dot;
                        OpenMaps("Horta", 12); // Adresse, Provider
                    }
                    PointLoc_Lst.Items.Clear();
                    loadfiledir.LoadFileDirectory(MapDir, "xml", "lst", PointLoc_Lst);
                    break;
                case 5:
                    Tools_TAB_0.Visible = false;
                    Tools_TAB_1.Visible = false;
                    Tools_TAB_3.Visible = false;
                    Tools_TAB_4.Visible = false;
                    URLtxt_txt.Text = "";
                    Text = "Json";
                    TableOpn_Lbl.Visible = false;
                    CountFeed_Lbl.Visible = false;
                    JavaDisable_Lbl.Visible = false;
                    JavaDisableFeed_Lbl.Visible = false;
                    DBSelectOpen_Lbl.Visible = false;
                    TableCount_Lbl.Visible = false;
                    TableOpen_Lbl.Visible = false;
                    RecordsCount_Lbl.Visible = false;
                    LatTCurrent_Lbl.Visible = false;
                    Separator.Visible = false;
                    LonGtCurrent_Lbl.Visible = false;
                    ProjectMapOpn_Lbl.Visible = false;
                    TtsButton_Sts.Visible = false;

                    OutParse = JsonOutA_txt;
                    OutJson = JsonOutB_txt;
                    break;
                case 6:
                    Tools_TAB_0.Visible = false;
                    Tools_TAB_1.Visible = false;
                    Tools_TAB_3.Visible = false;
                    Tools_TAB_4.Visible = false;
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
                    LatTCurrent_Lbl.Visible = false;
                    Separator.Visible = false;
                    LonGtCurrent_Lbl.Visible = false;
                    ProjectMapOpn_Lbl.Visible = false;
                    TtsButton_Sts.Visible = false;

                    DireSizeCalc(AppStart, OstiumDir_Lbl);
                    DireSizeCalc(Plugins, AddOnDir_Lbl);
                    DireSizeCalc(DBdirectory, DatabseDir_Lbl);
                    DireSizeCalc(FeedDir, FeedDir_Lbl);
                    DireSizeCalc(Scripts, ScriptDir_Lbl);
                    DireSizeCalc(Workflow, WorkFlowDir_Lbl);
                    DireSizeCalc(WorkflowModel, WorkFlowModelDir_Lbl);
                    DireSizeCalc(Pictures, PictureDir_Lbl);
                    DireSizeCalc(WebView2Dir, WebView2Dir_Lbl);
                    DireSizeCalc(DiagramDir, DiagramDir_Lbl);
                    DireSizeCalc(Setirps, SpritesDir_Lbl);
                    DireSizeCalc(BkmkltDir, BkmkltDir_Lbl);
                    DireSizeCalc(MapDir, MapDir_Lbl);
                    break;
            }
        }

        void CtrlTabBrowsx()
        {
            Tools_TAB_0.Visible = true;
            Tools_TAB_1.Visible = false;
            Tools_TAB_3.Visible = false;
            Tools_TAB_4.Visible = false;
            Text = TmpTitleWBrowse;
            URLtxt_txt.Text = WBrowse.Source.AbsoluteUri;
            TableOpn_Lbl.Visible = true;
            CountFeed_Lbl.Visible = false;
            DBSelectOpen_Lbl.Visible = false;
            TableCount_Lbl.Visible = false;
            TableOpen_Lbl.Visible = false;
            RecordsCount_Lbl.Visible = false;
            LatTCurrent_Lbl.Visible = false;
            Separator.Visible = false;
            LonGtCurrent_Lbl.Visible = false;
            ProjectMapOpn_Lbl.Visible = false;
            TtsButton_Sts.Visible = true;

            if (JavaEnableDisable_Btn.Text == "Javascript Disable")
                JavaDisable_Lbl.Visible = true;

            JavaDisableFeed_Lbl.Visible = false;
        }
        ///
        /// <summary>
        /// URL construction from a list loaded with the URL construction file selected and
        /// created a temporary file <param name="A"></param> to open all URLs in the multi-window form Temp-Url-Construct.txt
        /// </summary>
        /// <param name="replace_query">Replacement value with the searched nickname/word</param>
        /// 
        void Construct_URL(string URLc)
        {
            try 
            {
                URLbrowse_Cbx.Items.Clear();
                string A = AppStart + @"\filesdir\grp-frm\Temp-Url-Construct.txt";

                for (int i = 0; i < ConstructURL_Lst.Items.Count; i++)
                {
                    string formatURI = Regex.Replace(ConstructURL_Lst.Items[i].ToString(), "replace_query", URLc);
                    URLbrowse_Cbx.Items.Add(formatURI);
                }

                if (File.Exists(A))
                    File.Delete(A);

                using (StreamWriter SW = new StreamWriter(A, true))
                {
                    foreach (string itm in URLbrowse_Cbx.Items)
                        SW.WriteLine(itm);
                }

                URLbrowse_Cbx.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Construct_URL: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Downloading and saving the source of the current WEB page overwriting the previous, only remote files. This operation is 
        /// carried out in order to respond to certain analysis operations according to demand, without having to multiply queries
        /// </summary>
        /// 
        async void Download_Source_Page()
        {
            try
            {
                if (Class_Var.URL_USER_AGENT_SRC_PAGE == "")
                    Class_Var.URL_USER_AGENT_SRC_PAGE = lstUrlDfltCnf[5].ToString();

                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.UserAgent.ParseAdd(Class_Var.URL_USER_AGENT_SRC_PAGE);

                var response = await client.GetAsync(WBrowse.Source.AbsoluteUri);
                string pageContents = await response.Content.ReadAsStringAsync();

                File_Write(AppStart + "sourcepage", pageContents);
            }
            catch
            { }
        }

        void CreateNameAleat()
        {
            Una = DateTime.Now.ToString("d").Replace("/", "_") + "_" + DateTime.Now.ToString("HH:mm:ss").Replace(":", "_");
        }

        #region Prompt_

        void OnKeyConsole_Cmd_Txt(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CMD_Console(Console_Cmd_Txt.Text);
                Console_Cmd_Txt.Text = "";
                Console_Cmd_Txt.Text = "> ";
                Console_Cmd_Txt.Select(Console_Cmd_Txt.Text.Length, 0);
            }
        }
        ///
        /// <summary>
        /// Executing commands in the console
        /// </summary>
        /// <param name="Cmd">Command entered in the console processed in the switch</param>
        /// <param name="CMD_Console_Exec">regEX command to execute</param>
        /// <param name="yn"></param>
        /// <param value="0">CMD_Console_Exec False</param>
        /// <param value="1">CMD_Console_Exec True</param>
        /// 
        void CMD_Console(string Cmd)
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
        ///
        /// <summary>
        /// Command regEX preformatted or random
        /// </summary>
        /// <param name="cmdSwitch">Command preformatted regEX => links | word | text link | word without duplicate</param>
        /// <param name="regxCmd">Random regEX comand entered by user ==> regex</param>
        ///
        void CMD_Console_Exec(int cmdSwitch, string regxCmd)
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
        ///
        /// <summary>
        /// File creation with overwriting if the file exists
        /// </summary>
        /// <param name="fileName">Name of the file to create</param>
        /// <param name="content">Data of the file to register</param>
        /// 
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
                senderror.ErrorLog("Error! File_Write: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Creation of List file
        /// </summary>
        /// <param name="nameFile">List file name</param>
        /// <param name="opnOrNo"></param>
        /// <param value="yes">Saves the file and opens the list in the "Open_Source_Frm" window</param>
        /// <param value="no">Save the file without displaying it</param>
        ///
        void List_Create(string nameFile, string opnOrNo)
        {
            try
            {
                if (File.Exists(nameFile))
                    File.Delete(nameFile);

                var ListElements = nameFile;
                using (StreamWriter SW = new StreamWriter(ListElements, true))
                {
                    foreach (string itm in List_Object.Items)
                    {
                        SW.WriteLine(itm);
                    }
                }

                if (opnOrNo == "yes")
                    Open_Source_Frm(nameFile);
                else
                    Beep(1200, 200);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! List_Create: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// File creation in add mode
        /// </summary>
        /// <param name="fileName">Name of the file to create</param>
        /// <param name="fileValue">File data to register</param>
        /// 
        void CreateData(string fileName, string fileValue)
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
        ///
        /// <summary>
        /// Opening file to "OpenFile_Editor"
        /// </summary>
        /// <param name="dir_dir">Checking the existence of the file if False it is created then opened with the editor</param>
        /// 
        void OpnFileOpt(string dir_dir)
        {
            if (!File.Exists(AppStart + "OstiumE.exe"))
            {
                MessageBox.Show("The OstiumE editor is missing!", "Missing editor");
                return;
            }

            if (!File.Exists(dir_dir))
                File_Write(dir_dir, "");

            OpenFile_Editor(dir_dir);
        }

        #endregion
        ///
        /// <summary>
        /// Opening a file with the editor
        /// </summary>
        /// <param name="FileSelect">File to open</param>
        ///
        void OpenFile_Editor(string FileSelect)
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
        ///
        /// <summary>
        /// Opening file in the "Doc_Frm" window
        /// </summary>
        /// <param name="fileNameSelect">File to open</param>
        /// 
        void Open_Doc_Frm(string fileSelect)
        {
            Class_Var.File_Open = fileSelect;
            docForm = new Doc_Frm();
            docForm.Show();
        }
        ///
        /// <summary>
        /// Opening file in the "OpenSource_Frm" window
        /// </summary>
        /// <param name="fileNameSelect">File to open</param>
        /// 
        void Open_Source_Frm(string fileSelect)
        {
            Class_Var.File_Open = fileSelect;
            openSourceForm = new OpenSource_Frm();
            openSourceForm.Show();
        }

        #region Database_OpenAdd
        ///
        /// <summary>
        /// Opening URL category in wBrowser
        /// </summary>
        /// <param name="Sqlite_ReadUri">Opens the URL and displays it in wBrowser</param>
        ///
        void URL_SAVE_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Tools_TAB_0.Focus();

                Cbx_Object = URL_SAVE_Cbx;
                string usct = URL_SAVE_Cbx.Text;

                Sqlite_ReadUri("SELECT * FROM " + TableOpen + " WHERE url_name = '" + usct + "'", "url_adress");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! URL_SAVE_Cbx_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Displays the database URL section in add mode
        /// </summary>
        /// <param name="Tables_Lst_Opt"></param>
        /// <param value="add">Adding a URL to the database</param>
        /// <param value="open">Opening a database URL</param>
        ///
        void AddURLink_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Displays the database URL section in reading mode
        /// </summary>
        /// <param name="Tables_Lst_Opt"></param>
        /// <param value="add">Adding a URL to the database</param>
        /// <param value="open">Opening a database URL</param>
        ///
        void OpnURL_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Displays the database URL section
        /// </summary>
        /// <param name="OpnAllTable()">Opens and displays all database URL categories</param>
        ///
        void DatabasePnl()
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
        ///
        /// <summary>
        /// Add Table, adding a new category to the database
        /// </summary>
        /// <param name="Sqlite_Cmd">mysql command to execute</param>
        ///
        void AddTable_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Opening a database category or adding a URL
        /// </summary>
        /// <param name="Tables_Lst_Opt"></param>
        /// <param value="add">Adding a URL to the database</param>
        /// <param value="open">Opening a database URL</param>
        /// <param name="tlsi">var => Table List Selected Item (category) of the database</param>
        ///
        void Tables_Lst_SelectedIndexChanged(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Opening all categories of the database and display
        /// </summary>
        /// <param name="Sqlite_Read">mysql command to execute</param>
        /// <param name="List_Object">CBX or List object to load</param>
        /// 
        void OpnAllTable()
        {
            Tables_Lst.Items.Clear();
            List_Object = Tables_Lst;

            Sqlite_Read("SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1", "name", "lst");
        }
        ///
        /// <summary>
        /// Execution of mysql commands
        /// </summary>
        /// <param name="execSql">Command to execute</param>
        /// <param name="DBadmin"></param>
        /// <param value="on">Random mysql commands entered by the user in the TAB Data database management section</param>
        /// <param value="off">Open preformatted mysql commands | Add | Delete | Change</param>
        /// 
        void Sqlite_Cmd(string execSql)
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
        ///
        /// <summary>
        /// Reading from the database
        /// </summary>
        /// <param name="execSql">mysql command to execute</param>
        /// <param name="valueDB">Table value to load</param>
        /// <param name="ObjLstCbx">Selection of the type of object to load List or CBX</param>
        ///
        void Sqlite_Read(string execSql, string valueDB, string ObjLstCbx)
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
        ///
        /// <summary>
        /// Loading database URL in wBrowser or in the "DataValue_Opn" Textbox of the "TAB Data" section
        /// </summary>
        /// <param name="DBadmin"></param>
        /// <param value="on">Opening URL in Textbox DataValue_Opn</param>
        /// <param value="off">Opening the URL in wBrowser</param>
        /// 
        void Sqlite_ReadUri(string execSql, string valueDB)
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
        ///
        /// <summary>
        /// Change of default DataBase and creation if no exists
        /// </summary>
        /// <param name="ChangeDBdefault">Saving the new default database in the config.xml file</param>
        /// 
        void ChangeDefDB_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataBaze_Opn.Text != "")
                {
                    if (File.Exists(DBdirectory + DataBaze_Opn.Text))
                    {
                        string message = "Change default Database?";
                        string caption = "Database default";
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
                        string message = "The Database not exist, create first!";
                        string caption = "Database default";
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
        ///
        /// <summary>
        /// mysql command from the "TAB Data" section
        /// </summary>
        /// <param name="Sqlite_Read">mysql command</param>
        ///
        void ExecuteCMDsql_Btn_Click(object sender, EventArgs e)
        {
            DataValue_Lst.Items.Clear();
            List_Object = DataValue_Lst;

            Sqlite_Read(ExecuteCMDsql_Txt.Text, ValueCMDsql_Txt.Text, "lst");
        }
        ///
        /// <summary>
        /// Delete Table (category)
        /// </summary>
        ///
        void Db_Delete_Table_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Delete an entry (category url)
        /// </summary>
        ///
        void Db_Delete_Table_Value_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Delete All entries (all urls in a category)
        /// </summary>
        ///
        void Db_Delete_Table_AllValue_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Rename the URL of a database category
        /// </summary>
        /// 
        void Db_Update_Name_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Modify the value of a URL of a database category
        /// </summary>
        ///
        void Db_Update_Value_Btn_Click(object sender, EventArgs e)
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

        void DataBaze_Lst_SelectedIndexChanged(object sender, EventArgs e)
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

        void DataTable_Lst_SelectedIndexChanged(object sender, EventArgs e)
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

        void DataValue_Lst_SelectedIndexChanged(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Change of the default database and modification in the "config.xml" file
        /// </summary>
        /// <param name="NameDB">Database name</param>
        /// 
        void ChangeDBdefault(string NameDB)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(AppStart + "config.xml");
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Xwparsingxml/Xwparsingnode/DB_USE_DEFAULT") is XmlElement nod)
                {
                    nod.InnerText = NameDB;
                }

                xmlReader.Close();
                doc.Save(AppStart + "config.xml");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ChangeDBdefault: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void ValueChange_Txt_Click(object sender, EventArgs e)
        {
            ValueChange_Txt.ForeColor = Color.White;
            ValueChange_Txt.Text = "";
        }

        void Db_OpnLink_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser(DataValue_Opn.Text, 0);
            CtrlTabBrowsx();
            Control_Tab.SelectedIndex = 0;
        }

        void Db_OrderLst_Btn_Click(object sender, EventArgs e)
        {
            if (DataValue_Lst.Sorted)
            {
                Db_OrderLst_Btn.ForeColor = Color.White;
                DataValue_Lst.Sorted = false;
            }
            else
            {
                Db_OrderLst_Btn.ForeColor = Color.Lime;
                DataValue_Lst.Sorted = true;
            }            
        }

        #endregion

        #region Feed_

        void Title_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Title_Lst.SelectedIndex != -1)
                WBrowsefeed.Source = new Uri(Link_Lst.Items[Title_Lst.SelectedIndex].ToString());
        }
        ///
        /// <summary>
        /// Loading the RSS feed category into List or administering the RSS feed category file
        /// </summary>
        /// <param name="ManageFeed"></param>
        /// <param name="on">Opening rss feed category file for editing</param>
        /// <param name="off">Loading the category's RSS feed</param>
        /// <param name="LoadFeed"></param>
        /// <param value="0">Cleaning the List before loading</param>
        ///
        void CategorieFeed_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tools_TAB_1.Focus();

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
                CollapseTitleFeed_Btn.Text = "Collapse Off";
            }
        }
        ///
        /// <summary>
        /// RSS feed selection site by site
        /// </summary>
        ///
        void CountBlockSite_Lbl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CountBlockSite_Lbl.SelectedIndex != -1)
            {
                CountBlockFeed_Lbl.Items[CountBlockSite_Lbl.SelectedIndex].ToString();
                int value = Convert.ToInt32(CountBlockFeed_Lbl.Items[CountBlockSite_Lbl.SelectedIndex].ToString());

                Title_Lst.SetSelected(value, true);
            }
        }
        ///
        /// <summary>
        /// Loading the list of Flows
        /// </summary>
        /// <param name="value">RSS Feed URL for the category </param>
        /// <param name="FeedTitle">Loading RSS feed sites</param>
        /// <param name="ListCount">Counter of the number of Titles per RSS feed site</param>
        /// <param name="ItemTitleAdd">Added title in "Title_Lst"</param>
        /// <param name="ItemLinkAdd">Added title URL in Link_Lst</param>
        /// <param name="CountFeed">Display of the number of titles in "Title_Lst"</param>
        /// <param name="AllTrue">Enable elements</param>
        /// <param name="Msleave">Overlaying the RSS feed site window on top</param>
        ///
        void LoadFeed(int value)
        {
            string URL = "";
            int i = value;

            if (i == 0)
                Invoke(new Action<string>(CountBlockSite_Invk), "Clear");

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

        void CreatCategorie_Btn_Click(object sender, EventArgs e)
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

        void NewCategory_Txt_Click(object sender, EventArgs e)
        {
            NewCategory_Txt.ForeColor = Color.White;
            NewCategory_Txt.Text = "";
        }

        void NewFeed_Txt_Click(object sender, EventArgs e)
        {
            NewFeed_Txt.ForeColor = Color.White;
            NewFeed_Txt.Text = "";
        }

        void AddFeed_Btn_Click(object sender, EventArgs e)
        {
            if (NewFeed_Txt.Text != "" && NewFeed_Txt.Text != "new feed")
            {
                if (CategorieFeed_Cbx.Text != "")
                {
                    CreateData(FeedDir + CategorieFeed_Cbx.Text, NewFeed_Txt.Text);

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

        void ManageFeed_Btn_Click(object sender, EventArgs e)
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

        void DeleteCatfeed_Btn_Click(object sender, EventArgs e)
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

        void DeleteURLfeed_Btn_Click(object sender, EventArgs e)
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

        void CollapseTitleFeed_Btn_Click(object sender, EventArgs e)
        {
            splitContain_Rss.Panel1Collapsed = !splitContain_Rss.Panel1Collapsed;

            if (splitContain_Rss.Panel1Collapsed == false)
                CollapseTitleFeed_Btn.Text = "Collapse Off";
            else
                CollapseTitleFeed_Btn.Text = "Collapse On";
        }
        ///
        /// <summary>
        /// Select a title by its index number
        /// </summary>
        /// 
        void GoFeed_Btn_Click(object sender, EventArgs e)
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

        void GoFeed_Txt_Click(object sender, EventArgs e)
        {
            GoFeed_Txt.Text = "";
        }
        ///
        /// <summary>
        /// Opening the Home Page if configured "@Class_Var.URL_HOME" or opening the default page of the file "url_dflt_cnf.ost"
        /// </summary>
        ///
        void HomeFeed_Btn_Click(object sender, EventArgs e)
        {
            WBrowsefeed.Source = new Uri(HomeUrlRSS);
            CountFeed_Lbl.Text = "";
        }

        #endregion

        #region FeedSpeak

        void SpeakOpenPnl_Btn_Click(object sender, EventArgs e)
        {
            if (!Speak_Pnl.Visible)
            {
                Speak_Pnl.Visible = true;
                
                if (VerifLangOpn == 0)
                    LoadLang();

                VolumeValue_Lbl.Text = "Volume: " + Convert.ToString(VolumeVal_Track.Value);
                RateValue_Lbl.Text = "Rate: " + Convert.ToString(RateVal_Track.Value);
            }
            else
            {
                Speak_Pnl.Visible = false;
            }
        }

        void ReadTitle_Btn_Click(object sender, EventArgs e)
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

        void ReadClipB_Btn_Click(object sender, EventArgs e)
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

        void PauseSpeak_Btn_Click(object sender, EventArgs e)
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

        void StopSpeak_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                synth.SpeakAsyncCancelAll();
                PauseSpeak_Btn.Text = "Pause";
            }
            catch
            { }
        }

        void VolumeVal_Track_Scroll(object sender, EventArgs e)
        {
            Class_Var.VOLUME_TRACK = VolumeVal_Track.Value;
            VolumeValue_Lbl.Text = "Volume: " + Convert.ToString(VolumeVal_Track.Value);
        }

        void RateVal_Track_Scroll(object sender, EventArgs e)
        {
            Class_Var.RATE_TRACK = RateVal_Track.Value;
            RateValue_Lbl.Text = "Rate: " + Convert.ToString(RateVal_Track.Value);
        }

        void SaveVolRat_Btn_Click(object sender, EventArgs e)
        {
            SaveVolumeRate("VOLUME_TRACK_VAR", Class_Var.VOLUME_TRACK);
            SaveVolumeRate("RATE_TRACK_VAR", Class_Var.RATE_TRACK);

            Beep(800, 200);
        }

        void LoadLang()
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

        void SaveVolumeRate(string nodeselect, int value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(AppStart + "config.xml");
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Xwparsingxml/Xwparsingnode/" + nodeselect) is XmlElement nod)
                {
                    nod.InnerText = Convert.ToString(value);
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

        void CountBlockSite_Lbl_MouseEnter(object sender, EventArgs e)
        {
            MSenter();
        }

        void CountBlockSite_Lbl_Leave(object sender, EventArgs e)
        {
            MSleave();
        }

        void MSenter()
        {
            CountBlockSite_Lbl.BringToFront();
            CountBlockSite_Lbl.Width = 250;
            CountBlockSite_Lbl.Visible = true;
        }

        void MSleave()
        {
            CountBlockSite_Lbl.SendToBack();
            CountBlockSite_Lbl.Width = 50;
        }

        #region Param_

        void Download_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("edge://downloads", 0);
        }

        void History_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("edge://history", 0);
        }

        async void DevTools_Param_Click(object sender, EventArgs e)
        {
            await WBrowse.EnsureCoreWebView2Async();
            WBrowse.CoreWebView2.OpenDevToolsWindow();
        }

        void EdgeURL_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("edge://edge-urls", 0);
        }

        void AdvancedOption_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("about:flags", 0);
        }

        void SiteEngament_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("edge://site-engagement", 0);
        }

        private void ClrHistory_Param_Click(object sender, EventArgs e)
        {
            ClearData(0);
        }

        void System_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("edge://system", 0);
        }

        #endregion
        /// <summary>
        /// Cookies save
        /// </summary>
        /// <param name="URLs">Saved cookies only if SaveCookies_Chk checked = True, by default is False</param>
        async void GetCookie(string URLs)
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
        ///
        /// <summary>
        /// Cookie injection if True
        /// </summary>
        /// CookieName_Txt | CookieValue_Txt | CookieValue_Txt
        ///
        void SetCookie_Chk_CheckedChanged(object sender, EventArgs e)
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

        private void SaveCookies_Chk_CheckedChanged(object sender, EventArgs e)
        {
            if (SaveCookies_Chk.Checked)
                Class_Var.COOKIES_SAVE = 1; // Save
            else
                Class_Var.COOKIES_SAVE = 0; // No save
        }

        #region Workflow
        ///
        /// <summary>
        /// Creation of the XML file for a new WorkFlow project
        /// Word Duplicate Check
        /// Delete Space and Line Break
        /// </summary>
        ///
        void CreateXMLwf_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (AddItemswf_Txt.Text == "")
                    return;

                if (NameProjectwf_Txt.Text == "")
                {
                    NameProjectwf_Txt.BackColor = Color.Red;
                    MessageBox.Show("Select Project name first!");
                    NameProjectwf_Txt.BackColor = Color.FromArgb(28, 28, 28);
                    return;
                }
                ///
                /// Verifie si doublons
                /// 
                ListBox WordVerify = new ListBox();
                ListBox WordCompare = new ListBox();

                var fileTmp = @"tmp.txt";

                File_Write(fileTmp, "AddItemswf_Txt.Text");

                WordVerify.Items.AddRange(File.ReadAllLines(@"tmp.txt"));

                for (int i = 0; i < WordVerify.Items.Count; i++)
                {
                    int x;
                    x = WordCompare.FindStringExact(WordVerify.Items[i].ToString());
                    if (x == -1)
                    {
                        WordCompare.Items.Add(WordVerify.Items[i].ToString());
                    }
                    else
                    {
                        MessageBox.Show("Word [" + WordVerify.Items[i].ToString() + "] duplicate in your workflow, correct this for continue!");
                        return;
                    }
                }

                List_Wf = new ListBox();
                ///
                /// Adding items and Formatting => removing spaces and line breaks
                /// 
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
                ModelEdit_Btn.Enabled = false;
                ModelList_Lst.ClearSelected();

                Beep(1000, 400);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateXMLwf_btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void CreateNode(string pValue, XmlTextWriter writer)
        {
            writer.WriteComment(pValue);
            writer.WriteEndElement();
        }
        ///
        /// <summary>
        /// Adding items to the WorkFlow project XML file and reloading values
        /// </summary>
        /// <param name="AddDataWorkflow"></param>
        /// <param value="y">Adding attributes when a new value is added</param>
        /// <param value="n">No adding attributes when a new Item is added</param>
        /// <param name="LoadItemKeyword_Thr"></param>
        /// <param value="y">Reload "Workflow_Cbx" (<= checks if an item does not already exist) "Workflow_Lst" "AddItemswf_Txt" with the new Item created</param>
        /// <param value="n">Creation of a List of recorded items and display</param>
        /// 
        void AddItemwf_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (AddSingleItemswf_Txt.Text == "")
                    return;

                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(Workflow + NameProjectwf_Txt.Text + ".xml");
                doc.Load(xmlReader);

                string Markup = "<!--" + AddSingleItemswf_Txt.Text + "-->";

                if (doc.SelectSingleNode("/Table") is XmlElement nod)
                {
                    int x;
                    x = Itemwf_Cbx.FindStringExact(AddSingleItemswf_Txt.Text);
                    if (x == -1)
                    {
                        XmlElement elem = doc.CreateElement(AddSingleItemswf_Txt.Text);
                        elem.InnerXml = Markup; // Markup
                        nod.AppendChild(elem);
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

        void Reset()
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
            ModelEdit_Btn.Enabled = true;
        }
        ///
        /// <summary>
        /// Loading the Timeline and Items
        /// </summary>
        /// <param name="LoadItemKeyword_Thr"></param>
        /// <param value="y">Reload "Workflow_Cbx" (<= checks if an item does not already exist) "Workflow_Lst" "AddItemswf_Txt"</param>
        /// <param value="n">Creation of a List of recorded items and display</param>
        /// 
        void ProjectOpn_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ProjectOpn_Lst.SelectedIndex != -1)
                {
                    Timeline_Lst.Items.Clear();
                    string tml = ProjectOpn_Lst.SelectedItem.ToString().Replace(".xml", ".ost");
                    
                    if (File.Exists(Workflow + tml))
                        Timeline_Lst.Items.AddRange(File.ReadAllLines(Workflow + tml));

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
                        ModelEdit_Btn.Enabled = false;
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
        ///
        /// <summary>
        /// Adding a record to the WorkFlow project XML file from the text box or clipboard
        /// </summary>
        /// <param name="FormatValue()">Remove spaces and line breaks and send to => "AddDataWorkflow"</param>
        ///
        void WorkflowItem_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WorkflowItem_Lst.SelectedIndex != -1)
            {
                if (AddTextWorkflow_Txt.Text != "" || AddTNoteWorkflow_Txt.Text != "" || AddUrlWorkflow_Txt.Text != "")
                {
                    if (AddTextWorkflow_Txt.Text == "")
                        AddTextWorkflow_Txt.Text = "None";

                    if (AddTNoteWorkflow_Txt.Text == "")
                        AddTNoteWorkflow_Txt.Text = "None";

                    if (AddUrlWorkflow_Txt.Text == "")
                        AddUrlWorkflow_Txt.Text = "None";

                    FormatValue();
                }
                else
                {
                    WorkflowItem_Lst.ClearSelected();
                }
                //else
                //{
                //    string verifClip = Clipboard.GetText(TextDataFormat.Text);
                //    if (verifClip != "")
                //    {
                //        AddTextWorkflow_Txt.Text = verifClip;
                //        FormatValue();
                //    }
                //    else
                //    {
                //        WorkflowItem_Lst.ClearSelected();
                //    }
                //}
            }
        }
        ///
        /// <summary>
        /// Remove spaces and line breaks and send to => "AddDataWorkflow"
        /// </summary>
        /// Encapsulation "[[ ]]" of URLs so that they are clickable in the SVG diagram
        /// <param name="AddDataWorkflow">Adding data to the WorkFlow project XML file</param>
        /// <param value="y">Adding attributes when a new value is added</param>
        /// <param value="n">No adding attributes when a new Item is added</param>
        ///
        void FormatValue()
        {
            try
            {
                string element = WorkflowItem_Lst.SelectedItem.ToString();
                element += "_" + element;
                ///
                /// Formatting => removing spaces and line breaks
                /// 
                string[] str = AddTextWorkflow_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                AddTextWorkflow_Txt.Text = "";
                foreach (string s in str)
                {
                    if (s.Trim().Length > 0)
                    {
                        AddTextWorkflow_Txt.Text += s + " ";
                    }
                }

                string header = AddTextWorkflow_Txt.Text;
                AddTextWorkflow_Txt.Text = header.Trim(new char[] { ' ' });

                string value = AddTextWorkflow_Txt.Text;
                string valMessage = AddTextWorkflow_Txt.Text + "\r\n" + AddTNoteWorkflow_Txt.Text + "\r\n" + AddUrlWorkflow_Txt.Text + "\r\n";
                string valTimeline = AddTextWorkflow_Txt.Text + " -- " + AddTNoteWorkflow_Txt.Text + " -- " + AddUrlWorkflow_Txt.Text + " -- ";

                string message = "Add => \r\n" + valMessage + " => in " + WorkflowItem_Lst.SelectedItem.ToString() + " ?";
                string caption = "";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Timeline_Lst.Items.Add(WorkflowItem_Lst.SelectedItem.ToString() + ": " + valTimeline);
                    using (StreamWriter SW = new StreamWriter(Workflow + NameProjectwf_Txt.Text + ".ost", false))
                    {
                        foreach (string itm in Timeline_Lst.Items)
                            SW.WriteLine(itm);
                    }

                    var rawItem = value;
                    if (rawItem.Contains("https://") || rawItem.Contains("http://"))
                        value = "[[" + value + "]]";

                    AddDataWorkflow(WorkflowItem_Lst.SelectedItem.ToString(), element, value, "yes");
                }
                else
                {
                    WorkflowItem_Lst.ClearSelected();
                    AddTextWorkflow_Txt.Text = "";
                    AddTNoteWorkflow_Txt.Text = "";
                    AddUrlWorkflow_Txt.Text = "";
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! FormatValue: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Addition of data in Item of the WorkFlow project
        /// </summary>
        /// <param name="nodeselect">XML => Table/KeywordItemCollect/Item</param>
        /// <param name="elementselect">Random Item</param>
        /// <param name="value"></param>
        /// <param value="y">Adding attributes when a new value is added</param>
        /// <param value="n">No adding attributes when a new Item is added</param>
        /// <param name="LoadStatWorkflow()">Displaying recording statistics</param>
        ///
        void AddDataWorkflow(string nodeselect, string elementselect, string value, string attrib)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(Workflow + NameProjectwf_Txt.Text + ".xml");
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Table/" + nodeselect) is XmlElement nod)
                {
                    XmlElement elem = doc.CreateElement(elementselect);

                    if (attrib != "no")
                    {
                        if (AddTNoteWorkflow_Txt.Text != "")
                        {
                            ///
                            /// Formatting => removing spaces and line breaks
                            ///
                            string[] str = AddTNoteWorkflow_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            AddTNoteWorkflow_Txt.Text = "";
                            foreach (string s in str)
                            {
                                if (s.Trim().Length > 0)
                                {
                                    AddTNoteWorkflow_Txt.Text += s + " ";
                                }
                            }

                            string header = AddTNoteWorkflow_Txt.Text;
                            AddTNoteWorkflow_Txt.Text = header.Trim(new char[] { ' ' });
                        }

                        if (AddUrlWorkflow_Txt.Text != "")
                        {
                            ///
                            /// Formatting => removing spaces and line breaks
                            ///
                            string[] str = AddUrlWorkflow_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            AddUrlWorkflow_Txt.Text = "";
                            foreach (string s in str)
                            {
                                if (s.Trim().Length > 0)
                                {
                                    AddUrlWorkflow_Txt.Text += s + " ";
                                }
                            }

                            string header = AddUrlWorkflow_Txt.Text;
                            AddUrlWorkflow_Txt.Text = header.Trim(new char[] { ' ' });
                        }

                        var rawItemTxt = AddTNoteWorkflow_Txt.Text;
                        if (rawItemTxt.Contains("https://") || rawItemTxt.Contains("http://"))
                            AddTNoteWorkflow_Txt.Text = "[[" + AddTNoteWorkflow_Txt.Text + "]]";

                        var rawItemUrl = AddUrlWorkflow_Txt.Text;
                        if (rawItemUrl.Contains("https://") || rawItemUrl.Contains("http://"))
                            AddUrlWorkflow_Txt.Text = "[[" + AddUrlWorkflow_Txt.Text + "]]";

                        elem.SetAttribute("author", Author_Txt.Text);
                        elem.SetAttribute("date", DateTime.Now.ToString("d"));
                        elem.SetAttribute("time", DateTime.Now.ToString("HH:mm:ss"));
                        elem.SetAttribute("note", AddTNoteWorkflow_Txt.Text);
                        elem.SetAttribute("url", AddUrlWorkflow_Txt.Text);
                    }

                    elem.InnerText = value;
                    nod.AppendChild(elem);
                }

                xmlReader.Close();
                doc.Save(Workflow + NameProjectwf_Txt.Text + ".xml");

                AddTextWorkflow_Txt.Text = "";
                AddTNoteWorkflow_Txt.Text = "";
                AddUrlWorkflow_Txt.Text = "";
                WorkflowItem_Lst.ClearSelected();

                LoadStatWorkflow();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddDataWorkflow: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Displaying the data addition section of WorkFlow projects in "TAB BROWSx"
        /// </summary>
        ///
        void OpnWorkflowTools_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text != "")
            {
                Panel_Workflow.Visible = !Panel_Workflow.Visible;
            }
            else
            {
                if (Panel_Workflow.Visible)
                    Panel_Workflow.Visible = false;
                else
                    MessageBox.Show("Open Workflow project first!");
            }
        }
        ///
        /// <summary>
        /// Displaying WorkFlow project item data
        /// </summary>
        /// <param name="LoadItemKeyword_Thr"></param>
        /// <param value="y">Reload Workflow_Cbx (<= checks if an item does not already exist) Workflow_Lst AddItemswf_Txt</param>
        /// <param value="n">Creation of a List of recorded items and display</param>
        ///
        void StatWorkflow_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            StatWorkflow_Lst.ClearSelected();
            //try
            //{
            //    if (StatWorkflow_Lst.SelectedIndex != -1)
            //    {
            //        string KeywordItem = StatWorkflow_Lst.SelectedItem.ToString();
            //        KeywordItem = Regex.Replace(KeywordItem, "[^a-zA-Z]", "");

            //        string element = KeywordItem;
            //        element += "_" + element;

            //        Thread LoadItemKeyword = new Thread(() => LoadItemKeyword_Thr(KeywordItem, element, "n"));
            //        LoadItemKeyword.Start();

            //        StatWorkflow_Lst.ClearSelected();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    senderror.ErrorLog("Error! StatWorkflow_Lst_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            //}
        }
        ///
        /// <summary>
        /// Loading Items
        /// </summary>
        /// <param name="nodeselect">XML => Table/KeywordItemCollect/Item</param>
        /// <param name="elementselect">Random Item</param>
        /// <param name="LoadStat"></param>
        /// <param value="y">Reload Workflow_Cbx (<= checks if an item does not already exist) Workflow_Lst AddItemswf_Txt</param>
        /// <param value="n">Creation of a List of recorded items and display</param>
        ///
        void LoadItemKeyword_Thr(string nodeselect, string elementselect, string LoadStat)
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
        ///
        /// <summary>
        /// Display of recovered data statistics
        /// </summary>
        ///
        void LoadStatWorkflow()
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
        ///
        /// <summary>
        /// Creation of Model file for adding WorkFlow project Items
        /// </summary>
        /// 
        void ModelCreate_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ModelName_Txt.Text != "" && ModelItem_Txt.Text != "")
                {
                    ///
                    /// Formatting => removing spaces and line breaks
                    ///
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
                            return;
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

        void ModelList_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ModelList_Lst.SelectedIndex != -1)
                {
                    if (File.Exists(WorkflowModel + ModelList_Lst.SelectedItem.ToString()))
                    {
                        AddItemswf_Txt.Text = "";

                        using (StreamReader sr = new StreamReader(WorkflowModel + ModelList_Lst.SelectedItem.ToString()))
                        {
                            AddItemswf_Txt.Text = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ModelList_Lst_SelectedIndexChanged: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void ModelDelete_Btn_Click(object sender, EventArgs e)
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

        void ModelEdit_Btn_Click(object sender, EventArgs e)
        {
            if (ModelList_Lst.SelectedIndex != -1)
            {
                string filePath = WorkflowModel + ModelList_Lst.SelectedItem.ToString();

                if (File.Exists(filePath))
                    OpenFile_Editor(filePath);
            }
        }

        void Timeline_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Timeline_Lst.SelectedIndex != -1)
            {
                Class_Var.Text_Load = Timeline_Lst.SelectedItem.ToString();
                Open_Doc_Frm("textload");
            }
        }

        void ResetTimeline_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Saving the "config.xml" configuration and accessing the various configuration files
        /// </summary>
        /// <param name="CreateConfigFile"></param>
        /// <param value="0">Loading default URLs from the "url_dflt_cnf.ost" file</param>
        /// <param value="1">Save with the chosen parameters</param>
        /// 
        void SaveConfig_Opt_Btn_Click(object sender, EventArgs e)
        {
            CreateConfigFile(1);
            MessageBox.Show("Config save.");
        }

        void URLdirect_Opt_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(FileDir + "url.txt");
        }

        void URLconstruct_Opt_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(FileDir + @"url-constructor\construct_url.txt");
        }

        void URLconstructDir_Opt_Btn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(FileDir + "url-constructor"))
                Process.Start(FileDir + "url-constructor");
        }

        void AddOntools_Opt_Btn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Plugins))
                Process.Start(Plugins);
        }

        void MultipleWin_Opt_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(FileDir + @"grp-frm\grp_frm_url_opn.txt");
        }

        void MultipleWinDir_Opt_Btn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(FileDir + "grp-frm"))
                Process.Start(FileDir + "grp-frm");
        }

        void GoogleDork_Opt_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(FileDir + "gdork.txt"); 
        }

        void OstiumDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(AppStart))
                Process.Start(AppStart);
        }

        void AddOnDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Plugins))
                Process.Start(Plugins);
        }

        void DatabaseDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(DBdirectory))
                Process.Start(DBdirectory);
        }

        void FeedDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(FeedDir))
                Process.Start(FeedDir);
        }

        void ScriptDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Scripts))
                Process.Start(Scripts);
        }

        void WorkFlowDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Workflow))
                Process.Start(Workflow);
        }

        void WorkFlowModelDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(WorkflowModel))
                Process.Start(WorkflowModel);
        }

        void PictureDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Pictures))
                Process.Start(Pictures);
        }

        void WebView2Dir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(WebView2Dir))
                Process.Start(WebView2Dir);
        }

        void DiagramDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(DiagramDir))
                Process.Start(DiagramDir);
        }

        void SpritesDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Setirps))
                Process.Start(Setirps);
        }

        void BkmkltDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(BkmkltDir))
                Process.Start(BkmkltDir);
        }

        void MapDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(MapDir))
                Process.Start(MapDir);
        }

        #endregion

        #region Process_

        void VerifProcessRun_Btn_Click(object sender, EventArgs e)
        {
            VerifyProcess("javaw");
        }

        void KillProcess_Btn_Click(object sender, EventArgs e)
        {
            Process[] localByName = Process.GetProcessesByName("javaw");
            if (localByName.Length > 0)
            {
                string message = "Are you sure to Kill Process?";
                string caption = "Process Kill";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    KillProcessJAVAW("javaw");
                    MessageBox.Show("The Process: javaw was successfully stopped!", "Kill process");
                }
            }
            else
            {
                MessageBox.Show("Process not running.", "Process");
            }

        }
        ///
        /// <summary>
        /// Checks if process True
        /// </summary>
        /// <param name="ProcessVerif">Name of process to check</param>
        /// 
        void VerifyProcess(string ProcessVerif)
        {
            try
            {
                Process[] localByName = Process.GetProcessesByName(ProcessVerif);
                if (localByName.Length > 0)
                    MessageBox.Show("Process is True.", "Process");
                else
                    MessageBox.Show("Process is False.", "Process");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! VerifyProcess: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Checks if process True and Kill process
        /// </summary>
        /// <param name="ProcessKill">Process Name to Kill</param>
        /// 
        void KillProcessJAVAW(string ProcessKill)
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
        ///
        /// <summary>
        /// Checks if the plantUML javaw process is TRUE if False diagram display
        /// </summary>
        /// <param name="Timo_Tick">Timo start when creating a Diagram</param>
        /// <param name="CreateDiagram_Invk">Display of the Diagram when the process is False (terminated)</param>
        ///
        void Timo_Tick(object sender, EventArgs e)
        {
            Process[] localByName = Process.GetProcessesByName("javaw");
            if (localByName.Length > 0)
            { }  // Process True
            else
            {
                Timo.Enabled = false;
                Thread.Sleep(1000);
                CreateDiagram_Invk(FileDiag); // Process False
            }
        }

        #region Invoke_
        ///
        /// <summary>
        /// Unshort URL display real URL
        /// </summary>
        /// <param name="value">Real URL</param>
        ///
        void URLbrowseCbxText(string value)
        {
            URLbrowse_Cbx.Text = value;
            URLtxt_txt.Text = string.Format("Unshorten URL => {0}", value);
        }
        ///
        /// <summary>
        /// Dump creation and filling of List
        /// </summary>
        /// <param name="value">Action to perform</param>
        ///
        void SRCpageAdd(string value)
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
        ///
        /// <summary>
        /// Loading Items for WorkFlow projects
        /// </summary>
        /// <param name="value">Item Add</param>
        ///
        void LoadItemKeyword_Invk(string value)
        {
            Workflow_Cbx.Items.Add(value);
            Workflow_Lst.Items.Add(value);
            AddItemswf_Txt.AppendText(value + "\r\n");            
        }
        ///
        /// <summary>
        /// Displaying WorkFlow open project statistics
        /// </summary>
        ///
        void LoadStatWorkflow_Invk()
        {
            LoadStatWorkflow();
        }

        #endregion

        #region Invoke_Feed

        void CountBlockSite_Invk(string xswitch)
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
        ///
        /// <summary>
        /// Opening the diagram in a new wBrowser nail
        /// </summary>
        /// <param name="value"></param>
        /// <param value="0">The diagram creation file is in the diagram directory</param>
        /// <param value="1">The diagram creation file is in a random directory</param>
        ///
        void CreateDiagram_Invk(string value)
        {
            if (Commut == 0)
            {
                GoBrowser("file:///" + DiagramDir + value, 1);
            }
            else if(Commut == 1)
            {
                GoBrowser("file:///" + value, 1);
            }
        }

        #endregion

        #region Bkmklt

        void OpnBokmark_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!PanelBkmklt_Pnl.Visible)
                    loadfiledir.LoadFileDirectory(BkmkltDir, "xml", "lst", Bookmarklet_Lst);

                PanelBkmklt_Pnl.Visible = !PanelBkmklt_Pnl.Visible;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnBokmark_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void Bookmarklet_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            string BkmScr = Regex.Replace(Bookmarklet_Lst.Text, @".xml", "");
            OpnBookmark(BkmScr);
        }

        void OpnBookmark(string strAttrib)
        {
            try
            {
                string strFile = BkmkltDir + Bookmarklet_Lst.Text;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strFile);
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Table/Bkmklt/" + strAttrib);

                string strDesc = string.Format("{0}", nodeList[0].Attributes.Item(1).InnerText);
                MinifyScr = string.Format("{0}", nodeList[0].Attributes.Item(2).InnerText);
                Desc_Lbl.Text = strDesc;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnBookmark: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void InjectBkmklt_Btn_Click(object sender, EventArgs e)
        {
            if (Bookmarklet_Lst.SelectedIndex != -1)
                InjectBkmklt(MinifyScr);
        }

        async void InjectBkmklt(string Bkmklt)
        {
            try
            {
                await WBrowse.ExecuteScriptAsync(Bkmklt);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this, ex.Message, "Execute Script Fails!");
            }
        }

        void ClosePnlBkmklt_Btn_Click(object sender, EventArgs e)
        {
            PanelBkmklt_Pnl.Visible = false;
        }

        #endregion

        #region Maps_

        void OpenMaps(string adress, int provid)
        {
            try
            {
                if (KeywordMap_Txt.Text == "")
                    KeywordMap_Txt.Text = "Here";

                VerifySizeZoom();
                VerifMapOpn = "on";
                GMap_Ctrl.SetPositionByKeywords(adress);
                GmapProviderSelect(provid);
                GMap_Ctrl.Zoom = MapZoom;
                GMap_Ctrl.IgnoreMarkerOnMouseWheel = true;
                GMap_Ctrl.Manager.Mode = AccessMode.ServerOnly;
                GMap_Ctrl.Overlays.Add(overlayOne);
                GMap_Ctrl.ShowCenter = true;

                LatT = double.Parse(LatTCurrent_Lbl.Text, CultureInfo.InvariantCulture);
                LonGt = double.Parse(LonGtCurrent_Lbl.Text, CultureInfo.InvariantCulture);
                GMap_Ctrl.Position = new PointLatLng(LatT, LonGt);
                GMapOverlay markers = new GMapOverlay("markers");
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(LatT, LonGt), Mkmarker)
                {
                    ToolTipMode = MarkerTooltipMode.Always,
                    ToolTipText = KeywordMap_Txt.Text
                };

                marker.ToolTip.Fill = Brushes.Black;
                marker.ToolTip.Foreground = Brushes.White;
                marker.ToolTip.Stroke = Pens.Black;
                marker.ToolTip.TextPadding = new Size(10, 10);
                if (TxtMarker_Chk.Checked)
                    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                markers.Markers.Add(marker);
                GMap_Ctrl.Overlays.Add(markers);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenMaps: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void NewProjectMap_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                CreateProjectMap(0);                
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! NewProject_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void NewProjectMapList_Tls_Click(object sender, EventArgs e)
        {
            CreateProjectMap(1);

            string fileopen = openfile.Fileselect(AppStart, "txt files (*.txt)|*.txt|All files (*.*)|*.*", 2);

            if (fileopen == "")
                return;

            MessageBox.Show(MessageStartGeoloc);

            Thread AutoCreatePoints = new Thread(() => AutoCreatePoints_Thrd(fileopen));
            AutoCreatePoints.Start();
        }
        ///
        /// <summary>Create XML location from a list</summary>
        /// <param name="fileopn">Project create</param>
        /// <param name="values[0]">Point Name</param>
        /// <param name="values[1]">Latitude</param>
        /// <param name="values[2]">Longitude</param>
        /// <param name="values[3]">Description/Infos</param>
        /// 
        void AutoCreatePoints_Thrd(string fileopn)
        {
            using (var reader = new StreamReader(fileopn))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    AddNewLocPoints(values[0], values[1], values[2], values[3]);
                }
            }

            MessageBox.Show("Ok");
        }

        void CreateProjectMap(int val)
        {
            string message, title;
            object NameInsert;

            message = "Select Name Project.";
            title = "Project Name";

            NameInsert = Interaction.InputBox(message, title);
            string ValName = Convert.ToString(NameInsert);

            if (ValName != "")
            {
                if (File.Exists(MapDir + ValName + ".xml"))
                {
                    string avert = "The file already exists, delete?";
                    string caption = "Ostium";
                    var result = MessageBox.Show(avert, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                        File.Delete(MapDir + ValName + ".xml");
                    else
                        return;
                }

                XmlTextWriter writer = new XmlTextWriter(MapDir + ValName + ".xml", Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.Indentation = 2;

                writer.WriteStartElement("Table");

                writer.WriteStartElement("Location");
                writer.WriteEndElement();

                writer.WriteEndDocument();
                writer.Close();

                if (val == 0)
                    MessageBox.Show("XML File created.");

                PointLoc_Lst.Items.Clear();
                GMap_Ctrl.Overlays.Clear();
                overlayOne.Markers.Clear();
                loadfiledir.LoadFileDirectory(MapDir, "xml", "lst", PointLoc_Lst);

                ProjectMapOpn_Lbl.Text = "Project open: " + ValName + ".xml";

                MapXmlOpn = MapDir + ValName + ".xml";
            }
        }

        void OpnDirMap_Tls_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(MapDir))
                Process.Start(MapDir);
        }

        void EditXMLMap_Tls_Click(object sender, EventArgs e)
        {
            if (MapXmlOpn == "")
            {
                MessageBox.Show("No project selected! Select one.");
                return;
            }
            OpnFileOpt(MapXmlOpn);
        }

        void ShowXMLMap_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapXmlOpn == "")
                {
                    MessageBox.Show("No project selected! Select one.");
                    return;
                }

                if (File.Exists(MapXmlOpn))
                {
                    WBrowse.Source = new Uri(MapXmlOpn);

                    CtrlTabBrowsx();
                    Control_Tab.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ShowXMLMap_Tls_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void DelProjectMap_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapXmlOpn == "")
                {
                    MessageBox.Show("No project selected! Select one.");
                    return;
                }

                if (PointLoc_Lst.SelectedIndex != -1)
                {
                    string message = "Do you want to delete the project? " + PointLoc_Lst.SelectedItem.ToString();
                    string caption = "Delete Projet";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (File.Exists(MapDir + PointLoc_Lst.SelectedItem.ToString()))
                            File.Delete(MapDir + PointLoc_Lst.SelectedItem.ToString());

                        loadfiledir.LoadFileDirectory(MapDir, "xml", "lst", PointLoc_Lst);

                        ProjectMapOpn_Lbl.Text = "";

                        GMap_Ctrl.Overlays.Clear();
                        overlayOne.Markers.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("No project selected! Select one.");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DeleteProjectMap_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void OpnListLocation_Tls_Click(object sender, EventArgs e)
        {
            Map_Cmd_Pnl.Visible = !Map_Cmd_Pnl.Visible;
        }

        void CrossCenter_Tls_Click(object sender, EventArgs e)
        {
            if (CrossCenter == "on")
            {
                CrossCenter = "off";
                GMap_Ctrl.ShowCenter = false;
            }
            else
            {
                CrossCenter = "on";
                GMap_Ctrl.ShowCenter = true;
            }
        }

        void ScreenShotGmap_Tls_Click(object sender, EventArgs e)
        {
            CreateNameAleat();

            try
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "PNG (*.png)|*.png";
                    dialog.FileName = Una + "_Ostium_image";
                    Image image = GMap_Ctrl.ToImage();

                    if (image != null)
                    {
                        using (image)
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                string fileName = dialog.FileName;

                                if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                                    fileName += ".png";

                                image.Save(fileName);
                                MessageBox.Show("The picture has been saved:" + dialog.FileName, "Ostium Map", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ScreenShotGmap_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void OpnBingMap_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                string VerifyStyle = "a"; // a Aerien - r Road
                string ZommValueMaps = "19";
                string TiltMapsValue = "100";
                string Direction = "North";

                Process.Start("bingmaps:?cp=" + LatTCurrent_Lbl.Text + "~" + LonGtCurrent_Lbl.Text + "&lvl=" + ZommValueMaps + "&sty=" + VerifyStyle + "&pit=" + TiltMapsValue + "&hdg=" + Direction + "");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnBingMap_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void OpnGoogleMaps_Tls_Click(object sender, EventArgs e)
        {           
            GoBrowser(lstUrlDfltCnf[7].ToString() + LatTCurrent_Lbl.Text  + "%2C" + LonGtCurrent_Lbl.Text, 0);
            CtrlTabBrowsx();
            Control_Tab.SelectedIndex = 0;
        }

        void OpnGoogleStreet_Tls_Click(object sender, EventArgs e)
        {
            GoBrowser(lstUrlDfltCnf[8].ToString() + LatTCurrent_Lbl.Text + "%2C" + LonGtCurrent_Lbl.Text, 0);
            CtrlTabBrowsx();
            Control_Tab.SelectedIndex = 0;
        }

        void OpenGoogleEarth_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                CreateNameAleat();

                using (StreamWriter addtxt = new StreamWriter(MapDir + "temp.kml"))
                {
                    addtxt.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    addtxt.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\" xmlns:kml=\"http://www.opengis.net/kml/2.2\" xmlns:atom=\"http://www.w3.org/2005/Atom\">");
                    addtxt.WriteLine("<Document>");
                    addtxt.WriteLine("	<Placemark>");
                    addtxt.WriteLine("		<name>Ostium location " + Una + "</name>");
                    addtxt.WriteLine("		<LookAt>");
                    addtxt.WriteLine("			<longitude>" + LonGtCurrent_Lbl.Text + "</longitude>");
                    addtxt.WriteLine("			<latitude>" + LatTCurrent_Lbl.Text + "</latitude>");
                    addtxt.WriteLine("			<altitude>0</altitude>");
                    addtxt.WriteLine("			<heading>-1.99957389508707</heading>");
                    addtxt.WriteLine("			<range>8314.559936586644</range>");
                    addtxt.WriteLine("			<gx:altitudeMode>relativeToSeaFloor</gx:altitudeMode>");
                    addtxt.WriteLine("		</LookAt>");
                    addtxt.WriteLine("	</Placemark>");
                    addtxt.WriteLine("</Document>");
                    addtxt.WriteLine("</kml>");
                }
                Process.Start(MapDir + "temp.kml");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenGoogleEarth_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void CopyGeoMap_Tls_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, LatTCurrent_Lbl.Text + " " + LonGtCurrent_Lbl.Text);
        }

        void GoLatLong_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                if (LatT_Txt.Text == "" || LonGt_txt.Text == "")
                {
                    LatT_Txt.BackColor = Color.Red;
                    LonGt_txt.BackColor = Color.Red;
                    MessageBox.Show("False coordinates!");
                    LatT_Txt.BackColor = Color.FromArgb(28, 28, 28);
                    LonGt_txt.BackColor = Color.FromArgb(28, 28, 28);
                    return;
                }

                GMap_Ctrl.Overlays.Clear();
                overlayOne.Markers.Clear();
                GoLatLong(LatT_Txt.Text, LonGt_txt.Text, LatT_Txt.Text + " " + LonGt_txt.Text);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! GoLatLong_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void GoWord_Tls_Click(object sender, EventArgs e)
        {
            if (KeywordMap_Txt.Text == "")
            {
                KeywordMap_Txt.BackColor = Color.Red;
                MessageBox.Show("Insert keyword!");
                KeywordMap_Txt.BackColor = Color.FromArgb(28, 28, 28);
                return;
            }

            GMap_Ctrl.Overlays.Clear();
            overlayOne.Markers.Clear();
            OpenMaps(KeywordMap_Txt.Text, 12); // Adresse, Provider
        }

        void AddNewLoc_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapXmlOpn == "")
                {
                    MessageBox.Show("No project selected! Select one or create one.");
                    return;
                }

                if (LocationName_Txt.Text == "")
                {
                    LocationName_Txt.BackColor = Color.Red;
                    MessageBox.Show("Insert Name!");
                    LocationName_Txt.BackColor = Color.FromArgb(28, 28, 28);
                    return;
                }

                AddNewLocPoints(LocationName_Txt.Text, LatTCurrent_Lbl.Text, LonGtCurrent_Lbl.Text, TextMarker_Txt.Text);
                OpnLocationPoints();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddNewLoc_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void AddNewLocPoints(string locationname, string lat, string lon, string txtmarker)
        {
            XmlDocument doc = new XmlDocument();
            XmlTextReader xmlReader = new XmlTextReader(MapXmlOpn);
            doc.Load(xmlReader);

            if (doc.SelectSingleNode("/Table/Location") is XmlElement nod)
            {
                XmlElement elem = doc.CreateElement("Point_Point");
                elem.SetAttribute("latitude", lat);
                elem.SetAttribute("longitude", lon);
                elem.SetAttribute("textmarker", txtmarker);
                elem.InnerText = locationname;
                nod.AppendChild(elem);
            }

            xmlReader.Close();
            doc.Save(MapXmlOpn);
        }

        void GmapProvider_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            GmapProviderSelect(GmapProvider_Cbx.SelectedIndex);
            GMap_Ctrl.Focus();
        }

        void GmapProviderSelect(int val)
        {
            switch (val)
            {
                case 0:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.BingHybridMap;
                        break;
                    }
                case 1:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.BingMap;
                        break;
                    }
                case 2:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.BingOSMap;
                        break;
                    }
                case 3:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.BingSatelliteMap;
                        break;
                    }
                case 4:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.GoogleMap;
                        break;
                    }
                case 5:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.GoogleSatelliteMap;
                        break;
                    }
                case 6:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.GoogleTerrainMap;
                        break;
                    }
                case 7:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.OpenCycleLandscapeMap;
                        break;
                    }
                case 8:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.OpenCycleMap;
                        break;
                    }
                case 9:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.OpenCycleTransportMap;
                        break;
                    }
                case 10:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.OpenSeaMapHybrid;
                        break;
                    }
                case 11:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.OpenStreet4UMap;
                        break;
                    }
                case 12:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.OpenStreetMap;
                        break;
                    }
                case 13:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.WikiMapiaMap;
                        break;
                    }
                case 14:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.CzechGeographicMap;
                        break;
                    }
                case 15:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_Imagery_World_2D_Map;
                        break;
                    }
                case 16:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_StreetMap_World_2D_Map;
                        break;
                    }
                case 17:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_World_Physical_Map;
                        break;
                    }
                case 18:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_World_Shaded_Relief_Map;
                        break;
                    }
                case 19:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_World_Street_Map;
                        break;
                    }
                case 20:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_World_Terrain_Base_Map;
                        break;
                    }
                default:
                    {
                        GMap_Ctrl.MapProvider = GMapProviders.BingMap;
                        break;
                    }
            }
        }

        void GoLatLong(string lat, string lon, string tooltxt)
        {
            try
            {
                if (tooltxt == "")
                    tooltxt = "Here";

                VerifySizeZoom();

                LatT = double.Parse(lat, CultureInfo.InvariantCulture);
                LonGt = double.Parse(lon, CultureInfo.InvariantCulture);

                GMap_Ctrl.Position = new PointLatLng(LatT, LonGt);
                GMapOverlay markers = new GMapOverlay("markers");
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(LatT, LonGt), Mkmarker)
                {
                    ToolTipMode = MarkerTooltipMode.Always,
                    ToolTipText = tooltxt
                };

                marker.ToolTip.Fill = Brushes.Yellow;
                marker.ToolTip.Foreground = Brushes.Black;
                marker.ToolTip.Stroke = Pens.Black;
                marker.ToolTip.TextPadding = new Size(10, 10);
                if (TxtMarker_Chk.Checked)
                    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                markers.Markers.Add(marker);
                GMap_Ctrl.Overlays.Add(markers);
                GMap_Ctrl.Zoom = MapZoom;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! GoLatLong: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void GMap_Ctrl_OnPositionChanged(PointLatLng point)
        {            
            LatTCurrent_Lbl.Text = point.Lat.ToString(CultureInfo.InvariantCulture);
            LonGtCurrent_Lbl.Text = point.Lng.ToString(CultureInfo.InvariantCulture);
        }

        void GMap_Ctrl_OnMapZoomChanged()
        {
            ZoomValMap_Lbl.Text = GMap_Ctrl.Zoom.ToString();
        }

        void PointLoc_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PointLoc_Lst.SelectedIndex != -1)
            {
                GMap_Ctrl.Overlays.Clear();
                overlayOne.Markers.Clear();
                Mkmarker = GMarkerGoogleType.blue;
                MapXmlOpn = MapDir + PointLoc_Lst.SelectedItem.ToString();
                ProjectMapOpn_Lbl.Text = "Project open: " + PointLoc_Lst.SelectedItem.ToString();
                OpnLocationPoints();
            }
        }

        void OpnLocationPoints()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MapXmlOpn);
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/Table/Location/Point_Point");

                for (int i = 0; i < nodeList.Count; i++)
                {
                    //string name = string.Format("{0}", nodeList[i].ChildNodes.Item(0).InnerText);
                    string lat = string.Format("{0}", nodeList[i].Attributes.Item(0).InnerText);
                    string lon = string.Format("{0}", nodeList[i].Attributes.Item(1).InnerText);
                    string txtmarker = string.Format("{0}", nodeList[i].Attributes.Item(2).InnerText);

                    GoLatLong(lat, lon, txtmarker);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnLocationPoints: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        void VerifySizeZoom()
        {
            MapZoom = Convert.ToInt32(ZoomValMap_Lbl.Text);

            if (MapZoom < 1 || MapZoom > 20)
            {
                if (MapZoom < 1)
                {
                    ZoomValMap_Lbl.Text = "1";
                    MapZoom = 1;
                }
                else if (MapZoom > 20)
                {
                    ZoomValMap_Lbl.Text = "20";
                    MapZoom = 20;
                }
                MessageBox.Show("Zoom cannot be less than 1 and should not exceed 20! Necessary changes have been applied.");
            }
        }

        void GMap_Ctrl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            GMap_Ctrl.Position = GMap_Ctrl.FromLocalToLatLng(e.X, e.Y);
            VerifySizeZoom();
            GMap_Ctrl.Zoom = MapZoom;

            GMapOverlay markers = new GMapOverlay("markers");
            GMapMarker marker = new GMarkerGoogle(new PointLatLng(GMap_Ctrl.Position.Lat, GMap_Ctrl.Position.Lng), Mkmarker)
            {
                ToolTipMode = MarkerTooltipMode.Always,
                ToolTipText = "Here"
            };

            marker.ToolTip.Fill = Brushes.Black;
            marker.ToolTip.Foreground = Brushes.White;
            marker.ToolTip.Stroke = Pens.Black;
            marker.ToolTip.TextPadding = new Size(20, 10);
            if (TxtMarker_Chk.Checked)
                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            markers.Markers.Add(marker);
            GMap_Ctrl.Overlays.Add(markers);
            GMap_Ctrl.Zoom = MapZoom;
        }

        void Main_Frm_KeyUp(object sender, KeyEventArgs e)
        {
            int offset = -22;

            switch (e.KeyCode)
            {
                case Keys.NumPad4:
                    GMap_Ctrl.Offset(-offset, 0);
                    break;
                case Keys.NumPad6:
                    GMap_Ctrl.Offset(offset, 0);
                    break;
                case Keys.NumPad8:
                    GMap_Ctrl.Offset(0, -offset);
                    break;
                case Keys.NumPad2:
                    GMap_Ctrl.Offset(0, offset);
                    break;
                case Keys.NumPad7:
                    GMap_Ctrl.Zoom = ((int)(GMap_Ctrl.Zoom + 0.99)) - 1;
                    break;
                case Keys.NumPad9:
                    GMap_Ctrl.Zoom = ((int)GMap_Ctrl.Zoom) + 1;
                    break;
                case Keys.NumPad1:
                    GMap_Ctrl.Bearing--;
                    break;
                case Keys.NumPad3:
                    GMap_Ctrl.Bearing++;
                    break;
            }
        }

        #endregion
        /// <summary>
        /// Statusbar Button, open URL in BROWSx Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OpnURL_TlsTools_Click(object sender, EventArgs e)
        {
            GoBrowser(URLtxt_txt.Text, 1);
        }

        #region Json_

        void JsonOpnFile_Btn_Click(object sender, EventArgs e)
        {
            string fileopen = openfile.Fileselect(AppStart, "json files (*.json)|*.json|All files (*.*)|*.*", 2);

            if (fileopen != "")
            {
                OutParse.Text = "";
                FileAsync(fileopen);
            }
        }

        async void FileAsync(string val)
        {
            using (StreamReader sr = new StreamReader(val))
            {
                string line = await sr.ReadToEndAsync();
                OutParse.Text = line;
            }
        }

        void LastJson_Btn_Click(object sender, EventArgs e)
        {
            if (File.Exists(JsonDir + "test-json.json"))
            {
                using (StreamReader sr = new StreamReader(JsonDir + "test-json.json"))
                {
                    JsonOutA_txt.Text = sr.ReadToEnd();
                }
            }
        }

        void JsonSaveFile_Btn_Click(object sender, EventArgs e)
        {
            if (OutParse.Text != "")
                SavefileShowDiag(OutParse.Text, "json files (*.json)|*.json");
        }

        void JsonSaveUri_Btn_Click(object sender, EventArgs e)
        {
            if (JsonUri_Txt.Text != "")
            {
                CreateData(JsonDir + "list-url-json.txt", JsonUri_Txt.Text);
                Beep(1200, 200);
            }
        }

        void JsonOpnListUri_Btn_Click(object sender, EventArgs e)
        {
            if (File.Exists(JsonDir + "list-url-json.txt"))
                Open_Source_Frm(JsonDir + "list-url-json.txt");
        }

        void JsonSaveData_Btn_Click(object sender, EventArgs e)
        {
            if (OutJson.Text != "")
                SavefileShowDiag(OutJson.Text, "files (*.*)|*.*");
        }

        void GetJson_Btn_Click(object sender, EventArgs e)
        {
            if (Class_Var.URL_USER_AGENT_SRC_PAGE == "")
                Class_Var.URL_USER_AGENT_SRC_PAGE = lstUrlDfltCnf[5].ToString();

            OutParse.Text = "";
            GetAsync(JsonUri_Txt.Text);
        }

        void ParseJson_Btn_Click(object sender, EventArgs e)
        {
            OutJson.Text = "";

            Thread ParseVal = new Thread(() => ParseVal_Thrd(JsonVal_Txt.Text, OutParse.Text, CharSpace_Txt.Text));
            ParseVal.Start();
        }

        void ParseNodeJson_Btn_Click(object sender, EventArgs e)
        {
            OutJson.Text = "";

            Thread ParseNode = new Thread(() => ParseNode_Thrd(JsonVal_Txt.Text, JsonCnt_txt.Text, OutParse.Text, JsonNode_Txt.Text, CharSpace_Txt.Text));
            ParseNode.Start();
        }

        void TableParse_Btn_Click(object sender, EventArgs e)
        {
            Thread TableParseVal = new Thread(() => TableParseVal_Thrd());
            TableParseVal.Start();
        }

        void TableNode_Btn_Click(object sender, EventArgs e)
        {
            Thread TableParseNode = new Thread(() => TableParseNode_Thrd());
            TableParseNode.Start();
        }

        void OpnTableList_Btn_Click(object sender, EventArgs e)
        {
            loadfiledir.LoadFileDirectory(JsonDirTable, "html", "lst", TableJson_Lst);
            JsonList_Pnl.Visible = !JsonList_Pnl.Visible;
        }

        void ReplaceBrckt_btn_Click(object sender, EventArgs e)
        {
            Thread ReplaceBrckt = new Thread(() => ReplaceBrckt_Thrd());
            ReplaceBrckt.Start();
        }

        void ReplaceBrckt_Thrd()
        {
            OutParse.Text = Regex.Replace(OutParse.Text, BrcktA_Txt.Text, BrcktB_Txt.Text);
            OutParse.Text = OutParse.Text;
        }

        void OpnJsonDirTable_Btn_Click(object sender, EventArgs e)
        {
            Process.Start(JsonDirTable);
        }

        void TableJson_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TableJson_Lst.SelectedIndex != -1)
                GoBrowser("file:///" + JsonDirTable + TableJson_Lst.SelectedItem.ToString(), 1);
        }

        async void GetAsync(string Urijson)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd(JsonUsrAgt_Txt.Text);
                var response = await client.GetAsync(Urijson);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                OutParse.Text = $"{jsonResponse}";

                File_Write(JsonDir + "tmp-json.json", OutParse.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        void ParseVal_Thrd(string value, string jsonout, string charspace)
        {
            try
            {
                string xT = value;
                string xO = "";
                char[] charsToTrim = { ',' };
                string[] words = xT.Split();
                string sendval = "";
                
                JArray jsonVal = JArray.Parse(jsonout);
                dynamic valjson = jsonVal;

                foreach (dynamic val in valjson)
                {
                    foreach (string word in words)
                    {
                        xO += val[word.TrimEnd(charsToTrim)] + charspace;
                    }

                    sendval += xO + "\r\n";
                    xO = "";
                }

                Invoke(new Action<string>(ValAdd_Invk), sendval);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        void TableParseVal_Thrd()
        {
            try
            {
                CreateNameAleat();
                StreamWriter file = new StreamWriter(JsonDirTable + Una + "_table.html");

                string xT = JsonVal_Txt.Text;
                string xO = "";
                char[] charsToTrim = { ',' };
                string[] words = xT.Split();

                JArray jsonVal = JArray.Parse(OutParse.Text);
                dynamic valjson = jsonVal;

                string t = "<!DOCTYPE html><html lang=\"fr\"><head><meta charset=\"UTF-8\"><title>Ostium Home</title><link rel=\"stylesheet\" " +
                    "href=\"style.css\"></head><body><div class=\"relative overflow-hidden shadow-md rounded-lg\"><table class=\"table-fixed w-full " +
                    "text-left\"><thead class=\"uppercase bg-[#6b7280] text-[#e5e7eb]\" style=\"background-color: #6b7280; color: #e5e7eb;\"><tr>";

                file.WriteLine(t);

                foreach (string word in words)
                {
                    xO += "<td class=\"py-1 border text-center  p-4\">" + word.TrimEnd(charsToTrim) + "</td>";
                }

                file.WriteLine(xO);

                t = "</tr></thead><tbody class=\"bg-white text-gray-500 bg-[#FFFFFF] text-[#6b7280]\" style=\"background-color: #FFFFFF; " +
                    "color: #6b7280;\"><tr class=\"py-5\">";

                file.WriteLine(t);

                xO = "";

                foreach (dynamic val in valjson)
                {
                    foreach (string word in words)
                    {
                        xO += "<td class=\"py-1 border text-center  p-4\">" + val[word.TrimEnd(charsToTrim)] + "</td>";
                    }

                    file.WriteLine(xO);
                    xO = "";
                    file.WriteLine("</tr><tr class=\"py-5\">");
                }

                file.WriteLine("</tr></tbody></table></div></body></html>");
                file.Close();

                Invoke(new Action<string>(OpnTableJson_Invk), JsonDirTable + Una + "_table.html");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        void ParseNode_Thrd(string value, string count, string jsonout, string jsonnode, string charspace)
        {
            try
            {
                string xT = value;
                string xO = "";
                char[] charsToTrim = { ',' };
                string[] words = xT.Split();
                string sendval = "";

                int CntEnd = Convert.ToInt32(count);
                JsonNode CastNode = JsonNode.Parse(jsonout);
                JsonNode SelNode = CastNode[jsonnode];

                for (int i = 0; i < CntEnd; i++)
                {
                    JsonNode SrcName = SelNode[i];

                    JArray jsonVal = JArray.Parse("[" + SrcName.ToJsonString() + "]");
                    dynamic valjson = jsonVal;
                    foreach (dynamic val in valjson)
                    {
                        foreach (string word in words)
                        {
                            xO += val[word.TrimEnd(charsToTrim)] + charspace;
                        }

                        sendval += xO + "\r\n";
                        xO = "";
                    }
                }
                Invoke(new Action<string>(ValAdd_Invk), sendval);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        void TableParseNode_Thrd()
        {
            try
            {
                CreateNameAleat();
                StreamWriter file = new StreamWriter(JsonDirTable + Una + "_table.html");

                string xT = JsonVal_Txt.Text;
                string xO = "";
                char[] charsToTrim = { ',' };
                string[] words = xT.Split();

                int CntEnd = Convert.ToInt32(JsonCnt_txt.Text);
                JsonNode CastNode = JsonNode.Parse(OutParse.Text);
                JsonNode SelNode = CastNode[JsonNode_Txt.Text];

                string t = "<!DOCTYPE html><html lang=\"fr\"><head><meta charset=\"UTF-8\"><title>Ostium Home</title><link rel=\"stylesheet\" " +
    "href=\"style.css\"></head><body><div class=\"relative overflow-hidden shadow-md rounded-lg\"><table class=\"table-fixed w-full " +
    "text-left\"><thead class=\"uppercase bg-[#6b7280] text-[#e5e7eb]\" style=\"background-color: #6b7280; color: #e5e7eb;\"><tr>";

                file.WriteLine(t);

                foreach (string word in words)
                {
                    xO += "<td class=\"py-1 border text-center  p-4\">" + word.TrimEnd(charsToTrim) + "</td>";
                }

                file.WriteLine(xO);

                t = "</tr></thead><tbody class=\"bg-white text-gray-500 bg-[#FFFFFF] text-[#6b7280]\" style=\"background-color: #FFFFFF; " +
                    "color: #6b7280;\"><tr class=\"py-5\">";

                file.WriteLine(t);

                xO = "";

                for (int i = 0; i < CntEnd; i++)
                {
                    JsonNode SrcName = SelNode[i];

                    JArray jsonVal = JArray.Parse("[" + SrcName.ToJsonString() + "]");
                    dynamic valjson = jsonVal;
                    foreach (dynamic val in valjson)
                    {
                        foreach (string word in words)
                        {
                            xO += "<td class=\"py-1 border text-center  p-4\">" + val[word.TrimEnd(charsToTrim)] + "</td>";
                        }

                        file.WriteLine(xO);
                        xO = "";
                        file.WriteLine("</tr><tr class=\"py-5\">");
                    }
                }

                file.WriteLine("</tr></tbody></table></div></body></html>");
                file.Close();

                Invoke(new Action<string>(OpnTableJson_Invk), JsonDirTable + Una + "_table.html");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        void SavefileShowDiag(string Data, string Filter)
        {
            Stream isData;
            SaveFileDialog saveFD = new SaveFileDialog
            {
                InitialDirectory = AppStart,
                Filter = Filter,
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFD.ShowDialog() == DialogResult.OK)
            {
                if ((isData = saveFD.OpenFile()) != null)
                {
                    using (StreamWriter file_create = new StreamWriter(isData))
                    {
                        file_create.Write(Data);
                    }

                    isData.Close();

                    Beep(1200, 200);
                }
            }
        }

        void OutJsonA_Chk_Click(object sender, EventArgs e)
        {
            if (OutJsonA_Chk.Checked)
            {
                OutParse = JsonOutA_txt;
                OutJson = JsonOutB_txt;
                OutJsonB_Chk.Checked = false;
            }
            else
            {
                OutParse = JsonOutB_txt;
                OutJson = JsonOutA_txt;
                OutJsonB_Chk.Checked = true;
            }              
        }

        void OutJsonB_Chk_Click(object sender, EventArgs e)
        {
            if (OutJsonB_Chk.Checked)
            {
                OutParse = JsonOutB_txt;
                OutJson = JsonOutA_txt;
                OutJsonA_Chk.Checked = false;
            }
            else
            {
                OutParse = JsonOutA_txt;
                OutJson = JsonOutB_txt;
                OutJsonA_Chk.Checked = true;
            }
        }

        void BrktsL_Btn_Click(object sender, EventArgs e)
        {
            Thread Brkts = new Thread(() => Brkts_Thrd());
            Brkts.Start();
        }

        void Brkts_Thrd()
        {
            OutParse.Select(0, 0);
            OutParse.Text = OutParse.Text.Insert(OutParse.SelectionStart, "[");
            OutParse.Select(OutParse.Text.Length, 0);
            OutParse.Text = OutParse.Text.Insert(OutParse.SelectionStart, "]");
        }

        #endregion

        #region Invoke_Json

        void ValAdd_Invk(string val)
        {            
            OutJson.Text = val;
        }

        void OpnTableJson_Invk(string val)
        {
            GoBrowser("file:///" + val, 1);
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

        #region Update_
        ///
        /// <summary>
        /// Checking updates via Http request and comparison with the variable => "versionNow"
        /// </summary>
        /// <param name="softName">Application name</param>
        /// <param name="announcement"></param>
        /// <param value="0">Auto check no announcement if update False announcement if True</param>
        /// <param value="1">Manual check announces whether False or True</param>
        /// <param name="AnnonceUpdate">Update message available</param>
        ///
        async void VerifyUPDT(string softName, int annoncE)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(upftOnlineFile);
                string updtValue = await response.Content.ReadAsStringAsync();

                if (versionNow != updtValue)
                {
                    AnnonceUpdate(softName);
                }
                else
                {
                    if (annoncE == 1)
                        MessageBox.Show("No update available.", softName);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! VerifyUPDT: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Announcement if Updated True
        /// </summary>
        /// <param name="softName">Application name</param>
        ///
        void AnnonceUpdate(string softName)
        {
            var result = MessageBox.Show("An update is available for the " + softName + " software, open the update page now?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            try
            {
                if (result == DialogResult.Yes)
                {
                    GoBrowser(WebPageUpdate, 0);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AnnonceUpdate: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        #endregion
    }
}
