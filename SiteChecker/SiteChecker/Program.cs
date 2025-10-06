using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SiteChecker
{
    class Program
    {
        static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 5
        })
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        static readonly ConcurrentBag<string> sitesOnline = new ConcurrentBag<string>();
        static readonly ConcurrentBag<string> sitesOffline = new ConcurrentBag<string>();
        static readonly ConcurrentBag<SiteError> errors = new ConcurrentBag<SiteError>();
        static readonly SemaphoreSlim semaphore = new SemaphoreSlim(10);
        static int completedCount = 0;
        static int totalCount = 0;

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== SiteChecker - Website Status Verification ===\n");

            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

            string inputFile = GetInputFile(args);

            if (string.IsNullOrEmpty(inputFile))
            {
                Console.WriteLine("\nNo file specified. Closing application.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nSelected file: {inputFile}");

            string[] urls = File.ReadAllLines(inputFile)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                .ToArray();

            if (urls.Length == 0)
            {
                Console.WriteLine("No valid URLs found in file.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            totalCount = urls.Length;
            Console.WriteLine($"Checking {totalCount} site(s)...\n");

            var startTime = DateTime.Now;

            var progressTask = Task.Run(() => DisplayProgress());

            var tasks = urls.Select(url => CheckSiteAsync(url)).ToArray();
            await Task.WhenAll(tasks);

            await progressTask;

            var duration = DateTime.Now - startTime;

            Console.Write($"\rProgress: 100% ({totalCount}/{totalCount} sites checked)     \n");

            WriteResults();

            Console.WriteLine("\n=== Summary ===");
            Console.WriteLine($"Online sites: {sitesOnline.Count}");
            Console.WriteLine($"Offline sites: {sitesOffline.Count}");
            Console.WriteLine($"Execution time: {duration.TotalSeconds:F2}s");
            Console.WriteLine("\nGenerated files:");
            Console.WriteLine("   - in.txt (online sites)");
            Console.WriteLine("   - out.txt (offline sites)");
            Console.WriteLine("   - report.txt (detailed report)");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static string GetInputFile(string[] args)
        {

            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    return args[0];
                }
                else
                {
                    Console.WriteLine($"Warning: File '{args[0]}' does not exist.");
                }
            }

            if (File.Exists("sites.txt"))
            {
                Console.WriteLine("File 'sites.txt' found.");
                Console.Write("Use this file (Y/n): ");
                string response = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(response) || response == "y" || response == "yes")
                {
                    return "sites.txt";
                }
            }

            Console.WriteLine("\nPlease enter the path to the file containing sites:");
            Console.Write("> ");
            string inputFile = Console.ReadLine()?.Trim().Trim('"');

            if (!string.IsNullOrEmpty(inputFile) && File.Exists(inputFile))
            {
                return inputFile;
            }
            else if (!string.IsNullOrEmpty(inputFile))
            {
                Console.WriteLine($"Error: File '{inputFile}' does not exist.");

                Console.Write("\nCreate sample file 'sites.txt' (Y/n): ");
                string createExample = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(createExample) || createExample == "y" || createExample == "yes")
                {
                    CreateSampleFile();
                    Console.WriteLine("\nFile created. Edit it and restart the application.");
                }
            }

            return null;
        }

        static async Task DisplayProgress()
        {
            while (completedCount < totalCount)
            {
                int completed = completedCount;
                double percentage = (double)completed / totalCount * 100;
                Console.Write($"\rProgress: {percentage:F1}% ({completed}/{totalCount} sites checked)");
                await Task.Delay(100);
            }
        }

        static async Task CheckSiteAsync(string url)
        {
            await semaphore.WaitAsync();
            try
            {
                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    url = "https://" + url;
                }

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    sitesOnline.Add(url);
                }
                else
                {
                    sitesOffline.Add(url);
                    errors.Add(new SiteError
                    {
                        Url = url,
                        ErrorMessage = $"HTTP {(int)response.StatusCode} - {response.ReasonPhrase}"
                    });
                }
            }
            catch (HttpRequestException ex)
            {
                sitesOffline.Add(url);
                errors.Add(new SiteError
                {
                    Url = url,
                    ErrorMessage = $"Connection error: {ex.Message}"
                });
            }
            catch (TaskCanceledException)
            {
                sitesOffline.Add(url);
                errors.Add(new SiteError
                {
                    Url = url,
                    ErrorMessage = "Timeout - Site does not respond within the time limit"
                });
            }
            catch (Exception ex)
            {
                sitesOffline.Add(url);
                errors.Add(new SiteError
                {
                    Url = url,
                    ErrorMessage = $"Unexpected error: {ex.Message}"
                });
            }
            finally
            {
                Interlocked.Increment(ref completedCount);
                semaphore.Release();
            }
        }

        static void WriteResults()
        {
            File.WriteAllLines("in.txt", sitesOnline.OrderBy(s => s));
            File.WriteAllLines("out.txt", sitesOffline.OrderBy(s => s));

            var reportLines = new List<string>
            {
                "=== SITE VERIFICATION REPORT ===",
                $"Date: {DateTime.Now:MM/dd/yyyy HH:mm:ss}",
                "",
                $"Online sites: {sitesOnline.Count}",
                $"Offline sites: {sitesOffline.Count}",
                $"Total: {sitesOnline.Count + sitesOffline.Count}",
                ""
            };

            if (errors.Count > 0)
            {
                reportLines.Add("=== ERROR DETAILS ===");
                reportLines.Add("");

                foreach (var error in errors.OrderBy(e => e.Url))
                {
                    reportLines.Add($"URL: {error.Url}");
                    reportLines.Add($"Error: {error.ErrorMessage}");
                    reportLines.Add("");
                }
            }
            else
            {
                reportLines.Add("No errors detected - All sites are online!");
            }

            File.WriteAllLines("report.txt", reportLines);
        }

        static void CreateSampleFile()
        {
            var sampleSites = new[]
            {
                "# Sample file - List of sites to check",
                "# Lines starting with # are ignored",
                "",
                "https://www.google.com",
                "https://www.github.com",
                "https://www.microsoft.com",
                "https://www.example.com",
                "https://sitedoesnotexist123456.com"
            };

            File.WriteAllLines("sites.txt", sampleSites);
            Console.WriteLine("File 'sites.txt' created with examples.");
        }
    }

    class SiteError
    {
        public string Url { get; set; }
        public string ErrorMessage { get; set; }
    }
}