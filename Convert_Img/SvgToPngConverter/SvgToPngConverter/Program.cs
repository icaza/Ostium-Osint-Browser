using Svg;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace SvgToPngConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║        SVG to PNG Converter v1.0.0.1       ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            try
            {
                string inputPath = GetInputPath(args);

                if (File.Exists(inputPath))
                {
                    string outputPath = GetOutputPath(inputPath, args);
                    ConvertSvgToPng(inputPath, outputPath);
                    DisplaySuccess(outputPath);
                }
                else if (Directory.Exists(inputPath))
                {
                    string outputDirectory = GetOutputDirectory(inputPath, args);
                    ConvertDirectory(inputPath, outputDirectory);
                }
                else
                {
                    throw new FileNotFoundException("The specified path does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ Error : {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static string GetInputPath(string[] args)
        {
            string inputPath;
            if (args.Length > 0 && (File.Exists(args[0]) || Directory.Exists(args[0])))
            {
                inputPath = args[0];
            }
            else
            {
                Console.WriteLine("Enter the path to the SVG file or directory:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("> ");
                Console.ResetColor();
                inputPath = Console.ReadLine()?.Trim('"', ' ');
            }

            if (string.IsNullOrEmpty(inputPath))
            {
                throw new ArgumentException("No path specified.");
            }

            return inputPath;
        }

        static string GetOutputPath(string inputPath, string[] args)
        {
            string outputPath;

            if (args.Length > 1)
            {
                outputPath = args[1];
            }
            else
            {
                string directory = Path.GetDirectoryName(inputPath);
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(inputPath);
                outputPath = Path.Combine(directory, $"{fileNameWithoutExt}.png");

                Console.WriteLine($"\nDefault output file : {outputPath}");
                Console.WriteLine("Press Enter to accept or specify another path:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("> ");
                Console.ResetColor();

                string customPath = Console.ReadLine()?.Trim('"', ' ');
                if (!string.IsNullOrEmpty(customPath))
                {
                    outputPath = customPath;
                }
            }

            if (!Path.GetExtension(outputPath).Equals(".png", StringComparison.OrdinalIgnoreCase))
            {
                outputPath += ".png";
            }

            return outputPath;
        }

        static string GetOutputDirectory(string inputDirectory, string[] args)
        {
            string outputDirectory;

            if (args.Length > 1)
            {
                outputDirectory = args[1];
            }
            else
            {
                outputDirectory = Path.Combine(inputDirectory, "converted");

                Console.WriteLine($"\nDefault output directory : {outputDirectory}");
                Console.WriteLine("Press Enter to accept or specify another path:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("> ");
                Console.ResetColor();

                string customPath = Console.ReadLine()?.Trim('"', ' ');
                if (!string.IsNullOrEmpty(customPath))
                {
                    outputDirectory = customPath;
                }
            }

            return outputDirectory;
        }

        static void ConvertDirectory(string inputDirectory, string outputDirectory)
        {
            var svgFiles = Directory.GetFiles(inputDirectory, "*.svg", SearchOption.AllDirectories);

            if (svgFiles.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nNo SVG files were found in the directory..");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"\n► {svgFiles.Length} SVG file(s) found");
            Console.WriteLine($"► Output directory: {outputDirectory}\n");

            int successCount = 0;
            int failCount = 0;

            for (int i = 0; i < svgFiles.Length; i++)
            {
                string svgFile = svgFiles[i];
                string relativePath = Path.Combine(inputDirectory, svgFile);
                string outputFile = Path.Combine(outputDirectory, Path.ChangeExtension(relativePath, ".png"));

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[{i + 1}/{svgFiles.Length}] {relativePath}");
                Console.ResetColor();

                try
                {
                    ConvertSvgToPng(svgFile, outputFile);
                    successCount++;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"    ✓ Converted successfully");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    failCount++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"    ✗ Error : {ex.Message}");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }

            Console.WriteLine("═══════════════════════════════════════════");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Successful : {successCount}");
            Console.ResetColor();

            if (failCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Chess  : {failCount}");
                Console.ResetColor();
            }

            Console.WriteLine("═══════════════════════════════════════════");
        }

        static void ConvertSvgToPng(string svgPath, string pngPath)
        {
            var svgDocument = SvgDocument.Open(svgPath) ?? throw new InvalidOperationException("Unable to load SVG document.");

            float width = svgDocument.Width.Value;
            float height = svgDocument.Height.Value;

            if (svgDocument.Width.Type == SvgUnitType.Percentage || width == 0)
            {
                if (svgDocument.ViewBox != null && svgDocument.ViewBox.Width > 0)
                {
                    width = svgDocument.ViewBox.Width;
                    height = svgDocument.ViewBox.Height;
                }
                else
                {
                    width = 800;
                    height = 600;
                }
            }

            using (var bitmap = svgDocument.Draw((int)width, (int)height))
            {
                if (bitmap == null)
                {
                    throw new InvalidOperationException("The SVG to Bitmap conversion failed..");
                }

                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(
                    Encoder.Quality,
                    100L
                );

                var pngEncoder = GetEncoder(ImageFormat.Png);

                Directory.CreateDirectory(Path.GetDirectoryName(pngPath));
                bitmap.Save(pngPath, pngEncoder, encoderParameters);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        static void DisplaySuccess(string outputPath)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✓ Successful conversion!");
            Console.ResetColor();
            Console.WriteLine($"Output file : {outputPath}");
            Console.WriteLine($"Size : {new FileInfo(outputPath).Length / 1024} KB");
        }

        static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}