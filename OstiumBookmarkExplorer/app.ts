// app.ts
// Requires Deno 2.x (node:sqlite built-in)
// Run: deno run --allow-net --allow-read --allow-write --allow-env --allow-sys app.ts

// @ts-ignore
import { DatabaseSync } from "node:sqlite";

const PORT = 8000;
const MAX_FILE_SIZE = 100 * 1024 * 1024; // 100 MB
const MAX_QUERY_RESULTS = 10000;

let currentDb: DatabaseSync | null = null;
let currentDbPath = "";
let currentDbName = "";
let schema: TableInfo[] = [];
let selectedTable = "";
let selectedColumns = { url: "", name: "", date: "" };

interface ColumnInfo {
  name: string;
  type: string;
}

interface TableInfo {
  name: string;
  columns: ColumnInfo[];
  rowCount: number;
}

// ─── Validation Helpers ──────────────────────────────────────────────────

function validateIdentifier(name: string): boolean {
  if (!name || typeof name !== "string" || name.length > 255) return false;
  // SQLite identifiers: alphanumeric, underscore, but can't start with number
  return /^[a-zA-Z_][a-zA-Z0-9_]*$/.test(name);
}

function quoteIdent(name: string): string {
  if (!validateIdentifier(name)) {
    throw new Error(`Invalid identifier: ${name}`);
  }
  return `"${name}"`;
}

function escapeLike(value: string): string {
  return value.replace(/[\\%_]/g, "\\$&");
}

// ─── Column Detection ─────────────────────────────────────────────────────

function isUrlCol(n: string): boolean {
  const lower = n.toLowerCase();
  return (
    /\b(url|uri|adress|address|link|href|location)\b/.test(lower) &&
    !/\b(name|title|label|description|desc|date)\b/.test(lower)
  );
}

function isNameCol(n: string): boolean {
  const lower = n.toLowerCase();
  return (
    /\b(name|title|label|description|desc|nom)\b/.test(lower) ||
    lower === "url_name"
  );
}

function isDateCol(n: string): boolean {
  const lower = n.toLowerCase();
  return /\b(date|created|modified|added|last_visit|timestamp|time|updated|visited|visit_date)\b/.test(lower);
}

function detectColumns(cols: ColumnInfo[]): { url: string; name: string; date: string } {
  let url = "";
  let name = "";
  let date = "";
  for (const col of cols) {
    const lower = col.name.toLowerCase();
    if (!date && isDateCol(lower)) date = col.name;
    if (!name && isNameCol(lower)) name = col.name;
    if (!url && isUrlCol(lower)) url = col.name;
  }
  if (!name && url) name = url;
  return { url, name, date };
}

function discoverSchema(db: DatabaseSync): TableInfo[] {
  const tables = db
    .prepare("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'")
    .all() as { name: string }[];

  const result: TableInfo[] = [];
  for (const t of tables) {
    try {
      if (!validateIdentifier(t.name)) {
        console.warn(`Skipping table with invalid name: ${t.name}`);
        continue;
      }
      const quoted = quoteIdent(t.name);
      const columns = db.prepare(`PRAGMA table_info(${quoted})`).all() as ColumnInfo[];
      let rowCount = 0;
      try {
        const r = db.prepare(`SELECT COUNT(*) AS c FROM ${quoted}`).get() as { c: number };
        rowCount = Number(r.c);
      } catch (e) {
        console.warn(`Unable to count rows in ${t.name}:`, e);
        rowCount = 0;
      }
      result.push({ name: t.name, columns, rowCount });
    } catch (e) {
      console.error(`Skipping table ${t.name}:`, e);
    }
  }
  return result;
}

function scoreTable(t: TableInfo): number {
  let score = 0;
  const hasUrl = t.columns.some((c) => isUrlCol(c.name.toLowerCase()));
  const hasName = t.columns.some((c) => isNameCol(c.name.toLowerCase()));
  const hasDate = t.columns.some((c) => isDateCol(c.name.toLowerCase()));
  if (hasUrl) score += 20;
  if (hasName) score += 10;
  if (hasDate) score += 5;
  if (t.rowCount > 0) score += Math.min(t.rowCount / 100, 15);
  return score;
}

function autoSelectTable(tables: TableInfo[]): string {
  if (!tables.length) return "";
  let best = tables[0];
  let bestScore = -Infinity;
  for (const t of tables) {
    const s = scoreTable(t);
    if (s > bestScore) {
      bestScore = s;
      best = t;
    }
  }
  return best.name;
}

function getSearchableCols(
  selectedCols: { url: string; name: string; date: string },
  columns: ColumnInfo[],
): string[] {
  const set = new Set<string>();
  if (selectedCols.url && validateIdentifier(selectedCols.url)) {
    set.add(selectedCols.url);
  }
  if (selectedCols.name && validateIdentifier(selectedCols.name)) {
    set.add(selectedCols.name);
  }
  for (const col of columns) {
    if (!validateIdentifier(col.name)) continue;
    const lower = col.name.toLowerCase();
    if (/\b(title|description|desc|tags|keyword|note|comment|summary)\b/.test(lower)) {
      set.add(col.name);
    }
  }
  return Array.from(set);
}

// ─── Search Parser ────────────────────────────────────────────────────────

type QueryNode =
  | { kind: "term"; value: string }
  | { kind: "and"; left: QueryNode; right: QueryNode }
  | { kind: "or"; left: QueryNode; right: QueryNode }
  | { kind: "not"; child: QueryNode };

type Token =
  | { type: "term"; value: string; phrase?: boolean }
  | { type: "and" }
  | { type: "or" }
  | { type: "not" }
  | { type: "lparen" }
  | { type: "rparen" };

function tokenizeQuery(input: string): Token[] {
  const tokens: Token[] = [];
  let i = 0;
  const len = Math.min(input.length, 1000); // Limit query size
  
  while (i < len) {
    const ch = input[i];
    if (/\s/.test(ch)) {
      i++;
      continue;
    }
    if (ch === "(") {
      tokens.push({ type: "lparen" });
      i++;
      continue;
    }
    if (ch === ")") {
      tokens.push({ type: "rparen" });
      i++;
      continue;
    }
    if (ch === '"') {
      i++;
      let value = "";
      while (i < len && input[i] !== '"') {
        value += input[i];
        i++;
      }
      if (i < len) i++;
      if (value.length > 0) {
        tokens.push({ type: "term", value, phrase: true });
      }
      continue;
    }
    // negative prefix: -term
    if (ch === "-" && i + 1 < len && !/\s/.test(input[i + 1])) {
      const prev = i > 0 ? input[i - 1] : " ";
      if (/\s|[()]/.test(prev)) {
        tokens.push({ type: "not" });
        i++;
        continue;
      }
    }
    let word = "";
    while (i < len && !/[\s()"]/.test(input[i])) {
      word += input[i];
      i++;
    }
    if (word) {
      const upper = word.toUpperCase();
      if (upper === "AND") tokens.push({ type: "and" });
      else if (upper === "OR") tokens.push({ type: "or" });
      else if (upper === "NOT") tokens.push({ type: "not" });
      else tokens.push({ type: "term", value: word });
    }
  }
  return tokens;
}

class QueryParser {
  private tokens: Token[];
  private pos = 0;

  constructor(tokens: Token[]) {
    this.tokens = tokens;
  }

  private peek(): Token | undefined {
    return this.tokens[this.pos];
  }

  private eat(): Token | undefined {
    return this.tokens[this.pos++];
  }

  parse(): QueryNode {
    if (this.tokens.length === 0) return { kind: "term", value: "" };
    return this.parseOr();
  }

  private parseOr(): QueryNode {
    let left = this.parseAnd();
    while (this.peek()?.type === "or") {
      this.eat();
      const right = this.parseAnd();
      left = { kind: "or", left, right };
    }
    return left;
  }

  private parseAnd(): QueryNode {
    let left = this.parseNot();
    while (true) {
      if (this.peek()?.type === "and") {
        this.eat();
        const right = this.parseNot();
        left = { kind: "and", left, right };
      } else if (
        this.peek() &&
        (this.peek()!.type === "term" || this.peek()!.type === "not" || this.peek()!.type === "lparen")
      ) {
        const right = this.parseNot();
        left = { kind: "and", left, right };
      } else {
        break;
      }
    }
    return left;
  }

  private parseNot(): QueryNode {
    if (this.peek()?.type === "not") {
      this.eat();
      const child = this.parseNot();
      return { kind: "not", child };
    }
    if (this.peek()?.type === "lparen") {
      this.eat();
      const node = this.parseOr();
      if (this.peek()?.type === "rparen") this.eat();
      return node;
    }
    const token = this.eat();
    if (token && token.type === "term") {
      return { kind: "term", value: token.value };
    }
    return { kind: "term", value: "" };
  }
}

function splitField(value: string, columns: ColumnInfo[]): { field: string; term: string } {
  const idx = value.indexOf(":");
  if (idx > 0 && idx < value.length - 1) {
    const field = value.slice(0, idx);
    const term = value.slice(idx + 1);
    if (columns.some((c) => c.name.toLowerCase() === field.toLowerCase())) {
      return { field, term };
    }
  }
  return { field: "", term: value };
}

function termToSql(
  value: string,
  columns: ColumnInfo[],
  params: unknown[],
  searchableCols: string[],
): string {
  if (!value || typeof value !== "string" || value.length > 500) return "1=0";

  const { field, term } = splitField(value, columns);
  if (field) {
    const col = columns.find((c) => c.name.toLowerCase() === field.toLowerCase());
    if (!col || !validateIdentifier(col.name)) return "1=0";
    const pattern = `%${escapeLike(term)}%`;
    params.push(pattern);
    return `CAST(${quoteIdent(col.name)} AS TEXT) COLLATE NOCASE LIKE ? ESCAPE '\\'`;
  }

  if (searchableCols.length === 0) return "1=0";
  const parts: string[] = [];
  for (const colName of searchableCols) {
    if (!validateIdentifier(colName)) continue;
    const pattern = `%${escapeLike(value)}%`;
    params.push(pattern);
    parts.push(`CAST(${quoteIdent(colName)} AS TEXT) COLLATE NOCASE LIKE ? ESCAPE '\\'`);
  }
  return parts.length > 0 ? `(${parts.join(" OR ")})` : "1=0";
}

function nodeToSql(
  node: QueryNode,
  columns: ColumnInfo[],
  params: unknown[],
  searchableCols: string[],
): string {
  switch (node.kind) {
    case "term":
      return termToSql(node.value, columns, params, searchableCols);
    case "not":
      return `(NOT ${nodeToSql(node.child!, columns, params, searchableCols)})`;
    case "and":
      return `(${nodeToSql(node.left!, columns, params, searchableCols)} AND ${
        nodeToSql(node.right!, columns, params, searchableCols)
      })`;
    case "or":
      return `(${nodeToSql(node.left!, columns, params, searchableCols)} OR ${
        nodeToSql(node.right!, columns, params, searchableCols)
      })`;
  }
}

function buildSearchCondition(
  q: string,
  columns: ColumnInfo[],
  searchableCols: string[],
  params: unknown[],
): string {
  if (!q || typeof q !== "string" || !q.trim()) return "";
  try {
    const tokens = tokenizeQuery(q);
    if (tokens.length === 0) return "";
    const parser = new QueryParser(tokens);
    const ast = parser.parse();
    return nodeToSql(ast, columns, params, searchableCols);
  } catch (e) {
    console.error("Search query parse error:", e);
    return "";
  }
}

function getSortSql(
  sortBy: string,
  sortDir: string,
  cols: { url: string; name: string; date: string },
): string {
  const dir = sortDir === "desc" ? "DESC" : "ASC";
  switch (sortBy) {
    case "url":
      return cols.url && validateIdentifier(cols.url)
        ? `CAST(${quoteIdent(cols.url)} AS TEXT) COLLATE NOCASE ${dir}`
        : "";
    case "name":
      return cols.name && validateIdentifier(cols.name)
        ? `CAST(${quoteIdent(cols.name)} AS TEXT) COLLATE NOCASE ${dir}`
        : "";
    case "date":
      return cols.date && validateIdentifier(cols.date) ? `CAST(${quoteIdent(cols.date)} AS TEXT) ${dir}` : "";
    case "domain":
      return cols.url && validateIdentifier(cols.url)
        ? `CASE WHEN instr(${quoteIdent(cols.url)}, '://') > 0 THEN substr(${quoteIdent(cols.url)}, instr(${quoteIdent(cols.url)}, '://')+3) ELSE ${
          quoteIdent(cols.url)
        } END COLLATE NOCASE ${dir}`
        : "";
    default:
      return "";
  }
}

async function cleanupDatabase() {
  if (currentDb) {
    try {
      currentDb.close();
    } catch (e) {
      console.error("Error closing database:", e);
    }
    currentDb = null;
  }
  if (currentDbPath) {
    try {
      await Deno.remove(currentDbPath);
      console.log("Temporary database cleaned up");
    } catch (e) {
      console.error("Error removing temp file:", e);
    }
    currentDbPath = "";
  }
}

// ─── HTTP helpers ─────────────────────────────────────────────────────────

function jsonResponse(data: unknown, status = 200): Response {
  return new Response(JSON.stringify(data), {
    status,
    headers: { "content-type": "application/json; charset=utf-8" },
  });
}

function jsonError(message: string, status: number): Response {
  return jsonResponse({ error: message }, status);
}

// ─── Handlers ─────────────────────────────────────────────────────────────

async function handleLoad(req: Request): Promise<Response> {
  if (!req.body) return jsonError("No file content", 400);

  let fileContent: ArrayBuffer;
  try {
    fileContent = await req.arrayBuffer();
  } catch (e) {
    console.error("Error reading request body:", e);
    return jsonError(`Failed to read file: ${e.message}`, 500);
  }

  if (fileContent.byteLength === 0) {
    return jsonError("Uploaded file is empty", 400);
  }

  if (fileContent.byteLength > MAX_FILE_SIZE) {
    return jsonError(`File exceeds maximum size of ${MAX_FILE_SIZE} bytes`, 413);
  }

  // Validate SQLite file header
  const view = new Uint8Array(fileContent);
  const header = new TextDecoder().decode(view.slice(0, 16));
  if (!header.startsWith("SQLite format 3")) {
    return jsonError("Invalid SQLite database file", 400);
  }

  const tempDir = await Deno.makeTempDir();
  const filePath = `${tempDir}/bookmarks_${crypto.randomUUID()}.db`;

  try {
    await Deno.writeFile(filePath, new Uint8Array(fileContent));
  } catch (e) {
    console.error("Error writing file:", e);
    return jsonError(`Failed to write file: ${e.message}`, 500);
  }

  // Cleanup previous database
  await cleanupDatabase();

  try {
    try {
      currentDb = new DatabaseSync(filePath, { readOnly: true } as any);
    } catch {
      currentDb = new DatabaseSync(filePath);
      try {
        currentDb.exec("PRAGMA query_only = ON");
      } catch {
        // ignore
      }
    }
  } catch (e) {
    console.error("Error opening database:", e);
    try {
      await Deno.remove(filePath);
    } catch {
      // ignore
    }
    return jsonError(`Failed to open database: ${e.message}`, 400);
  }

  currentDbPath = filePath;
  currentDbName = req.headers.get("x-file-name") || "uploaded.db";

  try {
    schema = discoverSchema(currentDb);
  } catch (e) {
    console.error("Error discovering schema:", e);
    await cleanupDatabase();
    return jsonError(`Schema discovery failed: ${e.message}`, 400);
  }

  selectedTable = autoSelectTable(schema);
  if (selectedTable) {
    const table = schema.find((t) => t.name === selectedTable)!;
    selectedColumns = detectColumns(table.columns);
  } else {
    selectedColumns = { url: "", name: "", date: "" };
  }

  return jsonResponse({ ok: true, dbName: currentDbName, selectedTable, selectedColumns });
}

function handleInfo(): Response {
  if (!currentDb) {
    return jsonResponse({
      dbName: "",
      tables: [],
      selectedTable: "",
      selectedColumns: { url: "", name: "", date: "" },
      totalRows: 0,
      tableColumns: [],
    });
  }
  const table = schema.find((t) => t.name === selectedTable);
  return jsonResponse({
    dbName: currentDbName,
    tables: schema,
    selectedTable,
    selectedColumns,
    totalRows: table ? table.rowCount : 0,
    tableColumns: table ? table.columns.map((c) => c.name) : [],
  });
}

async function handleSelectTable(req: Request): Promise<Response> {
  if (!currentDb) return jsonError("No database loaded", 400);
  let body: unknown;
  try {
    body = await req.json();
  } catch {
    return jsonError("Invalid JSON", 400);
  }
  const tableName = String((body as any).table ?? "").trim();
  if (!tableName || !validateIdentifier(tableName)) {
    return jsonError("Invalid table name", 400);
  }
  
  const table = schema.find((t) => t.name === tableName);
  if (!table) return jsonError("Table not found", 404);
  
  selectedTable = table.name;
  selectedColumns = detectColumns(table.columns);
  return jsonResponse({
    ok: true,
    selectedTable,
    selectedColumns,
    tableColumns: table.columns.map((c) => c.name),
  });
}

async function handleSelectColumns(req: Request): Promise<Response> {
  if (!currentDb || !selectedTable) return jsonError("No database loaded", 400);
  let body: any;
  try {
    body = await req.json();
  } catch {
    return jsonError("Invalid JSON", 400);
  }

  const tableInfo = schema.find((t) => t.name === selectedTable);
  if (!tableInfo) return jsonError("Selected table not found", 400);

  const validColumns = new Set(tableInfo.columns.map((c) => c.name));
  const url = String(body.url ?? "").trim();
  const name = String(body.name ?? "").trim();
  const date = String(body.date ?? "").trim();

  if (url && (!validateIdentifier(url) || !validColumns.has(url))) {
    return jsonError(`Invalid URL column '${url}'`, 400);
  }
  if (name && (!validateIdentifier(name) || !validColumns.has(name))) {
    return jsonError(`Invalid Name column '${name}'`, 400);
  }
  if (date && (!validateIdentifier(date) || !validColumns.has(date))) {
    return jsonError(`Invalid Date column '${date}'`, 400);
  }

  selectedColumns = { url, name, date };
  return jsonResponse({ ok: true, selectedColumns });
}

async function handleQuery(req: Request): Promise<Response> {
  if (!currentDb || !selectedTable) return jsonError("No database loaded", 400);
  let body: any;
  try {
    body = await req.json();
  } catch {
    return jsonError("Invalid JSON", 400);
  }

  const q = String(body.q ?? "").trim().slice(0, 500);
  const domain = String(body.domain ?? "").trim().slice(0, 255);
  const dateFrom = String(body.dateFrom ?? "").trim().slice(0, 10);
  const dateTo = String(body.dateTo ?? "").trim().slice(0, 10);
  const sortBy = String(body.sortBy ?? "none");
  const sortDir = body.sortDir === "desc" ? "desc" : "asc";

  let limit = parseInt(body.limit ?? "100", 10);
  if (!Number.isFinite(limit) || limit < 1) limit = 100;
  if (limit > MAX_QUERY_RESULTS) limit = MAX_QUERY_RESULTS;

  let offset = parseInt(body.offset ?? "0", 10);
  if (!Number.isFinite(offset) || offset < 0) offset = 0;

  const tableInfo = schema.find((t) => t.name === selectedTable);
  if (!tableInfo) return jsonError("Selected table not found", 400);
  const columns = tableInfo.columns;
  const searchableCols = getSearchableCols(selectedColumns, columns);

  const params: unknown[] = [];
  const where: string[] = [];

  // Build search condition
  if (q) {
    const cond = buildSearchCondition(q, columns, searchableCols, params);
    if (cond) where.push(cond);
  }

  // Build domain filter
  if (domain && selectedColumns.url && validateIdentifier(selectedColumns.url)) {
    const col = quoteIdent(selectedColumns.url);
    const pattern = `%${escapeLike(domain)}%`;
    params.push(pattern);
    where.push(`CAST(${col} AS TEXT) COLLATE NOCASE LIKE ? ESCAPE '\\'`);
  }

  // Build date filters
  if (selectedColumns.date && validateIdentifier(selectedColumns.date) && (dateFrom || dateTo)) {
    const col = quoteIdent(selectedColumns.date);
    if (dateFrom) {
      params.push(dateFrom);
      where.push(
        `CASE WHEN typeof(${col}) IN ('integer','real') THEN date(${col}, 'unixepoch') ELSE date(${col}) END >= ?`,
      );
    }
    if (dateTo) {
      params.push(dateTo);
      where.push(
        `CASE WHEN typeof(${col}) IN ('integer','real') THEN date(${col}, 'unixepoch') ELSE date(${col}) END <= ?`,
      );
    }
  }

  const whereSql = where.length ? ` WHERE ${where.join(" AND ")}` : "";
  
  let tableSql = "";
  try {
    tableSql = quoteIdent(selectedTable);
  } catch {
    return jsonError("Invalid table", 400);
  }

  let total = 0;
  try {
    const countRow = currentDb
      .prepare(`SELECT COUNT(*) AS c FROM ${tableSql}${whereSql}`)
      .get(...params) as { c: number };
    total = Number(countRow.c);
  } catch (e) {
    console.error("Count query error:", e);
    return jsonError(`Query error: ${e.message}`, 500);
  }

  let sql = `SELECT * FROM ${tableSql}${whereSql}`;
  const sortSql = getSortSql(sortBy, sortDir, selectedColumns);
  if (sortSql) sql += ` ORDER BY ${sortSql}`;
  sql += ` LIMIT ? OFFSET ?`;
  params.push(limit, offset);

  let rows: unknown[] = [];
  try {
    rows = currentDb.prepare(sql).all(...params) as unknown[];
  } catch (e) {
    console.error("Query error:", e);
    return jsonError(`Query error: ${e.message}`, 500);
  }

  return jsonResponse({ rows, total, columns: selectedColumns, table: selectedTable });
}

// ─── HTML UI ──────────────────────────────────────────────────────────────

const html = `<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<meta http-equiv="X-UA-Compatible" content="ie=edge">
<title>OSTIUM Bookmark Explorer</title>
<style>
:root {
  --bg: #0f1115;
  --surface: #181b22;
  --surface2: #20242e;
  --text: #e6e8ee;
  --text2: #a0a6b5;
  --accent: #4f8cff;
  --accent2: #6c5ce7;
  --danger: #ff4d4d;
  --border: #2a2f3a;
  --radius: 12px;
}
* { box-sizing: border-box; margin: 0; padding: 0; }
body {
  background: var(--bg);
  color: var(--text);
  font-family: system-ui, -apple-system, Segoe UI, Roboto, sans-serif;
  min-height: 100vh;
}
header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 2rem;
  background: var(--surface);
  border-bottom: 1px solid var(--border);
  flex-wrap: wrap;
  gap: 0.75rem;
}
h1 { font-size: 1.5rem; font-weight: 600; }
.db-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}
#dbName {
  color: var(--text2);
  font-size: 0.9rem;
}
button {
  background: var(--surface2);
  color: var(--text);
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 0.5rem 1rem;
  cursor: pointer;
  font-size: 0.9rem;
  transition: background 0.2s;
}
button:hover { background: var(--border); }
button:disabled { opacity: 0.5; cursor: not-allowed; }
main {
  max-width: 1100px;
  margin: 0 auto;
  padding: 1.5rem;
}
#controls {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: var(--radius);
  padding: 1.25rem;
  margin-bottom: 1.5rem;
}
.row {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}
label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  color: var(--text2);
}
select, input[type="search"], input[type="text"], input[type="date"] {
  background: var(--bg);
  color: var(--text);
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  font-size: 0.9rem;
}
select:focus, input:focus {
  outline: none;
  border-color: var(--accent);
}
.search-area {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}
#searchInput {
  flex: 1;
  min-width: 220px;
}
.filters {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}
.sort-group {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}
#tableInfo {
  color: var(--text2);
  font-size: 0.85rem;
}
#results {
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: var(--radius);
  padding: 1.25rem;
}
#resultInfo {
  color: var(--text2);
  font-size: 0.9rem;
  margin-bottom: 1rem;
}
#resultList {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}
.card {
  display: flex;
  gap: 1rem;
  background: var(--surface2);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 0.9rem 1rem;
  transition: border-color 0.2s;
}
.card:hover {
  border-color: var(--accent);
}
.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: var(--accent2);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  color: white;
  flex-shrink: 0;
}
.card-content {
  min-width: 0;
}
.bookmark-title {
  color: var(--text);
  font-weight: 600;
  text-decoration: none;
  display: block;
  margin-bottom: 0.2rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.bookmark-title:hover {
  color: var(--accent);
}
.bookmark-url {
  color: var(--text2);
  font-size: 0.85rem;
  word-break: break-all;
}
.bookmark-date {
  color: var(--text2);
  font-size: 0.8rem;
  margin-top: 0.25rem;
}
.group-section {
  margin-bottom: 1.25rem;
}
.group-title {
  font-weight: 600;
  color: var(--accent);
  margin-bottom: 0.5rem;
  font-size: 1rem;
}
#pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  margin-top: 1.5rem;
}
.loading {
  display: none;
  text-align: center;
  color: var(--text2);
  padding: 1rem;
}
.loading.active {
  display: block;
}
.help-text {
  font-size: 0.8rem;
  color: var(--text2);
  margin-top: 0.5rem;
}
footer {
  text-align: center;
  padding: 1.5rem;
  color: var(--text2);
  font-size: 0.8rem;
}
</style>
</head>
<body>
<header>
  <h1>📚 OSTIUM Bookmark Explorer</h1>
  <div class="db-actions">
    <button id="loadBtn">Load Database</button>
    <input type="file" id="fileInput" accept=".db,.sqlite,.sqlite3,.db3" hidden>
    <span id="dbName">No database loaded</span>
  </div>
</header>
<main>
  <section id="controls">
    <div class="row">
      <label>Table:
        <select id="tableSelect"></select>
      </label>
      <span id="tableInfo"></span>
    </div>
    <div class="row column-mapping">
      <label>URL column:
        <select id="urlColumnSelect"></select>
      </label>
      <label>Name column:
        <select id="nameColumnSelect"></select>
      </label>
      <label>Date column:
        <select id="dateColumnSelect"></select>
      </label>
    </div>
    <div class="search-area">
      <input type="search" id="searchInput" placeholder="Search... e.g. python OR django -old url:github.com">
      <button id="searchBtn">Search</button>
      <button id="resetBtn">Reset</button>
    </div>
    <div class="help-text">💡 Operators: AND, OR, NOT, or use "-" for NOT. Use quotes for phrases. Use "field:value" for field search.</div>
    <div class="filters">
      <input type="text" id="domainFilter" placeholder="Domain filter (e.g. github.com)">
      <input type="date" id="dateFrom" title="From date">
      <input type="date" id="dateTo" title="To date">
    </div>
    <div class="sort-group">
      <label>Sort by:
        <select id="sortBy">
          <option value="none">None</option>
          <option value="url">URL</option>
          <option value="name">Name</option>
          <option value="date">Date</option>
          <option value="domain">Domain</option>
        </select>
      </label>
      <label>Order:
        <select id="sortDir">
          <option value="asc">Ascending</option>
          <option value="desc">Descending</option>
        </select>
      </label>
      <label>Group by:
        <select id="groupBy">
          <option value="none">None</option>
          <option value="domain">Domain</option>
          <option value="year">Year</option>
          <option value="month">Month</option>
        </select>
      </label>
    </div>
  </section>
  <section id="results">
    <div class="loading" id="loading">Loading...</div>
    <div id="resultInfo"></div>
    <div id="resultList"></div>
    <div id="pagination"></div>
  </section>
</main>
<footer>OSTIUM Bookmark Explorer — local Deno app v1.1</footer>
<script>
(function() {
  var state = {
    currentInfo: null,
    currentRows: [],
    currentTotal: 0,
    limit: 100,
    offset: 0,
    isLoading: false
  };

  function showLoading(show) {
    state.isLoading = show;
    var loading = document.getElementById('loading');
    if (show) {
      loading.classList.add('active');
    } else {
      loading.classList.remove('active');
    }
  }

  function escapeHtml(str) {
    var div = document.createElement('div');
    div.textContent = str === null || str === undefined ? '' : String(str);
    return div.innerHTML;
  }

  function getDomain(url) {
    if (!url) return '';
    try {
      var u = new URL(url);
      return u.hostname;
    } catch (e) {
      var m = url.match(/^(?:https?:\\/\\/)?([^\\/]+)/i);
      return m ? m[1] : '';
    }
  }

  function formatUrl(url) {
    if (!url) return '#';
    if (/^[a-zA-Z][a-zA-Z0-9+.-]*:/.test(url)) return url;
    return 'https://' + url;
  }

  function formatDate(value) {
    if (!value) return '';
    var num = Number(value);
    if (!isNaN(num) && num > 100000) {
      var d = new Date(num > 1e12 ? num : num * 1000);
      if (!isNaN(d)) return d.toLocaleString();
    }
    var d = new Date(value);
    if (!isNaN(d)) return d.toLocaleString();
    return value;
  }

  function getYear(value) {
    var d = new Date(value);
    if (!isNaN(d)) return String(d.getFullYear());
    var num = Number(value);
    if (!isNaN(num) && num > 100000) {
      d = new Date(num > 1e12 ? num : num * 1000);
      if (!isNaN(d)) return String(d.getFullYear());
    }
    return String(value).slice(0, 4);
  }

  function getMonth(value) {
    var d = new Date(value);
    if (!isNaN(d)) return d.toISOString().slice(0, 7);
    var num = Number(value);
    if (!isNaN(num) && num > 100000) {
      d = new Date(num > 1e12 ? num : num * 1000);
      if (!isNaN(d)) return d.toISOString().slice(0, 7);
    }
    return String(value).slice(0, 7);
  }

  function groupRows(rows, groupBy, cols) {
    var groups = new Map();
    for (var i = 0; i < rows.length; i++) {
      var row = rows[i];
      var key = '';
      if (groupBy === 'domain') {
        key = getDomain(row[cols.url]) || 'Unknown';
      } else if (groupBy === 'year') {
        key = getYear(row[cols.date]) || 'Unknown';
      } else if (groupBy === 'month') {
        key = getMonth(row[cols.date]) || 'Unknown';
      } else {
        key = 'All';
      }
      if (!groups.has(key)) groups.set(key, []);
      groups.get(key).push(row);
    }
    return groups;
  }

  function createCard(row, cols) {
    var url = row[cols.url] || '';
    var name = row[cols.name] || url || '';
    var date = row[cols.date] || '';
    var domain = getDomain(url);
    var initial = domain ? domain.charAt(0).toUpperCase() : '?';
    var href = formatUrl(url);
    return '<article class="card">' +
      '<div class="avatar">' + escapeHtml(initial) + '</div>' +
      '<div class="card-content">' +
      '<a href="' + escapeHtml(href) + '" target="_blank" rel="noopener noreferrer" class="bookmark-title">' + escapeHtml(name) + '</a>' +
      '<div class="bookmark-url">' + escapeHtml(url) + '</div>' +
      (date ? '<div class="bookmark-date">' + escapeHtml(formatDate(date)) + '</div>' : '') +
      '</div>' +
      '</article>';
  }

  function renderResults(data) {
    var resultInfo = document.getElementById('resultInfo');
    var resultList = document.getElementById('resultList');
    var pagination = document.getElementById('pagination');
    var cols = data.columns || {};
    var total = data.total;
    var rows = data.rows;

    resultInfo.textContent = total + ' result(s) found';
    resultList.innerHTML = '';

    if (!rows.length) {
      resultList.innerHTML = '<p>No results found.</p>';
      pagination.innerHTML = '';
      return;
    }

    var groupBy = document.getElementById('groupBy').value;
    if (groupBy === 'none') {
      for (var i = 0; i < rows.length; i++) {
        resultList.insertAdjacentHTML('beforeend', createCard(rows[i], cols));
      }
    } else {
      var groups = groupRows(rows, groupBy, cols);
      var sortedKeys = Array.from(groups.keys()).sort();
      sortedKeys.forEach(function(key) {
        var items = groups.get(key);
        resultList.insertAdjacentHTML('beforeend', '<div class="group-section"><div class="group-title">' + escapeHtml(key) + ' (' + items.length + ')</div></div>');
        for (var i = 0; i < items.length; i++) {
          resultList.insertAdjacentHTML('beforeend', createCard(items[i], cols));
        }
      });
    }

    var page = Math.floor(state.offset / state.limit) + 1;
    var pages = Math.max(1, Math.ceil(total / state.limit));
    var prevDisabled = state.offset <= 0;
    var nextDisabled = state.offset + state.limit >= total;
    pagination.innerHTML = '<button ' + (prevDisabled ? 'disabled' : '') + ' data-offset="' + (state.offset - state.limit) + '">Previous</button>' +
      '<span>Page ' + page + '/' + pages + '</span>' +
      '<button ' + (nextDisabled ? 'disabled' : '') + ' data-offset="' + (state.offset + state.limit) + '">Next</button>';
  }

  async function runSearch() {
    showLoading(true);
    var params = {
      q: document.getElementById('searchInput').value.trim(),
      domain: document.getElementById('domainFilter').value.trim(),
      dateFrom: document.getElementById('dateFrom').value,
      dateTo: document.getElementById('dateTo').value,
      sortBy: document.getElementById('sortBy').value,
      sortDir: document.getElementById('sortDir').value,
      groupBy: document.getElementById('groupBy').value,
      limit: state.limit,
      offset: state.offset
    };

    try {
      var res = await fetch('/api/query', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(params)
      });
      if (!res.ok) {
        var err = await res.json().catch(function() { return { error: 'Request failed' }; });
        alert('Search error: ' + (err.error || res.status));
        showLoading(false);
        return;
      }
      var data = await res.json();
      state.currentTotal = data.total;
      state.currentRows = data.rows;
      renderResults(data);
    } catch (e) {
      alert('Network error: ' + e.message);
    } finally {
      showLoading(false);
    }
  }

  async function loadInfo() {
    try {
      var res = await fetch('/api/info');
      if (!res.ok) throw new Error('Failed to load info');
      var info = await res.json();
      state.currentInfo = info;
      document.getElementById('dbName').textContent = info.dbName || 'No database loaded';

      var select = document.getElementById('tableSelect');
      select.innerHTML = '';
      info.tables.forEach(function(t) {
        var opt = document.createElement('option');
        opt.value = t.name;
        opt.textContent = t.name + ' (' + t.rowCount + ')';
        if (t.name === info.selectedTable) opt.selected = true;
        select.appendChild(opt);
      });

      document.getElementById('tableInfo').textContent = 'URL: ' + (info.selectedColumns.url || '—') +
        ' | Name: ' + (info.selectedColumns.name || '—') +
        ' | Date: ' + (info.selectedColumns.date || '—');

      var tableColumns = info.tableColumns || [];
      ['url', 'name', 'date'].forEach(function(key) {
        var colSelect = document.getElementById(key + 'ColumnSelect');
        if (!colSelect) return;
        colSelect.innerHTML = '';
        var emptyOpt = document.createElement('option');
        emptyOpt.value = '';
        emptyOpt.textContent = '--';
        colSelect.appendChild(emptyOpt);
        tableColumns.forEach(function(col) {
          var opt = document.createElement('option');
          opt.value = col;
          opt.textContent = col;
          if (col === info.selectedColumns[key]) opt.selected = true;
          colSelect.appendChild(opt);
        });
      });
    } catch (e) {
      alert('Failed to load database info: ' + e.message);
    }
  }

  async function uploadDatabase(file) {
    showLoading(true);
    try {
      var res = await fetch('/api/load', {
        method: 'POST',
        headers: { 'X-File-Name': file.name },
        body: file
      });
      if (!res.ok) {
        var err = await res.json().catch(function() { return { error: 'Unknown' }; });
        alert('Load failed: ' + (err.error || res.status));
        showLoading(false);
        return;
      }
      await loadInfo();
      state.offset = 0;
      await runSearch();
    } catch (e) {
      alert('Upload error: ' + e.message);
      showLoading(false);
    }
  }

  document.getElementById('loadBtn').addEventListener('click', function() {
    document.getElementById('fileInput').click();
  });

  document.getElementById('fileInput').addEventListener('change', function(e) {
    var file = e.target.files[0];
    if (file) {
      uploadDatabase(file);
    }
    e.target.value = '';
  });

  document.getElementById('searchBtn').addEventListener('click', function() {
    state.offset = 0;
    runSearch();
  });

  document.getElementById('resetBtn').addEventListener('click', function() {
    document.getElementById('searchInput').value = '';
    document.getElementById('domainFilter').value = '';
    document.getElementById('dateFrom').value = '';
    document.getElementById('dateTo').value = '';
    document.getElementById('sortBy').value = 'none';
    document.getElementById('sortDir').value = 'asc';
    document.getElementById('groupBy').value = 'none';
    state.offset = 0;
    runSearch();
  });

  document.getElementById('tableSelect').addEventListener('change', async function(e) {
    try {
      var res = await fetch('/api/select-table', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ table: e.target.value })
      });
      if (!res.ok) {
        alert('Failed to select table');
        return;
      }
      await loadInfo();
      state.offset = 0;
      await runSearch();
    } catch (err) {
      alert('Error: ' + err.message);
    }
  });

  ['url', 'name', 'date'].forEach(function(key) {
    var select = document.getElementById(key + 'ColumnSelect');
    if (select) {
      select.addEventListener('change', async function() {
        var body = {
          url: document.getElementById('urlColumnSelect').value,
          name: document.getElementById('nameColumnSelect').value,
          date: document.getElementById('dateColumnSelect').value,
        };
        try {
          var res = await fetch('/api/select-columns', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
          });
          if (!res.ok) {
            var err = await res.json().catch(function() { return { error: 'Unknown' }; });
            alert('Failed to update columns: ' + (err.error || res.status));
            return;
          }
          var info = await (await fetch('/api/info')).json();
          document.getElementById('tableInfo').textContent = 'URL: ' + (info.selectedColumns.url || '—') +
            ' | Name: ' + (info.selectedColumns.name || '—') +
            ' | Date: ' + (info.selectedColumns.date || '—');
          state.offset = 0;
          await runSearch();
        } catch (e) {
          alert('Network error: ' + e.message);
        }
      });
    }
  });

  document.getElementById('pagination').addEventListener('click', function(e) {
    var btn = e.target.closest('button');
    if (!btn || btn.disabled) return;
    var offset = parseInt(btn.dataset.offset, 10);
    if (!isNaN(offset)) {
      state.offset = offset;
      runSearch();
    }
  });

  document.getElementById('sortBy').addEventListener('change', function() {
    state.offset = 0;
    runSearch();
  });
  document.getElementById('sortDir').addEventListener('change', function() {
    state.offset = 0;
    runSearch();
  });
  document.getElementById('groupBy').addEventListener('change', function() {
    state.offset = 0;
    runSearch();
  });

  loadInfo();
})();
</script>
</body>
</html>`;

// ─── Server ───────────────────────────────────────────────────────────────

async function handler(req: Request): Promise<Response> {
  const url = new URL(req.url);

  if (req.method === "GET" && url.pathname === "/") {
    return new Response(html, {
      headers: {
        "content-type": "text/html; charset=utf-8",
        "content-security-policy":
          "default-src 'self'; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self'",
        "x-content-type-options": "nosniff",
        "referrer-policy": "no-referrer",
      },
    });
  }

  if (req.method === "POST" && url.pathname === "/api/load") {
    return await handleLoad(req);
  }

  if (req.method === "GET" && url.pathname === "/api/info") {
    return handleInfo();
  }

  if (req.method === "POST" && url.pathname === "/api/select-table") {
    return await handleSelectTable(req);
  }

  if (req.method === "POST" && url.pathname === "/api/select-columns") {
    return await handleSelectColumns(req);
  }

  if (req.method === "POST" && url.pathname === "/api/query") {
    return await handleQuery(req);
  }

  return new Response("Not Found", { status: 404 });
}

// Graceful shutdown
globalThis.addEventListener("unload", () => {
  cleanupDatabase();
});

Deno.serve({ hostname: "127.0.0.1", port: PORT }, handler);