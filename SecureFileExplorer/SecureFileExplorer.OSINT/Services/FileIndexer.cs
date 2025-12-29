using SecureFileExplorer.OSINT.Models;

namespace SecureFileExplorer.OSINT.Services;

public class FileIndexer(AppConfig config)
{
    readonly List<FileDto> _index = [];
    readonly AppConfig _config = config;
    public IEnumerable<FileDto> All() => _index;

    public void Build()
    {
        foreach (var file in Directory.EnumerateFiles(
            _config.RootDirectory, "*", SearchOption.AllDirectories))
        {
            var ext = Path.GetExtension(file).ToLower();
            if (!_config.AllowedExtensions.Contains(ext)) continue;

            var info = new FileInfo(file);

            _index.Add(new FileDto
            {
                Name = info.Name,
                Path = Path.GetRelativePath(_config.RootDirectory, file),
                Extension = ext,
                Size = info.Length,
                Created = info.CreationTimeUtc,
                Modified = info.LastWriteTimeUtc,
                Sha256 = HashService.Sha256(file),
                IsImage = ext is ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" or ".svg"
            });
        }
    }

    public IEnumerable<FileDto> Search(string q) =>
        _index.Where(f => f.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
}
