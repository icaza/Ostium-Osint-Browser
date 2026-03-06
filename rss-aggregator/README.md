# Flux · RSS Aggregator

Agrégateur RSS complet en **Node.js pur** — aucune dépendance npm requise.

## Prérequis

- Node.js 14+ installé sur Windows  
  → https://nodejs.org/fr/download/

## Démarrage rapide

```bash
# 1. Ouvrir un terminal dans ce dossier
# 2. Lancer le serveur :
node server.js

# 3. Ouvrir dans le navigateur :
#    http://localhost:3000
```

## Fonctionnalités

| Fonctionnalité | Description |
|---|---|
| 📡 Multi-flux | Agrégation de tous vos flux RSS/Atom en une seule vue |
| ➕ Ajout de flux | Interface modale avec nom, catégorie et couleur personnalisée |
| 🗑️ Suppression | Supprimer un flux en un clic |
| 🔍 Recherche | Filtrage en temps réel par titre, description ou source |
| 📂 Catégories | Filtrer par catégorie (Tech, Actualités, Science…) |
| ⭐ Favoris | Marquer des articles pour les retrouver facilement |
| ⏰ Vue "Aujourd'hui" | Articles publiés dans la journée |
| 🔄 Actualisation | Rechargement de tous les flux en un clic |
| 💾 Persistance | Les flux sont sauvegardés dans `feeds.json` |

## Architecture

```
rss-aggregator/
├── server.js      ← Serveur HTTP + API REST + parser RSS natif
├── index.html     ← Interface web complète (HTML/CSS/JS vanilla)
├── feeds.json     ← Base de données des flux (auto-généré)
└── README.md
```

## API REST

| Méthode | Endpoint | Description |
|---|---|---|
| GET | `/api/feeds` | Liste tous les flux enregistrés |
| POST | `/api/feeds` | Ajoute un nouveau flux |
| DELETE | `/api/feeds/:id` | Supprime un flux |
| GET | `/api/all` | Récupère tous les articles triés par date |
| GET | `/api/fetch?url=...` | Fetch et parse un flux spécifique |

## Changer le port

Dans `server.js`, ligne 8 :
```js
const PORT = 3000; // ← modifier ici
```

## Flux RSS par défaut

Le fichier `feeds.json` est créé automatiquement avec 3 flux exemples.
Supprimez-le pour repartir de zéro ou éditez-le directement.

## Modules Node.js utilisés

- `http` — serveur web
- `https` — requêtes HTTPS
- `fs` — lecture/écriture fichiers
- `url` — parsing d'URLs
- `path` — gestion des chemins

**Aucune installation npm nécessaire.**
