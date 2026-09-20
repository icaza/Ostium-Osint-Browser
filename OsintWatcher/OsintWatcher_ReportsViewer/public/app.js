'use strict';

const $ = (id) => document.getElementById(id);

const els = {
  list:       $('list'),
  search:     $('search'),
  refresh:    $('refresh'),
  count:      $('count'),
  status:     $('status'),
  frame:      $('frame'),
  empty:      $('empty'),
  viewerName: $('viewer-name'),
  openExt:    $('open-external'),
};

const state = {
  reports:  [],
  filtered: [],
  selected: null,
  filter:   '',
  loading:  false,
};

const NAME_RE = /^(\d{4})_(\d{2})_(\d{2})_(\d{2})_(\d{2})_(\d{2})_(\d{3})_([0-9a-fA-F]{8,})\.html?$/i;

/* ============================================================
   Utilities
   ============================================================ */

function parseName(name) {
  const m = NAME_RE.exec(name);
  if (!m) return null;
  const [, y, mo, d, h, mi, s, ms, guid] = m;
  const date = new Date(+y, +mo - 1, +d, +h, +mi, +s, +ms);
  if (Number.isNaN(date.getTime())) return null;
  return {
    name,
    guid,
    date,
    ts:  date.getTime(),
    ymd: `${y}-${mo}-${d}`,
  };
}

const timeFmt = new Intl.DateTimeFormat('en-US', {
  hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false,
});
const dayFmt = new Intl.DateTimeFormat('en-US', {
  weekday: 'long', year: 'numeric', month: 'long', day: 'numeric',
});

function timeAgo(ts) {
  const s = Math.max(0, Math.floor((Date.now() - ts) / 1000));
  if (s < 60)  return `${s}s ago`;
  const m = Math.floor(s / 60);
  if (m < 60)  return `${m}m ago`;
  const h = Math.floor(m / 60);
  if (h < 24)  return `${h}h ago`;
  const d = Math.floor(h / 24);
  if (d < 30)  return `${d}d ago`;
  const mo = Math.floor(d / 30);
  if (mo < 12) return `${mo}mo ago`;
  return `${Math.floor(mo / 12)}y ago`;
}

function escapeHtml(str) {
  return str.replace(/[&<>"']/g, (c) => ({
    '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;',
  }[c]));
}

function setStatus(text, kind = '') {
  els.status.textContent = text;
  els.status.className = 'status' + (kind ? ' ' + kind : '');
}

/* ============================================================
   Data loading
   ============================================================ */

async function loadReports({ silent = false } = {}) {
  if (state.loading) return;
  state.loading = true;
  if (!silent) setStatus('Loading…');

  try {
    const res = await fetch('/api/reports', { cache: 'no-store' });
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const data = await res.json();

    state.reports = data
      .map((r) => {
        const p = parseName(r.name);
        return p ? { ...p, size: r.size } : null;
      })
      .filter(Boolean)
      .sort((a, b) => b.ts - a.ts);

    setStatus(
      `${state.reports.length} report${state.reports.length === 1 ? '' : 's'}`,
      'ok'
    );
    render();
  } catch (err) {
    console.error(err);
    setStatus('Connection error', 'err');
  } finally {
    state.loading = false;
  }
}

/* ============================================================
   Rendering
   ============================================================ */

function render() {
  const q = state.filter.trim().toLowerCase();
  state.filtered = q
    ? state.reports.filter(
        (r) =>
          r.name.toLowerCase().includes(q) ||
          r.guid.toLowerCase().includes(q) ||
          r.ymd.includes(q)
      )
    : state.reports;

  els.count.textContent = q
    ? `${state.filtered.length} / ${state.reports.length}`
    : String(state.reports.length);

  if (!state.filtered.length) {
    const div = document.createElement('div');
    div.className = 'no-results';
    div.textContent = state.reports.length ? 'No matching reports' : 'No reports found';
    els.list.replaceChildren(div);
    return;
  }

  // Group by calendar day
  const groups = new Map();
  for (const r of state.filtered) {
    let g = groups.get(r.ymd);
    if (!g) { g = []; groups.set(r.ymd, g); }
    g.push(r);
  }

  const frag = document.createDocumentFragment();

  for (const [, items] of groups) {
    const groupEl = document.createElement('div');
    groupEl.className = 'group';

    const header = document.createElement('div');
    header.className = 'group-header';
    header.textContent = dayFmt.format(items[0].date);
    groupEl.appendChild(header);

    for (const r of items) {
      const btn = document.createElement('button');
      btn.type = 'button';
      btn.className = 'item' + (state.selected === r.name ? ' active' : '');
      btn.dataset.name = r.name;
      btn.title = r.name;
      btn.innerHTML =
        `<div class="item-time">${timeFmt.format(r.date)}</div>` +
        `<div class="item-meta">` +
          `<div class="item-guid">${escapeHtml(r.guid.slice(0, 12))}</div>` +
          `<div class="item-ago">${timeAgo(r.ts)}</div>` +
        `</div>`;
      btn.addEventListener('click', () => select(r.name));
      groupEl.appendChild(btn);
    }

    frag.appendChild(groupEl);
  }

  els.list.replaceChildren(frag);
}

/* ============================================================
   Selection — uses .hidden class, never the HTML hidden attribute
   ============================================================ */

function select(name) {
  const r = state.reports.find((x) => x.name === name);
  if (!r) return;

  state.selected = name;

  // Sidebar: update active state without a full re-render
  const prev = els.list.querySelector('.item.active');
  if (prev) prev.classList.remove('active');
  const next = els.list.querySelector(`.item[data-name="${CSS.escape(name)}"]`);
  if (next) {
    next.classList.add('active');
    next.scrollIntoView({ block: 'nearest' });
  }

  // Viewer: toggle via classes — bulletproof against display overrides
  els.empty.classList.add('hidden');
  els.frame.classList.remove('hidden');

  // Set src AFTER unhiding so the iframe can size itself correctly
  els.frame.src = `/reports/${encodeURIComponent(name)}`;

  els.viewerName.textContent = name;
  els.viewerName.classList.add('has-file');
  els.openExt.disabled = false;
}

/* ============================================================
   Events
   ============================================================ */

els.search.addEventListener('input', (e) => {
  state.filter = e.target.value;
  render();
});

els.refresh.addEventListener('click', () => loadReports());

els.openExt.addEventListener('click', () => {
  if (!state.selected) return;
  window.open(`/reports/${encodeURIComponent(state.selected)}`, '_blank', 'noopener');
});

document.addEventListener('keydown', (e) => {
  const inSearch = e.target === els.search;

  // Ctrl+K → focus search
  if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
    e.preventDefault();
    els.search.focus();
    els.search.select();
    return;
  }

  // Escape → clear search / blur
  if (e.key === 'Escape' && inSearch) {
    if (els.search.value) {
      els.search.value = '';
      state.filter = '';
      render();
    } else {
      els.search.blur();
    }
    return;
  }

  // Arrow navigation (when not in search field)
  if ((e.key === 'ArrowDown' || e.key === 'ArrowUp') && !inSearch) {
    e.preventDefault();
    moveSelection(e.key === 'ArrowDown' ? 1 : -1);
    return;
  }

  // R → refresh (outside inputs)
  if (!inSearch && (e.key === 'r' || e.key === 'R') && !e.ctrlKey && !e.metaKey) {
    const tag = document.activeElement?.tagName;
    if (tag !== 'INPUT' && tag !== 'TEXTAREA') {
      e.preventDefault();
      loadReports();
    }
  }
});

function moveSelection(delta) {
  const list = state.filtered;
  if (!list.length) return;
  const idx = list.findIndex((r) => r.name === state.selected);
  const next = idx === -1
    ? (delta > 0 ? 0 : list.length - 1)
    : Math.max(0, Math.min(list.length - 1, idx + delta));
  if (next !== idx) select(list[next].name);
}

/* ============================================================
   Boot
   ============================================================ */

loadReports();

// Poll for new reports (cheap JSON). Pause when the tab is hidden.
const poll = setInterval(() => {
  if (!document.hidden) loadReports({ silent: true });
}, 20_000);

document.addEventListener('visibilitychange', () => {
  if (!document.hidden) loadReports({ silent: true });
});

window.addEventListener('beforeunload', () => clearInterval(poll));