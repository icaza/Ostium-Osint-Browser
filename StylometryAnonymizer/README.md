[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8.1-blue.svg)](https://dotnet.microsoft.com/download/dotnet-framework)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](https://github.com/icaza/Ostium-Osint-Browser/blob/feature%2314/StylometryAnonymizer/LICENSE.txt)

# 🚀 Voir aussi la nouvelle app IAnon (Multilingue)

IAnon modifie le style rédactionnel avec l'IA en local via Ollama pour rendre l'analyse stylométrique plus complexe, protégeant ainsi l'identité de l'auteur. [plus d'infos](https://github.com/icaza/IAnon)

---

# StylometryAnonymizer

**StylometryAnonymizer** est conçue pour générer des variations de texte en français tout en préservant le sens original. L'outil modifie le style d'écriture pour rendre l'analyse stylométrique plus difficile, protégeant ainsi l'identité de l'auteur.

## Objectif

La stylométrie permet d'identifier un auteur par son style d'écriture unique. StylometryAnonymizer combat cette analyse en :
- Remplaçant le vocabulaire par des synonymes contextuels
- Variant la structure syntaxique des phrases
- Modifiant la ponctuation de manière subtile
- Restructurant l'ordre et la composition des phrases

## Caractéristiques

### Transformations Multiples
- **Remplacement de vocabulaire** : Utilise un dictionnaire de synonymes personnalisable
- **Variation syntaxique** : Change la voix (active/passive), inverse sujet-verbe
- **Modification de ponctuation** : Adapte les guillemets, tirets, virgules de façon naturelle
- **Restructuration de phrases** : Fusionne ou divise les phrases, réorganise l'ordre

### Performance Optimisée
- **Regex précompilés** : Patterns compilés pour des performances maximales
- **Thread-safe** : Utilisation de `ThreadLocal<Random>` pour le multithreading
- **StringBuilder optimisé** : Pré-allocation mémoire intelligente
- **Lock minimal** : Verrouillage uniquement sur les opérations critiques

### Protection Anti-Stylométrie
- **Probabilités réduites** : Modifications subtiles pour éviter les patterns détectables
- **Élimination des signatures** : Suppression automatique des doubles ponctuations
- **Variations naturelles** : Transformations qui préservent la lisibilité

## Installation

### Prérequis
- .NET Framework 4.8.1 ou supérieur
- Newtonsoft.Json (via NuGet)

### Via NuGet
```bash
Install-Package Newtonsoft.Json
```

## Usage

### Configuration de Base

```csharp
using System;
using System.Collections.Generic;

// Initialiser avec un fichier de synonymes
var anonymizer = new TextAnonymizer("synonyms.json");

// Configurer les options
var options = new AnonymizationOptions
{
    ReplaceVocabulary = true,
    VariateSyntax = true,
    ModifyPunctuation = true,
    RestructureSentences = true
};

// Générer des variations
string texteOriginal = "Votre texte à anonymiser ici.";
List<string> variations = anonymizer.GenerateVariations(texteOriginal, 10, options);

// Afficher les résultats
for (int i = 0; i < variations.Count; i++)
{
    Console.WriteLine($"Variation {i + 1}: {variations[i]}");
}
```

### Format du Fichier de Synonymes

Créez un fichier `synonyms.json` au format suivant :

```json
{
  "économie": ["finances", "épargne", "rationalisation", "budget"],
  "souffrance": ["misère", "tourment", "tribulation", "affliction"],
  "causer": ["provoquer", "engendrer", "susciter", "déclencher"],
  "nation": ["pays", "état", "patrie", "territoire"]
}
```

### Options de Personnalisation

```csharp
var options = new AnonymizationOptions
{
    ReplaceVocabulary = true,      // Active le remplacement des synonymes
    VariateSyntax = true,           // Active les variations syntaxiques
    ModifyPunctuation = true,       // Active la modification de ponctuation
    RestructureSentences = false    // Désactive la restructuration (optionnel)
};
```

## Exemple de Sortie

**Texte original :**
```
L'économie française traverse une période difficile. Les citoyens souffrent des conséquences.
```

**Variations générées :**
```
Variation 1: Les finances françaises traversent une période difficile. Les citoyens endurent les conséquences.

Variation 2: L'épargne française traverse une période difficile. En outre, les habitants souffrent des répercussions.

Variation 3: Le budget français traverse une période difficile ; les résidents subissent les conséquences.
```

### Méthodes Principales

#### GenerateVariations
```csharp
List<string> GenerateVariations(string text, int count, AnonymizationOptions options)
```
Génère un nombre spécifié de variations du texte d'entrée.

**Paramètres :**
- `text` : Le texte original à transformer
- `count` : Nombre de variations à générer
- `options` : Configuration des transformations

**Retour :** Liste de chaînes contenant les variations

#### ReloadSynonyms
```csharp
void ReloadSynonyms(string jsonFilePath = "synonyms.json")
```
Recharge le dictionnaire de synonymes sans redémarrer l'application.

### Classe AnonymizationOptions

```csharp
public class AnonymizationOptions
{
    public bool ReplaceVocabulary { get; set; }      // Défaut: true
    public bool VariateSyntax { get; set; }          // Défaut: true
    public bool ModifyPunctuation { get; set; }      // Défaut: true
    public bool RestructureSentences { get; set; }   // Défaut: true
}
```

## Architecture

### Composants Principaux

```
TextAnonymizer
├── LoadSynonymsFromJson()      // Chargement du dictionnaire
├── GenerateVariations()         // Point d'entrée principal
├── TransformText()              // Orchestration des transformations
├── ReplaceSynonyms()            // Remplacement vocabulaire
├── VariateSyntax()              // Modifications syntaxiques
├── ModifyPunctuation()          // Adaptations de ponctuation
├── RestructureSentences()       // Restructuration de phrases
└── JoinSentences()              // Assemblage et normalisation
```

### Patterns Regex Précompilés

L'application utilise des expressions régulières compilées pour une performance optimale :
- `SentenceSplitPattern` : Division en phrases
- `PassiveVoicePattern` : Détection voix passive
- `SubjectVerbPattern` : Identification sujet-verbe
- `WhitespaceNormalizer` : Normalisation des espaces
- `DoubleDotsRemover` : Élimination des doubles points

## Sécurité et Confidentialité

### Limitations
- **Pas de garantie d'anonymat absolu** : StylometryAnonymizer rend l'analyse stylométrique plus difficile mais ne garantit pas un anonymat total
- **Contexte important** : L'efficacité dépend de la qualité du dictionnaire de synonymes
- **Révision recommandée** : Il est conseillé de relire les variations générées

### Bonnes Pratiques
1. Utilisez un dictionnaire de synonymes riche et contextuel
2. Générez plusieurs variations et sélectionnez la plus naturelle
3. Combinez avec d'autres techniques (paraphrase manuelle, modification du ton)
4. Testez avec des outils d'analyse stylométrique pour valider l'efficacité

## Développement

### Structure du Projet
```
StylometryAnonymizer/
├── TextAnonymizer.cs          // Classe principale
├── AnonymizationOptions.cs    // Configuration
├── synonyms.json              // Dictionnaire de synonymes
├── README.md                  // Documentation
└── LICENSE                    // Licence MIT
```

## Performance

### Benchmarks

Sur un texte de 1000 mots avec un dictionnaire de 500 synonymes :

| Opération | Temps moyen | Allocations |
|-----------|-------------|-------------|
| 1 variation | ~15ms | ~50KB |
| 10 variations | ~120ms | ~450KB |
| 100 variations | ~1.1s | ~4.5MB |

*Testé sur Intel i7-9700K, 16GB RAM*

## Licence

Ce projet est sous licence MIT. Voir le fichier ([LICENSE](https://github.com/icaza/Ostium-Osint-Browser/blob/feature%2314/StylometryAnonymizer/LICENSE.txt)) pour plus de détails.

## 👤 Auteur

**ICAZA**
- GitHub: [@icaza](https://github.com/icaza)

## Ressources

### Stylométrie et Anonymisation
- [wikipedia](https://en.wikipedia.org/wiki/Stylometry)

### Documentation Technique
- [Newtonsoft.Json Documentation](https://www.newtonsoft.com/json/help/html/Introduction.htm)
- [.NET Framework 4.8.1](https://docs.microsoft.com/en-us/dotnet/framework/)
- [Regex Performance in .NET](https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices)

---
