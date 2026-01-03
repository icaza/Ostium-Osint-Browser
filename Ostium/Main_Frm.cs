using Dirsize;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Icaza;
using LoadDirectory;
using Microsoft.VisualBasic;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ostium.Properties;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel.Syndication;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using GMapMarker = GMap.NET.WindowsForms.GMapMarker;
using GMapPolygon = GMap.NET.WindowsForms.GMapPolygon;
using GMapRoute = GMap.NET.WindowsForms.GMapRoute;

namespace Ostium
{
    public partial class Main_Frm : Form
    {
        #region Var_
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
        readonly string MapDirGpx = Application.StartupPath + @"\map\gpx\";
        readonly string JsonDir = Application.StartupPath + @"\json-files\";
        readonly string JsonDirTable = Application.StartupPath + @"\json-files\table\";
        readonly string SVGviewerdir = Application.StartupPath + @"\SVGviewer\";
        readonly string Keeptrack = Application.StartupPath + @"\KeepTrack\";

        string databasePath = "default_database_name";
        ///
        /// <summary>
        /// Objects
        /// </summary>
        /// 
        Webview_Frm webviewForm;
        HtmlText_Frm HtmlTextFrm;
        Mdi_Frm mdiFrm;
        Doc_Frm docForm;
        Keeptrack_Frm keeptrackForm;
        DeserializeJson_Frm deserializeForm;
        OpenSource_Frm openSourceForm;
        ScriptCreator scriptCreatorFrm;
        Bookmarklets_Frm bookmarkletsFrm;
        ListBox List_Object;
        ToolStripComboBox Cbx_Object;
        ListBox List_Wf;
        ComboBox Workflow_Cbx;
        ListBox Workflow_Lst;
        Label SizeAll_Lbl;

        // Json
        Microsoft.Web.WebView2.WinForms.WebView2 WbOutJson;
        Microsoft.Web.WebView2.WinForms.WebView2 WbOutParse;
        readonly string JsonA = Application.StartupPath + @"\json-files\out-a-json.json";
        readonly string JsonB = Application.StartupPath + @"\json-files\out-b-json.json";
        ///
        /// <summary>
        /// Variables
        /// </summary>
        /// 
        readonly string SoftVersion = string.Empty;
        string ClearOnOff = "on";
        string NameUriDB = string.Empty;
        string UnshortURLval = string.Empty;
        string Una = string.Empty;
        string TableOpen = string.Empty;
        string Tables_Lst_Opt = "add";
        string DataAddSelectUri = string.Empty;
        string tlsi = string.Empty; // Table List selected Item
        string DBadmin = "off";
        string ManageFeed = "off";
        string TmpTitleWBrowse = string.Empty;
        string TmpTitleWBrowsefeed = string.Empty;
        int VerifLangOpn = 0;
        string TitleFeed;
        string AddTitleItem;
        string AddLinkItem;
        string UserAgentOnOff = "off";
        string UserAgentSelect = string.Empty;
        string ThemeDiag;
        string FileDiag = string.Empty;
        string MinifyScr = string.Empty;
        string Scriptl = "off";
        ///
        /// <summary>
        /// Map variables
        /// </summary>
        /// 
        int MapZoom = 1;
        string CrossCenter = "on";
        string VerifMapOpn = "off";
        string MapXmlOpn = string.Empty;
        string MapRouteOpn = string.Empty;
        string LocatRoute = string.Empty;
        string KmlGpxOpn = "off";
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
        readonly string updtOnlineFile = "https://veydunet.com/2x24/sft/updt/updt_ostium.html"; // <= Change the URL to distribute your version
        readonly string WebPageUpdate = "https://veydunet.com/ostium/update.html"; // <= Change the URL to distribute your version
        readonly string versionNow = "31";

        readonly string HomeUrlRSS = "https://veydunet.com/ostium/rss.html";
        int Vrfy = 0;
        string FileOpnJson = string.Empty;
        readonly string HighlitFile = Application.StartupPath + @"\hwcf.txt";

        static readonly HttpClient client = new HttpClient();
        const int TimeoutInSeconds = 10;

        bool IsTimelineEnabled = false;
        bool IsParentLinkEnabled = false;
        string FileTimeLineName = "visit";
        bool ProcessLoad = false;

        int _historyIndex = -1;
        readonly List<string> _commandHistory = new List<string>();

        FloodHeader HeaderFlood;
        readonly DirectoryTreeExporter exporter = new DirectoryTreeExporter();
        readonly Stack<string> history = new Stack<string>();

        bool IsAdsTrackersBlocked = false;
        // redlist
        HashSet<string> _blockedFullUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> _blockedHosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> _blockedPatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, bool> _urlCache = new Dictionary<string, bool>();
        const int MAX_CACHE_SIZE = 10000;
        // Whitelist - priority over blocklist
        HashSet<string> _allowedFullUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> _allowedHosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> _allowedPatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        List<Regex> _compiledAllowedPatterns = new List<Regex>();
        // Pre-compiling regexes for patterns
        List<Regex> _compiledPatterns;
        string BlockedUrl = "blocked_min.txt";
        readonly string AllowUrl = "allowed.txt";
        int urlBlocked = 0;

        #endregion

        #region Frm_

        public Main_Frm()
        {
            InitializeComponent();

            _urlCache = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

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
                BeginInvoke((MethodInvoker)delegate
                {
                    CreateDirectory();
                    ///
                    /// Loading default URLs into a List
                    ///
                    if (File.Exists(Path.Combine(AppStart, "url_dflt_cnf.ost")))
                    {
                        lstUrlDfltCnf.Clear();
                        lstUrlDfltCnf.AddRange(File.ReadAllLines(Path.Combine(AppStart, "url_dflt_cnf.ost")));
                    }
                    else
                    {
                        MessageBox.Show("The url_dflt_cnf.ost file is missing! Go to Ostium GitHub page to download this " +
                            "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    ///
                    /// Loading configuration
                    /// <param name="CreateConfigFile"></param>
                    /// <param value="0">Resetting the config, loading the default URLs from the url_dflt_cnf.ost file</param>
                    /// <param value="1">Save the chosen configuration and Reload</param>
                    /// 
                    if (File.Exists(Path.Combine(AppStart, "config.xml")))
                        LoadConfiguration(Path.Combine(AppStart, "config.xml"));
                    else
                        CreateConfigFile(0);
                    ///
                    /// Web URL Home page wBrowser Tab => index and wBrowser Tab => feed
                    /// If empty loading from default URL file
                    ///
                    if (string.IsNullOrEmpty(@Class_Var.URL_HOME))
                    {
                        try
                        {
                            if (lstUrlDfltCnf.Count > 0)
                                @Class_Var.URL_HOME = lstUrlDfltCnf[1].ToString();
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                                "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        catch (ArgumentException ex)
                        {
                            senderror.ErrorLog("Error! lstUrlDfltCnf Main_Frm_Load: ", ex.ToString(), "Main_Frm", AppStart);
                        }
                    }

                    if (!string.IsNullOrEmpty(@Class_Var.URL_HOME))
                        WBrowse.Source = new Uri(@Class_Var.URL_HOME);

                    WBrowsefeed.Source = new Uri(HomeUrlRSS);

                    Tools_TAB_0.Visible = true;
                    ///
                    /// Checking auto updates
                    /// <param value="0">Message only if update available</param>
                    /// <param value="1">Message False or True update</param>
                    ///
                    VerifyUPDT("Ostium", 0);

                    WbOutParse = WbOutA;
                    WbOutJson = WbOutB;
                });
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Main_Frm_Load: ", ex.ToString(), "Main_Frm", AppStart);
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
                    var result = MessageBox.Show("Delete all history? (Run purge.bat after closing Ostium, " +
                        "for complete deletion of the WebView2 usage directory)", "Delete all history",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                        ClearData(1);
                }
            }
            catch { }
        }

        void Main_Frm_LocationChanged(object sender, EventArgs e)
        {
            Form secondForm = Application.OpenForms["Keeptrack_Frm"];

            if (secondForm != null && !secondForm.IsDisposed)
            {
                int x = Location.X + Width - secondForm.Width - 32;
                int y = Location.Y + Height - secondForm.Height - 39;
                secondForm.Location = new Point(x, y);
            }
        }

        #endregion

        void Form_EventHandler()
        {
            FormClosing += new FormClosingEventHandler(Main_Frm_FormClosed);
            LocationChanged += new EventHandler(Main_Frm_LocationChanged);
            CategorieFeed_Cbx.SelectedIndexChanged += new EventHandler(CategorieFeed_Cbx_SelectedIndexChanged);
            RSSListSite_Lbl.MouseEnter += new EventHandler(RSSListSite_Lbl_Lbl_MouseEnter);
            RSSListSite_Lbl.MouseLeave += new EventHandler(RSSListSite_Lbl_Lbl_Leave);
            ThemDiag_Cbx.SelectedIndexChanged += new EventHandler(ThemDiag_Cbx_SelectedIndexChanged);
            GMap_Ctrl.OnPositionChanged += new PositionChanged(GMap_Ctrl_OnPositionChanged);
            GMap_Ctrl.OnMapZoomChanged += new MapZoomChanged(GMap_Ctrl_OnMapZoomChanged);
            GMap_Ctrl.MouseDoubleClick += new MouseEventHandler(GMap_Ctrl_MouseDoubleClick);
            GMap_Ctrl.KeyDown += new KeyEventHandler(Main_Frm_KeyUp);
            OutJsonA_Chk.Click += new EventHandler(OutJsonA_Chk_Click);
            OutJsonB_Chk.Click += new EventHandler(OutJsonB_Chk_Click);
            TtsButton_Sts.ButtonClick += new EventHandler(TtsButton_Sts_ButtonClick);
            TtsButton_Sts.DropDownItemClicked += TtsButton_Sts_DropDownItemClicked;

            JsonUri_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            JsonVal_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            JsonNode_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            JsonCnt_txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            CharSpace_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            BrcktA_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            BrcktB_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            NameProjectwf_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            Author_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            ModelName_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            AddSingleItemswf_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            DataBaze_Opn.KeyPress += new KeyPressEventHandler(Object_Keypress);
            DataTable_Opn.KeyPress += new KeyPressEventHandler(Object_Keypress);
            DataValue_Name.KeyPress += new KeyPressEventHandler(Object_Keypress);
            DataValue_Opn.KeyPress += new KeyPressEventHandler(Object_Keypress);
            ValueChange_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            DB_Default_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            ExecuteCMDsql_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            ValueCMDsql_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            ThemDiag_Cbx.KeyPress += new KeyPressEventHandler(Object_Keypress);
            CategorieFeed_Cbx.KeyPress += new KeyPressEventHandler(Object_Keypress);
            NewCategory_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            NewFeed_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            GoFeed_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            GmapProvider_Cbx.KeyPress += new KeyPressEventHandler(Object_Keypress);
            LatLon_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            KeywordMap_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            ZoomValMap_Lbl.KeyPress += new KeyPressEventHandler(Object_Keypress);
            OrderLL_txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            UrlHome_Opt_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            UrlTradWebPage_Opt_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            SearchEngine_Opt_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            UserAgent_Opt_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            UserAgentHttp_Opt_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            GoogBot_Opt_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            DefaultEditor_Opt_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            CyberChef_Opt_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            URL_URL_Cbx.KeyPress += new KeyPressEventHandler(Object_Keypress);
            URL_SAVE_Cbx.KeyPress += new KeyPressEventHandler(Object_Keypress);
            Construct_URL_Cbx.KeyPress += new KeyPressEventHandler(Object_Keypress);
            Word_URL_Builder_Txt.KeyPress += new KeyPressEventHandler(Object_Keypress);
            AddOn_Cbx.KeyPress += new KeyPressEventHandler(Object_Keypress);

            UrlHome_Opt_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            UrlTradWebPage_Opt_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            SearchEngine_Opt_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            UserAgent_Opt_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            UserAgentHttp_Opt_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            GoogBot_Opt_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            DefaultEditor_Opt_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            CyberChef_Opt_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            ArchiveAdd_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            JsonUri_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            JsonVal_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            JsonNode_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            JsonUsrAgt_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            TextMarker_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            NameProjectwf_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            Author_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            ModelName_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            AddSingleItemswf_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            ExecuteCMDsql_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
            ValueCMDsql_Txt.DoubleClick += new EventHandler(ClearObject_Keypress);
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
                if (lstUrlDfltCnf.Count > 0)
                {
                    try
                    {
                        dbDflt = lstUrlDfltCnf[0].ToString();
                        urlHom = lstUrlDfltCnf[1].ToString();
                        urlTra = lstUrlDfltCnf[2].ToString();
                        Search = lstUrlDfltCnf[3].ToString();
                        UsrAgt = lstUrlDfltCnf[4].ToString();
                        UsrHtt = lstUrlDfltCnf[5].ToString();
                        GoogBo = lstUrlDfltCnf[6].ToString();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                            "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    catch (ArgumentException ex)
                    {
                        senderror.ErrorLog("Error! lstUrlDfltCnf CreateConfigFile: ", ex.ToString(), "Main_Frm", AppStart);
                    }
                }

                DefaultEditor_Opt_Txt.Text = Path.Combine(AppStart, "OstiumE.exe");
                Redlist_Txt.Text = Path.Combine(AppStart, "data", BlockedUrl);
                CyberChef_Opt_Txt.Text = "";
            }
            ///
            /// Create XML file "config.xml"
            ///
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                CheckCharacters = true
            };

            using (XmlWriter writer = XmlWriter.Create(Path.Combine(AppStart, "config.xml"), settings))
            {
                writer.WriteStartElement("Xwparsingxml");
                writer.WriteStartElement("Xwparsingnode");
                writer.WriteElementString("DB_USE_DEFAULT", dbDflt);
                writer.WriteElementString("URL_HOME_VAR", urlHom);
                writer.WriteElementString("URL_TRAD_WEBPAGE_VAR", urlTra);
                //writer.WriteElementString("URL_TRAD_WEBTXT_VAR", "https://translate.google.fr/?hl=fr&sl=auto&tl=fr&text=replace_query&op=translate"); // Not implemented
                writer.WriteElementString("URL_DEFAUT_WSEARCH_VAR", Search);
                writer.WriteElementString("URL_USER_AGENT_VAR", UsrAgt);
                writer.WriteElementString("URL_USER_AGENT_SRC_PAGE_VAR", UsrHtt);
                writer.WriteElementString("URL_GOOGLEBOT_VAR", GoogBo);
                writer.WriteElementString("DEFAULT_EDITOR_VAR", DefaultEditor_Opt_Txt.Text);
                writer.WriteElementString("CYBERCHEF_VAR", CyberChef_Opt_Txt.Text);
                writer.WriteElementString("REDLIST_VAR", Redlist_Txt.Text);
                writer.WriteElementString("VOLUME_TRACK_VAR", Convert.ToString(VolumeVal_Track.Value));
                writer.WriteElementString("RATE_TRACK_VAR", Convert.ToString(RateVal_Track.Value));
                writer.WriteEndElement();
                writer.Flush();
            }

            if (ArchiveAdd_Txt.Text != string.Empty)
            {
                using (StreamWriter fc = new StreamWriter(Path.Combine(AppStart, "archiveAdd.txt")))
                {
                    fc.Write(ArchiveAdd_Txt.Text);
                }
            }
            ///
            /// Loading the configuration from the "config.xml" file
            ///
            LoadConfiguration(Path.Combine(AppStart, "config.xml"));
        }

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
                        Workflow,
                        Workflow + "model",
                        DiagramDir,
                        BkmkltDir,
                        Setirps,
                        MapDir,
                        JsonDir,
                        JsonDir + "table",
                        Keeptrack + "thumbnails"
                    };

                for (int i = 0; i < CreateDir.Count; i++)
                {
                    DirectoryCreate(CreateDir[i].ToString());
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateDirectory: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void DirectoryCreate(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        #region LoadConfiguration_

        void LoadConfiguration(string configFile)
        {
            try
            {
                LoadConfigFromXml(configFile);
                InitializeDatabase();
                LoadAdditionalFiles();
                InitializeClassVariables();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error in LoadConfiguration: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void LoadConfigFromXml(string configFile)
        {
            using (XmlReader reader = XmlReader.Create(configFile))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "DB_USE_DEFAULT":
                                DB_Default_Txt.Text = Convert.ToString(reader.ReadString());
                                DB_Default_Opt_Txt.Text = DB_Default_Txt.Text;
                                break;
                            case "URL_HOME_VAR":
                                Class_Var.URL_HOME = reader.ReadString();
                                UrlHome_Opt_Txt.Text = Class_Var.URL_HOME;
                                break;
                            case "URL_TRAD_WEBPAGE_VAR":
                                Class_Var.URL_TRAD_WEBPAGE = reader.ReadString();
                                UrlTradWebPage_Opt_Txt.Text = Class_Var.URL_TRAD_WEBPAGE;
                                break;
                            case "URL_TRAD_WEBTXT_VAR":
                                Class_Var.URL_TRAD_WEBTXT = reader.ReadString();
                                break;
                            case "URL_DEFAUT_WSEARCH_VAR":
                                Class_Var.URL_DEFAUT_WSEARCH = reader.ReadString();
                                SearchEngine_Opt_Txt.Text = Class_Var.URL_DEFAUT_WSEARCH;
                                break;
                            case "URL_USER_AGENT_VAR":
                                Class_Var.URL_USER_AGENT = reader.ReadString();
                                UserAgent_Opt_Txt.Text = Class_Var.URL_USER_AGENT;
                                break;
                            case "URL_USER_AGENT_SRC_PAGE_VAR":
                                Class_Var.URL_USER_AGENT_SRC_PAGE = reader.ReadString();
                                UserAgentHttp_Opt_Txt.Text = Class_Var.URL_USER_AGENT_SRC_PAGE;
                                JsonUsrAgt_Txt.Text = Class_Var.URL_USER_AGENT_SRC_PAGE;
                                break;
                            case "URL_GOOGLEBOT_VAR":
                                Class_Var.URL_GOOGLEBOT = reader.ReadString();
                                GoogBot_Opt_Txt.Text = Class_Var.URL_GOOGLEBOT;
                                break;
                            case "DEFAULT_EDITOR_VAR":
                                Class_Var.DEFAULT_EDITOR = reader.ReadString();
                                DefaultEditor_Opt_Txt.Text = Class_Var.DEFAULT_EDITOR;
                                break;
                            case "CYBERCHEF_VAR":
                                CyberChef_Opt_Txt.Text = Convert.ToString(reader.ReadString());
                                if (!string.IsNullOrEmpty(CyberChef_Opt_Txt.Text))
                                    CyberChef_Btn.Enabled = true;
                                else
                                    CyberChef_Btn.Enabled = false;
                                break;
                            case "REDLIST_VAR":
                                Redlist_Txt.Text = Convert.ToString(reader.ReadString());
                                BlockedUrl = Redlist_Txt.Text;
                                LoadBlockList();
                                break;
                            case "VOLUME_TRACK_VAR":
                                Class_Var.VOLUME_TRACK = Convert.ToInt32(reader.ReadString());
                                VolumeVal_Track.Value = Class_Var.VOLUME_TRACK;
                                break;
                            case "RATE_TRACK_VAR":
                                Class_Var.RATE_TRACK = Convert.ToInt32(reader.ReadString());
                                RateVal_Track.Value = Class_Var.RATE_TRACK;
                                break;
                        }
                    }
                }
            }
        }

        void InitializeDatabase()
        {
            if (DB_Default_Txt.Text != "0x0")
            {
                databasePath = Path.Combine(DBdirectory, DB_Default_Txt.Text);
            }
            else
            {
                string message = "First use. \n\nChoose a name for the database or leave empty.";
                string title = "Database default name";
                string userInput = Interaction.InputBox(message, title);
                string sanitizedInput = Regex.Replace(userInput, "[^a-zA-Z0-9]", string.Empty) + ".db";

                if (!string.IsNullOrEmpty(sanitizedInput) && sanitizedInput != ".db")
                {
                    databasePath = Path.Combine(DBdirectory, sanitizedInput);
                    if (!File.Exists(databasePath))
                        SQLiteConnection.CreateFile(databasePath);

                    DB_Default_Txt.Text = sanitizedInput;
                    ChangeDBdefault(sanitizedInput);
                }
                else
                {
                    DB_Default_Txt.Text = "D4taB.db";
                    databasePath = Path.Combine(DBdirectory, "D4taB.db");
                    if (!File.Exists(databasePath))
                        SQLiteConnection.CreateFile(databasePath);

                    ChangeDBdefault("D4taB.db");
                }
                DB_Default_Opt_Txt.Text = DB_Default_Txt.Text;
            }
        }

        void LoadAdditionalFiles()
        {
            if (File.Exists(Path.Combine(FileDir, "url.txt")))
            {
                URL_URL_Cbx.Items.Clear();
                URL_URL_Cbx.Items.AddRange(File.ReadAllLines(Path.Combine(FileDir, "url.txt")));
            }

            if (File.Exists(Path.Combine(FileDir, "url-constructor", "construct_url.txt")))
            {
                ConstructURL_Lst.Items.Clear();
                ConstructURL_Lst.Items.AddRange(File.ReadAllLines(Path.Combine(FileDir, "url-constructor", "construct_url.txt")));
            }
            ///
            /// Loading JS scripts from "script url.ost" file for injection.
            /// 
            if (File.Exists(Path.Combine(Scripts, "scripturl.ost")))
            {
                ScriptUrl_Lst.Items.Clear();
                ScriptUrl_Lst.Items.AddRange(File.ReadAllLines(Path.Combine(Scripts, "scripturl.ost")));
            }

            if (File.Exists(Path.Combine(AppStart, "archiveAdd.txt")))
            {
                using (StreamReader sr = new StreamReader(Path.Combine(AppStart, "archiveAdd.txt")))
                {
                    ArchiveAdd_Txt.Text = sr.ReadToEnd();
                }
                ArchiveAdd_Lst.Items.AddRange(File.ReadAllLines(Path.Combine(AppStart, "archiveAdd.txt")));
            }

            loadfiledir.LoadFileDirectory(Plugins, "exe", "cbxts", AddOn_Cbx);
            loadfiledir.LoadFileDirectory(Path.Combine(FileDir, "url-constructor"), "txt", "cbxts", Construct_URL_Cbx);
            loadfiledir.LoadFileDirectory(FeedDir, "*", "cbxts", CategorieFeed_Cbx);
            loadfiledir.LoadFileDirectory(Workflow, "xml", "lst", ProjectOpn_Lst);
            loadfiledir.LoadFileDirectory(WorkflowModel, "txt", "lst", ModelList_Lst);
            loadfiledir.LoadFileDirectory(Path.Combine(Scripts, "scriptsl"), "js", "splitb", TtsButton_Sts);
        }

        void InitializeClassVariables()
        {
            Class_Var.COOKIES_SAVE = 0; // Save all cookies in the cookie.txt file at the root if SaveCookies_Chk checked = True, default = False
            Class_Var.SCRIPTCREATOR = "off";
        }

        #endregion

        async void UpdateDirectorySize(string directoryPath, object objectsend)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
                long dirSize = await Task.Run(() => dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));
                string formattedSize = sizedireturn.GetSizeName(long.Parse(Convert.ToString(dirSize)));

                SizeAll_Lbl = (Label)objectsend;
                SizeAll_Lbl.Text = formattedSize;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! UpdateDirectorySize: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        #region Browser_Event Handler
        /// <summary>
        /// Block Ads/Trackers
        /// </summary>
        void WBrowse_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            try
            {
                if (IsAdsTrackersBlocked)
                {
                    string requestUri = e.Request.Uri;

                    if (ShouldBlockUrlFast(requestUri))
                    {
                        try
                        {
                            var emptyStream = new MemoryStream();

                            e.Response = WBrowse.CoreWebView2.Environment.CreateWebResourceResponse(
                                emptyStream,
                                204,
                                "No Content",
                                "Content-Type: text/plain");

                            urlBlocked += 1;
                            Count_urlCache.Text = $"Blocked {urlBlocked}";
                            //Task.Run(() => Console.WriteLine($"[BLOCKED RESOURCE] {requestUri}"));
                        }
                        catch
                        {
                            senderror.ErrorLog("Error! WBrowse_WebResourceRequested: ", $"[BLOCKED - Fallback] {requestUri}", "Main_Frm", AppStart);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! WBrowse_WebResourceRequested: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        bool ShouldBlockUrlFast(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (_urlCache.TryGetValue(url, out bool cachedResult))
            {
                return cachedResult;
            }

            bool shouldBlock = false;

            try
            {
                // WHITELIST verify
                if (IsUrlAllowed(url))
                {
                    shouldBlock = false;
                    // Caching
                    if (_urlCache.Count < MAX_CACHE_SIZE)
                        _urlCache[url] = false;
                    return false;
                }

                // Host
                string host = ExtractHostFast(url);

                // Verify Host (HashSet = O(1))
                if (!string.IsNullOrEmpty(host) && _blockedHosts.Contains(host))
                {
                    shouldBlock = true;
                }
                else if (!string.IsNullOrEmpty(host))
                {
                    // Subdomains
                    foreach (var blockedHost in _blockedHosts)
                    {
                        if (host.EndsWith("." + blockedHost, StringComparison.OrdinalIgnoreCase))
                        {
                            shouldBlock = true;
                            break;
                        }
                    }
                }

                // URLs complete
                if (!shouldBlock)
                {
                    string normalizedUrl = NormalizeUrlFast(url);

                    if (_blockedFullUrls.Contains(normalizedUrl))
                    {
                        shouldBlock = true;
                    }
                    else
                    {
                        // Prefixes
                        foreach (var blockedUrl in _blockedFullUrls)
                        {
                            if (normalizedUrl.StartsWith(blockedUrl, StringComparison.OrdinalIgnoreCase))
                            {
                                shouldBlock = true;
                                break;
                            }
                        }
                    }
                }

                // Patterns
                if (!shouldBlock && _compiledPatterns.Count > 0)
                {
                    foreach (var regex in _compiledPatterns)
                    {
                        if (regex.IsMatch(url))
                        {
                            shouldBlock = true;
                            break;
                        }
                    }
                }
            }
            catch
            {
                shouldBlock = false;
            }

            // Caching
            if (_urlCache.Count < MAX_CACHE_SIZE)
            {
                _urlCache[url] = shouldBlock;
            }
            else if (_urlCache.Count >= MAX_CACHE_SIZE)
            {
                // Clear cache
                _urlCache.Clear();
            }

            return shouldBlock;
        }

        bool IsUrlAllowed(string url)
        {
            try
            {
                // Authorized host
                string host = ExtractHostFast(url);
                if (!string.IsNullOrEmpty(host))
                {
                    if (_allowedHosts.Contains(host))
                        return true;

                    // Allowed subdomains
                    foreach (var allowedHost in _allowedHosts)
                    {
                        if (host.EndsWith("." + allowedHost, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }

                // URLs complete
                string normalizedUrl = NormalizeUrlFast(url);
                if (_allowedFullUrls.Contains(normalizedUrl))
                    return true;

                // Prefixe
                foreach (var allowedUrl in _allowedFullUrls)
                {
                    if (normalizedUrl.StartsWith(allowedUrl, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                // Patterns
                if (_compiledAllowedPatterns.Count > 0)
                {
                    foreach (var regex in _compiledAllowedPatterns)
                    {
                        if (regex.IsMatch(url))
                            return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        string ExtractHostFast(string url)
        {
            try
            {
                // http:// ou https://
                int schemeEnd = url.IndexOf("://");
                if (schemeEnd < 0) return null;

                int hostStart = schemeEnd + 3;
                int hostEnd = url.IndexOf('/', hostStart);
                if (hostEnd < 0) hostEnd = url.IndexOf('?', hostStart);
                if (hostEnd < 0) hostEnd = url.IndexOf('#', hostStart);
                if (hostEnd < 0) hostEnd = url.Length;

                string host = url.Substring(hostStart, hostEnd - hostStart);

                // Remove the port if present
                int portIndex = host.IndexOf(':');
                if (portIndex > 0)
                {
                    host = host.Substring(0, portIndex);
                }

                return host.ToLowerInvariant();
            }
            catch
            {
                // Fallback on Uri.TryCreate
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    return uri.Host;
                }
                return null;
            }
        }

        void LoadBlockedHosts(string filePath)
        {
            try
            {
                _blockedFullUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _blockedHosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _blockedPatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _compiledPatterns = new List<Regex>();

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath,
                        "# Add here the domains or URLs to block\n" +
                        "# Examples:\n" +
                        "# ads.example.com\n" +
                        "# *ads*\n" +
                        "# https://tracking.example.com/track.js\n");
                    return;
                }

                var lines = File.ReadAllLines(filePath)
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#"));

                foreach (var line in lines)
                {
                    if (line.Contains("*"))
                    {
                        // Pattern with wildcards - PRE-COMPILE regexes
                        _blockedPatterns.Add(line);
                        try
                        {
                            string regexPattern = "^" + Regex.Escape(line)
                                .Replace("\\*", ".*")
                                .Replace("\\?", ".") + "$";

                            var regex = new Regex(
                                regexPattern,
                                RegexOptions.IgnoreCase |
                                RegexOptions.Compiled);

                            _compiledPatterns.Add(regex);
                        }
                        catch { }
                    }
                    else if (line.StartsWith("http://") || line.StartsWith("https://"))
                    {
                        // Full URL - normalize
                        string normalized = NormalizeUrlFast(line);
                        _blockedFullUrls.Add(normalized);

                        // Extract host
                        if (Uri.TryCreate(line, UriKind.Absolute, out var uri))
                        {
                            _blockedHosts.Add(uri.Host);
                        }
                    }
                    else
                    {
                        // Domaine/host
                        _blockedHosts.Add(line);
                    }
                }

                // Clear cache after reload
                _urlCache.Clear();

                Console.WriteLine($"[CHARGED BLOCKLIST] {_blockedFullUrls.Count} URLs, {_blockedHosts.Count} hosts, {_blockedPatterns.Count} patterns");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading redlist : {ex.Message}");
                _blockedFullUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _blockedHosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _blockedPatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _compiledPatterns = new List<Regex>();
            }
        }

        void LoadAllowedHosts(string filePath)
        {
            try
            {
                _allowedFullUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _allowedHosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _allowedPatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _compiledAllowedPatterns = new List<Regex>();

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath,
                        "# Authorized URLs and domains (takes priority over blocked.txt)\n" +
                        "# Examples:\n" +
                        "# cdn.example.com\n" +
                        "# https://static.facebook.com/specific-needed-script.js\n" +
                        "# *important*\n");
                    return;
                }

                var lines = File.ReadAllLines(filePath)
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#"));

                foreach (var line in lines)
                {
                    if (line.Contains("*"))
                    {
                        // Pattern with wildcards
                        _allowedPatterns.Add(line);
                        try
                        {
                            string regexPattern = "^" + Regex.Escape(line)
                                .Replace("\\*", ".*")
                                .Replace("\\?", ".") + "$";

                            var regex = new Regex(
                                regexPattern,
                                RegexOptions.IgnoreCase |
                                RegexOptions.Compiled);

                            _compiledAllowedPatterns.Add(regex);
                        }
                        catch { }
                    }
                    else if (line.StartsWith("http://") || line.StartsWith("https://"))
                    {
                        // URLs complete
                        string normalized = NormalizeUrlFast(line);
                        _allowedFullUrls.Add(normalized);

                        // Extract host
                        if (Uri.TryCreate(line, UriKind.Absolute, out var uri))
                        {
                            _allowedHosts.Add(uri.Host);
                        }
                    }
                    else
                    {
                        // Domaine/host
                        _allowedHosts.Add(line);
                    }
                }

                // Clear cache after reload
                _urlCache.Clear();

                Console.WriteLine($"[CHARGED WHITELIST] {_allowedFullUrls.Count} URLs, {_allowedHosts.Count} hosts, {_allowedPatterns.Count} patterns");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading whitelist : {ex.Message}");
                _allowedFullUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _allowedHosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _allowedPatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _compiledAllowedPatterns = new List<Regex>();
            }
        }

        string NormalizeUrlFast(string url)
        {
            try
            {
                int fragmentIndex = url.IndexOf('#');
                if (fragmentIndex > 0)
                {
                    url = url.Substring(0, fragmentIndex);
                }
                return url.ToLowerInvariant();
            }
            catch
            {
                return url.ToLowerInvariant();
            }
        }

        public void LoadBlockList()
        {
            LoadBlockedHosts(Path.Combine(AppStart, "data", BlockedUrl));
            LoadAllowedHosts(Path.Combine(AppStart, "data", AllowUrl));
        }
        /// \\\

        ///
        /// <summary>
        /// Adding an item to the wBrowse Context Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="newItem0">Speech</param>
        /// <param name="newItem1">Search on the WayBackMachine</param>
        /// <param name="newItem2">Embed Youtube video</param>
        /// <param name="newItem3">Embed Youtube video in new Tab</param>
        /// <param name="newItem4">Text select Auto Clipboard</param>
        /// <param name="newItem5">Keep Track</param>
        /// <param name="newItem6">Tracking tool</param>
        /// 
        void WBrowse_ContextMenuRequested(object sender, CoreWebView2ContextMenuRequestedEventArgs args)
        {
            IList<CoreWebView2ContextMenuItem> menuList = args.MenuItems;

            CoreWebView2ContextMenuItem newItem0 = WBrowse.CoreWebView2.Environment.CreateContextMenuItem("Speech", null, CoreWebView2ContextMenuItemKind.Command);
            newItem0.CustomItemSelected += delegate (object send, object ex)
            {
                if (VerifLangOpn == 0)
                    LoadLang();

                Speak_Clipboard_Text();
            };

            CoreWebView2ContextMenuItem newItem1 = WBrowse.CoreWebView2.Environment.CreateContextMenuItem("Search on WayBackMachine", null, CoreWebView2ContextMenuItemKind.Command);
            newItem1.CustomItemSelected += delegate (object send, object ex)
            {
                string pageUri = args.ContextMenuTarget.PageUri; ;

                SynchronizationContext.Current.Post((_) =>
                {
                    GoBrowser("http://web.archive.org/web/*/" + pageUri, 1);
                }, null);
            };

            CoreWebView2ContextMenuItem newItem2 = WBrowse.CoreWebView2.Environment.CreateContextMenuItem("Youtube embed", null, CoreWebView2ContextMenuItemKind.Command);
            newItem2.CustomItemSelected += delegate (object send, object ex)
            {
                SynchronizationContext.Current.Post((_) =>
                {
                    string currentUrl = WBrowse.Source.ToString();

                    if (currentUrl.Contains("youtube.com/watch?v="))
                    {
                        int vIndex = currentUrl.IndexOf("v=") + 2;
                        string videoId = currentUrl.Substring(vIndex, 11);

                        string encodedUrl = Uri.EscapeDataString(currentUrl);
                        string customUrl = $"https://icaza.github.io/?v={videoId}&url={encodedUrl}";

                        GoBrowser(customUrl, 0);
                    }
                    else
                    {
                        MessageBox.Show("You are not on Youtube!");
                    }
                }, null);
            };

            CoreWebView2ContextMenuItem newItem3 = WBrowse.CoreWebView2.Environment.CreateContextMenuItem("Youtube embed new Tab", null, CoreWebView2ContextMenuItemKind.Command);
            newItem3.CustomItemSelected += delegate (object send, object ex)
            {
                SynchronizationContext.Current.Post((_) =>
                {
                    string currentUrl = WBrowse.Source.ToString();

                    if (currentUrl.Contains("youtube.com/watch?v="))
                    {
                        int vIndex = currentUrl.IndexOf("v=") + 2;
                        string videoId = currentUrl.Substring(vIndex, 11);

                        string encodedUrl = Uri.EscapeDataString(currentUrl);
                        string customUrl = $"https://icaza.github.io/?v={videoId}&url={encodedUrl}";

                        GoBrowser(customUrl, 1);
                    }
                    else
                    {
                        MessageBox.Show("You are not on Youtube!");
                    }
                }, null);
            };

            CoreWebView2ContextMenuItem newItem4 = WBrowse.CoreWebView2.Environment.CreateContextMenuItem("Text select Auto Clipboard", null, CoreWebView2ContextMenuItemKind.Command);
            newItem4.CustomItemSelected += delegate (object send, object ex)
            {
                InjectScript("(function(){const n=document.createElement(\"div\");n.style.position=\"fixed\";n.style.top=\"10px\";n.style.left=\"10px\";n.style.width=\"15px\";n.style.height=\"15px\";n.style.borderRadius=\"50%\";n.style.backgroundColor=\"red\";n.style.zIndex=\"9999\";n.title=\"Automatic copy script is activated\";document.body.appendChild(n);document.addEventListener(\"mouseup\",()=>{const t=window.getSelection(),n=t.toString();n&&navigator.clipboard.writeText(n).then(()=>{console.log(\"Text: \",n)}).catch(n=>{console.error(\"Error: \",n)})})})()");
            };

            CoreWebView2ContextMenuItem newItem5 = WBrowse.CoreWebView2.Environment.CreateContextMenuItem("Keep Track", null, CoreWebView2ContextMenuItemKind.Command);
            newItem5.CustomItemSelected += delegate (object send, object ex)
            {
                IsTimelineEnabled = !IsTimelineEnabled;

                if (IsTimelineEnabled)
                {
                    Browser_Tab.BackColor = Color.Red;
                    CreateNameAleat();
                    FileTimeLineName = Una + ".csv";

                    if (!IsParentLinkEnabled)
                    {
                        IsParentLinkEnabled = !IsParentLinkEnabled;
                        ForceLinkParent_Btn.ForeColor = Color.Red;
                    }
                }
                else
                    Browser_Tab.BackColor = Color.FromArgb(41, 44, 51);
            };

            CoreWebView2ContextMenuItem newItem6 = WBrowse.CoreWebView2.Environment.CreateContextMenuItem("Tracking tool", null, CoreWebView2ContextMenuItemKind.Command);
            newItem6.CustomItemSelected += delegate (object send, object ex)
            {
                keeptrackForm = new Keeptrack_Frm();
                keeptrackForm.Show();

                if (!IsParentLinkEnabled)
                {
                    IsParentLinkEnabled = !IsParentLinkEnabled;
                    ForceLinkParent_Btn.ForeColor = Color.Red;
                }
            };

            menuList.Insert(menuList.Count, newItem0);
            menuList.Insert(menuList.Count, newItem1);
            menuList.Insert(menuList.Count, newItem2);
            menuList.Insert(menuList.Count, newItem3);
            menuList.Insert(menuList.Count, newItem4);
            menuList.Insert(menuList.Count, newItem5);
            menuList.Insert(menuList.Count, newItem6);
        }
        ///
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
        /// <param value="on">If enabled user-agent modification</param>
        /// 
        async void WBrowse_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var settings = WBrowse.CoreWebView2.Settings;
            /// Block Ads/Trackers
            if (IsAdsTrackersBlocked)
            {
                try
                {
                    urlBlocked = 0;
                    Count_urlCache.Text = $"Blocked {urlBlocked}";

                    if (ShouldBlockUrlFast(e.Uri))
                    {
                        e.Cancel = true;

                        urlBlocked += 1;
                        Count_urlCache.Text = $"Blocked {urlBlocked}";
                        //_ = Task.Run(() => Console.WriteLine($"[NAVIGATION BLOCKED] {e.Uri}"));
                    }
                }
                catch (Exception ex)
                {
                    senderror.ErrorLog("Error! WBrowse_NavigationStarting: ", ex.ToString(), "Main_Frm", AppStart);
                }
            }
            /// \\\
            if (UserAgentOnOff == "on")
            {
                settings.UserAgent = UserAgentSelect;
            }

            if (FloodHeader_Chk.Checked)
            {
                HeaderFlood = new FloodHeader(WBrowse.CoreWebView2);
                await HeaderFlood.FloodHeaderAsync();

                await WBrowse.EnsureCoreWebView2Async();
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

            ScripInj();

            if (IsTimelineEnabled)
            {
                var logger = new VisitLogger(Path.Combine(Keeptrack, FileTimeLineName));
                logger.LogVisit(WBrowse.Source.AbsoluteUri, "No tag");

                FaviconLoad();
            }
        }

        async void ScripInj()
        {
            await ScriptInject();
        }

        async void FaviconLoad()
        {
            var downloader = new FaviconDownloader();

            try
            {
                string icoName = GenerateFileName(WBrowse.Source.IdnHost);

                if (!File.Exists(Path.Combine(Keeptrack, "thumbnails", icoName + ".ico")))
                {
                    var favicon = await downloader.GetFaviconAsync(WBrowse.Source.AbsoluteUri);
                    File.WriteAllBytes(Path.Combine(Keeptrack, "thumbnails", icoName + ".ico"), favicon);
                    //Console.WriteLine("Favicon sucess download !");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
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
        /// 
        void WBrowse_DocumentTitleChanged(object sender, object e)
        {
            Text = WBrowse.CoreWebView2.DocumentTitle;
            NameUriDB = WBrowse.CoreWebView2.DocumentTitle;

            WBrowse_UpdtTitleEvent("DocumentTitleChanged");
        }

        void NewWindow_Requested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            if (IsTimelineEnabled || IsParentLinkEnabled)
            {
                e.Handled = true; // Force _parent Links
                WBrowse.CoreWebView2.Navigate(e.Uri);
            }
        }

        void WBrowse_InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                MessageBox.Show($"WBrowse creation failed with exception = {e.InitializationException}");
                WBrowse_UpdtTitleEvent("Initialization Completed failed");
                return;
            }

            WBrowse.CoreWebView2.HistoryChanged += WBrowse_HistoryChanged;
            WBrowse.CoreWebView2.DocumentTitleChanged += WBrowse_DocumentTitleChanged;
            WBrowse.CoreWebView2.ContextMenuRequested += WBrowse_ContextMenuRequested;
            WBrowse.CoreWebView2.NewWindowRequested += NewWindow_Requested;

            WBrowse.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            WBrowse.CoreWebView2.WebResourceRequested += WBrowse_WebResourceRequested;

            WBrowse.CoreWebView2.Settings.AreHostObjectsAllowed = false;
            WBrowse.CoreWebView2.Settings.IsWebMessageEnabled = false;
            WBrowse.CoreWebView2.Settings.IsScriptEnabled = true;
            WBrowse.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;

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

        void WBrowsefeed_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            try
            {
                if (IsAdsTrackersBlocked)
                {
                    string requestUri = e.Request.Uri;

                    if (ShouldBlockUrlFast(requestUri))
                    {
                        try
                        {
                            var emptyStream = new MemoryStream();

                            e.Response = WBrowsefeed.CoreWebView2.Environment.CreateWebResourceResponse(
                                emptyStream,
                                204,
                                "No Content",
                                "Content-Type: text/plain");

                            urlBlocked += 1;
                            Count_urlCache.Text = $"Blocked {urlBlocked}";
                            //Task.Run(() => Console.WriteLine($"[BLOCKED RESOURCE] {requestUri}"));
                        }
                        catch
                        {
                            senderror.ErrorLog("Error! WBrowsefeed_WebResourceRequested: ", $"[BLOCKED - Fallback] {requestUri}", "Main_Frm", AppStart);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! WBrowsefeed_WebResourceRequested: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void WBrowsefeed_UpdtTitleEvent(string message)
        {
            string currentDocumentTitle = WBrowsefeed?.CoreWebView2?.DocumentTitle ?? "Uninitialized";
            Text = currentDocumentTitle + " [" + message + "]";
            TmpTitleWBrowsefeed = Text;
        }

        async void WBrowsefeed_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            /// Block Ads/Trackers
            if (IsAdsTrackersBlocked)
            {
                try
                {
                    urlBlocked = 0;
                    Count_urlCache.Text = $"Blocked {urlBlocked}";

                    if (ShouldBlockUrlFast(e.Uri))
                    {
                        e.Cancel = true;

                        urlBlocked += 1;
                        Count_urlCache.Text = $"Blocked {urlBlocked}";
                        //_ = Task.Run(() => Console.WriteLine($"[NAVIGATION BLOCKED] {e.Uri}"));
                    }
                }
                catch (Exception ex)
                {
                    senderror.ErrorLog("Error! WBrowsefeed_NavigationStarting: ", ex.ToString(), "Main_Frm", AppStart);
                }
            }
            /// \\\

            if (FloodHeader_Chk.Checked)
            {
                HeaderFlood = new FloodHeader(WBrowsefeed.CoreWebView2);
                await HeaderFlood.FloodHeaderAsync();

                await WBrowsefeed.EnsureCoreWebView2Async();
            }

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

            WBrowsefeed.CoreWebView2.HistoryChanged += WBrowsefeed_HistoryChanged;
            WBrowsefeed.CoreWebView2.DocumentTitleChanged += WBrowsefeed_DocumentTitleChanged;

            WBrowsefeed.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            WBrowsefeed.CoreWebView2.WebResourceRequested += WBrowse_WebResourceRequested;

            WBrowsefeed.CoreWebView2.Settings.AreHostObjectsAllowed = false;
            WBrowsefeed.CoreWebView2.Settings.IsWebMessageEnabled = false;
            WBrowsefeed.CoreWebView2.Settings.IsScriptEnabled = true;

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
        /// <param name="WBrowse.Source.AbsoluteUri">URL of the current site, verify if URL exist on list script, if exist removing special characters
        ///  from the URL and adding the ".js" extension to check if a ".js" script name of the same name exists, if true injecting the script contained 
        ///  in the file on the current web page</param>
        /// 
        async Task ScriptInject()
        {
            try
            {
                if (ScriptUrl_Lst.Items.Count > 0)
                {
                    string currentUrl = WBrowse.Source.AbsoluteUri;

                    foreach (var item in ScriptUrl_Lst.Items)
                    {
                        if (currentUrl.Contains(item.ToString()))
                        {
                            string scriptName = GenerateFileName(currentUrl);
                            string filePath = Path.Combine(Scripts, scriptName + ".js");

                            if (File.Exists(filePath))
                            {
                                string scriptContent = File.ReadAllText(filePath);
                                await WBrowse.CoreWebView2.ExecuteScriptAsync(scriptContent);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ScriptInject: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        string GenerateFileName(string sdata)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(sdata));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
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
        void ClearData(int val)
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
                                      CoreWebView2BrowsingDataKinds.DiskCache |
                                      CoreWebView2BrowsingDataKinds.FileSystems |
                                      CoreWebView2BrowsingDataKinds.ServiceWorkers |
                                      CoreWebView2BrowsingDataKinds.Settings |
                                      CoreWebView2BrowsingDataKinds.WebSql |
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
        void GoBrowser(string inputUrl, int WebviewRedirect)
        {
            try
            {
                Uri uri;

                if (inputUrl.Contains("file:///"))
                {
                    string filePath = inputUrl.Replace("file:///", string.Empty);
                    if (File.Exists(filePath))
                    {
                        uri = new Uri(inputUrl);
                    }
                    else
                    {
                        throw new FileNotFoundException($"File not found: {filePath}");
                    }
                }
                else if (Uri.IsWellFormedUriString(inputUrl, UriKind.Absolute))
                {
                    uri = new Uri(inputUrl);
                }
                else if (!inputUrl.Contains(" ") && inputUrl.Contains("."))
                {
                    uri = new Uri("https://" + inputUrl);
                }
                else
                {
                    if (string.IsNullOrEmpty(Class_Var.URL_DEFAUT_WSEARCH))
                    {
                        try
                        {
                            if (lstUrlDfltCnf.Count > 0)
                                Class_Var.URL_DEFAUT_WSEARCH = lstUrlDfltCnf[3].ToString();
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                                "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        catch (ArgumentException ex)
                        {
                            senderror.ErrorLog("Error! lstUrlDfltCnf GoBrowser: ", ex.ToString(), "Main_Frm", AppStart);
                        }
                    }

                    ///
                    /// Change, for those who use Google as their default search engine, the link used was the following 
                    /// https://www.google.com/search?q=SEARCHWORD you just had to enter the search term in Ostium or the Url. 
                    /// For the Url it is therefore direct for the search it was a combination of the Google Url + the search term. 
                    /// This method is deprecated and requires us to validate a stupid captha. So I modified the code so that it 
                    /// opens the Google search page (for those who use it) rather than leaving the automation to facilitate a step. 
                    /// I grayed out the old line for those who want to keep it in the code.
                    ///
                    /// uri = new Uri(Class_Var.URL_DEFAUT_WSEARCH + Uri.EscapeDataString(inputUrl));
                    uri = new Uri(Class_Var.URL_DEFAUT_WSEARCH);
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
                senderror.ErrorLog("Error! GoBrowser: ", ex.ToString(), "Main_Frm", AppStart);
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
                senderror.ErrorLog("Error! GoNewtab: ", ex.ToString(), "Main_Frm", AppStart);
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
            if (@Class_Var.URL_HOME == string.Empty)
            {
                try
                {
                    if (lstUrlDfltCnf.Count > 0)
                        @Class_Var.URL_HOME = lstUrlDfltCnf[1].ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                        "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (ArgumentException ex)
                {
                    senderror.ErrorLog("Error! lstUrlDfltCnf Home_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
                }
            }

            WBrowse.Source = new Uri(@Class_Var.URL_HOME);
        }

        void OnKey_URLbrowse(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                int x;
                x = URLbrowse_Cbx.FindStringExact(URLbrowse_Cbx.Text);
                if (x == -1)
                    URLbrowse_Cbx.Items.Add(URLbrowse_Cbx.Text);

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
                senderror.ErrorLog("Error! URL_URL_Cbx_SelectedIndexChanged: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void CleanSearch_Btn_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Items.Clear();
            URLbrowse_Cbx.Text = string.Empty;
        }

        #endregion

        public class ConfigSFE
        {
            public int Port { get; set; }
        }

        #region Tools_Tab_0
        ///
        /// <summary>
        /// Modification of the user-agent by the replacement agent if configured "Class_Var.URL_USER_AGENT" or by the default agent in the file
        /// "url_dflt_cnf.ost" and/or restart the application to return to the default wBrowser agent
        /// </summary>
        /// <param name="ClearOnOff"></param>
        /// <param value="on">Clean request notification True</param>
        /// <param value="off">No notification of cleaning request when auto restart of the application to return the default user-agent</param>
        /// 
        void UserAgentChange_Btn_Click(object sender, EventArgs e)
        {
            ChangeUserAgent();
        }

        void ChangeUserAgent()
        {
            if (Class_Var.URL_USER_AGENT == string.Empty)
            {
                try
                {
                    if (lstUrlDfltCnf.Count > 0)
                        Class_Var.URL_USER_AGENT = lstUrlDfltCnf[4].ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                        "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (ArgumentException ex)
                {
                    senderror.ErrorLog("Error! lstUrlDfltCnf ChangeUserAgent: ", ex.ToString(), "Main_Frm", AppStart);
                }
            }

            bool isUserAgentChange = UserAgentChange_Btn.Text == "Change User Agent On";

            if (!isUserAgentChange)
            {
                UserAgentChange_Btn.Text = "Change User Agent On";
                UserAgentChange_Btn.ForeColor = Color.Red;
                UserAgentSelect = Class_Var.URL_USER_AGENT;
                UserAgentOnOff = "on";
            }
            else
            {
                ClearOnOff = "off";
                Process.Start(Path.Combine(AppStart, "Ostium.exe"));
                Close();
            }
        }
        ///
        /// <summary>
        /// Modification of the user-agent by a "GoogleBot" type agent if configured in "Class_Var.URL_USER_AGENT" or by the default agent in the file
        /// "url_dflt_cnf.ost" and/or restarting the application
        /// </summary>
        /// <param name="ClearOnOff"></param>
        /// <param value="on">Clean request notification True</param>
        /// <param value="off">No notification of cleaning request when auto restart of the application to return the default user-agent</param>
        /// 
        void Googlebot_Btn_Click(object sender, EventArgs e)
        {
            GoogleBot();
        }

        void GoogleBot()
        {
            if (Class_Var.URL_GOOGLEBOT == string.Empty)
            {
                try
                {
                    if (lstUrlDfltCnf.Count > 0)
                        Class_Var.URL_GOOGLEBOT = lstUrlDfltCnf[6].ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                        "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (ArgumentException ex)
                {
                    senderror.ErrorLog("Error! lstUrlDfltCnf GoogleBot: ", ex.ToString(), "Main_Frm", AppStart);
                }
            }

            bool isGooglebotOn = Googlebot_Btn.Text == "Googlebot On";

            if (!isGooglebotOn)
            {
                Googlebot_Btn.Text = "Googlebot On";
                Googlebot_Btn.ForeColor = Color.Red;
                UserAgentSelect = Class_Var.URL_GOOGLEBOT;
                UserAgentOnOff = "on";
            }
            else
            {
                ClearOnOff = "off";
                Process.Start(Path.Combine(AppStart, "Ostium.exe"));
                Close();
            }
        }
        ///
        /// <summary>
        /// Reformatting of the nickname/word entered in "Word_Construct_URL_Txt" before sending to => Construct_URL for URL construction
        /// </summary>
        /// <param name="Construct_URL">Creation of URL with the nickname or search word</param>
        /// 
        void Word_URL_Builder_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Word_URL_Builder_Txt.Text != string.Empty)
                {
                    Word_URL_Builder_Txt.Text = Word_URL_Builder_Txt.Text.Replace(" ", "%20");
                    URL_Builder(Word_URL_Builder_Txt.Text);
                    Console.Beep(800, 200);
                }
                else
                {
                    Word_URL_Builder_Txt.BackColor = Color.Red;
                    MessageBox.Show("First insert Name, ID or Word!");
                    Word_URL_Builder_Txt.BackColor = Color.FromArgb(41, 44, 51);
                    return;
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Word_Construct_URL_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

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
                senderror.ErrorLog("Error! Mute_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void CopyURL_Mnu_Click(object sender, EventArgs e)
        {
            try
            {
                string textData = WBrowse.Source.AbsoluteUri;
                Clipboard.SetData(DataFormats.Text, textData);
                Console.Beep(1500, 400);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CopyURL_Mnu_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Translation of the web page via the default translation site of the file "url_dflt_cnf.ost"
        /// or the one chosen by the user "Class_Var.URL_TRAD_WEBPAGE"
        /// </summary>
        /// <param name="replace_query">Replacement characters for the URL to translate and open in wBrowser</param>
        /// 
        void TraductPage_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Class_Var.URL_TRAD_WEBPAGE == string.Empty)
                {
                    try
                    {
                        if (lstUrlDfltCnf.Count > 0)
                            Class_Var.URL_TRAD_WEBPAGE = lstUrlDfltCnf[2].ToString();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                            "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    catch (ArgumentException ex)
                    {
                        senderror.ErrorLog("Error! lstUrlDfltCnf TraductPage_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
                    }
                }

                string formatURI = Regex.Replace(Class_Var.URL_TRAD_WEBPAGE, "replace_query", WBrowse.Source.AbsoluteUri);
                WBrowse.Source = new Uri(@formatURI);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! TraductPage_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void UnshortUrl_Btn_Click(object sender, EventArgs e)
        {
            if (URLbrowse_Cbx.Text != string.Empty)
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

        void StartUnshortUrl(string Uri)
        {
            UnshortURLval = Uri;

            Thread Thr_UnshortUrl = new Thread(new ThreadStart(UnshortUrl));
            Thr_UnshortUrl.Start();
        }

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
                senderror.ErrorLog("Error! => UnshortUrl() WebException: ", ex.ToString(), "Main_Frm", AppStart);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! => UnshortUrl() Exception: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void AddOn_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Tools_TAB_0.Focus();

                if (!File.Exists(Path.Combine(Plugins, AddOn_Cbx.Text)))
                    return;

                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = Path.Combine(Plugins, AddOn_Cbx.Text);
                    proc.StartInfo.Arguments = string.Empty;
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.WorkingDirectory = Plugins;
                    proc.StartInfo.RedirectStandardOutput = false;
                    proc.Start();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddOn_Cbx_SelectedIndexChanged: ", ex.ToString(), "Main_Frm", AppStart);
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
                senderror.ErrorLog("Error! Construct_URL_Cbx_SelectedIndexChanged: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Opening in the "OpenSource_Frm" window of the file "filesdir/gdork.txt" (Google Dork) if file exists True
        ///
        void GoogleDork_Btn_Click(object sender, EventArgs e)
        {
            GoogleDork();
        }

        void GoogleDork()
        {
            if (File.Exists(Path.Combine(FileDir, "gdork.txt")))
                Open_Source_Frm(Path.Combine(FileDir, "gdork.txt"));
        }

        void WebpageToPng_Btn_Click(object sender, EventArgs e)
        {
            WebpageCapture();
        }

        async void WebpageCapture()
        {
            try
            {
                string Domain = WBrowse.Source.IdnHost;
                var img = await TakeWebScreenshot();
                CreateNameAleat();
                string filePath = Path.Combine(Pictures, $"{Una}_{Domain}.png");

                img.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                Console.Beep(800, 200);

                Process.Start(filePath);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! WebpageToPng_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        async Task<Image> TakeWebScreenshot(bool currentControlClipOnly = false)
        {
            JObject scl = null;
            Size siz;

            if (!currentControlClipOnly)
            {
                await Task.Delay(500);

                var res = await WBrowse.CoreWebView2.ExecuteScriptAsync(@"
                JSON.stringify({
                    w: document.documentElement.scrollWidth, 
                    h: document.documentElement.scrollHeight
                });
            ");

                try
                {
                    scl = JObject.Parse(res.Trim('"').Replace("\\", ""));
                }
                catch (Exception ex)
                {
                    senderror.ErrorLog("JSON Parse Error in TakeWebScreenshot: ", ex.ToString(), "Main_Frm", AppStart);
                    scl = null;
                }
            }

            siz = scl != null && scl["w"] != null && scl["h"] != null
                ? new Size(
                    Math.Max((int)scl["w"], WBrowse.Width),
                    Math.Max((int)scl["h"], WBrowse.Height)
                  )
                : WBrowse.Size;

            var img = await GetWebBrowserBitmap(siz);
            return img;
        }

        async Task<Bitmap> GetWebBrowserBitmap(Size clipSize)
        {
            JObject clip = new JObject
            {
                ["x"] = 0,
                ["y"] = 0,
                ["width"] = clipSize.Width,
                ["height"] = clipSize.Height,
                ["scale"] = 1
            };

            JObject settings = new JObject
            {
                ["format"] = "png",
                ["clip"] = clip,
                ["fromSurface"] = true,
                ["captureBeyondViewport"] = true
            };

            string p = settings.ToString(Newtonsoft.Json.Formatting.None);

            var devData = await WBrowse.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", p);

            var parsedData = JObject.Parse(devData);
            if (parsedData.ContainsKey("data"))
            {
                var imgData = (string)parsedData["data"];
                var ms = new MemoryStream(Convert.FromBase64String(imgData));
                return (Bitmap)Image.FromStream(ms);
            }
            else
            {
                throw new Exception("Error: 'data' field is missing in devData response.");
            }
        }

        void HTMLtxt_Btn_Click(object sender, EventArgs e)
        {
            HtmlTextFrm = new HtmlText_Frm();
            HtmlTextFrm.Show();
        }

        void Cookie_Btn_Click(object sender, EventArgs e)
        {
            CookieLoad();
        }

        void CookieLoad()
        {
            if (!SaveCookies_Chk.Checked)
                MessageBox.Show("Saving cookies in a text file is not enabled in the options.");

            if (File.Exists(Path.Combine(AppStart, "cookie.txt")))
                OpenFile_Editor(Path.Combine(AppStart, "cookie.txt"));
        }

        void CookiesAdd_Btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CookieName_Txt.Text) ||
                string.IsNullOrWhiteSpace(CookieValue_Txt.Text) ||
                string.IsNullOrWhiteSpace(CookieDomain_Txt.Text))
            {
                MessageBox.Show("Enter all values!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CoreWebView2Cookie cookie = WBrowse.CoreWebView2.CookieManager.CreateCookie(CookieName_Txt.Text, CookieValue_Txt.Text, CookieDomain_Txt.Text, "/");
            WBrowse.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);

            Console.Beep(800, 200);
        }

        void SetCookie_Btn_Click(object sender, EventArgs e)
        {
            Cookie_Pnl.Visible = !Cookie_Pnl.Visible;
        }

        void OpnFilOnEditor_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(Class_Var.DEFAULT_EDITOR))
                {
                    MessageBox.Show("Editor are not exist in directory, verify your config file!", "Error!");
                    return;
                }

                string fileopen = openfile.Fileselect(AppStart, "txt files (*.txt)|*.txt|All files (*.*)|*.*", 2);

                if (!string.IsNullOrEmpty(fileopen))
                    OpenFile_Editor(fileopen);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnFilOnEditor_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpenListLink_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                string fileopen = openfile.Fileselect(AppStart, "txt files (*.txt)|*.txt|All files (*.*)|*.*", 2);

                if (fileopen != string.Empty)
                    Open_Source_Frm(fileopen);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenListLink_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpnGroupFrm_Btn_Click(object sender, EventArgs e)
        {
            mdiFrm = new Mdi_Frm();
            mdiFrm.Show();
        }

        void Memo_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(Path.Combine(AppStart, "memo.txt"));
        }

        void Editor_Btn_Click(object sender, EventArgs e)
        {
            OpenEdit();
        }

        void CyberChef_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(CyberChef_Opt_Txt.Text))
                    if (File.Exists(CyberChef_Opt_Txt.Text))
                        GoBrowser("file:///" + CyberChef_Opt_Txt.Text, 0);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CyberChef_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpenEdit()
        {
            if (File.Exists(Class_Var.DEFAULT_EDITOR))
                Process.Start(Class_Var.DEFAULT_EDITOR);
            else
                MessageBox.Show("Editor are not exist in directory, verify your config file!", "Error!");
        }

        void OpnDirectory_Btn_Click(object sender, EventArgs e)
        {
            Process.Start(AppStart);
        }

        void IndexDir_Btn_Click(object sender, EventArgs e)
        {
            WBrowse.Source = new Uri(@AppStart);
        }

        void InjectScript_Btn_Click(object sender, EventArgs e)
        {
            InjectScript();
        }

        void InjectScript()
        {
            try
            {
                string message, title;
                object ScriptSelect;

                message = "Enter some JavaScript to be executed in the context of this page. Don't use scripts you don't understand!";
                title = "Inject Script";

                ScriptSelect = Interaction.InputBox(message, title);
                string ScriptInject = Convert.ToString(ScriptSelect);

                if (ScriptInject != string.Empty)
                    InjectScript(ScriptInject);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! InjectScript_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        async void InjectScript(string scrinj)
        {
            try
            {
                await WBrowse.ExecuteScriptAsync(scrinj);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this, ex.ToString(), "Execute Script Fails!");
            }
        }

        void OpenScriptEdit_Btn_Click(object sender, EventArgs e)
        {
            if (Class_Var.SCRIPTCREATOR == "off")
            {
                Class_Var.SCRIPTCREATOR = "on";
                scriptCreatorFrm = new ScriptCreator();
                scriptCreatorFrm.Show();
            }
        }

        void RegexCmd_Btn_Click(object sender, EventArgs e)
        {
            RegexInject();
        }

        void RegexInject()
        {
            try
            {
                if (!File.Exists(Path.Combine(AppStart, "sourcepage")))
                {
                    MessageBox.Show("The source of the current WEB page not exist, start [sourcepage] command first!", "No source Page", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Console_Cmd_Txt.Enabled = true;
                    return;
                }

                string message, title;
                object RegexSelect;

                message = "Enter some Regex to be executed in the context of this page.";
                title = "Regular Expression";

                RegexSelect = Interaction.InputBox(message, title);
                string ScriptInject = Convert.ToString(RegexSelect);

                if (ScriptInject != string.Empty)
                {
                    Thread Thr_CMDConsoleExec = new Thread(() => CMD_Console_Exec(4, ScriptInject));
                    Thr_CMDConsoleExec.Start();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! RegexCmd_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void HiglitAddWord_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(HighlitFile);
        }

        void HiglitInject_Btn_Click(object sender, EventArgs e)
        {
            using (var reader = new StreamReader(HighlitFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    ColorWord(values[0], values[1]);
                }
            }
        }

        async void ColorWord(string valword, string valcolor)
        {
            try
            {
                await WBrowse.ExecuteScriptAsync("function highlightWord(n){function t(i){if(i.nodeType===Node.TEXT_NODE){const r=new RegExp(`(${n})`,\"gi\")," +
                    "t=i.parentNode;if(t&&t.nodeName!==\"A\"){const u=i.textContent.replace(r,'<span style=\"color: " + valcolor + "; font-weight: bold;\">$1" +
                    "<\\/span>'),n=document.createElement(\"span\");n.innerHTML=u;t.replaceChild(n,i)}}else i.nodeType===Node.ELEMENT_NODE&&Array.from(i.childNodes)" +
                    ".forEach(t)}t(document.body)}highlightWord(\"" + valword + "\")");
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this, ex.ToString(), "Execute ColorWord Script Fails!");
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

        void FetchDomainInfo_Btn_Click(object sender, EventArgs e)
        {
            deserializeForm = new DeserializeJson_Frm();
            deserializeForm.Show();
        }

        void JavaScriptToggle_Btn_Click(object sender, EventArgs e)
        {
            JavaScript();
        }

        void JavaScript()
        {
            try
            {
                if (WBrowse?.CoreWebView2?.Settings != null)
                {
                    var settings = WBrowse.CoreWebView2.Settings;
                    settings.IsScriptEnabled = !settings.IsScriptEnabled;

                    UpdateButtonState(settings.IsScriptEnabled);
                }
                else
                {
                    MessageBox.Show("WebView2 is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void UpdateButtonState(bool isScriptEnabled)
        {
            if (isScriptEnabled)
            {
                JavaScriptToggle_Btn.Text = "JavaScript Enabled";
                JavaScriptToggle_Btn.ForeColor = Color.Black;
            }
            else
            {
                JavaScriptToggle_Btn.Text = "JavaScript Disabled";
                JavaScriptToggle_Btn.ForeColor = Color.Red;
            }
        }

        void ArchiveDirectory_Btn_Click(object sender, EventArgs e)
        {
            ArchiveData();
        }

        void ArchiveData()
        {
            try
            {
                var ArchiveDir = new List<string>()
                    {
                        DBdirectory,
                        FeedDir,
                        FileDir,
                        Workflow,
                        Scripts,
                        Setirps,
                        MapDir,
                        JsonDir
                    };

                using (StreamWriter addtxt = new StreamWriter(AppStart + "Archive-DB-FILES-FEED.bat"))
                {
                    addtxt.WriteLine("@echo off");
                    addtxt.WriteLine("echo ".PadRight(39, '-'));
                    addtxt.WriteLine("echo          Ostium by ICAZA MEDIA");
                    addtxt.WriteLine("echo ".PadRight(39, '-'));
                    addtxt.WriteLine("@echo.");
                    addtxt.WriteLine("echo Backup: DATABASE - FEED - FILES");
                    addtxt.WriteLine("@echo.");

                    for (int i = 0; i < ArchiveDir.Count; i++)
                    {
                        addtxt.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + ArchiveDir[i].ToString() + " -mx9 -mtc=on");
                    }

                    if (ArchiveAdd_Lst.Items.Count > 0)
                    {
                        for (int i = 0; i < ArchiveAdd_Lst.Items.Count; i++)
                        {
                            addtxt.WriteLine("7za.exe u -tzip " + AppStart + "Archives.zip " + ArchiveAdd_Lst.Items[i].ToString() + " -mx9 -mtc=on");
                        }
                    }

                    addtxt.WriteLine("pause");
                }

                Process.Start(AppStart + "Archive-DB-FILES-FEED.bat");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ArchiveDirectory_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
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

        void OpnEncFrm_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(AppStart, "RW5jcnlwdA", "RW5jcnlwdA.exe")))
            {
                MessageBox.Show("RW5jcnlwdA are not exist in directory, verify your config file!", "Error!");
            }
            else
            {
                Process.Start(Path.Combine(AppStart, "RW5jcnlwdA", "RW5jcnlwdA.exe"));
            }
        }

        void KeepTrackViewer_Btn_Click(object sender, EventArgs e)
        {
            OpenKeepTrack();
        }

        void ConfigSFE_Btn_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(AppStart, "SecureFileExplorer", "config.json")))
            {
                MessageBox.Show("SecureFileExplorer is not install, go to Discord channel Ostium for fix and help. is not started!",
                    "SecureFileExplorer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            OpenFile_Editor(Path.Combine(AppStart, "SecureFileExplorer", "config.json"));
        }

        void StartSFE_Btn_Click(object sender, EventArgs e)
        {
            try 
            {
                if (!File.Exists(Path.Combine(AppStart, "SecureFileExplorer", "SecureFileExplorer.exe")))
                {
                    MessageBox.Show("SecureFileExplorer is not install, go to Discord channel Ostium for fix and help. is not started!", 
                        "SecureFileExplorer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                VerifyProcess("SecureFileExplorer");

                if (ProcessLoad)
                {
                    MessageBox.Show("SecureFileExplorer is already running!", "SecureFileExplorre", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = Path.Combine(AppStart, "SecureFileExplorer", "SecureFileExplorer.exe");
                    proc.StartInfo.Arguments = string.Empty;
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.WorkingDirectory = Path.Combine(AppStart, "SecureFileExplorer");
                    proc.StartInfo.RedirectStandardOutput = false;
                    proc.Start();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! StartSFE_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void LocalhostSFE_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                VerifyProcess("SecureFileExplorer");

                if (!ProcessLoad)
                {
                    MessageBox.Show("SecureFileExplorer is not started!", "SecureFileExplorer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                string json = File.ReadAllText(Path.Combine(AppStart, "SecureFileExplorer", "config.json"));
                ConfigSFE config = System.Text.Json.JsonSerializer.Deserialize<ConfigSFE>(json, options);

                GoBrowser($"http://localhost:{config.Port}", 0);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LocalhostSFE_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void LogPathSFE_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Path.Combine(AppStart, "SecureFileExplorer", "logs"));
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LogPathSFE_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }            
        }

        void OpenKeepTrack()
        {
            if (File.Exists(Path.Combine(Keeptrack, "Keeptrack.html")))
            {
                GoBrowser("file:///" + Keeptrack + "Keeptrack.html", 0);
            }
        }

        void TreeExport_Btn_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem objBtn;
            objBtn = (sender as ToolStripMenuItem);

            try
            {
                MessageBox.Show("1- select the directory whose tree you want to create.\r\n2- select the directory where " +
                    "the tree file of the chosen directory will be saved.", "OOB", MessageBoxButtons.OK, MessageBoxIcon.None);

                string dirselect = selectdir.Dirselect();
                CreateNameAleat();

                if (!string.IsNullOrEmpty(dirselect))
                {
                    string dirFilesave = selectdir.Dirselect();
                    if (!string.IsNullOrEmpty(dirFilesave))
                    {
                        switch (objBtn.Text)
                        {
                            case "TXT export":
                                exporter.ExportDirectoryTree(dirselect, Path.Combine(dirFilesave, $"Treeview_{Una}.txt"));
                                break;
                            case "XML export":
                                exporter.ExportDirectoryTreeAsXml(dirselect, Path.Combine(dirFilesave, $"Treeview_{Una}.xml"));
                                break;
                            case "JSON export":
                                exporter.ExportDirectoryTreeAsJson(dirselect, Path.Combine(dirFilesave, $"Treeview_{Una}.json"));
                                break;
                        }
                        MessageBox.Show("Operation completed!", "OOB", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! TreeExport_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        #endregion

        #region Tools_Tab_1

        void OpnFileCategory_Btn_Click(object sender, EventArgs e)
        {
            if (CategorieFeed_Cbx.Text != string.Empty)
            {
                OpenFile_Editor(Path.Combine(FeedDir, CategorieFeed_Cbx.Text));
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
            Console.Beep(1500, 400);
        }

        void TraductPageFeed_Btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Class_Var.URL_TRAD_WEBPAGE))
            {
                try
                {
                    if (lstUrlDfltCnf.Count > 0)
                        Class_Var.URL_TRAD_WEBPAGE = lstUrlDfltCnf[2].ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                        "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (ArgumentException ex)
                {
                    senderror.ErrorLog("Error! lstUrlDfltCnf TraductPageFeed_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
                }
            }

            string formatURI = Regex.Replace(Class_Var.URL_TRAD_WEBPAGE, "replace_query", WBrowsefeed.Source.AbsoluteUri);
            WBrowsefeed.Source = new Uri(@formatURI);
        }

        void JavaScriptFeed_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (WBrowsefeed?.CoreWebView2?.Settings != null)
                {
                    var settings = WBrowsefeed.CoreWebView2.Settings;
                    settings.IsScriptEnabled = !settings.IsScriptEnabled;

                    UpdateButtonFeedState(settings.IsScriptEnabled);
                }
                else
                {
                    MessageBox.Show("WebView2 is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void UpdateButtonFeedState(bool isScriptEnabled)
        {
            if (isScriptEnabled)
            {
                JavaScriptFeed_Btn.Text = "JS Enabled";
                JavaScriptFeed_Btn.ForeColor = Color.White;
                JavaDisableFeed_Lbl.Visible = false;
            }
            else
            {
                JavaScriptFeed_Btn.Text = "JS Disabled";
                JavaScriptFeed_Btn.ForeColor = Color.Red;
                JavaDisableFeed_Lbl.Visible = true;
            }
        }

        #endregion

        #region Tools_Tab_3

        void NewProject_Tls_Click(object sender, EventArgs e)
        {
            Reset();
        }

        void DeleteProject_Tls_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameProjectwf_Txt.Text))
                return;

            string message = $"Do you want to delete the project? {NameProjectwf_Txt.Text}";
            string caption = "Delete Projet";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (File.Exists(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml")))
                    File.Delete(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"));

                if (File.Exists(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".ost")))
                    File.Delete(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".ost"));

                Reset();

                loadfiledir.LoadFileDirectory(Workflow, "xml", "lst", ProjectOpn_Lst);
            }
        }

        void SVGviewer_Tls_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(SVGviewerdir, "SVGviewer.exe")))
            {
                MessageBox.Show("SVGviewer are not exist in directory, go to Discord channel Ostium for fix and help.", "Error!", 
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                string fileopen = openfile.Fileselect(AppStart, "svg files (*.svg)|*.svg", 2);
                if (!string.IsNullOrEmpty(fileopen))
                {
                    using (Process proc = new Process())
                    {
                        proc.StartInfo.FileName = Path.Combine(SVGviewerdir, "SVGviewer.exe");
                        proc.StartInfo.Arguments = fileopen;
                        proc.StartInfo.UseShellExecute = true;
                        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        proc.Start();
                        proc.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! SVGviewer_Tls_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void ViewXml_Tls_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameProjectwf_Txt.Text))
                return;

            if (File.Exists(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml")))
            {
                GoBrowser("file:///" + Workflow + NameProjectwf_Txt.Text + ".xml", 0);
                CtrlTabBrowsx();
                Control_Tab.SelectedIndex = 0;
            }
        }

        void EditXml_Tls_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameProjectwf_Txt.Text))
                return;

            if (File.Exists(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml")))
                OpenFile_Editor(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"));
        }

        void ExportXml_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(NameProjectwf_Txt.Text))
                    return;

                string dirselect = selectdir.Dirselect();
                if (!string.IsNullOrEmpty(dirselect))
                {
                    if (File.Exists(Path.Combine(dirselect, NameProjectwf_Txt.Text + ".xml")))
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
                    File.Copy(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"), Path.Combine(dirselect, NameProjectwf_Txt.Text + ".xml"), true);
                    Console.Beep(1000, 400);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ExportXml_Tls_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void ExportJson_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(NameProjectwf_Txt.Text))
                    return;

                string dirselect = selectdir.Dirselect();

                if (!string.IsNullOrEmpty(dirselect))
                {
                    if (File.Exists(Path.Combine(dirselect, NameProjectwf_Txt.Text + ".json")))
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
                    ConvertJson(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"), Path.Combine(dirselect, NameProjectwf_Txt.Text + ".json"), 0);
                    Console.Beep(1000, 400);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ExportJson_Tls_Click: ", ex.ToString(), "Main_Frm", AppStart);
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
            if (string.IsNullOrEmpty(NameProjectwf_Txt.Text))
                return;

            if (!File.Exists(Path.Combine(DiagramDir, "plantuml.jar")))
            {
                MessageBox.Show("PlantUML is not install, go to Discord channel Ostium for fix and help.",
                    "PlantUML", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (File.Exists(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".txt")))
                File.Delete(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".txt"));

            if (File.Exists(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".svg")))
                File.Delete(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".svg"));

            MessageBox.Show(MessageStartDiagram);
            Timo.Enabled = true;

            ConvertJson(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"), Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".txt"), 1);

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
                if (string.IsNullOrEmpty(ThemeDiag))
                    ThemeDiag = "cloudscape-design";

                var xmlFile = xmlFileConvert;
                var doc = new XmlDocument();
                doc.Load(xmlFile);

                string json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);

                json = Regex.Replace(json, @"/\*.+?\*/", string.Empty); // Delete comments

                using (StreamWriter fc = new StreamWriter(dirFileConvert))
                {
                    if (value == 1)
                    {
                        fc.WriteLine("@startjson");
                        fc.WriteLine("!theme " + ThemeDiag);
                    }

                    fc.WriteLine(json);

                    if (value == 1)
                        fc.Write("@endjson");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ConvertJson: ", ex.ToString(), "Main_Frm", AppStart);
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
                string limitsize = string.Empty;
                string argumentsIs = string.Empty;

                if (Limitsize_Chk.Checked)
                    limitsize = "-DPLANTUML_LIMIT_SIZE=8192";

                if (value == 0)
                    argumentsIs = "java " + limitsize + " -jar plantuml.jar " + DiagramDir + fileselect + " -tsvg " + CharsetPlant_Txt.Text;
                else if (value == 1)
                    argumentsIs = "java " + limitsize + " -jar plantuml.jar " + fileselect + " -tsvg " + CharsetPlant_Txt.Text;

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
                senderror.ErrorLog("Error! CreateDiagram_Thrd: ", ex.ToString(), "Main_Frm", AppStart);
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
            if (string.IsNullOrEmpty(NameProjectwf_Txt.Text))
                return;

            if (!File.Exists(Path.Combine(DiagramDir, "plantuml.jar")))
            {
                MessageBox.Show("Sorry, PlantUML is not install, go to Discord channel Ostium for fix and help.");
                return;
            }

            MessageBox.Show(MessageStartDiagram);
            Timo.Enabled = true;

            ThemeDiag = ThemDiag_Cbx.Text;
            if (string.IsNullOrEmpty(ThemeDiag))
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
            if (string.IsNullOrEmpty(NameProjectwf_Txt.Text))
                return;

            if (!File.Exists(Path.Combine(DiagramDir, "plantuml.jar")))
            {
                MessageBox.Show("PlantUML is not install, go to Discord channel Ostium for fix and help.",
                    "PlantUML", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            MessageBox.Show(MessageStartDiagram);
            Timo.Enabled = true;

            ThemeDiag = ThemDiag_Cbx.Text;
            if (string.IsNullOrEmpty(ThemeDiag))
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
                if (!File.Exists(Path.Combine(DiagramDir, "plantuml.jar")))
                {
                    MessageBox.Show("PlantUML is not install, go to Discord channel Ostium for fix and help.",
                        "PlantUML", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (File.Exists(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".txt")))
                    File.Delete(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".txt"));

                if (File.Exists(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".svg")))
                    File.Delete(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".svg"));

                string element;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"));

                using (StreamWriter fc = File.AppendText(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".txt")))
                {
                    fc.WriteLine("@startmindmap");
                    fc.WriteLine("skinparam titleBorderRoundCorner 15");
                    fc.WriteLine("skinparam titleBorderThickness 2");
                    fc.WriteLine("skinparam titleBorderColor red");
                    fc.WriteLine("skinparam titleBackgroundColor Aqua-CadetBlue");
                    fc.WriteLine("!theme " + ThemeDiag);
                    fc.WriteLine("footer https://veydunet.com/ostium");
                    fc.WriteLine("caption Ostium Osint Browser");
                    fc.WriteLine("title " + NameProjectwf_Txt.Text);

                    if (value == 0)
                        fc.WriteLine("+ " + NameProjectwf_Txt.Text);
                    else if (value == 1)
                        fc.WriteLine("* " + NameProjectwf_Txt.Text);
                }

                for (int i = 0; i < WorkflowItem_Lst.Items.Count; i++)
                {
                    if (WorkflowItem_Lst.Items[i].ToString() != string.Empty)
                    {
                        element = WorkflowItem_Lst.Items[i].ToString();
                        element += "_" + element;

                        using (StreamWriter fc = File.AppendText(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".txt")))
                        {
                            if (value == 0)
                                fc.WriteLine("++ " + WorkflowItem_Lst.Items[i].ToString());
                            else if (value == 1)
                                fc.WriteLine("** " + WorkflowItem_Lst.Items[i].ToString());
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

                            using (StreamWriter fc = File.AppendText(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".txt")))
                            {
                                if (value == 0)
                                {
                                    fc.WriteLine("+++ " + strtext);
                                }
                                else if (value == 1)
                                {
                                    fc.WriteLine("***:" + strnote);
                                    fc.WriteLine("<code>");
                                    fc.WriteLine(strtext);
                                    fc.WriteLine(strurl);
                                    fc.WriteLine("-----------");
                                    fc.WriteLine(string.Format("{0} {1} {2}", strauth, strdate, strtime));
                                    fc.WriteLine("</code>");
                                    fc.WriteLine(";");
                                }
                            }
                        }
                    }
                }

                using (StreamWriter fc = File.AppendText(Path.Combine(DiagramDir, NameProjectwf_Txt.Text + ".txt")))
                {
                    fc.Write("@endmindmap");
                }

                FileDiag = NameProjectwf_Txt.Text + ".svg";
                Commut = 0;

                Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd(NameProjectwf_Txt.Text + ".txt", 0));
                CreateDiagram.Start();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateDiagramMinMapFile: ", ex.ToString(), "Main_Frm", AppStart);
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
                if (!File.Exists(Path.Combine(DiagramDir, "plantuml.jar")))
                {
                    MessageBox.Show("PlantUML is not install, go to Discord channel Ostium for fix and help.",
                        "PlantUML", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                TmpFile_Txt.Text = string.Empty;

                ThemeDiag = ThemDiag_Cbx.Text;
                if (string.IsNullOrEmpty(ThemeDiag))
                    ThemeDiag = "cloudscape-design";

                string fileopen = openfile.Fileselect(AppStart, "json files (*.json)|*.json", 2);

                if (!string.IsNullOrEmpty(fileopen))
                {
                    MessageBox.Show(MessageStartDiagram);
                    Timo.Enabled = true;

                    if (File.Exists(Path.Combine(DiagramDir, "temp_file.txt")))
                        File.Delete(Path.Combine(DiagramDir, "temp_file.txt"));

                    if (File.Exists(Path.Combine(DiagramDir, "temp_file.svg")))
                        File.Delete(Path.Combine(DiagramDir, "temp_file.svg"));

                    using (StreamReader sr = new StreamReader(fileopen))
                    {
                        TmpFile_Txt.Text = sr.ReadToEnd();
                    }

                    using (StreamWriter fc = new StreamWriter(Path.Combine(DiagramDir, "temp_file.txt")))
                    {
                        fc.WriteLine("@startjson");
                        fc.WriteLine("!theme " + ThemeDiag);
                        fc.WriteLine(TmpFile_Txt.Text);
                        fc.Write("@endjson");
                    }

                    FileDiag = "temp_file.svg";
                    Commut = 0;

                    Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd("temp_file.txt", 0));
                    CreateDiagram.Start();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnJsonFile_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
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
                if (!File.Exists(Path.Combine(DiagramDir, "plantuml.jar")))
                {
                    MessageBox.Show("PlantUML is not install, go to Discord channel Ostium for fix and help.",
                        "PlantUML", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string fileopen = openfile.Fileselect(AppStart, "xml files (*.xml)|*.xml", 2);

                if (!string.IsNullOrEmpty(fileopen))
                {
                    MessageBox.Show(MessageStartDiagram);
                    Timo.Enabled = true;

                    if (File.Exists(Path.Combine(DiagramDir, "temp_file.txt")))
                        File.Delete(Path.Combine(DiagramDir, "temp_file.txt"));

                    if (File.Exists(Path.Combine(DiagramDir, "temp_file.svg")))
                        File.Delete(Path.Combine(DiagramDir, "temp_file.svg"));

                    ConvertJson(fileopen, Path.Combine(DiagramDir, "temp_file.txt"), 1);

                    FileDiag = "temp_file.svg";
                    Commut = 0;

                    Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd("temp_file.txt", 0));
                    CreateDiagram.Start();
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnXMLFile_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
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
            if (!File.Exists(Path.Combine(DiagramDir, "plantuml.jar")))
            {
                MessageBox.Show("PlantUML is not install, go to Discord channel Ostium for fix and help.",
                    "PlantUML", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string fileopen = openfile.Fileselect(AppStart, "txt files (*.txt)|*.txt", 2);

            if (!string.IsNullOrEmpty(fileopen))
            {
                MessageBox.Show(MessageStartDiagram);
                Timo.Enabled = true;

                string strName = Path.GetFileNameWithoutExtension(fileopen);
                string strDir = Path.GetDirectoryName(fileopen);

                FileDiag = Path.Combine(strDir, strName + ".svg");
                Commut = 1;

                Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd(fileopen, 1));
                CreateDiagram.Start();
            }
        }

        void PlantUmlVersion_Mnu_Click(object sender, EventArgs e) // version or license
        {
            string Btn = (sender as ToolStripMenuItem).Text;

            if (!File.Exists(Path.Combine(DiagramDir, "plantuml.jar")))
            {
                MessageBox.Show("PlantUML is not install, go to Discord channel Ostium for fix and help.",
                    "PlantUML", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!File.Exists(Path.Combine(DiagramDir, Btn + "_plantuml.txt")))
            {
                MessageBox.Show(Btn + "_plantuml.txt file is not exist, go to Discord channel Ostium for fix and help.");
                return;
            }

            MessageBox.Show(MessageStartDiagram);
            Timo.Enabled = true;

            FileDiag = Btn + "_plantuml.svg";
            Commut = 0;

            Thread CreateDiagram = new Thread(() => CreateDiagram_Thrd(Btn + "_plantuml.txt", 0));
            CreateDiagram.Start();
        }

        void PlantUmlLUpdate_Mnu_Click(object sender, EventArgs e)
        {
            GoBrowser("https://plantuml.com/download", 0);
            CtrlTabBrowsx();
            Control_Tab.SelectedIndex = 0;
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
                if (FileDiag == string.Empty)
                {
                    MessageBox.Show("SVG file not exist!");
                    return;
                }

                CreateNameAleat();
                string nameSVG = Una + "_" + FileDiag;
                string nameSVGb = Una;

                string dirselect = selectdir.Dirselect();
                if (string.IsNullOrEmpty(dirselect))
                    return;

                if (Commut == 0)
                {
                    File.Copy(Path.Combine(DiagramDir, FileDiag), Path.Combine(dirselect, nameSVG));
                    MessageBox.Show($"File [{nameSVG}] export.");
                }
                else if (Commut == 1)
                {
                    File.Copy(FileDiag, dirselect + @"\" + nameSVGb + ".svg");
                    MessageBox.Show($"File [{nameSVG}.svg] export.");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ExportDiag_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void ThemDiag_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tools_TAB_3.Focus();
        }

        void OpnSprites_Btn_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(AppStart, "setirps.exe")))
            {
                MessageBox.Show("Setirps is missing!", "Missing editor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Process.Start(Path.Combine(AppStart, "setirps.exe"));
        }

        #endregion

        void Control_Tab_Click(object sender, EventArgs e)
        {
            switch (Control_Tab.SelectedIndex)
            {
                case 0:
                    CtrlTabBrowsx();
                    break;
                case 1:
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
                    TableVal_Lbl.Visible = false;
                    RecordsCount_Lbl.Visible = false;
                    LatTCurrent_Lbl.Visible = false;
                    Separator.Visible = false;
                    LonGtCurrent_Lbl.Visible = false;
                    ProjectMapOpn_Lbl.Visible = false;
                    TtsButton_Sts.Visible = false;
                    FileOpnJson_Lbl.Visible = false;

                    if (JavaScriptFeed_Btn.Text == "JS Disabled")
                        JavaDisableFeed_Lbl.Visible = true;

                    JavaDisable_Lbl.Visible = false;

                    NewCategory_Txt.ForeColor = Color.DimGray;
                    NewCategory_Txt.Text = "new category";
                    NewFeed_Txt.ForeColor = Color.DimGray;
                    NewFeed_Txt.Text = "new feed";
                    break;
                case 2:
                    Tools_TAB_0.Visible = false;
                    Tools_TAB_1.Visible = false;
                    Tools_TAB_3.Visible = false;
                    Tools_TAB_4.Visible = false;
                    URLtxt_txt.Text = string.Empty;
                    Text = "DataBase Url";
                    TableOpn_Lbl.Visible = false;
                    CountFeed_Lbl.Visible = false;
                    JavaDisable_Lbl.Visible = false;
                    JavaDisableFeed_Lbl.Visible = false;
                    DBSelectOpen_Lbl.Visible = true;
                    TableCount_Lbl.Visible = true;
                    TableOpen_Lbl.Visible = true;
                    TableVal_Lbl.Visible = true;
                    RecordsCount_Lbl.Visible = true;
                    LatTCurrent_Lbl.Visible = false;
                    Separator.Visible = false;
                    LonGtCurrent_Lbl.Visible = false;
                    ProjectMapOpn_Lbl.Visible = false;
                    TtsButton_Sts.Visible = false;
                    FileOpnJson_Lbl.Visible = false;

                    ValueChange_Txt.ForeColor = Color.DimGray;
                    ValueChange_Txt.Text = "update URL and Name here";

                    loadfiledir.LoadFileDirectory(DBdirectory, "*", "lst", DataBaze_Lst);
                    break;
                case 3:
                    Tools_TAB_0.Visible = false;
                    Tools_TAB_1.Visible = false;
                    Tools_TAB_3.Visible = true;
                    Tools_TAB_4.Visible = false;
                    URLtxt_txt.Text = string.Empty;
                    Text = "Workflow";
                    TableOpn_Lbl.Visible = false;
                    CountFeed_Lbl.Visible = false;
                    JavaDisable_Lbl.Visible = false;
                    JavaDisableFeed_Lbl.Visible = false;
                    DBSelectOpen_Lbl.Visible = false;
                    TableCount_Lbl.Visible = false;
                    TableOpen_Lbl.Visible = false;
                    TableVal_Lbl.Visible = false;
                    RecordsCount_Lbl.Visible = false;
                    LatTCurrent_Lbl.Visible = false;
                    Separator.Visible = false;
                    LonGtCurrent_Lbl.Visible = false;
                    ProjectMapOpn_Lbl.Visible = false;
                    TtsButton_Sts.Visible = false;
                    FileOpnJson_Lbl.Visible = false;
                    break;
                case 4:
                    Tools_TAB_0.Visible = false;
                    Tools_TAB_1.Visible = false;
                    Tools_TAB_3.Visible = false;
                    Tools_TAB_4.Visible = true;
                    URLtxt_txt.Text = string.Empty;
                    Text = "Map";
                    TableOpn_Lbl.Visible = false;
                    CountFeed_Lbl.Visible = false;
                    JavaDisable_Lbl.Visible = false;
                    JavaDisableFeed_Lbl.Visible = false;
                    DBSelectOpen_Lbl.Visible = false;
                    TableCount_Lbl.Visible = false;
                    TableOpen_Lbl.Visible = false;
                    TableVal_Lbl.Visible = false;
                    RecordsCount_Lbl.Visible = false;
                    LatTCurrent_Lbl.Visible = true;
                    Separator.Visible = true;
                    LonGtCurrent_Lbl.Visible = true;
                    ProjectMapOpn_Lbl.Visible = true;
                    TtsButton_Sts.Visible = false;
                    FileOpnJson_Lbl.Visible = false;

                    if (VerifMapOpn == "off")
                    {
                        Mkmarker = GMarkerGoogleType.red_dot;
                        OpenMaps("Paris", 12); // Adresse, Provider
                        PointLoc_Lst.Items.Clear();
                        loadfiledir.LoadFileDirectory(MapDir, "xml", "lst", PointLoc_Lst);
                    }
                    break;
                case 5:
                    Tools_TAB_0.Visible = false;
                    Tools_TAB_1.Visible = false;
                    Tools_TAB_3.Visible = false;
                    Tools_TAB_4.Visible = false;
                    URLtxt_txt.Text = string.Empty;
                    Text = "Json";
                    TableOpn_Lbl.Visible = false;
                    CountFeed_Lbl.Visible = false;
                    JavaDisable_Lbl.Visible = false;
                    JavaDisableFeed_Lbl.Visible = false;
                    DBSelectOpen_Lbl.Visible = false;
                    TableCount_Lbl.Visible = false;
                    TableOpen_Lbl.Visible = false;
                    TableVal_Lbl.Visible = false;
                    RecordsCount_Lbl.Visible = false;
                    LatTCurrent_Lbl.Visible = false;
                    Separator.Visible = false;
                    LonGtCurrent_Lbl.Visible = false;
                    ProjectMapOpn_Lbl.Visible = false;
                    TtsButton_Sts.Visible = false;
                    FileOpnJson_Lbl.Visible = true;
                    break;
                case 6:
                    Tools_TAB_0.Visible = false;
                    Tools_TAB_1.Visible = false;
                    Tools_TAB_3.Visible = false;
                    Tools_TAB_4.Visible = false;
                    URLtxt_txt.Text = string.Empty;
                    Text = "Options";
                    TableOpn_Lbl.Visible = false;
                    CountFeed_Lbl.Visible = false;
                    JavaDisable_Lbl.Visible = false;
                    JavaDisableFeed_Lbl.Visible = false;
                    DBSelectOpen_Lbl.Visible = false;
                    TableCount_Lbl.Visible = false;
                    TableOpen_Lbl.Visible = false;
                    TableVal_Lbl.Visible = false;
                    RecordsCount_Lbl.Visible = false;
                    LatTCurrent_Lbl.Visible = false;
                    Separator.Visible = false;
                    LonGtCurrent_Lbl.Visible = false;
                    ProjectMapOpn_Lbl.Visible = false;
                    TtsButton_Sts.Visible = false;
                    FileOpnJson_Lbl.Visible = false;

                    UpdateDirectorySize(AppStart, OstiumDir_Lbl);
                    UpdateDirectorySize(Plugins, AddOnDir_Lbl);
                    UpdateDirectorySize(DBdirectory, DatabseDir_Lbl);
                    UpdateDirectorySize(FeedDir, FeedDir_Lbl);
                    UpdateDirectorySize(Scripts, ScriptDir_Lbl);
                    UpdateDirectorySize(Workflow, WorkFlowDir_Lbl);
                    UpdateDirectorySize(WorkflowModel, WorkFlowModelDir_Lbl);
                    UpdateDirectorySize(Pictures, PictureDir_Lbl);
                    UpdateDirectorySize(WebView2Dir, WebView2Dir_Lbl);
                    UpdateDirectorySize(DiagramDir, DiagramDir_Lbl);
                    UpdateDirectorySize(Setirps, SpritesDir_Lbl);
                    UpdateDirectorySize(BkmkltDir, BkmkltDir_Lbl);
                    UpdateDirectorySize(MapDir, MapDir_Lbl);
                    UpdateDirectorySize(JsonDir, JsonDir_Lbl);
                    UpdateDirectorySize(Keeptrack, KeepTrackDir_Lbl);
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
            try
            {
                URLtxt_txt.Text = WBrowse.Source.AbsoluteUri;
            }
            catch { }
            TableOpn_Lbl.Visible = true;
            CountFeed_Lbl.Visible = false;
            DBSelectOpen_Lbl.Visible = false;
            TableCount_Lbl.Visible = false;
            TableOpen_Lbl.Visible = false;
            TableVal_Lbl.Visible = false;
            RecordsCount_Lbl.Visible = false;
            LatTCurrent_Lbl.Visible = false;
            Separator.Visible = false;
            LonGtCurrent_Lbl.Visible = false;
            ProjectMapOpn_Lbl.Visible = false;
            TtsButton_Sts.Visible = true;
            FileOpnJson_Lbl.Visible = false;

            if (JavaScriptToggle_Btn.Text == "Javascript Disable")
                JavaDisable_Lbl.Visible = true;

            JavaDisableFeed_Lbl.Visible = false;
        }
        ///
        /// <summary>
        /// URL construction from a list loaded with the URL construction file selected and created a temporary file
        /// to open all URLs in the multi-window form
        /// </summary>
        /// <param name="replace_query">Replacement value with the searched nickname/word</param>
        /// 
        void URL_Builder(string searchQuery)
        {
            try
            {
                URLbrowse_Cbx.Items.Clear();

                string A = Path.Combine(AppStart, "filesdir", "grp-frm", "Temp-Url-Construct.txt");

                HashSet<string> uniqueUrls = new HashSet<string>();

                foreach (var item in ConstructURL_Lst.Items)
                {
                    if (!string.IsNullOrEmpty(item?.ToString()) && !string.IsNullOrEmpty(searchQuery))
                    {
                        string formattedUrl = item.ToString().Replace("replace_query", searchQuery);
                        if (uniqueUrls.Add(formattedUrl))
                        {
                            URLbrowse_Cbx.Items.Add(formattedUrl);
                        }
                    }
                }

                using (StreamWriter SW = new StreamWriter(A, false))
                {
                    foreach (string itm in URLbrowse_Cbx.Items)
                        SW.WriteLine(itm);
                }

                if (URLbrowse_Cbx.Items.Count != 0)
                    URLbrowse_Cbx.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Construct_URL: ", ex.ToString(), "Main_Frm", AppStart);
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
                if (WBrowse.Source?.AbsoluteUri.Contains("file:///") == true || WBrowse.Source?.AbsoluteUri.Contains("edge://") == true)
                    return;

                if (string.IsNullOrWhiteSpace(Class_Var.URL_USER_AGENT_SRC_PAGE))
                {
                    try
                    {
                        if (lstUrlDfltCnf.Count > 0)
                            Class_Var.URL_USER_AGENT_SRC_PAGE = lstUrlDfltCnf[5].ToString();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                            "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    catch (ArgumentException ex)
                    {
                        senderror.ErrorLog("Error! lstUrlDfltCnf Download_Source_Page: ", ex.ToString(), "Main_Frm", AppStart);
                    }
                }

                client.DefaultRequestHeaders.UserAgent.ParseAdd(Class_Var.URL_USER_AGENT_SRC_PAGE);

                if (WBrowse?.Source == null)
                {
                    return;
                }

                HttpResponseMessage response = await client.GetAsync(WBrowse.Source.AbsoluteUri);

                if (!response.IsSuccessStatusCode)
                {
                    return;
                }

                byte[] contentBytes = await response.Content.ReadAsByteArrayAsync();

                Encoding encoding = GetEncodingFromResponse(response) ?? Encoding.UTF8;

                string pageContents = encoding.GetString(contentBytes);

                File_Write(Path.Combine(AppStart, "sourcepage"), pageContents);
                Console.Beep(300, 200);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Download_Source_Page: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        Encoding GetEncodingFromResponse(HttpResponseMessage response)
        {
            try
            {
                var charset = response.Content.Headers.ContentType?.CharSet;
                if (string.IsNullOrEmpty(charset))
                    return null;

                charset = charset.Replace("\"", "").Trim();

                if (charset.Equals("utf8", StringComparison.OrdinalIgnoreCase))
                    charset = "utf-8";

                return Encoding.GetEncoding(charset);
            }
            catch
            {
                return null;
            }
        }

        void CreateNameAleat()
        {
            Una = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + "_" + Guid.NewGuid().ToString("N");
        }

        #region Prompt_

        void Console_Cmd_Txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Console_Cmd_Txt.SelectionStart < 2 && e.KeyChar != (char)Keys.Enter)
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcessCommand();
            }
        }

        void Console_Cmd_Txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                NavigateHistory(e.KeyCode);
            }

            if ((e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete) &&
                Console_Cmd_Txt.SelectionStart < 2)
            {
                e.Handled = true;
            }
        }

        void ProcessCommand()
        {
            string input = Console_Cmd_Txt.Text.Trim();

            if (input.Length > 2)
            {
                string command = input.Substring(2);

                CMD_Console(command);
                _commandHistory.Add(command);
                _historyIndex = _commandHistory.Count;
            }

            Console_Cmd_Txt.Text = "> ";
            Console_Cmd_Txt.Select(Console_Cmd_Txt.Text.Length, 0);
        }

        void NavigateHistory(Keys key)
        {
            if (_commandHistory.Count == 0) return;

            if (key == Keys.Up && _historyIndex > 0)
            {
                _historyIndex--;
            }
            else if (key == Keys.Down && _historyIndex < _commandHistory.Count - 1)
            {
                _historyIndex++;
            }

            if (_historyIndex >= 0 && _historyIndex < _commandHistory.Count)
            {
                Console_Cmd_Txt.Text = $"> {_commandHistory[_historyIndex]}";
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
            string regxCmd = string.Empty;

            string s = Cmd;
            s = s.Replace(">", string.Empty); s = s.Replace(" ", string.Empty);

            switch (s)
            {
                case "version":
                    MessageBox.Show(SoftVersion);
                    break;
                case "sourcepage":
                    Download_Source_Page();
                    break;
                case "links":
                    cmdSwitch = 0;
                    yn = 1;
                    break;
                case "word":
                    cmdSwitch = 1;
                    yn = 1;
                    break;
                case "wordwd":
                    cmdSwitch = 3;
                    yn = 1;
                    break;
                case "help":
                    Open_Doc_Frm(Path.Combine(FileDir, "cmdc.txt"));
                    break;
                case "textlink":
                    cmdSwitch = 2;
                    yn = 1;
                    break;
                case "config":
                    OpenFile_Editor(AppStart + "config.xml");
                    break;
                case "id":
                    var browserInfo = WBrowse.CoreWebView2.BrowserProcessId;
                    MessageBox.Show(this, "Browser ID: " + browserInfo.ToString(), "Process ID");
                    break;
                case "gdork":
                    GoogleDork();
                    break;
                case "capture":
                    WebpageCapture();
                    break;
                case "htmltext":
                    HtmlTextFrm = new HtmlText_Frm();
                    HtmlTextFrm.Show();
                    break;
                case "multi":
                    mdiFrm = new Mdi_Frm();
                    mdiFrm.Show();
                    break;
                case "cookie":
                    CookieLoad();
                    break;
                case "addcookie":
                    Cookie_Pnl.Visible = !Cookie_Pnl.Visible;
                    break;
                case "editor":
                    OpenEdit();
                    break;
                case "index":
                    WBrowse.Source = new Uri(@AppStart);
                    break;
                case "dns":
                    deserializeForm = new DeserializeJson_Frm();
                    deserializeForm.Show();
                    break;
                case "javascript":
                    JavaScript();
                    break;
                case "bookmark":
                    BookMarklet();
                    break;
                case "inject":
                    InjectScript();
                    break;
                case "regex":
                    RegexInject();
                    break;
                case "backup":
                    ArchiveData();
                    break;
                case "encrypt":
                    if (!File.Exists(Path.Combine(AppStart, "RW5jcnlwdA", "RW5jcnlwdA.exe")))
                    {
                        MessageBox.Show("RW5jcnlwdA are not exist in directory, verify your config file!", "Error!");
                    }
                    else
                    {
                        Process.Start(Path.Combine(AppStart, "RW5jcnlwdA", "RW5jcnlwdA.exe"));
                    }
                    break;
                case "track":
                    OpenKeepTrack();
                    break;
                case "history-clear":
                    ClearData(0);
                    break;
                case "useragent":
                    ChangeUserAgent();
                    break;
                case "googlebot":
                    GoogleBot();
                    break;
                case "exit":
                    Console_Cmd_Txt.Visible = !Console_Cmd_Txt.Visible;
                    break;
                default:
                    MessageBox.Show("Command not recognized! Type help for more information.", "Error!");
                    return;
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
            if (!File.Exists(Path.Combine(AppStart, "sourcepage")))
            {
                MessageBox.Show("The source of the current WEB page not exist, start [sourcepage] command first!", "No source Page", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Console_Cmd_Txt.Enabled = true;
                return;
            }

            StreamReader sr = new StreamReader(Path.Combine(AppStart, "sourcepage"));
            Invoke(new Action<string>(SRCpageAdd), "listclear");
            string line;

            try
            {
                switch (cmdSwitch)
                {
                    case 0: // links
                        Source_Page_Lst.Sorted = false;

                        while ((line = sr.ReadLine()) != null)
                        {
                            foreach (Match match in Regex.Matches(sr.ReadToEnd(), @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*"))
                            {
                                Invoke(new Action<string>(SRCpageAdd), match.Value);
                            }
                        }
                        break;
                    case 1: // word
                        Source_Page_Lst.Sorted = true;

                        while ((line = sr.ReadLine()) != null)
                        {
                            foreach (Match match in Regex.Matches(sr.ReadToEnd(), @"\b[a-zA-ZÀ-ž]\w+"))
                            {
                                Invoke(new Action<string>(SRCpageAdd), match.Value);
                            }
                        }
                        break;
                    case 2: // text link
                        Source_Page_Lst.Sorted = false;

                        while ((line = sr.ReadLine()) != null)
                        {
                            string ex = @"<\s*a[^>]*>(?<valeur>([^<]*))</a>";
                            Regex regex = new Regex(ex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            MatchCollection resultats = regex.Matches(sr.ReadToEnd());
                            foreach (Match resultat in resultats)
                            {
                                Invoke(new Action<string>(SRCpageAdd), resultat.Groups["valeur"].Value);
                            }
                        }
                        break;
                    case 3: // word without duplicate
                        Source_Page_Lst.Sorted = true;

                        HashSet<string> Words = new HashSet<string>();

                        while ((line = sr.ReadLine()) != null)
                        {
                            foreach (Match match in Regex.Matches(line, @"\b[\p{L}']+\b"))
                            {
                                string word = match.Value;
                                if (Words.Add(word))
                                {
                                    Invoke(new Action<string>(SRCpageAdd), word);
                                }
                            }
                        }
                        break;
                    case 4: // regex
                        Source_Page_Lst.Sorted = true;

                        foreach (Match match in Regex.Matches(sr.ReadToEnd(), regxCmd))
                        {
                            Invoke(new Action<string>(SRCpageAdd), match.Value);
                        }
                        break;
                    default:
                        {
                            MessageBox.Show("Command not recognized! Type help for more information.");
                            break;
                        }
                }

                Invoke(new Action<string>(SRCpageAdd), "listcreate");

                sr.Close();
            }
            catch
            { }
        }

        #endregion

        #region File_List_Create

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
                senderror.ErrorLog("Error! File_Write: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Creation of List file
        /// </summary>
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
                    Console.Beep(1200, 200);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! List_Create: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void CreateData(string fileName, string fileValue)
        {
            try
            {
                using (StreamWriter fc = File.AppendText(fileName))
                {
                    fc.WriteLine(fileValue);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateData: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpnFileOpt(string dirPath)
        {
            if (!File.Exists(Class_Var.DEFAULT_EDITOR))
            {
                MessageBox.Show("Editor are not exist in directory, verify your config file!", "Error!");
                return;
            }

            if (!File.Exists(dirPath))
                File_Write(dirPath, string.Empty);

            OpenFile_Editor(dirPath);
        }

        #endregion

        void OpenFile_Editor(string fileSelect)
        {
            try
            {
                if (!File.Exists(Class_Var.DEFAULT_EDITOR))
                {
                    MessageBox.Show("Editor are not exist in directory, verify your config file!", "Error!");
                    return;
                }

                string aArg = string.Empty;
                string strName = Path.GetFileName(Class_Var.DEFAULT_EDITOR);
                if (strName == "OstiumE.exe")
                    aArg = "/input=\"" + fileSelect + "\"";
                else
                    aArg = fileSelect;


                if (fileSelect != string.Empty)
                {
                    if (File.Exists(fileSelect))
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
                senderror.ErrorLog("Error! OpenFile_Editor: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void Open_Doc_Frm(string fileSelect)
        {
            Class_Var.File_Open = fileSelect;
            docForm = new Doc_Frm();
            docForm.Show();
        }

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
                senderror.ErrorLog("Error! URL_SAVE_Cbx_SelectedIndexChanged: ", ex.ToString(), "Main_Frm", AppStart);
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
                senderror.ErrorLog("Error! DatabasePnl: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void AddTable_Btn_Click(object sender, EventArgs e)
        {
            TableName_Txt.Text = Regex.Replace(TableName_Txt.Text, "[^a-zA-Z]", string.Empty);

            if (TableName_Txt.Text != string.Empty)
            {
                Sqlite_Cmd("CREATE TABLE IF NOT EXISTS " + TableName_Txt.Text + " (url_date TEXT, url_name TEXT, url_adress TEXT)");
                TableName_Txt.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Insert name Table first!", string.Empty);
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
                Cbx_Object.Text = string.Empty;
                Cbx_Object.Items.Clear();

                if (Tables_Lst_Opt == "add")
                {
                    UrlName_Txt.Text = Regex.Replace(UrlName_Txt.Text, "[^\\w\\\x20_-]", string.Empty);

                    if (UrlName_Txt.Text != string.Empty)
                    {
                        Sqlite_Read("SELECT * FROM " + tlsi + string.Empty, "url_name", "cbx");

                        int x;
                        x = Cbx_Object.FindStringExact(UrlName_Txt.Text);
                        if (x == -1)
                        {
                            Sqlite_Cmd("INSERT INTO " + tlsi + " (url_date, url_name, url_adress) VALUES ('" + DateTime.Now.ToString("d") + "', '" + UrlName_Txt.Text + "', '" + DataAddSelectUri + "')");

                            UrlName_Txt.Text = string.Empty;
                            Cbx_Object.Text = string.Empty;
                            Cbx_Object.Items.Clear();

                            Sqlite_Read("SELECT * FROM " + tlsi + string.Empty, "url_name", "cbx");

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
                        MessageBox.Show("Insert name URi first!", string.Empty);
                    }
                }
                else
                {
                    Sqlite_Read("SELECT * FROM " + tlsi + string.Empty, "url_name", "cbx");
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
                myDB = new SQLiteConnection("Data Source=" + databasePath + ";Version=3;");
                myDB.Open();

                string sql = execSql;
                SQLiteCommand commande = new SQLiteCommand(sql, myDB);
                commande.ExecuteNonQuery();

                myDB.Close();

                if (DBadmin == "off")
                {
                    OpnAllTable();
                    Console.Beep(1000, 400);
                }
                DBadmin = "off";
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Sqlite_Cmd: ", ex.ToString(), "Main_Frm", AppStart);
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
                using (SQLiteConnection myDB = new SQLiteConnection("Data Source=" + databasePath + ";Version=3;"))
                {
                    myDB.Open();
                    using (SQLiteCommand commande = new SQLiteCommand(execSql, myDB))
                    using (SQLiteDataReader reader = commande.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (ObjLstCbx == "lst")
                                List_Object.Items.Add(reader[valueDB]);
                            else
                                Cbx_Object.Items.Add(reader[valueDB]);
                        }
                    }
                }

                if (List_Object == DataTable_Lst)
                    TableCount_Lbl.Text = "table " + List_Object.Items.Count;

                if (List_Object == DataValue_Lst)
                    RecordsCount_Lbl.Text = "records " + List_Object.Items.Count;

                DBadmin = "off";
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Sqlite_Read: ", ex.ToString(), "Main_Frm", AppStart);
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
                myDB = new SQLiteConnection("Data Source=" + databasePath + ";Version=3;");
                myDB.Open();

                string sql = execSql;
                SQLiteCommand commande = new SQLiteCommand(sql, myDB);

                SQLiteDataReader reader = commande.ExecuteReader();

                while (reader.Read())
                {
                    if (DBadmin == "off")
                        WBrowse.Source = new Uri((string)reader[valueDB]);
                    else
                        DataValue_Opn.Text = (string)reader[valueDB];
                }

                myDB.Close();

                DBadmin = "off";
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Sqlite_ReadUri: ", ex.ToString(), "Main_Frm", AppStart);
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
                if (!string.IsNullOrEmpty(DataBaze_Opn.Text))
                {
                    if (File.Exists(Path.Combine(DBdirectory, DataBaze_Opn.Text)))
                    {
                        string message = "Change default Database?";
                        string caption = "Database default";
                        var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            ChangeDBdefault(DataBaze_Opn.Text);
                            databasePath = DBdirectory + DataBaze_Opn.Text;
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
                            DataBaze_Opn.Text = Regex.Replace(DataBaze_Opn.Text, "[^a-zA-Z0-9]", string.Empty);
                            DataBaze_Opn.Text += ".db";

                            SQLiteConnection.CreateFile(DBdirectory + DataBaze_Opn.Text);
                            ChangeDBdefault(DataBaze_Opn.Text);
                            databasePath = DBdirectory + DataBaze_Opn.Text;
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
                senderror.ErrorLog("Error! ChangeDefDB_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void ExecuteCMDsql_Btn_Click(object sender, EventArgs e)
        {
            DataValue_Lst.Items.Clear();
            List_Object = DataValue_Lst;

            Sqlite_Read(ExecuteCMDsql_Txt.Text, ValueCMDsql_Txt.Text, "lst");
        }

        void Db_Delete_Table_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataTable_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    tlsi = DataTable_Opn.Text;

                    string message = $"Are you sure that you would like to delete the Database? {tlsi}";
                    const string caption = "Database delete";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Sqlite_Cmd("DROP TABLE [" + tlsi + "];");

                        DataTable_Lst.Items.Clear();
                        DataValue_Lst.Items.Clear();
                        List_Object = DataTable_Lst;

                        Sqlite_Read("SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1", "name", "lst");

                        DataTable_Opn.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("No data select!");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Db_Delete_Table_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void Db_Delete_Table_Value_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataValue_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    string message = $"Are you sure that you would like to delete the entry? {DataValue_Lst.Text}";
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

                        Sqlite_Read("SELECT * FROM " + tlsi + string.Empty, "url_name", "lst");

                        DataValue_Opn.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("No data select!");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Db_Delete_Table_Value_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void Db_Delete_Table_AllValue_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataTable_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    tlsi = DataTable_Opn.Text;

                    string message = $"Are you sure that you would like to delete all entry? {tlsi}";
                    const string caption = "Database delete";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Sqlite_Cmd("DELETE FROM [" + tlsi + "];");

                        DataValue_Lst.Items.Clear();
                        DataValue_Opn.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("No data select!");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Db_Delete_Table_AllValue_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void Db_Update_Name_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                ValueChange_Txt.Text = Regex.Replace(ValueChange_Txt.Text, "[^\\w\\\x20_-]", string.Empty);

                if (DataValue_Lst.SelectedIndex != -1)
                {
                    if (ValueChange_Txt.Text != string.Empty && ValueChange_Txt.Text != "update URL and Name here")
                    {
                        DBadmin = "on";

                        string usct = DataValue_Lst.Text;
                        tlsi = DataTable_Opn.Text;

                        string message = $"Are you sure that you update entry? \r\n {DataValue_Lst.Text} <=> {ValueChange_Txt.Text}";
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

                                Sqlite_Read("SELECT * FROM " + tlsi + string.Empty, "url_name", "lst");

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
                senderror.ErrorLog("Error! Db_Update_Name_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void Db_Update_Value_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataValue_Lst.SelectedIndex != -1)
                {
                    if (ValueChange_Txt.Text != string.Empty && ValueChange_Txt.Text != "update URL and Name here")
                    {
                        DBadmin = "on";

                        string usct = DataValue_Opn.Text;
                        tlsi = DataTable_Opn.Text;

                        string message = $"Are you sure that you modify entry? \r\n {DataValue_Opn.Text} <=> {ValueChange_Txt.Text}";
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
                senderror.ErrorLog("Error! Db_Update_Value_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void DataBaze_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataBaze_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    DataTable_Opn.Text = string.Empty;
                    DataValue_Name.Text = string.Empty;
                    DataValue_Opn.Text = string.Empty;
                    ValueChange_Txt.ForeColor = Color.DimGray;
                    ValueChange_Txt.Text = "update URL and Name here";

                    DataValue_Lst.Items.Clear();
                    DataTable_Lst.Items.Clear();
                    List_Object = DataTable_Lst;

                    DataBaze_Opn.Text = DataBaze_Lst.SelectedItem.ToString();
                    DBSelectOpen_Lbl.Text = $"DB open: {DataBaze_Opn.Text}";
                    TableOpen_Lbl.Text = string.Empty;
                    RecordsCount_Lbl.Text = string.Empty;

                    databasePath = DBdirectory + DataBaze_Opn.Text;

                    Sqlite_Read("SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1", "name", "lst");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DataBaze_Lst_SelectedIndexChanged: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void DataTable_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataTable_Lst.SelectedIndex != -1)
                {
                    DBadmin = "on";

                    DataValue_Name.Text = string.Empty;
                    DataValue_Opn.Text = string.Empty;
                    ValueChange_Txt.ForeColor = Color.DimGray;
                    ValueChange_Txt.Text = "update URL and Name here";

                    DataValue_Lst.Items.Clear();
                    List_Object = DataValue_Lst;
                    tlsi = DataTable_Lst.SelectedItem.ToString();

                    DataTable_Opn.Text = DataTable_Lst.SelectedItem.ToString();
                    TableOpen_Lbl.Text = $"Table open: {DataTable_Opn.Text}";

                    Sqlite_Read("SELECT * FROM " + tlsi + string.Empty, "url_name", "lst");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! DataTable_Lst_SelectedIndexChanged: ", ex.ToString(), "Main_Frm", AppStart);
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
                senderror.ErrorLog("Error! DataValue_Lst_SelectedIndexChanged: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void ChangeDBdefault(string NameDB)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(AppStart + "config.xml");
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Xwparsingxml/Xwparsingnode/DB_USE_DEFAULT") is XmlElement nod)
                    nod.InnerText = NameDB;

                xmlReader.Close();
                doc.Save(AppStart + "config.xml");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ChangeDBdefault: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void ValueChange_Txt_Click(object sender, EventArgs e)
        {
            ValueChange_Txt.ForeColor = Color.White;
            ValueChange_Txt.Text = string.Empty;
        }

        void Db_OpnLink_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser(DataValue_Opn.Text, 0);
            CtrlTabBrowsx();
            Control_Tab.SelectedIndex = 0;
        }

        void Db_OrderLst_Btn_Click(object sender, EventArgs e)
        {
            DataValue_Lst.Sorted = !DataValue_Lst.Sorted;

            if (DataValue_Lst.Sorted)
                Db_OrderLst_Btn.ForeColor = Color.Lime;
            else
                Db_OrderLst_Btn.ForeColor = Color.White;
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
        /// <param value="on">Opening rss feed category file for editing</param>
        /// <param value="off">Loading the category's RSS feed</param>
        /// <param name="LoadFeed"></param>
        /// <param value="0">Cleaning the List before loading</param>
        ///
        void CategorieFeed_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tools_TAB_1.Focus();

            if (ManageFeed == "on")
            {
                ListFeed_Lst.Items.Clear();
                ListFeed_Lst.Items.AddRange(File.ReadAllLines(Path.Combine(FeedDir, CategorieFeed_Cbx.Text)));
                return;
            }

            if (File.Exists(Path.Combine(FeedDir, CategorieFeed_Cbx.Text)))
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
                ListFeed_Lst.Items.AddRange(File.ReadAllLines(Path.Combine(FeedDir, CategorieFeed_Cbx.Text)));

                Title_Lst.Items.Clear();
                Link_Lst.Items.Clear();

                MSenter();

                Thread Thr_RssFeed = new Thread(() => LoadFeed(0));
                Thr_RssFeed.Start();

                splitContain_Rss.Panel1Collapsed = false;
                CollapseTitleFeed_Btn.Text = "Collapse Off";
            }
        }

        void CountBlockSite_Lbl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RSSListSite_Lbl.SelectedIndex != -1)
            {
                CountBlockFeed_Lbl.Items[RSSListSite_Lbl.SelectedIndex].ToString();
                int value = Convert.ToInt32(CountBlockFeed_Lbl.Items[RSSListSite_Lbl.SelectedIndex].ToString());

                Title_Lst.SetSelected(value, true);
            }
        }
        ///
        /// <summary>
        /// Loading the list of Flows
        /// </summary>
        /// <param value="value">RSS Feed URL for the category </param>
        /// <param name="FeedTitle">Loading RSS feed sites</param>
        /// <param name="ListCount">Counter of the number of Titles per RSS feed site</param>
        /// <param name="ItemTitleAdd">Added title in "Title_Lst"</param>
        /// <param name="ItemLinkAdd">Added title URL in Link_Lst</param>
        /// <param name="CountFeed">Display of the number of titles in "Title_Lst"</param>
        /// <param name="AllTrue">Enable elements</param>
        /// <param name="Msleave">Overlaying the RSS feed site window on top</param>
        ///       
        async void LoadFeed(int startIndex)
        {
            if (startIndex == 0)
                Invoke(new Action<string>(CountBlockSite_Invk), "Clear");

            try
            {
                Invoke(new Action<string>(CountBlockSite_Invk), "Disable");

                for (int i = startIndex; i < ListFeed_Lst.Items.Count; i++)
                {
                    if (ListFeed_Lst.Items[i] == null) continue;

                    string url = ListFeed_Lst.Items[i].ToString();
                    if (string.IsNullOrWhiteSpace(url)) continue;

                    try
                    {
                        await Task.Run(() => ProcessFeed(url));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading RSS feed: {url}\nDetails: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                Invoke(new Action(() =>
                {
                    CountBlockSite_Invk("CountFeed");
                    CountBlockSite_Invk("AllTrue");
                    CountBlockSite_Invk("Msleave");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void ProcessFeed(string url)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                Async = true,
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Ignore
            };

            using (XmlReader reader = XmlReader.Create(url, settings))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                var uiUpdates = new List<Action>
                {
                    () =>
                {
                    TitleFeed = feed.Title?.Text ?? "Untitled Feed";
                    CountBlockSite_Invk("FeedTitle");
                    CountBlockSite_Invk("ListCount");
                }
                };

                foreach (SyndicationItem item in feed.Items)
                {
                    string title = item.Title?.Text ?? "Untitled Item";
                    string link = item.Links.FirstOrDefault()?.Uri?.ToString() ?? string.Empty;

                    uiUpdates.Add(() =>
                    {
                        AddTitleItem = title;
                        CountBlockSite_Invk("ItemTitleAdd");
                        if (!string.IsNullOrEmpty(link))
                        {
                            AddLinkItem = link;
                            CountBlockSite_Invk("ItemLinkAdd");
                        }
                    });
                }

                Invoke(new Action(() =>
                {
                    foreach (var update in uiUpdates)
                    {
                        update();
                    }
                }));
            }
        }

        void CreatCategorie_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (NewCategory_Txt.Text != string.Empty && NewCategory_Txt.Text != "new category")
                {
                    if (!File.Exists(Path.Combine(FeedDir, NewCategory_Txt.Text)))
                    {
                        File_Write(FeedDir + NewCategory_Txt.Text, string.Empty);

                        CategorieFeed_Cbx.Items.Clear();
                        loadfiledir.LoadFileDirectory(FeedDir, "*", "cbxts", CategorieFeed_Cbx);

                        CategorieFeed_Cbx.Text = string.Empty;
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
                senderror.ErrorLog("Error! CreatCategorie_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void NewCategory_Txt_Click(object sender, EventArgs e)
        {
            NewCategory_Txt.ForeColor = Color.White;
            NewCategory_Txt.Text = string.Empty;
        }

        void NewFeed_Txt_Click(object sender, EventArgs e)
        {
            NewFeed_Txt.ForeColor = Color.White;
            NewFeed_Txt.Text = string.Empty;
        }

        void AddFeed_Btn_Click(object sender, EventArgs e)
        {
            if (NewFeed_Txt.Text != string.Empty && NewFeed_Txt.Text != "new feed")
            {
                if (CategorieFeed_Cbx.Text != string.Empty)
                {
                    CreateData(Path.Combine(FeedDir, CategorieFeed_Cbx.Text), NewFeed_Txt.Text);

                    CategorieFeed_Cbx.Items.Clear();
                    loadfiledir.LoadFileDirectory(FeedDir, "*", "cbxts", CategorieFeed_Cbx);

                    CategorieFeed_Cbx.Text = string.Empty;
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
            bool isManageFeed = ManageFeed_Btn.Text == "Manage feed";

            if (isManageFeed)
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
                if (!string.IsNullOrEmpty(CategorieFeed_Cbx.Text))
                {
                    string message = $"Are you sure that you would like to delete the category and all content? {CategorieFeed_Cbx.Text}";
                    const string caption = "Delete Category";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (File.Exists(Path.Combine(FeedDir, CategorieFeed_Cbx.Text)))
                        {
                            File.Delete(Path.Combine(FeedDir, CategorieFeed_Cbx.Text));

                            CategorieFeed_Cbx.Text = string.Empty;
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
                senderror.ErrorLog("Error! DeleteCatfeed_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void DeleteURLfeed_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ListFeed_Lst.SelectedIndex != -1)
                {
                    string message = $"Are you sure that you would like to delete the URL? => {ListFeed_Lst.SelectedItem}";
                    const string caption = "Suppress URL";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        ListFeed_Lst.Items.Remove(ListFeed_Lst.SelectedItem);
                        File.Delete(Path.Combine(FeedDir, CategorieFeed_Cbx.Text));

                        var CategoryFile = Path.Combine(FeedDir, CategorieFeed_Cbx.Text);
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
                senderror.ErrorLog("Error! DeleteURLfeed_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void CollapseTitleFeed_Btn_Click(object sender, EventArgs e)
        {
            splitContain_Rss.Panel1Collapsed = !splitContain_Rss.Panel1Collapsed;

            if (!splitContain_Rss.Panel1Collapsed)
                CollapseTitleFeed_Btn.Text = "Collapse Off";
            else
                CollapseTitleFeed_Btn.Text = "Collapse On";
        }

        void GoFeed_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Title_Lst.Items.Count > 0)
                {
                    if (GoFeed_Txt.Text != string.Empty && GoFeed_Txt.Text != "0")
                    {
                        int icr = Convert.ToInt32(GoFeed_Txt.Text);

                        if (icr <= Title_Lst.Items.Count)
                            Title_Lst.SetSelected(icr - 1, true);
                        else
                            MessageBox.Show($"Max count title = {Title_Lst.Items.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! GoFeed_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void GoFeed_Txt_Click(object sender, EventArgs e)
        {
            GoFeed_Txt.Text = string.Empty;
        }

        void HomeFeed_Btn_Click(object sender, EventArgs e)
        {
            WBrowsefeed.Source = new Uri(HomeUrlRSS);
            CountFeed_Lbl.Text = string.Empty;
        }

        #endregion

        #region Feed_Speak

        void LoadLang()
        {
            try
            {
                VerifLangOpn = 1;
                var VoiceInstall = synth.GetInstalledVoices();

                foreach (InstalledVoice v in VoiceInstall)
                    LangSelect_Lst.Items.Add(v.VoiceInfo.Name);

                LangSelect_Lst.SelectedIndex = 0;

                synth.SetOutputToDefaultAudioDevice();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadLang: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void SpeakOpenPnl_Btn_Click(object sender, EventArgs e)
        {
            Speak_Pnl.Visible = !Speak_Pnl.Visible;

            if (VerifLangOpn == 0)
                LoadLang();

            if (Speak_Pnl.Visible)
            {
                VolumeValue_Lbl.Text = "Volume: " + Convert.ToString(VolumeVal_Track.Value);
                RateValue_Lbl.Text = "Rate: " + Convert.ToString(RateVal_Track.Value);
            }
        }

        void ReadTitle_Btn_Click(object sender, EventArgs e)
        {
            if (CategorieFeed_Cbx.Text != string.Empty)
            {
                synth.Volume = Class_Var.VOLUME_TRACK;
                synth.Rate = Class_Var.RATE_TRACK;
                synth.SelectVoice(LangSelect_Lst.SelectedItem.ToString());

                for (int i = 0; i < Link_Lst.Items.Count; i++)
                {
                    synth.SpeakAsync("Title " + Convert.ToString(i + 1) + ": " + Title_Lst.Items[i].ToString());
                }
            }
        }

        void ReadClipB_Btn_Click(object sender, EventArgs e)
        {
            if (CategorieFeed_Cbx.Text != string.Empty)
                Speak_Clipboard_Text();
        }

        void PauseSpeak_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                bool isPauseOn = PauseSpeak_Btn.Text == "Pause";

                if (isPauseOn)
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

            Console.Beep(800, 200);
        }

        void SaveVolumeRate(string nodeselect, int value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(Path.Combine(AppStart, "config.xml"));
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Xwparsingxml/Xwparsingnode/" + nodeselect) is XmlElement nod)
                    nod.InnerText = Convert.ToString(value);

                xmlReader.Close();
                doc.Save(Path.Combine(AppStart, "config.xml"));
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! SaveVolumeRate: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void Speak_Clipboard_Text()
        {
            try
            {
                synth.Volume = Class_Var.VOLUME_TRACK;
                synth.Rate = Class_Var.RATE_TRACK;
                synth.SelectVoice(LangSelect_Lst.SelectedItem.ToString());
                synth.SpeakAsync(Clipboard.GetText(TextDataFormat.Text));
            }
            catch
            { }
        }

        #endregion

        void RSSListSite_Lbl_Lbl_MouseEnter(object sender, EventArgs e)
        {
            MSenter();
        }

        void RSSListSite_Lbl_Lbl_Leave(object sender, EventArgs e)
        {
            MSleave();
        }

        void MSenter()
        {
            RSSListSite_Lbl.BringToFront();
            RSSListSite_Lbl.Width = 250;
            RSSListSite_Lbl.Visible = true;
        }

        void MSleave()
        {
            RSSListSite_Lbl.SendToBack();
            RSSListSite_Lbl.Width = 50;
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

        void ClrHistory_Param_Click(object sender, EventArgs e)
        {
            ClearData(0);
        }

        void System_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("edge://system", 0);
        }

        #endregion
        ///
        /// <summary>
        /// Cookies save
        /// </summary>
        /// <param value="URLs">Saved cookies only if SaveCookies_Chk checked = True, by default is False</param>
        /// 
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
            { }
        }

        void SaveCookies_Chk_CheckedChanged(object sender, EventArgs e)
        {
            if (SaveCookies_Chk.Checked)
            {
                Class_Var.COOKIES_SAVE = 1; // Save
                SaveCookies_Chk.ForeColor = Color.Yellow;
            }
            else
            {
                Class_Var.COOKIES_SAVE = 0; // No save
                SaveCookies_Chk.ForeColor = Color.White;
            }
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
                if (string.IsNullOrEmpty(AddItemswf_Txt.Text))
                    return;

                if (string.IsNullOrEmpty(NameProjectwf_Txt.Text))
                {
                    NameProjectwf_Txt.BackColor = Color.Red;
                    MessageBox.Show("Select Project name first!");
                    NameProjectwf_Txt.BackColor = Color.FromArgb(28, 28, 28);
                    return;
                }
                ///
                /// removing spaces and line breaks
                /// 
                string text = AddItemswf_Txt.Text;
                string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = lines[i].Trim();
                    lines[i] = lines[i].Replace(" ", "_");
                }
                string formattedText = string.Join(Environment.NewLine, lines);
                AddItemswf_Txt.Text = formattedText;
                ///
                /// Check for duplicates
                /// 
                ListBox WordVerify = new ListBox();
                string[] verifyWord = AddItemswf_Txt.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in verifyWord)
                {
                    WordVerify.Items.Add(line);
                }

                HashSet<string> WordAdd = new HashSet<string>();

                foreach (var word in WordVerify.Items)
                {
                    if (!string.IsNullOrEmpty(word?.ToString()))
                    {
                        if (!WordAdd.Add(word.ToString()))
                        {
                            MessageBox.Show("Word [" + word.ToString() + "] duplicate in your workflow, correct this for continue!");
                            WordAdd.Clear();
                            return;
                        }
                    }
                }

                ListBox List_Wf = new ListBox();
                string[] linesAdd = AddItemswf_Txt.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in linesAdd)
                {
                    List_Wf.Items.Add(line);
                }

                XmlTextWriter writer = new XmlTextWriter(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"), Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.Indentation = 2;

                writer.WriteStartElement("Table");
                writer.WriteStartElement("KeywordItemCollect");

                foreach (var itemcollect in List_Wf.Items)
                {
                    if (!string.IsNullOrEmpty(itemcollect?.ToString()))
                    {
                        writer.WriteElementString("Item", itemcollect.ToString());
                    }
                }

                writer.WriteEndElement();

                foreach (var itemcollect in List_Wf.Items)
                {
                    if (!string.IsNullOrEmpty(itemcollect?.ToString()))
                    {
                        writer.WriteStartElement(itemcollect.ToString());
                        CreateNode(itemcollect.ToString(), writer); // Markup
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

                loadfiledir.LoadFileDirectory(Workflow, "xml", "lst", ProjectOpn_Lst);
                AddItemswf_Txt.Text = string.Empty;
                NameProjectwf_Txt.Text = string.Empty;

                if (File.Exists(Path.Combine(AppStart, "tempItemAdd.txt")))
                    File.Delete(Path.Combine(AppStart, "tempItemAdd.txt"));

                ModelList_Lst.Enabled = false;
                ModelDelete_Btn.Enabled = false;
                ModelEdit_Btn.Enabled = false;
                ModelList_Lst.ClearSelected();

                Console.Beep(1000, 400);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateXMLwf_btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
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
                if (AddSingleItemswf_Txt.Text == string.Empty)
                    return;

                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"));
                doc.Load(xmlReader);

                string Markup = "<!--" + AddSingleItemswf_Txt.Text + "-->";
                ///
                /// removing spaces and line breaks
                ///
                string[] str = AddSingleItemswf_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                AddSingleItemswf_Txt.Text = string.Empty;
                foreach (string s in str)
                {
                    if (s.Trim().Length > 0)
                        AddSingleItemswf_Txt.Text += s + " ";
                }
                string header = AddSingleItemswf_Txt.Text;
                AddSingleItemswf_Txt.Text = header.Trim(new char[] { ' ' });

                if (doc.SelectSingleNode("/Table") is XmlElement nod)
                {
                    int WordWithoutDuplicate;
                    WordWithoutDuplicate = Itemwf_Cbx.FindStringExact(AddSingleItemswf_Txt.Text);
                    if (WordWithoutDuplicate == -1)
                    {
                        XmlElement elem = doc.CreateElement(AddSingleItemswf_Txt.Text);
                        elem.InnerXml = Markup; // Markup
                        nod.AppendChild(elem);
                    }
                }

                xmlReader.Close();
                doc.Save(Workflow + NameProjectwf_Txt.Text + ".xml");

                int KeywordWithoutDuplicate;
                KeywordWithoutDuplicate = Itemwf_Cbx.FindStringExact(AddSingleItemswf_Txt.Text);
                if (KeywordWithoutDuplicate == -1)
                    AddDataWorkflow("KeywordItemCollect", "Item", AddSingleItemswf_Txt.Text, "no");

                Itemwf_Cbx.Items.Clear();
                WorkflowItem_Lst.Items.Clear();
                AddItemswf_Txt.Text = string.Empty;

                Workflow_Cbx = Itemwf_Cbx;
                Workflow_Lst = WorkflowItem_Lst;

                Thread LoadAllXML = new Thread(() => LoadItemKeyword_Thr("KeywordItemCollect", "Item", "y"));
                LoadAllXML.Start();

                AddSingleItemswf_Txt.Text = string.Empty;
                Console.Beep(1000, 400);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddItemwf_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
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
            AddItemswf_Txt.Text = string.Empty;
            AddSingleItemswf_Txt.Enabled = false;
            AddItemwf_Btn.Enabled = false;
            NameProjectwf_Txt.Enabled = true;
            NameProjectwf_Txt.Text = string.Empty;
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
                        Timeline_Lst.Items.AddRange(File.ReadAllLines(Path.Combine(Workflow, tml)));

                    if (File.Exists(Path.Combine(Workflow, ProjectOpn_Lst.SelectedItem.ToString())))
                    {
                        Itemwf_Cbx.Items.Clear();
                        WorkflowItem_Lst.Items.Clear();
                        AddItemswf_Txt.Text = string.Empty;
                        CreateXMLwf_btn.Enabled = false;
                        AddSingleItemswf_Txt.Enabled = true;
                        AddItemwf_Btn.Enabled = true;
                        ModelList_Lst.Enabled = false;
                        ModelDelete_Btn.Enabled = false;
                        ModelEdit_Btn.Enabled = false;
                        ModelList_Lst.ClearSelected();

                        string s = ProjectOpn_Lst.SelectedItem.ToString().Replace(".xml", string.Empty);
                        NameProjectwf_Txt.Text = s;
                        NameProjectwf_Txt.Enabled = false;
                        WFProjectOpn_Lbl.Text = s;
                        AddItemswf_Txt.Enabled = false;

                        Workflow_Cbx = Itemwf_Cbx;
                        Workflow_Lst = WorkflowItem_Lst;

                        Thread LoadItemKeyword = new Thread(() => LoadItemKeyword_Thr("KeywordItemCollect", "Item", "y"));
                        LoadItemKeyword.Start();
                    }
                    else
                    {
                        MessageBox.Show("The Project not exist!", "Ostium", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ProjectOpn_Lst_SelectedIndexChanged: ", ex.ToString(), "Main_Frm", AppStart);
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
                if (AddTextWorkflow_Txt.Text != string.Empty ||
                    AddTNoteWorkflow_Txt.Text != string.Empty ||
                    AddUrlWorkflow_Txt.Text != string.Empty)
                {
                    if (AddTextWorkflow_Txt.Text == string.Empty)
                        AddTextWorkflow_Txt.Text = "None";

                    if (AddTNoteWorkflow_Txt.Text == string.Empty)
                        AddTNoteWorkflow_Txt.Text = "None";

                    if (AddUrlWorkflow_Txt.Text == string.Empty)
                        AddUrlWorkflow_Txt.Text = "None";

                    FormatValue();
                }
                else
                {
                    WorkflowItem_Lst.ClearSelected();
                }
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
                /// removing spaces and line breaks
                ///
                string[] str = AddTextWorkflow_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                AddTextWorkflow_Txt.Text = string.Empty;
                foreach (string s in str)
                {
                    if (s.Trim().Length > 0)
                        AddTextWorkflow_Txt.Text += s + " ";
                }
                string header = AddTextWorkflow_Txt.Text;
                AddTextWorkflow_Txt.Text = header.Trim(new char[] { ' ' });

                string value = AddTextWorkflow_Txt.Text;
                string valMessage = AddTextWorkflow_Txt.Text + "\r\n" + AddTNoteWorkflow_Txt.Text + "\r\n" + AddUrlWorkflow_Txt.Text + "\r\n";

                string message = "Add => \r\n" + valMessage + " => in " + WorkflowItem_Lst.SelectedItem.ToString() + " ?";
                string caption = string.Empty;
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    AddDataWorkflow(WorkflowItem_Lst.SelectedItem.ToString(), element, value, "yes");
                }
                else
                {
                    WorkflowItem_Lst.ClearSelected();
                    AddTextWorkflow_Txt.Text = string.Empty;
                    AddTNoteWorkflow_Txt.Text = string.Empty;
                    AddUrlWorkflow_Txt.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! FormatValue: ", ex.ToString(), "Main_Frm", AppStart);
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
                XmlTextReader xmlReader = new XmlTextReader(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"));
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Table/" + nodeselect) is XmlElement nod)
                {
                    XmlElement elem = doc.CreateElement(elementselect);

                    if (attrib != "no")
                    {
                        ///
                        /// removing spaces and line breaks
                        ///
                        if (AddTNoteWorkflow_Txt.Text != string.Empty)
                        {
                            string[] str = AddTNoteWorkflow_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            AddTNoteWorkflow_Txt.Text = string.Empty;
                            foreach (string s in str)
                            {
                                if (s.Trim().Length > 0)
                                    AddTNoteWorkflow_Txt.Text += s + " ";
                            }
                            string header = AddTNoteWorkflow_Txt.Text;
                            AddTNoteWorkflow_Txt.Text = header.Trim(new char[] { ' ' });
                        }

                        if (AddUrlWorkflow_Txt.Text != string.Empty)
                        {
                            string[] str = AddUrlWorkflow_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            AddUrlWorkflow_Txt.Text = string.Empty;
                            foreach (string s in str)
                            {
                                if (s.Trim().Length > 0)
                                    AddUrlWorkflow_Txt.Text += s + " ";
                            }
                            string header = AddUrlWorkflow_Txt.Text;
                            AddUrlWorkflow_Txt.Text = header.Trim(new char[] { ' ' });
                        }

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
                doc.Save(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"));

                if (WorkflowItem_Lst.SelectedIndex != -1)
                {
                    string valTimeline = AddTextWorkflow_Txt.Text + " -- " + AddTNoteWorkflow_Txt.Text + " -- " + AddUrlWorkflow_Txt.Text + " -- ";
                    Timeline_Lst.Items.Add(WorkflowItem_Lst.SelectedItem.ToString() + ": " + valTimeline);
                    using (StreamWriter SW = new StreamWriter(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".ost"), false))
                    {
                        foreach (string itm in Timeline_Lst.Items)
                            SW.WriteLine(itm);
                    }
                }

                AddTextWorkflow_Txt.Text = string.Empty;
                AddTNoteWorkflow_Txt.Text = string.Empty;
                AddUrlWorkflow_Txt.Text = string.Empty;
                WorkflowItem_Lst.ClearSelected();

                LoadStatWorkflow();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddDataWorkflow: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void ForceLinkParent_Btn_Click(object sender, EventArgs e)
        {
            IsParentLinkEnabled = !IsParentLinkEnabled;

            if (IsParentLinkEnabled)
                ForceLinkParent_Btn.ForeColor = Color.Red;
            else
                ForceLinkParent_Btn.ForeColor = Color.Black;
        }
        ///
        /// <summary>
        /// Displaying the data addition section of WorkFlow projects in "TAB BROWSx"
        /// </summary>
        ///
        void OpnWorkflowTools_Tls_Click(object sender, EventArgs e)
        {
            if (NameProjectwf_Txt.Text != string.Empty)
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
                xmlDoc.Load(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".xml"));
                string xpath = $"/Table/{nodeselect}/{elementselect}";
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes(xpath);

                if (LoadStat == "n")
                    Invoke(new Action<string>(SRCpageAdd), "listclear");

                for (int i = 0; i < nodeList.Count; i++)
                {
                    string strvalue = string.Format("{0}", nodeList[i].ChildNodes.Item(0).InnerText);

                    if (LoadStat == "y")
                        Invoke(new Action<string>(LoadItemKeyword_Invk), strvalue);
                    else
                        Invoke(new Action<string>(SRCpageAdd), strvalue);
                }

                if (LoadStat == "n")
                    Invoke(new Action<string>(SRCpageAdd), "listcreate");

                if (LoadStat == "y")
                    Invoke(new Action(LoadStatWorkflow_Invk));
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadItemKeyword_Thr: ", ex.ToString(), "Main_Frm", AppStart);
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
                    if (WorkflowItem_Lst.Items[i].ToString() != string.Empty)
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
                senderror.ErrorLog("Error! LoadStatWorkflow: ", ex.ToString(), "Main_Frm", AppStart);
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
                if (!string.IsNullOrEmpty(ModelName_Txt.Text) && !string.IsNullOrEmpty(ModelItem_Txt.Text))
                {
                    string header = ModelName_Txt.Text;
                    ModelName_Txt.Text = header.Trim(new char[] { ' ' });
                    ModelName_Txt.Text = ModelName_Txt.Text.Replace(" ", "_");

                    ModelItem_Txt.Text = Regex.Replace(ModelItem_Txt.Text, @" ", string.Empty);
                    string[] str = ModelItem_Txt.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    ModelItem_Txt.Text = string.Empty;
                    foreach (string s in str)
                    {
                        if (s.Trim().Length > 0)
                            ModelItem_Txt.Text += s + "\r\n";
                    }

                    if (File.Exists(Path.Combine(WorkflowModel, ModelName_Txt.Text + ".txt")))
                    {
                        string message = "File " + ModelName_Txt.Text + " exist, continue?";
                        string caption = "File exist";
                        var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.No)
                            return;
                    }

                    File_Write(Path.Combine(WorkflowModel, ModelName_Txt.Text + ".txt"), ModelItem_Txt.Text);

                    ModelItem_Txt.Text = string.Empty;
                    ModelName_Txt.Text = string.Empty;

                    loadfiledir.LoadFileDirectory(WorkflowModel, "txt", "lst", ModelList_Lst);

                    Console.Beep(1000, 400);
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
                senderror.ErrorLog("Error! ModelCreate_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void ModelList_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ModelList_Lst.SelectedIndex != -1)
                {
                    if (File.Exists(Path.Combine(WorkflowModel, ModelList_Lst.SelectedItem.ToString())))
                    {
                        AddItemswf_Txt.Text = string.Empty;

                        using (StreamReader sr = new StreamReader(Path.Combine(WorkflowModel, ModelList_Lst.SelectedItem.ToString())))
                        {
                            AddItemswf_Txt.Text = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ModelList_Lst_SelectedIndexChanged: ", ex.ToString(), "Main_Frm", AppStart);
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
                        if (File.Exists(Path.Combine(WorkflowModel, ModelList_Lst.SelectedItem.ToString())))
                        {
                            File.Delete(Path.Combine(WorkflowModel, ModelList_Lst.SelectedItem.ToString()));
                            AddItemswf_Txt.Text = string.Empty;

                            loadfiledir.LoadFileDirectory(WorkflowModel, "txt", "lst", ModelList_Lst);
                            Console.Beep(1000, 400);
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
                senderror.ErrorLog("Error! ModelDelete_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
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

                    if (File.Exists(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".ost")))
                        File.Delete(Path.Combine(Workflow, NameProjectwf_Txt.Text + ".ost"));
                }
            }
        }

        #endregion

        void BlockAds_Mnu_Click(object sender, EventArgs e)
        {
            IsAdsTrackersBlocked = !IsAdsTrackersBlocked;

            if (IsAdsTrackersBlocked)
            {
                BlockAdmenu_Mnu.ForeColor = Color.Red;
                BlockAdFeed_Btn.ForeColor = Color.Red;
            }
            else
            {
                BlockAdmenu_Mnu.ForeColor = Color.Lime;
                BlockAdFeed_Btn.ForeColor = Color.Lime;
            }
        }

        void ReloadListAds_Mnu_Click(object sender, EventArgs e)
        {
            LoadBlockList();
        }

        void AllowUrlAds_Mnu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(URLbrowse_Cbx.Text))
            {
                URLbrowse_Cbx.BackColor = Color.Red;
                MessageBox.Show("Add a URL to get started!", "Empty URL", MessageBoxButtons.OK, MessageBoxIcon.Information);
                URLbrowse_Cbx.BackColor = Color.FromArgb(41, 44, 51);
                return;
            }

            string message = "Are you sure you want to add this URL to the whitelist?";
            string caption = "Whitelist Add";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                using (StreamWriter add_url = File.AppendText(Path.Combine(AppStart, "data", AllowUrl)))
                {
                    add_url.WriteLine(URLbrowse_Cbx.Text);
                }
                LoadBlockList();
            }
        }

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

        void Furldir_Opt_Click(object sender, EventArgs e)
        {
            OpnFileOpt(Path.Combine(FileDir, "url.txt"));
        }

        void Furlconst_Opt_Click(object sender, EventArgs e)
        {
            OpnFileOpt(Path.Combine(FileDir, "url-constructor", "construct_url.txt"));
        }

        void Furlconstdir_Opt_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Path.Combine(FileDir, "url-constructor")))
                Process.Start(Path.Combine(FileDir, "url-constructor"));
        }

        void AddOntools_Opt_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Plugins))
                Process.Start(Plugins);
        }

        void Fmultiplewin_Opt_Click(object sender, EventArgs e)
        {
            OpnFileOpt(Path.Combine(FileDir, "grp-frm", "grp_frm_url_opn.txt"));
        }

        void MultipleDir_Opt_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Path.Combine(FileDir, "grp-frm")))
                Process.Start(Path.Combine(FileDir, "grp-frm"));
        }

        void Fgdork_Opt_Click(object sender, EventArgs e)
        {
            OpnFileOpt(Path.Combine(FileDir, "gdork.txt"));
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

        void JsonDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(JsonDir))
                Process.Start(JsonDir);
        }

        void KeepTrackDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Keeptrack))
                Process.Start(Keeptrack);
        }

        #endregion

        #region Process_

        void VerifProcessRun_Btn_Click(object sender, EventArgs e)
        {
            VerifyProcess("javaw");

            if (ProcessLoad)
                MessageBox.Show("Process is True.", "javaw");
            else
                MessageBox.Show("Process is False.", "javaw");
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
                    try
                    {
                        foreach (var process in localByName)
                        {
                            process.Kill();
                        }
                        MessageBox.Show("The Process: javaw was successfully stopped!", "Kill process");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to kill process: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Process not running.", "Process");
            }
        }

        void VerifyProcess(string ProcessVerif)
        {
            try
            {
                Process[] localByName = Process.GetProcessesByName(ProcessVerif);
                if (localByName.Length > 0)
                    ProcessLoad = true;
                else
                    ProcessLoad = false;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog($"Error! VerifyProcess: {ProcessVerif} ", ex.ToString(), "Main_Frm", AppStart);
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
            URLtxt_txt.Text = $"Unshorten URL => {value}";
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
                Console_Cmd_Txt.Enabled = false;
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
                    RSSListSite_Lbl.Items.Clear();
                    CountBlockFeed_Lbl.Items.Clear();
                    break;
                case "Disable":
                    RSSListSite_Lbl.Enabled = false;
                    break;
                case "AllTrue":
                    CategorieFeed_Cbx.Enabled = true;
                    RSSListSite_Lbl.Enabled = true;
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
                    string trimmedTitleFeed = TitleFeed.Trim();
                    RSSListSite_Lbl.Items.Add(trimmedTitleFeed);
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
                    RSSListSite_Lbl.SendToBack();
                    RSSListSite_Lbl.Width = 50;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Invoke_Diagram
        ///
        /// <summary>
        /// Opening the diagram in a new wBrowser
        /// </summary>
        /// <param name="value"></param>
        /// <param value="0">The diagram creation file is in the diagram directory</param>
        /// <param value="1">The diagram creation file is in a random directory</param>
        ///
        void CreateDiagram_Invk(string value)
        {
            if (Commut == 0)
                GoBrowser("file:///" + DiagramDir + value, 1);
            else if (Commut == 1)
                GoBrowser("file:///" + value, 1);
        }

        #endregion

        #region Bkmklt

        void OpnBookmark_Btn_Click(object sender, EventArgs e)
        {
            BookMarklet();
        }

        void BookMarklet()
        {
            try
            {
                Scriptl = "off";

                if (!PanelBkmklt_Pnl.Visible)
                    loadfiledir.LoadFileDirectory(BkmkltDir, "xml", "lst", Bookmarklet_Lst);

                PanelBkmklt_Pnl.Visible = !PanelBkmklt_Pnl.Visible;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnBokmark_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpnScriptl_Btn_Click(object sender, EventArgs e)
        {
            Scriptl = "on";

            if (!PanelBkmklt_Pnl.Visible)
                loadfiledir.LoadFileDirectory(Scripts + "scriptsl", "js", "lst", Bookmarklet_Lst);

            PanelBkmklt_Pnl.Visible = !PanelBkmklt_Pnl.Visible;
        }

        void Bookmarklet_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Scriptl == "off")
            {
                string BkmScr = Regex.Replace(Bookmarklet_Lst.Text, @".xml", string.Empty);
                OpnBookmark(BkmScr);
            }
        }

        void OpnBookmark(string strAttrib)
        {
            try
            {
                string gpxFile = BkmkltDir + Bookmarklet_Lst.Text;

                using (XmlReader reader = XmlReader.Create(gpxFile))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == strAttrib)
                        {
                            string strDesc = reader.GetAttribute("desc");
                            MinifyScr = reader.GetAttribute("mini");

                            Desc_Lbl.Text = strDesc;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnBookmark: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void InjectBkmklt_Btn_Click(object sender, EventArgs e)
        {
            if (Bookmarklet_Lst.SelectedIndex != -1)
            {
                if (Scriptl == "off")
                    InjectBkmklt(MinifyScr);
                else
                    InjectScriptl(Scripts + @"scriptsl\" + Bookmarklet_Lst.SelectedItem.ToString());
            }
        }

        async void InjectBkmklt(string Bkmklt)
        {
            try
            {
                await WBrowse.ExecuteScriptAsync(Bkmklt);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this, ex.ToString(), "Execute Script Fails!");
            }
        }

        async void InjectScriptl(string Scriptpath)
        {
            try
            {
                string scrl = File.ReadAllText(Scriptpath);
                await WBrowse.ExecuteScriptAsync(scrl);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this, ex.ToString(), "Execute Script Fails!");
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
                if (KeywordMap_Txt.Text == string.Empty)
                    KeywordMap_Txt.Text = "Here";

                VerifySizeZoom();
                VerifMapOpn = "on";
                GmapProviderSelect(provid);
                GMap_Ctrl.Manager.Mode = AccessMode.ServerOnly;
                GMap_Ctrl.SetPositionByKeywords(adress);
                GMap_Ctrl.Zoom = MapZoom;
                GMap_Ctrl.IgnoreMarkerOnMouseWheel = true;
                GMap_Ctrl.Overlays.Add(overlayOne);
                GMap_Ctrl.ShowCenter = true;
                GMap_Ctrl.MouseClick += Gmap_MouseClick;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenMaps: ", ex.ToString(), "Main_Frm", AppStart);
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
                senderror.ErrorLog("Error! NewProject_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Create a new project from a list of points
        /// </summary>
        /// <param name="Vrfy">Checks if the operation is not abandoned</param>
        /// <param value="0">Continue</param>
        /// <param value="1">Return</param>
        /// 
        void NewProjectMapList_Tls_Click(object sender, EventArgs e)
        {
            int model;
            string Btn = (sender as ToolStripMenuItem).Name;
            if (Btn == NewProjectMapList_Tls.Name)
                model = 0;
            else
                model = 1;

            Vrfy = 0;

            CreateProjectMap(1);

            if (Vrfy == 1)
                return;

            string fileopen = openfile.Fileselect(AppStart, "txt files (*.txt)|*.txt|All files (*.*)|*.*", 2);

            if (fileopen == string.Empty)
                return;

            MessageBox.Show(MessageStartGeoloc);

            Thread AutoCreatePoints = new Thread(() => AutoCreatePoints_Thrd(fileopen, model));
            AutoCreatePoints.Start();
        }
        ///
        /// <summary>Create XML location from a list</summary>
        /// <param name="fileopn">Project create</param>
        /// <param name="values[0]">Point Name</param>
        /// <param name="values[1]">Latitude</param>
        /// <param name="values[2]">Longitude</param>
        /// <param name="values[3]">Marker/Description/Infos</param>
        /// <param name="model">Model csv or other</param>
        /// <param value="0">Name/Latitude/Longitude/Marker</param>
        /// <param value="1">Latitude/Longitude</param>
        /// <param name="Una">Use random name</param>
        /// 
        void AutoCreatePoints_Thrd(string fileopn, int model)
        {
            CreateNameAleat();
            int inct = 0;

            using (var reader = new StreamReader(fileopn))
            {
                while (!reader.EndOfStream)
                {
                    inct += 1;
                    var line = reader.ReadLine();
                    string trimmedLine = line.Trim();
                    if (!string.IsNullOrEmpty(trimmedLine))
                    {
                        var values = line.Split(',');
                        if (model == 0)
                            AddNewLocPoints(values[0], values[1], values[2], values[3]);
                        else
                            AddNewLocPoints(Una + inct, values[0], values[1], Una + inct);
                    }
                }
            }

            MessageBox.Show("Completed.");
        }

        void NewRouteProject_Tls_Click(object sender, EventArgs e)
        {
        SelectName:

            string message, title;
            object NameInsert;

            message = "Select Name Project.";
            title = "Project Name";

            NameInsert = Interaction.InputBox(message, title);
            string ValName = Convert.ToString(NameInsert);

            if (!string.IsNullOrEmpty(ValName))
            {
                if (File.Exists(Path.Combine(MapDir, ValName + ".txt")))
                {
                    string avert = "The file already exists, delete?";
                    string caption = "Ostium";
                    var result = MessageBox.Show(avert, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                        File.Delete(Path.Combine(MapDir, ValName + ".txt"));
                    else
                        goto SelectName;
                }

                GMap_Ctrl.Overlays.Clear();
                overlayOne.Markers.Clear();

                UndoRoutePoint_Btn.Visible = true;
                LocatRoute = "route";
                Map_Cmd_Pnl.Visible = true;
                LocatRoute_Lbl.Text = "Routes";
                TxtMarker_Lbl.Text = "Distance (Km)";
                TxtMarker_Chk.Enabled = false;
                AddNewLoc_Btn.Visible = false;
                SaveRoute_Btn.Visible = true;
                SaveGPX_Btn.Visible = false;
                PointRoute_Lst.Visible = true;

                using (StreamWriter fc = new StreamWriter(Path.Combine(MapDir, ValName + ".txt")))
                {
                    fc.Write("");
                }

                loadfiledir.LoadFileDirectory(MapDir, "txt", "lst", PointLoc_Lst);

                MapRouteOpn = MapDir + ValName + ".txt";

                PointRoute_Lst.Items.Clear();
                PointRoute_Lst.Items.AddRange(File.ReadAllLines(MapRouteOpn));

                ProjectMapOpn_Lbl.Text = $"Project open: {ValName}.txt";
            }
            else
            {
                return;
            }
        }

        void CreateProjectMap(int val)
        {
        SelectName:

            string message, title;
            object NameInsert;

            message = "Select Name Project.";
            title = "Project Name";

            NameInsert = Interaction.InputBox(message, title);
            string ValName = Convert.ToString(NameInsert);

            if (!string.IsNullOrEmpty(ValName))
            {
                if (File.Exists(Path.Combine(MapDir, ValName + ".xml")))
                {
                    string avert = "The file already exists, delete?";
                    string caption = "Ostium";
                    var result = MessageBox.Show(avert, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                        File.Delete(Path.Combine(MapDir, ValName + ".xml"));
                    else
                    {
                        Vrfy = 1;
                        goto SelectName;
                    }
                }

                XmlTextWriter writer = new XmlTextWriter(Path.Combine(MapDir, ValName + ".xml"), Encoding.UTF8);
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

                UndoRoutePoint_Btn.Visible = false;
                MapRouteOpn = string.Empty;
                LocatRoute = "locat";
                Map_Cmd_Pnl.Visible = true;
                LocatRoute_Lbl.Text = "Location Points";
                TxtMarker_Lbl.Text = "Text Marker";
                TextMarker_Txt.Text = string.Empty;
                TxtMarker_Chk.Enabled = true;
                AddNewLoc_Btn.Visible = true;
                SaveRoute_Btn.Visible = false;
                SaveGPX_Btn.Visible = false;
                SaveRoute_Btn.Text = "Save route Off";
                SaveRoute_Btn.ForeColor = Color.White;
                PointRoute_Lst.Visible = false;

                loadfiledir.LoadFileDirectory(MapDir, "xml", "lst", PointLoc_Lst);

                ProjectMapOpn_Lbl.Text = $"Project open: {ValName}.xml";
                MapXmlOpn = MapDir + ValName + ".xml";
            }
            else
            {
                Vrfy = 1;
            }
        }

        void OpnDirMap_Tls_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(MapDir))
                Process.Start(MapDir);
        }

        void EditXMLMap_Tls_Click(object sender, EventArgs e)
        {
            if (MapXmlOpn == string.Empty && MapRouteOpn == string.Empty)
            {
                MessageBox.Show("No project selected! Select one.");
                return;
            }

            if (LocatRoute == "locat")
                OpnFileOpt(MapXmlOpn);
            else
                OpnFileOpt(MapRouteOpn);
        }

        void ShowXMLMap_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                int vrfy = 0;
                if (MapXmlOpn == string.Empty && MapRouteOpn == string.Empty)
                {
                    MessageBox.Show("No project selected! Select one.");
                    return;
                }

                string strExt = Path.GetExtension(MapRouteOpn);
                if (strExt == ".kml")
                    return;

                if (LocatRoute == "locat")
                {
                    if (File.Exists(MapXmlOpn))
                    {
                        vrfy = 1;
                        WBrowse.Source = new Uri(MapXmlOpn);
                    }
                }
                else
                {
                    if (File.Exists(MapRouteOpn))
                    {
                        vrfy = 1;
                        WBrowse.Source = new Uri(MapRouteOpn);
                    }
                }

                if (vrfy == 1)
                {
                    CtrlTabBrowsx();
                    Control_Tab.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! ShowXMLMap_Tls_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void ExportGPX_Tls_Click(object sender, EventArgs e)
        {
            string strExt = Path.GetExtension(MapRouteOpn);
            if (strExt != ".txt")
                return;

            string dirselect = selectdir.Dirselect();
            if (dirselect != string.Empty)
            {
                string outputFile = Path.Combine(dirselect, Path.GetFileNameWithoutExtension(MapRouteOpn) + ".gpx");

                if (File.Exists(outputFile))
                {
                    string avert = "The file already exists, delete?";
                    string caption = "Ostium";
                    var result = MessageBox.Show(avert, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                        File.Delete(outputFile);
                    else
                        return;
                }

                CreateGpxFromCoordinates(MapRouteOpn, outputFile);
            }
        }

        void DelProjectMap_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapXmlOpn == string.Empty && MapRouteOpn == string.Empty)
                {
                    MessageBox.Show("No project selected! Select one.");
                    return;
                }

                if (PointLoc_Lst.SelectedIndex != -1)
                {
                    string message = $"Do you want to delete the project? {PointLoc_Lst.SelectedItem}";
                    string caption = "Delete Projet";
                    var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (LocatRoute == "routegpx")
                        {
                            if (File.Exists(Path.Combine(MapDirGpx, PointLoc_Lst.SelectedItem.ToString())))
                                File.Delete(Path.Combine(MapDirGpx, PointLoc_Lst.SelectedItem.ToString()));
                        }
                        else
                        {
                            if (File.Exists(Path.Combine(MapDir, PointLoc_Lst.SelectedItem.ToString())))
                                File.Delete(Path.Combine(MapDir, PointLoc_Lst.SelectedItem.ToString()));
                        }

                        if (LocatRoute == "locat")
                            loadfiledir.LoadFileDirectory(MapDir, "xml", "lst", PointLoc_Lst);
                        else if (LocatRoute == "route")
                            loadfiledir.LoadFileDirectory(MapDir, "txt", "lst", PointLoc_Lst);
                        else if (LocatRoute == "routegpx")
                            loadfiledir.LoadFileDirectory(MapDirGpx, "*.*", "lst", PointLoc_Lst);

                        ProjectMapOpn_Lbl.Text = string.Empty;

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
                senderror.ErrorLog("Error! DeleteProjectMap_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpnListLocation_Tls_Click(object sender, EventArgs e)
        {
            if (!Map_Cmd_Pnl.Visible || Map_Cmd_Pnl.Visible && LocatRoute == "route" || Map_Cmd_Pnl.Visible && LocatRoute == "routegpx")
            {
                UndoRoutePoint_Btn.Visible = false;
                MapRouteOpn = string.Empty;
                LocatRoute = "locat";
                Map_Cmd_Pnl.Visible = true;
                LocatRoute_Lbl.Text = "Location Points";
                TxtMarker_Lbl.Text = "Text Marker";
                TextMarker_Txt.Text = string.Empty;
                TxtMarker_Chk.Enabled = true;
                AddNewLoc_Btn.Visible = true;
                SaveGPX_Btn.Visible = false;
                SaveRoute_Btn.Visible = false;
                SaveRoute_Btn.Text = "Save route Off";
                SaveRoute_Btn.ForeColor = Color.White;
                PointRoute_Lst.Visible = false;

                loadfiledir.LoadFileDirectory(MapDir, "xml", "lst", PointLoc_Lst);
            }
            else
            {
                UndoRoutePoint_Btn.Visible = false;
                Map_Cmd_Pnl.Visible = false;
            }
        }

        void OpnListRoute_Tls_Click(object sender, EventArgs e)
        {
            if (!Map_Cmd_Pnl.Visible || Map_Cmd_Pnl.Visible && LocatRoute == "locat" || Map_Cmd_Pnl.Visible && LocatRoute == "routegpx")
            {
                UndoRoutePoint_Btn.Visible = true;
                LocatRoute = "route";
                Map_Cmd_Pnl.Visible = true;
                LocatRoute_Lbl.Text = "Routes";
                TxtMarker_Lbl.Text = "Distance (Km)";
                TxtMarker_Chk.Enabled = false;
                AddNewLoc_Btn.Visible = false;
                SaveRoute_Btn.Visible = true;
                SaveGPX_Btn.Visible = false;
                PointRoute_Lst.Visible = true;

                loadfiledir.LoadFileDirectory(MapDir, "txt", "lst", PointLoc_Lst);
            }
            else
            {
                UndoRoutePoint_Btn.Visible = false;
                Map_Cmd_Pnl.Visible = false;
            }
        }

        void OpnListRouteGpx_Tls_Click(object sender, EventArgs e)
        {
            if (!Map_Cmd_Pnl.Visible || Map_Cmd_Pnl.Visible && LocatRoute == "locat" || Map_Cmd_Pnl.Visible && LocatRoute == "route")
            {
                KmlGpxOpn = "off";
                SaveGPX_Btn.Visible = false;
                RouteListGPX();
            }
            else
            {
                UndoRoutePoint_Btn.Visible = false;
                Map_Cmd_Pnl.Visible = false;
            }
        }

        void RouteListGPX()
        {
            if (!Directory.Exists(MapDirGpx))
                Directory.CreateDirectory(MapDirGpx);

            UndoRoutePoint_Btn.Visible = false;
            LocatRoute = "routegpx";
            Map_Cmd_Pnl.Visible = true;
            LocatRoute_Lbl.Text = "Routes";
            TxtMarker_Lbl.Text = "Distance (Km)";
            TxtMarker_Chk.Enabled = false;
            AddNewLoc_Btn.Visible = false;
            SaveRoute_Btn.Visible = false;
            SaveRoute_Btn.Text = "Save route Off";
            if (KmlGpxOpn == "on")
            {
                SaveGPX_Btn.Visible = true;
            }

            PointRoute_Lst.Visible = false;

            loadfiledir.LoadFileDirectory(MapDirGpx, "*", "lst", PointLoc_Lst);
        }

        void OpnGPXRoute_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                string fileopen = openfile.Fileselect(AppStart, "kml gpx files (*.gpx;*.kml;*geojson;*json)|*.gpx;*.kml;*geojson;*json", 2);
                string strname = string.Empty;

                if (fileopen != string.Empty)
                {
                    GMap_Ctrl.Overlays.Clear();
                    overlayOne.Markers.Clear();

                    string strExt = Path.GetExtension(fileopen);
                    strname = Path.GetFileName(fileopen);

                    if (strExt == ".kml")
                    {
                        LoadKmlFile(fileopen);
                    }
                    else if (strExt == ".gpx")
                    {
                        LoadGpxFile(fileopen);
                    }
                    else if (strExt == ".geojson" || strExt == ".json")
                    {
                        LoadGeoJsonFile(fileopen);
                    }

                    UndoRoutePoint_Btn.Visible = false;
                    LocatRoute = "routegpx";
                    KmlGpxOpn = "on";
                    SaveRoute_Btn.Visible = false;
                    AddNewLoc_Btn.Visible = false;
                    SaveGPX_Btn.Visible = true;
                    SaveRoute_Btn.Text = "Save route Off";
                    SaveRoute_Btn.ForeColor = Color.White;

                    MapRouteOpn = fileopen;

                    RouteListGPX();

                    ProjectMapOpn_Lbl.Text = $"File open: {strname}";
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnGPXRoute_Tls_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void CrossCenter_Tls_Click(object sender, EventArgs e)
        {
            GMap_Ctrl.ShowCenter = !GMap_Ctrl.ShowCenter;

            if (CrossCenter == "on")
                CrossCenter = "off";
            else
                CrossCenter = "on";
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
                senderror.ErrorLog("Error! ScreenShotGmap_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpnBingMap_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                GoBrowser(lstUrlDfltCnf[9].ToString() + LatTCurrent_Lbl.Text + "~" + LonGtCurrent_Lbl.Text, 0);
                CtrlTabBrowsx();
                Control_Tab.SelectedIndex = 0;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                    "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (ArgumentException ex)
            {
                senderror.ErrorLog("Error! OpnBingMap_Tls_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpnGoogleMaps_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                GoBrowser(lstUrlDfltCnf[7].ToString() + LatTCurrent_Lbl.Text + "%2C" + LonGtCurrent_Lbl.Text, 0);
                CtrlTabBrowsx();
                Control_Tab.SelectedIndex = 0;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                    "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (ArgumentException ex)
            {
                senderror.ErrorLog("Error! OpnGoogleMaps_Tls_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void OpnGoogleStreet_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                GoBrowser(lstUrlDfltCnf[8].ToString() + LatTCurrent_Lbl.Text + "%2C" + LonGtCurrent_Lbl.Text, 0);
                CtrlTabBrowsx();
                Control_Tab.SelectedIndex = 0;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                    "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (ArgumentException ex)
            {
                senderror.ErrorLog("Error! OpnGoogleMaps_Tls_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
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
                senderror.ErrorLog("Error! OpenGoogleEarth_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void CopyGeoMap_Tls_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, LatTCurrent_Lbl.Text + " " + LonGtCurrent_Lbl.Text);
            Console.Beep(1000, 400);
        }

        void NegativeModeMap_Tls_Click(object sender, EventArgs e)
        {
            GMap_Ctrl.NegativeMode = !GMap_Ctrl.NegativeMode;
        }

        void ClearMap_Tls_Click(object sender, EventArgs e)
        {
            GMap_Ctrl.Overlays.Clear();
            overlayOne.Markers.Clear();
            GMap_Ctrl.Refresh();
        }

        void GoLatLong_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                if (LatLon_Txt.Text == string.Empty)
                {
                    LatLon_Txt.BackColor = Color.Red;
                    MessageBox.Show("False coordinates!");
                    LatLon_Txt.BackColor = Color.FromArgb(28, 28, 28);
                    return;
                }

                string LaT = string.Empty;
                string LoN = string.Empty;
                string stn = LatLon_Txt.Text;

                string[] words = stn.Split();

                for (int i = 0; i < words.Length; i++)
                {
                    if (OrderLL_txt.Text == "lalo")
                    {
                        LaT = words[0]; LoN = words[1];
                    }
                    else if (OrderLL_txt.Text == "lola")
                    {
                        LaT = words[1]; LoN = words[0];
                    }
                    else
                    {
                        MessageBox.Show("Select the correct order for Latitude and Longitude! lalo or lola.");
                    }
                }

                LatT = double.Parse(LaT, CultureInfo.InvariantCulture);
                LonGt = double.Parse(LoN, CultureInfo.InvariantCulture);
                GMap_Ctrl.Position = new PointLatLng(LatT, LonGt);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! GoLatLong_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void GoWord_Tls_Click(object sender, EventArgs e)
        {
            if (KeywordMap_Txt.Text == string.Empty)
            {
                KeywordMap_Txt.BackColor = Color.Red;
                MessageBox.Show("Insert keyword!");
                KeywordMap_Txt.BackColor = Color.FromArgb(28, 28, 28);
                return;
            }

            OpenMaps(KeywordMap_Txt.Text, 12); // Adresse, Provider
        }

        void AddNewLoc_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapXmlOpn == string.Empty)
                {
                    MessageBox.Show("No project selected! Select one or create one.");
                    return;
                }

                CreateNameAleat();
                LocationName_Txt.Text = "Pts_" + Una;
                AddNewLocPoints(LocationName_Txt.Text, LatTCurrent_Lbl.Text, LonGtCurrent_Lbl.Text, TextMarker_Txt.Text);
                OpnLocationPoints();
                Console.Beep(1000, 400);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddNewLoc_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void AddNewLocPoints(string locationname, string lat, string lon, string txtmarker)
        {
            if (lat == string.Empty || lon == string.Empty)
                return;

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

        void SaveRoute_Btn_Click(object sender, EventArgs e)
        {
            bool isSaveRouteOn = SaveRoute_Btn.Text == "Save route Off";

            if (!isSaveRouteOn)
            {
                SaveRoute_Btn.Text = "Save route Off";
                SaveRoute_Btn.ForeColor = Color.White;
            }
            else
            {
                SaveRoute_Btn.Text = "Save route On";
                SaveRoute_Btn.ForeColor = Color.Red;
            }
        }

        void SaveGPX_Btn_Click(object sender, EventArgs e)
        {
        SelectName:

            if (!Directory.Exists(MapDirGpx))
                Directory.CreateDirectory(MapDirGpx);

            string strExt = Path.GetExtension(MapRouteOpn);

            string message, title;
            object NameInsert;

            message = "Select Name File.";
            title = "File Name";

            NameInsert = Interaction.InputBox(message, title);
            string ValName = Convert.ToString(NameInsert);

            if (ValName != string.Empty)
            {
                if (File.Exists(Path.Combine(MapDirGpx, ValName, strExt)))
                {
                    string avert = "The file already exists, delete?";
                    string caption = "Ostium";
                    var result = MessageBox.Show(avert, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                        File.Delete(Path.Combine(MapDirGpx, ValName, strExt));
                    else
                        goto SelectName;
                }

                File.Copy(MapRouteOpn, Path.Combine(MapDirGpx, ValName + strExt));

                SaveRoute_Btn.Visible = false;
                SaveGPX_Btn.Visible = false;
                AddNewLoc_Btn.Visible = false;

                LocatRoute = "routegpx";

                loadfiledir.LoadFileDirectory(MapDirGpx, "*.*", "lst", PointLoc_Lst);
            }
            else
            {
                return;
            }
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
                    GMap_Ctrl.MapProvider = GMapProviders.BingHybridMap;
                    break;
                case 1:
                    GMap_Ctrl.MapProvider = GMapProviders.BingMap;
                    break;
                case 2:
                    GMap_Ctrl.MapProvider = GMapProviders.BingOSMap;
                    break;
                case 3:
                    GMap_Ctrl.MapProvider = GMapProviders.BingSatelliteMap;
                    break;
                case 4:
                    GMap_Ctrl.MapProvider = GMapProviders.GoogleMap;
                    break;
                case 5:
                    GMap_Ctrl.MapProvider = GMapProviders.GoogleSatelliteMap;
                    break;
                case 6:
                    GMap_Ctrl.MapProvider = GMapProviders.GoogleTerrainMap;
                    break;
                case 7:
                    GMap_Ctrl.MapProvider = GMapProviders.OpenCycleLandscapeMap;
                    break;
                case 8:
                    GMap_Ctrl.MapProvider = GMapProviders.OpenCycleMap;
                    break;
                case 9:
                    GMap_Ctrl.MapProvider = GMapProviders.OpenCycleTransportMap;
                    break;
                case 10:
                    GMap_Ctrl.MapProvider = GMapProviders.OpenSeaMapHybrid;
                    break;
                case 11:
                    GMap_Ctrl.MapProvider = GMapProviders.OpenStreet4UMap;
                    break;
                case 12:
                    GMap_Ctrl.MapProvider = GMapProviders.OpenStreetMap;
                    break;
                case 13:
                    GMap_Ctrl.MapProvider = GMapProviders.WikiMapiaMap;
                    break;
                case 14:
                    GMap_Ctrl.MapProvider = GMapProviders.CzechGeographicMap;
                    break;
                case 15:
                    GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_Imagery_World_2D_Map;
                    break;
                case 16:
                    GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_StreetMap_World_2D_Map;
                    break;
                case 17:
                    GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_World_Physical_Map;
                    break;
                case 18:
                    GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_World_Shaded_Relief_Map;
                    break;
                case 19:
                    GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_World_Street_Map;
                    break;
                case 20:
                    GMap_Ctrl.MapProvider = GMapProviders.ArcGIS_World_Terrain_Base_Map;
                    break;
                default:
                    GMap_Ctrl.MapProvider = GMapProviders.BingMap;
                    break;
            }
        }

        void GoLatLong(string lat, string lon, string txtmarker)
        {
            try
            {
                if (txtmarker == string.Empty)
                    txtmarker = "Here";

                VerifySizeZoom();

                LatT = double.Parse(lat, CultureInfo.InvariantCulture);
                LonGt = double.Parse(lon, CultureInfo.InvariantCulture);

                GMapOverlay markers = new GMapOverlay("markers");
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(LatT, LonGt), Mkmarker)
                {
                    ToolTipMode = MarkerTooltipMode.Always,
                    ToolTipText = "\r\n" + txtmarker
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
                senderror.ErrorLog("Error! GoLatLong: ", ex.ToString(), "Main_Frm", AppStart);
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

                string msg = "The project no longer exists! It will be removed from the list.";

                if (LocatRoute == "locat")
                {
                    Mkmarker = GMarkerGoogleType.blue;
                    MapXmlOpn = MapDir + PointLoc_Lst.SelectedItem.ToString();

                    if (!File.Exists(MapXmlOpn))
                    {
                        MessageBox.Show(msg);
                        loadfiledir.LoadFileDirectory(MapDir, "xml", "lst", PointLoc_Lst);
                    }
                    else
                    {
                        AddNewLoc_Btn.Visible = true;
                        SaveRoute_Btn.Visible = false;
                        SaveGPX_Btn.Visible = false;

                        OpnLocationPoints();
                    }
                }
                else if (LocatRoute == "route")
                {
                    MapRouteOpn = MapDir + PointLoc_Lst.SelectedItem.ToString();

                    if (!File.Exists(MapRouteOpn))
                    {
                        MessageBox.Show(msg);
                        loadfiledir.LoadFileDirectory(MapDir, "txt", "lst", PointLoc_Lst);
                    }
                    else
                    {
                        KmlGpxOpn = "off";
                        SaveRoute_Btn.Visible = true;
                        SaveGPX_Btn.Visible = false;
                        AddNewLoc_Btn.Visible = false;

                        history.Clear();
                        PointRoute_Lst.Items.Clear();
                        PointRoute_Lst.Items.AddRange(File.ReadAllLines(MapRouteOpn));
                        LoadRouteFromFile(MapRouteOpn);
                    }
                }
                else if (LocatRoute == "routegpx")
                {
                    MapRouteOpn = MapDirGpx + PointLoc_Lst.SelectedItem.ToString();

                    string strExt = Path.GetExtension(MapRouteOpn);

                    if (!File.Exists(MapRouteOpn))
                    {
                        MessageBox.Show(msg);
                        loadfiledir.LoadFileDirectory(MapDirGpx, "*.*", "lst", PointLoc_Lst);
                    }
                    else
                    {
                        KmlGpxOpn = "off";
                        SaveRoute_Btn.Visible = false;
                        SaveGPX_Btn.Visible = false;
                        AddNewLoc_Btn.Visible = false;

                        if (strExt == ".gpx")
                            LoadGpxFile(MapRouteOpn);
                        else if (strExt == ".kml")
                            LoadKmlFile(MapRouteOpn);
                        else if (strExt == ".geojson" || strExt == ".json")
                            LoadGeoJsonFile(MapRouteOpn);
                    }
                }

                ProjectMapOpn_Lbl.Text = $"Project open: {PointLoc_Lst.SelectedItem}";
            }
        }

        void PointRoute_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PointRoute_Lst.SelectedIndex != -1)
            {
                string LaT = string.Empty;
                string LoN = string.Empty;
                string stn = PointRoute_Lst.SelectedItem.ToString();

                char[] charsToTrim = { ',' };
                string[] words = stn.Split();

                for (int i = 0; i < words.Length; i++)
                {
                    LaT = words[0].TrimEnd(charsToTrim); LoN = words[1].TrimEnd(charsToTrim);
                }

                LatT = double.Parse(LaT, CultureInfo.InvariantCulture);
                LonGt = double.Parse(LoN, CultureInfo.InvariantCulture);
                GMap_Ctrl.Position = new PointLatLng(LatT, LonGt);
            }
        }

        void OpnLocationPoints()
        {
            try
            {
                string lat = string.Empty;
                string lon = string.Empty;
                string txtmarker = string.Empty;

                using (XmlReader reader = XmlReader.Create(MapXmlOpn))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Point_Point")
                        {
                            lat = reader.GetAttribute("latitude");
                            lon = reader.GetAttribute("longitude");
                            txtmarker = reader.GetAttribute("textmarker");

                            GoLatLong(lat, lon, txtmarker);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lon))
                {
                    if (double.TryParse(lat, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedLat) &&
                        double.TryParse(lon, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedLon))
                    {
                        LatT = parsedLat;
                        LonGt = parsedLon;
                        GMap_Ctrl.Position = new PointLatLng(LatT, LonGt);
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnLocationPoints: ", ex.ToString(), "Main_Frm", AppStart);
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

        void EgHelp_Tls_Click(object sender, EventArgs e)
        {
            Open_Doc_Frm(Path.Combine(FileDir, "map_points.txt"));
        }

        void Gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (SaveRoute_Btn.Text == "Save route On")
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (MapRouteOpn == string.Empty)
                    {
                        MessageBox.Show("No route project selected! Select one or create one.");
                        return;
                    }

                    PointLatLng point = GMap_Ctrl.FromLocalToLatLng(e.X, e.Y);
                    using (StreamWriter fc = File.AppendText(MapRouteOpn))
                    {
                        fc.WriteLine($"{point.Lat.ToString(CultureInfo.InvariantCulture)}, {point.Lng.ToString(CultureInfo.InvariantCulture)}");
                    }
                    history.Push($"{point.Lat.ToString(CultureInfo.InvariantCulture)}, {point.Lng.ToString(CultureInfo.InvariantCulture)}");

                    PointRoute_Lst.Items.Clear();
                    PointRoute_Lst.Items.AddRange(File.ReadAllLines(MapRouteOpn));
                    LoadRouteFromFile(MapRouteOpn);
                }
            }
        }

        void LoadRouteFromFile(string filePath)
        {
            GMapOverlay routes = new GMapOverlay("routes");
            List<PointLatLng> points = new List<PointLatLng>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (!string.IsNullOrEmpty(trimmedLine))
                    {
                        string[] coordinates = line.Split(',');
                        LatT = double.Parse(coordinates[0], CultureInfo.InvariantCulture);
                        LonGt = double.Parse(coordinates[1], CultureInfo.InvariantCulture);
                        points.Add(new PointLatLng(LatT, LonGt));
                    }
                }

                GMapRoute route = new GMapRoute(points, "A walk")
                {
                    Stroke = new Pen(Color.Red, 3)
                };
                routes.Routes.Add(route);
                GMap_Ctrl.Overlays.Add(routes);
                GMap_Ctrl.Position = new PointLatLng(LatT, LonGt);

                TextMarker_Txt.Text = Convert.ToString(route.Distance);
            }
            catch (FormatException)
            {
                MessageBox.Show("Format exception!");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadRouteFromFile: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void LoadKmlFile(string filePath)
        {
            try
            {
                XDocument kml = XDocument.Load(filePath);
                XNamespace ns = kml.Root.GetDefaultNamespace();

                var overlay = new GMapOverlay("kml_overlay");

                foreach (var placemark in kml.Descendants(ns + "Placemark"))
                {
                    var name = placemark.Element(ns + "name")?.Value;
                    var coordinates = placemark.Descendants(ns + "coordinates").FirstOrDefault()?.Value;

                    if (coordinates != null)
                    {
                        var points = coordinates.Split(' ')
                            .Select(c => c.Split(','))
                            .Where(c => c.Length >= 2)
                            .Select(c => new PointLatLng(double.Parse(c[1], CultureInfo.InvariantCulture), double.Parse(c[0], CultureInfo.InvariantCulture)))
                            .ToList();

                        if (points.Count > 1)
                        {
                            var route = new GMapRoute(points, name)
                            {
                                Stroke = new Pen(Color.Red, 3)
                            };
                            overlay.Routes.Add(route);

                            TextMarker_Txt.Text = Convert.ToString(route.Distance);
                        }
                        else if (points.Count == 1)
                        {
                            var marker = new GMarkerGoogle(points[0], GMarkerGoogleType.red_dot)
                            {
                                ToolTipText = name
                            };
                            overlay.Markers.Add(marker);
                        }
                    }
                }

                GMap_Ctrl.Overlays.Add(overlay);
                GMap_Ctrl.ZoomAndCenterRoutes("kml_overlay");
            }
            catch (FormatException)
            {
                MessageBox.Show("Format exception!");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadKmlFile: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void LoadGpxFile(string filePath)
        {
            try
            {
                XDocument gpx = XDocument.Load(filePath);
                XNamespace ns = gpx.Root.GetDefaultNamespace();

                var route = new GMapRoute("gpx_route");
                var overlay = new GMapOverlay("gpx_overlay");

                foreach (var trkpt in gpx.Descendants(ns + "trkpt"))
                {
                    double lat = double.Parse(trkpt.Attribute("lat").Value, CultureInfo.InvariantCulture);
                    double lon = double.Parse(trkpt.Attribute("lon").Value, CultureInfo.InvariantCulture);
                    route.Points.Add(new PointLatLng(lat, lon));
                }

                route.Stroke = new Pen(Color.Red, 3);
                overlay.Routes.Add(route);
                GMap_Ctrl.Overlays.Add(overlay);

                GMap_Ctrl.ZoomAndCenterRoutes("gpx_overlay");

                TextMarker_Txt.Text = Convert.ToString(route.Distance);
            }
            catch (FormatException)
            {
                MessageBox.Show("Format exception!");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadGpxFile: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void LoadGeoJsonFile(string filePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                JObject geoJson = JObject.Parse(jsonContent);

                if (geoJson["features"] is JArray features)
                {
                    GMapOverlay overlay = new GMapOverlay("geojson");

                    foreach (var feature in features)
                    {
                        var geometry = feature["geometry"];
                        string geometryType = geometry["type"].ToString();

                        switch (geometryType)
                        {
                            case "Point":
                                AddPoint(overlay, geometry);
                                break;
                            case "LineString":
                                AddLineString(overlay, geometry);
                                break;
                            case "Polygon":
                                AddPolygon(overlay, geometry);
                                break;
                            case "MultiPolygon":
                                AddMultiPolygon(overlay, geometry);
                                break;
                        }
                    }

                    GMap_Ctrl.Overlays.Add(overlay);
                }

                GMap_Ctrl.ZoomAndCenterMarkers("geojson");
            }
            catch (FormatException)
            {
                MessageBox.Show("Format exception!");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadGeoJsonFile: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void AddPoint(GMapOverlay overlay, JToken geometry)
        {
            var coordinates = geometry["coordinates"];
            double lon = coordinates[0].Value<double>();
            double lat = coordinates[1].Value<double>();
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lon), GMarkerGoogleType.red_dot);
            overlay.Markers.Add(marker);

            TextMarker_Txt.Text = string.Empty;
        }

        void AddLineString(GMapOverlay overlay, JToken geometry)
        {
            var coordinates = geometry["coordinates"] as JArray;
            List<PointLatLng> points = new List<PointLatLng>();
            foreach (var coord in coordinates)
            {
                double lon = coord[0].Value<double>();
                double lat = coord[1].Value<double>();
                points.Add(new PointLatLng(lat, lon));
            }
            GMapRoute route = new GMapRoute(points, "LineString")
            {
                Stroke = new Pen(Color.Red, 2)
            };
            overlay.Routes.Add(route);

            TextMarker_Txt.Text = Convert.ToString(route.Distance);
        }

        void AddPolygon(GMapOverlay overlay, JToken geometry)
        {
            var coordinates = geometry["coordinates"][0] as JArray;
            List<PointLatLng> points = new List<PointLatLng>();
            foreach (var coord in coordinates)
            {
                double lon = coord[0].Value<double>();
                double lat = coord[1].Value<double>();
                points.Add(new PointLatLng(lat, lon));
            }
            GMapPolygon polygon = new GMapPolygon(points, "Polygon")
            {
                Fill = new SolidBrush(Color.FromArgb(50, Color.Red)),
                Stroke = new Pen(Color.Red, 1)
            };
            overlay.Polygons.Add(polygon);

            TextMarker_Txt.Text = string.Empty;
        }

        void AddMultiPolygon(GMapOverlay overlay, JToken geometry)
        {
            var polygons = geometry["coordinates"] as JArray;
            foreach (var polygonCoords in polygons)
            {
                var coordinates = polygonCoords[0] as JArray;
                List<PointLatLng> points = new List<PointLatLng>();
                foreach (var coord in coordinates)
                {
                    double lon = coord[0].Value<double>();
                    double lat = coord[1].Value<double>();
                    points.Add(new PointLatLng(lat, lon));
                }
                GMapPolygon polygon = new GMapPolygon(points, "MultiPolygon")
                {
                    Fill = new SolidBrush(Color.FromArgb(50, Color.Blue)),
                    Stroke = new Pen(Color.Blue, 1)
                };
                overlay.Polygons.Add(polygon);

                TextMarker_Txt.Text = string.Empty;
            }
        }

        void CreateGpxFromCoordinates(string inputFile, string outputFile)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true
                };

                using (XmlWriter writer = XmlWriter.Create(outputFile, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");
                    writer.WriteAttributeString("version", "1.1");
                    writer.WriteAttributeString("creator", "Ostium Osint Browser GPX Generator");

                    writer.WriteStartElement("trk");
                    writer.WriteElementString("name", "Generated Track");

                    writer.WriteStartElement("trkseg");

                    string[] lines = File.ReadAllLines(inputFile);
                    foreach (string line in lines)
                    {
                        string[] coordinates = line.Split(',');
                        if (coordinates.Length == 2)
                        {
                            writer.WriteStartElement("trkpt");
                            writer.WriteAttributeString("lat", coordinates[0].ToString());
                            writer.WriteAttributeString("lon", coordinates[1].ToString());
                            writer.WriteEndElement(); // trkpt
                        }
                    }

                    writer.WriteEndElement(); // trkseg
                    writer.WriteEndElement(); // trk
                    writer.WriteEndElement(); // gpx
                    writer.WriteEndDocument();
                }

                MessageBox.Show("GPX file created successfully.");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateGpxFromCoordinates: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void UndoRoutePoint_Btn_Click(object sender, EventArgs e)
        {
            if (history.Count > 0)
            {
                string lastItem = history.Pop();
                PointRoute_Lst.Items.Remove(lastItem);

                GMap_Ctrl.Overlays.Clear();
                overlayOne.Markers.Clear();

                using (StreamWriter SW = new StreamWriter(MapRouteOpn, false))
                {
                    foreach (string itm in PointRoute_Lst.Items)
                    {
                        if (!string.IsNullOrEmpty(itm?.ToString()))
                        {
                            SW.WriteLine(itm);
                        }
                    }
                }

                PointRoute_Lst.Items.Clear();
                PointRoute_Lst.Items.AddRange(File.ReadAllLines(MapRouteOpn));

                LoadRouteFromFile(MapRouteOpn);
            }
        }

        #endregion

        void TtsButton_Sts_ButtonClick(object sender, EventArgs e)
        {
            GoBrowser(URLtxt_txt.Text, 1);
        }

        void TtsButton_Sts_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string scriptEx = e.ClickedItem.Text;
            InjectScriptl(Path.Combine(Scripts, "scriptsl", scriptEx));
        }

        void FloodHeader_Chk_CheckedChanged(object sender, EventArgs e)
        {
            if (FloodHeader_Chk.Checked)
            {
                @Class_Var.FLOOD_HEADER = 1;
                FloodHeader_Chk.ForeColor = Color.Red;

                MessageBox.Show("The method used against fingerprinting is aggressive; website functionality is disrupted, " +
                    "and you are flagged as a robot.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (!IsParentLinkEnabled)
                {
                    IsParentLinkEnabled = !IsParentLinkEnabled;
                    ForceLinkParent_Btn.ForeColor = Color.Red;
                }
            }
            else
            {
                @Class_Var.FLOOD_HEADER = 0;
                FloodHeader_Chk.ForeColor = Color.White;
            }
        }

        void Limitsize_Chk_CheckedChanged(object sender, EventArgs e)
        {
            if (Limitsize_Chk.Checked)
                Limitsize_Chk.ForeColor = Color.Yellow;
            else
                Limitsize_Chk.ForeColor = Color.White;
        }

        #region Json_

        void JsonOpnFile_Btn_Click(object sender, EventArgs e)
        {
            string fileopen = openfile.Fileselect(AppStart, "json files (*.json)|*.json|All files (*.*)|*.*", 2);
            FileOpnJson = fileopen;

            if (fileopen != string.Empty)
            {
                FileOpnJson_Lbl.Text = "File open: " + Path.GetFileName(fileopen);

                if (OutJsonA_Chk.Checked)
                {
                    File.Copy(fileopen, JsonA, true);
                    Uri uri = new Uri("file:///" + JsonA);
                    WbOutA.Source = uri;
                }
                else
                {
                    File.Copy(fileopen, JsonB, true);
                    Uri uri = new Uri("file:///" + JsonB);
                    WbOutB.Source = uri;
                }
                Console.Beep(1000, 400);
            }
        }

        void JsonSaveFile_Btn_Click(object sender, EventArgs e)
        {
            string Jselect;
            if (OutJsonA_Chk.Checked)
                Jselect = JsonA;
            else
                Jselect = JsonB;

            SavefileShowDiag(Jselect, "json files (*.json)|*.json");
        }

        void JsonSaveUri_Btn_Click(object sender, EventArgs e)
        {
            if (JsonUri_Txt.Text != string.Empty)
            {
                CreateData(JsonDir + "list-url-json.txt", JsonUri_Txt.Text);
                Console.Beep(1200, 200);
            }
        }

        void JsonOpnListUri_Btn_Click(object sender, EventArgs e)
        {
            if (File.Exists(Path.Combine(JsonDir, "list-url-json.txt")))
                Open_Source_Frm(Path.Combine(JsonDir, "list-url-json.txt"));
        }

        void JsonSaveData_Btn_Click(object sender, EventArgs e)
        {
            string Jselect;
            if (OutJsonA_Chk.Checked)
                Jselect = JsonA;
            else
                Jselect = JsonB;

            SavefileShowDiag(Jselect, "files (*.*)|*.*");
        }

        void GetJson_Btn_Click(object sender, EventArgs e)
        {
            if (JsonUri_Txt.Text == string.Empty)
            {
                JsonUri_Txt.BackColor = Color.Red;
                MessageBox.Show("Insert valid URL!");
                JsonUri_Txt.BackColor = Color.Black;
                return;
            }

            if (Class_Var.URL_USER_AGENT_SRC_PAGE == string.Empty)
            {
                try
                {
                    if (lstUrlDfltCnf.Count > 0)
                        Class_Var.URL_USER_AGENT_SRC_PAGE = lstUrlDfltCnf[5].ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                        "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (ArgumentException ex)
                {
                    senderror.ErrorLog("Error! lstUrlDfltCnf GetJson_Btn_Click: ", ex.ToString(), "Main_Frm", AppStart);
                }
            }

            GetAsync(JsonUri_Txt.Text);
        }

        void ParseJson_Btn_Click(object sender, EventArgs e)
        {
            if (JsonVal_Txt.Text == string.Empty)
            {
                JsonVal_Txt.BackColor = Color.Red;
                MessageBox.Show("Insert keyword!");
                JsonVal_Txt.BackColor = Color.Black;
                return;
            }

            string Jselect;
            if (OutJsonA_Chk.Checked)
                Jselect = JsonA;
            else
                Jselect = JsonB;

            Thread ParseVal = new Thread(() => ParseVal_Thrd(JsonVal_Txt.Text, Jselect, CharSpace_Txt.Text));
            ParseVal.Start();
        }

        void ParseNodeJson_Btn_Click(object sender, EventArgs e)
        {
            if (JsonVal_Txt.Text == string.Empty || JsonNode_Txt.Text == string.Empty)
            {
                JsonVal_Txt.BackColor = Color.Red;
                JsonNode_Txt.BackColor = Color.Red;
                MessageBox.Show("Insert keyword!");
                JsonVal_Txt.BackColor = Color.Black;
                JsonNode_Txt.BackColor = Color.Black;
                return;
            }

            string Jselect;
            if (OutJsonA_Chk.Checked)
                Jselect = JsonA;
            else
                Jselect = JsonB;

            Thread ParseNode = new Thread(() => ParseNode_Thrd(JsonVal_Txt.Text, JsonCnt_txt.Text, Jselect, JsonNode_Txt.Text, CharSpace_Txt.Text));
            ParseNode.Start();
        }

        void TableParse_Btn_Click(object sender, EventArgs e)
        {
            if (JsonVal_Txt.Text == string.Empty)
            {
                JsonVal_Txt.BackColor = Color.Red;
                MessageBox.Show("Insert keyword!");
                JsonVal_Txt.BackColor = Color.Black;
                return;
            }

            Thread TableParseVal = new Thread(() => TableParseVal_Thrd());
            TableParseVal.Start();
        }

        void TableNode_Btn_Click(object sender, EventArgs e)
        {
            if (JsonVal_Txt.Text == string.Empty || JsonNode_Txt.Text == string.Empty)
            {
                JsonVal_Txt.BackColor = Color.Red;
                JsonNode_Txt.BackColor = Color.Red;
                MessageBox.Show("Insert keyword!");
                JsonVal_Txt.BackColor = Color.Black;
                JsonNode_Txt.BackColor = Color.Black;
                return;
            }

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
            string Jselect;
            if (OutJsonA_Chk.Checked)
                Jselect = JsonA;
            else
                Jselect = JsonB;

            string txt = File.ReadAllText(Jselect);
            txt = txt.Replace(BrcktA_Txt.Text, BrcktB_Txt.Text);
            File.WriteAllText(Jselect, txt);
            Console.Beep(1200, 200);
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

                string Jselect;
                if (OutJsonA_Chk.Checked)
                    Jselect = JsonA;
                else
                    Jselect = JsonB;

                File_Write(Jselect, $"{jsonResponse}");
                Uri uri = new Uri("file:///" + Jselect);

                if (OutJsonA_Chk.Checked)
                    WbOutA.Source = uri;
                else
                    WbOutB.Source = uri;

                Console.Beep(1000, 400);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!");
            }
        }

        void ParseVal_Thrd(string value, string jsonout, string charspace)
        {
            try
            {
                string xT = value;
                string xO = string.Empty;
                char[] charsToTrim = { ',' };
                string[] words = xT.Split();
                string sendval = string.Empty;
                string OutJs = string.Empty;

                using (StreamReader sr = new StreamReader(jsonout))
                {
                    OutJs = sr.ReadToEnd();
                }
                if (Brkt_Chk.Checked)
                    OutJs = "[" + OutJs + "]";

                JArray jsonVal = JArray.Parse(OutJs);
                dynamic valjson = jsonVal;

                foreach (dynamic val in valjson)
                {
                    foreach (string word in words)
                    {
                        if (ChgCult_Chk.Checked)
                            xO += val[word.TrimEnd(charsToTrim)].ToString(new CultureInfo("us-US")) + charspace;
                        else
                            xO += val[word.TrimEnd(charsToTrim)] + charspace;
                    }

                    sendval += xO + "\r\n";
                    xO = string.Empty;
                }

                Invoke(new Action<string>(ValAdd_Invk), sendval);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!");
            }
        }

        void TableParseVal_Thrd()
        {
            try
            {
                CreateNameAleat();
                StreamWriter fw = new StreamWriter(JsonDirTable + Una + "_table.html");

                string xT = JsonVal_Txt.Text;
                string xO = string.Empty;
                char[] charsToTrim = { ',' };
                string[] words = xT.Split();
                string OutJs = string.Empty;

                string Jselect;
                if (OutJsonA_Chk.Checked)
                    Jselect = JsonA;
                else
                    Jselect = JsonB;

                using (StreamReader sr = new StreamReader(Jselect))
                {
                    OutJs = sr.ReadToEnd();
                }
                if (Brkt_Chk.Checked)
                    OutJs = "[" + OutJs + "]";

                JArray jsonVal = JArray.Parse(OutJs);
                dynamic valjson = jsonVal;

                string t = "<!DOCTYPE html><html lang=\"fr\"><head><meta charset=\"UTF-8\"><title>Ostium Home</title><link rel=\"stylesheet\" " +
                    "href=\"style.css\"></head><body><div class=\"relative overflow-hidden shadow-md rounded-lg\"><table class=\"table-fixed w-full " +
                    "text-left\"><thead class=\"uppercase bg-[#6b7280] text-[#e5e7eb]\" style=\"background-color: #6b7280; color: #e5e7eb;\"><tr>";

                fw.WriteLine(t);

                foreach (string word in words)
                {
                    if (ChgCult_Chk.Checked)
                        xO += "<td class=\"py-1 border text-center  p-4\">" + word.TrimEnd(charsToTrim).ToString(new CultureInfo("us-US")) + "</td>";
                    else
                        xO += "<td class=\"py-1 border text-center  p-4\">" + word.TrimEnd(charsToTrim) + "</td>";
                }

                fw.WriteLine(xO);

                t = "</tr></thead><tbody class=\"bg-white text-gray-500 bg-[#FFFFFF] text-[#6b7280]\" style=\"background-color: #FFFFFF; " +
                    "color: #6b7280;\"><tr class=\"py-5\">";

                fw.WriteLine(t);

                xO = string.Empty;

                foreach (dynamic val in valjson)
                {
                    foreach (string word in words)
                    {
                        if (ChgCult_Chk.Checked)
                            xO += "<td class=\"py-1 border text-center  p-4\">" + val[word.TrimEnd(charsToTrim)].ToString(new CultureInfo("us-US")) + "</td>";
                        else
                            xO += "<td class=\"py-1 border text-center  p-4\">" + val[word.TrimEnd(charsToTrim)] + "</td>";
                    }

                    fw.WriteLine(xO);
                    xO = string.Empty;
                    fw.WriteLine("</tr><tr class=\"py-5\">");
                }

                fw.WriteLine("</tr></tbody></table></div></body></html>");
                fw.Close();

                Invoke(new Action<string>(OpnTableJson_Invk), JsonDirTable + Una + "_table.html");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!");
            }
        }

        void ParseNode_Thrd(string value, string count, string jsonout, string jsonnode, string charspace)
        {
            try
            {
                string xT = value;
                string xO = string.Empty;
                char[] charsToTrim = { ',' };
                string[] words = xT.Split();
                string sendval = string.Empty;
                string OutJs = string.Empty;

                using (StreamReader sr = new StreamReader(jsonout))
                {
                    OutJs = sr.ReadToEnd();
                }
                if (Brkt_Chk.Checked)
                    OutJs = "[" + OutJs + "]";

                int CntEnd = Convert.ToInt32(count);
                JsonNode CastNode = JsonNode.Parse(OutJs);
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
                            if (ChgCult_Chk.Checked)
                                xO += val[word.TrimEnd(charsToTrim)].ToString(new CultureInfo("us-US")) + charspace;
                            else
                                xO += val[word.TrimEnd(charsToTrim)] + charspace;
                        }

                        sendval += xO + "\r\n";
                        xO = string.Empty;
                    }
                }
                Invoke(new Action<string>(ValAdd_Invk), sendval);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!");
            }
        }

        void TableParseNode_Thrd()
        {
            try
            {
                CreateNameAleat();
                StreamWriter fw = new StreamWriter(JsonDirTable + Una + "_table.html");

                string xT = JsonVal_Txt.Text;
                string xO = string.Empty;
                char[] charsToTrim = { ',' };
                string[] words = xT.Split();
                string OutJs = string.Empty;

                string Jselect;
                if (OutJsonA_Chk.Checked)
                    Jselect = JsonA;
                else
                    Jselect = JsonB;

                using (StreamReader sr = new StreamReader(Jselect))
                {
                    OutJs = sr.ReadToEnd();
                }
                if (Brkt_Chk.Checked)
                    OutJs = "[" + OutJs + "]";

                int CntEnd = Convert.ToInt32(JsonCnt_txt.Text);
                JsonNode CastNode = JsonNode.Parse(OutJs);
                JsonNode SelNode = CastNode[JsonNode_Txt.Text];

                string t = "<!DOCTYPE html><html lang=\"fr\"><head><meta charset=\"UTF-8\"><title>Ostium Home</title><link rel=\"stylesheet\" " +
    "href=\"style.css\"></head><body><div class=\"relative overflow-hidden shadow-md rounded-lg\"><table class=\"table-fixed w-full " +
    "text-left\"><thead class=\"uppercase bg-[#6b7280] text-[#e5e7eb]\" style=\"background-color: #6b7280; color: #e5e7eb;\"><tr>";

                fw.WriteLine(t);

                foreach (string word in words)
                {
                    if (ChgCult_Chk.Checked)
                        xO += "<td class=\"py-1 border text-center  p-4\">" + word.TrimEnd(charsToTrim).ToString(new CultureInfo("us-US")) + "</td>";
                    else
                        xO += "<td class=\"py-1 border text-center  p-4\">" + word.TrimEnd(charsToTrim) + "</td>";
                }

                fw.WriteLine(xO);

                t = "</tr></thead><tbody class=\"bg-white text-gray-500 bg-[#FFFFFF] text-[#6b7280]\" style=\"background-color: #FFFFFF; " +
                    "color: #6b7280;\"><tr class=\"py-5\">";

                fw.WriteLine(t);

                xO = string.Empty;

                for (int i = 0; i < CntEnd; i++)
                {
                    JsonNode SrcName = SelNode[i];

                    JArray jsonVal = JArray.Parse("[" + SrcName.ToJsonString() + "]");
                    dynamic valjson = jsonVal;
                    foreach (dynamic val in valjson)
                    {
                        foreach (string word in words)
                        {
                            if (ChgCult_Chk.Checked)
                                xO += "<td class=\"py-1 border text-center  p-4\">" + val[word.TrimEnd(charsToTrim)].ToString(new CultureInfo("us-US")) + "</td>";
                            else
                                xO += "<td class=\"py-1 border text-center  p-4\">" + val[word.TrimEnd(charsToTrim)] + "</td>";
                        }

                        fw.WriteLine(xO);
                        xO = string.Empty;
                        fw.WriteLine("</tr><tr class=\"py-5\">");
                    }
                }

                fw.WriteLine("</tr></tbody></table></div></body></html>");
                fw.Close();

                Invoke(new Action<string>(OpnTableJson_Invk), JsonDirTable + Una + "_table.html");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!");
            }
        }

        void SavefileShowDiag(string DataFile, string Filter)
        {
            SaveFileDialog saveFD = new SaveFileDialog
            {
                InitialDirectory = AppStart,
                Filter = Filter,
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFD.ShowDialog() == DialogResult.OK)
            {
                File.Copy(DataFile, saveFD.FileName, true);
                Console.Beep(1000, 400);
            }
        }

        void OutJsonA_Chk_Click(object sender, EventArgs e)
        {
            if (OutJsonA_Chk.Checked)
                OutJsonB_Chk.Checked = false;
            else
                OutJsonB_Chk.Checked = true;
        }

        void OutJsonB_Chk_Click(object sender, EventArgs e)
        {
            if (OutJsonB_Chk.Checked)
                OutJsonA_Chk.Checked = false;
            else
                OutJsonA_Chk.Checked = true;
        }

        void Rfresh_Btn_Click(object sender, EventArgs e)
        {
            string Btn = (sender as Button).Name;
            if (Btn == RfreshA_Btn.Name)
                WbOutA.Reload();
            else
                WbOutB.Reload();
        }

        void Empty_Btn_Click(object sender, EventArgs e)
        {
            string Btn = (sender as Button).Name;
            Uri uri = new Uri("https://veydunet.com/ostium/ostium.html");
            if (Btn == EmptyA_Btn.Name)
                WbOutA.Source = uri;
            else
                WbOutB.Source = uri;
        }

        void Extd_Btn_Click(object sender, EventArgs e)
        {
            string Btn = (sender as Button).Name;
            if (Btn == ExtdA_Btn.Name)
            {
                if (!PanelBjson_Pnl.Visible)
                {
                    WbOutA.Dock = DockStyle.Top;
                    WbOutB.Dock = DockStyle.Fill;
                    WbOutB.Visible = true;
                    PanelBjson_Pnl.Visible = true;
                }
                else
                {
                    WbOutA.Dock = DockStyle.Fill;
                    WbOutB.Dock = DockStyle.None;
                    WbOutB.Visible = false;
                    PanelBjson_Pnl.Visible = false;
                }

                PanelAjson_Pnl.Visible = true;
                WbOutA.Visible = true;
            }
            else
            {
                if (!PanelAjson_Pnl.Visible)
                {
                    WbOutA.Dock = DockStyle.Top;
                    WbOutB.Dock = DockStyle.Fill;
                    WbOutA.Visible = true;
                    PanelAjson_Pnl.Visible = true;
                }
                else
                {
                    WbOutA.Dock = DockStyle.None;
                    WbOutB.Dock = DockStyle.Fill;
                    WbOutB.Visible = false;
                    PanelAjson_Pnl.Visible = false;
                }

                PanelBjson_Pnl.Visible = true;
                WbOutB.Visible = true;
            }
        }

        void Object_Keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        #endregion

        void ClearObject_Keypress(object sender, EventArgs e)
        {
            TextBox objTxt;
            objTxt = (sender as TextBox);
            objTxt.Text = "";
        }

        #region Invoke_Json

        void ValAdd_Invk(string val)
        {
            string Jselect;
            if (OutJsonA_Chk.Checked)
                Jselect = JsonB;
            else
                Jselect = JsonA;

            File_Write(Jselect, val);
            Uri uri = new Uri("file:///" + Jselect);

            if (OutJsonA_Chk.Checked)
                WbOutB.Source = uri;
            else
                WbOutA.Source = uri;

            Console.Beep(1000, 400);
        }

        void OpnTableJson_Invk(string val)
        {
            GoBrowser("file:///" + val, 1);
        }

        #endregion

        #region Update_
        ///
        /// <summary>
        /// Checking updates via Http request and comparison with the variable => "versionNow"
        /// </summary>
        /// <param value="0">Auto check no announcement if update False announcement if True</param>
        /// <param value="1">Manual check announces whether False or True</param>
        ///
        async void VerifyUPDT(string softName, int annoncE)
        {
            try
            {
                bool isConnected = await CheckInternetConnectionAsync();

                if (isConnected)
                {
                    HttpClient client = new HttpClient();
                    var response = await client.GetAsync(updtOnlineFile);
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
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! VerifyUPDT: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        void AnnonceUpdate(string softName)
        {
            var result = MessageBox.Show($"An update is available for the {softName} " +
                $"software, open the update page now?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            try
            {
                if (result == DialogResult.Yes)
                    GoBrowser(WebPageUpdate, 0);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AnnonceUpdate: ", ex.ToString(), "Main_Frm", AppStart);
            }
        }

        static async Task<bool> CheckInternetConnectionAsync()
        {
            try
            {
                string host = "8.8.8.8";
                int timeout = 1000;

                Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync(host, timeout);

                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
