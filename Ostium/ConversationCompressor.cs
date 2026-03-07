using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ConversationCompressor
{
    public sealed class SemanticTextCompressor : IDisposable
    {
        #region Configuration
        const int MIN_SENTENCE_LENGTH = 20;
        const int MAX_COMPRESSION_RATIO = 70;
        const double SEMANTIC_THRESHOLD = 0.65;
        readonly HashSet<string> _stopWords;
        readonly SHA256 _hashAlgorithm;
        bool _disposed;
        #endregion

        #region Constructor

        public SemanticTextCompressor()
        {
            _stopWords = LoadStopWords();
            _hashAlgorithm = SHA256.Create();
        }

        static HashSet<string> LoadStopWords()
        {
            string filePath = Application.StartupPath + @"\OOBai\french_words.txt";

            try
            {
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    return new HashSet<string>(lines, StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    MessageBox.Show($"The file {filePath} not found.", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading stop words : {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        #endregion

        #region Public methods

        public string CompressConversation(string conversation, string contextKey = null)
        {
            ValidateInput(conversation);

            try
            {
                // Initial cleaning
                var cleanedText = PreprocessText(conversation);

                // Segmentation into semantic units
                var semanticUnits = ExtractSemanticUnits(cleanedText);

                // Semantic filtering
                var filteredUnits = FilterSemanticUnits(semanticUnits, contextKey);

                // Optimized Reconstruction
                var compressedText = ReconstructText(filteredUnits);

                return ValidateCompression(conversation, compressedText);
            }
            catch (Exception ex)
            {
                LogError(ex, conversation);
                return conversation;
            }
        }

        public string GenerateContextSignature(string conversation)
        {
            if (string.IsNullOrWhiteSpace(conversation))
                return string.Empty;

            var normalized = conversation.ToLowerInvariant();
            var words = Regex.Split(normalized, @"\W+")
                .Where(w => w.Length > 3 && !_stopWords.Contains(w))
                .OrderBy(w => w)
                .Take(10); // Take the 10 most significant words

            var context = string.Join("|", words);
            var hashBytes = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(context));

            return BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 16);
        }

        #endregion

        #region Compression methods

        string PreprocessText(string text)
        {
            return Regex.Replace(text.Trim(), @"\s+", " ");
        }

        List<SemanticUnit> ExtractSemanticUnits(string text)
        {
            var units = new List<SemanticUnit>();

            // Segmentation by sentences
            var sentences = Regex.Split(text, @"(?<=[.!?])\s+");

            foreach (var sentence in sentences)
            {
                if (sentence.Length < MIN_SENTENCE_LENGTH)
                    continue;

                var words = TokenizeSentence(sentence);
                var keywords = ExtractKeywords(words);
                var entities = ExtractEntities(sentence);

                units.Add(new SemanticUnit
                {
                    OriginalText = sentence,
                    Keywords = keywords,
                    Entities = entities,
                    ImportanceScore = CalculateImportanceScore(words, keywords, entities),
                    Position = units.Count
                });
            }

            return units;
        }

        List<SemanticUnit> FilterSemanticUnits(List<SemanticUnit> units, string contextKey)
        {
            if (units.Count <= 1)
                return units;

            // Sort by importance
            var sortedUnits = units.OrderByDescending(u => u.ImportanceScore).ToList();

            // Keeps the most important units (with threshold)
            var threshold = sortedUnits.Average(u => u.ImportanceScore) * SEMANTIC_THRESHOLD;
            var importantUnits = sortedUnits.Where(u => u.ImportanceScore >= threshold).ToList();

            // Ensures narrative continuity
            importantUnits = EnsureNarrativeContinuity(importantUnits, units);

            // Limits the compression ratio
            return ApplyCompressionLimit(importantUnits, units.Count);
        }

        string ReconstructText(List<SemanticUnit> units)
        {
            // Reorganize in the original order
            var orderedUnits = units.OrderBy(u => u.Position).ToList();

            var builder = new StringBuilder();
            for (int i = 0; i < orderedUnits.Count; i++)
            {
                builder.Append(orderedUnits[i].OriginalText);

                if (i < orderedUnits.Count - 1)
                {
                    builder.Append(' ');
                }
            }

            return builder.ToString();
        }

        #endregion

        #region Semantic extraction algorithms

        List<string> TokenizeSentence(string sentence)
        {
            return Regex.Matches(sentence, @"\b[\w']+\b")
                .Cast<Match>()
                .Select(m => m.Value.ToLowerInvariant())
                .ToList();
        }

        List<string> ExtractKeywords(List<string> words)
        {
            return words
                .Where(w => w.Length > 3 && !_stopWords.Contains(w))
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .Take(5) // Maximum 5 keywords per unit
                .Select(g => g.Key)
                .ToList();
        }

        List<string> ExtractEntities(string sentence)
        {
            // Simple detection of entities (proper names, etc.)
            var entities = new List<string>();

            // Detection of significant capital letters
            var words = sentence.Split(' ');
            foreach (var word in words)
            {
                if (word.Length > 2 && char.IsUpper(word[0]) && word.Skip(1).All(c => char.IsLower(c)))
                {
                    entities.Add(word);
                }
            }

            return entities.Distinct().ToList();
        }

        float CalculateImportanceScore(List<string> words, List<string> keywords, List<string> entities)
        {
            float score = 0f;

            // Score based on keywords
            score += keywords.Count * 0.3f;

            // Score based on entities
            score += entities.Count * 0.4f;

            // Score based on length (neither too short nor too long)
            var wordCount = words.Count;
            if (wordCount > 5 && wordCount < 30)
                score += 0.2f;

            // Penalty for repetitions
            var uniqueWords = new HashSet<string>(words);
            var uniquenessRatio = (float)uniqueWords.Count / wordCount;
            score *= uniquenessRatio;

            return score;
        }

        #endregion

        #region Optimization methods

        List<SemanticUnit> EnsureNarrativeContinuity(List<SemanticUnit> importantUnits, List<SemanticUnit> allUnits)
        {
            var result = new List<SemanticUnit>(importantUnits);

            // Check for narrative gaps
            for (int i = 0; i < allUnits.Count - 1; i++)
            {
                var current = allUnits[i];
                var next = allUnits[i + 1];

                bool currentKept = result.Contains(current);
                bool nextKept = result.Contains(next);

                // If there is too significant a narrative leap
                if (!currentKept && nextKept && i > 0)
                {
                    // Add the previous unit for continuity
                    result.Add(allUnits[i - 1]);
                }
            }

            return result.Distinct().ToList();
        }

        List<SemanticUnit> ApplyCompressionLimit(List<SemanticUnit> units, int originalCount)
        {
            var maxUnits = Math.Max(1, (int)(originalCount * (MAX_COMPRESSION_RATIO / 100.0)));

            if (units.Count <= maxUnits)
                return units;

            return units
                .OrderByDescending(u => u.ImportanceScore)
                .Take(maxUnits)
                .OrderBy(u => u.Position)
                .ToList();
        }

        string ValidateCompression(string original, string compressed)
        {
            if (string.IsNullOrWhiteSpace(compressed))
                return original;

            var originalLength = original.Length;
            var compressedLength = compressed.Length;

            if (compressedLength >= originalLength)
                return original; // No gain, we're keeping the original

            if ((float)compressedLength / originalLength * 100 < 100 - MAX_COMPRESSION_RATIO)
            {
                return ExpandCompression(original, compressed);
            }

            return compressed;
        }

        string ExpandCompression(string original, string overCompressed)
        {
            var originalUnits = ExtractSemanticUnits(original);
            var compressedUnits = ExtractSemanticUnits(overCompressed);

            var missingUnits = originalUnits
                .Where(ou => !compressedUnits.Any(cu => cu.Keywords.Intersect(ou.Keywords).Any()))
                .OrderByDescending(u => u.ImportanceScore)
                .Take(2); // Adds a maximum of 2 units

            var expandedUnits = compressedUnits.Concat(missingUnits)
                .OrderBy(u => u.Position)
                .ToList();

            return ReconstructText(expandedUnits);
        }

        #endregion

        void ValidateInput(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("The text cannot be empty");

            if (text.Length > 1000000) // 1MB max
                throw new ArgumentException("The text is too large");

            if (text.Contains("\0") || text.Contains("\u0000"))
                throw new ArgumentException("Text containing null characters");
        }

        void LogError(Exception ex, string conversation)
        {
            System.Diagnostics.Debug.WriteLine($"Compression error: {ex.Message}");
            if (conversation != null && conversation.Length > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Conversation: {conversation.Substring(0, Math.Min(100, conversation.Length))}...");
            }
        }

        #region Internal structures

        class SemanticUnit
        {
            public string OriginalText { get; set; }
            public List<string> Keywords { get; set; }
            public List<string> Entities { get; set; }
            public float ImportanceScore { get; set; }
            public int Position { get; set; }

            public override bool Equals(object obj)
            {
                return obj is SemanticUnit other &&
                       OriginalText == other.OriginalText;
            }

            public override int GetHashCode()
            {
                return OriginalText != null ? OriginalText.GetHashCode() : 0;
            }
        }

        #endregion

        public void Dispose()
        {
            if (!_disposed)
            {
                _hashAlgorithm?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }

    public static class CompressionManager
    {
        private static readonly object _lock = new object();
        private static SemanticTextCompressor _compressor;

        public static string CompressConversation(string conversation)
        {
            lock (_lock)
            {
                if (_compressor == null)
                {
                    _compressor = new SemanticTextCompressor();
                }
                return _compressor.CompressConversation(conversation);
            }
        }

        public static string GenerateContextSignature(string conversation)
        {
            lock (_lock)
            {
                if (_compressor == null)
                {
                    _compressor = new SemanticTextCompressor();
                }
                return _compressor.GenerateContextSignature(conversation);
            }
        }

        public static void Cleanup()
        {
            lock (_lock)
            {
                _compressor?.Dispose();
                _compressor = null;
            }
        }
    }
}