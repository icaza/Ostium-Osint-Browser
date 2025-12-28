namespace SecureFileExplorer.OSINT.Services;

public static class FileSandbox
{
    /// <summary>
    /// Resolves a relative path securely, ensuring it remains within the root directory.
    /// </summary>
    public static string Resolve(string root, string relative)
    {
        var normalizedRoot = Path.GetFullPath(root);
        var cleanRelative = SanitizePath(relative);
        var combined = Path.Combine(normalizedRoot, cleanRelative);
        var fullPath = Path.GetFullPath(combined);

        if (!fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(
                $"Access denied: Path '{relative}' attempts to escape root directory");
        }

        return fullPath;
    }

    /// <summary>
    /// Cleans up a file path by removing potentially dangerous characters
    /// </summary>
    private static string SanitizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        path = path.Replace('/', Path.DirectorySeparatorChar)
                   .Replace('\\', Path.DirectorySeparatorChar);

        while (path.Contains($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}"))
        {
            path = path.Replace(
                $"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}",
                $"{Path.DirectorySeparatorChar}");
        }

        var parts = path.Split(Path.DirectorySeparatorChar)
            .Where(p => !string.IsNullOrWhiteSpace(p) && p != "..")
            .ToArray();

        return string.Join(Path.DirectorySeparatorChar, parts);
    }

    /// <summary>
    /// Checks if a path is safe (without forbidden characters)
    /// </summary>
    public static bool IsSafePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var invalidChars = Path.GetInvalidPathChars()
            .Concat(['<', '>', ':', '"', '|', '?', '*']);

        return !path.Any(c => invalidChars.Contains(c));
    }
}