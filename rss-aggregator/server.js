/**
 * RSS Aggregator - Serveur Node.js
 * Modules natifs uniquement — aucune dépendance npm
 */

const http = require('http');
const https = require('https');
const fs = require('fs');
const path = require('path');

const PORT = 8888;
const DATA_FILE     = path.join(__dirname, 'feeds.json');
const USERDATA_FILE = path.join(__dirname, 'userdata.json');

// ── Persistance utilisateur (lus, favoris) ────────────────────────────────

function loadUserData() {
  if (!fs.existsSync(USERDATA_FILE)) {
    var empty = { starred: [], read: [], prefs: {} };
    fs.writeFileSync(USERDATA_FILE, JSON.stringify(empty, null, 2));
    return empty;
  }
  try {
    return JSON.parse(fs.readFileSync(USERDATA_FILE, 'utf8'));
  } catch(e) {
    console.error('userdata.json corrompu, réinitialisation');
    fs.unlinkSync(USERDATA_FILE);
    return loadUserData();
  }
}

function saveUserData(data) {
  fs.writeFileSync(USERDATA_FILE, JSON.stringify(data, null, 2));
}

// ── Flux sauvegardés ──────────────────────────────────────────────────────

function loadFeeds() {
  if (!fs.existsSync(DATA_FILE)) {
    const defaults = [
      { id: 1, name: 'Le Monde', url: 'https://www.lemonde.fr/rss/une.xml', category: 'Actualités', color: '#1a73e8' },
      { id: 2, name: 'Hacker News', url: 'https://news.ycombinator.com/rss', category: 'Tech', color: '#ff6600' },
      { id: 3, name: 'NASA News', url: 'https://www.nasa.gov/rss/dyn/breaking_news.rss', category: 'Science', color: '#0b3d91' },
    ];
    fs.writeFileSync(DATA_FILE, JSON.stringify(defaults, null, 2));
    return defaults;
  }
  try {
    return JSON.parse(fs.readFileSync(DATA_FILE, 'utf8'));
  } catch(e) {
    console.error('feeds.json corrompu, réinitialisation');
    fs.unlinkSync(DATA_FILE);
    return loadFeeds();
  }
}

function saveFeeds(feeds) {
  fs.writeFileSync(DATA_FILE, JSON.stringify(feeds, null, 2));
}

// ── Fetch HTTP/HTTPS ──────────────────────────────────────────────────────

function fetchUrl(rawUrl, redirectCount) {
  redirectCount = redirectCount || 0;
  if (redirectCount > 5) return Promise.reject(new Error('Trop de redirections'));

  return new Promise(function(resolve, reject) {
    var parsed;
    try { parsed = new URL(rawUrl); }
    catch(e) { return reject(new Error('URL invalide : ' + rawUrl)); }

    var lib = parsed.protocol === 'https:' ? https : http;
    var reqPath = (parsed.pathname || '/') + (parsed.search || '');

    var options = {
      hostname: parsed.hostname,
      port: parsed.port ? parseInt(parsed.port) : (parsed.protocol === 'https:' ? 443 : 80),
      path: reqPath,
      method: 'GET',
      headers: {
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36',
        'Accept': 'text/html,application/xhtml+xml,application/xml,application/rss+xml,*/*',
        'Accept-Language': 'fr-FR,fr;q=0.9,en;q=0.8',
        'Cache-Control': 'no-cache',
      },
      timeout: 15000,
      rejectUnauthorized: false,
    };

    console.log('[fetch] ' + options.hostname + reqPath);

    var req = lib.request(options, function(res) {
      console.log('[' + res.statusCode + '] ' + options.hostname);

      if ([301,302,303,307,308].indexOf(res.statusCode) !== -1 && res.headers.location) {
        res.resume();
        var nextUrl = res.headers.location;
        if (!nextUrl.startsWith('http')) nextUrl = parsed.protocol + '//' + parsed.hostname + nextUrl;
        return fetchUrl(nextUrl, redirectCount + 1).then(resolve).catch(reject);
      }

      if (res.statusCode >= 400) {
        res.resume();
        return reject(new Error('HTTP ' + res.statusCode));
      }

      var chunks = [];
      res.on('data', function(chunk) { chunks.push(chunk); });
      res.on('end', function() { resolve(Buffer.concat(chunks).toString('utf8')); });
      res.on('error', reject);
    });

    req.on('error', function(e) { reject(new Error(options.hostname + ' : ' + e.message)); });
    req.on('timeout', function() { req.destroy(); reject(new Error('Timeout sur ' + options.hostname)); });
    req.end();
  });
}

// ── Parser RSS/Atom ───────────────────────────────────────────────────────

function extractTag(xml, tag) {
  var cdataRe = new RegExp('<' + tag + '[^>]*><!\\[CDATA\\[([\\s\\S]*?)\\]\\]><\\/' + tag + '>', 'i');
  var cm = xml.match(cdataRe);
  if (cm) return cm[1];
  var re = new RegExp('<' + tag + '[^>]*>([\\s\\S]*?)<\\/' + tag + '>', 'i');
  var m = xml.match(re);
  return m ? m[1] : null;
}

function extractAttr(xml, tag, attr) {
  var re = new RegExp('<' + tag + '[^>]*\\s' + attr + '=["\']([^"\']+)["\']', 'i');
  var m = xml.match(re);
  return m ? m[1] : null;
}

function extractSelfClosingLink(xml) {
  var m = xml.match(/<link[^>]+href=["']([^"']+)["'][^>]*\/?>/i);
  return m ? m[1] : null;
}

function stripHtml(str) {
  if (!str) return '';
  return str
    .replace(/<[^>]+>/g, ' ')
    .replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>')
    .replace(/&quot;/g, '"').replace(/&#39;/g, "'").replace(/&apos;/g, "'")
    .replace(/&nbsp;/g, ' ').replace(/&#\d+;/g, '')
    .replace(/\s+/g, ' ').trim();
}

function parseRSS(xml) {
  if (!xml || xml.length < 50) return { title: 'Feed vide', items: [] };
  var items = [];
  var itemRe = /<(item|entry)[\s>]([\s\S]*?)<\/(item|entry)>/gi;
  var match;
  while ((match = itemRe.exec(xml)) !== null) {
    var block = match[2];
    var title = extractTag(block, 'title');
    var link = extractTag(block, 'link') || extractSelfClosingLink(block) || extractAttr(block, 'link', 'href');
    var pubDate = extractTag(block, 'pubDate') || extractTag(block, 'published') || extractTag(block, 'updated') || extractTag(block, 'dc:date');
    var description = extractTag(block, 'description') || extractTag(block, 'summary') || extractTag(block, 'content:encoded') || extractTag(block, 'content');
    var author = extractTag(block, 'dc:creator') || extractTag(block, 'author') || extractTag(block, 'name') || '';
    if (!title) continue;
    var dateStr = pubDate ? new Date(pubDate).toISOString() : new Date().toISOString();
    if (isNaN(new Date(dateStr))) dateStr = new Date().toISOString();
    items.push({
      title: stripHtml(title),
      link: link ? link.trim().replace(/\s/g, '') : '#',
      date: dateStr,
      description: description ? stripHtml(description).substring(0, 300) : '',
      author: author ? stripHtml(author).substring(0, 80) : '',
    });
  }
  var feedTitle = extractTag(xml, 'title') || 'Feed';
  console.log('[parse] ' + items.length + ' articles, titre: ' + stripHtml(feedTitle));
  return { title: stripHtml(feedTitle), items: items.slice(0, 50) };
}

// ── Extracteur de contenu (mode lecture) ─────────────────────────────────
// Extrait le contenu principal d'une page HTML sans dépendances

function extractReadableContent(html, baseUrl) {
  // 1. Titre
  var titleMatch = html.match(/<title[^>]*>([^<]+)<\/title>/i);
  var title = titleMatch ? stripHtml(titleMatch[1]) : '';

  // Préférer og:title si disponible
  var ogTitle = html.match(/<meta[^>]+property=["']og:title["'][^>]+content=["']([^"']+)["']/i)
             || html.match(/<meta[^>]+content=["']([^"']+)["'][^>]+property=["']og:title["']/i);
  if (ogTitle) title = stripHtml(ogTitle[1]);

  // 2. Image principale
  var ogImage = html.match(/<meta[^>]+property=["']og:image["'][^>]+content=["']([^"']+)["']/i)
              || html.match(/<meta[^>]+content=["']([^"']+)["'][^>]+property=["']og:image["']/i);
  var heroImage = ogImage ? ogImage[1] : null;

  // 3. Description/résumé
  var ogDesc = html.match(/<meta[^>]+(?:name=["']description["']|property=["']og:description["'])[^>]+content=["']([^"']+)["']/i)
             || html.match(/<meta[^>]+content=["']([^"']+)["'][^>]+(?:name=["']description["']|property=["']og:description["'])/i);
  var description = ogDesc ? stripHtml(ogDesc[1]) : '';

  // 4. Auteur
  var authorMeta = html.match(/<meta[^>]+name=["']author["'][^>]+content=["']([^"']+)["']/i)
                || html.match(/<meta[^>]+content=["']([^"']+)["'][^>]+name=["']author["']/i);
  var author = authorMeta ? stripHtml(authorMeta[1]) : '';

  // 5. Date de publication
  var dateMeta = html.match(/<meta[^>]+(?:name=["'](?:article:published_time|pubdate)["']|property=["']article:published_time["'])[^>]+content=["']([^"']+)["']/i)
               || html.match(/(?:datePublished|publishedAt|published_time)["\s:]+["']([0-9T:\-Z+]+)["']/i);
  var publishedDate = dateMeta ? dateMeta[1] : '';

  // 6. Contenu principal — heuristiques par ordre de priorité
  var content = '';

  // Essayer les balises sémantiques connues
  var contentSelectors = [
    /<article[^>]*>([\s\S]*?)<\/article>/i,
    /<main[^>]*>([\s\S]*?)<\/main>/i,
    /<div[^>]+class=["'][^"']*(?:article|post|content|story|body|text|entry)[^"']*["'][^>]*>([\s\S]*?)<\/div>/i,
    /<div[^>]+id=["'][^"']*(?:article|post|content|story|body|text|entry)[^"']*["'][^>]*>([\s\S]*?)<\/div>/i,
  ];

  for (var i = 0; i < contentSelectors.length; i++) {
    var cm = html.match(contentSelectors[i]);
    if (cm && cm[1].length > 200) {
      content = cm[1];
      break;
    }
  }

  // Fallback : prendre le body entier
  if (!content) {
    var bodyMatch = html.match(/<body[^>]*>([\s\S]*?)<\/body[^>]*>/i);
    content = bodyMatch ? bodyMatch[1] : html;
  }

  // 7. Nettoyer le contenu HTML — supprimer les éléments parasites
  content = content
    // Scripts et styles
    .replace(/<script[\s\S]*?<\/script[^>]*>/gi, '')
    .replace(/<style[\s\S]*?<\/style[^>]*>/gi, '')
    .replace(/<noscript[\s\S]*?<\/noscript[^>]*>/gi, '')
    // Nav, footer, aside, pub
    .replace(/<nav[\s\S]*?<\/nav[^>]*>/gi, '')
    .replace(/<footer[\s\S]*?<\/footer[^>]*>/gi, '')
    .replace(/<aside[\s\S]*?<\/aside[^>]*>/gi, '')
    .replace(/<header[\s\S]*?<\/header[^>]*>/gi, '')
    .replace(/<form[\s\S]*?<\/form[^>]*>/gi, '')
    .replace(/<iframe[\s\S]*?<\/iframe[^>]*>/gi, '')
    .replace(/<svg[\s\S]*?<\/svg[^>]*>/gi, '')
    // Attributs dangereux
    .replace(/\s*on\w+="[^"]*"/gi, '')
    .replace(/\s*on\w+='[^']*'/gi, '')
    // Réécrire les URLs relatives des images en absolues
    .replace(/(<img[^>]+src=["'])(?!http)([^"']+)(["'])/gi, function(m, pre, src, post) {
      if (src.startsWith('//')) return pre + 'https:' + src + post;
      if (src.startsWith('/') && baseUrl) {
        try { return pre + new URL(src, baseUrl).href + post; } catch(e) {}
      }
      return m;
    });

  return {
    title: title,
    description: description,
    author: author,
    publishedDate: publishedDate,
    heroImage: heroImage,
    content: content,
  };
}

// ── Helpers HTTP ──────────────────────────────────────────────────────────

function jsonResp(res, data, status) {
  if (res.headersSent) return;
  res.writeHead(status || 200, { 'Content-Type': 'application/json; charset=utf-8' });
  res.end(JSON.stringify(data));
}

function readBody(req) {
  return new Promise(function(resolve) {
    var b = '';
    req.on('data', function(c) { b += c; });
    req.on('end', function() { resolve(b); });
  });
}

// ── Serveur ───────────────────────────────────────────────────────────────

var server = http.createServer(function(req, res) {
  var reqUrl = new URL(req.url, 'http://localhost:' + PORT);
  var pathname = reqUrl.pathname;

  res.setHeader('Access-Control-Allow-Origin', '*');
  res.setHeader('Access-Control-Allow-Methods', 'GET, POST, DELETE, OPTIONS');
  res.setHeader('Access-Control-Allow-Headers', 'Content-Type');
  if (req.method === 'OPTIONS') { res.writeHead(204); res.end(); return; }

  console.log(req.method + ' ' + pathname);

  // ── GET /api/feeds
  if (pathname === '/api/feeds' && req.method === 'GET') {
    return jsonResp(res, loadFeeds());
  }

  // ── POST /api/feeds
  if (pathname === '/api/feeds' && req.method === 'POST') {
    readBody(req).then(function(body) {
      try {
        var data = JSON.parse(body);
        if (!data.url) return jsonResp(res, { error: 'URL requise' }, 400);
        var feeds = loadFeeds();
        var newFeed = { id: Date.now(), name: data.name || data.url, url: data.url, category: data.category || 'Général', color: data.color || '#6366f1' };
        feeds.push(newFeed);
        saveFeeds(feeds);
        jsonResp(res, newFeed, 201);
      } catch(e) { jsonResp(res, { error: e.message }, 400); }
    });
    return;
  }

  // ── DELETE /api/feeds/:id
  if (pathname.startsWith('/api/feeds/') && req.method === 'DELETE') {
    var id = parseInt(pathname.split('/')[3]);
    var feeds = loadFeeds().filter(function(f) { return f.id !== id; });
    saveFeeds(feeds);
    return jsonResp(res, { success: true });
  }

  // ── GET /api/fetch?url=...
  if (pathname === '/api/fetch' && req.method === 'GET') {
    var feedUrl = reqUrl.searchParams.get('url');
    if (!feedUrl) return jsonResp(res, { error: 'URL manquante' }, 400);
    fetchUrl(feedUrl)
      .then(function(xml) { jsonResp(res, parseRSS(xml)); })
      .catch(function(e) { jsonResp(res, { error: e.message }, 500); });
    return;
  }

  // ── GET /api/all
  if (pathname === '/api/all' && req.method === 'GET') {
    var feeds = loadFeeds();
    if (feeds.length === 0) return jsonResp(res, []);
    var promises = feeds.map(function(feed) {
      return fetchUrl(feed.url)
        .then(function(xml) {
          var parsed = parseRSS(xml);
          return parsed.items.map(function(item) {
            return Object.assign({}, item, { feedId: feed.id, feedName: feed.name, feedColor: feed.color, feedCategory: feed.category });
          });
        })
        .catch(function(e) { console.error('[erreur] ' + feed.name + ' : ' + e.message); return []; });
    });
    Promise.all(promises).then(function(results) {
      var allItems = [];
      results.forEach(function(arr) { allItems = allItems.concat(arr); });
      allItems.sort(function(a, b) { return new Date(b.date) - new Date(a.date); });
      jsonResp(res, allItems.slice(0, 150));
    }).catch(function(e) { jsonResp(res, { error: e.message }, 500); });
    return;
  }

  // ── GET /api/extract?url=... — NOUVEAU : extraction mode lecture
  if (pathname === '/api/extract' && req.method === 'GET') {
    var articleUrl = reqUrl.searchParams.get('url');
    if (!articleUrl) return jsonResp(res, { error: 'URL manquante' }, 400);
    console.log('[extract] ' + articleUrl);
    fetchUrl(articleUrl)
      .then(function(html) {
        var result = extractReadableContent(html, articleUrl);
        result.url = articleUrl;
        jsonResp(res, result);
      })
      .catch(function(e) { jsonResp(res, { error: e.message }, 500); });
    return;
  }

  // ── GET /reader.html — page de lecture
  if (pathname === '/reader.html') {
    var readerFile = path.join(__dirname, 'reader.html');
    try {
      var html = fs.readFileSync(readerFile);
      res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
      res.end(html);
    } catch(e) { res.writeHead(500); res.end('reader.html introuvable'); }
    return;
  }

  // ── GET / — interface principale
  if (pathname === '/' || pathname === '/index.html') {
    var indexFile = path.join(__dirname, 'index.html');
    try {
      var html = fs.readFileSync(indexFile);
      res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
      res.end(html);
    } catch(e) { res.writeHead(500); res.end('index.html introuvable'); }
    return;
  }

  // ── GET /api/userdata — récupérer lus + favoris + prefs
  if (pathname === '/api/userdata' && req.method === 'GET') {
    var ud = loadUserData();
    // Migrer ancien format starred (strings) vers objets
    if (ud.starred.length > 0 && typeof ud.starred[0] === 'string') {
      ud.starred = ud.starred.map(function(l) { return { link: l }; });
      saveUserData(ud);
    }
    return jsonResp(res, {
      starred:         ud.starred.map(function(s) { return s.link; }),
      starredArticles: ud.starred,
      read:            ud.read,
      prefs:           ud.prefs,
    });
  }

  // ── POST /api/userdata/prefs — sauvegarder les préférences (thème, taille)
  if (pathname === '/api/userdata/prefs' && req.method === 'POST') {
    readBody(req).then(function(body) {
      try {
        var prefs = JSON.parse(body);
        var ud = loadUserData();
        ud.prefs = Object.assign(ud.prefs || {}, prefs);
        saveUserData(ud);
        jsonResp(res, { prefs: ud.prefs });
      } catch(e) { jsonResp(res, { error: e.message }, 400); }
    });
    return;
  }

  // ── POST /api/userdata/starred — toggle favori (stocke l'article complet)
  if (pathname === '/api/userdata/starred' && req.method === 'POST') {
    readBody(req).then(function(body) {
      try {
        var data = JSON.parse(body);
        var link = data.link;
        if (!link) return jsonResp(res, { error: 'link requis' }, 400);
        var ud = loadUserData();
        // Migrer l'ancien format (tableau de strings) vers le nouveau (tableau d'objets)
        if (ud.starred.length > 0 && typeof ud.starred[0] === 'string') {
          ud.starred = ud.starred.map(function(l) { return { link: l }; });
        }
        var idx = ud.starred.findIndex(function(s) { return s.link === link; });
        var action;
        if (idx >= 0) { ud.starred.splice(idx, 1); action = 'removed'; }
        else {
          // Stocker l'article complet (title, description, date, feedName, etc.)
          var article = {
            link:         link,
            title:        data.title        || '',
            description:  data.description  || '',
            date:         data.date         || new Date().toISOString(),
            author:       data.author       || '',
            feedId:       data.feedId       || null,
            feedName:     data.feedName     || '',
            feedColor:    data.feedColor    || '#f0c040',
            feedCategory: data.feedCategory || '',
          };
          ud.starred.push(article);
          action = 'added';
        }
        saveUserData(ud);
        // Retourner les liens pour la compatibilité avec le front
        var starredLinks = ud.starred.map(function(s) { return s.link; });
        jsonResp(res, { action: action, starred: starredLinks, starredArticles: ud.starred });
      } catch(e) { jsonResp(res, { error: e.message }, 400); }
    });
    return;
  }

  // ── POST /api/userdata/read — marquer lu (un ou plusieurs)
  if (pathname === '/api/userdata/read' && req.method === 'POST') {
    readBody(req).then(function(body) {
      try {
        var data = JSON.parse(body);
        // Accepte { link } ou { links: [...] }
        var links = data.links || (data.link ? [data.link] : []);
        if (!links.length) return jsonResp(res, { error: 'link(s) requis' }, 400);
        var ud = loadUserData();
        links.forEach(function(l) {
          if (!ud.read.includes(l)) ud.read.push(l);
        });
        saveUserData(ud);
        jsonResp(res, { read: ud.read });
      } catch(e) { jsonResp(res, { error: e.message }, 400); }
    });
    return;
  }

  // ── DELETE /api/userdata/read/:encoded — dé-marquer un article lu
  if (pathname.startsWith('/api/userdata/read/') && req.method === 'DELETE') {
    try {
      var link = decodeURIComponent(pathname.replace('/api/userdata/read/', ''));
      var ud = loadUserData();
      ud.read = ud.read.filter(function(l) { return l !== link; });
      saveUserData(ud);
      jsonResp(res, { read: ud.read });
    } catch(e) { jsonResp(res, { error: e.message }, 400); }
    return;
  }

  jsonResp(res, { error: 'Route non trouvée : ' + pathname }, 404);
});

server.listen(PORT, function() {
  console.log('\n✅  RSS Aggregator démarré !');
  console.log('🌐  Ouvrez : http://localhost:' + PORT);
  console.log('\n    Ctrl+C pour arrêter\n');
});
