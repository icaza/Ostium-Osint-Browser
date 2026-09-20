'use strict';

const http = require('http');
const fs = require('fs');
const fsp = fs.promises;
const path = require('path');

const PORT = process.env.PORT ? Number(process.env.PORT) : 6556;
const ROOT = __dirname;
const REPORTS_DIR = path.join(ROOT, '..', 'Reports_');
const PUBLIC_DIR = path.join(ROOT, 'public');

const MIME = {
  '.html': 'text/html; charset=utf-8',
  '.htm':  'text/html; charset=utf-8',
  '.css':  'text/css; charset=utf-8',
  '.js':   'application/javascript; charset=utf-8',
  '.json': 'application/json; charset=utf-8',
  '.svg':  'image/svg+xml',
  '.png':  'image/png',
  '.jpg':  'image/jpeg',
  '.jpeg': 'image/jpeg',
  '.gif':  'image/gif',
  '.webp': 'image/webp',
  '.ico':  'image/x-icon',
  '.woff2':'font/woff2',
  '.map':  'application/json; charset=utf-8',
};

// Filename pattern: YYYY_MM_DD_HH_MM_SS_mmm_<guid>.html
const NAME_RE = /^(\d{4})_(\d{2})_(\d{2})_(\d{2})_(\d{2})_(\d{2})_(\d{3})_([0-9a-fA-F]{8,})\.html?$/i;

/* ---------- Helpers ---------- */

function send(res, status, body, headers = {}) {
  res.writeHead(status, {
    'X-Content-Type-Options': 'nosniff',
    'Referrer-Policy': 'no-referrer',
    ...headers,
  });
  res.end(body);
}

function isInside(parent, child) {
  const rel = path.relative(parent, child);
  return rel && !rel.startsWith('..') && !path.isAbsolute(rel);
}

async function listReports() {
  let entries;
  try {
    entries = await fsp.readdir(REPORTS_DIR);
  } catch {
    return [];
  }
  const out = [];
  for (const name of entries) {
    if (!NAME_RE.test(name)) continue;
    try {
      const st = await fsp.stat(path.join(REPORTS_DIR, name));
      if (!st.isFile()) continue;
      out.push({ name, size: st.size, mtime: Math.floor(st.mtimeMs) });
    } catch { /* ignore */ }
  }
  return out;
}

/**
 * Serves a file with strict no-cache headers.
 * This guarantees CSS/JS edits are picked up immediately during development.
 */
async function serveFile(res, filePath) {
  try {
    const data = await fsp.readFile(filePath);
    const ext = path.extname(filePath).toLowerCase();
    send(res, 200, data, {
      'Content-Type': MIME[ext] || 'application/octet-stream',
      'Cache-Control': 'no-cache, no-store, must-revalidate',
      'Pragma': 'no-cache',
      'Expires': '0',
    });
  } catch {
    send(res, 404, 'Not Found', { 'Content-Type': 'text/plain; charset=utf-8' });
  }
}

/* ---------- Server ---------- */

const server = http.createServer(async (req, res) => {
  let pathname;
  try {
    pathname = decodeURIComponent(new URL(req.url, 'http://127.0.0.1').pathname);
  } catch {
    return send(res, 400, 'Bad Request', { 'Content-Type': 'text/plain' });
  }

  // --- API ---
  if (pathname === '/api/reports') {
    const list = await listReports();
    return send(res, 200, JSON.stringify(list), {
      'Content-Type': 'application/json',
      'Cache-Control': 'no-store',
    });
  }

  if (pathname === '/api/health') {
    return send(res, 200, '{"ok":true}', {
      'Content-Type': 'application/json',
      'Cache-Control': 'no-store',
    });
  }

  // --- Reports (HTML pages) ---
  if (pathname.startsWith('/reports/')) {
    const name = pathname.slice('/reports/'.length);
    if (!NAME_RE.test(name) || name.includes('/') || name.includes('\\') || name.includes('..')) {
      return send(res, 400, 'Bad Request', { 'Content-Type': 'text/plain' });
    }
    const full = path.resolve(REPORTS_DIR, name);
    if (!isInside(REPORTS_DIR, full)) {
      return send(res, 403, 'Forbidden', { 'Content-Type': 'text/plain' });
    }
    return serveFile(res, full);
  }

  // --- Static assets ---
  const rel = pathname === '/' ? 'index.html' : pathname.replace(/^\/+/, '');
  const full = path.resolve(PUBLIC_DIR, rel);
  if (!isInside(PUBLIC_DIR, full) && full !== PUBLIC_DIR) {
    return send(res, 403, 'Forbidden', { 'Content-Type': 'text/plain' });
  }
  return serveFile(res, full);
});

server.listen(PORT, '127.0.0.1', () => {
  console.log('');
  console.log('  \x1b[36m◈  OSINT Watcher — Reports Viewer\x1b[0m');
  console.log(`     http://127.0.0.1:${PORT}`);
  console.log(`     Reports dir: ${REPORTS_DIR}`);
  console.log('');
  console.log('  Press Ctrl+C to stop.');
  console.log('');
});