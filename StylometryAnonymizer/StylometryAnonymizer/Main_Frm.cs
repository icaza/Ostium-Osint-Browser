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
            InitializeCustomControls();
            EventHandler();

            anonymizer = new TextAnonymizer("synonyms.json");
        }

        void InitializeCustomControls()
        {
            cmbStyle.Items.AddRange(new object[]
            {
                WritingStyle.Neutre,
                WritingStyle.Formel,
                WritingStyle.Journalistique,
                WritingStyle.Littéraire,
                WritingStyle.Technique,
                WritingStyle.Décontracté
            });
            cmbStyle.SelectedIndex = 0;

            chkRandomStyle.CheckedChanged += (s, e) =>
            {
                cmbStyle.Enabled = !chkRandomStyle.Checked;
            };

            cmbStyle.Enabled = !chkRandomStyle.Checked;
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
                    RestructureSentences = chkStructure.Checked,
                    MinModificationRate = 20.0,
                    StyleTarget = GetSelectedStyle(),
                    AggressiveMode = true,
                    RemoveIdioms = true,
                    RandomizeStyle = chkRandomStyle.Checked
                };

                int count = (int)numVariations.Value;
                lblStatus.Text = $"Génération de {count} variations en cours...";

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

        WritingStyle GetSelectedStyle()
        {
            if (chkRandomStyle?.Checked == true)
            {
                return WritingStyle.Neutre;
            }

            if (cmbStyle?.SelectedItem != null)
            {
                return (WritingStyle)cmbStyle.SelectedItem;
            }

            return WritingStyle.Neutre;
        }

        void DisplayResults(List<AnonymizationResult> results)
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

                double percentage = HighlightDifferences(originalText, results[i].Text);

                int percentStart = txtOutput.TextLength;
                txtOutput.AppendText($"\n[Modifications : {percentage:F1}% | Score : {results[i].StylometricScore:F2}]");
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
            try
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
            catch { }
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

    public enum WritingStyle
    {
        Neutre,
        Formel,
        Journalistique,
        Littéraire,
        Technique,
        Décontracté
    }

    public class AnonymizationOptions
    {
        public bool VariateSyntax { get; set; }
        public bool ReplaceVocabulary { get; set; }
        public bool ModifyPunctuation { get; set; }
        public bool RestructureSentences { get; set; }
        public double MinModificationRate { get; set; } = 20.0;
        public WritingStyle StyleTarget { get; set; } = WritingStyle.Neutre;
        public bool AggressiveMode { get; set; } = true;
        public bool RemoveIdioms { get; set; } = true;
        public bool RandomizeStyle { get; set; } = false;
    }

    public class AnonymizationResult
    {
        public string Text { get; set; }
        public double ModificationRate { get; set; }
        public double StylometricScore { get; set; }
    }

    public class TextAnonymizer
    {
        readonly ThreadLocal<Random> threadLocalRng;
        readonly Dictionary<string, string[]> synonyms;
        readonly HashSet<string> commonIdioms;
        readonly object synonymsLock = new object();

        static readonly Regex SentenceSplitPattern = new Regex(
            @"(?<=[.!?])\s+",
            RegexOptions.Compiled
        );

        static readonly Regex PassiveVoicePattern = new Regex(
            @"(\w+)\s+(est|sont|était|étaient|fut|furent)\s+(\w+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        static readonly Regex SubjectVerbPattern = new Regex(
            @"^(\w+)\s+(est|sont|a|ont|peut|peuvent|doit|doivent)\s+",
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

        // Connecteurs variés par style
        readonly Dictionary<WritingStyle, string[]> styleConnectors = new Dictionary<WritingStyle, string[]>
        {
            [WritingStyle.Formel] = new[] { ". ", ". Néanmoins, ", ". Toutefois, ", ". En outre, ", ". Par ailleurs, ", ". De surcroît, ", ". Qui plus est, " },
            [WritingStyle.Journalistique] = new[] { ". ", ". Selon les sources, ", ". D'après les informations, ", ". Il semblerait que ", ". On constate que " },
            [WritingStyle.Littéraire] = new[] { ". ", ". Et voilà que ", ". C'est ainsi que ", ". Tel était ", ". Or il advint que " },
            [WritingStyle.Technique] = new[] { ". ", ". On observe que ", ". Il convient de noter que ", ". À cet égard, ", ". Dans ce contexte, " },
            [WritingStyle.Décontracté] = new[] { ". ", ". Du coup, ", ". Bon, ", ". Bref, ", ". Enfin, ", ". Alors, " },
            [WritingStyle.Neutre] = new[] { ". ", ". En outre, ", ". Par ailleurs, ", ". Également, ", ". Aussi, ", ". Ainsi, " }
        };

        readonly string[] advancedPassiveTemplates = new[]
        {
            "{0} est {1}", "{0} a été {1}", "{0} se trouve {1}", "{0} demeure {1}",
            "{0} s'avère {1}", "Il apparaît que {0} est {1}", "On constate que {0} est {1}",
            "Force est de constater que {0} est {1}", "{0} se révèle {1}"
        };

        public TextAnonymizer(string jsonFilePath = "synonyms.json")
        {
            synonyms = LoadSynonymsFromJson(jsonFilePath);
            threadLocalRng = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
            commonIdioms = InitializeCommonIdioms();
        }

        HashSet<string> InitializeCommonIdioms()
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // Idiomes d'expressions'
                "chassez le naturel, il revient au galop",
                "c'est l'hôpital qui se moque de la charité",
                "qui se ressemble s'assemble",
                "petit à petit, l'oiseau fait son nid",
                "la fin justifie les moyens",
                
                // Ajoutez ici d'autres idiomes à détecter
                "il n'y a pas de fumée sans feu",
                "les chiens aboient, la caravane passe",
                "chat échaudé craint l'eau froide",
                "pierre qui roule n'amasse pas mousse",
                "tel est pris qui croyait prendre"
            };
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
                    MessageBox.Show($"Le dictionnaire de synonymes ne contient aucun mot. Vous devez en ajouter.", "Fichier vide",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                }

                return new Dictionary<string, string[]>(loadedSynonyms, StringComparer.OrdinalIgnoreCase);
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

        public List<AnonymizationResult> GenerateVariationsAsync(string text, int count, AnonymizationOptions options)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Le texte ne peut pas être vide", nameof(text));

            if (count <= 0)
                throw new ArgumentException("Le nombre de variations doit être positif", nameof(count));

            var variations = new List<AnonymizationResult>(count);
            var sentences = SplitIntoSentences(text);

            for (int i = 0; i < count; i++)
            {
                var seed = Guid.NewGuid().GetHashCode();
                var localRng = new Random(seed);

                // Style aléatoire pour chaque variation (plus anonymisant)
                var randomStyle = options.StyleTarget;
                if (options.RandomizeStyle)
                {
                    var styles = Enum.GetValues(typeof(WritingStyle));
                    randomStyle = (WritingStyle)styles.GetValue(localRng.Next(styles.Length));
                }

                int attempts = 0;
                double modRate;

                string transformed;
                // Boucle jusqu'à atteindre le taux de modification minimum
                do
                {
                    transformed = TransformText(sentences, options, localRng, randomStyle);
                    modRate = CalculateModificationRate(text, transformed);
                    attempts++;

                    if (attempts > 10) break; // Éviter boucle infinie

                } while (modRate < options.MinModificationRate && attempts < 10);

                var score = CalculateStylometricScore(text, transformed);

                variations.Add(new AnonymizationResult
                {
                    Text = transformed,
                    ModificationRate = modRate,
                    StylometricScore = score
                });
            }

            return variations;
        }

        double CalculateModificationRate(string original, string modified)
        {
            var originalWords = original.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var modifiedWords = modified.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            int different = 0;
            int total = Math.Max(originalWords.Length, modifiedWords.Length);

            for (int i = 0; i < Math.Min(originalWords.Length, modifiedWords.Length); i++)
            {
                if (!originalWords[i].Equals(modifiedWords[i], StringComparison.OrdinalIgnoreCase))
                    different++;
            }

            different += Math.Abs(originalWords.Length - modifiedWords.Length);

            return total > 0 ? (different * 100.0 / total) : 0.0;
        }

        double CalculateStylometricScore(string original, string modified)
        {
            // Score simple basé sur: diversité lexicale, longueur phrases, ponctuation
            double score = 0;

            // 1. Diversité lexicale
            var origWords = new HashSet<string>(original.ToLower().Split());
            var modWords = new HashSet<string>(modified.ToLower().Split());
            double lexicalDiversity = 1.0 - (origWords.Intersect(modWords).Count() / (double)origWords.Count);
            score += lexicalDiversity * 40;

            // 2. Différence longueur moyenne phrases
            var origSentences = SentenceSplitPattern.Split(original);
            var modSentences = SentenceSplitPattern.Split(modified);
            double origAvg = origSentences.Average(s => s.Length);
            double modAvg = modSentences.Average(s => s.Length);
            double lengthDiff = Math.Abs(origAvg - modAvg) / origAvg;
            score += Math.Min(lengthDiff * 30, 30);

            // 3. Différence ponctuation
            int origPunct = original.Count(c => char.IsPunctuation(c));
            int modPunct = modified.Count(c => char.IsPunctuation(c));
            double punctDiff = Math.Abs(origPunct - modPunct) / (double)Math.Max(origPunct, 1);
            score += Math.Min(punctDiff * 30, 30);

            return Math.Min(score, 100);
        }

        string TransformText(List<string> sentences, AnonymizationOptions options, Random localRng, WritingStyle style)
        {
            var transformedSentences = new List<string>(sentences);

            // Suppression/réécriture d'idiomes
            if (options.RemoveIdioms)
            {
                transformedSentences = RemoveOrRewriteIdioms(transformedSentences, localRng);
            }

            // Restructuration AVANT transformations locales
            if (options.RestructureSentences && localRng.Next(100) < 70)
            {
                transformedSentences = RestructureSentencesAdvanced(transformedSentences, localRng);
            }

            // Transformations sur chaque phrase
            for (int i = 0; i < transformedSentences.Count; i++)
            {
                string transformed = transformedSentences[i];

                // Augmentation des taux de transformation
                if (options.ReplaceVocabulary)
                    transformed = ReplaceSynonymsAdvanced(transformed, localRng, options.AggressiveMode);

                if (options.VariateSyntax)
                    transformed = VariateSyntaxAdvanced(transformed, localRng, options.AggressiveMode);

                if (options.ModifyPunctuation)
                    transformed = ModifyPunctuationAdvanced(transformed, localRng, options.AggressiveMode);

                transformedSentences[i] = transformed;
            }

            return JoinSentences(transformedSentences, localRng, style);
        }

        List<string> RemoveOrRewriteIdioms(List<string> sentences, Random rng)
        {
            var result = new List<string>();

            foreach (var sentence in sentences)
            {
                string modified = sentence;

                foreach (var idiom in commonIdioms)
                {
                    if (modified.Contains(idiom))
                    {
                        // CONDITION 1: 60% de chance de SUPPRIMER l'idiome
                        if (rng.Next(100) < 60)
                        {
                            // Suppression pure et simple
                            modified = modified.Replace(idiom, "");
                        }
                        // CONDITION 2: 40% de chance de PARAPHRASER l'idiome
                        else
                        {
                            // Paraphrase via ParaphraseIdiom()
                            string paraphrase = ParaphraseIdiom(idiom, rng);

                            // Si une paraphrase existe, on remplace
                            if (!string.IsNullOrEmpty(paraphrase))
                            {
                                modified = modified.Replace(idiom, paraphrase);
                            }
                            else
                            {
                                // Sinon on supprime aussi
                                modified = modified.Replace(idiom, "");
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(modified))
                    result.Add(modified);
            }

            return result.Any() ? result : sentences;
        }

        string ParaphraseIdiom(string idiom, Random rng)
        {
            // Dictionnaire de paraphrases pour les idiomes courants
            var paraphrases = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["chassez le naturel, il revient au galop"] = new[]
                {
                    "les habitudes sont tenaces",
                    "on ne change pas si facilement",
                    "le tempérament reprend toujours le dessus",
                    "la nature profonde finit toujours par ressurgir",
                    "impossible d'échapper à sa vraie nature"
                },
                ["c'est l'hôpital qui se moque de la charité"] = new[]
                {
                    "une critique paradoxale",
                    "une remarque ironique venant de cette source",
                    "un reproche étonnant",
                    "quelle hypocrisie dans ce jugement",
                    "voilà un commentaire pour le moins contradictoire"
                },
                ["qui se ressemble s'assemble"] = new[]
                {
                    "les affinités rapprochent les gens",
                    "on s'entoure de nos semblables",
                    "les similitudes créent des liens"
                },
                ["petit à petit, l'oiseau fait son nid"] = new[]
                {
                    "la persévérance porte ses fruits",
                    "les efforts réguliers finissent par payer",
                    "progresser graduellement mène au succès"
                },
                ["la fin justifie les moyens"] = new[]
                {
                    "peu importe la méthode si le résultat est atteint",
                    "l'objectif prime sur le processus",
                    "tous les chemins sont bons pour arriver au but"
                }
            };

            // Chercher une paraphrase correspondante
            if (paraphrases.TryGetValue(idiom.Trim(), out var options))
            {
                // Sélectionner aléatoirement une paraphrase
                return options[rng.Next(options.Length)];
            }

            // Si aucune paraphrase n'existe, retourner vide
            return "";
        }

        List<string> SplitIntoSentences(string text)
        {
            var sentences = SentenceSplitPattern.Split(text)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            return sentences.Any() ? sentences : new List<string> { text };
        }

        string ReplaceSynonymsAdvanced(string text, Random localRng, bool aggressive)
        {
            if (synonyms.Count == 0)
                return text;

            var result = text;
            int replacementThreshold = aggressive ? 70 : 50; // Augmenté

            lock (synonymsLock)
            {
                // Mélange pour éviter patterns prévisibles
                var shuffledSynonyms = synonyms.OrderBy(_ => localRng.Next()).ToList();
                var usedReplacements = new HashSet<string>();

                foreach (var kvp in shuffledSynonyms)
                {
                    if (localRng.Next(100) >= replacementThreshold)
                        continue;

                    // Éviter de réutiliser les mêmes remplacements
                    var availableSynonyms = kvp.Value.Where(s => !usedReplacements.Contains(s)).ToArray();
                    if (availableSynonyms.Length == 0)
                        availableSynonyms = kvp.Value;

                    var pattern = $@"\b{Regex.Escape(kvp.Key)}\b";
                    var replacement = availableSynonyms[localRng.Next(availableSynonyms.Length)];
                    usedReplacements.Add(replacement);

                    // Ne remplacer qu'UNE seule occurrence aléatoire
                    var matches = Regex.Matches(result, pattern, RegexOptions.IgnoreCase);
                    if (matches.Count > 0)
                    {
                        var matchIndex = localRng.Next(matches.Count);
                        var match = matches[matchIndex];
                        result = result.Remove(match.Index, match.Length).Insert(match.Index, replacement);
                    }
                }
            }

            return result;
        }

        string VariateSyntaxAdvanced(string text, Random localRng, bool aggressive)
        {
            var result = text;
            int threshold = aggressive ? 50 : 30;

            // Voix passive - augmenté
            if (localRng.Next(100) < threshold)
            {
                var match = PassiveVoicePattern.Match(result);
                if (match.Success)
                {
                    var template = advancedPassiveTemplates[localRng.Next(advancedPassiveTemplates.Length)];
                    result = result.Replace(
                        match.Value,
                        string.Format(template, match.Groups[1].Value, match.Groups[3].Value)
                    );
                }
            }

            // Inversion sujet-verbe - augmenté
            if (localRng.Next(100) < threshold)
            {
                result = SubjectVerbPattern.Replace(
                    result,
                    m => m.Groups[2].Value + " " + m.Groups[1].Value + " ",
                    1
                );
            }

            //// Ajout de subordonnées
            if (localRng.Next(100) < 30 && result.Contains(","))
            {
                var subordinates = new[] { "bien que ", "sachant que ", "étant donné que ", "puisque " };
                var parts = result.Split(new[] { ", " }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    parts[0] += ", " + subordinates[localRng.Next(subordinates.Length)];
                    result = string.Join(" ", parts);
                }
            }

            // Variation ordre propositions
            if (localRng.Next(100) < 25 && result.Contains(" et "))
            {
                var parts = result.Split(new[] { " et " }, 2, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    result = parts[1].Trim() + " et " + char.ToLower(parts[0][0]) + parts[0].Substring(1);
                }
            }

            return result;
        }

        string ModifyPunctuationAdvanced(string text, Random localRng, bool aggressive)
        {
            var result = text;
            int threshold = aggressive ? 50 : 30;

            // Variation guillemets
            if (localRng.Next(100) < threshold && result.Contains("\""))
            {
                var styles = new[] { "« $1 »", "‹ $1 ›", "'$1'" };
                result = Regex.Replace(result, @"""([^""]+)""", styles[localRng.Next(styles.Length)]);
            }

            // Points de suspension
            if (localRng.Next(100) < 15)
            {
                if (result.Contains("..."))
                    result = result.Replace("...", localRng.Next(2) == 0 ? "." : "…");
            }

            // Variation ponctuation forte
            if (localRng.Next(100) < 20)
            {
                if (result.EndsWith(".") && localRng.Next(3) == 0)
                    result = result.Substring(0, result.Length - 1) + (localRng.Next(2) == 0 ? " !" : " ?");
            }

            // Tirets et virgules
            if (localRng.Next(100) < threshold)
            {
                if (localRng.Next(2) == 0)
                    result = result.Replace(" - ", ", ");
                else
                    result = result.Replace(", ", " - ");
            }

            // Points-virgules
            if (localRng.Next(100) < 20 && result.Contains(", "))
            {
                result = Regex.Replace(result, @", (\w)", m => "; " + m.Groups[1].Value, (RegexOptions)1);
            }

            return result;
        }

        List<string> RestructureSentencesAdvanced(List<string> sentences, Random localRng)
        {
            if (sentences.Count < 2)
                return sentences;

            var restructured = new List<string>(sentences);

            // Fusion phrases - augmenté à 40%
            for (int i = 0; i < restructured.Count - 1; i++)
            {
                if (localRng.Next(100) < 40 &&
                    restructured[i].Length < 80 &&
                    restructured[i + 1].Length < 80)
                {
                    var connectors = new[] { " et ", ", ", " ; ", " car ", " donc ", " mais " };
                    var connector = connectors[localRng.Next(connectors.Length)];

                    restructured[i] = restructured[i].TrimEnd('.', '!', '?') + connector +
                        char.ToLower(restructured[i + 1][0]) + restructured[i + 1].Substring(1);
                    restructured.RemoveAt(i + 1);
                }
            }

            // Division phrases - augmenté à 35%
            for (int i = 0; i < restructured.Count; i++)
            {
                if (localRng.Next(100) < 35 && restructured[i].Length > 100)
                {
                    var splitPoints = new[] { ", ", " et ", " mais ", " car " };
                    foreach (var point in splitPoints)
                    {
                        if (restructured[i].Contains(point))
                        {
                            var parts = restructured[i].Split(new[] { point }, 2, StringSplitOptions.None);
                            if (parts.Length == 2 && parts[0].Length > 30)
                            {
                                restructured[i] = parts[0].Trim() + ".";
                                restructured.Insert(i + 1, char.ToUpper(parts[1][0]) + parts[1].Substring(1));
                                i++;
                                break;
                            }
                        }
                    }
                }
            }

            //  Réorganisation plus agressive - 25%
            if (localRng.Next(100) < 25 && restructured.Count > 3)
            {
                var swapCount = Math.Max(2, restructured.Count / 3);
                for (int i = 0; i < swapCount; i++)
                {
                    var idx1 = localRng.Next(restructured.Count);
                    var idx2 = localRng.Next(restructured.Count);

                    if (idx1 != idx2 && Math.Abs(idx1 - idx2) > 1) // Éviter échanges adjacents
                    {
                        (restructured[idx2], restructured[idx1]) = (restructured[idx1], restructured[idx2]);
                    }
                }
            }

            // Inversion ordre dans phrase complexe
            for (int i = 0; i < restructured.Count; i++)
            {
                if (localRng.Next(100) < 20 && restructured[i].Contains(" qui "))
                {
                    // Exemple: "L'homme qui parle" -> "Celui qui parle, l'homme"
                    var match = Regex.Match(restructured[i], @"(\w+\s+\w+)\s+qui\s+(.+)");
                    if (match.Success)
                    {
                        restructured[i] = $"Celui qui {match.Groups[2].Value}, {match.Groups[1].Value.ToLower()}";
                    }
                }
            }

            return restructured;
        }

        string JoinSentences(List<string> sentences, Random localRng, WritingStyle style)
        {
            var connectors = styleConnectors.ContainsKey(style)
                ? styleConnectors[style]
                : styleConnectors[WritingStyle.Neutre];

            // Mélanger les connecteurs pour éviter patterns
            var shuffledConnectors = connectors.OrderBy(_ => localRng.Next()).ToArray();
            int connectorIndex = 0;

            var sb = new StringBuilder(sentences.Sum(s => s.Length) + sentences.Count * 30);

            for (int i = 0; i < sentences.Count; i++)
            {
                var sentence = sentences[i];

                // Assurer ponctuation finale
                if (!sentence.EndsWith(".") && !sentence.EndsWith("!") && !sentence.EndsWith("?"))
                {
                    sentence += ".";
                }

                sb.Append(sentence);

                // Connecteur varié (pas pour la dernière phrase)
                if (i < sentences.Count - 1)
                {
                    // Randomiser davantage le choix des connecteurs
                    string connector;
                    if (localRng.Next(100) < 30)
                    {
                        connector = " "; // Pas de connecteur parfois
                    }
                    else
                    {
                        connector = shuffledConnectors[connectorIndex % shuffledConnectors.Length];
                        connectorIndex++;
                    }
                    sb.Append(connector);
                }
            }

            var result = sb.ToString();

            // Normalisation
            result = WhitespaceNormalizer.Replace(result, " ");
            result = PunctuationSpacingBefore.Replace(result, "$1");
            result = PunctuationSpacingAfter.Replace(result, "$1 $2");

            // Éliminer doubles ponctuations
            result = Regex.Replace(result, @"\.(\s*\.)+", ".");
            result = Regex.Replace(result, @"([.!?])\s+\.", "$1");
            result = Regex.Replace(result, @"([.!?])\s+([.!?])", "$1");

            // Variation espacement après ponctuation (style)
            if (style == WritingStyle.Littéraire && localRng.Next(100) < 20)
            {
                result = Regex.Replace(result, @"([.!?]) ", "$1  "); // Double espace parfois
            }

            return result.Trim();
        }

        public void Dispose()
        {
            threadLocalRng?.Dispose();
        }
    }
}