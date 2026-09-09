using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using OsintWatcher.Core.Models;

namespace OsintWatcher.Core.Services;

/// <summary>
/// Builds the layered FingerprintSet from a fetched page. Kept dependency-free (regex-based
/// tag stripping rather than a full HTML parser) so OsintWatcher.Core has zero third-party
/// packages; this is precise enough to fingerprint reliably but is not a spec-compliant HTML
/// parser — a page that relies on exotic malformed markup could confuse the structural pass.
/// </summary>
public static partial class FingerprintService
{
    [GeneratedRegex(@"<!--.*?-->", RegexOptions.Singleline)]
    private static partial Regex CommentRegex();

    [GeneratedRegex(@"<script\b[^>]*>.*?</script>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex ScriptRegex();

    [GeneratedRegex(@"<style\b[^>]*>.*?</style>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex StyleRegex();

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex AnyTagRegex();

    [GeneratedRegex(@"<\s*([a-zA-Z][a-zA-Z0-9]*)")]
    private static partial Regex TagNameRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    public static FingerprintSet Compute(FetchResult fetch)
    {
        var raw = Sha256Hex(fetch.RawBytes);

        var stripped = StripCommentsScriptsStyles(fetch.Html);

        var normalized = WhitespaceRegex().Replace(stripped, " ").Trim();
        var normalizedHash = Sha256HexOfString(normalized);

        var visibleText = ExtractVisibleText(stripped);
        var visibleTextHash = Sha256HexOfString(visibleText);

        var structuralSkeleton = ExtractTagSkeleton(stripped);
        var structuralHash = Sha256HexOfString(structuralSkeleton);

        return new FingerprintSet
        {
            RawSha256 = raw,
            NormalizedSha256 = normalizedHash,
            VisibleTextSha256 = visibleTextHash,
            StructuralSha256 = structuralHash,
            TlsCertificateSha256 = fetch.TlsCertificateSha256,
            ContentLengthBytes = fetch.RawBytes.LongLength
        };
    }

    /// <summary>Visible text only — used both for hashing and as the stored snapshot that
    /// DiffService compares between runs.</summary>
    public static string ExtractVisibleText(string htmlWithoutScriptsOrStyles)
    {
        var noTags = AnyTagRegex().Replace(htmlWithoutScriptsOrStyles, "\n");
        var decoded = WebUtility.HtmlDecode(noTags);
        var lines = decoded
            .Split('\n')
            .Select(l => WhitespaceRegex().Replace(l, " ").Trim())
            .Where(l => l.Length > 0);
        return string.Join("\n", lines);
    }

    static string StripCommentsScriptsStyles(string html)
    {
        var noComments = CommentRegex().Replace(html, " ");
        var noScripts = ScriptRegex().Replace(noComments, " ");
        var noStyles = StyleRegex().Replace(noScripts, " ");
        return noStyles;
    }

    static string ExtractTagSkeleton(string html)
    {
        var sb = new StringBuilder();
        foreach (Match m in TagNameRegex().Matches(html))
        {
            sb.Append(m.Groups[1].Value.ToLowerInvariant());
            sb.Append('>');
        }
        return sb.ToString();
    }

    static string Sha256Hex(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    static string Sha256HexOfString(string s) => Sha256Hex(Encoding.UTF8.GetBytes(s));
}
