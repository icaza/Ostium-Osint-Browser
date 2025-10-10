using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StylometryAnonymizer
{
    public partial class Main_Frm : Form
    {
        readonly TextAnonymizer anonymizer;
        bool txtFocus = true;

        public Main_Frm()
        {
            InitializeComponent();
            EventHandler();

            anonymizer = new TextAnonymizer("synonyms.json");
        }

        void EventHandler()
        {
            txtInput.Click += new EventHandler(TxtInOut_Click);
            txtOutput.Click += new EventHandler(TxtInOut_Click);
        }

        async void BtnAnonymize_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                MessageBox.Show("Veuillez entrer un texte à anonymiser.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            BtnAnonymize.Enabled = false;
            txtOutput.Clear();

            try
            {
                var options = new AnonymizationOptions
                {
                    VariateSyntax = chkSyntax.Checked,
                    ReplaceVocabulary = chkVocabulary.Checked,
                    ModifyPunctuation = chkPunctuation.Checked,
                    RestructureSentences = chkStructure.Checked
                };

                int count = (int)numVariations.Value;
                lblStatus.Text = $"Génération de {count} variations en cours...";

                var anonymizer = new TextAnonymizer();
                var results = await Task.Run(() => anonymizer.GenerateVariationsAsync(txtInput.Text, count, options));

                DisplayResults(results);
                lblStatus.Text = $"{count} variations générées avec succès";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Erreur lors de l'anonymisation";
            }
            finally
            {
                BtnAnonymize.Enabled = true;
            }
        }

        void DisplayResults(List<string> results)
        {
            txtOutput.Clear();
            string originalText = txtInput.Text;

            for (int i = 0; i < results.Count; i++)
            {
                int titleStart = txtOutput.TextLength;
                txtOutput.AppendText($"═══ Variation {i + 1} ═══\n");
                txtOutput.Select(titleStart, txtOutput.TextLength - titleStart);
                txtOutput.SelectionColor = Color.Blue;
                txtOutput.SelectionFont = new Font(txtOutput.Font, FontStyle.Bold);

                double percentage = HighlightDifferences(originalText, results[i]);

                int percentStart = txtOutput.TextLength;
                txtOutput.AppendText($"\n[Modifications : {percentage:F1}%]");
                txtOutput.Select(percentStart, txtOutput.TextLength - percentStart);
                txtOutput.SelectionColor = Color.Gray;
                txtOutput.SelectionFont = new Font(txtOutput.Font, FontStyle.Italic);

                txtOutput.AppendText("\n\n");
            }

            txtOutput.SelectionStart = 0;
            txtOutput.SelectionColor = txtOutput.ForeColor;
        }

        void BtnCopy_Click(object sender, EventArgs e)
        {
            if (txtFocus)
            {
                if (!string.IsNullOrWhiteSpace(txtInput.Text))
                    Clipboard.SetText(txtInput.SelectedText);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(txtOutput.Text))
                    Clipboard.SetText(txtOutput.SelectedText);
            }

            lblStatus.Text = "Copié dans le presse-papiers";
                Console.Beep(800, 600);
        }

        void BtnClear_Click(object sender, EventArgs e)
        {
            txtInput.Text = "";
            txtOutput.Text = "";
        }

        void BtnLoadList_Click(object sender, EventArgs e)
        {
            anonymizer.ReloadSynonyms("synonyms.json");
        }

        double HighlightDifferences(string originalText, string modifiedText)
        {
            string normalizedOriginal = Regex.Replace(originalText, @"\s+", " ").Trim();
            string normalizedModified = Regex.Replace(modifiedText, @"\s+", " ").Trim();

            // Utiliser un algorithme de différence simple
            var originalTokens = TokenizeText(normalizedOriginal);
            var modifiedTokens = TokenizeText(normalizedModified);

            var originalDict = new Dictionary<string, int>();
            foreach (var token in originalTokens)
            {
                string key = token.ToLower();
                if (originalDict.ContainsKey(key))
                    originalDict[key]++;
                else
                    originalDict[key] = 1;
            }

            int totalTokens = 0;
            int modifiedCount = 0;

            foreach (var token in modifiedTokens)
            {
                int startIndex = txtOutput.TextLength;
                txtOutput.AppendText(token);

                if (token == " ") continue;

                totalTokens++;

                string tokenLower = token.ToLower();
                bool isDifferent = false;
                bool isPunctuation = false;

                if (token.Any(char.IsLetter))
                {
                    if (!originalDict.ContainsKey(tokenLower))
                    {
                        isDifferent = true;
                        modifiedCount++;
                    }
                }
                else if (token.Any(c => char.IsPunctuation(c) || c == '(' || c == ')'))
                {
                    if (!originalTokens.Any(t => t == token))
                    {
                        isDifferent = true;
                        isPunctuation = true;
                        modifiedCount++;
                    }
                }

                if (isDifferent)
                {
                    txtOutput.Select(startIndex, token.Length);
                    txtOutput.SelectionColor = isPunctuation ? Color.Orange : Color.White;
                    txtOutput.SelectionBackColor = Color.Black;
                }
            }

            txtOutput.SelectionStart = txtOutput.TextLength;
            txtOutput.SelectionColor = txtOutput.ForeColor;
            txtOutput.SelectionBackColor = txtOutput.BackColor;

            return totalTokens > 0 ? (modifiedCount * 100.0 / totalTokens) : 0.0;
        }

        List<string> TokenizeText(string text)
        {
            var tokens = new List<string>();
            var currentToken = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (char.IsWhiteSpace(c))
                {
                    if (currentToken.Length > 0)
                    {
                        tokens.Add(currentToken.ToString());
                        currentToken.Clear();
                    }
                    tokens.Add(" ");
                }
                else if (char.IsPunctuation(c) || c == '(' || c == ')')
                {
                    if (currentToken.Length > 0)
                    {
                        tokens.Add(currentToken.ToString());
                        currentToken.Clear();
                    }
                    tokens.Add(c.ToString());
                }
                else
                {
                    currentToken.Append(c);
                }
            }

            if (currentToken.Length > 0)
            {
                tokens.Add(currentToken.ToString());
            }

            return tokens;
        }

        void TxtInOut_Click(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                txtFocus = true;
            }
            else if (sender is RichTextBox)
            {
                txtFocus = false;
            }
        }
    }

    public class AnonymizationOptions
    {
        public bool VariateSyntax { get; set; }
        public bool ReplaceVocabulary { get; set; }
        public bool ModifyPunctuation { get; set; }
        public bool RestructureSentences { get; set; }
    }

    public class TextAnonymizer
    {
        readonly ThreadLocal<Random> threadLocalRng;
        readonly Dictionary<string, string[]> synonyms;
        readonly object synonymsLock = new object();

        // Patterns compilés
        static readonly Regex SentenceSplitPattern = new Regex(
            @"(?<=[.!?])\s+",
            RegexOptions.Compiled
        );

        static readonly Regex PassiveVoicePattern = new Regex(
            @"(\w+)\s+(est|sont|était|étaient)\s+(\w+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        static readonly Regex SubjectVerbPattern = new Regex(
            @"^(\w+)\s+(est|sont|a|ont|peut|peuvent)\s+",
            RegexOptions.Compiled
        );

        static readonly Regex ConjunctionPattern = new Regex(
            @"(\w+)\s+(et|ou|donc|car|mais)\s+",
            RegexOptions.Compiled
        );

        static readonly Regex LongSentenceSplitPattern = new Regex(
            @"(.{60,}?)[,;]\s+(.+)",
            RegexOptions.Compiled
        );

        static readonly Regex WhitespaceNormalizer = new Regex(
            @"\s+",
            RegexOptions.Compiled
        );

        static readonly Regex PunctuationSpacingBefore = new Regex(
            @"\s+([.,;:!?])",
            RegexOptions.Compiled
        );

        static readonly Regex PunctuationSpacingAfter = new Regex(
            @"([.,;:!?])([^\s])",
            RegexOptions.Compiled
        );

        static readonly Regex DoubleDotsRemover = new Regex(
            @"\.(\s*\.)+",
            RegexOptions.Compiled
        );

        readonly string[] sentenceConnectors = new[]
        {
        "", ". ", ". En outre, ", ". Par ailleurs, ", ". De plus, ", ". Également, ",
        ". Aussi, ", ". D'autre part, ", ". En effet, ", ". Ainsi, ", ". Cependant, ",
        ". Néanmoins, ", ". Toutefois, ", ". Pourtant, ", ". Or, ", ". Car, ",
        ". En conséquence, ", ". Par conséquent, ", ". Dès lors, ", ". De ce fait, "
    };

        readonly string[] passiveVoiceTemplates = new[]
        {
        "{0} est {1}", "{0} a été {1}", "{0} se trouve {1}",
        "{0} demeure {1}", "{0} s'avère {1}", "Il apparaît que {0} est {1}"
    };

        // Constructeur
        public TextAnonymizer(string jsonFilePath = "synonyms.json")
        {
            synonyms = LoadSynonymsFromJson(jsonFilePath);
            threadLocalRng = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
        }

        Dictionary<string, string[]> LoadSynonymsFromJson(string jsonFilePath)
        {
            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    MessageBox.Show($"Erreur lors du chargement du dictionnaire de synonymes. Le fichier synonyms.json n'existe pas.\n" +
                        $"\nVous devez créer un fichier de synonymes à la racine du répertoire de l'application.", "Fichier manquant", 
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                }

                string jsonString = File.ReadAllText(jsonFilePath);
                var loadedSynonyms = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(jsonString);

                if (loadedSynonyms == null || loadedSynonyms.Count == 0)
                {
                    MessageBox.Show($"Le dictionnaire de synonymes ne contiens aucun mot. Vous devez en ajouter.", "Fichier vide",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                }

                var result = new Dictionary<string, string[]>(loadedSynonyms, StringComparer.OrdinalIgnoreCase);

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du dictionnaire :\n{ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public void ReloadSynonyms(string jsonFilePath = "synonyms.json")
        {
            var newSynonyms = LoadSynonymsFromJson(jsonFilePath);

            if (newSynonyms.Count > 0)
            {
                lock (synonymsLock)
                {
                    synonyms.Clear();
                    foreach (var kvp in newSynonyms)
                    {
                        synonyms[kvp.Key] = kvp.Value;
                    }
                }

                MessageBox.Show("Le dictionnaire de synonymes a été rechargé avec succès.", 
                    "Rechargement réussi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public List<string> GenerateVariationsAsync(string text, int count, AnonymizationOptions options)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Le texte ne peut pas être vide", nameof(text));

            if (count <= 0)
                throw new ArgumentException("Le nombre de variations doit être positif", nameof(count));

            var variations = new List<string>(count);
            var sentences = SplitIntoSentences(text);

            for (int i = 0; i < count; i++)
            {
                var seed = Guid.NewGuid().GetHashCode();
                var localRng = new Random(seed);
                var transformed = TransformText(sentences, options, localRng);
                variations.Add(transformed);
            }

            return variations;
        }

        string TransformText(List<string> sentences, AnonymizationOptions options, Random localRng)
        {
            var transformedSentences = new List<string>(sentences.Count);

            foreach (var sentence in sentences)
            {
                string transformed = sentence;

                if (options.ReplaceVocabulary)
                    transformed = ReplaceSynonyms(transformed, localRng);

                if (options.VariateSyntax)
                    transformed = VariateSyntax(transformed, localRng);

                if (options.ModifyPunctuation)
                    transformed = ModifyPunctuation(transformed, localRng);

                transformedSentences.Add(transformed);
            }

            if (options.RestructureSentences)
                transformedSentences = RestructureSentences(transformedSentences, localRng);

            return JoinSentences(transformedSentences, localRng);
        }

        List<string> SplitIntoSentences(string text)
        {
            var sentences = SentenceSplitPattern.Split(text)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            return sentences.Any() ? sentences : new List<string> { text };
        }

        string ReplaceSynonyms(string text, Random localRng)
        {
            if (synonyms.Count == 0)
                return text;

            var result = text;

            lock (synonymsLock)
            {
                // Créer une liste ordonnée aléatoirement 1x
                var shuffledSynonyms = synonyms.OrderBy(_ => localRng.Next()).ToList();

                foreach (var kvp in shuffledSynonyms)
                {
                    if (localRng.Next(100) >= 50)
                        continue;

                    var pattern = $@"\b{Regex.Escape(kvp.Key)}\b";
                    var replacement = kvp.Value[localRng.Next(kvp.Value.Length)];

                    result = Regex.Replace(
                        result,
                        pattern,
                        replacement,
                        RegexOptions.IgnoreCase
                    );
                }
            }

            return result;
        }

        string VariateSyntax(string text, Random localRng)
        {
            var result = text;

            // Variation de la voix (active/passive) - réduit à 20%
            if (localRng.Next(100) < 20)
            {
                var match = PassiveVoicePattern.Match(result);
                if (match.Success && localRng.Next(2) == 0)
                {
                    var template = passiveVoiceTemplates[localRng.Next(passiveVoiceTemplates.Length)];
                    result = result.Replace(
                        match.Value,
                        string.Format(template, match.Groups[1].Value, match.Groups[3].Value)
                    );
                }
            }

            // Inversion sujet-verbe occasionnelle - réduit à 15%
            if (localRng.Next(100) < 15)
            {
                result = SubjectVerbPattern.Replace(
                    result,
                    m => m.Groups[2].Value + " " + m.Groups[1].Value + " "
                );
            }

            // Modification de virgules - réduit à 25%
            if (localRng.Next(100) < 25)
            {
                if (localRng.Next(2) == 0)
                    result = result.Replace(", ", " ");
                else
                    result = ConjunctionPattern.Replace(result, "$1, $2 ");
            }

            return result;
        }

        string ModifyPunctuation(string text, Random localRng)
        {
            var result = text;

            // Limiter les points de suspension
            if (localRng.Next(100) < 5 && result.Contains("..."))
            {
                // Remplacer par un point simple la plupart du temps
                if (localRng.Next(100) < 70)
                    result = result.Replace("...", ".");
                else
                    result = result.Replace("...", "…");
            }

            // Variation des guillemets - réduit à 20%
            if (localRng.Next(100) < 20 && result.Contains("\""))
            {
                var usesFrenchQuotes = localRng.Next(2) == 0;
                if (usesFrenchQuotes)
                {
                    result = Regex.Replace(result, @"""([^""]+)""", "« $1 »");
                }
            }

            // Ajout de parenthèses occasionnelles - réduit à 10%
            if (localRng.Next(100) < 10)
            {
                var words = result.Split(' ');
                if (words.Length > 5)
                {
                    var pos = localRng.Next(2, words.Length - 2);
                    words[pos] = $"({words[pos]})";
                    result = string.Join(" ", words);
                }
            }

            // Variation tirets/virgules - réduit à 20%
            if (localRng.Next(100) < 20)
            {
                if (localRng.Next(2) == 0)
                    result = result.Replace(" - ", ", ");
                else
                    result = result.Replace(", ", " - ");
            }

            return result;
        }

        List<string> RestructureSentences(List<string> sentences, Random localRng)
        {
            if (sentences.Count < 2)
                return sentences;

            var restructured = new List<string>(sentences);

            // Fusion de phrases courtes - réduit à 20%
            for (int i = 0; i < restructured.Count - 1; i++)
            {
                if (localRng.Next(100) < 20 &&
                    restructured[i].Length < 60 &&
                    restructured[i + 1].Length < 60)
                {
                    var connectors = new[] { " et ", ", ", " ; " };
                    var connector = connectors[localRng.Next(connectors.Length)];

                    restructured[i] = restructured[i].TrimEnd('.', '!', '?') + connector +
                        char.ToLower(restructured[i + 1][0]) + restructured[i + 1].Substring(1);
                    restructured.RemoveAt(i + 1);
                }
            }

            // Division de phrases longues - réduit à 15%
            for (int i = 0; i < restructured.Count; i++)
            {
                if (localRng.Next(100) < 15 && restructured[i].Length > 120)
                {
                    var match = LongSentenceSplitPattern.Match(restructured[i]);
                    if (match.Success)
                    {
                        restructured[i] = match.Groups[1].Value.Trim() + ".";
                        restructured.Insert(
                            i + 1,
                            char.ToUpper(match.Groups[2].Value[0]) + match.Groups[2].Value.Substring(1)
                        );
                        i++;
                    }
                }
            }

            // Réorganisation aléatoire - réduit à 10%
            if (localRng.Next(100) < 10 && restructured.Count > 2)
            {
                var swapCount = Math.Min(1, restructured.Count / 4);
                for (int i = 0; i < swapCount; i++)
                {
                    var idx1 = localRng.Next(restructured.Count);
                    var idx2 = localRng.Next(restructured.Count);

                    if (idx1 != idx2)
                    {
                        (restructured[idx2], restructured[idx1]) = (restructured[idx1], restructured[idx2]);
                    }
                }
            }

            return restructured;
        }

        string JoinSentences(List<string> sentences, Random localRng)
        {
            var sb = new StringBuilder(sentences.Sum(s => s.Length) + sentences.Count * 20);

            for (int i = 0; i < sentences.Count; i++)
            {
                // Ajouter la phrase en s'assurant qu'elle se termine par une ponctuation
                var sentence = sentences[i];
                if (!sentence.EndsWith(".") &&
                    !sentence.EndsWith("!") &&
                    !sentence.EndsWith("?"))
                {
                    sentence += ".";
                }

                sb.Append(sentence);

                // Ajouter le connecteur APRÈS la phrase (sauf pour la dernière)
                if (i < sentences.Count - 1)
                {
                    var connector = sentenceConnectors[localRng.Next(sentenceConnectors.Length)];
                    sb.Append(connector);
                }
            }

            var result = sb.ToString();

            // Normalisation des espaces et ponctuation
            result = WhitespaceNormalizer.Replace(result, " ");
            result = PunctuationSpacingBefore.Replace(result, "$1");
            result = PunctuationSpacingAfter.Replace(result, "$1 $2");

            // Éliminer les doubles points
            result = DoubleDotsRemover.Replace(result, ".");

            // Éliminer ponctuation double (! . ou ? . ou ! ! etc.)
            result = Regex.Replace(result, @"([.!?])\s+\.", "$1");
            result = Regex.Replace(result, @"([.!?])\s+([.!?])", "$1");

            return result.Trim();
        }

        public void Dispose()
        {
            threadLocalRng?.Dispose();
        }
    }
}
