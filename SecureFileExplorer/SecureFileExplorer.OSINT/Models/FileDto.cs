namespace SecureFileExplorer.OSINT.Models;

public class FileDto
{
    public string Name { get; init; }
    public string Path { get; init; }
    public string Extension { get; init; }
    public long Size { get; init; }
    public DateTime Created { get; init; }
    public DateTime Modified { get; init; }
    public string Sha256 { get; init; }
    public bool IsImage { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = [];
}
