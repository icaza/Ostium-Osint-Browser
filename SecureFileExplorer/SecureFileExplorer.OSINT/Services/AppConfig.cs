using System.Text.Json;

namespace SecureFileExplorer.OSINT.Services;

public class AppConfig
{
    public string RootDirectory { get; set; } = "C:\\OSINT_DATA";
    public int Port { get; set; } = 8787;

    public List<string> AllowedExtensions { get; set; } = [
            // Documents
            ".pdf", ".docx", ".doc", ".txt", ".csv", ".rtf",
            ".xlsx", ".xls", ".pptx", ".ppt",
            
            // Images
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".ico",
            
            // Web files
            ".html", ".htm", ".css", ".js", ".json", ".xml", ".md",
            
            // Archives
            ".zip", ".rar", ".7z",
            
            // Email
            ".eml", ".msg",
            
            // Code files (read-only viewing)
            ".py", ".java", ".cpp", ".c", ".h", ".cs", ".php", ".sql",
            ".sh", ".bat", ".ps1", ".vbs",
            
            // Config files
            ".yml", ".yaml", ".ini", ".cfg", ".log",
            
            // Fonts
            ".woff", ".woff2", ".ttf"];

    public static AppConfig Load(string path = "config.json")
    {
        if (!File.Exists(path))
        {
            var cfg = new AppConfig();
            File.WriteAllText(path, JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true }));
            return cfg;
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
    }

    public bool Allowed(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLower();
        return AllowedExtensions.Contains(ext);
    }
}
