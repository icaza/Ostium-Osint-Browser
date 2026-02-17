using Icaza;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Ostium
{
    public partial class HtmlText_Frm : Form
    {
        #region Var_
        readonly IcazaClass senderror = new IcazaClass();
        //private readonly IcazaClass selectdir = new IcazaClass();
        readonly string AppStart = Application.StartupPath + @"\";

        static readonly HttpClient client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
            MaxConnectionsPerServer = 10
        })
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        readonly List<string> lstUrlDfltCnf = new List<string>();
        readonly ConcurrentDictionary<string, bool> urlHistory = new ConcurrentDictionary<string, bool>();
        CancellationTokenSource cancellationTokenSource;

        string currentUrl = string.Empty;
        int wordCount = 0;
        int linkCount = 0;
        long pageSize = 0;

        static readonly ObjectPool<StringBuilder> stringBuilderPool = new ObjectPool<StringBuilder>(
            () => new StringBuilder(4096),
            sb => sb.Clear()
        );

        static readonly Regex linkPattern = new Regex(
            @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        static readonly Regex cleanupPattern = new Regex(
            @"(&.+?;)|\n\r|\s{2,}",
            RegexOptions.Compiled
        );

        static readonly Regex emailPattern = new Regex(
            @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b",
            RegexOptions.Compiled
        );

        const string ASCII_SEPARATOR = "╔═══════════════════════════════════════════════════════════════════════════════╗";
        const string ASCII_BOTTOM = "╚═══════════════════════════════════════════════════════════════════════════════╝";
        const string ASCII_IMAGE = @"
   ╔═══════════════════════╗
   ║   [📷 IMAGE]          ║
   ║                       ║
   ║   ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓     ║
   ║   ▓░░░░░░░░░░░░░▓     ║
   ║   ▓░░░░░░░░░░░░░▓     ║
   ║   ▓░░░░░░░░░░░░░▓     ║
   ║   ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓     ║
   ╚═══════════════════════╝";

        const string ASCII_VIDEO = @"
   ╔═══════════════════════╗
   ║   [▶️ VIDEO]           ║
   ║   ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓     ║
   ║   ▓      ▶       ▓    ║
   ║   ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓     ║
   ╚═══════════════════════╝";

        const string ASCII_LINK = "🔗";
        #endregion

        #region Constructor & Initialization
        public HtmlText_Frm()
        {
            InitializeComponent();
            InitializeEvents();
            ConfigureHttpClient();
        }

        void InitializeEvents()
        {
            WbrowseTxt.LinkClicked += RTFLink_Clicked;
            URLbrowse_Cbx.KeyPress += URLbrowseCbx_Keypress;
            FormClosing += HtmlText_Frm_FormClosing;

            WbrowseTxt.TextChanged += WbrowseTxt_TextChanged;
        }

        void ConfigureHttpClient()
        {
            if (!client.DefaultRequestHeaders.Contains("Accept-Language"))
            {
                client.DefaultRequestHeaders.Add("Accept-Language", "fr-FR,fr;q=0.9,en-US;q=0.8,en;q=0.7");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("DNT", "1");
            }
        }

        void HtmlText_Frm_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
            DisplayWelcomeMessage();
        }

        void LoadConfiguration()
        {
            try
            {
                string configPath = Path.Combine(AppStart, "url_dflt_cnf.ost");
                if (File.Exists(configPath))
                {
                    lstUrlDfltCnf.Clear();
                    lstUrlDfltCnf.AddRange(File.ReadAllLines(configPath));
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! LoadConfiguration: ", ex.ToString(), "HtmlText_Frm", AppStart);
            }
        }

        void DisplayWelcomeMessage()
        {
            var sb = stringBuilderPool.Get();
            try
            {
                sb.AppendLine(ASCII_SEPARATOR);
                sb.AppendLine("║                         HTML TEXT BROWSER - OSTIUM                            ║");
                sb.AppendLine("║                                                                               ║");
                sb.AppendLine("║  ┌─────────────────────────────────────────────────────────────────┐          ║");
                sb.AppendLine("║  │  Optimized text browser - Ultra-light and fast                  │          ║");
                sb.AppendLine("║  │                                                                 │          ║");
                sb.AppendLine("║  │  Features:                                                      │          ║");
                sb.AppendLine("║  │  • Intelligent text extraction                                  │          ║");
                sb.AppendLine("║  │  • Automatic detection of images, videos and links              │          ║");
                sb.AppendLine("║  │  • Real-time statistics                                         │          ║");
                sb.AppendLine("║  │  • Search for content (Ctrl+F)                                  │          ║");
                sb.AppendLine("║  │  • Export in multiple formats                                   │          ║");
                sb.AppendLine("║  │  • Optimized reading mode                                       │          ║");
                sb.AppendLine("║  └─────────────────────────────────────────────────────────────────┘          ║");
                sb.AppendLine(ASCII_BOTTOM);
                sb.AppendLine();
                sb.AppendLine("Enter a URL and click 'Go' to begin...");

                WbrowseTxt.Text = sb.ToString();
            }
            finally
            {
                stringBuilderPool.Return(sb);
            }
        }
        #endregion

        #region URL Navigation
        void Go_Btn_Click(object sender, EventArgs e)
        {
            StartOpenWebPageTxt();
        }

        void URLbrowseCbx_Keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                StartOpenWebPageTxt();
            }
        }

        void StartOpenWebPageTxt()
        {
            try
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();

                if (string.IsNullOrWhiteSpace(URLbrowse_Cbx.Text))
                {
                    MessageBox.Show("Please enter a URL!", "URL required",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var inputUrl = URLbrowse_Cbx.Text.Trim();
                Uri uri = ParseUrl(inputUrl);

                if (uri == null)
                {
                    MessageBox.Show("Invalid URL!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (uri.Scheme == "file")
                {
                    MessageBox.Show("Local files are not supported!",
                        "WEB only", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                URLbrowse_Cbx.Text = uri.ToString();
                currentUrl = uri.ToString();

                if (urlHistory.TryAdd(currentUrl, true))
                {
                    if (!URLbrowse_Cbx.Items.Contains(currentUrl))
                    {
                        URLbrowse_Cbx.Items.Add(currentUrl);
                    }
                }

                WbrowseTxt.Clear();
                ListLinks_Lst.Items.Clear();

                ShowLoadingIndicator();

                _ = OpenWebPageTxtAsync(cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! StartOpenWebPageTxt: ", ex.ToString(), "HtmlText_Frm", AppStart);
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        Uri ParseUrl(string inputUrl)
        {
            try
            {
                if (Uri.IsWellFormedUriString(inputUrl, UriKind.Absolute))
                {
                    return new Uri(inputUrl);
                }
                else if (!inputUrl.Contains(" ") && inputUrl.Contains("."))
                {
                    return new Uri("https://" + inputUrl);
                }
                else
                {
                    string searchEngine = GetDefaultSearchEngine();
                    return new Uri(searchEngine + Uri.EscapeDataString(inputUrl));
                }
            }
            catch
            {
                return null;
            }
        }

        string GetDefaultSearchEngine()
        {
            try
            {
                if (!string.IsNullOrEmpty(Class_Var.URL_DEFAUT_WSEARCH))
                    return Class_Var.URL_DEFAUT_WSEARCH;

                if (lstUrlDfltCnf.Count > 3)
                {
                    Class_Var.URL_DEFAUT_WSEARCH = lstUrlDfltCnf[3];
                    return Class_Var.URL_DEFAUT_WSEARCH;
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! GetDefaultSearchEngine: ", ex.ToString(), "HtmlText_Frm", AppStart);
            }

            return "https://www.google.com/search?client=firefox-b-d&q=";
        }

        void ShowLoadingIndicator()
        {
            var sb = stringBuilderPool.Get();
            try
            {
                sb.AppendLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
                sb.AppendLine("║                                                                               ║");
                sb.AppendLine("║                          ⏳ LOADING...                                        ║");
                sb.AppendLine("║                                                                               ║");
                sb.AppendLine("║                             ▓▓▓▓▓▓▓▓▓▓▓▓▓                                     ║");
                sb.AppendLine("║                                                                               ║");
                sb.AppendLine($"║  URL: {currentUrl,-70}║");
                sb.AppendLine("║                                                                               ║");
                sb.AppendLine("╚═══════════════════════════════════════════════════════════════════════════════╝");

                WbrowseTxt.Text = sb.ToString();
            }
            finally
            {
                stringBuilderPool.Return(sb);
            }
        }
        #endregion

        #region Web Page Processing
        async Task OpenWebPageTxtAsync(CancellationToken token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(currentUrl))
                {
                    senderror.ErrorLog("Error! OpenWebPageTxt: ", "URL is empty", "HtmlText_Frm", AppStart);
                    return;
                }

                string userAgent = string.IsNullOrWhiteSpace(UserAgent_Txt.Text)
                    ? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/144.0.0.0 Safari/537.36"
                    : UserAgent_Txt.Text;

                client.DefaultRequestHeaders.Remove("User-Agent");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);

                var startTime = DateTime.Now;
                HttpResponseMessage response = await client.GetAsync(currentUrl, token);

                if (!response.IsSuccessStatusCode)
                {
                    ShowErrorPage($"HTTP Error {(int)response.StatusCode}: {response.ReasonPhrase}");
                    return;
                }

                byte[] contentBytes = await response.Content.ReadAsByteArrayAsync();
                pageSize = contentBytes.Length;
                string pageContents = Encoding.UTF8.GetString(contentBytes);
                var loadTime = (DateTime.Now - startTime).TotalSeconds;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageContents);

                var extractedContent = await Task.Run(() => ExtractContent(doc, pageContents), token);

                if (!token.IsCancellationRequested)
                {
                    Invoke(new Action(() =>
                    {
                        DisplayExtractedContent(extractedContent, loadTime);
                    }));
                }
            }
            catch (OperationCanceledException)
            {
                // Loading cancelled
            }
            catch (HttpRequestException ex)
            {
                ShowErrorPage($"Connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! OpenWebPageTxt: ", ex.ToString(), "HtmlText_Frm", AppStart);
                ShowErrorPage($"Error: {ex.Message}");
            }
        }

        ExtractedContent ExtractContent(HtmlDocument doc, string rawHtml)
        {
            var content = new ExtractedContent();
            var sb = stringBuilderPool.Get();
            var links = new HashSet<string>();
            var emails = new HashSet<string>();

            try
            {
                // Page title
                var titleNode = doc.DocumentNode.SelectSingleNode("//title");
                content.Title = titleNode?.InnerText.Trim() ?? "Untitled";

                // Meta description
                var metaDesc = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");
                content.Description = metaDesc?.GetAttributeValue("content", "")?.Trim() ?? "";

                // Extract the text while preserving the structure
                ExtractTextWithStructure(doc.DocumentNode, sb, 0);

                // Detect images
                var imgNodes = doc.DocumentNode.SelectNodes("//img");
                if (imgNodes != null)
                {
                    content.ImageCount = imgNodes.Count;
                    foreach (var img in imgNodes.Take(100)) // Limit for performance
                    {
                        string src = img.GetAttributeValue("src", "");
                        string alt = img.GetAttributeValue("alt", "No description");
                        if (!string.IsNullOrWhiteSpace(src))
                        {
                            content.Images.Add(new ImageInfo { Src = src, Alt = alt });
                        }
                    }
                }

                // Detect videos
                var videoNodes = doc.DocumentNode.SelectNodes("//video | //iframe[contains(@src, 'youtube') or contains(@src, 'vimeo')]");
                if (videoNodes != null)
                {
                    content.VideoCount = videoNodes.Count;
                }

                // Extract the links
                var linkMatches = linkPattern.Matches(rawHtml);
                foreach (Match match in linkMatches)
                {
                    links.Add(match.Value);
                }
                content.Links = links.ToList();

                // Extract the emails
                var emailMatches = emailPattern.Matches(sb.ToString());
                foreach (Match match in emailMatches)
                {
                    emails.Add(match.Value);
                }
                content.Emails = emails.ToList();

                // Clean up the text
                string cleanText = cleanupPattern.Replace(sb.ToString(), " ");
                content.Text = cleanText.Trim();
                content.WordCount = CountWords(content.Text);

                return content;
            }
            finally
            {
                stringBuilderPool.Return(sb);
            }
        }

        void ExtractTextWithStructure(HtmlAgilityPack.HtmlNode node, StringBuilder sb, int depth)
        {
            if (node == null) return;

            // Ignore scripts and styles
            if (node.Name == "script" || node.Name == "style" || node.Name == "noscript")
                return;

            if (node.Name.StartsWith("h") && node.Name.Length == 2 && char.IsDigit(node.Name[1]))
            {
                sb.AppendLine();
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
                sb.Append("  ");
            }
            else if (node.Name == "p" || node.Name == "div" || node.Name == "section" || node.Name == "article")
            {
                sb.AppendLine();
            }
            else if (node.Name == "br")
            {
                sb.AppendLine();
                return;
            }
            else if (node.Name == "li")
            {
                sb.Append("  • ");
            }

            // Node text
            if (node.NodeType == HtmlAgilityPack.HtmlNodeType.Text)
            {
                string text = node.InnerText.Trim();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    sb.Append(text);
                    sb.Append(" ");
                }
            }

            if (node.HasChildNodes)
            {
                foreach (var child in node.ChildNodes)
                {
                    ExtractTextWithStructure(child, sb, depth + 1);
                }
            }

            if (node.Name == "p" || node.Name == "div" || node.Name.StartsWith("h"))
            {
                sb.AppendLine();
            }
        }

        void DisplayExtractedContent(ExtractedContent content, double loadTime)
        {
            var sb = stringBuilderPool.Get();
            try
            {
                sb.AppendLine(ASCII_SEPARATOR);
                sb.AppendLine($"║  📄 TITLE: {TruncateText(content.Title, 67),-67}║");
                sb.AppendLine($"║  🌐 URL: {TruncateText(currentUrl, 69),-69}║");
                sb.AppendLine("╠═══════════════════════════════════════════════════════════════════════════════╣");
                sb.AppendLine($"║  📊 STATISTICS:                                                              ║");
                sb.AppendLine($"║     • Words: {content.WordCount,-10} • Links: {content.Links.Count,-10} • Time: {loadTime:F2}s{new string(' ', 20)}║");
                sb.AppendLine($"║     • Images: {content.ImageCount,-7} • Videos: {content.VideoCount,-8} • Size: {FormatBytes(pageSize),-10}{new string(' ', 17)}║");

                if (!string.IsNullOrWhiteSpace(content.Description))
                {
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════════════════════╣");
                    sb.AppendLine($"║  📝 DESCRIPTION: {TruncateText(content.Description, 61).PadRight(61)}║");
                }

                sb.AppendLine(ASCII_BOTTOM);
                sb.AppendLine();
                sb.AppendLine();

                sb.AppendLine(content.Text);
                sb.AppendLine();
                sb.AppendLine();

                if (content.Images.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
                    sb.AppendLine($"║  🖼️  IMAGES DETECTED ({content.ImageCount})                                  ║");
                    sb.AppendLine("╚═══════════════════════════════════════════════════════════════════════════════╝");

                    foreach (var img in content.Images.Take(10))
                    {
                        sb.AppendLine(ASCII_IMAGE);
                        sb.AppendLine($"   Source: {TruncateText(img.Src, 70)}");
                        sb.AppendLine($"   Alt: {TruncateText(img.Alt, 70)}");
                        sb.AppendLine();
                    }

                    if (content.ImageCount > 10)
                    {
                        sb.AppendLine($"   ... and {content.ImageCount - 10} other images");
                        sb.AppendLine();
                    }
                }

                if (content.VideoCount > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
                    sb.AppendLine($"║  🎬 VIDEOS DETECTED ({content.VideoCount})                                   ║");
                    sb.AppendLine("╚═══════════════════════════════════════════════════════════════════════════════╝");
                    sb.AppendLine(ASCII_VIDEO);
                    sb.AppendLine();
                }

                if (content.Emails.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
                    sb.AppendLine($"║  📧 EMAILS DETECTED ({content.Emails.Count})                                 ║");
                    sb.AppendLine("╚═══════════════════════════════════════════════════════════════════════════════╝");
                    foreach (var email in content.Emails.Take(20))
                    {
                        sb.AppendLine($"   {ASCII_LINK} {email}");
                    }
                    sb.AppendLine();
                }

                WbrowseTxt.Text = sb.ToString();

                foreach (string link in content.Links.Take(1000))
                {
                    AddLinkList(link);
                }

                wordCount = content.WordCount;
                linkCount = content.Links.Count;
            }
            finally
            {
                stringBuilderPool.Return(sb);
            }
        }

        void ShowErrorPage(string errorMessage)
        {
            Invoke(new Action(() =>
            {
                var sb = stringBuilderPool.Get();
                try
                {
                    sb.AppendLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
                    sb.AppendLine("║                                                                               ║");
                    sb.AppendLine("║                              ❌ ERROR                                         ║");
                    sb.AppendLine("║                                                                               ║");
                    sb.AppendLine("╠═══════════════════════════════════════════════════════════════════════════════╣");
                    sb.AppendLine($"║  {errorMessage,-77}║");
                    sb.AppendLine("║                                                                               ║");
                    sb.AppendLine("╚═══════════════════════════════════════════════════════════════════════════════╝");

                    WbrowseTxt.Text = sb.ToString();
                }
                finally
                {
                    stringBuilderPool.Return(sb);
                }
            }));
        }
        #endregion

        #region Link Management
        void AddLinkList(string value)
        {
            try
            {
                if (ListLinks_Lst.FindStringExact(value) == -1)
                {
                    ListLinks_Lst.Items.Add(value);
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! AddLinkList: ", ex.ToString(), "HtmlText_Frm", AppStart);
            }
        }

        void RTFLink_Clicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                URLbrowse_Cbx.Text = e.LinkText;

                if (URLbrowse_Cbx.FindStringExact(e.LinkText) == -1)
                {
                    URLbrowse_Cbx.Items.Add(e.LinkText);
                }

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

                if (URLbrowse_Cbx.FindStringExact(URLbrowse_Cbx.Text) == -1)
                {
                    URLbrowse_Cbx.Items.Add(ListLinks_Lst.SelectedItem.ToString());
                }

                StartOpenWebPageTxt();
            }
        }
        #endregion

        #region File Operations
        void SavePageTxt_Btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(WbrowseTxt.Text))
            {
                MessageBox.Show("No content to save!", "Attention",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Text file (*.txt)|*.txt|Markdown (*.md)|*.md|HTML (*.html)|*.html|All files (*.*)|*.*";
                    sfd.FilterIndex = 1;
                    sfd.FileName = GenerateSafeFileName(currentUrl);

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        string content = WbrowseTxt.Text;

                        var metadata = new StringBuilder();
                        metadata.AppendLine("════════════════════════════════════════════════════════════════");
                        metadata.AppendLine($"URL: {currentUrl}");
                        metadata.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        metadata.AppendLine($"Words: {wordCount} | Links: {linkCount} | Size: {FormatBytes(pageSize)}");
                        metadata.AppendLine("════════════════════════════════════════════════════════════════");
                        metadata.AppendLine();
                        metadata.AppendLine(content);

                        File.WriteAllText(sfd.FileName, metadata.ToString(), Encoding.UTF8);

                        Console.Beep(800, 200);
                        MessageBox.Show($"File save: {sfd.FileName}", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! SavePageTxt_Btn_Click: ", ex.ToString(), "HtmlText_Frm", AppStart);
                MessageBox.Show($"Error during backup: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void CopyUrl_Btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(URLbrowse_Cbx.Text))
            {
                Clipboard.SetText(URLbrowse_Cbx.Text);
                Console.Beep(1500, 400);
            }
        }

        void EmptyCbx_Btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear URL history?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                URLbrowse_Cbx.Items.Clear();
                urlHistory.Clear();
            }
        }
        #endregion

        #region Utility Methods
        int CountWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text ?? "";

            return text.Substring(0, maxLength - 3) + "...";
        }

        string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return $"{len:F2} {sizes[order]}";
        }

        string GenerateSafeFileName(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string fileName = uri.Host.Replace("www.", "");

                if (!string.IsNullOrWhiteSpace(uri.AbsolutePath) && uri.AbsolutePath != "/")
                {
                    fileName += uri.AbsolutePath.Replace("/", "_");
                }

                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    fileName = fileName.Replace(c, '_');
                }

                fileName += $"_{DateTime.Now:yyyyMMdd_HHmmss}";

                return fileName.Length > 100 ? fileName.Substring(0, 100) : fileName;
            }
            catch
            {
                return $"page_{DateTime.Now:yyyyMMdd_HHmmss}";
            }
        }

        void WbrowseTxt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Text = $"HTML Text - {wordCount} mots, {linkCount} liens";
            }
            catch { }
        }

        void HtmlText_Frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }
        #endregion

        #region Helper Classes
        class ExtractedContent
        {
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";
            public string Text { get; set; } = "";
            public List<string> Links { get; set; } = new List<string>();
            public List<string> Emails { get; set; } = new List<string>();
            public List<ImageInfo> Images { get; set; } = new List<ImageInfo>();
            public int ImageCount { get; set; }
            public int VideoCount { get; set; }
            public int WordCount { get; set; }
        }

        class ImageInfo
        {
            public string Src { get; set; }
            public string Alt { get; set; }
        }
        #endregion
    }

    #region Object Pool for Performance
    /// <summary>
    /// Object pool to reduce allocations and improve performance
    /// </summary>
    internal class ObjectPool<T> where T : class
    {
        readonly ConcurrentBag<T> objects = new ConcurrentBag<T>();
        readonly Func<T> objectGenerator;
        readonly Action<T> objectResetter;

        public ObjectPool(Func<T> objectGenerator, Action<T> objectResetter = null)
        {
            this.objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            this.objectResetter = objectResetter;
        }

        public T Get()
        {
            return objects.TryTake(out T item) ? item : objectGenerator();
        }

        public void Return(T item)
        {
            objectResetter?.Invoke(item);
            objects.Add(item);
        }
    }
    #endregion
}
