using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using OsintWatcher.Core.Models;

namespace OsintWatcher.Core.Storage;

/// <summary>
/// Local, encrypted-at-rest persistence. One workspace = one folder on disk:
///   workspace/
///     key.dat              DPAPI-sealed AES-256 key (see SecureCrypto)
///     pages.enc            Encrypted JSON array of MonitoredPage
///     history/{pageId}.enc Encrypted, hash-chained JSON array of ChainedRecord
/// Nothing here ever leaves the machine; there is no network call in this class.
/// </summary>
public sealed class EncryptedStore
{
    readonly string _root;
    readonly byte[] _key;
    static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = false };

    public EncryptedStore(string workspaceRoot)
    {
        _root = workspaceRoot;
        Directory.CreateDirectory(_root);
        Directory.CreateDirectory(HistoryDir);
        _key = SecureCrypto.LoadOrCreateKey(Path.Combine(_root, "key.dat"));
    }

    string PagesFile => Path.Combine(_root, "pages.enc");
    string HistoryDir => Path.Combine(_root, "history");
    string HistoryFile(Guid pageId) => Path.Combine(HistoryDir, pageId.ToString("N") + ".enc");

    // ---------- Monitored pages ----------

    public List<MonitoredPage> LoadPages()
    {
        if (!File.Exists(PagesFile)) return [];
        var json = DecryptToString(File.ReadAllBytes(PagesFile));
        return JsonSerializer.Deserialize<List<MonitoredPage>>(json, JsonOpts) ?? [];
    }

    public void SavePages(IEnumerable<MonitoredPage> pages)
    {
        var json = JsonSerializer.Serialize(pages, JsonOpts);
        File.WriteAllBytes(PagesFile, EncryptFromString(json));
    }

    // ---------- Scan history (hash-chained) ----------

    public List<ScanResult> GetHistory(Guid pageId)
    {
        var chain = LoadChain(pageId);
        return [.. chain.Select(c => c.Result).OrderBy(r => r.TimestampUtc)];
    }

    public ScanResult? GetLatest(Guid pageId) => GetHistory(pageId).LastOrDefault();

    public void AppendScanResult(Guid pageId, ScanResult result)
    {
        var chain = LoadChain(pageId);
        var prevHash = chain.Count == 0 ? GenesisHash(pageId) : chain[^1].ChainHash;

        var canonical = JsonSerializer.Serialize(result, JsonOpts);
        var chainHash = ComputeChainHash(prevHash, canonical);

        chain.Add(new ChainedRecord { Result = result, PrevChainHash = prevHash, ChainHash = chainHash });
        SaveChain(pageId, chain);
    }

    /// <summary>Recomputes the hash chain from scratch and reports whether it still matches
    /// what was recorded — a broken chain means a past entry was altered or removed outside
    /// the application.</summary>
    public bool VerifyChainIntegrity(Guid pageId, out int firstBrokenIndex)
    {
        var chain = LoadChain(pageId);
        var expectedPrev = GenesisHash(pageId);

        for (int i = 0; i < chain.Count; i++)
        {
            var record = chain[i];
            if (record.PrevChainHash != expectedPrev) { firstBrokenIndex = i; return false; }

            var canonical = JsonSerializer.Serialize(record.Result, JsonOpts);
            var expectedHash = ComputeChainHash(expectedPrev, canonical);
            if (expectedHash != record.ChainHash) { firstBrokenIndex = i; return false; }

            expectedPrev = record.ChainHash;
        }

        firstBrokenIndex = -1;
        return true;
    }

    static string GenesisHash(Guid pageId) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("OSINTWATCHER-GENESIS-" + pageId.ToString("N"))));

    static string ComputeChainHash(string prevHash, string canonicalRecordJson) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(prevHash + "|" + canonicalRecordJson)));

    List<ChainedRecord> LoadChain(Guid pageId)
    {
        var file = HistoryFile(pageId);
        if (!File.Exists(file)) return [];
        var json = DecryptToString(File.ReadAllBytes(file));
        return JsonSerializer.Deserialize<List<ChainedRecord>>(json, JsonOpts) ?? [];
    }

    void SaveChain(Guid pageId, List<ChainedRecord> chain)
    {
        var json = JsonSerializer.Serialize(chain, JsonOpts);
        File.WriteAllBytes(HistoryFile(pageId), EncryptFromString(json));
    }

    public void DeleteHistory(Guid pageId)
    {
        var file = HistoryFile(pageId);
        if (File.Exists(file)) File.Delete(file);
    }

    // ---------- gzip + AES-GCM helpers ----------

    byte[] EncryptFromString(string plaintext)
    {
        using var msIn = new MemoryStream(Encoding.UTF8.GetBytes(plaintext));
        using var msOut = new MemoryStream();
        using (var gz = new GZipStream(msOut, CompressionLevel.Optimal, leaveOpen: true))
            msIn.CopyTo(gz);
        return SecureCrypto.Encrypt(msOut.ToArray(), _key);
    }

    string DecryptToString(byte[] blob)
    {
        var compressed = SecureCrypto.Decrypt(blob, _key);
        using var msIn = new MemoryStream(compressed);
        using var gz = new GZipStream(msIn, CompressionMode.Decompress);
        using var msOut = new MemoryStream();
        gz.CopyTo(msOut);
        return Encoding.UTF8.GetString(msOut.ToArray());
    }

    /// <summary>Compresses arbitrary snapshot bytes for storage inside a ScanResult
    /// (used by the fetcher before handing a ScanResult to AppendScanResult).</summary>
    public static byte[] CompressSnapshot(string text)
    {
        using var msOut = new MemoryStream();
        using (var gz = new GZipStream(msOut, CompressionLevel.Optimal, leaveOpen: true))
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            gz.Write(bytes, 0, bytes.Length);
        }
        return msOut.ToArray();
    }

    public static string DecompressSnapshot(byte[] gz)
    {
        using var msIn = new MemoryStream(gz);
        using var gzs = new GZipStream(msIn, CompressionMode.Decompress);
        using var msOut = new MemoryStream();
        gzs.CopyTo(msOut);
        return Encoding.UTF8.GetString(msOut.ToArray());
    }
}
