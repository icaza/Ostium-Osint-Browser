using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Ostium
{
    public partial class DiscoverRSS : Form
    {
        static readonly HttpClient _httpClient = CreateHttpClient();
        CancellationTokenSource _cts;

        static readonly string[] FeedPaths = new[]
        {
            "/feed", "/feed/", "/rss", "/rss/", "/rss.xml", "/feed.xml",
            "/atom.xml", "/feed/atom", "/feeds/posts/default", "/index.xml",
            "/?feed=rss2", "/?feed=atom", "/blog/feed", "/news/feed"
        };

        public DiscoverRSS()
        {
            InitializeComponent();

            urlTextBox.Enter += new EventHandler(UrlTextBox_Enter);
            urlTextBox.Leave += new EventHandler(UrlTextBox_Leave);
            titleTextBox.ReadOnly = true;
        }

        void UrlTextBox_Enter(object sender, EventArgs e)
        {
            if (urlTextBox.Text == "Enter website URL (e.g. https://example.com)")
            {
                urlTextBox.Text = string.Empty;
            }
        }

        void UrlTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(urlTextBox.Text))
            {
                urlTextBox.Text = "Enter website URL (e.g. https://example.com)";
            }
        }

        async void DiscoverRSS_Btn_Click(object sender, EventArgs e)
        {
            CopyRSSdiscover_Btn.Enabled = false;

            string input = urlTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input) || input == "Enter website URL (e.g. https://example.com)")
            {
                ShowError("Please enter a URL.");
                return;
            }

            if (!TryNormalizeUrl(input, out Uri baseUri))
            {
                ShowError("Invalid URL. Make sure it starts with http:// or https://");
                return;
            }

            SetBusy(true);
            titleTextBox.Clear();

            try
            {
                _cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                FeedResult result = await DiscoverFeedAsync(baseUri, _cts.Token);

                if (result != null)
                {
                    urlTextBox.Text = result.FeedUrl;
                    titleTextBox.Text = result.Title;
                }
                else
                {
                    titleTextBox.Text = "No RSS/Atom feed found for this URL.";
                }
            }
            catch (OperationCanceledException)
            {
                titleTextBox.Text = "Search timed out. Try a different URL.";
            }
            catch (Exception ex)
            {
                titleTextBox.Text = $"Error: {ex.Message}";
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
                SetBusy(false);
                CopyRSSdiscover_Btn.Enabled = true;
            }
        }

        void CopyRSSdiscover_Btn_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, urlTextBox.Text);
            Console.Beep(800, 200);
        }

        async void BatchDiscover_Btn_Click(object sender, EventArgs e)
        {
            string inputPath = PromptOpenFile();
            if (inputPath == null) return;

            string[] rawLines;
            try
            {
                rawLines = File.ReadAllLines(inputPath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                ShowError($"Could not read file:\n{ex.Message}");
                return;
            }

            List<string> urls = rawLines
                .Select(l => l.Trim())
                .Where(l => l.Length > 0 && !l.StartsWith("#"))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (urls.Count == 0)
            {
                ShowError("The file contains no valid URLs.");
                return;
            }

            SetBusy(true);
            titleTextBox.Text = $"Processing {urls.Count} URL(s)...";

            var results = new List<BatchEntry>(urls.Count);
            _cts = new CancellationTokenSource();

            try
            {
                const int maxConcurrency = 4;
                using (var semaphore = new SemaphoreSlim(maxConcurrency))
                {
                    var tasks = urls.Select(async raw =>
                    {
                        await semaphore.WaitAsync(_cts.Token);
                        try { return await ProcessSingleBatchEntry(raw, _cts.Token); }
                        finally { semaphore.Release(); }
                    }).ToList();

                    BatchEntry[] completed = await Task.WhenAll(tasks);
                    results.AddRange(completed);
                }
            }
            catch (OperationCanceledException)
            {
                titleTextBox.Text = "Batch search cancelled.";
                SetBusy(false);
                return;
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }

            string outputPath = BuildOutputPath(inputPath);
            try
            {
                SaveBatchReport(outputPath, results);
            }
            catch (Exception ex)
            {
                ShowError($"Could not save report:\n{ex.Message}");
                SetBusy(false);
                return;
            }

            int successCount = results.Count(r => r.Success);
            int failCount = results.Count - successCount;

            titleTextBox.Text =
                $"Batch done — {successCount} found, {failCount} not found. " +
                $"Report: {outputPath}";

            SetBusy(false);

            if (MessageBox.Show(
                    $"Batch complete.\n\nFound     : {successCount}\nNot found : {failCount}\n\nOpen the report?",
                    "Batch RSS Discovery",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information) == DialogResult.Yes)
            {
                try { Process.Start("notepad.exe", outputPath); }
                catch {}
            }
        }

        async Task<BatchEntry> ProcessSingleBatchEntry(string raw, CancellationToken ct)
        {
            var entry = new BatchEntry { InputUrl = raw };

            if (!TryNormalizeUrl(raw, out Uri baseUri))
            {
                entry.Error = "Invalid URL";
                return entry;
            }

            entry.NormalizedUrl = baseUri.AbsoluteUri;

            try
            {
                using (var localCts = CancellationTokenSource.CreateLinkedTokenSource(ct))
                {
                    localCts.CancelAfter(TimeSpan.FromSeconds(20));
                    FeedResult result = await DiscoverFeedAsync(baseUri, localCts.Token);

                    if (result != null)
                    {
                        entry.FeedUrl = result.FeedUrl;
                        entry.Title = result.Title;
                        entry.Success = true;
                    }
                    else
                    {
                        entry.Error = "No feed found";
                    }
                }
            }
            catch (OperationCanceledException) { entry.Error = "Timed out"; }
            catch (Exception ex) { entry.Error = SanitizeErrorMessage(ex.Message); }

            return entry;
        }

        static void SaveBatchReport(string path, IReadOnlyList<BatchEntry> results)
        {
            var sb = new StringBuilder();
            sb.AppendLine("RSS Batch Discovery Report");
            sb.AppendLine($"Generated : {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine($"Total     : {results.Count}");
            sb.AppendLine($"Found     : {results.Count(r => r.Success)}");
            sb.AppendLine($"Not found : {results.Count(r => !r.Success)}");
            sb.AppendLine(new string('-', 72));
            sb.AppendLine();

            sb.AppendLine("[ FOUND ]");
            sb.AppendLine();
            foreach (BatchEntry e in results.Where(r => r.Success))
            {
                sb.AppendLine($"  Input : {e.InputUrl}");
                sb.AppendLine($"  Feed  : {e.FeedUrl}");
                sb.AppendLine($"  Title : {e.Title}");
                sb.AppendLine();
            }

            sb.AppendLine("[ NOT FOUND / ERRORS ]");
            sb.AppendLine();
            foreach (BatchEntry e in results.Where(r => !r.Success))
            {
                sb.AppendLine($"  Input  : {e.InputUrl}");
                sb.AppendLine($"  Reason : {e.Error}");
                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        async Task<FeedResult> DiscoverFeedAsync(Uri baseUri, CancellationToken ct)
        {
            FeedResult direct = await TryParseFeedAsync(baseUri.AbsoluteUri, ct);
            if (direct != null) return direct;

            foreach (string feedUrl in await DiscoverFromHtmlAsync(baseUri, ct))
            {
                FeedResult r = await TryParseFeedAsync(feedUrl, ct);
                if (r != null) return r;
            }

            foreach (string path in FeedPaths)
            {
                if (ct.IsCancellationRequested) break;
                FeedResult r = await TryParseFeedAsync(new Uri(baseUri, path).AbsoluteUri, ct);
                if (r != null) return r;
            }

            return null;
        }

        async Task<IEnumerable<string>> DiscoverFromHtmlAsync(Uri baseUri, CancellationToken ct)
        {
            var feeds = new List<string>();
            try
            {
                string html = await _httpClient.GetStringAsync(baseUri).WithCancellation(ct);

                const string pattern =
                    @"<link[^>]+(?:type=""application/(?:rss|atom)\+xml"")[^>]*href=""([^""]+)""" +
                    @"|<link[^>]+href=""([^""]+)""[^>]*(?:type=""application/(?:rss|atom)\+xml"")";

                foreach (Match m in Regex.Matches(html, pattern, RegexOptions.IgnoreCase))
                {
                    string href = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
                    if (!string.IsNullOrWhiteSpace(href) &&
                        Uri.TryCreate(baseUri, href, out Uri absolute))
                        feeds.Add(absolute.AbsoluteUri);
                }
            }
            catch { }
            return feeds;
        }

        async Task<FeedResult> TryParseFeedAsync(string url, CancellationToken ct)
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync(
                           url, HttpCompletionOption.ResponseHeadersRead, ct))
                {
                    if (!response.IsSuccessStatusCode) return null;

                    string contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
                    bool looksLikeFeed =
                        contentType.Contains("rss") ||
                        contentType.Contains("atom") ||
                        contentType.Contains("xml");

                    string body = await response.Content.ReadAsStringAsync().WithCancellation(ct);

                    if (!looksLikeFeed && !body.TrimStart().StartsWith("<")) return null;

                    string title = ExtractFeedTitle(body);
                    if (title == null) return null;

                    return new FeedResult { FeedUrl = url, Title = title };
                }
            }
            catch { return null; }
        }

        static string ExtractFeedTitle(string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNode rssTitle = doc.SelectSingleNode("//channel/title")
                                ?? doc.SelectSingleNode(
                                       "//*[local-name()='channel']/*[local-name()='title']");
                if (rssTitle != null) return rssTitle.InnerText.Trim();

                var ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                XmlNode atomTitle = doc.SelectSingleNode("/atom:feed/atom:title", ns);
                if (atomTitle != null) return atomTitle.InnerText.Trim();

                return null;
            }
            catch { return null; }
        }

        static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 5,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler, disposeHandler: false)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                "User-Agent", "RssDiscoveryTool/1.0 (compatible; +https://github.com)");
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                "Accept", "application/rss+xml, application/atom+xml, text/html;q=0.9, */*;q=0.8");
            return client;
        }

        static bool TryNormalizeUrl(string input, out Uri uri)
        {
            if (!input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !input.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                input = "https://" + input;

            return Uri.TryCreate(input, UriKind.Absolute, out uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        static string SanitizeErrorMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return "Unknown error";
            message = message.Replace('\n', ' ').Replace('\r', ' ');
            return message.Length > 120 ? message.Substring(0, 120) + "..." : message;
        }

        static string PromptOpenFile()
        {
            using (var ofd = new OpenFileDialog
            {
                Title = "Select URL list (TXT)",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            })
            {
                return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : null;
            }
        }

        static string BuildOutputPath(string inputPath)
        {
            string dir = Path.GetDirectoryName(inputPath) ?? Directory.GetCurrentDirectory();
            string baseName = Path.GetFileNameWithoutExtension(inputPath);
            string stamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            return Path.Combine(dir, $"{baseName}_rss_report_{stamp}.txt");
        }

        void SetBusy(bool busy)
        {
            DiscoverRSS_Btn.Enabled = !busy;
            DiscoverRSS_Btn.Text = busy ? "Searching..." : "Discover RSS";
            BatchDiscover_Btn.Enabled = !busy;
            BatchDiscover_Btn.Text = busy ? "Processing..." : "Batch Discover";
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        void ShowError(string msg) =>
            MessageBox.Show(msg, "RSS Discovery", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        sealed class FeedResult
        {
            public string FeedUrl { get; set; }
            public string Title { get; set; }
        }

        sealed class BatchEntry
        {
            public string InputUrl { get; set; }
            public string NormalizedUrl { get; set; }
            public bool Success { get; set; }
            public string FeedUrl { get; set; }
            public string Title { get; set; }
            public string Error { get; set; }
        }
    }

    internal static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (ct.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(ct);
            }
            return await task;
        }
    }
}