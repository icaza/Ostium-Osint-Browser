using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Drawing.Imaging;
using System.Text;

namespace OOBpdfC
{
    public enum OutputFormat
    {
        Txt,
        Markdown,
        Both
    }

    public class ConversionOptions
    {
        public OutputFormat OutputFormat { get; set; } = OutputFormat.Both;
        public bool ExtractMetadata { get; set; } = true;
        public bool ExtractImages { get; set; } = true;
        public bool ExtractTables { get; set; } = true;
        public bool PreserveFormatting { get; set; } = true;
    }

    public class PdfProcessor
    {
        public const int MaxFileSizeMb = 100;
        public const long MaxFileSizeBytes = MaxFileSizeMb * 1024L * 1024L;

        public static async Task<bool> ConvertPdfAsync(string pdfPath, ConversionOptions options)
        {
            if (!ValidatePdfPath(pdfPath, out string? errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return await Task.Run(() =>
            {
                try
                {
                    string directory = Path.GetDirectoryName(pdfPath)
                    ?? Environment.CurrentDirectory;
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(pdfPath);

                    var pdfData = ExtractPdfData(pdfPath, options);

                    bool success = false;

                    if (options.OutputFormat == OutputFormat.Txt || options.OutputFormat == OutputFormat.Both)
                    {
                        string txtPath = Path.Combine(directory, $"{fileNameWithoutExt}.txt");
                        SaveTextFile(txtPath, GenerateTextOutput(pdfData, options));
                        success = true;
                    }

                    if (options.OutputFormat == OutputFormat.Markdown || options.OutputFormat == OutputFormat.Both)
                    {
                        string mdPath = Path.Combine(directory, $"{fileNameWithoutExt}.md");
                        SaveTextFile(mdPath, GenerateMarkdownOutput(pdfData, fileNameWithoutExt, options));
                        success = true;
                    }

                    if (options.ExtractImages && pdfData.Images.Count > 0)
                    {
                        string imagesDir = Path.Combine(directory, $"{fileNameWithoutExt}_images");
                        Directory.CreateDirectory(imagesDir);

                        for (int i = 0; i < pdfData.Images.Count; i++)
                        {
                            string imagePath = Path.Combine(imagesDir, $"image_{i + 1}.png");
                            pdfData.Images[i].Save(imagePath, ImageFormat.Png);
                        }
                    }

                    return success;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error during conversion: {ex.Message}", ex);
                }
            });
        }

        static PdfData ExtractPdfData(string pdfPath, ConversionOptions options)
        {
            var pdfData = new PdfData();

            using (PdfReader reader = new(pdfPath))
            using (PdfDocument pdfDoc = new(reader))
            {
                if (options.ExtractMetadata)
                {
                    pdfData.Metadata = ExtractMetadata(pdfDoc);
                }

                int numberOfPages = pdfDoc.GetNumberOfPages();
                pdfData.TotalPages = numberOfPages;

                for (int i = 1; i <= numberOfPages; i++)
                {
                    var page = pdfDoc.GetPage(i);
                    var pageData = new PageData { PageNumber = i };

                    if (options.PreserveFormatting)
                    {
                        var strategy = new LocationTextExtractionStrategy();
                        pageData.Text = PdfTextExtractor.GetTextFromPage(page, strategy);
                    }
                    else
                    {
                        pageData.Text = PdfTextExtractor.GetTextFromPage(page);
                    }

                    if (options.ExtractImages)
                    {
                        var images = ExtractImagesFromPage(page);
                        pdfData.Images.AddRange(images);
                        pageData.ImageCount = images.Count;
                    }

                    if (options.ExtractTables)
                    {
                        pageData.HasTables = DetectTables(pageData.Text);
                    }

                    pdfData.Pages.Add(pageData);
                }
            }

            return pdfData;
        }

        static Dictionary<string, string> ExtractMetadata(PdfDocument pdfDoc)
        {
            var metadata = new Dictionary<string, string>();

            var info = pdfDoc.GetDocumentInfo();

            if (!string.IsNullOrWhiteSpace(info.GetTitle()))
                metadata["Title"] = info.GetTitle();

            if (!string.IsNullOrWhiteSpace(info.GetAuthor()))
                metadata["Author"] = info.GetAuthor();

            if (!string.IsNullOrWhiteSpace(info.GetSubject()))
                metadata["Subject"] = info.GetSubject();

            if (!string.IsNullOrWhiteSpace(info.GetKeywords()))
                metadata["Keywords"] = info.GetKeywords();

            if (!string.IsNullOrWhiteSpace(info.GetCreator()))
                metadata["Creator"] = info.GetCreator();

            if (!string.IsNullOrWhiteSpace(info.GetProducer()))
                metadata["Producer"] = info.GetProducer();

            var pdfVersion = pdfDoc.GetPdfVersion();
            metadata["PDF version"] = pdfVersion.ToString();

            metadata["Number of pages"] = pdfDoc.GetNumberOfPages().ToString();

            return metadata;
        }

        static List<Bitmap> ExtractImagesFromPage(PdfPage page)
        {
            var images = new List<Bitmap>();

            try
            {
                var strategy = new ImageRenderListener();
                var parser = new PdfCanvasProcessor(strategy);
                parser.ProcessPageContent(page);
                images.AddRange(strategy.Images);
            }
            catch
            {
                // Ignore image extraction errors
            }

            return images;
        }

        static bool DetectTables(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var lines = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            int alignedLines = 0;

            foreach (var line in lines)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(line, @"\s{2,}"))
                    alignedLines++;
            }

            return alignedLines > lines.Length * 0.3;
        }

        static string GenerateTextOutput(PdfData pdfData, ConversionOptions options)
        {
            var sb = new StringBuilder();

            if (options.ExtractMetadata && pdfData.Metadata.Count > 0)
            {
                sb.AppendLine("========================================");
                sb.AppendLine("           PDF METADATA");
                sb.AppendLine("========================================");
                sb.AppendLine();

                foreach (var kvp in pdfData.Metadata)
                {
                    sb.AppendLine($"{kvp.Key}: {kvp.Value}");
                }

                sb.AppendLine();
                sb.AppendLine("========================================");
                sb.AppendLine("            DOCUMENT CONTENT");
                sb.AppendLine("========================================");
                sb.AppendLine();
            }

            foreach (var page in pdfData.Pages)
            {
                sb.AppendLine($"--- Page {page.PageNumber} ---");
                sb.AppendLine();
                sb.AppendLine(page.Text);
                sb.AppendLine();

                if (page.ImageCount > 0)
                {
                    sb.AppendLine($"[{page.ImageCount} image(s) extracted from this page]");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        static string GenerateMarkdownOutput(PdfData pdfData, string title, ConversionOptions options)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"# {EscapeMarkdown(title)}");
            sb.AppendLine();
            sb.AppendLine($"*Document converted on {DateTime.Now:dd/MM/yyyy à HH:mm:ss}*");
            sb.AppendLine();

            if (options.ExtractMetadata && pdfData.Metadata.Count > 0)
            {
                sb.AppendLine("## 📋 Métadonnées");
                sb.AppendLine();

                foreach (var kvp in pdfData.Metadata)
                {
                    sb.AppendLine($"- **{kvp.Key}**: {EscapeMarkdown(kvp.Value)}");
                }

                sb.AppendLine();
            }

            sb.AppendLine("## 📊 Document statistics");
            sb.AppendLine();
            sb.AppendLine($"- **Total pages**: {pdfData.TotalPages}");
            sb.AppendLine($"- **Images extracted**: {pdfData.Images.Count}");
            sb.AppendLine($"- **Pages with tables**: {pdfData.Pages.Count(p => p.HasTables)}");
            sb.AppendLine();

            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 📄 Content");
            sb.AppendLine();

            foreach (var page in pdfData.Pages)
            {
                sb.AppendLine($"### Page {page.PageNumber}");
                sb.AppendLine();

                if (page.ImageCount > 0)
                {
                    sb.AppendLine($"> 🖼️ *This page contains {page.ImageCount} image(s)*");
                    sb.AppendLine();
                }

                if (page.HasTables)
                {
                    sb.AppendLine($"> 📊 *This page probably contains one or more tables*");
                    sb.AppendLine();
                }

                var lines = page.Text.Split(['\r', '\n'], StringSplitOptions.None);
                bool inCodeBlock = false;

                foreach (var line in lines)
                {
                    string trimmed = line.Trim();

                    if (line.Length > 0 && line[0] == ' ' && line.TrimStart().Length > 0)
                    {
                        if (!inCodeBlock)
                        {
                            sb.AppendLine("```");
                            inCodeBlock = true;
                        }
                        sb.AppendLine(line);
                    }
                    else
                    {
                        if (inCodeBlock)
                        {
                            sb.AppendLine("```");
                            sb.AppendLine();
                            inCodeBlock = false;
                        }
                        sb.AppendLine(EscapeMarkdown(trimmed));
                    }
                }

                if (inCodeBlock)
                {
                    sb.AppendLine("```");
                }

                sb.AppendLine();
                sb.AppendLine("---");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        static string EscapeMarkdown(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text.Replace("\\", "\\\\")
                      .Replace("`", "\\`")
                      .Replace("*", "\\*")
                      .Replace("_", "\\_")
                      .Replace("{", "\\{")
                      .Replace("}", "\\}")
                      .Replace("[", "\\[")
                      .Replace("]", "\\]")
                      .Replace("(", "\\(")
                      .Replace(")", "\\)")
                      .Replace("#", "\\#")
                      .Replace("+", "\\+")
                      .Replace("-", "\\-")
                      .Replace(".", "\\.")
                      .Replace("!", "\\!");
        }

        static bool ValidatePdfPath(string pdfPath, out string? errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(pdfPath))
            {
                errorMessage = "The file path is empty.";
                return false;
            }

            if (!File.Exists(pdfPath))
            {
                errorMessage = "The file does not exist.";
                return false;
            }

            string extension = Path.GetExtension(pdfPath)
                .ToLowerInvariant();
            if (extension != ".pdf")
            {
                errorMessage = "The file must be in PDF format.";
                return false;
            }

            var fileInfo = new FileInfo(pdfPath);
            if (fileInfo.Length > MaxFileSizeBytes)
            {
                errorMessage = $"The file exceeds the maximum allowed size ({MaxFileSizeMb} MB).";
                return false;
            }

            if (fileInfo.Length == 0)
            {
                errorMessage = "The file is empty.";
                return false;
            }

            return true;
        }

        static void SaveTextFile(string filePath, string content)
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, content, Encoding.UTF8);
        }
    }

    internal class PdfData
    {
        public Dictionary<string, string> Metadata { get; set; } = new();
        public List<PageData> Pages { get; set; } = new();
        public List<Bitmap> Images { get; set; } = new();
        public int TotalPages { get; set; }
    }

    internal class PageData
    {
        public int PageNumber { get; set; }
        public string Text { get; set; } = string.Empty;
        public int ImageCount { get; set; }
        public bool HasTables { get; set; }
    }

    internal class ImageRenderListener : IEventListener
    {
        public List<Bitmap> Images { get; } = [];

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type != EventType.RENDER_IMAGE)
                return;

            try
            {
                if (data is ImageRenderInfo imageInfo)
                {
                    var image = imageInfo.GetImage();
                    if (image != null)
                    {
                        var imageBytes = image.GetImageBytes();
                        using var ms = new MemoryStream(imageBytes);
                        var bitmap = new Bitmap(ms);
                        Images.Add(new Bitmap(bitmap)); // Clone pour éviter les problèmes de disposal
                    }
                }
            }
            catch
            {
                // Ignore errors in extracting individual images
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return [EventType.RENDER_IMAGE];
        }
    }
}