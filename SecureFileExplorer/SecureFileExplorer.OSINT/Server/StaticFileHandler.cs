using System.Net;

namespace SecureFileExplorer.OSINT.Server;

public static class StaticFileHandler
{
    static readonly string WebRoot = "www";

    public static void Handle(HttpListenerContext ctx)
    {
        try
        {
            var requestPath = ctx.Request.Url!.AbsolutePath.TrimStart('/');

            if (string.IsNullOrEmpty(requestPath) || requestPath == "/")
                requestPath = "index.html";

            var filePath = Path.Combine(WebRoot, requestPath.Replace('/', Path.DirectorySeparatorChar));

            var fullPath = Path.GetFullPath(filePath);
            var fullWebRoot = Path.GetFullPath(WebRoot);

            if (!fullPath.StartsWith(fullWebRoot, StringComparison.OrdinalIgnoreCase))
            {
                SendError(ctx, 403, "Forbidden");
                return;
            }

            if (!File.Exists(fullPath))
            {
                SendError(ctx, 404, "File not found");
                return;
            }

            var bytes = File.ReadAllBytes(fullPath);
            ctx.Response.ContentType = GetContentType(fullPath);
            ctx.Response.ContentLength64 = bytes.Length;
            ctx.Response.StatusCode = 200;

            if (IsCacheable(fullPath))
            {
                ctx.Response.AddHeader("Cache-Control", "public, max-age=3600");
            }

            ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
            ctx.Response.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Static file handler error: {ex.Message}");
            SendError(ctx, 500, "Internal server error");
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

    static bool IsCacheable(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLower();
        return ext is ".css" or ".js" or ".png" or ".jpg" or ".jpeg"
            or ".gif" or ".svg" or ".ico" or ".woff" or ".woff2" or ".ttf";
    }

    static void SendError(HttpListenerContext ctx, int statusCode, string message)
    {
        try
        {
            ctx.Response.StatusCode = statusCode;
            var html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Error {statusCode}</title>
                <style>
                    body {{ 
                        font-family: 'Segoe UI', sans-serif; 
                        background: #0b0f19; 
                        color: #eee; 
                        display: flex; 
                        justify-content: center; 
                        align-items: center; 
                        height: 100vh; 
                        margin: 0;
                    }}
                    .error-container {{
                        text-align: center;
                        padding: 40px;
                        background: #111421;
                        border-radius: 8px;
                        box-shadow: 0 4px 20px rgba(0,0,0,0.3);
                    }}
                    h1 {{ color: #e74c3c; font-size: 72px; margin: 0; }}
                    p {{ color: #888; font-size: 18px; }}
                </style>
            </head>
            <body>
                <div class='error-container'>
                    <h1>{statusCode}</h1>
                    <p>{message}</p>
                </div>
            </body>
            </html>";
            var bytes = System.Text.Encoding.UTF8.GetBytes(html);
            ctx.Response.ContentType = "text/html; charset=utf-8";
            ctx.Response.ContentLength64 = bytes.Length;
            ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
            ctx.Response.Close();
        }
        catch
        {
            try { ctx.Response.Close(); } catch { }
        }
    }
}