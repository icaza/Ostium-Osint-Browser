using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SemanticAnalyzer
{
    public class TextSemanticAnalyzer
    {
        #region Constants
        private const int MinWordLength = 3;
        private const int MaxWordLength = 50;
        private const int MaxFileSizeMB = 50;
        private const int TopWordsCount = 20;
        private const int TopBigramsCount = 15;
        private const int TopTrigramsCount = 10;
        private const int TopThemeWordsCount = 5;
        #endregion

        #region Properties

        LanguageConfig CurrentLanguage { get; set; }

        public static readonly HashSet<string> SupportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".md", ".csv", ".xml", ".json", ".log", ".html", ".htm",
            ".rtf", ".tex", ".rst", ".yaml", ".yml", ".ini", ".cfg", ".conf"
        };

        #endregion

        #region Public Methods

        public async Task<SemanticAnalysisResult> AnalyzeDocumentAsync(string filePath, string languageFilePath, IProgress<int> progress = null)
        {
            ValidateInputs(filePath, languageFilePath);

            progress?.Report(10);
            CurrentLanguage = await LoadLanguageConfigAsync(languageFilePath);

            progress?.Report(20);
            string content = await LoadDocumentContentAsync(filePath);

            progress?.Report(40);
            var result = await Task.Run(() => PerformSemanticAnalysis(content, filePath, progress));

            progress?.Report(100);
            return result;
        }

        public async Task SaveAsHtmlAsync(SemanticAnalysisResult result, string outputPath)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("The exit path is invalid.", nameof(outputPath));

            var html = GenerateHtmlReport(result);
            await Task.Run(() => File.WriteAllText(outputPath, html, Encoding.UTF8));
        }

        public async Task SaveAsMarkdownAsync(SemanticAnalysisResult result, string outputPath)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentException("The exit path is invalid.", nameof(outputPath));

            var markdown = GenerateMarkdownReport(result);
            await Task.Run(() => File.WriteAllText(outputPath, markdown, Encoding.UTF8));
        }

        #endregion

        #region Validation

        void ValidateInputs(string filePath, string languageFilePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("The file path is invalid.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("The file does not exist.", filePath);

            if (string.IsNullOrWhiteSpace(languageFilePath))
                throw new ArgumentException("The language file path is invalid.", nameof(languageFilePath));

            if (!File.Exists(languageFilePath))
                throw new FileNotFoundException("The language file does not exist.", languageFilePath);

            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > MaxFileSizeMB * 1024 * 1024)
                throw new InvalidOperationException($"The file exceeds the maximum size {MaxFileSizeMB} MB");

            var extension = Path.GetExtension(filePath);
            if (!SupportedExtensions.Contains(extension))
                throw new NotSupportedException($"The extension {extension} is not supported");
        }

        #endregion

        #region Document Loading

        async Task<string> LoadDocumentContentAsync(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            string content;

            try
            {
                var encoding = DetectEncoding(filePath);
                content = await Task.Run(() => File.ReadAllText(filePath, Encoding.UTF8));

                content = ExtractTextByFormat(content, extension);

                content = NormalizeText(content);

                if (string.IsNullOrWhiteSpace(content))
                    throw new InvalidOperationException("The file does not contain any usable text.");

                return content;
            }
            catch (IOException ex)
            {
                throw new IOException($"Error reading file: {ex.Message}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Access denied to the file: {ex.Message}", ex);
            }
        }

        Encoding DetectEncoding(string filePath)
        {
            using (var reader = new StreamReader(filePath, Encoding.Default, true))
            {
                reader.Peek();
                return reader.CurrentEncoding;
            }
        }

        string ExtractTextByFormat(string content, string extension)
        {
            switch (extension)
            {
                case ".xml":
                    return ExtractTextFromXml(content);
                case ".html":
                case ".htm":
                    return ExtractTextFromHtml(content);
                case ".csv":
                    return ExtractTextFromCsv(content);
                case ".json":
                    return ExtractTextFromJson(content);
                default:
                    return content;
            }
        }

        string ExtractTextFromXml(string content)
        {
            try
            {
                var doc = XDocument.Parse(content);
                return string.Join(" ", doc.Descendants().Select(e => e.Value));
            }
            catch
            {
                return content;
            }
        }

        string ExtractTextFromHtml(string content)
        {
            var noTags = Regex.Replace(content, @"<script[^>]*>.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            noTags = Regex.Replace(noTags, @"<style[^>]*>.*?</style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            noTags = Regex.Replace(noTags, @"<[^>]+>", " ");

            noTags = System.Net.WebUtility.HtmlDecode(noTags);

            return noTags;
        }

        string ExtractTextFromCsv(string content)
        {
            return Regex.Replace(content, @"[,;|\t]", " ");
        }

        string ExtractTextFromJson(string content)
        {
            var values = Regex.Matches(content, @"""[^""\\]*(?:\\.[^""\\]*)*""");
            return string.Join(" ", values.Cast<Match>().Select(m => m.Value.Trim('"')));
        }

        string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            text = Regex.Replace(text, @"[ \t]+", " ");
            text = Regex.Replace(text, @"\r\n|\r|\n", "\n");
            text = Regex.Replace(text, @"\n{3,}", "\n\n");

            return text.Trim();
        }

        #endregion

        #region Language Configuration

        async Task<LanguageConfig> LoadLanguageConfigAsync(string filePath)
        {
            try
            {
                var content = await Task.Run(() => File.ReadAllText(filePath, Encoding.UTF8));
                var doc = XDocument.Parse(content);
                var root = doc.Element("Language");

                return root == null
                    ? throw new InvalidOperationException("Invalid language file format.")
                    : new LanguageConfig
                    {
                        LanguageName = root.Element("Name")?.Value ?? "Unknown",
                        StopWords = LoadStopWords(root.Element("StopWords")),
                        SentimentWords = LoadSentimentWords(root.Element("Sentiment")),
                        Categories = LoadCategories(root.Element("Categories"))
                    };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error loading language configuration: {ex.Message}", ex);
            }
        }

        HashSet<string> LoadStopWords(XElement element)
        {
            if (element == null) return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            return element.Elements("Word")
                .Select(w => w.Value?.ToLowerInvariant())
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        Dictionary<string, int> LoadSentimentWords(XElement element)
        {
            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (element == null) return dict;

            var positive = element.Element("Positive");
            if (positive != null)
            {
                foreach (var word in positive.Elements("Word"))
                {
                    var value = word.Value?.ToLowerInvariant();
                    if (!string.IsNullOrWhiteSpace(value))
                        dict[value] = 1;
                }
            }

            var negative = element.Element("Negative");
            if (negative != null)
            {
                foreach (var word in negative.Elements("Word"))
                {
                    var value = word.Value?.ToLowerInvariant();
                    if (!string.IsNullOrWhiteSpace(value))
                        dict[value] = -1;
                }
            }

            return dict;
        }

        Dictionary<string, HashSet<string>> LoadCategories(XElement element)
        {
            var dict = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            if (element == null) return dict;

            foreach (var cat in element.Elements("Category"))
            {
                var name = cat.Attribute("name")?.Value;
                if (string.IsNullOrWhiteSpace(name)) continue;

                dict[name] = cat.Elements("Word")
                    .Select(w => w.Value?.ToLowerInvariant())
                    .Where(w => !string.IsNullOrWhiteSpace(w))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }

            return dict;
        }

        #endregion

        #region Analysis

        SemanticAnalysisResult PerformSemanticAnalysis(string content, string fileName, IProgress<int> progress)
        {
            var result = new SemanticAnalysisResult
            {
                FileName = Path.GetFileName(fileName),
                AnalysisDate = DateTime.Now,
                Language = CurrentLanguage.LanguageName,
                TotalCharacters = content.Length,
                TotalCharactersNoSpaces = content.Count(c => !char.IsWhiteSpace(c))
            };

            progress?.Report(50);

            var sentences = SplitIntoSentences(content);
            result.TotalSentences = sentences.Count;
            result.AverageSentenceLength = sentences.Count > 0
                ? sentences.Average(s => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length)
                : 0;

            var paragraphs = content.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            result.TotalParagraphs = paragraphs.Length;

            progress?.Report(60);

            var allWords = ExtractWords(content);
            result.TotalWords = allWords.Count;

            var wordFrequency = allWords
                .GroupBy(w => w.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

            result.UniqueWords = wordFrequency.Count;
            result.LexicalDiversity = result.TotalWords > 0
                ? (double)result.UniqueWords / result.TotalWords
                : 0;

            progress?.Report(70);

            AnalyzeTopWords(wordFrequency, result);

            AnalyzeEnhancedSentiment(allWords, result);

            AnalyzeThemes(wordFrequency, result);
            AnalyzeComplexity(allWords, result);
            AnalyzeReadability(sentences, allWords, result);
            AnalyzeNGrams(allWords, result);

            progress?.Report(90);

            AnalyzeWordDistribution(wordFrequency, result);
            AnalyzeVocabularyRichness(allWords, wordFrequency, result);

            return result;
        }

        List<string> SplitIntoSentences(string text)
        {
            var pattern = @"(?<=[.!?])\s+(?=[A-ZÀ-Ý])";
            var sentences = Regex.Split(text, pattern)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            return sentences.Count > 0 ? sentences : new List<string> { text };
        }

        List<string> ExtractWords(string text)
        {
            return Regex.Matches(text, @"\b[\w'-]+\b")
                .Cast<Match>()
                .Select(m => m.Value.ToLowerInvariant())
                .Where(w => w.Length >= MinWordLength && w.Length <= MaxWordLength)
                .Where(w => !Regex.IsMatch(w, @"^\d+$"))
                .ToList();
        }

        void AnalyzeTopWords(Dictionary<string, int> wordFrequency, SemanticAnalysisResult result)
        {
            result.TopWords = wordFrequency
                .Where(kvp => !CurrentLanguage.StopWords.Contains(kvp.Key) && kvp.Key.Length >= MinWordLength)
                .OrderByDescending(kvp => kvp.Value)
                .Take(TopWordsCount)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
        }

        void AnalyzeEnhancedSentiment(List<string> words, SemanticAnalysisResult result)
        {
            int positiveCount = 0;
            int negativeCount = 0;

            var positiveDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var negativeDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var word in words)
            {
                var lowerWord = word.ToLowerInvariant().Trim();

                if (lowerWord.EndsWith(".") || lowerWord.EndsWith("!") || lowerWord.EndsWith("?") ||
                    lowerWord.EndsWith(",") || lowerWord.EndsWith(";") || lowerWord.EndsWith(":"))
                {
                    lowerWord = lowerWord.Substring(0, lowerWord.Length - 1);
                }

                if (CurrentLanguage.SentimentWords.TryGetValue(lowerWord, out int sentimentValue))
                {
                    if (sentimentValue > 0)
                    {
                        positiveCount++;
                        if (positiveDict.ContainsKey(lowerWord))
                            positiveDict[lowerWord]++;
                        else
                            positiveDict[lowerWord] = 1;
                    }
                    else if (sentimentValue < 0)
                    {
                        negativeCount++;
                        if (negativeDict.ContainsKey(lowerWord))
                            negativeDict[lowerWord]++;
                        else
                            negativeDict[lowerWord] = 1;
                    }
                }
                else
                {
                    // Attempt at simple lemmatization (for "successful" -> "successful")
                    // Remove common feminine/plural endings in French
                    var baseForms = new List<string> { lowerWord };

                    // Remove the base endings
                    if (lowerWord.EndsWith("e") && lowerWord.Length > 1)
                        baseForms.Add(lowerWord.Substring(0, lowerWord.Length - 1));
                    if (lowerWord.EndsWith("s") && lowerWord.Length > 1)
                        baseForms.Add(lowerWord.Substring(0, lowerWord.Length - 1));
                    if (lowerWord.EndsWith("es") && lowerWord.Length > 2)
                        baseForms.Add(lowerWord.Substring(0, lowerWord.Length - 2));
                    if (lowerWord.EndsWith("e") && lowerWord.Length > 1)
                        baseForms.Add(lowerWord.Substring(0, lowerWord.Length - 1) + "é");

                    // Search among the basic forms
                    foreach (var baseForm in baseForms.Distinct())
                    {
                        if (CurrentLanguage.SentimentWords.TryGetValue(baseForm, out sentimentValue))
                        {
                            if (sentimentValue > 0)
                            {
                                positiveCount++;
                                if (positiveDict.ContainsKey(baseForm))
                                    positiveDict[baseForm]++;
                                else
                                    positiveDict[baseForm] = 1;
                                break;
                            }
                            else if (sentimentValue < 0)
                            {
                                negativeCount++;
                                if (negativeDict.ContainsKey(baseForm))
                                    negativeDict[baseForm]++;
                                else
                                    negativeDict[baseForm] = 1;
                                break;
                            }
                        }
                    }
                }
            }

            result.PositiveWordCount = positiveCount;
            result.NegativeWordCount = negativeCount;
            result.SentimentScore = positiveCount - negativeCount;

            int totalSentimentWords = positiveCount + negativeCount;
            result.SentimentRatio = totalSentimentWords > 0
                ? (double)positiveCount / totalSentimentWords
                : 0.5;

            result.TopPositiveWords = positiveDict
                .OrderByDescending(p => p.Value)
                .Take(10)
                .ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);

            result.TopNegativeWords = negativeDict
                .OrderByDescending(n => n.Value)
                .Take(10)
                .ToDictionary(n => n.Key, n => n.Value, StringComparer.OrdinalIgnoreCase);
        }

        void AnalyzeThemes(Dictionary<string, int> wordFrequency, SemanticAnalysisResult result)
        {
            result.ThematicCategories = new Dictionary<string, ThematicInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var category in CurrentLanguage.Categories)
            {
                var matchedWords = wordFrequency
                    .Where(kvp => category.Value.Contains(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

                if (matchedWords.Count > 0)
                {
                    result.ThematicCategories[category.Key] = new ThematicInfo
                    {
                        TotalOccurrences = matchedWords.Values.Sum(),
                        UniqueWords = matchedWords.Count,
                        TopWords = matchedWords
                            .OrderByDescending(kvp => kvp.Value)
                            .Take(TopThemeWordsCount)
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase)
                    };
                }
            }
        }

        void AnalyzeComplexity(List<string> words, SemanticAnalysisResult result)
        {
            result.AverageWordLength = words.Count > 0 ? words.Average(w => w.Length) : 0;

            var longWords = words.Where(w => w.Length > 6).ToList();
            result.LongWordsCount = longWords.Count;
            result.LongWordsPercentage = words.Count > 0
                ? (double)longWords.Count / words.Count * 100
                : 0;

            var complexWords = words.Where(w => w.Length > 12).ToList();
            result.ComplexWordsCount = complexWords.Count;

            result.TopLongWords = longWords
                .GroupBy(w => w.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(g => g.Count())
                .Take(15)
                .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);
        }

        void AnalyzeReadability(List<string> sentences, List<string> words, SemanticAnalysisResult result)
        {
            if (sentences.Count == 0 || words.Count == 0) return;

            double avgSyllablesPerWord = EstimateAverageSyllables(words);
            double avgWordsPerSentence = (double)words.Count / sentences.Count;

            result.FleschReadingEase = 206.835 - (1.015 * avgWordsPerSentence) - (84.6 * avgSyllablesPerWord);

            double complexWordRatio = words.Count > 0 ? (double)result.ComplexWordsCount / words.Count : 0;
            result.GunningFogIndex = 0.4 * (avgWordsPerSentence + (100 * complexWordRatio));
        }

        double EstimateAverageSyllables(List<string> words)
        {
            if (words.Count == 0) return 0;

            int totalSyllables = words.Sum(w => EstimateSyllablesInWord(w));
            return (double)totalSyllables / words.Count;
        }

        int EstimateSyllablesInWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return 0;

            word = word.ToLowerInvariant();
            int count = 0;
            bool previousWasVowel = false;
            var vowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u', 'y', 'à', 'â', 'ä', 'é', 'è', 'ê', 'ë', 'ï', 'î', 'ô', 'ù', 'û', 'ü' };

            foreach (char c in word)
            {
                bool isVowel = vowels.Contains(c);
                if (isVowel && !previousWasVowel)
                    count++;
                previousWasVowel = isVowel;
            }

            if (word.EndsWith("e") || word.EndsWith("es") || word.EndsWith("ent"))
                count = Math.Max(1, count - 1);

            return Math.Max(1, count);
        }

        void AnalyzeNGrams(List<string> words, SemanticAnalysisResult result)
        {
            result.TopBigrams = ExtractNGrams(words, 2, TopBigramsCount);
            result.TopTrigrams = ExtractNGrams(words, 3, TopTrigramsCount);
        }

        Dictionary<string, int> ExtractNGrams(List<string> words, int n, int topCount)
        {
            var ngrams = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i <= words.Count - n; i++)
            {
                var ngramWords = words.Skip(i).Take(n).Select(w => w.ToLowerInvariant()).ToArray();

                if (ngramWords.Any(w => CurrentLanguage.StopWords.Contains(w)))
                    continue;

                var ngram = string.Join(" ", ngramWords);

                if (ngrams.ContainsKey(ngram))
                    ngrams[ngram]++;
                else
                    ngrams[ngram] = 1;
            }

            return ngrams
                .Where(kvp => kvp.Value > 1)
                .OrderByDescending(kvp => kvp.Value)
                .Take(topCount)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
        }

        void AnalyzeWordDistribution(Dictionary<string, int> wordFrequency, SemanticAnalysisResult result)
        {
            var frequencies = wordFrequency.Values.OrderByDescending(f => f).ToList();

            if (frequencies.Count == 0) return;

            result.MostFrequentWordCount = frequencies.First();
            result.MedianWordFrequency = frequencies.Count % 2 == 0
                ? (frequencies[frequencies.Count / 2 - 1] + frequencies[frequencies.Count / 2]) / 2.0
                : frequencies[frequencies.Count / 2];
        }

        void AnalyzeVocabularyRichness(List<string> words, Dictionary<string, int> wordFrequency, SemanticAnalysisResult result)
        {
            result.HapaxLegomenaCount = wordFrequency.Count(kvp => kvp.Value == 1);
            result.HapaxPercentage = result.UniqueWords > 0
                ? (double)result.HapaxLegomenaCount / result.UniqueWords * 100
                : 0;

            result.TypeTokenRatio = result.TotalWords > 0
                ? (double)result.UniqueWords / result.TotalWords
                : 0;
        }

        #endregion

        #region Report Generation

        string GenerateHtmlReport(SemanticAnalysisResult result)
        {
            var html = new StringBuilder();

            AppendHtmlHeader(html, result);
            AppendHtmlGeneralStats(html, result);
            AppendHtmlChartsSection(html, result);
            AppendHtmlSentimentAnalysis(html, result);
            AppendHtmlTopWords(html, result);
            AppendHtmlThematicAnalysis(html, result);
            AppendHtmlComplexityAnalysis(html, result);
            AppendHtmlNGrams(html, result);
            AppendHtmlVocabularyRichness(html, result);
            AppendHtmlFooter(html, result);

            return html.ToString();
        }

        void AppendHtmlChartsSection(StringBuilder html, SemanticAnalysisResult result)
        {
            html.AppendLine("        <div class='section charts-section'>");
            html.AppendLine("            <h2>📈 Interactive Charts</h2>");
            html.AppendLine("            <div class='chart-grid'>");

            // Graphique 1
            html.AppendLine("                <div class='chart-item'>");
            html.AppendLine("                    <h3>📊 Word Distribution</h3>");
            html.AppendLine("                    <div class='chart-wrapper'>");
            html.AppendLine("                        <canvas id='chartDistribution'></canvas>");
            html.AppendLine("                    </div>");
            html.AppendLine("                    <div class='chart-stats'>");
            html.AppendLine($"                        <div><span class='stat-label'>Uniques</span><span class='stat-value'>{result.UniqueWords:N0}</span></div>");
            html.AppendLine($"                        <div><span class='stat-label'>Long</span><span class='stat-value'>{result.LongWordsCount:N0}</span></div>");
            html.AppendLine($"                        <div><span class='stat-label'>Hapax</span><span class='stat-value'>{result.HapaxLegomenaCount:N0}</span></div>");
            html.AppendLine("                    </div>");
            html.AppendLine("                </div>");

            // Graphique 2
            html.AppendLine("                <div class='chart-item'>");
            html.AppendLine("                    <h3>😊 Sentiment Analysis</h3>");
            html.AppendLine("                    <div class='chart-wrapper'>");
            html.AppendLine("                        <canvas id='chartSentiment'></canvas>");
            html.AppendLine("                    </div>");
            html.AppendLine("                    <div class='chart-stats'>");
            html.AppendLine($"                        <div><span class='stat-label'>Positive</span><span class='stat-value'>{result.PositiveWordCount:N0}</span></div>");
            html.AppendLine($"                        <div><span class='stat-label'>Negative</span><span class='stat-value'>{result.NegativeWordCount:N0}</span></div>");
            html.AppendLine($"                        <div><span class='stat-label'>Ratio</span><span class='stat-value'>{result.SentimentRatio:P1}</span></div>");
            html.AppendLine("                    </div>");
            html.AppendLine("                </div>");

            // Graphique 3
            html.AppendLine("                <div class='chart-item'>");
            html.AppendLine("                    <h3>🔤 Top 10 Words</h3>");
            html.AppendLine("                    <div class='chart-wrapper'>");
            html.AppendLine("                        <canvas id='chartTopWords'></canvas>");
            html.AppendLine("                    </div>");
            html.AppendLine("                    <div class='chart-stats'>");
            if (result.TopWords.Count > 0)
            {
                var topWord = result.TopWords.First();
                html.AppendLine($"                        <div><span class='stat-label'>#1</span><span class='stat-value'>{EscapeHtml(topWord.Key)}</span></div>");
                html.AppendLine($"                        <div><span class='stat-label'>Fréquence</span><span class='stat-value'>{topWord.Value:N0}</span></div>");
            }
            html.AppendLine($"                        <div><span class='stat-label'>Total</span><span class='stat-value'>{result.TotalWords:N0}</span></div>");
            html.AppendLine("                    </div>");
            html.AppendLine("                </div>");

            // Graphique 4
            html.AppendLine("                <div class='chart-item'>");
            html.AppendLine("                    <h3>🎯 Categories</h3>");
            html.AppendLine("                    <div class='chart-wrapper'>");
            html.AppendLine("                        <canvas id='chartCategories'></canvas>");
            html.AppendLine("                    </div>");
            html.AppendLine("                    <div class='chart-stats'>");
            html.AppendLine($"                        <div><span class='stat-label'>Detected</span><span class='stat-value'>{result.ThematicCategories?.Count ?? 0}</span></div>");
            if (result.ThematicCategories?.Count > 0)
            {
                var topCat = result.ThematicCategories.OrderByDescending(c => c.Value.TotalOccurrences).First();
                html.AppendLine($"                        <div><span class='stat-label'>Main</span><span class='stat-value'>{EscapeHtml(topCat.Key)}</span></div>");
            }
            html.AppendLine($"                        <div><span class='stat-label'>Words analyzed</span><span class='stat-value'>{result.TotalWords:N0}</span></div>");
            html.AppendLine("                    </div>");
            html.AppendLine("                </div>");

            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
        }

        void AppendHtmlHeader(StringBuilder html, SemanticAnalysisResult result)
        {
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='fr'>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine($"    <title>OOB Semantic Analysis - {EscapeHtml(result.FileName)}</title>");

            html.AppendLine("    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>");

            html.AppendLine("    <style>");

            html.AppendLine("        * { margin: 0; padding: 0; box-sizing: border-box; }");
            html.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f5f5f5; color: #333; line-height: 1.6; }");
            html.AppendLine("        .container { max-width: 1400px; margin: 0 auto; padding: 20px; }");
            html.AppendLine("        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; border-radius: 10px; margin-bottom: 30px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }");
            html.AppendLine("        .header h1 { font-size: 2.5em; margin-bottom: 10px; }");
            html.AppendLine("        .header .meta { opacity: 0.9; font-size: 0.95em; }");
            html.AppendLine("        .section { background: white; padding: 25px; margin-bottom: 20px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); }");
            html.AppendLine("        h2 { color: #2c3e50; margin-bottom: 20px; padding-bottom: 10px; border-bottom: 3px solid #667eea; font-size: 1.8em; }");
            html.AppendLine("        h3 { color: #34495e; margin: 25px 0 15px 0; font-size: 1.3em; }");

            html.AppendLine("        .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 15px; margin: 20px 0; }");
            html.AppendLine("        .stat-card { background: #f8f9fa; padding: 15px; border-radius: 8px; border-left: 4px solid #667eea; }");
            html.AppendLine("        .stat-label { font-weight: 600; color: #555; font-size: 0.9em; display: block; margin-bottom: 5px; }");
            html.AppendLine("        .stat-value { color: #667eea; font-size: 1.5em; font-weight: bold; }");
            html.AppendLine("        table { width: 100%; border-collapse: collapse; margin: 15px 0; }");
            html.AppendLine("        th, td { padding: 12px; text-align: left; border-bottom: 1px solid #e0e0e0; }");
            html.AppendLine("        th { background-color: #667eea; color: white; font-weight: 600; position: sticky; top: 0; }");
            html.AppendLine("        tr:hover { background-color: #f5f5f5; }");
            html.AppendLine("        .positive { color: #27ae60; font-weight: 600; }");
            html.AppendLine("        .negative { color: #e74c3c; font-weight: 600; }");
            html.AppendLine("        .badge { display: inline-block; padding: 4px 12px; border-radius: 20px; font-size: 0.85em; font-weight: 600; }");
            html.AppendLine("        .badge-success { background: #d4edda; color: #155724; }");
            html.AppendLine("        .badge-warning { background: #fff3cd; color: #856404; }");
            html.AppendLine("        .badge-danger { background: #f8d7da; color: #721c24; }");

            html.AppendLine("        .charts-section .chart-grid {");
            html.AppendLine("            display: grid;");
            html.AppendLine("            grid-template-columns: repeat(2, 1fr);");
            html.AppendLine("            gap: 25px;");
            html.AppendLine("            margin: 20px 0;");
            html.AppendLine("        }");
            html.AppendLine("        ");
            html.AppendLine("        .chart-item {");
            html.AppendLine("            background: #f8f9fa;");
            html.AppendLine("            border-radius: 10px;");
            html.AppendLine("            padding: 20px;");
            html.AppendLine("            border: 1px solid #e0e0e0;");
            html.AppendLine("            transition: transform 0.3s ease;");
            html.AppendLine("        }");
            html.AppendLine("        ");
            html.AppendLine("        .chart-item:hover {");
            html.AppendLine("            transform: translateY(-3px);");
            html.AppendLine("            box-shadow: 0 5px 15px rgba(0,0,0,0.1);");
            html.AppendLine("        }");
            html.AppendLine("        ");
            html.AppendLine("        .chart-wrapper {");
            html.AppendLine("            position: relative;");
            html.AppendLine("            height: 250px;");
            html.AppendLine("            margin: 15px 0;");
            html.AppendLine("        }");
            html.AppendLine("        ");
            html.AppendLine("        .chart-stats {");
            html.AppendLine("            display: grid;");
            html.AppendLine("            grid-template-columns: repeat(3, 1fr);");
            html.AppendLine("            gap: 10px;");
            html.AppendLine("            margin-top: 15px;");
            html.AppendLine("            padding-top: 15px;");
            html.AppendLine("            border-top: 1px solid #e0e0e0;");
            html.AppendLine("        }");
            html.AppendLine("        ");
            html.AppendLine("        .chart-stats .stat-label {");
            html.AppendLine("            font-size: 0.8em;");
            html.AppendLine("            color: #666;");
            html.AppendLine("            margin-bottom: 3px;");
            html.AppendLine("        }");
            html.AppendLine("        ");
            html.AppendLine("        .chart-stats .stat-value {");
            html.AppendLine("            font-size: 1.1em;");
            html.AppendLine("            color: #2c3e50;");
            html.AppendLine("        }");
            html.AppendLine("        ");
            html.AppendLine("        /* Media queries pour les graphiques seulement */");
            html.AppendLine("        @media (max-width: 1100px) {");
            html.AppendLine("            .charts-section .chart-grid {");
            html.AppendLine("                grid-template-columns: 1fr;");
            html.AppendLine("            }");
            html.AppendLine("            .chart-wrapper {");
            html.AppendLine("                height: 220px;");
            html.AppendLine("            }");
            html.AppendLine("        }");
            html.AppendLine("        ");
            html.AppendLine("        @media (max-width: 768px) {");
            html.AppendLine("            .chart-wrapper {");
            html.AppendLine("                height: 200px;");
            html.AppendLine("            }");
            html.AppendLine("            .chart-stats {");
            html.AppendLine("                grid-template-columns: 1fr;");
            html.AppendLine("                gap: 8px;");
            html.AppendLine("            }");
            html.AppendLine("        }");

            html.AppendLine("        @media print { ");
            html.AppendLine("            body { background: white; } ");
            html.AppendLine("            .section { box-shadow: none; } ");
            html.AppendLine("            .chart-item { break-inside: avoid; }");
            html.AppendLine("        }");

            html.AppendLine(".generated-info {");
            html.AppendLine("    font-style: italic;");
            html.AppendLine("    text-align: center;");
            html.AppendLine("    font-size: 0.7em;");
            html.AppendLine("    color: #808080;");
            html.AppendLine("    font-weight: bold;");
            html.AppendLine("    font-family: Arial, sans-serif;");
            html.AppendLine("    padding: 10px 0;");
            html.AppendLine("    border-top: 1px solid #e0e0e0;");
            html.AppendLine("    margin-top: 20px;");
            html.AppendLine("}");

            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class='container'>");
            html.AppendLine("        <div class='header'>");
            html.AppendLine("            <h1>📊 OOB Complete Semantic Analysis</h1>");
            html.AppendLine($"            <div class='meta'>📄 {EscapeHtml(result.FileName)} | 🕒 {result.AnalysisDate:dd/MM/yyyy HH:mm:ss} | 🌐 {EscapeHtml(result.Language)}</div>");
            html.AppendLine("        </div>");
        }

        void AppendHtmlGeneralStats(StringBuilder html, SemanticAnalysisResult result)
        {
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <h2>📈 General Statistics</h2>");
            html.AppendLine("            <div class='stats-grid'>");

            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Total characters</span><span class='stat-value'>{result.TotalCharacters:N0}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Characters (without spaces)</span><span class='stat-value'>{result.TotalCharactersNoSpaces:N0}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Total words</span><span class='stat-value'>{result.TotalWords:N0}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Single words</span><span class='stat-value'>{result.UniqueWords:N0}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Sentences</span><span class='stat-value'>{result.TotalSentences:N0}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Paragraphs</span><span class='stat-value'>{result.TotalParagraphs:N0}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Lexical diversity</span><span class='stat-value'>{result.LexicalDiversity:P2}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Average word length</span><span class='stat-value'>{result.AverageWordLength:F2}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Average sentence length</span><span class='stat-value'>{result.AverageSentenceLength:F2}</span></div>");

            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
        }

        void AppendHtmlSentimentAnalysis(StringBuilder html, SemanticAnalysisResult result)
        {
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <h2>😊 Sentiment Analysis</h2>");
            html.AppendLine("            <div class='stats-grid'>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Sentiment score</span><span class='stat-value'>{result.SentimentScore}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Positive ratio</span><span class='stat-value'>{result.SentimentRatio:P2}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label positive'>Positive words</span><span class='stat-value positive'>{result.PositiveWordCount}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label negative'>Negative words</span><span class='stat-value negative'>{result.NegativeWordCount}</span></div>");
            html.AppendLine("            </div>");

            if (result.TopPositiveWords?.Count > 0)
            {
                html.AppendLine("            <h3>Top Positive Words</h3>");
                html.AppendLine("            <table><tr><th>Word</th><th>Frequency</th></tr>");
                foreach (var word in result.TopPositiveWords)
                    html.AppendLine($"                <tr><td class='positive'>{EscapeHtml(word.Key)}</td><td>{word.Value}</td></tr>");
                html.AppendLine("            </table>");
            }

            if (result.TopNegativeWords?.Count > 0)
            {
                html.AppendLine("            <h3>Top Negative Words</h3>");
                html.AppendLine("            <table><tr><th>Word</th><th>Frequency</th></tr>");
                foreach (var word in result.TopNegativeWords)
                    html.AppendLine($"                <tr><td class='negative'>{EscapeHtml(word.Key)}</td><td>{word.Value}</td></tr>");
                html.AppendLine("            </table>");
            }

            html.AppendLine("        </div>");
        }

        void AppendHtmlTopWords(StringBuilder html, SemanticAnalysisResult result)
        {
            if (result.TopWords?.Count > 0)
            {
                html.AppendLine("        <div class='section'>");
                html.AppendLine("            <h2>🔤 Most Frequent Words</h2>");
                html.AppendLine("            <table><tr><th>Word</th><th>Frequency</th><th>Percentage</th></tr>");
                foreach (var word in result.TopWords)
                {
                    double percentage = (double)word.Value / result.TotalWords * 100;
                    html.AppendLine($"                <tr><td>{EscapeHtml(word.Key)}</td><td>{word.Value}</td><td>{percentage:F2}%</td></tr>");
                }
                html.AppendLine("            </table>");
                html.AppendLine("        </div>");
            }
        }

        void AppendHtmlThematicAnalysis(StringBuilder html, SemanticAnalysisResult result)
        {
            if (result.ThematicCategories?.Count > 0)
            {
                html.AppendLine("        <div class='section'>");
                html.AppendLine("            <h2>🎯 Thematic Analysis</h2>");
                foreach (var theme in result.ThematicCategories.OrderByDescending(kvp => kvp.Value.TotalOccurrences))
                {
                    html.AppendLine($"            <h3>{EscapeHtml(theme.Key)}</h3>");
                    html.AppendLine("            <div class='stats-grid'>");
                    html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Total occurrences</span><span class='stat-value'>{theme.Value.TotalOccurrences}</span></div>");
                    html.AppendLine($"                <div class='stat-card'><span class='stat-label'>single words</span><span class='stat-value'>{theme.Value.UniqueWords}</span></div>");
                    html.AppendLine("            </div>");
                    html.AppendLine("            <table><tr><th>Word</th><th>Frequency</th></tr>");
                    foreach (var word in theme.Value.TopWords)
                        html.AppendLine($"                <tr><td>{EscapeHtml(word.Key)}</td><td>{word.Value}</td></tr>");
                    html.AppendLine("            </table>");
                }
                html.AppendLine("        </div>");
            }
        }

        void AppendHtmlComplexityAnalysis(StringBuilder html, SemanticAnalysisResult result)
        {
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <h2>📚 Complexity and Readability</h2>");
            html.AppendLine("            <div class='stats-grid'>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Long words (>6 letters)</span><span class='stat-value'>{result.LongWordsCount} ({result.LongWordsPercentage:F2}%)</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Complex words (>12 letters)</span><span class='stat-value'>{result.ComplexWordsCount}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Hint Flesch</span><span class='stat-value'>{result.FleschReadingEase:F2}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Hint Gunning Fog</span><span class='stat-value'>{result.GunningFogIndex:F2}</span></div>");
            html.AppendLine("            </div>");

            if (result.TopLongWords?.Count > 0)
            {
                html.AppendLine("            <h3>Frequent Long Words</h3>");
                html.AppendLine("            <table><tr><th>Word</th><th>Length</th><th>Frequency</th></tr>");
                foreach (var word in result.TopLongWords)
                    html.AppendLine($"                <tr><td>{EscapeHtml(word.Key)}</td><td>{word.Key.Length}</td><td>{word.Value}</td></tr>");
                html.AppendLine("            </table>");
            }
            html.AppendLine("        </div>");
        }

        void AppendHtmlNGrams(StringBuilder html, SemanticAnalysisResult result)
        {
            if (result.TopBigrams?.Count > 0)
            {
                html.AppendLine("        <div class='section'>");
                html.AppendLine("            <h2>🔗 Bigrams (Word Pairs)</h2>");
                html.AppendLine("            <table><tr><th>Bigrams</th><th>Frequency</th></tr>");
                foreach (var bigram in result.TopBigrams)
                    html.AppendLine($"                <tr><td>{EscapeHtml(bigram.Key)}</td><td>{bigram.Value}</td></tr>");
                html.AppendLine("            </table>");
                html.AppendLine("        </div>");
            }

            if (result.TopTrigrams?.Count > 0)
            {
                html.AppendLine("        <div class='section'>");
                html.AppendLine("            <h2>🔗 Trigrams (Word Triplets)</h2>");
                html.AppendLine("            <table><tr><th>Trigramme</th><th>Frequency</th></tr>");
                foreach (var trigram in result.TopTrigrams)
                    html.AppendLine($"                <tr><td>{EscapeHtml(trigram.Key)}</td><td>{trigram.Value}</td></tr>");
                html.AppendLine("            </table>");
                html.AppendLine("        </div>");
            }
        }

        void AppendHtmlVocabularyRichness(StringBuilder html, SemanticAnalysisResult result)
        {
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <h2>💎 Richness of Vocabulary</h2>");
            html.AppendLine("            <div class='stats-grid'>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Hapax Legomena</span><span class='stat-value'>{result.HapaxLegomenaCount} ({result.HapaxPercentage:F2}%)</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Type-Token Ratio</span><span class='stat-value'>{result.TypeTokenRatio:F4}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Median frequency</span><span class='stat-value'>{result.MedianWordFrequency:F2}</span></div>");
            html.AppendLine($"                <div class='stat-card'><span class='stat-label'>Most frequent word</span><span class='stat-value'>{result.MostFrequentWordCount} fois</span></div>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
        }

        void AppendHtmlFooter(StringBuilder html, SemanticAnalysisResult result)
        {
            html.AppendLine("    </div>");

            html.AppendLine("    <script>");
            html.AppendLine("        // Data");
            html.AppendLine($"        const totalWords = {result.TotalWords};");
            html.AppendLine($"        const uniqueWords = {result.UniqueWords};");
            html.AppendLine($"        const longWords = {result.LongWordsCount};");
            html.AppendLine($"        const complexWords = {result.ComplexWordsCount};");
            html.AppendLine($"        const hapaxWords = {result.HapaxLegomenaCount};");
            html.AppendLine($"        const positiveWords = {result.PositiveWordCount};");
            html.AppendLine($"        const negativeWords = {result.NegativeWordCount};");
            html.AppendLine($"        const neutralWords = Math.max(0, totalWords - positiveWords - negativeWords);");
            html.AppendLine("");

            // Top Words
            html.AppendLine("        const topWordsData = {");
            if (result.TopWords.Count > 0)
            {
                var top10 = result.TopWords.Take(10).ToList();
                html.AppendLine("            labels: [" + string.Join(", ", top10.Select(w => $"'{EscapeJsString(w.Key)}'")) + "],");
                html.AppendLine("            values: [" + string.Join(", ", top10.Select(w => w.Value)) + "]");
            }
            else
            {
                html.AppendLine("            labels: [], values: []");
            }
            html.AppendLine("        };");
            html.AppendLine("");

            // Categories
            html.AppendLine("        const categoriesData = {");
            if (result.ThematicCategories?.Count > 0)
            {
                var topCats = result.ThematicCategories
                    .Where(c => c.Value.TotalOccurrences > 0)
                    .OrderByDescending(c => c.Value.TotalOccurrences)
                    .Take(8)
                    .ToList();

                if (topCats.Count > 0)
                {
                    html.AppendLine("            labels: [" + string.Join(", ", topCats.Select(c => $"'{EscapeJsString(c.Key)}'")) + "],");
                    html.AppendLine("            values: [" + string.Join(", ", topCats.Select(c => c.Value.TotalOccurrences)) + "]");
                }
                else
                {
                    html.AppendLine("            labels: [], values: []");
                }
            }
            else
            {
                html.AppendLine("            labels: [], values: []");
            }
            html.AppendLine("        };");
            html.AppendLine("");

            html.AppendLine("        // Initialization");
            html.AppendLine("        document.addEventListener('DOMContentLoaded', function() {");
            html.AppendLine("            const colors = ['#667eea', '#764ba2', '#27ae60', '#f39c12', '#e74c3c', '#9b59b6'];");
            html.AppendLine("");

            // 1. Distribution Chart
            html.AppendLine("            // Graphique Répartition");
            html.AppendLine("            if (document.getElementById('chartDistribution')) {");
            html.AppendLine("                new Chart(document.getElementById('chartDistribution'), {");
            html.AppendLine("                    type: 'doughnut',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: ['single words', 'Long words', 'Complex words', 'Hapax legomena'],");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            data: [uniqueWords, longWords, complexWords, hapaxWords],");
            html.AppendLine("                            backgroundColor: colors,");
            html.AppendLine("                            borderWidth: 1");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        maintainAspectRatio: false,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            legend: { position: 'bottom', labels: { padding: 15 } }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");
            html.AppendLine("            }");
            html.AppendLine("");

            // 2. Sentiment Chart
            html.AppendLine("            // Sentiment Chart");
            html.AppendLine("            if (document.getElementById('chartSentiment')) {");
            html.AppendLine("                new Chart(document.getElementById('chartSentiment'), {");
            html.AppendLine("                    type: 'pie',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: ['Positive', 'Negative', 'Neutral'],");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            data: [positiveWords, negativeWords, neutralWords],");
            html.AppendLine("                            backgroundColor: ['#27ae60', '#e74c3c', '#f39c12'],");
            html.AppendLine("                            borderWidth: 1");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        maintainAspectRatio: false,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            legend: { position: 'bottom' }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");
            html.AppendLine("            }");
            html.AppendLine("");

            // 3. Top Words Chart
            html.AppendLine("            // Top Words Chart");
            html.AppendLine("            if (document.getElementById('chartTopWords') && topWordsData.labels.length > 0) {");
            html.AppendLine("                new Chart(document.getElementById('chartTopWords'), {");
            html.AppendLine("                    type: 'bar',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: topWordsData.labels,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Frequency',");
            html.AppendLine("                            data: topWordsData.values,");
            html.AppendLine("                            backgroundColor: '#667eea',");
            html.AppendLine("                            borderColor: '#764ba2',");
            html.AppendLine("                            borderWidth: 1");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        maintainAspectRatio: false,");
            html.AppendLine("                        indexAxis: 'y',");
            html.AppendLine("                        plugins: { legend: { display: false } },");
            html.AppendLine("                        scales: {");
            html.AppendLine("                            x: { beginAtZero: true, title: { display: true, text: 'Occurrences' } },");
            html.AppendLine("                            y: { ticks: { autoSkip: false } }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");
            html.AppendLine("            }");
            html.AppendLine("");

            // 4. Chart Categories
            html.AppendLine("            // Graphique Catégories");
            html.AppendLine("            if (document.getElementById('chartCategories') && categoriesData.labels.length > 0) {");
            html.AppendLine("                new Chart(document.getElementById('chartCategories'), {");
            html.AppendLine("                    type: 'radar',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: categoriesData.labels,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Occurrences',");
            html.AppendLine("                            data: categoriesData.values,");
            html.AppendLine("                            backgroundColor: 'rgba(102, 126, 234, 0.2)',");
            html.AppendLine("                            borderColor: '#667eea',");
            html.AppendLine("                            borderWidth: 2,");
            html.AppendLine("                            pointBackgroundColor: '#667eea'");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        maintainAspectRatio: false,");
            html.AppendLine("                        scales: {");
            html.AppendLine("                            r: { beginAtZero: true, ticks: { display: false } }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");
            html.AppendLine("            }");
            html.AppendLine("        });");
            html.AppendLine("    </script>");

            html.AppendLine("<div class=\"generated-info\">");
            html.AppendLine($"Analysis automatically generated on {result.AnalysisDate:dd/MM/yyyy HH:mm:ss} - Ostium OSINT Browser");
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
        }

        string GenerateMarkdownReport(SemanticAnalysisResult result)
        {
            var md = new StringBuilder();

            md.AppendLine("# 📊 OOB Semantic Analysis");
            md.AppendLine();
            md.AppendLine($"**File:** {result.FileName}  ");
            md.AppendLine($"**Date of analysis:** {result.AnalysisDate:dd/MM/yyyy HH:mm:ss}  ");
            md.AppendLine($"**Language:** {result.Language}");
            md.AppendLine();
            md.AppendLine("---");
            md.AppendLine();

            AppendMarkdownGeneralStats(md, result);
            AppendMarkdownSentiment(md, result);
            AppendMarkdownTopWords(md, result);
            AppendMarkdownThematic(md, result);
            AppendMarkdownComplexity(md, result);
            AppendMarkdownNGrams(md, result);
            AppendMarkdownVocabularyRichness(md, result);

            md.AppendLine("---");
            md.AppendLine();
            md.AppendLine($"*Analysis automatically generated on {DateTime.Now:dd/MM/yyyy à HH:mm:ss}* Ostium Osint Browser");

            return md.ToString();
        }

        void AppendMarkdownGeneralStats(StringBuilder md, SemanticAnalysisResult result)
        {
            md.AppendLine("## 📈 General Statistics");
            md.AppendLine();
            md.AppendLine($"- **Total characters:** {result.TotalCharacters:N0}");
            md.AppendLine($"- **Characters (without spaces):** {result.TotalCharactersNoSpaces:N0}");
            md.AppendLine($"- **Total words:** {result.TotalWords:N0}");
            md.AppendLine($"- **Single words:** {result.UniqueWords:N0}");
            md.AppendLine($"- **Sentences:** {result.TotalSentences:N0}");
            md.AppendLine($"- **Paragraphs:** {result.TotalParagraphs:N0}");
            md.AppendLine($"- **Lexical diversity:** {result.LexicalDiversity:P2}");
            md.AppendLine($"- **Average word length:** {result.AverageWordLength:F2} letters");
            md.AppendLine($"- **Average sentence length:** {result.AverageSentenceLength:F2} Words");
            md.AppendLine();
        }

        void AppendMarkdownSentiment(StringBuilder md, SemanticAnalysisResult result)
        {
            md.AppendLine("## 😊 Sentiment Analysis");
            md.AppendLine();
            md.AppendLine($"- **Sentiment score:** {result.SentimentScore}");
            md.AppendLine($"- **Positive ratio:** {result.SentimentRatio:P2}");
            md.AppendLine($"- **Positive words:** {result.PositiveWordCount}");
            md.AppendLine($"- **Negative words:** {result.NegativeWordCount}");
            md.AppendLine();

            if (result.TopPositiveWords?.Count > 0)
            {
                md.AppendLine("### Top Positive Words");
                md.AppendLine();
                md.AppendLine("| Word | Frequency |");
                md.AppendLine("|------|-----------|");
                foreach (var word in result.TopPositiveWords)
                    md.AppendLine($"| {word.Key} | {word.Value} |");
                md.AppendLine();
            }

            if (result.TopNegativeWords?.Count > 0)
            {
                md.AppendLine("### Top Negative Words");
                md.AppendLine();
                md.AppendLine("| Word | Frequency |");
                md.AppendLine("|------|-----------|");
                foreach (var word in result.TopNegativeWords)
                    md.AppendLine($"| {word.Key} | {word.Value} |");
                md.AppendLine();
            }
        }

        void AppendMarkdownTopWords(StringBuilder md, SemanticAnalysisResult result)
        {
            if (result.TopWords?.Count > 0)
            {
                md.AppendLine("## 🔤 Most Frequent Words");
                md.AppendLine();
                md.AppendLine("| Word | Frequency | Percentage |");
                md.AppendLine("|------|-----------|------------|");
                foreach (var word in result.TopWords)
                {
                    double percentage = (double)word.Value / result.TotalWords * 100;
                    md.AppendLine($"| {word.Key} | {word.Value} | {percentage:F2}% |");
                }
                md.AppendLine();
            }
        }

        void AppendMarkdownThematic(StringBuilder md, SemanticAnalysisResult result)
        {
            if (result.ThematicCategories?.Count > 0)
            {
                md.AppendLine("## 🎯 Thematic Analysis");
                md.AppendLine();
                foreach (var theme in result.ThematicCategories.OrderByDescending(kvp => kvp.Value.TotalOccurrences))
                {
                    md.AppendLine($"### {theme.Key}");
                    md.AppendLine();
                    md.AppendLine($"- **Total occurrences:** {theme.Value.TotalOccurrences}");
                    md.AppendLine($"- **single words:** {theme.Value.UniqueWords}");
                    md.AppendLine();
                    md.AppendLine("| Word | Frequency |");
                    md.AppendLine("|------|-----------|");
                    foreach (var word in theme.Value.TopWords)
                        md.AppendLine($"| {word.Key} | {word.Value} |");
                    md.AppendLine();
                }
            }
        }

        void AppendMarkdownComplexity(StringBuilder md, SemanticAnalysisResult result)
        {
            md.AppendLine("## 📚 Complexity and Readability");
            md.AppendLine();
            md.AppendLine($"- **Long words (>6 letters):** {result.LongWordsCount} ({result.LongWordsPercentage:F2}%)");
            md.AppendLine($"- **Complex words (>12 letters):** {result.ComplexWordsCount}");
            md.AppendLine($"- **Flesch Readability Index:** {result.FleschReadingEase:F2}");
            md.AppendLine($"- **Indice Gunning Fog:** {result.GunningFogIndex:F2}");
            md.AppendLine();

            if (result.TopLongWords?.Count > 0)
            {
                md.AppendLine("### Frequent Long Words");
                md.AppendLine();
                md.AppendLine("| Word | Length | Frequency |");
                md.AppendLine("|------|--------|-----------|");
                foreach (var word in result.TopLongWords)
                    md.AppendLine($"| {word.Key} | {word.Key.Length} | {word.Value} |");
                md.AppendLine();
            }
        }

        void AppendMarkdownNGrams(StringBuilder md, SemanticAnalysisResult result)
        {
            if (result.TopBigrams?.Count > 0)
            {
                md.AppendLine("## 🔗 Bigrams (Word Pairs)");
                md.AppendLine();
                md.AppendLine("| Bigrams | Frequency |");
                md.AppendLine("|---------|-----------|");
                foreach (var bigram in result.TopBigrams)
                    md.AppendLine($"| {bigram.Key} | {bigram.Value} |");
                md.AppendLine();
            }

            if (result.TopTrigrams?.Count > 0)
            {
                md.AppendLine("## 🔗 Trigrams (Word Triplets)");
                md.AppendLine();
                md.AppendLine("| Trigrams | Frequency |");
                md.AppendLine("|----------|-----------|");
                foreach (var trigram in result.TopTrigrams)
                    md.AppendLine($"| {trigram.Key} | {trigram.Value} |");
                md.AppendLine();
            }
        }

        void AppendMarkdownVocabularyRichness(StringBuilder md, SemanticAnalysisResult result)
        {
            md.AppendLine("## 💎 Richness of Vocabulary");
            md.AppendLine();
            md.AppendLine($"- **Hapax Legomena:** {result.HapaxLegomenaCount} ({result.HapaxPercentage:F2}%)");
            md.AppendLine($"- **Type-Token Ratio:** {result.TypeTokenRatio:F4}");
            md.AppendLine($"- **Word frequency median:** {result.MedianWordFrequency:F2}");
            md.AppendLine($"- **Most frequent word (occurrences):** {result.MostFrequentWordCount}");
            md.AppendLine();
        }

        string EscapeHtml(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return System.Net.WebUtility.HtmlEncode(text);
        }

        string EscapeJsString(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            return text
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }

        #endregion
    }
}