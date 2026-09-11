using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;

namespace OsintWatcher.Core.Storage;

/// <summary>
/// At-rest encryption for everything the app writes to disk (page lists, scan history,
/// snapshots). Uses AES-256-GCM for the data itself, with the AES key sealed by Windows
/// DPAPI (CryptProtectData) so it is bound to the current Windows user account and machine —
/// no passphrase for the investigator to manage, and the encrypted key file is useless if
/// copied to another machine or account. DPAPI is called directly via P/Invoke into
/// crypt32.dll so the project carries no extra third-party crypto dependency.
/// </summary>
[SupportedOSPlatform("windows")]
public static class SecureCrypto
{
    const int KeySizeBytes = 32; // AES-256
    const int NonceSizeBytes = 12; // GCM standard nonce
    const int TagSizeBytes = 16;

    /// <summary>Loads the AES key from <paramref name="keyFilePath"/>, generating and
    /// DPAPI-sealing a new random key on first run.</summary>
    public static byte[] LoadOrCreateKey(string keyFilePath)
    {
        if (File.Exists(keyFilePath))
        {
            var sealedKey = File.ReadAllBytes(keyFilePath);
            return DpapiUnprotect(sealedKey);
        }

        var key = RandomNumberGenerator.GetBytes(KeySizeBytes);
        var sealedNew = DpapiProtect(key);
        Directory.CreateDirectory(Path.GetDirectoryName(keyFilePath)!);
        File.WriteAllBytes(keyFilePath, sealedNew);
        return key;
    }

    public static byte[] Encrypt(byte[] plaintext, byte[] key)
    {
        var nonce = RandomNumberGenerator.GetBytes(NonceSizeBytes);
        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSizeBytes];

        using var aes = new AesGcm(key, TagSizeBytes);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        // Layout: [nonce(12)] [tag(16)] [ciphertext(...)]
        var output = new byte[NonceSizeBytes + TagSizeBytes + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, output, 0, NonceSizeBytes);
        Buffer.BlockCopy(tag, 0, output, NonceSizeBytes, TagSizeBytes);
        Buffer.BlockCopy(ciphertext, 0, output, NonceSizeBytes + TagSizeBytes, ciphertext.Length);
        return output;
    }

    public static byte[] Decrypt(byte[] blob, byte[] key)
    {
        if (blob.Length < NonceSizeBytes + TagSizeBytes)
            throw new CryptographicException("Encrypted blob is truncated or corrupt.");

        var nonce = blob.AsSpan(0, NonceSizeBytes).ToArray();
        var tag = blob.AsSpan(NonceSizeBytes, TagSizeBytes).ToArray();
        var ciphertext = blob.AsSpan(NonceSizeBytes + TagSizeBytes).ToArray();
        var plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, TagSizeBytes);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);
        return plaintext;
    }

    // --- DPAPI (crypt32.dll) P/Invoke — no extra NuGet package required ---

    static byte[] DpapiProtect(byte[] data)
    {
        var input = ToBlob(data);
        if (!CryptProtectData(ref input, "OsintWatcher key store", IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, out var output))
            throw new CryptographicException("DPAPI CryptProtectData failed.");
        try { return FromBlob(output); }
        finally { LocalFree(output.pbData); }
    }

    static byte[] DpapiUnprotect(byte[] data)
    {
        var input = ToBlob(data);
        if (!CryptUnprotectData(ref input, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, out var output))
            throw new CryptographicException("DPAPI CryptUnprotectData failed. The key store may belong to a different Windows user/machine.");
        try { return FromBlob(output); }
        finally { LocalFree(output.pbData); }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DATA_BLOB
    {
        public int cbData;
        public IntPtr pbData;
    }

    static DATA_BLOB ToBlob(byte[] data)
    {
        var ptr = Marshal.AllocHGlobal(data.Length);
        Marshal.Copy(data, 0, ptr, data.Length);
        return new DATA_BLOB { cbData = data.Length, pbData = ptr };
    }

    static byte[] FromBlob(DATA_BLOB blob)
    {
        var result = new byte[blob.cbData];
        Marshal.Copy(blob.pbData, result, 0, blob.cbData);
        return result;
    }

    [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool CryptProtectData(ref DATA_BLOB dataIn, string? description,
        IntPtr entropy, IntPtr reserved, IntPtr promptStruct, int flags, out DATA_BLOB dataOut);

    [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool CryptUnprotectData(ref DATA_BLOB dataIn, IntPtr description,
        IntPtr entropy, IntPtr reserved, IntPtr promptStruct, int flags, out DATA_BLOB dataOut);

    [DllImport("kernel32.dll")]
    static extern IntPtr LocalFree(IntPtr hMem);
}
