using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WebPToPngConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║       WebP to PNG Converter v1.0.0.1       ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            try
            {
                if (args.Length > 0)
                {
                    ProcessCommandLine(args);
                }
                else
                {
                    ProcessInteractive();
                }
            }
            catch (Exception ex)
            {
                DisplayError($"Critical error: {ex.Message}");
                Environment.Exit(1);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void ProcessCommandLine(string[] args)
        {
            string inputPath = args[0];
            string outputPath = args.Length > 1 ? args[1] : null;

            if (Directory.Exists(inputPath))
            {
                ConvertDirectory(inputPath, outputPath);
            }
            else if (File.Exists(inputPath))
            {
                ConvertSingleFile(inputPath, outputPath);
            }
            else
            {
                DisplayError("The specified path does not exist.");
            }
        }

        static void ProcessInteractive()
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("  [1] Convert a single file");
            Console.WriteLine("  [2] Convert all files in a folder");
            Console.WriteLine("  [3] Exit");
            Console.Write("\nYour choice : ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    InteractiveSingleFile();
                    break;
                case "2":
                    InteractiveDirectory();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    DisplayWarning("Invalid choice.");
                    break;
            }
        }

        static void InteractiveSingleFile()
        {
            Console.Write("WebP file path: ");
            string inputPath = Console.ReadLine()?.Trim('"');

            if (string.IsNullOrEmpty(inputPath) || !File.Exists(inputPath))
            {
                DisplayError("File not found.");
                return;
            }

            Console.Write("PNG output path (empty = same folder) : ");
            string outputPath = Console.ReadLine()?.Trim('"');

            ConvertSingleFile(inputPath, outputPath);
        }

        static void InteractiveDirectory()
        {
            Console.Write("Path to the folder containing the WebP files: ");
            string inputPath = Console.ReadLine()?.Trim('"');

            if (string.IsNullOrEmpty(inputPath) || !Directory.Exists(inputPath))
            {
                DisplayError("Dossier introuvable.");
                return;
            }

            Console.Write("Output folder (empty = same folder) : ");
            string outputPath = Console.ReadLine()?.Trim('"');

            ConvertDirectory(inputPath, outputPath);
        }

        static void ConvertSingleFile(string inputPath, string outputPath = null)
        {
            if (!IsWebPFile(inputPath))
            {
                DisplayError("The file is not in WebP format.");
                return;
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = Path.ChangeExtension(inputPath, ".png");
            }
            else if (Directory.Exists(outputPath))
            {
                string fileName = Path.GetFileNameWithoutExtension(inputPath) + ".png";
                outputPath = Path.Combine(outputPath, fileName);
            }
            else if (string.IsNullOrEmpty(Path.GetExtension(outputPath)))
            {
                outputPath += ".png";
            }

            Console.Write($"Conversion of '{Path.GetFileName(inputPath)}'... ");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            if (ConvertWebPToPng(inputPath, outputPath))
            {
                stopwatch.Stop();
                DisplaySuccess($"✓ Finished ({stopwatch.ElapsedMilliseconds}ms)");
                Console.WriteLine($"   File created: {outputPath}");
            }
            else
            {
                DisplayError("✗ Failure");
            }
        }

        static void ConvertDirectory(string inputDir, string outputDir = null)
        {
            if (string.IsNullOrEmpty(outputDir))
            {
                outputDir = inputDir;
            }
            else if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var webpFiles = Directory.GetFiles(inputDir, "*.webp", SearchOption.TopDirectoryOnly);

            if (webpFiles.Length == 0)
            {
                DisplayWarning("No WebP files found in folder.");
                return;
            }

            Console.WriteLine($"Files found: {webpFiles.Length}");
            Console.WriteLine();

            int success = 0;
            int failed = 0;
            var totalStopwatch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var inputFile in webpFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(inputFile) + ".png";
                string outputFile = Path.Combine(outputDir, fileName);

                Console.Write($"[{success + failed + 1}/{webpFiles.Length}] {Path.GetFileName(inputFile)}... ");

                if (ConvertWebPToPng(inputFile, outputFile))
                {
                    DisplaySuccess("✓");
                    success++;
                }
                else
                {
                    DisplayError("✗");
                    failed++;
                }
            }

            totalStopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine(new string('─', 50));
            DisplaySuccess($"Successful Conversions: {success}");
            if (failed > 0) DisplayError($"failed conversions : {failed}");
            Console.WriteLine($"Total time : {totalStopwatch.ElapsedMilliseconds}ms");
        }

        static bool ConvertWebPToPng(string inputPath, string outputPath)
        {
            try
            {
                // Use WIC (Windows Imaging Component) to read WebP
                byte[] webpBytes = File.ReadAllBytes(inputPath);

                using (var bitmap = LoadWebPWithWIC(webpBytes))
                {
                    if (bitmap == null)
                    {
                        DisplayError("\n   Error: Unable to decode WebP file.");
                        return false;
                    }

                    bitmap.Save(outputPath, ImageFormat.Png);
                }

                return true;
            }
            catch (Exception ex)
            {
                DisplayError($"\n   Error : {ex.Message}");
                return false;
            }
        }

        static Bitmap LoadWebPWithWIC(byte[] webpData)
        {
            try
            {
                // Create a memory stream
                using (var stream = new MemoryStream(webpData))
                {
                    // Use BitmapImage which can decode WebP via WIC
                    var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    // Convert to GDI+ Bitmap
                    int width = bitmapImage.PixelWidth;
                    int height = bitmapImage.PixelHeight;

                    var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    var bitmapData = bitmap.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb
                    );

                    try
                    {
                        // Copy the pixels
                        bitmapImage.CopyPixels(
                            new System.Windows.Int32Rect(0, 0, width, height),
                            bitmapData.Scan0,
                            bitmapData.Stride * height,
                            bitmapData.Stride
                        );
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    return bitmap;
                }
            }
            catch
            {
                // If WIC fails, try using Image.FromStream (for supported formats).
                try
                {
                    using (var stream = new MemoryStream(webpData))
                    {
                        return new Bitmap(Image.FromStream(stream));
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        static bool IsWebPFile(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".webp", StringComparison.OrdinalIgnoreCase);
        }

        static void DisplaySuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void DisplayWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}