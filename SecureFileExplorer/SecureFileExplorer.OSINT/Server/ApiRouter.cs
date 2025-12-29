using SecureFileExplorer.OSINT.Services;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SecureFileExplorer.OSINT.Server;

public static class ApiRouter
{
    public static void Route(HttpListenerContext ctx, AppConfig config, FileIndexer indexer)
    {
        try
        {
            var path = ctx.Request.Url!.AbsolutePath;

            // Recherche
            if (path == "/api/search")
            {
                var q = ctx.Request.QueryString["q"] ?? "";
                WriteJson(ctx, indexer.Search(q));
                return;
            }

            // Timeline
            if (path == "/api/timeline")
            {
                WriteJson(ctx, indexer.All()
                    .OrderBy(f => f.Created)
                    .Select(f => new
                    {
                        f.Name,
                        f.Created,
                        f.Modified,
                        f.Path
                    }));
                return;
            }

            // Graph
            if (path == "/api/graph")
            {
                var allFiles = indexer.All().ToList();
                var nodes = allFiles
                    .Select(f => new
                    {
                        id = f.Sha256,
                        label = f.Name,
                        ext = f.Extension,
                        path = f.Path
                    });

                var links = allFiles
                    .GroupBy(f => f.Extension)
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g.Skip(1)
                        .Select(f => new {
                            source = g.First().Sha256,
                            target = f.Sha256
                        }));

                WriteJson(ctx, new { nodes, links });
                return;
            }

            // Export forensic
            if (path == "/api/export")
            {
                var format = ctx.Request.QueryString["f"] ?? "json";
                if (format == "csv")
                {
                    var csv = "Name,Extension,Sha256,Created,Modified,Size\n" +
                        string.Join("\n",
                            indexer.All().Select(f =>
                                $"\"{f.Name}\",{f.Extension},{f.Sha256},{f.Created:o},{f.Modified:o},{f.Size}"));
                    WriteText(ctx, csv, "text/csv");
                    return;
                }
                WriteJson(ctx, indexer.All());
                return;
            }

            // Serve the files from the directory selected
            if (path.StartsWith("/files/"))
            {
                ServeOsintFile(ctx, config, path.Substring(7));
                return;
            }

            // All other static files (HTML, CSS, JS)
            StaticFileHandler.Handle(ctx);
        }
        catch (Exception ex)
        {
            WriteError(ctx, ex.Message);
        }
    }

    static void ServeOsintFile(HttpListenerContext ctx, AppConfig config, string relativePath)
    {
        try
        {
            var decodedPath = Uri.UnescapeDataString(relativePath);
            var fullPath = FileSandbox.Resolve(config.RootDirectory, decodedPath);

            if (!File.Exists(fullPath))
            {
                ctx.Response.StatusCode = 404;
                WriteText(ctx, "File not found");
                return;
            }

            if (!config.Allowed(fullPath))
            {
                ctx.Response.StatusCode = 403;
                WriteText(ctx, "File type not allowed");
                return;
            }

            var bytes = File.ReadAllBytes(fullPath);
            var contentType = GetContentType(fullPath);
            var fileName = Path.GetFileName(fullPath);
            var extension = Path.GetExtension(fullPath).ToLower();

            // Force the MIME type text/plain to force display in the browser
            // for the types that are normally downloaded
            if (extension is ".csv" or ".xml" or ".json" or ".md")
            {
                contentType = "text/plain; charset=utf-8";
            }

            ctx.Response.ContentType = contentType;
            ctx.Response.ContentLength64 = bytes.Length;
            ctx.Response.StatusCode = 200;

            // IMPORTANT: Do NOT set a Content-Disposition at all for certain types
            // The browser will then decide to display instead of downloading
            if (extension is not ".zip" and not ".rar" and not ".7z")
            {
                ctx.Response.AddHeader("Content-Disposition", $"inline; filename=\"{fileName}\"");
            }

            // Cache and security headers
            ctx.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            ctx.Response.AddHeader("X-Content-Type-Options", "nosniff");

            ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
            ctx.Response.Close();
        }
        catch (UnauthorizedAccessException)
        {
            ctx.Response.StatusCode = 403;
            WriteText(ctx, "Access denied");
        }
        catch (Exception ex)
        {
            ctx.Response.StatusCode = 500;
            WriteText(ctx, $"Error: {ex.Message}");
        }
    }

    static string GetContentType(string filePath)
    {
        return Path.GetExtension(filePath).ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".doc" => "application/msword",
            ".txt" => "text/plain; charset=utf-8",
            ".csv" => "text/csv; charset=utf-8",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            ".xml" => "text/xml; charset=utf-8",
            ".html" or ".htm" => "text/html; charset=utf-8",
            ".json" => "application/json; charset=utf-8",
            ".md" => "text/markdown; charset=utf-8",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".7z" => "application/x-7z-compressed",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".rtf" => "application/rtf",
            ".eml" => "message/rfc822",
            ".msg" => "application/vnd.ms-outlook",
            ".py" => "text/x-python",
            ".java" => "text/x-java-source",
            ".cpp" => "text/x-c++src",
            ".c" => "text/x-csrc",
            ".h" => "text/x-chdr",
            ".cs" => "text/x-csharp",
            ".php" => "text/x-php",
            ".sql" => "application/sql",
            ".sh" => "application/x-sh",
            ".bat" => "application/x-bat",
            ".ps1" => "application/x-powershell",
            ".vbs" => "text/vbscript",
            ".yml" => "text/x-yaml",
            ".yaml" => "text/x-yaml",
            ".ini" => "text/x-ini",
            ".cfg" => "text/x-config",
            ".log" => "text/plain",
            ".css" => "text/css; charset=utf-8",
            ".js" => "application/javascript; charset=utf-8",
            ".ico" => "image/x-icon",
            ".woff" => "font/woff",
            ".woff2" => "font/woff2",
            ".ttf" => "font/ttf",
            //_ => "text/plain; charset=utf-8",
            _ => "application/octet-stream"
        };
    }

    static void WriteJson(HttpListenerContext ctx, object obj)
    {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        var buf = Encoding.UTF8.GetBytes(json);
        ctx.Response.ContentType = "application/json";
        ctx.Response.ContentLength64 = buf.Length;
        ctx.Response.OutputStream.Write(buf, 0, buf.Length);
        ctx.Response.Close();
    }

    static void WriteText(HttpListenerContext ctx, string text, string contentType = "text/plain")
    {
        var buf = Encoding.UTF8.GetBytes(text);
        ctx.Response.ContentType = contentType;
        ctx.Response.ContentLength64 = buf.Length;
        ctx.Response.OutputStream.Write(buf, 0, buf.Length);
        ctx.Response.Close();
    }

    static void WriteError(HttpListenerContext ctx, string message)
    {
        ctx.Response.StatusCode = 500;
        WriteJson(ctx, new { error = message });
    }
}