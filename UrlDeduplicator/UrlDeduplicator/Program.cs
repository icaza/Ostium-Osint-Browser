using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace UrlDeduplicator
{
    /// <summary>
    /// Simple, low-overhead console tool that reads a text file containing one URL
    /// per line and writes a new file with every duplicate line removed, while the
    /// original order of first appearance is preserved.
    ///
    /// Design goals:
    ///  - Streams the file line by line (StreamReader/StreamWriter over buffered
    ///    FileStreams) instead of loading it into memory, so it scales to very
    ///    large files with a small, predictable memory footprint.
    ///  - Uses a HashSet&lt;string&gt; with an ordinal comparer for O(1) duplicate
    ///    checks (fastest string comparison available, and the correct one for
    ///    URLs, since two URLs that differ only by case are different resources).
    ///  - Reports live progress (percentage, line count, throughput) without
    ///    allocating on every line, keeping CPU overhead of the UI negligible.
    /// </summary>
    internal static class Program
    {
        const int IoBufferSize = 1 << 20;

        const int ProgressUpdateIntervalMs = 100;

        static int Main(string[] args)
        {
            var enUs = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = enUs;
            Thread.CurrentThread.CurrentUICulture = enUs;
            CultureInfo.DefaultThreadCurrentCulture = enUs;
            CultureInfo.DefaultThreadCurrentUICulture = enUs;

            PrintHeader();

            string inputPath = PromptForInputFile(args);
            if (inputPath == null)
            {
                return ExitWithPause(1);
            }

            string outputPath = BuildOutputPath(inputPath);

            try
            {
                RunDeduplication(inputPath, outputPath);
            }
            catch (IOException ex)
            {
                return FailWith("File error: " + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return FailWith("Access denied: " + ex.Message);
            }
            catch (Exception ex)
            {
                return FailWith("Unexpected error: " + ex.Message);
            }

            return ExitWithPause(0);
        }

        static void PrintHeader()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("        OSTIUM URL Deduplicator");
            Console.WriteLine("========================================");
            Console.WriteLine("Removes duplicate URLs from a large text file.");
            Console.WriteLine();
        }

        static string PromptForInputFile(string[] args)
        {
            string path = args.Length > 0 ? args[0] : null;

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.Write("Enter the path of the URL file to process, then press Enter: ");
                path = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("No file path was provided. Exiting.");
                return null;
            }

            path = path.Trim().Trim('"');

            if (!File.Exists(path))
            {
                Console.WriteLine("File not found: " + path);
                return null;
            }

            return path;
        }

        static string BuildOutputPath(string inputPath)
        {
            string directory = Path.GetDirectoryName(inputPath);
            string fileName = Path.GetFileNameWithoutExtension(inputPath);
            string extension = Path.GetExtension(inputPath);
            string outputFileName = fileName + "_deduplicated" + extension;

            return string.IsNullOrEmpty(directory)
                ? outputFileName
                : Path.Combine(directory, outputFileName);
        }

        static void RunDeduplication(string inputPath, string outputPath)
        {
            Console.WriteLine();
            Console.WriteLine("Analyzing file...");

            long fileSizeBytes = new FileInfo(inputPath).Length;
            long totalLines = CountLines(inputPath);

            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "File size:   {0:N2} MB", fileSizeBytes / (1024.0 * 1024.0)));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "Total lines: {0:N0}", totalLines));
            Console.WriteLine();
            Console.WriteLine("Processing...");
            Console.WriteLine();

            var seen = totalLines > 0 && totalLines < int.MaxValue
                ? new HashSet<string>((int)Math.Min(totalLines, 4_000_000), StringComparer.Ordinal)
                : new HashSet<string>(StringComparer.Ordinal);

            long processedLines = 0;
            long uniqueCount = 0;
            long duplicateCount = 0;
            long blankCount = 0;

            var stopwatch = Stopwatch.StartNew();
            long lastReportMs = -ProgressUpdateIntervalMs;

            using (var reader = new StreamReader(
                       new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read, IoBufferSize),
                       Encoding.UTF8, true, IoBufferSize))
            using (var writer = new StreamWriter(
                       new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, IoBufferSize),
                       new UTF8Encoding(false), IoBufferSize))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    processedLines++;

                    string url = line.Trim();

                    if (url.Length == 0)
                    {
                        blankCount++;
                    }
                    else if (seen.Add(url))
                    {
                        writer.WriteLine(url);
                        uniqueCount++;
                    }
                    else
                    {
                        duplicateCount++;
                    }

                    long elapsedMs = stopwatch.ElapsedMilliseconds;
                    if (elapsedMs - lastReportMs >= ProgressUpdateIntervalMs || processedLines == totalLines)
                    {
                        lastReportMs = elapsedMs;
                        ReportProgress(processedLines, totalLines, stopwatch.Elapsed);
                    }
                }
            }

            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine();
            PrintSummary(processedLines, uniqueCount, duplicateCount, blankCount, stopwatch.Elapsed, outputPath);
        }

        static long CountLines(string path)
        {
            long count = 0;
            bool sawAnyBytes = false;
            bool lastByteWasNewline = false;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, IoBufferSize))
            {
                byte[] buffer = new byte[IoBufferSize];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    sawAnyBytes = true;
                    for (int i = 0; i < bytesRead; i++)
                    {
                        if (buffer[i] == (byte)'\n')
                        {
                            count++;
                            lastByteWasNewline = true;
                        }
                        else
                        {
                            lastByteWasNewline = false;
                        }
                    }
                }
            }

            if (sawAnyBytes && !lastByteWasNewline)
            {
                count++;
            }

            return count;
        }

        static void ReportProgress(long processed, long total, TimeSpan elapsed)
        {
            double percent = total > 0 ? Math.Min(100.0, processed * 100.0 / total) : 100.0;
            double linesPerSecond = elapsed.TotalSeconds > 0 ? processed / elapsed.TotalSeconds : 0;

            const int barWidth = 30;
            int filled = (int)(barWidth * percent / 100.0);
            if (filled > barWidth) filled = barWidth;
            if (filled < 0) filled = 0;

            var line = new StringBuilder(128);
            line.Append('\r').Append('[');
            line.Append('#', filled);
            line.Append('-', barWidth - filled);
            line.Append(']');
            line.Append(' ').Append(percent.ToString("N1", CultureInfo.InvariantCulture).PadLeft(5)).Append('%');
            line.Append("  ").Append(processed.ToString("N0", CultureInfo.InvariantCulture));
            line.Append('/').Append(total.ToString("N0", CultureInfo.InvariantCulture)).Append(" lines");
            line.Append("  ").Append(linesPerSecond.ToString("N0", CultureInfo.InvariantCulture)).Append(" lines/sec");

            Console.Write(line.ToString().PadRight(100));
        }

        static void PrintSummary(long processed, long unique, long duplicates, long blanks,
                                          TimeSpan elapsed, string outputPath)
        {
            Console.WriteLine("Done.");
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Lines read:          {0:N0}", processed));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Unique URLs written: {0:N0}", unique));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Duplicates removed:  {0:N0}", duplicates));
            if (blanks > 0)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Blank lines skipped: {0:N0}", blanks));
            }
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Time elapsed:        {0:mm\\:ss\\.ff}", elapsed));
            Console.WriteLine();
            Console.WriteLine("Output file: " + outputPath);
        }

        static int FailWith(string message)
        {
            Console.WriteLine();
            ConsoleColor previous = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = previous;
            return ExitWithPause(1);
        }

        static int ExitWithPause(int exitCode)
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
            return exitCode;
        }
    }
}
