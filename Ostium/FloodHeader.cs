using System;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

public class FloodHeader
{
    readonly Random random = new Random();
    readonly CoreWebView2 webView;

    public FloodHeader(CoreWebView2 webView)
    {
        this.webView = webView ?? throw new ArgumentNullException(nameof(webView));
    }

    public async Task FloodHeaderAsync()
    {
        string userAgent = GenerateRandomUserAgent();
        var (screenWidth, screenHeight) = GenerateRandomResolution();
        string platform = GenerateRandomPlatform();
        int timezoneOffset = GenerateRandomTimezoneOffset();

        webView.Settings.UserAgent = userAgent;

        await FloodHeaderScripts(screenWidth, screenHeight, platform, timezoneOffset);
    }

    async Task FloodHeaderScripts(int width, int height, string platform, int timezoneOffset)
    {
        string script = $@"
        (function() {{
            Object.defineProperty(window.screen, 'width', {{ get: () => {width} }});
            Object.defineProperty(window.screen, 'height', {{ get: () => {height} }});
            Object.defineProperty(window, 'innerWidth', {{ get: () => {width} }});
            Object.defineProperty(window, 'innerHeight', {{ get: () => {height} }});

            Object.defineProperty(navigator, 'platform', {{ get: () => '{platform}' }});

            Date.prototype.getTimezoneOffset = function() {{ return {timezoneOffset}; }};
        }})();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    string GenerateRandomUserAgent()
    {
        string[] userAgents =
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.3",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.1.1 Safari/605.1.1",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36 Edg/133.0.0.",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:134.0) Gecko/20100101 Firefox/134.0",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 14.7; rv:135.0) Gecko/20100101 Firefox/135.0",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",
            "Mozilla/5.0 (X11; Linux i686; rv:135.0) Gecko/20100101 Firefox/135.0",
            "Mozilla/5.0 (X11; Ubuntu; Linux i686; rv:135.0) Gecko/20100101 Firefox/135.0"
        };

        return userAgents[random.Next(userAgents.Length)];
    }

    (int, int) GenerateRandomResolution()
    {
        (int, int)[] resolutions =
        {
            (1920, 1080), (1366, 768), (1440, 900), (1600, 900), (1280, 720),
            (2560, 1440), (3840, 2160), (1024, 768), (1280, 1024)
        };

        return resolutions[random.Next(resolutions.Length)];
    }

    string GenerateRandomPlatform()
    {
        string[] platforms =
        {
            "Win32", "Linux x86_64", "MacIntel", "Android", "iPhone", "Linux i686", "Ubuntu"
        };

        return platforms[random.Next(platforms.Length)];
    }

    int GenerateRandomTimezoneOffset()
    {
        int[] offsets = { -720, -480, -300, -240, -180, 0, 180, 300, 480, 720 };
        return offsets[random.Next(offsets.Length)];
    }
}
