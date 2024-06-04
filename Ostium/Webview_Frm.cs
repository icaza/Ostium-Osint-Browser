using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Ostium
{
    public partial class Webview_Frm : Form
    {
        #region Var_

        [DllImport("kernel32.dll")]
        static extern bool Beep(int freq, int duration);

        readonly string AppStart = Application.StartupPath + @"\";
        readonly List<string> lstUrlDfltCnf = new List<string>();

        #endregion

        public Webview_Frm()
        {
            InitializeComponent();
            WBrowse_EventHandlers(WBrowse);

            URLbrowse_Cbx.KeyPress += new KeyPressEventHandler(OnKey_URLbrowse);
        }

        void Webview_Frm_Load(object sender, EventArgs e)
        {
            WBrowse.Source = new Uri(@Class_Var.URL_WEBVIEW);
            ///
            /// Loading default configuration URLs into a List
            /// 
            if (File.Exists(AppStart + "url_dflt_cnf.ost"))
                lstUrlDfltCnf.AddRange(File.ReadAllLines(AppStart + "url_dflt_cnf.ost"));
        }

        #region Browser_Event Handler

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
                    GoBrowser("http://web.archive.org/web/*/" + pageUri);
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
                    GoBrowser("https://www.youtube.com/results?search_query=" + pageUri);
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
                        GoBrowser("file:///" + AppStart + "tmpytb.html");
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

        void WBrowse_UpdtTitleEvent(string message)
        {
            string currentDocumentTitle = WBrowse?.CoreWebView2?.DocumentTitle ?? "Uninitialized";
            Text = currentDocumentTitle + " [" + message + "]";
        }

        void WBrowse_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            WBrowse_UpdtTitleEvent("Navigation Starting");
        }
        ///
        /// <summary>
        /// Save cookies
        /// </summary>
        /// <param name="GetCookie">Save all cookies in the cookie.txt file at the root if SaveCookies_Chk checked = True</param>
        /// 
        void WBrowse_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (Class_Var.COOKIES_SAVE == 1)
                GetCookie(WBrowse.Source.AbsoluteUri);

            WBrowse_UpdtTitleEvent("Navigation Completed");
        }

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

        void WBrowse_DocumentTitleChanged(object sender, object e)
        {
            Text = WBrowse.CoreWebView2.DocumentTitle;
            WBrowse_UpdtTitleEvent("DocumentTitleChanged");
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

        #endregion

        #region Control_Browser

        void Go_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser(URLbrowse_Cbx.Text);
        }

        void GoBrowser(string URIopn)
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
                            Class_Var.URL_DEFAUT_WSEARCH = lstUrlDfltCnf[3].ToString();

                        uri = new Uri(Class_Var.URL_DEFAUT_WSEARCH +
                            string.Join("+", Uri.EscapeDataString(rawUrl).Split(new string[] { "%20" }, StringSplitOptions.RemoveEmptyEntries)));
                    }
                }

                WBrowse.Source = uri;
            }
            catch
            { }
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

        void Home_Btn_Click(object sender, EventArgs e)
        {
            if (@Class_Var.URL_HOME == "")
                @Class_Var.URL_HOME = lstUrlDfltCnf[1].ToString();

            WBrowse.Source = new Uri(@Class_Var.URL_HOME);
        }

        void Trad_Btn_Click(object sender, EventArgs e)
        {
            if (Class_Var.URL_TRAD_WEBPAGE == "")
                Class_Var.URL_TRAD_WEBPAGE = lstUrlDfltCnf[2].ToString();

            string formatURI = Regex.Replace(Class_Var.URL_TRAD_WEBPAGE, "replace_query", WBrowse.Source.AbsoluteUri);
            WBrowse.Source = new Uri(@formatURI);
        }

        void CopyURL_Btn_Click(object sender, EventArgs e)
        {
            string textData = WBrowse.Source.AbsoluteUri;
            Clipboard.SetData(DataFormats.Text, textData);
            Beep(1500, 400);
        }

        #endregion

        void JavaEnableDisable_Btn_Click(object sender, EventArgs e)
        {
            var settings = WBrowse.CoreWebView2.Settings;
            settings.IsScriptEnabled = !settings.IsScriptEnabled;

            if (JavaEnableDisable_Btn.Text == "Javascript Enable")
            {
                JavaEnableDisable_Btn.Text = "Javascript Disable";
                JavaEnableDisable_Btn.ForeColor = Color.Red;
            }
            else
            {
                JavaEnableDisable_Btn.Text = "Javascript Enable";
                JavaEnableDisable_Btn.ForeColor = Color.Lime;
            }
        }

        void OnKey_URLbrowse(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                GoBrowser(URLbrowse_Cbx.Text);
        }
        ///
        /// <summary>
        /// Cookies save
        /// </summary>
        /// <param value="URLs">Saved cookies only if SaveCookies_Chk checked = True,  by default is False</param>
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
                    cookieResult.Append($"\n\r{cookie.Name} {cookie.Value} {(cookie.IsSession ? "[session cookie]" : cookie.Expires.ToString("G"))}");
                }

                CreateData(Application.StartupPath + @"\cookie.txt", cookieResult.ToString());
                CreateData(Application.StartupPath + @"\cookie.txt", "\r\n+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+\r\n");
            }
            catch
            { }
        }

        void CreateData(string fileName, string fileValue)
        {
            try
            {
                using (StreamWriter file_create = File.AppendText(fileName))
                {
                    file_create.WriteLine(fileValue);
                }
            }
            catch
            { }
        }
    }
}
