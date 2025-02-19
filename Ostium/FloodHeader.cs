using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;

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
        string language = GenerateRandomLanguage();
        int hardwareConcurrency = GenerateRandomHardwareConcurrency();
        float deviceMemory = GenerateRandomDeviceMemory();
        int maxTouchPoints = GenerateRandomMaxTouchPoints();
        string webdriver = GenerateRandomWebdriver();
        string connectionType = GenerateRandomConnectionType();

        webView.Settings.UserAgent = userAgent;

        await FloodHeaderScripts(screenWidth, screenHeight, platform, timezoneOffset, language, hardwareConcurrency, deviceMemory, maxTouchPoints, webdriver, connectionType);
    }

    async Task FloodHeaderScripts(int width, int height, string platform, int timezoneOffset, string language, int hardwareConcurrency, float deviceMemory, int maxTouchPoints, string webdriver, string connectionType)
    {
        string script = $@"
    (function() {{
        // Manipulating Screen and Window Properties
        Object.defineProperty(window.screen, 'width', {{ get: () => {width} }});
        Object.defineProperty(window.screen, 'height', {{ get: () => {height} }});
        Object.defineProperty(window, 'innerWidth', {{ get: () => {width} }});
        Object.defineProperty(window, 'innerHeight', {{ get: () => {height} }});

        // Manipulate platform and time zone
        Object.defineProperty(navigator, 'platform', {{ get: () => '{platform}' }});
        Object.defineProperty(navigator, 'oscpu', {{ get: () => '{platform}' }});
        Date.prototype.getTimezoneOffset = function() {{ return {timezoneOffset}; }};

        // Manipulating languages
        Object.defineProperty(navigator, 'language', {{ get: () => '{language}' }});
        Object.defineProperty(navigator, 'languages', {{ get: () => ['{language}', 'en-US', 'en'] }});

        // Manipulating material properties
        Object.defineProperty(navigator, 'hardwareConcurrency', {{ get: () => {hardwareConcurrency} }});
        Object.defineProperty(navigator, 'deviceMemory', {{ get: () => {deviceMemory} }});
        Object.defineProperty(navigator, 'maxTouchPoints', {{ get: () => {maxTouchPoints} }});

        // Manipulating Webdriver (to avoid automation detection))
        Object.defineProperty(navigator, 'webdriver', {{ get: () => {webdriver.ToLower()} }});

        // Manipulate login information
        Object.defineProperty(navigator, 'connection', {{
            get: () => ({{
                downlink: 10,
                effectiveType: '{connectionType}',
                rtt: 100,
                saveData: false,
                type: '{connectionType}'
            }})
        }});

        // Manipulating the canvas to avoid fingerprinting
        // const canvasPrototype = HTMLCanvasElement.prototype;
        // const originalGetContext = canvasPrototype.getContext;
        // canvasPrototype.getContext = function(contextType) {{
        //     if (contextType === '2d') {{
        //         const context = originalGetContext.apply(this, arguments);
        //         context.fillText = () => {{}}; // Désactiver fillText
        //         return context;
        //     }}
        //     return originalGetContext.apply(this, arguments);
        // }};

        // Disable or manipulate the Web Audio API
        if (window.AudioContext || window.webkitAudioContext) {{
            const OriginalAudioContext = window.AudioContext || window.webkitAudioContext;

            window.AudioContext = function() {{
                const audioContext = new OriginalAudioContext();

                // Generate random values ​​for audio properties
                const originalCreateAnalyser = audioContext.createAnalyser;
                audioContext.createAnalyser = function() {{
                    const analyser = originalCreateAnalyser.apply(audioContext, arguments);
                    analyser.frequencyBinCount = Math.floor(Math.random() * 2048); // Random value
                    return analyser;
                }};

                const originalCreateOscillator = audioContext.createOscillator;
                audioContext.createOscillator = function() {{
                    const oscillator = originalCreateOscillator.apply(audioContext, arguments);
                    oscillator.frequency.value = Math.random() * 1000; // Random value
                    return oscillator;
                }};

                return audioContext;
            }};
        }}
    }})();
    ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    string GenerateRandomUserAgent()
    {
        string[] userAgents =
        {
            "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.3",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.1.1 Safari/605.1.1",
            "Mozilla/5.0 (X11; Ubuntu; Linux i686; rv:135.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36 Edg/133.0.0.",
            "Mozilla/5.0 (Windows NT 6.1; rv:109.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 14.7; rv:135.0) Gecko/20100101 Firefox/134.0",
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
            "Win32", "Linux x86_64", "MacIntel", "Android", "iPhone", "Linux i686", "Ubuntu", "Fedora"
        };

        return platforms[random.Next(platforms.Length)];
    }

    int GenerateRandomTimezoneOffset()
    {
        int[] offsets = { -720, -480, -300, -240, -180, 0, 180, 300, 480, 720 };
        return offsets[random.Next(offsets.Length)];
    }

    string GenerateRandomLanguage()
    {
        string[] languages = { "en-US", "fr-FR", "es-ES", "de-DE", "ja-JP" };
        return languages[random.Next(languages.Length)];
    }

    int GenerateRandomHardwareConcurrency()
    {
        return random.Next(2, 16); // Between 2 and 16 cores
    }

    float GenerateRandomDeviceMemory()
    {
        float[] memories = { 2, 4, 8, 16, 32 };
        return memories[random.Next(memories.Length)];
    }

    int GenerateRandomMaxTouchPoints()
    {
        return random.Next(0, 10); // Between 0 and 10 touch points
    }

    string GenerateRandomWebdriver()
    {
        return random.Next(2) == 0 ? "false" : "true"; // Randomly true or false
    }

    string GenerateRandomConnectionType()
    {
        string[] types = { "wifi", "cellular", "ethernet", "none" };
        return types[random.Next(types.Length)];
    }
}