using System;
using System.Collections.Generic;

namespace SemanticAnalyzer
{
    public class LanguageConfig
    {
        public string LanguageName { get; set; }
        public HashSet<string> StopWords { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> SentimentWords { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, HashSet<string>> Categories { get; set; } = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
    }

    public class SemanticAnalysisResult
    {
        // Metadata
        public string FileName { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string Language { get; set; }

        // Basic statistics
        public int TotalCharacters { get; set; }
        public int TotalCharactersNoSpaces { get; set; }
        public int TotalWords { get; set; }
        public int UniqueWords { get; set; }
        public int TotalSentences { get; set; }
        public int TotalParagraphs { get; set; }
        public double LexicalDiversity { get; set; }
        public double AverageWordLength { get; set; }
        public double AverageSentenceLength { get; set; }

        // Word frequency
        public Dictionary<string, int> TopWords { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Sentiment analysis
        public int PositiveWordCount { get; set; }
        public int NegativeWordCount { get; set; }
        public int SentimentScore { get; set; }
        public double SentimentRatio { get; set; }
        public Dictionary<string, int> TopPositiveWords { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> TopNegativeWords { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Thematic analysis
        public Dictionary<string, ThematicInfo> ThematicCategories { get; set; } = new Dictionary<string, ThematicInfo>(StringComparer.OrdinalIgnoreCase);

        // Complexity
        public int LongWordsCount { get; set; }
        public double LongWordsPercentage { get; set; }
        public int ComplexWordsCount { get; set; }
        public Dictionary<string, int> TopLongWords { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Readability
        public double FleschReadingEase { get; set; }
        public double GunningFogIndex { get; set; }

        // N-grammes
        public Dictionary<string, int> TopBigrams { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> TopTrigrams { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Richness of vocabulary
        public int HapaxLegomenaCount { get; set; }
        public double HapaxPercentage { get; set; }
        public double TypeTokenRatio { get; set; }
        public int MostFrequentWordCount { get; set; }
        public double MedianWordFrequency { get; set; }
    }

    public class ThematicInfo
    {
        public int TotalOccurrences { get; set; }
        public int UniqueWords { get; set; }
        public Dictionary<string, int> TopWords { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    }
}