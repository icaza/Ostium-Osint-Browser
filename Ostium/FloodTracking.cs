using System;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

public class FloodTracking
{
    readonly Random random = new Random();
    readonly CoreWebView2 webView;

    public FloodTracking(CoreWebView2 webView)
    {
        this.webView = webView ?? throw new ArgumentNullException(nameof(webView));
    }

    public async Task FloodTrackingAsync()
    {
        string userAgent = GenerateRandomUserAgent();
        var (screenWidth, screenHeight) = GenerateRandomResolution();
        string platform = GenerateRandomPlatform();
        int timezoneOffset = GenerateRandomTimezoneOffset();

        webView.Settings.UserAgent = userAgent;

        await InjectAntiTrackingScripts(screenWidth, screenHeight, platform, timezoneOffset);
    }

    async Task InjectAntiTrackingScripts(int width, int height, string platform, int timezoneOffset)
    {
        string script = $@"
        (function() {{
            // Fausser la résolution d'écran
            Object.defineProperty(window.screen, 'width', {{ get: () => {width} }});
            Object.defineProperty(window.screen, 'height', {{ get: () => {height} }});
            Object.defineProperty(window, 'innerWidth', {{ get: () => {width} }});
            Object.defineProperty(window, 'innerHeight', {{ get: () => {height} }});

            // Changer OS et User-Agent
            Object.defineProperty(navigator, 'platform', {{ get: () => '{platform}' }});

            // Modifier le fuseau horaire
            Date.prototype.getTimezoneOffset = function() {{ return {timezoneOffset}; }};
        }})();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    string GenerateRandomUserAgent()
    {
        string[] userAgents =
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36"
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
            "Win32", "Linux x86_64", "MacIntel", "Android", "iPhone"
        };

        return platforms[random.Next(platforms.Length)];
    }

    int GenerateRandomTimezoneOffset()
    {
        int[] offsets = { -720, -480, -300, -240, -180, 0, 180, 300, 480, 720 };
        return offsets[random.Next(offsets.Length)];
    }
}
