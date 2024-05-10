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

namespace Ostium
{
    public partial class Main_Frm : Form
    {
        #region Var_Function
        ///
        /// <summary>
        /// Emet un Bip à la fin de certaines opérations
        /// </summary>
        /// <param name="freq">Frequence de la tonalité du Beep</param>        
        /// <param name="duration">Durée d'émission du Beep</param>
        /// <returns></returns>
        /// 
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);
        ///
        /// <summary>
        /// Initialisation de la voix pour la Lecture des Feed Titles
        /// </summary>
        /// 
        readonly SpeechSynthesizer synth = new SpeechSynthesizer();
        ///
        /// <summary>
        /// Liste des URL de configuration par défaut du fichier "config.xml", charger à partir du fichier "url_dflt_cnf.ost"
        /// </summary>
        /// 
        readonly List<string> lstUrlDfltCnf = new List<string>();
        ///
        /// <summary>
        /// Repertoires des différents fichiers d'utilisation et de la Database
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
        public string D4ta = "default_database_name";
        ///
        /// <summary>
        /// Liste des Objets
        /// </summary>
        /// 
        public Webview_Frm webviewForm;
        public HtmlText_Frm HtmlTextFrm;
        public Mdi_Frm mdiFrm;
        public Doc_Frm docForm;
        public OpenSource_Frm openSourceForm;
        public ScriptCreator scriptCreatorFrm;
        public Bookmarklets_Frm bookmarkletsFrm;
        public ListBox List_Object;
        public ToolStripComboBox Cbx_Object;
        public ListBox List_Wf;
        public ComboBox Workflow_Cbx;
        public ListBox Workflow_Lst;
        public Label SizeAll_Lbl;
        ///
        /// <summary>
        /// Variables
        /// </summary>
        /// 
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
        public string ThemeDiag;
        public string FileDiag = "";
        string MinifyScr = "";
        string MapXmlOpn = "";
        string CrossCenter = "on";
        int MapZoom = 1;
        ///
        /// <summary>
        /// Map variable
        /// </summary>
        /// 
        public string VerifMapOpn = "off";
        public readonly GMapOverlay overlayOne = new GMapOverlay("OverlayOne");
        public double LatT = 48.8589507;
        public double LonGt = 2.2775175;
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
        /// Message affiché au lancement de la création d'un Diagram
        /// </summary>
        readonly string MessageStartDiagram = "When this window closes, the diagram creation process begins, be patient the time depends on the file size " +
            "and structure. In case of blockage! use Debug in the menu to kill the javaw process. Feel free to join the Discord channel for help.";
        ///
        /// <summary>
        /// Variable pour la vérification des mises à jour
        /// <param name="versionNow">Version actuel de l'application à comparer avec la requête Http = > "updt_ostium.html"</param>
        /// </summary>
        /// 
        readonly string upftOnlineFile = "https://veydunet.com/2x24/sft/updt/updt_ostium.html";
        readonly string WebPageUpdate = "http://veydunet.com/ostium/update.html";
        readonly string versionNow = "1";

        readonly string HomeUrlRSS = "https://veydunet.com/ostium/rss.html";

        GMarkerGoogleType Mkmarker = GMarkerGoogleType.red_dot;

        #endregion

        #region Frm_
        ///
        /// <param name="SoftVersion">Vérification de version et Assembly</param>
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

        private void Main_Frm_Load(object sender, EventArgs e)
        {
            try
            {
                BeginInvoke((MethodInvoker)delegate {
                    CreateDirectory();
                    ///
                    /// Chargement des URL par défaut dans une List
                    /// 
                    if (File.Exists(AppStart + "url_dflt_cnf.ost"))
                    {
                        lstUrlDfltCnf.Clear();
                        lstUrlDfltCnf.AddRange(File.ReadAllLines(AppStart + "url_dflt_cnf.ost"));
                    }
                    ///
                    /// Chargement de la configuration
                    /// <param name="CreateConfigFile"></param>
                    /// <param value="0"></param>
                    /// Réinitialisation de la config, chargement des URL par défaut du fichier url_dflt_cnf.ost
                    /// <param value="1"></param>
                    /// Sauvegarde de la configuration choisi et Reload
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
                    /// URL Web Page d'accueil wBrowser Tab => index et wBrowser Tab => feed
                    /// Si vide chargement à partir du fichier d'URL par défaut
                    /// 
                    if (@Class_Var.URL_HOME == "")
                    {
                        @Class_Var.URL_HOME = lstUrlDfltCnf[1].ToString();
                    }

                    WBrowse.Source = new Uri(@Class_Var.URL_HOME);
                    WBrowsefeed.Source = new Uri(HomeUrlRSS);

                    Tools_TAB_0.Visible = true;
                    ///
                    /// Vérification des mises à jour auto
                    /// <param value="0"></param>
                    /// Message uniquement si update available
                    /// <param value="1"></param>
                    /// Message False ou True update
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
        /// Demande de nettoyage à la fermeture de l'application
        /// </summary>
        /// <param name="ClearOnOff"></param>
        /// <param name="on">Nettoyage True</param>
        /// <param name="off">Pas de nettoyage demandé quand redémarrage auto de l'application pour modification de l'user-agent par défaut</param> 
        /// 
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
            ThemDiag_Cbx.SelectedIndexChanged += new EventHandler(ThemDiag_Cbx_SelectedIndexChanged);
            GMap_Ctrl.OnPositionChanged += new PositionChanged(GMap_Ctrl_OnPositionChanged);
            GMap_Ctrl.OnMapZoomChanged += new MapZoomChanged(GMap_Ctrl_OnMapZoomChanged);
        }
        ///
        /// <summary>
        /// Création du fichier de configuration "config.xml"
        /// </summary>
        /// <param name="val"></param>
        /// <param value="0">Chargement des URL par défaut du fichier "url_dflt_cnf.ost"</param>
        /// <param value="1">Sauvegarde avec les paramètres choisi par l'utilisateur</param>        
        /// 
        private void CreateConfigFile(int val)
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
            /// Creation du fichier XML "config.xml"
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
                writer.WriteElementString("URL_TRAD_WEBTXT_VAR", "https://translate.google.fr/?hl=fr&sl=auto&tl=fr&text=replace_query&op=translate");
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
            /// Chargement de la configuration du fichier "config.xml"
            /// 
            Config_Ini(AppStart + "config.xml");
        }
        ///
        /// <summary>
        /// Création des différents répertoires de l'application => DirectoryCreate
        /// </summary>
        /// 
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
                DirectoryCreate(Pictures);
                DirectoryCreate(Scripts);
                DirectoryCreate(Workflow);
                DirectoryCreate(Workflow + "model");
                DirectoryCreate(DiagramDir);
                DirectoryCreate(BkmkltDir);
                DirectoryCreate(MapDir);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! CreateDirectory: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Création des différents répertoires si Not exist!
        /// </summary>
        /// <param name="dir">Répertoire en cour de création</param>
        /// 
        public void DirectoryCreate(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        ///
        /// <summary>
        /// Chargement de la configuration à partir du fichier "config.xml"
        /// Enregistrements des variables Class_var.cs
        /// </summary>
        /// <param name="ConfigFile"></param>
        /// 
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
                ///
                /// Si DataBase exist True loading
                /// Si DataBase exist False create
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
                /// Chargement des scipts JS à partir du fichier "scripturl.ost" pour l'injection
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
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! Config_Ini: ", ex.Message, "Main_Frm", AppStart);
            }
        }
        ///
        /// <summary>
        /// Calcule et affichage de la taille des répertoires de l'application
        /// </summary>
        /// <param name="directoryname">Adresse du répertoire</param>
        /// <param name="objectsend">Chargement de la taille dans l'objet Label correspondant</param>
        /// 
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
        ///
        /// <summary>
        /// Ajout d'item au Menu Contextuel wBrowse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="newItem0">Recherche sur la WayBackMachine</param>
        /// <param name="newItem1">Recherche de vidéo sur Youtube</param>
        /// <param name="newItem2">Embed video Youtube</param>
        /// 
        public void WebView2Control_ContextMenuRequested(object sender, CoreWebView2ContextMenuRequestedEventArgs args)  // ContextMenu
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
                {
                    pageUri = Clipboard.GetText(TextDataFormat.Text);
                }

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
                        using (StreamWriter file_create = new StreamWriter(AppStart + "tmpytb.html"))
                        {
                            file_create.Write("<iframe width=100% height=100% src=\"" + pageUri + "\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" referrerpolicy=\"strict-origin-when-cross-origin\" allowfullscreen></iframe>");
                        }
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
        /// Mise à jour de la barre de titre de wBrowse TAB index
        /// </summary>
        /// <param name="message">Titre de la page Web en cour</param>
        /// <param name="TmpTitleWBrowse">Variable du Titre de l'application quand changement de TAB</param>        
        /// 
        public void WBrowse_UpdtTitleEvent(string message)
        {
            string currentDocumentTitle = WBrowse?.CoreWebView2?.DocumentTitle ?? "Uninitialized";
            Text = currentDocumentTitle + " [" + message + "]";
            TmpTitleWBrowse = Text;
        }
        ///
        /// <summary>
        /// Navigation starting => vérification si la modification de l'user-agent est activée
        /// </summary>
        /// <param name="UserAgentOnOff"></param>
        /// <param name="on">Si activé modification de l'user-agent</param>
        /// 
        public void WBrowse_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
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
        /// Injection de script et enregistrement de cookie et mise à jour de Title
        /// </summary>
        /// <param name="ScriptInject()">vérification si un script est enregistrer et a exécuter pour l'URL en cour</param>        
        /// <param name="GetCookie">Sauvegarde des cookies dans le fichier cookie.txt</param>
        /// 
        public void WBrowse_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            @Class_Var.URL_URI = WBrowse.Source.AbsoluteUri;

            GetCookie(WBrowse.Source.AbsoluteUri);
            WBrowse_UpdtTitleEvent("Navigation Completed");

            ScriptInject();
        }
        ///
        /// <summary>
        /// Vérifie si l'injection de cookie est activée
        /// </summary>
        /// <param name="SetCookie_Chk.Checked">Si True création du cookie configuré</param>
        /// 
        public void WBrowse_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (SetCookie_Chk.Checked)
            {
                CoreWebView2Cookie cookie = WBrowse.CoreWebView2.CookieManager.CreateCookie(CookieName_Txt.Text, CookieValue_Txt.Text, CookieDomain_Txt.Text, "/");
                WBrowse.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);
            }
        }
        ///
        /// <param name="URLtxt_txt">Sauvegarde de l'URL en cour dans Textbox pour réutilisation</param>
        /// 
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
        ///
        /// <param name="NameUriDB">Variable du Title de l'URL pour ajout dans la DataBase</param>        
        /// <param name="Download_Source_Page()">Téléchargement et enregistre du source de la page Web dans le fichier sourcepage pour réutilisation</param>
        /// 
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
            WBrowse.CoreWebView2.ContextMenuRequested += WebView2Control_ContextMenuRequested; // ContextMenu

            WBrowse_UpdtTitleEvent("Initialization Completed succeeded");
        }

        public void WBrowse_EventHandlers(Microsoft.Web.WebView2.WinForms.WebView2 control)
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
        ///
        /// <param name="GetCookie">Sauvegarde des cookies dans le fichier cookie.txt</param>
        ///
        public void WBrowsefeed_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            GetCookie(WBrowsefeed.Source.AbsoluteUri);
            WBrowsefeed_UpdtTitleEvent("Navigation Completed");
        }
        ///
        /// <param name="URLtxt_txt">Sauvegarde de l'URL en cour dans Textbox pour réutilisation</param>
        /// 
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

        public void WBrowsefeed_EventHandlers(Microsoft.Web.WebView2.WinForms.WebView2 control)
        {
            control.CoreWebView2InitializationCompleted += WBrowsefeed_InitializationCompleted;
            control.NavigationStarting += WBrowsefeed_NavigationStarting;
            control.NavigationCompleted += WBrowsefeed_NavigationCompleted;
            control.SourceChanged += WBrowsefeed_SourceChanged;
        }

        #endregion        
        ///
        /// <summary>
        /// Injection de scripts
        /// </summary>
        /// <param name="WBrowse.Source.AbsoluteUri">URL du site en cour, suppression des caractères spéciaux de l'URL et ajout de l'extensions ".js" 
        /// pour vérifier si un nom de script ".js" du même nom existe, si True injection du script contenu dans le fichier sur la page Web en cour</param>
        /// 
        public async void ScriptInject()
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
        ///
        /// <summary>
        /// Ouverture de la page Web
        /// </summary>
        /// <param name="GoBrowser"></param>
        /// <param value="0">Ouverture fenêtre parent</param>        
        /// <param value="1">Ouverture dans une nouvelle fenêtre</param>
        /// 
        public void Go_Btn_Click(object sender, EventArgs e)
        {
            int x;
            x = URLbrowse_Cbx.FindStringExact(URLbrowse_Cbx.Text);
            if (x == -1)
            {
                URLbrowse_Cbx.Items.Add(URLbrowse_Cbx.Text);
            }       
            GoBrowser(URLbrowse_Cbx.Text, 0);
        }

        public void GoWebwiev_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser(URLbrowse_Cbx.Text, 1);
        }
        ///
        /// <summary>
        /// Vérification de l'URL et reformattage puis ouverture de la page Web ou recherche sur le Web
        /// </summary>
        /// <param name="WebviewRedirect"></param>
        /// <param value="0">Ouverture ou Recherche dans la fenêtre parent</param>
        /// <param value="1">Ouverture ou Recherche dans une nouvelle fenêtre</param>
        /// <param value="file:///">Ouverture de fichier local</param>
        /// <param name="URIopn">URL open dans wBrowser "TAB BROWSx"</param>
        /// 
        public void GoBrowser(string URIopn, int WebviewRedirect)
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
        ///
        /// <summary>
        /// Ouverture de la Page Home si configuré "@Class_Var.URL_HOME" ou ouverture de la page par défaut du fichier "url_dflt_cnf.ost"
        /// </summary>
        /// 
        public void Home_Btn_Click(object sender, EventArgs e)
        {
            if (@Class_Var.URL_HOME == "")
            {
                @Class_Var.URL_HOME = lstUrlDfltCnf[1].ToString();
            }

            WBrowse.Source = new Uri(@Class_Var.URL_HOME);
        }

        public void OnKey_URLbrowse(object sender, KeyPressEventArgs e)
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

        public void URLbrowse_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (Control_Tab.SelectedIndex == 0)
            //{
            //    WBrowse.Source = new Uri(@URLbrowse_Cbx.Text);
            //}
        }
        ///
        /// <summary>
        /// Ouverture URL du fichier "filesdir/url.txt" chargé dans CBX
        /// </summary>
        /// 
        public void URL_URL_Cbx_SelectedIndexChanged(object sender, EventArgs e)
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

        private void CleanSearch_Btn_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Items.Clear();
            URLbrowse_Cbx.Text = "";
        }

        #endregion

        #region Tools_Tab_0
        ///
        /// <summary>
        /// Modification de l'user-agent par l'agent de remplacement si configuré "Class_Var.URL_USER_AGENT" ou par l'agent par défaut du fichier 
        /// "url_dflt_cnf.ost" et/ou redemarrage de l'application pour revenir à l'agent par défaut de wBrowser
        /// </summary>
        /// <param name="ClearOnOff"></param>
        /// <param name="on"></param>
        /// Notification de demande de nettoyage True
        /// <param name="off"></param>
        /// Pas de notification de demande de nettoyage quand redémarrage auto de l'application pour retour de l'user-agent par défaut
        /// 
        private void UserAgentChange_Btn_Click(object sender, EventArgs e)
        {
            if (Class_Var.URL_USER_AGENT == "")
            {
                Class_Var.URL_USER_AGENT = lstUrlDfltCnf[4].ToString();
            }

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
        /// Modification de l'user-agent par un agent type "GoogleBot" si configuré dans "Class_Var.URL_USER_AGENT" ou par l'agent par défaut du fichier 
        /// "url_dflt_cnf.ost" et/ou redemarrage de l'application
        /// </summary>
        /// <param name="ClearOnOff"></param>
        /// <param name="on"></param>
        /// Notification de demande de nettoyage True
        /// <param name="off"></param>
        /// Pas de notification de demande de nettoyage quand redémarrage auto de l'application pour retour de l'user-agent par défaut
        /// 
        private void Googlebot_Btn_Click(object sender, EventArgs e)
        {
            if (Class_Var.URL_GOOGLEBOT == "")
            {
                Class_Var.URL_GOOGLEBOT = lstUrlDfltCnf[6].ToString();
            }

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
        /// Reformattage du pseudo/mot saisi dans "Word_Construct_URL_Txt" avant envoi vers => Construct_URL pour construction d'URL
        /// </summary>
        /// <param name="Construct_URL"></param>
        /// Creation d'URL avec le pseudo ou mot de recherche
        /// 
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
        ///
        /// <summary>
        /// Ouverture.Fermeture de la console
        /// </summary>
        /// 
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
        ///
        /// <summary>
        /// Copy de l'URL de la page Web en cour dans le clipboard
        /// </summary>
        /// 
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
        ///
        /// <summary>
        /// Traduction de la page Web via le site de traduction par défaut par défaut du fichier "url_dflt_cnf.ost"
        /// ou celui choisi par l'utilisateur "Class_Var.URL_TRAD_WEBPAGE"
        /// </summary>
        /// <param name="VVVV0VVVV"></param>
        /// Caractères de remplacement par l'URL à traduire et ouverture dans wBrowser
        /// 
        public void TraductPage_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Class_Var.URL_TRAD_WEBPAGE == "")
                {
                    Class_Var.URL_TRAD_WEBPAGE = lstUrlDfltCnf[2].ToString();
                }

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
        ///
        /// <summary>
        /// Unshort URL
        /// </summary>
        /// <param name="Uri"></param>
        /// URL à vérifier
        /// 
        public void StartUnshortUrl(string Uri)
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
        ///
        /// <summary>
        /// Execution Add-on du répertoire add-on
        /// </summary>
        /// 
        public void AddOn_Cbx_SelectedIndexChanged(object sender, EventArgs e)
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
        /// Chargement du fichier URL constructeur sélectionné qui se trouve dans le répertoire "filesdir/url-constructor"
        /// </summary>
        /// 
        public void Construct_URL_Cbx_SelectedIndexChanged(object sender, EventArgs e)
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
        /// Ouverture dans la fenêtre "OpenSource_Frm" du fichier "filesdir/gh.txt" (Google Dork) si file exist True
        /// </summary>
        /// <param name="Open_Source_Frm">Ouverture de la fenêtre</param>
        /// 
        public void GoogleDork_Btn_Click(object sender, EventArgs e)
        {
            if (File.Exists(FileDir + "gdork.txt"))
                Open_Source_Frm(FileDir + "gdork.txt");
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
        ///
        /// <summary>
        /// Ouverture de la fenêtre "HtmlText_Frm" Webrowser textuel
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
        /// Capture de wBrowser
        /// 
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
        /// <summary>
        /// Web page to Png
        /// </summary>
        /// <param name="clipSize"></param>
        /// <returns></returns>
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
        ///
        /// <summary>
        /// Ouverture du fichier cookie dans l'éditeur
        /// </summary>
        /// <param name="OpenFile_Editor"></param>
        /// <param value="filesdir/memo.txt"></param>
        /// 
        public void Cookie_Btn_Click(object sender, EventArgs e)
        {
            OpenFile_Editor(AppStart + "cookie.txt");
        }
        ///
        /// <summary>
        /// Ouverture du module d'injection de cookie
        /// </summary>
        /// 
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
        ///
        /// <summary>
        /// <param name="fileopen">Selection et Ouverture de fichier dans l'éditeur</param>
        /// </summary>
        /// <param name="OpenFile_Editor"></param>
        /// <param value="filePath">Fichier sélectionné</param>
        /// 
        public void OpnFilOnEditor_Btn_Click(object sender, EventArgs e)
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
        ///
        /// <summary>
        /// Ouverture de Liste d'URL dans la fenêtre "OpenSource_Frm"
        /// </summary>
        /// <param name="Open_Source_Frm"></param>
        /// <param value="filePath"></param>
        /// Fichier sélectionné
        /// 
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
        ///
        /// <summary>
        /// MDI Form open
        /// </summary>
        /// 
        private void OpnGroupFrm_Btn_Click(object sender, EventArgs e)
        {
            mdiFrm = new Mdi_Frm();
            mdiFrm.Show();
        }
        ///
        /// <summary>
        /// Ouverture du fichier memo dans l'éditeur
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
        /// Vérification de l'existence et ouverture de l'éditeur par défaut "Class_Var.DEFAULT_EDITOR"
        /// </summary>
        /// 
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
        ///
        /// <summary>
        /// Injection de script JS dans page Web
        /// </summary>
        /// <param name="InputBox"></param>
        /// Saisi du script
        /// <param value="ScriptInject"></param>
        /// Script à injecter
        /// 
        private async void InjectScript_Btn_Click(object sender, EventArgs e)
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

        private void OpenScriptEdit_Btn_Click(object sender, EventArgs e)
        {
            scriptCreatorFrm = new ScriptCreator();
            scriptCreatorFrm.Show();
        }
        ///
        /// <summary>
        /// Injection RegEX dans page Web
        /// </summary>
        /// <param name="InputBox"></param>
        /// Saisi du script
        /// <param value="ScriptInject"></param>
        /// Script à injecter
        /// 
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
        ///
        /// <summary>
        /// Reset configuration par defaut
        /// </summary>
        /// <param name="CreateConfigFile"></param>
        /// <param value="0"></param>
        /// Chargement des URL par défaut du fichier "url_dflt_cnf.ost"
        /// <param value="1"></param>
        /// Sauvegarde avec les paramètres choisi par l'utilisateur
        /// 
        private void ResetConfig_Btn_Click(object sender, EventArgs e)
        {
            string message = "Reset Config?";
            string caption = "Config";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                CreateConfigFile(0);
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
        ///
        /// <summary>
        /// Archivage/Compression des répertoires sélectionnés avec 7zip création d'un fichier ".bat", création de l'archive "Archives.zip" 
        /// </summary>
        /// <param name="7zip"></param>
        /// <param name="Archive-DB-FILES-FEED.bat"></param>
        /// 
        private void ArchiveDirectory_Btn_Click(object sender, EventArgs e)
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
        /// Vérification des mises à jour (manuel)
        /// <param value="0">Message uniquement si update available</param>        
        /// <param value="1">Message False ou True update</param>
        /// 
        private void OstUpdt_Btn_Click(object sender, EventArgs e)
        {
            VerifyUPDT("Ostium", 1);
        }
        /// 
        /// <param name="GoBrowser"></param>
        /// <param name="WebviewRedirect"></param>
        /// <param value="0">Ouverture fenêtre parent</param>
        /// <param value="1">Ouverture dans une nouvelle fenêtre</param>
        ///
        private void HomePage_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser("https://veydunet.com/ostium/home.html", 0);
        }

        private void Credit_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(AppStart + "credits.txt");
        }

        private void CreateBokmark_Btn_Click(object sender, EventArgs e)
        {
            bookmarkletsFrm = new Bookmarklets_Frm();
            bookmarkletsFrm.Show();
        }

        #endregion

        #region Tools_Tab_1
        ///
        /// <summary>
        /// Ouverture de catégorie de flux RSS dans l'éditeur
        /// </summary>
        /// <param name="OpenFile_Editor"></param>
        /// <param name="CategorieFeed_Cbx"></param>
        /// Sélection de la catégorie à ouvrir
        /// 
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
        ///
        /// <summary>
        /// Traduction et affichage de la page Web via le site de traduction par défaut du fichier "url_dflt_cnf.ost" ou celui choisi par 
        /// l'utilisateur "Class_Var.URL_TRAD_WEBPAGE"
        /// </summary>
        /// <param name="VVVV0VVVV"></param>
        /// Valeur de remplacement par l'URL
        /// 
        private void TraductPageFeed_Btn_Click(object sender, EventArgs e)
        {
            if (Class_Var.URL_TRAD_WEBPAGE == "")
            {
                Class_Var.URL_TRAD_WEBPAGE = lstUrlDfltCnf[2].ToString();
            }

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
        ///
        /// <summary>
        /// Delete project WorkFlow 
        /// </summary>
        /// 
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
        ///
        /// <summary>
        /// Affichage du fichier XML du projet WorkFlow dans wBrowser
        /// </summary>
        /// 
        private void ViewXml_Tls_Click(object sender, EventArgs e)
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
        /// Ouverture du fichier XML du projet WorkFlow dans l'éditeur
        /// </summary>
        /// <param name="OpenFile_Editor"></param>
        /// <param value="NameProjectwf_Txt">Projet WorkFlow sélectionné</param>
        /// 
        private void EditXml_Tls_Click(object sender, EventArgs e)
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
        /// Export du projet WorkFlow au format XML
        /// </summary>
        /// 
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
        ///
        /// <summary>
        /// Export du projet WorkFlow au format Json
        /// </summary>
        /// 
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
        ///
        /// <summary>
        /// Création de diagramme de données Json d'un projet WorkFlow XML avec plantUML.jar (#Model 1)
        /// </summary>
        /// <param name="ConvertJson">Convertion du projet WorkFlow XML au format Json</param>
        /// <param value="1">Ajoute l'en-tête de script "@startjson" "!theme" puis de fin "@endjson" pour plantUML dans le fichier pour le transformer en SVG</param>
        /// <param value="0">Simple conversion de fichier XML au format Json pas d'en-tête</param>
        /// <param name="Commut"></param>
        /// <param value="0">Les fichiers de création de diagramme sont dans le répertoire diagram ce sont les fichiers XML des projets WorkFlow</param>
        /// <param value="1">Les fichiers de création de diagramme sont dans un répertoire aléatoire ce sont des fichiers plantUML</param>
        /// <param name="CreateDiagram_Thrd"></param>
        /// <param value="0">Le fichier de création du diagramme est dans le répertoire diagram</param>
        /// <param value="1">Le fichier de création du diagramme est dans un répertoire aléatoire</param>
        /// 
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
        /// Conversion du fichier XML de projet WorkFlow au format Json ou conversion du fichier XML pour conversion en diagramme SVG
        /// </summary>
        /// <param name="xmlFileConvert">Fichier XML à convertir au format Json</param>
        /// <param name="dirFileConvert">Répertoire d'enregistrement du fichier final</param>
        /// <param name="value"></param>
        /// <param value="1">Ajoute l'en-tête de script "@startjson" "!theme" puis de fin "@endjson" pour plantUML dans le fichier créé pour le transformer en SVG</param>
        /// <param value="0">Simple conversion de fichier XML au format Json pas d'en-tête</param>
        /// 
        private void ConvertJson(string xmlFileConvert, string dirFileConvert, int value)
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
        ///
        /// <summary>
        /// Création de diagramme de données Json avec plantUML.jar (#Model 1)
        /// </summary>
        /// <param name="fileselect">Fichier à créer</param>
        /// <param name="value"></param>
        /// <param value="0">Les fichiers de création de diagramme sont dans le répertoire diagram ce sont les fichiers XML des projets WorkFlow</param>
        /// <param value="1">Les fichiers de création de diagramme sont dans un répertoire aléatoire ce sont des fichiers plantUML</param>
        /// 
        private void CreateDiagram_Thrd(string fileselect, int value)
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
        /// Création de diagramme MindMap d'un projet WorkFlow XML avec plantUML.jar (#Model 2)
        /// </summary>
        /// <param name="CreateDiagramMinMapFile_Thrd"></param>
        /// <param value="0">"+" caractère de formatage pour un Diagramme MindMap non détaillé (#Model 3)</param>
        /// <param value="1">"*" caractère de formatage pour un Diagramme MindMap détaillé (#Model 2)</param>
        /// 
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
        ///
        /// <summary>
        /// Création de diagramme MindMap d'un projet WorkFlow XML avec plantUML.jar (#Model 3)
        /// </summary>
        /// <param name="CreateDiagramMinMapFile_Thrd"></param>
        /// <param value="0">"+" caractère de formatage pour un Diagramme MindMap non détaillé (#Model 3)</param>
        /// <param value="1">"*" caractère de formatage pour un Diagramme MindMap détaillé (#Model 2)</param>
        /// 
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
        ///
        /// <summary>
        /// Création de diagramme MindMap d'un projet WorkFlow XML avec plantUML.jar (#Model 2) ou (#Model 3)
        /// </summary>
        /// <param name="value"></param>
        /// <param value="0">"+" caractère de formatage pour un Diagramme MindMap non détaillé (#Model 3)</param>        
        /// <param value="1">"*" caractère de formatage pour un Diagramme MindMap détaillé (#Model 2)</param>        
        /// <param name="Commut"></param>
        /// <param value="0">Les fichiers de création de diagramme sont dans le répertoire diagram ce sont les fichiers XML des projets WorkFlow</param>        
        /// <param value="1">Les fichiers de création de diagramme sont dans un répertoire aléatoire ce sont des fichiers plantUML</param>        
        /// <param name="CreateDiagram_Thrd"></param>
        /// <param value="0">Les fichiers de création de diagramme sont dans le répertoire diagram ce sont les fichiers XML des projets WorkFlow</param>
        /// <param value="1">Les fichiers de création de diagramme sont dans un répertoire aléatoire ce sont des fichiers plantUML</param>
        /// 
        private void CreateDiagramMinMapFile_Thrd(int value)
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
        /// Création de diagramme Json à partir d'un fichier Json avec plantUML.jar
        /// Ouverture du fichier sélectionné dans "TmpFile_Txt.Text"
        /// </summary>
        /// <param name="Commut"></param>
        /// <param value="0">Les fichiers de création de diagramme sont dans le répertoire diagram ce sont les fichiers XML des projets WorkFlow</param>
        /// <param value="1">Les fichiers de création de diagramme sont dans un répertoire aléatoire ce sont des fichiers TXT au format plantUML</param>
        /// <param name="CreateDiagram_Thrd"></param>
        /// <param value="0">Les fichiers de création de diagramme sont dans le répertoire diagram ce sont les fichiers XML des projets WorkFlow</param>
        /// <param value="1">Les fichiers de création de diagramme sont dans un répertoire aléatoire ce sont des fichiers TXT au format plantUML</param>
        /// 
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

                    if (File.Exists(DiagramDir + "temp_file.svg"))
                        File.Delete(DiagramDir + "temp_file.svg");

                    using (StreamReader sr = new StreamReader(filePath))
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
        /// Création de diagramme Json à partir d'un fichier XML avec plantUML.jar
        /// Ouverture du fichier et conversion au format Json
        /// </summary>
        /// <param name="ConvertJson">Convertion du projet WorkFlow XML au format Json</param>
        /// <param value="1">Ajoute l'en-tête de script "@startjson" "!theme" puis de fin "@endjson" pour plantUML dans le fichier créé pour le transformer en SVG</param>
        /// <param value="0">Simple conversion de fichier XML au format Json pas d'en-tête</param>
        /// <param name="Commut"></param>
        /// <param value="0">Les fichiers de création de diagramme sont dans le répertoire diagram ce sont les fichiers XML des projets WorkFlow</param>
        /// <param value="1">Les fichiers de création de diagramme sont dans un répertoire aléatoire ce sont des fichiers TXT au format plantUML</param>
        /// <param name="CreateDiagram_Thrd"></param>
        /// <param value="0">Le fichier de création du diagramme est dans le répertoire diagram</param>
        /// <param value="1">Le fichier de création du diagramme est dans un répertoire aléatoire</param>
        /// 
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
        /// Création de diagramme Json à partir d'un fichier au format plantUML avec plantUML.jar
        /// </summary>
        /// <param name="Commut"></param>
        /// <param value="0">Les fichiers de création de diagramme sont dans le répertoire diagram ce sont les fichiers XML des projets WorkFlow</param>
        /// <param value="1">Les fichiers de création de diagramme sont dans un répertoire aléatoire ce sont des fichiers TXT au format plantUML</param>
        /// <param name="CreateDiagram_Thrd"></param>
        /// <param value="0">Les fichiers de création de diagramme sont dans le répertoire diagram ce sont les fichiers XML des projets WorkFlow</param>
        /// <param value="1">Les fichiers de création de diagramme sont dans un répertoire aléatoire ce sont des fichiers TXT au format plantUML</param>
        ///
        private void OpnPlantUMLFile_Btn_Click(object sender, EventArgs e)
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
        /// Exportation du diagramme créé
        /// </summary>
        /// <param name="Commut"></param>
        /// <param value="0">nom du fichier de sortie => UsenameAleatoire + "_" + FileDiag</param>
        /// <param value="1">nom du fichier de sortie => UsenameAleatoire</param>
        /// 
        private void ExportDiag_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (FileDiag == "")
                {
                    MessageBox.Show("SVG file not exist!");
                    return;
                }

                CreateNameAleat();
                string nameSVG = UsenameAleatoire + "_" + FileDiag;
                string nameSVGb = UsenameAleatoire;

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

        private void ThemDiag_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tools_TAB_3.Focus();
        }

        private void OpnSprites_Btn_Click(object sender, EventArgs e)
        {
            if (!File.Exists(AppStart + "setirps.exe"))
            {
                MessageBox.Show("Setirps is missing!", "Missing editor");
                return;
            }

            Process.Start(AppStart + "setirps.exe");
        }
        
        #endregion

        public void Control_Tab_Click(object sender, EventArgs e)
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

        private void CtrlTabBrowsx()
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

            if (JavaEnableDisable_Btn.Text == "Javascript Disable")
            {
                JavaDisable_Lbl.Visible = true;
            }

            JavaDisableFeed_Lbl.Visible = false;
        }
        ///
        /// <summary>
        /// Construction d'URL à partir de la List charger avec le fichier de construction d'URL sélectionné
        /// </summary>
        /// <param name="replace_query">Valeur de remplacement par le pseudo/mot recherché</param>
        /// 
        public void Construct_URL(string URLc)
        {
            URLbrowse_Cbx.Items.Clear();

            for (int i = 0; i < ConstructURL_Lst.Items.Count; i++)
            {
                string formatURI = Regex.Replace(ConstructURL_Lst.Items[i].ToString(), "replace_query", URLc);
                URLbrowse_Cbx.Items.Add(formatURI);
            }

            URLbrowse_Cbx.SelectedIndex = 0;
        }
        ///
        /// <summary>
        /// Téléchargement et enregistrement du source de la page WEB en cour, ne prend pas en compte le chargement de fichier local
        /// uniquement les fichiés distants.
        /// </summary>
        /// 
        public async void Download_Source_Page()
        {
            try
            {
                if (Class_Var.URL_USER_AGENT_SRC_PAGE == "")
                {
                    Class_Var.URL_USER_AGENT_SRC_PAGE = lstUrlDfltCnf[5].ToString();
                }

                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.UserAgent.ParseAdd(Class_Var.URL_USER_AGENT_SRC_PAGE);

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
        ///
        /// <summary>
        /// Exécution des commandes dans la console
        /// </summary>
        /// <param name="Cmd">Commande saisi dans la console traités dans le switch</param>
        /// <param name="CMD_Console_Exec">Commande regEX à éxécuter</param>
        /// <param name="yn"></param>
        /// <param value="0">CMD_Console_Exec False</param>
        /// <param value="1">CMD_Console_Exec True</param>
        /// 
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
        ///
        /// <summary>
        /// Command regEX préformatés ou aléatoires
        /// </summary>
        /// <param name="cmdSwitch">Command regEX préformatés => links | word | text link | word without duplicate</param>
        /// <param name="regxCmd">Comand regEX aléatoires saisi par l'utilisateur ==> regex</param>
        /// 
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
        ///
        /// <summary>
        /// Création de fichier avec écrasement si file exist
        /// </summary>
        /// <param name="fileName">Nom du fichier à créer</param>
        /// <param name="content">Données du fichier à inscrire</param>
        /// 
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
        ///
        /// <summary>
        /// Création de fichier de List
        /// </summary>
        /// <param name="nameFile">Nom du fichier de List</param>
        /// <param name="opnOrNo"></param>
        /// <param value="yes">Sauvegarde le fichier et ouvre la liste dans la fenêtre "Open_Source_Frm"</param>
        /// <param value="no">Sauvegarde le fichier sans l'afficher</param>
        /// 
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
        ///
        /// <summary>
        /// Création de fichier en mode ajout
        /// </summary>
        /// <param name="fileName">Nom du fichier à créer</param>
        /// <param name="fileValue">Données du fichier à inscrire</param>
        /// 
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
        ///
        /// <summary>
        /// Ouverture de fichier vers "OpenFile_Editor"
        /// </summary>
        /// <param name="dir_dir">Vérification de l'existance du fichier si False il est créé puis ouvert avec l'éditeur</param>
        /// 
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
        ///
        /// <summary>
        /// Ouverture de fichier avec l'éditeur
        /// </summary>
        /// <param name="FileSelect">Fichier à ouvrir</param>
        /// 
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
        ///
        /// <summary>
        /// Ouverture de fichier dans la fenêtre "Doc_Frm"
        /// </summary>
        /// <param name="fileNameSelect">Fichier à ouvrir</param>
        /// 
        public void Open_Doc_Frm(string fileSelect)
        {
            Class_Var.File_Open = fileSelect;
            docForm = new Doc_Frm();
            docForm.Show();
        }
        ///
        /// <summary>
        /// Ouverture de fichier dans la fenêtre "OpenSource_Frm"
        /// </summary>
        /// <param name="fileNameSelect">Fichier à ouvrir</param>
        /// 
        public void Open_Source_Frm(string fileSelect)
        {
            Class_Var.File_Open = fileSelect;
            openSourceForm = new OpenSource_Frm();
            openSourceForm.Show();
        }

        #region Database_OpenAdd
        ///
        /// <summary>
        /// Ouverture de catégorie d'URL dans wBrowser
        /// </summary>
        /// <param name="Sqlite_ReadUri">Ouvre l'URL et l'affiche dans wBrowser</param>
        ///
        public void URL_SAVE_Cbx_SelectedIndexChanged(object sender, EventArgs e)
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
        /// Affiche la section URL de la base de donnée en mode ajout
        /// </summary>
        /// <param name="Tables_Lst_Opt"></param>
        /// <param value="add">Ajout d'une URL à la base de donnée</param>
        /// <param value="open">Ouverture d'une URL de la base de donnée</param>
        /// 
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
        ///
        /// <summary>
        /// Affiche la section URL de la base de donnée en mode lecture
        /// </summary>
        /// <param name="Tables_Lst_Opt"></param>
        /// <param value="add">Ajout d'une URL à la base de donnée</param>
        /// <param value="open">Ouverture d'une URL de la base de donnée</param>
        /// 
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
        ///
        /// <summary>
        /// Affiche la section URL de la base de donnée
        /// </summary>
        /// <param name="OpnAllTable()">Ouvre et affiche toutes les catégories d'URL de la base de donnée</param>
        /// 
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
        ///
        /// <summary>
        /// Add Table, ajout d'une nouvelle catégorie dans la base de donnée
        /// </summary>
        /// <param name="Sqlite_Cmd">Commande mysql à éxécuter</param>
        /// 
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
        ///
        /// <summary>
        /// Ouverture d'une catégorie de la base de donnée ou ajout d'URL
        /// </summary>
        /// <param name="Tables_Lst_Opt"></param>
        /// <param value="add">Ajout d'une URL à la base de donnée</param>
        /// <param value="open">Ouverture d'une URL de la base de donnée</param>
        /// <param name="tlsi">var => Table List Selected Item (catégorie) de la base de donnée</param>
        /// 
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
        ///
        /// <summary>
        /// Ouverture de toutes les catégorie de la base de donnée et affichage
        /// </summary>
        /// <param name="Sqlite_Read">Commande mysql à éxécuter</param>
        /// <param name="List_Object">Objet CBX ou List à chaarger</param>
        /// 
        public void OpnAllTable()
        {
            Tables_Lst.Items.Clear();
            List_Object = Tables_Lst;

            Sqlite_Read("SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1", "name", "lst");
        }
        ///
        /// <summary>
        /// Exécution des commands mysql
        /// </summary>
        /// <param name="execSql">Commande à éxécuter</param>
        /// <param name="DBadmin"></param>
        /// <param value="on">Commandes mysql aléatoires saisi par l'utilisateur dans la section TAB Data gestion de la base de donnée</param>
        /// <param value="off">Commandes mysql préformatés Open | Add | Delete | Change</param>
        /// 
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
        ///
        /// <summary>
        /// Lecture dans la base de donnée
        /// </summary>
        /// <param name="execSql">Commande mysql à éxécuter</param>
        /// <param name="valueDB">Valeur de la Table à charger</param>
        /// <param name="ObjLstCbx">Sélection du type d'objet à charger List ou CBX</param>
        /// 
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
        ///
        /// <summary>
        /// Chargement d'URL de la base de donnée dans wBrowser ou dans le Textbox "DataValue_Opn" de la section "TAB Data"
        /// </summary>
        /// <param name="DBadmin"></param>
        /// <param value="on">Ouverture de l'URL dans Textbox DataValue_Opn</param>
        /// <param value="off">Ouverture de l'URL dans wBrowser</param>
        /// 
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
        ///
        /// <summary>
        /// Changement de la DataBase par défaut et création si no exist
        /// </summary>
        /// <param name="ChangeDBdefault">Enregistrement de la nouvelle base de donnée par défaut dans le fichier config.xml</param>
        /// 
        public void ChangeDefDB_Btn_Click(object sender, EventArgs e)
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
        /// Commande mysql de la section "TAB Data"
        /// </summary>
        /// <param name="Sqlite_Read">Commande mysql</param>
        /// 
        public void ExecuteCMDsql_Btn_Click(object sender, EventArgs e)
        {
            DataValue_Lst.Items.Clear();
            List_Object = DataValue_Lst;

            Sqlite_Read(ExecuteCMDsql_Txt.Text, ValueCMDsql_Txt.Text, "lst");
        }
        ///       
        /// <summary>
        /// Supprimer Table (catégorie)
        /// </summary>
        /// 
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
        ///       
        /// <summary>
        /// Supprimer une entrée (url d'une catégorie)
        /// </summary>
        /// 
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
        ///       
        /// <summary>
        /// Supprimer Toutes les entrées (toutes les url d'une catégorie)
        /// </summary>
        /// 
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
        ///       
        /// <summary>
        /// Renommer l'URL d'une catégorie de la base de donnée 
        /// </summary>
        /// 
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
        ///       
        /// <summary>
        /// Modifier la valeur d'une URL d'une catégorie de la base de donnée 
        /// </summary>
        /// 
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
        ///
        /// <summary>
        /// Changement de la base de donnée par défaut et modification dans le fichier "config.xml"
        /// </summary>
        /// <param name="NameDB">Nom de la base de donnée</param>
        /// 
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

        private void Db_OpnLink_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser(DataValue_Opn.Text, 0);
            CtrlTabBrowsx();
            Control_Tab.SelectedIndex = 0;
        }

        private void Db_OrderLst_Btn_Click(object sender, EventArgs e)
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

        private void Title_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Title_Lst.SelectedIndex != -1)
            {               
                WBrowsefeed.Source = new Uri(Link_Lst.Items[Title_Lst.SelectedIndex].ToString());
            }
        }
        ///
        /// <summary>
        /// Chargement de la catégorie du flux RSS dans List ou administration du fichier de catégorie de flux RSS
        /// </summary>
        /// <param name="ManageFeed"></param>
        /// <param name="on">Ouverture du fichier de catégorie de flux rss pour modification</param>
        /// <param name="off">Chargement du flux RSS de la catégorie</param>
        /// <param name="LoadFeed"></param>
        /// <param value="0">Nettoyage de la List avant chargement</param>
        /// 
        public void CategorieFeed_Cbx_SelectedIndexChanged(object sender, EventArgs e)
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
                CollapseTitleFeed_Btn.Text = "Collapse On";
            }
        }
        ///
        /// <summary>
        /// Selection du flux RSS site par site
        /// </summary>
        /// 
        private void CountBlockSite_Lbl_SelectedIndexChanged(object sender, EventArgs e)
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
        /// Chargement de la liste de Flux
        /// </summary>
        /// <param name="value">URL des Flux RSS de la catégorie </param>
        /// <param name="FeedTitle">Chargement des sites de flux RSS</param>
        /// <param name="ListCount">Compteur du nombre de Titre par site de flux RSS</param>
        /// <param name="ItemTitleAdd">Ajout du titre dans "Title_Lst"</param>
        /// <param name="ItemLinkAdd">Ajout de l'URL du titre dans Link_Lst</param>
        /// <param name="CountFeed">Affichage du nombre de titre dans "Title_Lst"</param>
        /// <param name="AllTrue">Enable les éléments</param>
        /// <param name="Msleave">Superposition de la fenêtre des site de flux RSS au dessus</param>
        /// 
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
        ///
        /// <summary>
        /// Selectionner un titre par son numéro d'index
        /// </summary>
        /// 
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
        ///
        /// <summary>
        /// Ouverture de la Page Home si configuré "@Class_Var.URL_HOME" ou ouverture de la page par défaut du fichier "url_dflt_cnf.ost"
        /// </summary>
        ///
        private void HomeFeed_Btn_Click(object sender, EventArgs e)
        {
            WBrowsefeed.Source = new Uri(HomeUrlRSS);
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
            GoBrowser("edge://downloads", 0);
        }

        private void History_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("edge://history", 0);
        }

        private async void DevTools_Param_Click(object sender, EventArgs e)
        {
            await WBrowse.EnsureCoreWebView2Async();
            WBrowse.CoreWebView2.OpenDevToolsWindow();
        }

        private void EdgeURL_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("edge://edge-urls", 0);
        }

        private void AdvancedOption_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("about:flags", 0);
        }

        private void SiteEngament_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("edge://site-engagement", 0);
        }

        private void System_Param_Click(object sender, EventArgs e)
        {
            GoBrowser("edge://system", 0);
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
        ///
        /// <summary>
        /// Injection de cookie si True
        /// </summary>
        /// CookieName_Txt | CookieValue_Txt | CookieValue_Txt
        /// 
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
        ///
        /// <summary>
        /// Crétion du fichier XML pour un nouveau projet WorkFlow
        /// Vérification Word Duplicate
        /// Suppression Espace et Saut de ligne
        /// </summary>
        /// 
        private void CreateXMLwf_btn_Click(object sender, EventArgs e)
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

                using (StreamWriter file_create = new StreamWriter(fileTmp))
                {
                    file_create.Write(AddItemswf_Txt.Text);
                }

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
                /// Ajout d'items et Reformattage => suppression des epaces et des sauts de lignes
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

        private void CreateNode(string pValue, XmlTextWriter writer)
        {
            writer.WriteComment(pValue);
            writer.WriteEndElement();
        }
        ///
        /// <summary>
        /// Ajout d'item dans le fichier XML du projet WorkFlow et rechargement des valeurs
        /// </summary>
        /// <param name="AddDataWorkflow"></param>
        /// <param value="y">Ajout des attributs quand une nouvelle valeur est ajouté</param>
        /// <param value="n">Pas d'ajout d'attributs quand un nouvel Item est ajouté</param>
        /// <param name="LoadItemKeyword_Thr"></param>
        /// <param value="y">Reload "Workflow_Cbx" (<= vérifie si un item n'est pas déja éxistant) "Workflow_Lst" "AddItemswf_Txt" avec le nouvel Item créé</param>
        /// <param value="n">Création d'une List des items enregistrés et affichage</param>
        /// 
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
            ModelEdit_Btn.Enabled = true;
        }
        ///
        /// <summary>
        /// Chargement de la Timeline et des Items
        /// </summary>
        /// <param name="LoadItemKeyword_Thr"></param>
        /// <param value="y"></param>
        /// Reload "Workflow_Cbx" (<= vérifie si un item n'est pas déja éxistant) "Workflow_Lst" "AddItemswf_Txt"
        /// <param value="n"></param>
        /// Création d'une List des items enregistrés et affichage
        /// 
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
        /// Ajout d'un enregistrement dans le fichier XML du projet WorkFlow à partir de la zone de texte ou du clipboard
        /// </summary>
        /// <param name="FormatValue()">Suppression des espaces et des sauts de ligne et envoie vers => "AddDataWorkflow"</param>
        /// 
        private void WorkflowItem_Lst_SelectedIndexChanged(object sender, EventArgs e)
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
        /// Suppression des espaces et des sauts de ligne et envoie vers => "AddDataWorkflow"
        /// </summary>
        /// Encapsulation "[[ ]]" des URL pour qu'elles soient cliquable dans le diagramme SVG
        /// <param name="AddDataWorkflow">Ajout des données dans le fichier XML du projet WorkFlow</param>
        /// <param value="y">Ajout des attributs quand une nouvelle valeur est ajouté</param>
        /// <param value="n">Pas d'ajout d'attributs quand un nouvel Item est ajouté</param>
        /// 
        private void FormatValue()
        {
            try
            {
                string element = WorkflowItem_Lst.SelectedItem.ToString();
                element += "_" + element;
                ///
                /// Reformattage => suppression des epaces et des sauts de lignes
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
        /// Ajout de donnée dans Item du projet WorkFlow
        /// </summary>
        /// <param name="nodeselect">XML => Table/KeywordItemCollect/Item</param>
        /// <param name="elementselect">Aléatoire Item</param>
        /// <param name="value"></param>
        /// <param value="y">Ajout des attributs quand une nouvelle valeur est ajouté</param>
        /// <param value="n">Pas d'ajout d'attributs quand un nouvel Item est ajouté</param>
        /// <param name="LoadStatWorkflow()">Affichage des statistiques d'enregistrements</param>
        /// 
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
                        if (AddTNoteWorkflow_Txt.Text != "")
                        {
                            ///
                            /// Reformattage => suppression des epaces et des sauts de lignes
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
                            /// Reformattage => suppression des epaces et des sauts de lignes
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
                    node1.AppendChild(elem);
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
        /// Affichage de la section d'ajouts de données des projet WorkFlow dans "TAB BROWSx"
        /// </summary>
        /// <
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
        ///
        /// <summary>
        /// Affichage des données d'items de projet WorkFlow
        /// </summary>
        /// <param name="LoadItemKeyword_Thr"></param>
        /// <param value="y">Reload Workflow_Cbx (<= vérifie si un item n'est pas déja éxistant) Workflow_Lst AddItemswf_Txt</param>
        /// <param value="n">Création d'une List des items enregistrés et affichage</param>
        /// 
        private void StatWorkflow_Lst_SelectedIndexChanged(object sender, EventArgs e)
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
        /// <summary>
        /// Chargement des Items
        /// </summary>
        /// <param name="nodeselect">XML => Table/KeywordItemCollect/Item</param>
        /// <param name="elementselect">Aléatoire Item</param>
        /// <param name="LoadStat"></param>
        /// <param value="y">Reload Workflow_Cbx (<= vérifie si un item n'est pas déja éxistant) Workflow_Lst AddItemswf_Txt</param>
        /// <param value="n">Création d'une List des items enregistrés et affichage</param>
        /// 
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
        /// <summary>
        /// Affichage des statistique de données récupérées
        /// </summary>
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
        ///
        /// <summary>
        /// Création de fichier Model pour l'ajout d'Item de projet WorkFlow
        /// </summary>
        /// 
        private void ModelCreate_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ModelName_Txt.Text != "" && ModelItem_Txt.Text != "")
                {
                    ///
                    /// Reformattage => suppression des epaces et des sauts de lignes
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

        private void ModelEdit_Btn_Click(object sender, EventArgs e)
        {
            if (ModelList_Lst.SelectedIndex != -1)
            {
                string filePath = WorkflowModel + ModelList_Lst.SelectedItem.ToString();

                if (File.Exists(filePath))
                    OpenFile_Editor(filePath);
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
        ///
        /// <summary>
        /// Sauvegarde de la configuration "config.xml" et accés au différents fichiers de configuration
        /// </summary>
        /// <param name="CreateConfigFile"></param>
        /// <param value="0">Chargement des URL par défaut du fichier "url_dflt_cnf.ost"</param>
        /// <param value="1">Sauvegarde avec les paramètres choisi</param>
        /// 
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
            if (Directory.Exists(FileDir + "url-constructor"))
                Process.Start(FileDir + "url-constructor");
        }

        private void AddOntools_Opt_Btn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Plugins))
                Process.Start(Plugins);
        }

        private void MultipleWin_Opt_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(FileDir + @"grp-frm\grp_frm_url_opn.txt");
        }

        private void MultipleWinDir_Opt_Btn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(FileDir + "grp-frm"))
                Process.Start(FileDir + "grp-frm");
        }

        private void GoogleDork_Opt_Btn_Click(object sender, EventArgs e)
        {
            OpnFileOpt(FileDir + "gdork.txt"); 
        }

        private void OstiumDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(AppStart))
                Process.Start(AppStart);
        }

        private void AddOnDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Plugins))
                Process.Start(Plugins);
        }

        private void DatabaseDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(DBdirectory))
                Process.Start(DBdirectory);
        }

        private void FeedDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(FeedDir))
                Process.Start(FeedDir);
        }

        private void ScriptDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Scripts))
                Process.Start(Scripts);
        }

        private void WorkFlowDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Workflow))
                Process.Start(Workflow);
        }

        private void WorkFlowModelDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(WorkflowModel))
                Process.Start(WorkflowModel);
        }

        private void PictureDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Pictures))
                Process.Start(Pictures);
        }

        private void WebView2Dir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(WebView2Dir))
                Process.Start(WebView2Dir);
        }

        private void DiagramDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(DiagramDir))
                Process.Start(DiagramDir);
        }

        private void SpritesDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Setirps))
                Process.Start(Setirps);
        }

        private void BkmkltDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(BkmkltDir))
                Process.Start(BkmkltDir);
        }

        private void MapDir_Opn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(MapDir))
                Process.Start(MapDir);
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
        /// Vérifie si process True
        /// </summary>
        /// <param name="ProcessVerif">Nom du process à vérifier</param>
        /// 
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
        ///
        /// <summary>
        /// Vérifie si process True et Kill process
        /// </summary>
        /// <param name="ProcessKill">Nom du process à Kill</param>
        /// 
        private void KillProcessJAVAW(string ProcessKill)
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
        /// Vérifie si le process javaw de plantUML est TRUE si False affichage du Diagramme
        /// </summary>
        /// <param name="Timo_Tick">Timo start quand création d'un Diagramme</param>      
        /// <param name="CreateDiagram_Invk">Affichage du Diagramme lorsque le process est False (terminé)</param>
        /// 
        private void Timo_Tick(object sender, EventArgs e)
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
        /// Unshort URL affichage de l'URL réel
        /// </summary>
        /// <param name="value">URL réel</param>
        /// 
        public void URLbrowseCbxText(string value)
        {
            URLbrowse_Cbx.Text = value;
            URLtxt_txt.Text = string.Format("Unshorten URL => {0}", value);
        }
        ///
        /// <summary>
        /// Vidage création et remplissage de List
        /// </summary>
        /// <param name="value">Action à réaliser</param>
        /// 
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
        ///
        /// <summary>
        /// Chargement des Items pour les projest WorkFlow
        /// </summary>
        /// <param name="value">Item Add</param>
        /// 
        private void LoadItemKeyword_Invk(string value)
        {
            Workflow_Cbx.Items.Add(value);
            Workflow_Lst.Items.Add(value);
            AddItemswf_Txt.AppendText(value + "\r\n");            
        }
        ///
        /// <summary>
        /// Affichage des statistiques du projet WorkFlow open
        /// </summary>
        /// 
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
        ///
        /// <summary>
        /// Ouverture du diagramme dans un nouvel ongle wBrowser
        /// </summary>
        /// <param name="value"></param>
        /// <param value="0">Le fichier de création du diagramme est dans le répertoire diagram</param>        
        /// <param value="1">Le fichier de création du diagramme est dans un répertoire aléatoire</param>
        /// 
        private void CreateDiagram_Invk(string value)
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

        private void OpnBokmark_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (PanelBkmklt_Pnl.Visible)
                {
                    PanelBkmklt_Pnl.Visible = false;
                }
                else
                {
                    loadfiledir.LoadFileDirectory(BkmkltDir, "xml", "lst", Bookmarklet_Lst);
                    PanelBkmklt_Pnl.Visible = true;
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpnBokmark_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void Bookmarklet_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            string BkmScr = Regex.Replace(Bookmarklet_Lst.Text, @".xml", "");
            OpnBookmark(BkmScr);
        }

        private void OpnBookmark(string strAttrib)
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

        private void InjectBkmklt_Btn_Click(object sender, EventArgs e)
        {
            if (Bookmarklet_Lst.SelectedIndex != -1)
            {
                InjectBkmklt(MinifyScr);
            }                
        }

        public async void InjectBkmklt(string Bkmklt)
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

        private void ClosePnlBkmklt_Btn_Click(object sender, EventArgs e)
        {
            PanelBkmklt_Pnl.Visible = false;
        }

        #endregion

        #region Maps_

        public void OpenMaps(string adress, int provid)
        {
            try
            {
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
                marker.ToolTip.TextPadding = new Size(20, 10);
                if (TxtMarker_Chk.Checked)
                    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                markers.Markers.Add(marker);
                GMap_Ctrl.Overlays.Add(markers);
                GMap_Ctrl.Zoom = MapZoom;
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenMaps: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void NewProjectMap_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                string message, title;
                object NameInsert;

                message = "Select Name Project.";
                title = "Project Name";

                NameInsert = Interaction.InputBox(message, title);
                string ValName = Convert.ToString(NameInsert);

                if (ValName != "")
                {
                    XmlTextWriter writer = new XmlTextWriter(MapDir + ValName + ".xml", Encoding.UTF8);
                    writer.WriteStartDocument(true);
                    writer.Formatting = System.Xml.Formatting.Indented;
                    writer.Indentation = 2;

                    writer.WriteStartElement("Table");

                    writer.WriteStartElement("Location");
                    writer.WriteEndElement();

                    writer.WriteEndDocument();
                    writer.Close();

                    MessageBox.Show("XML File created.");

                    PointLoc_Lst.Items.Clear();
                    loadfiledir.LoadFileDirectory(MapDir, "xml", "lst", PointLoc_Lst);

                    ProjectMapOpn_Lbl.Text = "";
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! NewProject_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        private void OpnDirMap_Tls_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(MapDir))
                Process.Start(MapDir);
        }

        private void EditXMLMap_Tls_Click(object sender, EventArgs e)
        {
            if (MapXmlOpn == "")
            {
                MessageBox.Show("No project selected! Select one.");
                return;
            }
            OpnFileOpt(MapXmlOpn);
        }

        private void ShowXMLMap_Tls_Click(object sender, EventArgs e)
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

        private void DelProjectMap_Tls_Click(object sender, EventArgs e)
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

        private void OpnListLocation_Tls_Click(object sender, EventArgs e)
        {
            if (Map_Cmd_Pnl.Visible)
            {
                Map_Cmd_Pnl.Visible = false;
            }
            else
            {
                Map_Cmd_Pnl.Visible = true;
            }
        }

        private void CrossCenter_Tls_Click(object sender, EventArgs e)
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

        public void ScreenShotGmap_Tls_Click(object sender, EventArgs e)
        {
            CreateNameAleat();

            try
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "PNG (*.png)|*.png";
                    dialog.FileName = UsenameAleatoire + "_Ostium_image";
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

        private void OpnBingMap_Tls_Click(object sender, EventArgs e)
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

        private void OpenGoogleEarth_Tls_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter addtxt = new StreamWriter(MapDir + "temp.kml"))
                {
                    addtxt.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    addtxt.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\" xmlns:kml=\"http://www.opengis.net/kml/2.2\" xmlns:atom=\"http://www.w3.org/2005/Atom\">");
                    addtxt.WriteLine("<Document>");
                    addtxt.WriteLine("	<Placemark>");
                    addtxt.WriteLine("		<name>Ostium location</name>");
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

        private void CopyGeoMap_Tls_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, LatTCurrent_Lbl.Text + " " + LonGtCurrent_Lbl.Text);
        }

        public void GoLatLong_Tls_Click(object sender, EventArgs e)
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

        private void GoWord_Tls_Click(object sender, EventArgs e)
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

        private void AddNewLoc_Btn_Click(object sender, EventArgs e)
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

                XmlDocument doc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(MapXmlOpn);
                doc.Load(xmlReader);

                if (doc.SelectSingleNode("/Table/Location") is XmlElement node1)
                {
                    XmlElement elem = doc.CreateElement("Point_Point");
                    elem.SetAttribute("latitude", LatTCurrent_Lbl.Text);
                    elem.SetAttribute("longitude", LonGtCurrent_Lbl.Text);
                    elem.SetAttribute("textmarker", TextMarker_Txt.Text);
                    elem.InnerText = LocationName_Txt.Text;
                    node1.AppendChild(elem);
                }

                xmlReader.Close();
                doc.Save(MapXmlOpn);
                OpnLocationPoints();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddNewLoc_Btn_Click: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void GmapProvider_Cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            GmapProviderSelect(GmapProvider_Cbx.SelectedIndex);
            GMap_Ctrl.Focus();
        }

        public void GmapProviderSelect(int val)
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

        private void GoLatLong(string lat, string lon, string tooltxt)
        {
            try
            {
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
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! GoLatLong: ", ex.Message, "Main_Frm", AppStart);
            }
        }

        public void GMap_Ctrl_OnPositionChanged(PointLatLng point)
        {            
            LatTCurrent_Lbl.Text = point.Lat.ToString(CultureInfo.InvariantCulture);
            LonGtCurrent_Lbl.Text = point.Lng.ToString(CultureInfo.InvariantCulture);
        }

        public void GMap_Ctrl_OnMapZoomChanged()
        {
            //double xZoom = GMap_Ctrl.Zoom;
            ZoomValMap_Lbl.Text = GMap_Ctrl.Zoom.ToString(); //Convert.ToString(xZoom);
        }

        private void PointLoc_Lst_SelectedIndexChanged(object sender, EventArgs e)
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

        private void OpnLocationPoints()
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

        private void VerifySizeZoom()
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

        #endregion

        #region Update_
        ///
        /// <summary>
        /// Vérification des mises à jour via requête Http et comparaison avec la variable => "versionNow"
        /// </summary>
        /// <param name="softName">Nom de l'application</param>
        /// <param name="annoncE"></param>
        /// <param value="0">Vérification auto pas d'annonce si mise a jour False annonce si True</param>
        /// <param value="1">Vérification manuel annonce si False ou True</param>
        /// <param name="AnnonceUpdate">Message de mise à jour available</param>
        /// 
        private async void VerifyUPDT(string softName, int annoncE)
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
        /// Annonce si mise à Jour True
        /// </summary>
        /// <param name="softName">Nom de l'application</param>
        /// 
        private void AnnonceUpdate(string softName)
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
