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

        await EnableWebGLProtectionAsync();
        await FloodHeaderScripts(screenWidth, screenHeight, platform, timezoneOffset, language, hardwareConcurrency, deviceMemory, maxTouchPoints, webdriver, connectionType);
        await EnableAudioContextProtectionAsync();
        await EnableCanvasAndWebGLProtectionAsync();
        await EnableWebRTCAndBatteryAndSpeechProtectionAsync();
    }

    public async Task EnableWebGLProtectionAsync()
    {
        string script = @"
        (function() {
            console.log('[Fingerprint Defender] Advanced WebGL Protection Enabled');

            // Fake GPU IDs
            const fakeVendor = 'FakeVendor';
            const fakeRenderer = 'FakeRenderer';

            function spoofWebGL() {
                const getParameterProxy = new Proxy(WebGLRenderingContext.prototype.getParameter, {
                    apply: function(target, thisArg, args) {
                        const param = args[0];

                        if (param === 0x1F00 || param === 0x1F01) { 
                            // VENDOR or RENDERER standard
                            return fakeVendor;
                        }
                        if (param === 0x9245) { 
                            // UNMASKED_VENDOR_WEBGL
                            return fakeVendor;
                        }
                        if (param === 0x9246) { 
                            // UNMASKED_RENDERER_WEBGL
                            return fakeRenderer;
                        }

                        return Reflect.apply(target, thisArg, args);
                    }
                });

                const getExtensionProxy = new Proxy(WebGLRenderingContext.prototype.getExtension, {
                    apply: function(target, thisArg, args) {
                        if (args[0] === 'WEBGL_debug_renderer_info') {
                            return null; // Prevents access to the extension
                        }
                        return Reflect.apply(target, thisArg, args);
                    }
                });

                WebGLRenderingContext.prototype.getParameter = getParameterProxy;
                WebGL2RenderingContext.prototype.getParameter = getParameterProxy;
                WebGLRenderingContext.prototype.getExtension = getExtensionProxy;
                WebGL2RenderingContext.prototype.getExtension = getExtensionProxy;
            }

            // Execute as soon as possible
            spoofWebGL();
            console.log('[Fingerprint Defender] UNMASKED_VENDOR & UNMASKED_RENDERER modified !');
        })();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
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
        }})();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    public async Task EnableAudioContextProtectionAsync()
    {
        string script = $@"
        (function() {{
            console.log('[Fingerprint Defender] AudioContext Protection Enabled');

            // Change the AudioContext footprint
            const AudioContext = window.AudioContext || window.webkitAudioContext;
            if (AudioContext) {{
                const originalGetChannelData = AudioBuffer.prototype.getChannelData;
                
                AudioBuffer.prototype.getChannelData = function() {{
                    const data = originalGetChannelData.apply(this, arguments);
                    for (let i = 0; i < data.length; i += 100) {{ 
                        data[i] += (Math.random() * 0.0001) - 0.00005; // Light disturbance
                    }}
                    return data;
                }};
        
                const originalCreateAnalyser = AudioContext.prototype.createAnalyser;
                AudioContext.prototype.createAnalyser = function() {{
                    const analyser = originalCreateAnalyser.apply(this, arguments);
                    const originalGetFloatFrequencyData = analyser.getFloatFrequencyData;
        
                    analyser.getFloatFrequencyData = function(array) {{
                        originalGetFloatFrequencyData.apply(this, [array]);
                        for (let i = 0; i < array.length; i++) {{
                            array[i] += (Math.random() * 0.1) - 0.05; // Changing frequencies
                        }}
                    }};
        
                    return analyser;
                }};
            }}

            //// Refresh fingerprint on every load
            //function refreshFingerprint() {{
            //    console.log('[Fingerprint Defender] New audio hash generated !');
            //    document.body.style.opacity = '0'; 
            //    setTimeout(() => {{
            //        document.body.style.opacity = '1';
            //    }}, 500);
            //}}
        
            //window.onload = function() {{
            //    refreshFingerprint();
            //}};
        }})();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    public async Task EnableCanvasAndWebGLProtectionAsync()
    {
        string script = @"
        (function() {
            console.log('[Fingerprint Defender] Canvas Protection activated');

            const getContext = HTMLCanvasElement.prototype.getContext;

            HTMLCanvasElement.prototype.getContext = function(type, attrs) {
                const ctx = getContext.apply(this, arguments);
                if (type === '2d') {
                    const originalToDataURL = ctx.toDataURL;
                    const originalGetImageData = ctx.getImageData;
                    const originalPutImageData = ctx.putImageData;
                    const originalToBlob = ctx.toBlob;
                    const originalFillText = ctx.fillText;
                    const originalFillRect = ctx.fillRect;

                    // Function to add noise
                    function addNoise(data) {
                        const factor = 20; // Noise intensity
                        for (let i = 0; i < data.length; i += 4) {
                            data[i] += (Math.random() * factor) - (factor / 2);     // Red
                            data[i + 1] += (Math.random() * factor) - (factor / 2); // Green
                            data[i + 2] += (Math.random() * factor) - (factor / 2); // Blue
                        }
                    }

                    // Function to apply light distortions
                    function distortCanvas(ctx) {
                        const { width, height } = ctx.canvas;
                        const imageData = ctx.getImageData(0, 0, width, height);
                        addNoise(imageData.data);
                        ctx.putImageData(imageData, 0, 0);
                    }

                    // Edit text and shapes to add noise
                    ctx.fillText = function(text, x, y, maxWidth) {
                        const offsetX = Math.random() * 2 - 1; 
                        const offsetY = Math.random() * 2 - 1;
                        originalFillText.apply(this, [text, x + offsetX, y + offsetY, maxWidth]);
                    };

                    ctx.fillRect = function(x, y, width, height) {
                        const offsetX = Math.random() * 2 - 1;
                        const offsetY = Math.random() * 2 - 1;
                        originalFillRect.apply(this, [x + offsetX, y + offsetY, width, height]);
                    };

                    ctx.toDataURL = function() {
                        distortCanvas(ctx);
                        return originalToDataURL.apply(this, arguments);
                    };

                    ctx.getImageData = function(x, y, width, height) {
                        const imageData = originalGetImageData.apply(this, arguments);
                        addNoise(imageData.data);
                        return imageData;
                    };

                    ctx.putImageData = function(imageData, x, y) {
                        addNoise(imageData.data);
                        originalPutImageData.apply(this, arguments);
                    };

                    ctx.toBlob = function() {
                        distortCanvas(ctx);
                        return originalToBlob.apply(this, arguments);
                    };
                }
                return ctx;
            };

            // Modify WebGL to change GPU fingerprint
            const getParameter = WebGLRenderingContext.prototype.getParameter;
            WebGLRenderingContext.prototype.getParameter = function(parameter) {
                if (parameter === 0x1F00) return 'RandomVendor_' + Math.random().toString(36).substring(7);
                if (parameter === 0x1F01) return 'RandomRenderer_' + Math.random().toString(36).substring(7);
                return getParameter.apply(this, arguments);
            };

            //// Generate a new hash on each load
            //function refreshFingerprint() {
            //    console.log('[Fingerprint Defender] New hash generated !');
            //    document.body.style.opacity = '0'; 
            //    setTimeout(() => {
            //        document.body.style.opacity = '1';
            //    }, 500);
            //}

            //window.onload = function() {
            //    refreshFingerprint();
            //};
        })();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    public async Task EnableWebRTCAndBatteryAndSpeechProtectionAsync()
    {
        string script = @"
        (function() {
            console.log('[Fingerprint Defender] WebRTC, Battery API and Speech Synthesis protection enabled');

            // WebRTC Leak Protection
            const originalRTCPeerConnection = window.RTCPeerConnection;
            window.RTCPeerConnection = function() {
                console.warn('[Fingerprint Defender] WebRTC blocked !');
                return null;
            };

            // Battery API Protection
            if (navigator.getBattery) {
                navigator.getBattery = function() {
                    console.warn('[Fingerprint Defender] API Battery Blocked !');
                    return new Promise(resolve => resolve({ level: 1, charging: true, chargingTime: 0, dischargingTime: Infinity }));
                };
            }

            // Speech Synthesis Protection
            const originalGetVoices = window.speechSynthesis.getVoices;
            window.speechSynthesis.getVoices = function() {
                console.warn('[Fingerprint Defender] Speech Synthesis blocked !');
                return [{ name: 'FakeVoice', lang: 'en-US', localService: false, voiceURI: 'fake' }];
            };
        })();
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