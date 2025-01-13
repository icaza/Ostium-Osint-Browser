using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Icaza;
using System.IO;

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
        string UriTxt = "";
        string Una = "";

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
                if (URLbrowse_Cbx.Text != "")
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

                    WbrowseTxt.Text = "";
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
                senderror.ErrorLog("Error! StartOpenWebPageTxt: ", ex.Message, "HtmlText_Frm", AppStart);
            }
        }

        async void OpenWebPageTxt()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent_Txt.Text);
                var response = await client.GetAsync(UriTxt);
                string pageContents = await response.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageContents);

                var root = doc.DocumentNode;
                var sb = new StringBuilder();

                foreach (var node in root.DescendantsAndSelf())
                {
                    if (!node.HasChildNodes && node.ParentNode.Name != "script" && node.ParentNode.Name != "style")
                    {
                        string text = node.InnerText;
                        if (!string.IsNullOrEmpty(text))
                            sb.AppendLine(text.Trim());
                    }
                }

                foreach (Match match in Regex.Matches(pageContents, @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*"))
                {
                    Invoke(new Action<string>(AddLinkList), match.Value);
                }

                string Txts = Regex.Replace(Convert.ToString(sb), @"(&.+?;)|\n\r", string.Empty);
                Invoke(new Action<string>(RegReplaceTxt), Txts);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenWebPageTxt: ", ex.Message, "HtmlText_Frm", AppStart);
            }
        }

        void CreateNameAleat()
        {            
            Una = DateTime.Now.ToString("d").Replace("/", "_") + "_" + DateTime.Now.ToString("HH:mm:ss").Replace(":", "_");
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
                senderror.ErrorLog("Error! RTFLink_Clicked: ", ex.Message, "HtmlText_Frm", AppStart);
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
            if (URLbrowse_Cbx.Text != "")
            {
                string textData = URLbrowse_Cbx.Text;
                Clipboard.SetData(DataFormats.Text, textData);
                Beep(1500, 400);
            }
        }

        private void SavePageTxt_Btn_Click(object sender, EventArgs e)
        {
            if (WbrowseTxt.Text != "")
            {
                string dirselect = selectdir.Dirselect();

                string namef = UriTxt;
                namef = Regex.Replace(namef, @"[^a-zA-Z0-9]", "_").Replace("https", "");

                string C = namef;
                string D = "";

                if (C.Length > 50)
                    D += C.Substring(0, 50);
                else
                    D = C;

                CreateNameAleat();

                try
                {
                    if (dirselect != "")
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
                    senderror.ErrorLog("Error! SavePageTxt_Btn_Click: ", ex.Message, "HtmlText_Frm", AppStart);
                }
            }
        }

        void EmptyCbx_Btn_Click(object sender, EventArgs e)
        {
            URLbrowse_Cbx.Items.Clear();
        }
    }
}
