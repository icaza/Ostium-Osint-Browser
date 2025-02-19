using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

class WebViewHandler
{
    readonly Dictionary<string, string> headersToModify = new Dictionary<string, string>();
    readonly CoreWebView2 webView;
    public bool IsWebViewReady => webView != null;

    readonly HashSet<string> blockedDomains = new HashSet<string>();
    readonly Dictionary<string, string> redirectRules = new Dictionary<string, string>();

    public WebViewHandler(CoreWebView2 webView, string jsonFilePath)
    {
        this.webView = webView;
        LoadSettings(jsonFilePath);
        webView.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        webView.WebResourceRequested += ModifyHttpHeaders;
    }

    void LoadSettings(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            Console.WriteLine("⚠ File JSON not found !");
            return;
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        JsonDocument doc = JsonDocument.Parse(jsonContent);

        try
        {
            if (!doc.RootElement.TryGetProperty("FloodSetting", out JsonElement settings))
            {
                Console.WriteLine("⚠ Key 'FloodSetting' empty !");
                return;
            }

            if (settings.TryGetProperty("BlockedDomains", out JsonElement blocked))
            {
                blockedDomains.Clear();
                foreach (JsonElement domain in blocked.EnumerateArray())
                {
                    blockedDomains.Add(domain.GetString());
                }
            }

            if (settings.TryGetProperty("RedirectRules", out JsonElement redirects))
            {
                redirectRules.Clear();
                foreach (JsonProperty rule in redirects.EnumerateObject())
                {
                    redirectRules[rule.Name] = rule.Value.GetString();
                }
            }

            headersToModify["ACCEPT-LANGUAGE"] = settings.TryGetProperty("FakeLang", out JsonElement lang) ? lang.GetString() : "fr,fr-FR;q=0.9,en;q=0.8";
            headersToModify["COOKIE"] = settings.TryGetProperty("FakeCookie", out JsonElement cookie) ? cookie.GetString() : "null";
            headersToModify["SEC-CH-PREFERS-COLOR-SCHEME"] = settings.TryGetProperty("FakeColorShem", out JsonElement color) ? color.GetString() : "light";
            headersToModify["SEC-CH-UA"] = settings.TryGetProperty("FakeBrowser", out JsonElement browser) ? browser.GetString() : "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"131\"";
            headersToModify["SEC-CH-UA-ARCH"] = settings.TryGetProperty("FakeArchitecture", out JsonElement arch) ? arch.GetString() : "x64";
            headersToModify["SEC-CH-UA-FULL-VERSION"] = settings.TryGetProperty("FakeFullVersion", out JsonElement fullVersion) ? fullVersion.GetString() : "131.0.6778.140";
            headersToModify["SEC-CH-UA-FULL-VERSION-LIST"] = settings.TryGetProperty("FakeFullVersionList", out JsonElement fullVersionList) ? fullVersionList.GetString() : "";
            headersToModify["SEC-CH-UA-PLATFORM"] = settings.TryGetProperty("FakeSystem", out JsonElement system) ? system.GetString() : "Linux x86_64";
            headersToModify["SEC-CH-UA-PLATFORM-VERSION"] = settings.TryGetProperty("FakePlateform", out JsonElement platform) ? platform.GetString() : "23.10";
            headersToModify["USER-AGENT"] = settings.TryGetProperty("FakeUserAgent", out JsonElement userAgent) ? userAgent.GetString() : "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36";
            headersToModify["REFERER"] = settings.TryGetProperty("FakeReferer", out JsonElement referer) ? referer.GetString() : "https://www.unknow.com/";
            headersToModify["SEC-CH-VIEWPORT-WIDTH"] = settings.TryGetProperty("FakeWidth", out JsonElement width) ? width.GetString() : "1920";
            headersToModify["SEC-CH-VIEWPORT-HEIGHT"] = settings.TryGetProperty("FakeHeight", out JsonElement height) ? height.GetString() : "1080";
            headersToModify["VIEWPORT-WIDTH"] = headersToModify["SEC-CH-VIEWPORT-WIDTH"];
        }
        finally
        {
            doc.Dispose();
        }
    }

    void ModifyHttpHeaders(object sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        var request = e.Request;
        var uri = new Uri(request.Uri);

        if (blockedDomains.Contains(uri.Host))
        {
            Console.WriteLine($"🚫 Blocked request to : {uri.Host}");
            e.Response = webView.Environment.CreateWebResourceResponse(null, 403, "Forbidden", "Content-Type: text/plain");
            return;
        }

        if (redirectRules.TryGetValue(uri.Host, out string newDomain))
        {
            string newUrl = uri.Scheme + "://" + newDomain + uri.PathAndQuery;
            Console.WriteLine($"🔀 Redirection : {uri.Host} → {newDomain}");
            request.Uri = newUrl;
        }

        foreach (var header in headersToModify)
        {
            string sanitizedValue = SanitizeHeader(header.Key, header.Value);
            if (!string.IsNullOrEmpty(sanitizedValue))
            {
                request.Headers.SetHeader(header.Key, sanitizedValue);
            }
        }
    }

    static string SanitizeHeader(string headerName, string headerValue)
    {
        if (string.IsNullOrWhiteSpace(headerValue)) return string.Empty;

        string[] blacklistedPatterns =
        {
            "<script.*?>.*?</script>",  // Block <script>...</script>
            "javascript:.*?",           // Block javascript:
            "vbscript:.*?",             // Block vbscript:
            "data:text/html.*?",        // Block data: URLs
            "onerror=.*?",              // Block onerror=
            "alert\\(.*?\\)",           // Block alert(...)
            "document\\.cookie",        // Block document.cookie
            "document\\.write"          // Block document.write
        };

        foreach (var pattern in blacklistedPatterns)
        {
            if (Regex.IsMatch(headerValue, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {
                return string.Empty;
            }
        }

        if (headerName.Equals("REFERER", StringComparison.OrdinalIgnoreCase) ||
            headerName.Equals("ORIGIN", StringComparison.OrdinalIgnoreCase))
        {
            return IsValidUrl(headerValue) ? headerValue.Trim() : string.Empty;
        }

        if (headerName.Equals("USER-AGENT", StringComparison.OrdinalIgnoreCase))
        {
            return IsValidUserAgent(headerValue) ? headerValue.Trim() : string.Empty;
        }

        return Regex.Replace(headerValue, @"[^\w\s\-/().,;:]", "").Trim();
    }

    static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    static bool IsValidUserAgent(string userAgent)
    {
        return !string.IsNullOrWhiteSpace(userAgent) && userAgent.Length > 10 && userAgent.Length < 512;
    }
}
