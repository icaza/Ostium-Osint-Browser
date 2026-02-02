using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UrlUnshortenWorker
{
    class Program
    {
        [STAThread]
        static async Task<int> Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("ERROR:No URL provided");
                return 1;
            }

            string shortUrl = args[0];
            int timeout = args.Length > 1 && int.TryParse(args[1], out int t) ? t : 15000;

            try
            {
                string result = await UnshortenUrl(shortUrl, timeout);

                if (!string.IsNullOrEmpty(result))
                {
                    Console.WriteLine($"SUCCESS:{result}");
                    return 0;
                }
                else
                {
                    Console.WriteLine($"ERROR:Failed to unshorten");
                    return 2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR:{ex.Message}");
                return 3;
            }
        }

        static async Task<string> UnshortenUrl(string shortUrl, int timeoutMs)
        {
            var tcs = new TaskCompletionSource<string>();
            Form hiddenForm = null;
            WebView2 webView = null;
            int navigationCount = 0;
            bool completed = false;

            var uiThread = new Thread(() =>
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    hiddenForm = new Form
                    {
                        Width = 1,
                        Height = 1,
                        FormBorderStyle = FormBorderStyle.None,
                        ShowInTaskbar = false,
                        Opacity = 0,
                        StartPosition = FormStartPosition.Manual,
                        Location = new Point(-10000, -10000)
                    };

                    webView = new WebView2
                    {
                        Dock = DockStyle.Fill
                    };

                    hiddenForm.Controls.Add(webView);

                    async void OnFormLoad(object sender, EventArgs e)
                    {
                        try
                        {
                            await webView.EnsureCoreWebView2Async(null);

                            webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                            webView.CoreWebView2.WebResourceRequested += (navSender, navArgs) =>
                            {
                                navArgs.Request.Headers.SetHeader("Upgrade-Insecure-Requests", "1");
                                
                                if (navArgs.Request.Uri.Contains("tracker") || navArgs.Request.Uri.Contains("analytics"))
                                {
                                    navArgs.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(
                                        null, 204, "Blocked", "Content-Type: text/plain");
                                }
                            };

                            // Secure configuration
                            webView.CoreWebView2.Settings.IsScriptEnabled = true;
                            webView.CoreWebView2.Settings.IsWebMessageEnabled = false;
                            webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
                            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                            webView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
                            webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;

                            // Block dangerous protocols
                            webView.CoreWebView2.NavigationStarting += (navSender, navArgs) =>
                            {
                                try
                                {
                                    Uri uri = new Uri(navArgs.Uri);

                                    // Block non-standard ports
                                    if (uri.Port != 80 && uri.Port != 443 && uri.Port != -1)
                                    {
                                        navArgs.Cancel = true;
                                        return;
                                    }

                                    // Block URLs using IP addresses instead of domain names
                                    if (System.Net.IPAddress.TryParse(uri.Host, out _))
                                    {
                                        navArgs.Cancel = true;
                                        return;
                                    }


                                    if (uri.Scheme != "http" && uri.Scheme != "https")
                                    {
                                        navArgs.Cancel = true;
                                        return;
                                    }

                                    string[] dangerousExtensions = { ".exe", ".bat", ".cmd", ".com", ".scr", ".vbs", ".jar", ".msi", ".ps1", ".dll" };
                                    foreach (string ext in dangerousExtensions)
                                    {
                                        if (uri.AbsolutePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                                        {
                                            navArgs.Cancel = true;
                                            return;
                                        }
                                    }
                                }
                                catch
                                {
                                    navArgs.Cancel = true;
                                }
                            };

                            // Block new windows
                            webView.CoreWebView2.NewWindowRequested += (winSender, winArgs) =>
                            {
                                winArgs.Handled = true;
                            };

                            // Block downloads
                            webView.CoreWebView2.DownloadStarting += (dlSender, dlArgs) =>
                            {
                                dlArgs.Cancel = true;
                            };

                            webView.CoreWebView2.NavigationCompleted += (navSender, navArgs) =>
                            {
                                if (completed) return;

                                navigationCount++;

                                try
                                {
                                    if (navArgs.IsSuccess)
                                    {
                                        string finalUrl = webView.CoreWebView2.Source;

                                        if (navigationCount >= 2 || !IsShortenerDomain(finalUrl))
                                        {
                                            completed = true;
                                            tcs.TrySetResult(finalUrl);
                                            Application.ExitThread();
                                        }
                                    }
                                    else
                                    {
                                        completed = true;
                                        tcs.TrySetResult(shortUrl);
                                        Application.ExitThread();
                                    }
                                }
                                catch
                                {
                                    completed = true;
                                    tcs.TrySetResult(shortUrl);
                                    Application.ExitThread();
                                }
                            };

                            webView.CoreWebView2.Navigate(shortUrl);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                            Application.ExitThread();
                        }
                    }

                    hiddenForm.Load += OnFormLoad;
                    hiddenForm.Show();
                    hiddenForm.Hide();
                    Application.Run();
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();

            var timeoutTask = Task.Delay(timeoutMs);
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                try
                {
                    Application.Exit();
                    if (uiThread.IsAlive && !uiThread.Join(1000))
                    {
                        // Thread blocked, last resort
                        uiThread.Interrupt();
                    }
                }
                catch
                {}

                return shortUrl;
            }

            return await tcs.Task;
        }

        static bool IsShortenerDomain(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            try
            {
                Uri uri = new Uri(url);
                string host = uri.Host.ToLower();

                string[] shorteners = new[]
                {
                    "bit.ly", "tinyurl.com", "shorturl.at", "goo.gl", "ow.ly",
                    "is.gd", "buff.ly", "adf.ly", "t.co", "lnkd.in",
                    "rebrand.ly", "cutt.ly", "short.io", "tiny.cc", "rb.gy", "urlz.fr"
                };

                foreach (string shortener in shorteners)
                {
                    if (host == shortener || host.EndsWith("." + shortener))
                        return true;
                }
            }
            catch { }

            return false;
        }
    }
}