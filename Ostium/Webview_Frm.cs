using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Ostium
{
    public partial class Webview_Frm : Form
    {
        #region Var_

        readonly string AppStart = Application.StartupPath + @"\";
        readonly List<string> lstUrlDfltCnf = new List<string>();

        FloodHeader TrackingFlood;

        #endregion

        public Webview_Frm()
        {
            InitializeComponent();
            WBrowsew_EventHandlers(WBrowsew);

            URLbrowse_Cbx.KeyPress += new KeyPressEventHandler(OnKey_URLbrowse);
        }

        void Webview_Frm_Load(object sender, EventArgs e)
        {
            WBrowsew.Source = new Uri(@Class_Var.URL_WEBVIEW);
            ///
            /// Loading default configuration URLs into a List
            /// 
            if (File.Exists(Path.Combine(AppStart, "url_dflt_cnf.ost")))
                lstUrlDfltCnf.AddRange(File.ReadAllLines(Path.Combine(AppStart, "url_dflt_cnf.ost")));
        }

        #region Browser_Event Handler

        void WBrowsew_ContextMenuRequested(object sender, CoreWebView2ContextMenuRequestedEventArgs args)
        {
            string UriYoutube = string.Empty;
            string C = WBrowsew.Source.AbsoluteUri;
            if (C.Length > 32)
                UriYoutube += C.Substring(0, 32);

            IList<CoreWebView2ContextMenuItem> menuList = args.MenuItems;

            CoreWebView2ContextMenuItem newItem0 = WBrowsew.CoreWebView2.Environment.CreateContextMenuItem("Search on WayBackMachine", null, CoreWebView2ContextMenuItemKind.Command);
            newItem0.CustomItemSelected += delegate (object send, object ex)
            {
                string pageUri = args.ContextMenuTarget.PageUri; ;

                SynchronizationContext.Current.Post((_) =>
                {
                    GoBrowser("http://web.archive.org/web/*/" + pageUri);
                }, null);
            };

            CoreWebView2ContextMenuItem newItem1 = WBrowsew.CoreWebView2.Environment.CreateContextMenuItem("Youtube embed", null, CoreWebView2ContextMenuItemKind.Command);
            newItem1.CustomItemSelected += delegate (object send, object ex)
            {
                SynchronizationContext.Current.Post((_) =>
                {
                    if (UriYoutube == "https://www.youtube.com/watch?v=")
                    {
                        string pageUri = WBrowsew.Source.AbsoluteUri;
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
        }

        void WBrowsew_UpdtTitleEvent(string message)
        {
            string currentDocumentTitle = WBrowsew?.CoreWebView2?.DocumentTitle ?? "Uninitialized";
            Text = currentDocumentTitle + " [" + message + "]";
        }

        async void WBrowsew_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (@Class_Var.FLOOD_HEADER == 1)
            {
                await WBrowsew.EnsureCoreWebView2Async();
                _ = new WebViewHandler(WBrowsew.CoreWebView2, "config.json");

                TrackingFlood = new FloodHeader(WBrowsew.CoreWebView2);
                await TrackingFlood.FloodHeaderAsync();
            }

            WBrowsew_UpdtTitleEvent("Navigation Starting");
        }
        ///
        /// <summary>
        /// Save cookies
        /// </summary>
        /// <param name="GetCookie">Save all cookies in the cookie.txt file at the root if SaveCookies_Chk checked = True</param>
        /// 
        void WBrowsew_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (Class_Var.COOKIES_SAVE == 1)
                GetCookie(WBrowsew.Source.AbsoluteUri);

            WBrowsew_UpdtTitleEvent("Navigation Completed");
        }

        void WBrowsew_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            URLtxt_txt.Text = WBrowsew.Source.AbsoluteUri;
            WBrowsew_UpdtTitleEvent("Source Changed");
        }

        void WBrowsew_HistoryChanged(object sender, object e)
        {
            Back_Btn.Enabled = WBrowsew.CoreWebView2.CanGoBack;
            Forward_Btn.Enabled = WBrowsew.CoreWebView2.CanGoForward;
            WBrowsew_UpdtTitleEvent("History Changed");
        }

        void WBrowsew_DocumentTitleChanged(object sender, object e)
        {
            Text = WBrowsew.CoreWebView2.DocumentTitle;
            WBrowsew_UpdtTitleEvent("DocumentTitleChanged");
        }

        void WBrowsew_InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                MessageBox.Show($"WBrowse creation failed with exception = {e.InitializationException}");
                WBrowsew_UpdtTitleEvent("Initialization Completed failed");
                return;
            }

            WBrowsew.CoreWebView2.HistoryChanged += WBrowsew_HistoryChanged;
            WBrowsew.CoreWebView2.DocumentTitleChanged += WBrowsew_DocumentTitleChanged;
            WBrowsew.CoreWebView2.ContextMenuRequested += WBrowsew_ContextMenuRequested;

            WBrowsew.CoreWebView2.Settings.AreHostObjectsAllowed = false;
            WBrowsew.CoreWebView2.Settings.IsWebMessageEnabled = false;
            WBrowsew.CoreWebView2.Settings.IsScriptEnabled = true;
            WBrowsew.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;

            WBrowsew_UpdtTitleEvent("Initialization Completed succeeded");
        }

        void WBrowsew_EventHandlers(Microsoft.Web.WebView2.WinForms.WebView2 control)
        {
            control.CoreWebView2InitializationCompleted += WBrowsew_InitializationCompleted;
            control.NavigationStarting += WBrowsew_NavigationStarting;
            control.NavigationCompleted += WBrowsew_NavigationCompleted;
            control.SourceChanged += WBrowsew_SourceChanged;
        }

        #endregion

        #region Control_Browser

        void Go_Btn_Click(object sender, EventArgs e)
        {
            GoBrowser(URLbrowse_Cbx.Text);
        }

        void GoBrowser(string inputUrl)
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
                            MessageBox.Show("Error! lstUrlDfltCnf GoBrowser: " + ex.Message, "Error!");
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

                WBrowsew.Source = uri;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! GoBrowser: " + ex.Message, "Error!");
            }
        }

        void Back_Btn_Click(object sender, EventArgs e)
        {
            WBrowsew.GoBack();
        }

        void Forward_Btn_Click(object sender, EventArgs e)
        {
            WBrowsew.GoForward();
        }

        void Refresh_Btn_Click(object sender, EventArgs e)
        {
            WBrowsew.Reload();
        }

        void Home_Btn_Click(object sender, EventArgs e)
        {
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
                    MessageBox.Show("Error! lstUrlDfltCnf Home_Btn_Click: " + ex.Message, "Error!");
                }
            }

            WBrowsew.Source = new Uri(@Class_Var.URL_HOME);
        }

        void Trad_Btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(@Class_Var.URL_TRAD_WEBPAGE))
            {
                try
                {
                    if (lstUrlDfltCnf.Count > 0)
                        @Class_Var.URL_TRAD_WEBPAGE = lstUrlDfltCnf[2].ToString();
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("The url_dflt_cnf.ost file is corrupted! Go to Ostium GitHub page to download this " +
                        "missing file or reinstall Ostium.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show("Error! lstUrlDfltCnf Trad_Btn_Click: " + ex.Message, "Error!");
                }
            }

            string formatURI = Regex.Replace(Class_Var.URL_TRAD_WEBPAGE, "replace_query", WBrowsew.Source.AbsoluteUri);
            WBrowsew.Source = new Uri(@formatURI);
        }

        void CopyURL_Btn_Click(object sender, EventArgs e)
        {
            string textData = WBrowsew.Source.AbsoluteUri;
            Clipboard.SetData(DataFormats.Text, textData);
            Console.Beep(1500, 400);
        }

        #endregion

        void JavaScriptToggle_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (WBrowsew?.CoreWebView2?.Settings != null)
                {
                    var settings = WBrowsew.CoreWebView2.Settings;
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
                JavaScriptToggle_Btn.ForeColor = Color.Lime;
            }
            else
            {
                JavaScriptToggle_Btn.Text = "JavaScript Disabled";
                JavaScriptToggle_Btn.ForeColor = Color.Red;
            }
        }

        void OnKey_URLbrowse(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                GoBrowser(URLbrowse_Cbx.Text);
            }
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
                List<CoreWebView2Cookie> cookieList = await WBrowsew.CoreWebView2.CookieManager.GetCookiesAsync(URLs);
                StringBuilder cookieResult = new StringBuilder(cookieList.Count + " cookie(s) received from " + URLs + " [DATE] " + DateTime.Now.ToString("F"));

                for (int i = 0; i < cookieList.Count; ++i)
                {
                    CoreWebView2Cookie cookie = WBrowsew.CoreWebView2.CookieManager.CreateCookieWithSystemNetCookie(cookieList[i].ToSystemNetCookie());
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
