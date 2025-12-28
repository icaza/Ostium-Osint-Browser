using System.Security.Cryptography;

namespace SecureFileExplorer.OSINT.Services;

public static class HashService
{
    public static string Sha256(string path)
    {
        using var sha = SHA256.Create();
        using var fs = File.OpenRead(path);
        return Convert.ToHexString(sha.ComputeHash(fs));
    }
}
