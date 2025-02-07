using Icaza;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static GMap.NET.Entity.OpenStreetMapGraphHopperRouteEntity;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Ostium
{
    public partial class HtmlText_Frm : Form
    {
        #region Var_

        [DllImport("kernel32.dll")]
        static extern bool Beep(int freq, int duration);

        readonly IcazaClass senderror = new IcazaClass();
        readonly IcazaClass selectdir = new IcazaClass();

        readonly string AppStart = Application.StartupPath + @"\";
        string UriTxt = string.Empty;
        string Una = string.Empty;

        private static readonly HttpClient client = new HttpClient();

        #endregion

        public HtmlText_Frm()
        {
            InitializeComponent();

            WbrowseTxt.LinkClicked += new LinkClickedEventHandler(RTFLink_Clicked);
            URLbrowse_Cbx.KeyPress += new KeyPressEventHandler(URLbrowseCbx_Keypress);
        }

        void Go_Btn_Click(object sender, EventArgs e)
        {
            StartOpenWebPageTxt();
        }

        void URLbrowseCbx_Keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                StartOpenWebPageTxt();
        }

        void StartOpenWebPageTxt()
        {
            try
            {
                if (!string.IsNullOrEmpty(URLbrowse_Cbx.Text))
                {
                    var inputUrl = URLbrowse_Cbx.Text;
                    Uri uri;

                    if (inputUrl.Contains("file:///"))
                    {
                        MessageBox.Show("HTML Text does not support locale files!", "WEB only", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
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
                            Class_Var.URL_DEFAUT_WSEARCH = "https://www.google.com/search?q=";
                        }

                        uri = new Uri(Class_Var.URL_DEFAUT_WSEARCH + Uri.EscapeDataString(inputUrl));
                    }

                    URLbrowse_Cbx.Text = Convert.ToString(uri);

                    WbrowseTxt.Text = string.Empty;
                    ListLinks_Lst.Items.Clear();

                    HashSet<string> Urls = new HashSet<string>();

                    if (URLbrowse_Cbx != null && !string.IsNullOrEmpty(URLbrowse_Cbx.Text))
                    {
                        try
                        {
                            if (Urls.Add(URLbrowse_Cbx.Text))
                            {
                                URLbrowse_Cbx.Items.Add(URLbrowse_Cbx.Text);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error in URLbrowse_Cbx operation: {ex.Message}");
                        }
                    }

                    UriTxt = URLbrowse_Cbx.Text;

                    Thread Thr_OpenWebPageTxt = new Thread(new ThreadStart(OpenWebPageTxt));
                    Thr_OpenWebPageTxt.Start();
                }
                else
                {
                    MessageBox.Show("Enter Url first!");
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! StartOpenWebPageTxt: ", ex.ToString(), "HtmlText_Frm", AppStart);
            }
        }

        async void OpenWebPageTxt()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UriTxt))
                {
                    senderror.ErrorLog("Error! OpenWebPageTxt: ", "URL is empty or null", "HtmlText_Frm", AppStart);
                    return;
                }

                if (!Uri.IsWellFormedUriString(UriTxt, UriKind.Absolute))
                {
                    senderror.ErrorLog("Error! OpenWebPageTxt: ", "Invalid URL format", "HtmlText_Frm", AppStart);
                    return;
                }

                string userAgent = string.IsNullOrWhiteSpace(UserAgent_Txt.Text) ? "Mozilla/5.0" : UserAgent_Txt.Text;
                client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

                HttpResponseMessage response = await client.GetAsync(UriTxt);
                if (!response.IsSuccessStatusCode)
                {
                    senderror.ErrorLog("Error! OpenWebPageTxt: ", $"HTTP Error {response.StatusCode}", "HtmlText_Frm", AppStart);
                    return;
                }

                string pageContents = await response.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageContents);

                var sb = new StringBuilder();
                foreach (var node in doc.DocumentNode.DescendantsAndSelf())
                {
                    if (!node.HasChildNodes && node.ParentNode != null &&
                        node.ParentNode.Name != "script" && node.ParentNode.Name != "style")
                    {
                        string text = node.InnerText.Trim();
                        if (!string.IsNullOrEmpty(text))
                        {
                            sb.AppendLine(text);
                        }
                    }
                }

                var links = Regex.Matches(pageContents, @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*")
                                 .Cast<Match>()
                                 .Select(m => m.Value)
                                 .ToList();

                Invoke(new Action(() =>
                {
                    foreach (string link in links)
                        AddLinkList(link);

                    string cleanText = Regex.Replace(sb.ToString(), @"(&.+?;)|\n\r", string.Empty);
                    RegReplaceTxt(cleanText);
                }));
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenWebPageTxt: ", ex.ToString(), "HtmlText_Frm", AppStart);
            }
        }

        void CreateNameAleat()
        {
            Una = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + "_" + Guid.NewGuid().ToString("N");
        }

        #region Invoke_

        void RegReplaceTxt(string value)
        {
            WbrowseTxt.Text = value;
        }

        void AddLinkList(string value)
        {
            int URLs;
            URLs = ListLinks_Lst.FindStringExact(value);
            if (URLs == -1)
            {
                ListLinks_Lst.Items.Add(value);
            }
        }

        #endregion

        void RTFLink_Clicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                URLbrowse_Cbx.Text = e.LinkText;

                int x;
                x = URLbrowse_Cbx.FindStringExact(URLbrowse_Cbx.Text);
                if (x == -1)
                    URLbrowse_Cbx.Items.Add(e.LinkText);

                StartOpenWebPageTxt();
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! RTFLink_Clicked: ", ex.ToString(), "HtmlText_Frm", AppStart);
            }
        }

        void ListLinks_Lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListLinks_Lst.SelectedIndex != -1)
            {
                URLbrowse_Cbx.Text = ListLinks_Lst.SelectedItem.ToString();

                int x;
                x = URLbrowse_Cbx.FindStringExact(URLbrowse_Cbx.Text);
                if (x == -1)
                    URLbrowse_Cbx.Items.Add(ListLinks_Lst.SelectedItem.ToString());

                StartOpenWebPageTxt();
            }
        }

        void CopyUrl_Btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(URLbrowse_Cbx.Text))
            {
                string textData = URLbrowse_Cbx.Text;
                Clipboard.SetData(DataFormats.Text, textData);
                Beep(1500, 400);
            }
        }

        void SavePageTxt_Btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(WbrowseTxt.Text))
            {
                string dirselect = selectdir.Dirselect();
                string namef = UriTxt;
                string urlName = GenerateFileName(namef);

                CreateNameAleat();

                try
                {
                    if (!string.IsNullOrEmpty(dirselect))
                    {
                        using (StreamWriter fc = File.AppendText(dirselect + @"\" + Una + "_" + urlName + ".txt"))
                        {
                            fc.WriteLine(WbrowseTxt.Text);
                        }
                        Beep(800, 200);
                    }
                }
                catch (Exception ex)
                {
                    senderror.ErrorLog("Error! SavePageTxt_Btn_Click: ", ex.ToString(), "HtmlText_Frm", AppStart);
                }
            }
        }

        void EmptyCbx_Btn_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Items.Clear();
        }

        string GenerateFileName(string url)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(url));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
