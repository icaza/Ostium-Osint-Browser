# Flux · RSS Aggregator

Complete RSS aggregator in **Node.js** — No npm dependencies required.

## Prerequisites

- Node.js 14+ installed on Windows 
  → https://nodejs.org/

## Quick Start

If you are using the tool via Ostium Osint Browser, click the "Start RSS2" button to start the local server and then click the "Localhost" button to access it.

```bash
# 1. Open a terminal in this folder
# 2. Start the server:
node server.js

# 3. Open in browser:
#    http://localhost:3000
```

## Features

| Features         | Description                                           |
|------------------|-------------------------------------------------------|
| 📡 Multi-feed    | Aggregate all your RSS/Atom feeds into a single view  |
| ➕ Add feeds     | Modal interface with custom name, category, and color |
| 🗑️ Delete        | Delete a feed with one click                          |
| 🔍 Search        | Real-time filtering by title, description, or source  |
| 📂 Categories    | Filter by category (Tech, News, Science…)             |
| ⭐ Favorites     | Bookmark articles for easy access                     |
| ⏰ "Today" view  | Articles published today                              |
| 🔄 Refresh       | Reload all feeds with one click                       |
| 💾 Persistence   | Feeds are saved in `feeds.json`                       |

## Architecture

```
rss-aggregator/
├── server.js      ← HTTP Server  + API REST + parser RSS natif
├── index.html     ← Full web interface (HTML/CSS/JS vanilla)
├── feeds.json     ← Feed database (auto-generated)
└── README.md
```

## API REST

| Method | Endpoint             | Description                           |
|--------|----------------------|---------------------------------------|
| GET    | `/api/feeds`         | List of all recorded streams          |
| POST   | `/api/feeds`         | Add a new feed                        |
| DELETE | `/api/feeds/:id`     | Delete a stream                       |
| GET    | `/api/all`           | Retrieves all articles sorted by date |
| GET    | `/api/fetch?url=...` | Fetch and parse a specific stream     |

## Change the port

in `server.js`, line 8 :
```js
const PORT = 3000; // ← here
```

## Default RSS feed

The `feeds.json` file is automatically created with 3 example feeds.
Delete it to start from scratch or edit it directly.

## Node.js modules used

- `http` — web server
- `https` — HTTPS request
- `fs` — read/write files
- `url` — URL parsing
- `path` — path management

**No npm installation required.**
