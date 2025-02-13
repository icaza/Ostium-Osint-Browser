using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class VisitLogger
{
    readonly string _csvFilePath;

    public VisitLogger(string csvFilePath)
    {
        _csvFilePath = csvFilePath;
        InitializeCsvFile();
    }

    void InitializeCsvFile()
    {
        if (!File.Exists(_csvFilePath))
        {
            File.WriteAllText(_csvFilePath, "Date,Heure,URL,Miniature,Tags\n");
        }
    }

    public void LogVisit(string url, string tags)
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var time = DateTime.Now.ToString("HH:mm:ss");
        var domain = new Uri(url).Host;
        string mini = GenerateFileName(domain);

        File.AppendAllText(_csvFilePath, $"{date},{time},{url},{mini}.ico,{tags}\n");
    }

    string GenerateFileName(string url)
    {
        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(url));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}