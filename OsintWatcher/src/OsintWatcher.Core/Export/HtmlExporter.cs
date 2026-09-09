using System.Net;
using System.Text;
using OsintWatcher.Core.Models;

namespace OsintWatcher.Core.Export;

/// <summary>
/// Renders the investigation report as a single, self-contained, offline HTML file — the
/// same template backs both the in-app dashboard (shown in WebView2) and the exported file.
/// No external network resources are referenced (fonts, scripts, images are all local/inline)
/// and a strict CSP meta tag blocks any resource the report itself does not embed, since the
/// report can contain text lifted from a monitored (untrusted) page and must not be able to
/// call out anywhere when opened later.
/// </summary>
public sealed class HtmlExporter : IReportExporter
{
    public string FileExtension => "html";
    public string DisplayName => "HTML (dashboard report)";

    public byte[] Export(InvestigationReport report) => Encoding.UTF8.GetBytes(Render(report));

    public static string Render(InvestigationReport report)
    {
        var sb = new StringBuilder();
        sb.Append("""<!DOCTYPE html><html lang="en-US"><head><meta charset="utf-8">""");
        sb.Append("""<meta name="viewport" content="width=device-width, initial-scale=1">""");
        sb.Append("""<meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src 'unsafe-inline'; script-src 'unsafe-inline'; img-src data:;">""");
        sb.Append($"<title>{H(report.Title)} — OSINT Watcher report</title>");
        sb.Append("<style>").Append(Css).Append("</style></head><body>");

        sb.Append("<header class='masthead'>");
        sb.Append("<div class='masthead-title'>");
        sb.Append($"<h1>{H(report.Title)}</h1>");
        sb.Append("<p class='sub'>Web page integrity &amp; change-monitoring report</p>");
        sb.Append("</div>");
        sb.Append("<dl class='masthead-meta'>");
        sb.Append($"<dt>Generated</dt><dd>{report.GeneratedAtUtc:yyyy-MM-dd HH:mm} UTC</dd>");
        sb.Append($"<dt>Pages tracked</dt><dd>{report.Entries.Count}</dd>");
        if (!string.IsNullOrWhiteSpace(report.InvestigatorNote))
            sb.Append($"<dt>Note</dt><dd>{H(report.InvestigatorNote)}</dd>");
        sb.Append("</dl></header>");

        sb.Append("<section class='summary'>");
        sb.Append(SummaryStat("modified", report.ModifiedCount, "Modified"));
        sb.Append(SummaryStat("unchanged", report.UnchangedCount, "Unchanged"));
        sb.Append(SummaryStat("unreachable", report.UnreachableCount, "Unreachable"));
        sb.Append(SummaryStat("baseline", report.BaselineCount, "Baseline only"));
        sb.Append("</section>");

        AppendLedger(sb, "Modified", "modified", report.Entries.Where(e => e.Latest?.Verdict == ScanVerdict.Modified));
        AppendLedger(sb, "Unchanged", "unchanged", report.Entries.Where(e => e.Latest?.Verdict == ScanVerdict.Unchanged));
        AppendLedger(sb, "Unreachable", "unreachable", report.Entries.Where(e => e.Latest?.Verdict == ScanVerdict.Unreachable));
        AppendLedger(sb, "Baseline only", "baseline", report.Entries.Where(e => e.Latest?.Verdict == ScanVerdict.Baseline));

        sb.Append("<footer>Generated locally by OSINT Watcher. No data in this file was sent anywhere.</footer>");
        sb.Append("<script>").Append(Js).Append("</script>");
        sb.Append("</body></html>");
        return sb.ToString();
    }

    private static string SummaryStat(string cls, int value, string label) =>
        $"<div class='stat stat-{cls}'><span class='stat-value'>{value}</span><span class='stat-label'>{H(label)}</span></div>";

    private static void AppendLedger(StringBuilder sb, string title, string cls, IEnumerable<PageReportEntry> entries)
    {
        var list = entries.ToList();
        sb.Append($"<section class='ledger'><h2>{H(title)} <span class='count'>({list.Count})</span></h2>");
        if (list.Count == 0) { sb.Append("<p class='empty'>None.</p></section>"); return; }

        foreach (var e in list)
        {
            var r = e.Latest!;
            sb.Append($"<article class='row row-{cls}'>");
            sb.Append("<div class='row-main'>");
            sb.Append($"<span class='swatch swatch-{cls}'></span>");
            sb.Append("<div class='row-id'>");
            sb.Append($"<div class='row-label'>{H(e.Page.Label.Length > 0 ? e.Page.Label : e.Page.Url)}</div>");
            sb.Append($"<div class='row-url'>{H(e.Page.Url)}</div>");
            sb.Append("</div>");
            sb.Append("<div class='row-when'>");
            sb.Append($"<time>{r.TimestampUtc:yyyy-MM-dd HH:mm} UTC</time>");
            sb.Append($"<span class='http'>{(r.HttpStatusCode is int code ? "HTTP " + code : "no response")}</span>");
            sb.Append("</div>");
            if (r.Diff is not null)
                sb.Append($"<button class='toggle' type='button' data-target='detail-{e.Page.Id:N}'>+{r.Diff.LinesAdded} / -{r.Diff.LinesRemoved} lines</button>");
            sb.Append("</div>"); // row-main

            sb.Append($"<div class='row-detail' id='detail-{e.Page.Id:N}' hidden>");
            if (!string.IsNullOrWhiteSpace(e.Page.CaseReference))
                sb.Append($"<div class='kv'><span>Case reference</span><span>{H(e.Page.CaseReference)}</span></div>");
            if (r.Fingerprint is not null)
            {
                sb.Append($"<div class='kv'><span>Visible-text SHA-256</span><code>{H(r.Fingerprint.VisibleTextSha256)}</code></div>");
                sb.Append($"<div class='kv'><span>Raw SHA-256</span><code>{H(r.Fingerprint.RawSha256)}</code></div>");
                sb.Append($"<div class='kv'><span>Structural SHA-256</span><code>{H(r.Fingerprint.StructuralSha256)}</code></div>");
                if (r.Fingerprint.TlsCertificateSha256 is not null)
                    sb.Append($"<div class='kv'><span>TLS certificate SHA-256</span><code>{H(r.Fingerprint.TlsCertificateSha256)}</code></div>");
            }
            if (r.ErrorMessage is not null)
                sb.Append($"<div class='kv error'><span>Error</span><span>{H(r.ErrorMessage)}</span></div>");
            if (r.Diff is not null && r.Diff.Sample.Count > 0)
            {
                sb.Append("<div class='diff'>");
                foreach (var line in r.Diff.Sample.Take(60))
                {
                    var sign = line.Kind == DiffLineKind.Added ? "+" : "-";
                    var kindCls = line.Kind == DiffLineKind.Added ? "add" : "rem";
                    sb.Append($"<div class='diff-line diff-{kindCls}'><span class='diff-sign'>{sign}</span>{H(Truncate(line.Text, 220))}</div>");
                }
                sb.Append("</div>");
            }
            sb.Append("</div>"); // row-detail
            sb.Append("</article>");
        }
        sb.Append("</section>");
    }

    private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "…";
    private static string H(string s) => WebUtility.HtmlEncode(s);

    private const string Css = """
        :root{
          --ink:#12161C; --panel:#1A2029; --panel-alt:#212836; --hairline:#2B3542;
          --text:#E7EAEE; --text-muted:#8A96A3;
          --amber:#E8A33D; --teal:#4FB6A8; --brick:#D96257; --baseline:#6B7A99;
          --font-display: Georgia, 'Iowan Old Style', 'Palatino Linotype', serif;
          --font-body: 'Segoe UI', system-ui, -apple-system, sans-serif;
          --font-mono: Consolas, 'Cascadia Mono', 'SFMono-Regular', Menlo, monospace;
        }
        *{box-sizing:border-box;}
        body{margin:0;background:var(--ink);color:var(--text);font-family:var(--font-body);
             line-height:1.5;}
        .masthead{display:flex;justify-content:space-between;align-items:flex-end;flex-wrap:wrap;
             gap:24px;padding:40px clamp(20px,5vw,64px) 28px;border-bottom:1px solid var(--hairline);}
        .masthead-title h1{font-family:var(--font-display);font-weight:600;font-size:clamp(24px,3vw,34px);
             margin:0;letter-spacing:.2px;}
        .masthead-title .sub{margin:6px 0 0;color:var(--text-muted);font-size:14px;}
        .masthead-meta{display:grid;grid-template-columns:auto auto;gap:2px 14px;margin:0;
             font-size:13px;color:var(--text-muted);}
        .masthead-meta dt{text-align:right;}
        .masthead-meta dd{margin:0;color:var(--text);}
        .summary{display:flex;gap:1px;background:var(--hairline);margin:0;
             border-bottom:1px solid var(--hairline);}
        .stat{flex:1;background:var(--panel);padding:18px clamp(16px,3vw,32px);display:flex;
             flex-direction:column;gap:4px;}
        .stat-value{font-family:var(--font-mono);font-size:28px;font-weight:600;}
        .stat-label{font-size:12.5px;color:var(--text-muted);}
        .stat-modified .stat-value{color:var(--amber);}
        .stat-unchanged .stat-value{color:var(--teal);}
        .stat-unreachable .stat-value{color:var(--brick);}
        .stat-baseline .stat-value{color:var(--baseline);}
        .ledger{padding:8px clamp(20px,5vw,64px) 28px;}
        .ledger h2{font-family:var(--font-display);font-weight:600;font-size:18px;
             margin:24px 0 10px;color:var(--text);}
        .ledger .count{color:var(--text-muted);font-weight:400;font-family:var(--font-body);font-size:14px;}
        .empty{color:var(--text-muted);font-size:14px;margin:0 0 12px;}
        .row{border-top:1px solid var(--hairline);}
        .row:last-child{border-bottom:1px solid var(--hairline);}
        .row-main{display:flex;align-items:center;gap:14px;padding:12px 4px;}
        .swatch{width:9px;height:9px;flex:none;}
        .swatch-modified{background:var(--amber);}
        .swatch-unchanged{background:var(--teal);}
        .swatch-unreachable{background:var(--brick);}
        .swatch-baseline{background:var(--baseline);}
        .row-id{flex:1;min-width:0;}
        .row-label{font-size:14.5px;}
        .row-url{font-family:var(--font-mono);font-size:12px;color:var(--text-muted);
             overflow:hidden;text-overflow:ellipsis;white-space:nowrap;}
        .row-when{display:flex;flex-direction:column;align-items:flex-end;gap:2px;
             font-size:12px;color:var(--text-muted);flex:none;}
        .toggle{background:var(--panel-alt);color:var(--text);border:1px solid var(--hairline);
             font-family:var(--font-mono);font-size:12px;padding:6px 10px;cursor:pointer;flex:none;}
        .toggle:hover{border-color:var(--amber);}
        .row-detail{background:var(--panel);border-top:1px solid var(--hairline);
             padding:14px 16px;margin-top:-1px;}
        .kv{display:flex;gap:10px;font-size:12.5px;padding:3px 0;}
        .kv span:first-child{color:var(--text-muted);min-width:170px;flex:none;}
        .kv code{font-family:var(--font-mono);word-break:break-all;}
        .kv.error span:last-child{color:var(--brick);}
        .diff{margin-top:10px;font-family:var(--font-mono);font-size:12px;
             background:var(--ink);border:1px solid var(--hairline);max-height:340px;overflow:auto;}
        .diff-line{padding:2px 10px;white-space:pre-wrap;word-break:break-word;}
        .diff-add{background:rgba(79,182,168,.10);color:var(--teal);}
        .diff-rem{background:rgba(217,98,87,.10);color:var(--brick);}
        .diff-sign{display:inline-block;width:14px;opacity:.8;}
        footer{padding:24px clamp(20px,5vw,64px) 40px;color:var(--text-muted);font-size:12px;}
        """;

    private const string Js = """
        document.addEventListener('click', function(ev){
          var btn = ev.target.closest('.toggle');
          if(!btn) return;
          var el = document.getElementById(btn.getAttribute('data-target'));
          if(!el) return;
          el.hidden = !el.hidden;
        });
        """;
}
