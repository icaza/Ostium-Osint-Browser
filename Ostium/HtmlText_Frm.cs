﻿using Icaza;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
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
                if (URLbrowse_Cbx.Text != string.Empty)
                {
                    var rawUrl = URLbrowse_Cbx.Text;
                    Uri uri;

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

                    URLbrowse_Cbx.Text = Convert.ToString(uri);

                    WbrowseTxt.Text = string.Empty;
                    ListLinks_Lst.Items.Clear();

                    int x;
                    x = URLbrowse_Cbx.FindStringExact(URLbrowse_Cbx.Text);
                    if (x == -1)
                        URLbrowse_Cbx.Items.Add(URLbrowse_Cbx.Text);

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
            ListLinks_Lst.Items.Add(value);
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
            if (URLbrowse_Cbx.Text != string.Empty)
            {
                string textData = URLbrowse_Cbx.Text;
                Clipboard.SetData(DataFormats.Text, textData);
                Beep(1500, 400);
            }
        }

        void SavePageTxt_Btn_Click(object sender, EventArgs e)
        {
            if (WbrowseTxt.Text != string.Empty)
            {
                string dirselect = selectdir.Dirselect();

                string namef = UriTxt;
                namef = Regex.Replace(namef, @"[^a-zA-Z0-9]", "_").Replace("https", string.Empty);

                string C = namef;
                string D = string.Empty;

                if (C.Length > 50)
                    D += C.Substring(0, 50);
                else
                    D = C;

                CreateNameAleat();

                try
                {
                    if (dirselect != string.Empty)
                    {
                        using (StreamWriter fc = File.AppendText(dirselect + @"\" + Una + "_" + D + ".txt"))
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
    }
}
