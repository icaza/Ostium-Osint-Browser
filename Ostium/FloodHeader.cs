// The method used against fingerprinting is aggressive; website functionality is disrupted, and you are flagged as a robot.
// A significant balance must be struck between active defense and 'normal' browser operation.

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
        bool webdriver = false; // Always false to avoid detection
        string connectionType = GenerateRandomConnectionType();
        // Random renderer selection
        string fakeRenderer = GenerateRandomRenderer();
        string fakeVendor = GenerateVendorForRenderer(fakeRenderer);

        webView.Settings.UserAgent = userAgent;

        // Injecting the scripts in the correct order
        await InjectCoreProtectionAsync();
        await FloodHeaderScripts(screenWidth, screenHeight, platform, timezoneOffset, language, hardwareConcurrency, deviceMemory, maxTouchPoints, webdriver, connectionType);
        await EnableWebGLProtectionAsync(fakeRenderer, fakeVendor);
        await EnableWebGL1ProtectionAsync(fakeRenderer, fakeVendor);
        await EnableCanvasProtectionAsync();
        await EnableAudioContextProtectionAsync();
        await EnableWebRTCAndBatteryAndSpeechProtectionAsync();
        await EnableAdvancedFingerprintProtectionAsync();
    }

    async Task InjectCoreProtectionAsync()
    {
        string script = @"
        (function() {
            'use strict';
            console.log('[Fingerprint Defender] Core Protection Enabled');

            // Remove traces of automation
            delete window.cdc_adoQpoasnfa76pfcZLmcfl_Array;
            delete window.cdc_adoQpoasnfa76pfcZLmcfl_Promise;
            delete window.cdc_adoQpoasnfa76pfcZLmcfl_Symbol;

            // Hide WebDriver properties
            const hideWebDriverProperties = () => {
                try {
                    // Strategy 1: Direct removal (if possible)
                    try {
                        delete navigator.webdriver;
                        console.log('[Fingerprint Defender] WebDriver deleted successfully');
                    } catch (e) {}
                    
                    // Strategy 2: Conditional Redefinition
                    try {
                        const descriptor = Object.getOwnPropertyDescriptor(navigator, 'webdriver');
                        if (!descriptor || descriptor.configurable) {
                            Object.defineProperty(navigator, 'webdriver', {
                                get: () => undefined,
                                configurable: false,
                                enumerable: false
                            });
                            console.log('[Fingerprint Defender] WebDriver redefined successfully');
                        }
                    } catch (e) {}
                    
                    // Strategy 3: Advanced masking for detection
                    try {
                        // Make the property non-enumerable
                        Object.defineProperty(navigator, 'webdriver', {
                            value: undefined,
                            enumerable: false,
                            configurable: true,
                            writable: true
                        });
                    } catch (e) {}
                    
                    // Final check
                    if (navigator.webdriver === undefined) {
                        console.log('[Fingerprint Defender] WebDriver protection: ACTIVE');
                    } else {
                        console.log('[Fingerprint Defender] WebDriver protection: LIMITED');
                    }
                    
                } catch (error) {
                    console.log('[Fingerprint Defender] WebDriver protection: Basic mode');
                }
            };
            
            hideWebDriverProperties();

            // Prevent detection via window.chrome
            if (!window.chrome) {
                window.chrome = {
                    runtime: {},
                    loadTimes: function() {},
                    csi: function() {},
                    app: {}
                };
            }

            // Hide automation permissions
            const originalQuery = window.navigator.permissions.query;
            window.navigator.permissions.query = (parameters) => (
                parameters.name === 'notifications' ?
                    Promise.resolve({ state: Notification.permission }) :
                    originalQuery(parameters)
            );

            // Plugin Protection
            const protectPluginsGuaranteed = () => {
                try {
                    const fakePlugins = [
                        {
                            name: 'Chrome PDF Plugin',
                            filename: 'internal-pdf-viewer',
                            description: 'Portable Document Format',
                            length: 1,
                            0: {
                                type: 'application/x-google-chrome-pdf',
                                suffixes: 'pdf',
                                description: 'Portable Document Format'
                            }
                        }
                    ];
            
                    // Direct method
                    Object.defineProperty(navigator, 'plugins', {
                        get: () => fakePlugins,
                        configurable: true,
                        enumerable: true
                    });
            
                    console.log('[Fingerprint Defender] Plugin protection active');
            
                } catch (error) {
                    // no message
                }
            };
            
            protectPluginsGuaranteed();

        })();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    async Task FloodHeaderScripts(int width, int height, string platform, int timezoneOffset, string language, int hardwareConcurrency, float deviceMemory, int maxTouchPoints, bool webdriver, string connectionType)
    {
        // Consistency: maxTouchPoints should be 0 for desktop
        if (platform.Contains("Win") || platform.Contains("Linux") || platform.Contains("Mac"))
        {
            maxTouchPoints = 0;
        }

        string script = $@"
        (function() {{
            'use strict';           

            // Screen Protection
            const protectScreen = () => {{
                const WIDTH = {width};
                const HEIGHT = {height};
                
                try {{
                    // Replace screen object completely
                    window.screen = {{
                        width: WIDTH,
                        height: HEIGHT,
                        availWidth: WIDTH - 20,
                        availHeight: HEIGHT - 80,
                        colorDepth: 24,
                        pixelDepth: 24,
                        availLeft: 0,
                        availTop: 0,
                        orientation: {{
                            type: WIDTH > HEIGHT ? 'landscape-primary' : 'portrait-primary',
                            angle: WIDTH > HEIGHT ? 0 : 90,
                            lock: () => Promise.resolve(),
                            unlock: () => {{}}
                        }}
                    }};
            
                    // Override key window properties
                    Object.defineProperty(window, 'outerWidth', {{ get: () => WIDTH, configurable: true }});
                    Object.defineProperty(window, 'outerHeight', {{ get: () => HEIGHT, configurable: true }});
                    Object.defineProperty(window, 'innerWidth', {{ get: () => WIDTH - 20, configurable: true }});
                    Object.defineProperty(window, 'innerHeight', {{ get: () => HEIGHT - 100, configurable: true }});
            
                    console.log(`[Fingerprint Defender] Screen: ${{WIDTH}}x${{HEIGHT}}`);
                    
                }} catch (error) {{
                    console.warn('Screen protection limited');
                }}
            }};
            
            protectScreen();

            // Manipulate platform and time zone
            Object.defineProperty(navigator, 'platform', {{ 
                get: () => '{platform}',
                configurable: true
            }});
            Object.defineProperty(navigator, 'oscpu', {{ 
                get: () => '{platform}',
                configurable: true
            }});
            
            const originalGetTimezoneOffset = Date.prototype.getTimezoneOffset;
            Date.prototype.getTimezoneOffset = function() {{ 
                return {timezoneOffset}; 
            }};

            // Manipulating languages
            Object.defineProperty(navigator, 'language', {{ 
                get: () => '{language}',
                configurable: true
            }});
            Object.defineProperty(navigator, 'languages', {{ 
                get: () => ['{language}', 'en-US'],
                configurable: true
            }});

            // Manipulating hardware properties
            Object.defineProperty(navigator, 'hardwareConcurrency', {{ 
                get: () => {hardwareConcurrency},
                configurable: true
            }});
            Object.defineProperty(navigator, 'deviceMemory', {{ 
                get: () => {deviceMemory},
                configurable: true
            }});
            Object.defineProperty(navigator, 'maxTouchPoints', {{ 
                get: () => {maxTouchPoints},
                configurable: true
            }});

            // Manipulating Webdriver
            try {{
                // Clean deletion attempt
                if (navigator.webdriver !== undefined) {{
                    delete navigator.webdriver;
                }}
            }} catch (e) {{
                // Fallback 1: Modification via prototype (less detectable)
                try {{
                    Object.defineProperty(Object.getPrototypeOf(navigator), 'webdriver', {{
                        get: () => undefined,
                        configurable: true,
                        enumerable: false
                    }});
                }} catch (e2) {{
                    // Fallback 2: Override at the global level
                    const originalDefineProperty = Object.defineProperty;
                    Object.defineProperty = function(obj, prop, descriptor) {{
                        if (obj === navigator && prop === 'webdriver') {{
                            return obj; // Silently ignores attempts at definition
                        }}
                        return originalDefineProperty.call(this, obj, prop, descriptor);
                    }};
                }}
            }}

            // Connection Protection
            if (navigator.connection) {{
                try {{
                    navigator.connection = {{
                        downlink: 10,
                        effectiveType: '{connectionType}',
                        rtt: 50, 
                        saveData: false,
                        type: '{connectionType}',
                        addEventListener: navigator.connection.addEventListener?.bind(navigator.connection),
                        removeEventListener: navigator.connection.removeEventListener?.bind(navigator.connection),
                        onchange: null
                    }};
                    console.log(`[Fingerprint Defender] Connection protection: ${connectionType}`);
                }} catch (error) {{
                    console.log('[Fingerprint Defender] Connection protection failed:', error.message);
                }}
            }} else {{
                console.log('[Fingerprint Defender] navigator.connection not available');
            }}

            // doNotTrack
            Object.defineProperty(navigator, 'doNotTrack', {{
                get: () => null,
                configurable: true
            }});

            // Vendor et product
            Object.defineProperty(navigator, 'vendor', {{
                get: () => 'Google Inc.',
                configurable: true
            }});
            Object.defineProperty(navigator, 'vendorSub', {{
                get: () => '',
                configurable: true
            }});

            // Simple Font Fingerprint Protection
            const simpleFontProtection = () => {{
                try {{
                    // Add random noise to font measurements
                    const addNoise = () => Math.floor(Math.random() * 4 - 2); // -2 to +2
                    
                    // Protect offset measurements
                    ['offsetWidth', 'offsetHeight', 'clientWidth', 'clientHeight'].forEach(prop => {{
                        const original = Object.getOwnPropertyDescriptor(HTMLElement.prototype, prop);
                        if (original) {{
                            Object.defineProperty(HTMLElement.prototype, prop, {{
                                get: function () {{
                                    const value = original.get.call(this);
                                    return Math.random() < 0.3 ? value + addNoise() : value;
                                }},
                                configurable: true
                            }});
                        }}
                    }});
                    
                    console.log('[Fingerprint Defender] Simple font protection active');
                }} catch (e) {{
                    console.log('[Fingerprint Defender] Font protection error:', e);
                }}
            }};
            
            simpleFontProtection();

            console.log('[Fingerprint Defender] Navigator properties modified');
        }})();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    public async Task EnableWebGLProtectionAsync(string fakeRenderer, string fakeVendor)
    {
        // Generate a random seed for this profile
        double noiseSeed = random.NextDouble();

        string script = $@"
        (function() {{
            'use strict';
            console.log('[Fingerprint Defender] Advanced WebGL Protection Enabled');

            // Seed for noise (only one per session)
            const noiseSeed = {noiseSeed.ToString(System.Globalization.CultureInfo.InvariantCulture)};

            // Fake GPU IDs
            const fakeVendor = '{fakeVendor}';
            const fakeRenderer = '{fakeRenderer}';

            function spoofWebGL() {{
                // hook getParameter to modify text AND numeric properties
                const getParameterHandler = {{
                    apply: function(target, thisArg, args) {{
                        const param = args[0];

                        if (param === 0x1F00) {{ // VENDOR
                            return fakeVendor;
                        }}
                        if (param === 0x1F01) {{ // RENDERER
                            return fakeRenderer;
                        }}
                        if (param === 0x9245) {{ // UNMASKED_VENDOR_WEBGL
                            return fakeVendor;
                        }}
                        if (param === 0x9246) {{ // UNMASKED_RENDERER_WEBGL
                            return fakeRenderer;
                        }}

                        // Slightly modify the numerical values ​​to change the hash
                        const result = Reflect.apply(target, thisArg, args);
                        
                        // If it's a number, add noise
                        if (typeof result === 'number') {{
                            const noise = (Math.sin(noiseSeed * param) * 2) - 1;
                            return Math.floor(result + noise);
                        }}

                        return result;
                    }}
                }};

                const getExtensionHandler = {{
                    apply: function(target, thisArg, args) {{
                        if (args[0] === 'WEBGL_debug_renderer_info') {{
                            return null;
                        }}
                        return Reflect.apply(target, thisArg, args);
                    }}
                }};

                // Hook readPixels to modify the rendered pixels (core of the WebGL hash)
                const readPixelsHandler = {{
                    apply: function(target, thisArg, args) {{
                        const result = Reflect.apply(target, thisArg, args);
                        
                        // Slightly adjust the pixels
                        if (args[6] && args[6].length) {{
                            for (let i = 0; i < args[6].length; i += 100) {{
                                const noise = Math.floor((Math.sin(noiseSeed * i) * 3));
                                args[6][i] = Math.max(0, Math.min(255, args[6][i] + noise));
                            }}
                        }}
                        
                        return result;
                    }}
                }};

                // Hook shaderSource to modify shaders (affects rendering)
                const shaderSourceHandler = {{
                    apply: function(target, thisArg, args) {{
                        let source = args[1];
                        
                        // Add a unique comment based on the seed
                        if (typeof source === 'string') {{
                            source = '// Seed: ' + noiseSeed.toFixed(8) + '\\n' + source;
                            args[1] = source;
                        }}
                        
                        return Reflect.apply(target, thisArg, args);
                    }}
                }};

                // Apply the proxies
                WebGLRenderingContext.prototype.getParameter = new Proxy(WebGLRenderingContext.prototype.getParameter, getParameterHandler);
                WebGL2RenderingContext.prototype.getParameter = new Proxy(WebGL2RenderingContext.prototype.getParameter, getParameterHandler);
                WebGLRenderingContext.prototype.getExtension = new Proxy(WebGLRenderingContext.prototype.getExtension, getExtensionHandler);
                WebGL2RenderingContext.prototype.getExtension = new Proxy(WebGL2RenderingContext.prototype.getExtension, getExtensionHandler);
                WebGLRenderingContext.prototype.readPixels = new Proxy(WebGLRenderingContext.prototype.readPixels, readPixelsHandler);
                WebGL2RenderingContext.prototype.readPixels = new Proxy(WebGL2RenderingContext.prototype.readPixels, readPixelsHandler);
                WebGLRenderingContext.prototype.shaderSource = new Proxy(WebGLRenderingContext.prototype.shaderSource, shaderSourceHandler);
                WebGL2RenderingContext.prototype.shaderSource = new Proxy(WebGL2RenderingContext.prototype.shaderSource, shaderSourceHandler);

                // Protect getSupportedExtensions
                const originalGetSupportedExtensions = WebGLRenderingContext.prototype.getSupportedExtensions;
                WebGLRenderingContext.prototype.getSupportedExtensions = function() {{
                    const extensions = originalGetSupportedExtensions.apply(this, arguments);
                    return extensions ? extensions.filter(ext => ext !== 'WEBGL_debug_renderer_info') : [];
                }};

                // Hook getShaderPrecisionFormat to modify the precision
                const precisionHandler = {{
                    apply: function(target, thisArg, args) {{
                        const result = Reflect.apply(target, thisArg, args);
                        if (result) {{
                            const noise = Math.floor(Math.sin(noiseSeed * 1000) * 2);
                            return {{
                                rangeMin: result.rangeMin + noise,
                                rangeMax: result.rangeMax + noise,
                                precision: result.precision
                            }};
                        }}
                        return result;
                    }}
                }};

                WebGLRenderingContext.prototype.getShaderPrecisionFormat = new Proxy(
                    WebGLRenderingContext.prototype.getShaderPrecisionFormat, 
                    precisionHandler
                );
                WebGL2RenderingContext.prototype.getShaderPrecisionFormat = new Proxy(
                    WebGL2RenderingContext.prototype.getShaderPrecisionFormat, 
                    precisionHandler
                );
            }}

            spoofWebGL();
            console.log('[Fingerprint Defender] WebGL fingerprint randomized with seed:', noiseSeed.toFixed(8));
        }})();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    public async Task EnableWebGL1ProtectionAsync(string fakeRenderer, string fakeVendor)
    {
        // Generate a random seed for this profile
        double noiseSeed = random.NextDouble();

        string script = $@"
    (function() {{
        'use strict';
        console.log('[Fingerprint Defender] WebGL1 Specific Protection Enabled');

        // Seed for noise (only one per session)
        const noiseSeed = {noiseSeed.ToString(System.Globalization.CultureInfo.InvariantCulture)};

        // Fake GPU IDs for WebGL1
        //const fakeVendor = 'Google Inc. (Intel)';
        //const fakeRenderer = 'ANGLE (Intel, Intel(R) HD Graphics Direct3D11 vs_5_0 ps_5_0, D3D11)';
        const fakeVendor = '{fakeVendor}';
        const fakeRenderer = '{fakeRenderer}';

        function spoofWebGL1() {{
            // Hook getParameter for WebGL1 only
            const getParameterHandler = {{
                apply: function(target, thisArg, args) {{
                    const param = args[0];

                    // String parameters
                    if (param === 0x1F00) {{ // GL_VENDOR
                        return fakeVendor;
                    }}
                    if (param === 0x1F01) {{ // GL_RENDERER
                        return fakeRenderer;
                    }}
                    if (param === 0x1F02) {{ // GL_VERSION
                        return 'WebGL 1.0 (OpenGL ES 2.0 Chromium)';
                    }}
                    if (param === 0x8B8C) {{ // GL_SHADING_LANGUAGE_VERSION
                        return 'WebGL GLSL ES 1.0 (OpenGL ES GLSL ES 1.0 Chromium)';
                    }}
                    if (param === 0x9245) {{ // UNMASKED_VENDOR_WEBGL
                        return fakeVendor;
                    }}
                    if (param === 0x9246) {{ // UNMASKED_RENDERER_WEBGL
                        return fakeRenderer;
                    }}

                    // Numeric parameters with noise
                    const result = Reflect.apply(target, thisArg, args);
                    
                    if (typeof result === 'number') {{
                        const noise = (Math.sin(noiseSeed * param) * 2) - 1;
                        return Math.floor(result + noise);
                    }}

                    return result;
                }}
            }};

            // Hook getExtension to block debug info
            const getExtensionHandler = {{
                apply: function(target, thisArg, args) {{
                    const extName = args[0];
                    
                    // Block debug renderer info extension
                    if (extName === 'WEBGL_debug_renderer_info') {{
                        return null;
                    }}
                    
                    return Reflect.apply(target, thisArg, args);
                }}
            }};

            // Hook readPixels to modify rendered pixels
            const readPixelsHandler = {{
                apply: function(target, thisArg, args) {{
                    const result = Reflect.apply(target, thisArg, args);
                    
                    // Modify pixel data (args[6] is the pixel array)
                    if (args[6] && args[6].length) {{
                        for (let i = 0; i < args[6].length; i += 100) {{
                            const noise = Math.floor((Math.sin(noiseSeed * i) * 3));
                            args[6][i] = Math.max(0, Math.min(255, args[6][i] + noise));
                        }}
                    }}
                    
                    return result;
                }}
            }};

            // Hook shaderSource to modify shader code
            const shaderSourceHandler = {{
                apply: function(target, thisArg, args) {{
                    let source = args[1];
                    
                    if (typeof source === 'string') {{
                        // Add unique comment based on seed
                        source = '// WebGL1 Seed: ' + noiseSeed.toFixed(8) + '\\n' + source;
                        args[1] = source;
                    }}
                    
                    return Reflect.apply(target, thisArg, args);
                }}
            }};

            // Hook getShaderPrecisionFormat
            const precisionHandler = {{
                apply: function(target, thisArg, args) {{
                    const result = Reflect.apply(target, thisArg, args);
                    
                    if (result) {{
                        const noise = Math.floor(Math.sin(noiseSeed * 1000) * 2);
                        return {{
                            rangeMin: result.rangeMin + noise,
                            rangeMax: result.rangeMax + noise,
                            precision: result.precision
                        }};
                    }}
                    
                    return result;
                }}
            }};

            // Hook getContextAttributes
            const contextAttributesHandler = {{
                apply: function(target, thisArg, args) {{
                    const result = Reflect.apply(target, thisArg, args);
                    
                    if (result) {{
                        // Slightly modify context attributes
                        return {{
                            ...result,
                            antialias: Math.sin(noiseSeed) > 0,
                            premultipliedAlpha: Math.cos(noiseSeed) > 0
                        }};
                    }}
                    
                    return result;
                }}
            }};

            // Apply all proxies to WebGL1 RenderingContext
            WebGLRenderingContext.prototype.getParameter = new Proxy(
                WebGLRenderingContext.prototype.getParameter, 
                getParameterHandler
            );
            
            WebGLRenderingContext.prototype.getExtension = new Proxy(
                WebGLRenderingContext.prototype.getExtension, 
                getExtensionHandler
            );
            
            WebGLRenderingContext.prototype.readPixels = new Proxy(
                WebGLRenderingContext.prototype.readPixels, 
                readPixelsHandler
            );
            
            WebGLRenderingContext.prototype.shaderSource = new Proxy(
                WebGLRenderingContext.prototype.shaderSource, 
                shaderSourceHandler
            );
            
            WebGLRenderingContext.prototype.getShaderPrecisionFormat = new Proxy(
                WebGLRenderingContext.prototype.getShaderPrecisionFormat, 
                precisionHandler
            );
            
            WebGLRenderingContext.prototype.getContextAttributes = new Proxy(
                WebGLRenderingContext.prototype.getContextAttributes, 
                contextAttributesHandler
            );

            // Protect getSupportedExtensions for WebGL1
            const originalGetSupportedExtensions = WebGLRenderingContext.prototype.getSupportedExtensions;
            WebGLRenderingContext.prototype.getSupportedExtensions = function() {{
                const extensions = originalGetSupportedExtensions.apply(this, arguments);
                return extensions ? extensions.filter(ext => ext !== 'WEBGL_debug_renderer_info') : [];
            }};

            // Hook getAttachedShaders to add noise
            const attachedShadersHandler = {{
                apply: function(target, thisArg, args) {{
                    const result = Reflect.apply(target, thisArg, args);
                    
                    // Shuffle array order based on seed
                    if (result && Array.isArray(result) && result.length > 1) {{
                        if (Math.sin(noiseSeed) > 0) {{
                            return result.reverse();
                        }}
                    }}
                    
                    return result;
                }}
            }};
            
            WebGLRenderingContext.prototype.getAttachedShaders = new Proxy(
                WebGLRenderingContext.prototype.getAttachedShaders, 
                attachedShadersHandler
            );
        }}

        spoofWebGL1();
        console.log('[Fingerprint Defender] WebGL1 fingerprint randomized with seed:', noiseSeed.toFixed(8));
    }})();
    ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    public async Task EnableCanvasProtectionAsync()
    {
        string script = @"
        (function() {
            'use strict';
            console.log('[Fingerprint Defender] Canvas Protection activated');

            // Use a consistent seed per session to avoid excessive variations
            let noiseSeed = Math.random();

            // Deterministic noise function based on position
            function getNoiseValue(index) {
                const x = Math.sin(noiseSeed * index) * 10000;
                return x - Math.floor(x);
            }

            const getContext = HTMLCanvasElement.prototype.getContext;

            HTMLCanvasElement.prototype.getContext = function(type, attrs) {
                const ctx = getContext.apply(this, arguments);
                
                if (type === '2d') {
                    const originalToDataURL = this.toDataURL;
                    const originalGetImageData = ctx.getImageData;

                    // Function to add subtle noise
                    function addNoise(imageData) {
                        const data = imageData.data;
                        const factor = 1; // Very slight noise
                        for (let i = 0; i < data.length; i += 4) {
                            const noise = getNoiseValue(i) * factor * 2 - factor;
                            data[i] = Math.max(0, Math.min(255, data[i] + noise));     // R
                            data[i + 1] = Math.max(0, Math.min(255, data[i + 1] + noise)); // G
                            data[i + 2] = Math.max(0, Math.min(255, data[i + 2] + noise)); // B
                        }
                    }

                    // Override toDataURL
                    this.toDataURL = function() {
                        const imageData = ctx.getImageData(0, 0, this.width, this.height);
                        addNoise(imageData);
                        ctx.putImageData(imageData, 0, 0);
                        return originalToDataURL.apply(this, arguments);
                    };

                    // Override getImageData
                    ctx.getImageData = function() {
                        const imageData = originalGetImageData.apply(this, arguments);
                        addNoise(imageData);
                        return imageData;
                    };
                }

                return ctx;
            };

            console.log('[Fingerprint Defender] Canvas fingerprint modified');
        })();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    public async Task EnableAudioContextProtectionAsync()
    {
        string script = @"
        (function() {
            'use strict';
            console.log('[Fingerprint Defender] AudioContext Protection Enabled');

            const AudioContext = window.AudioContext || window.webkitAudioContext;
            if (!AudioContext) return;

            // Seed for audio noise
            let audioSeed = Math.random() * 1000;

            const originalGetChannelData = AudioBuffer.prototype.getChannelData;
            AudioBuffer.prototype.getChannelData = function() {
                const data = originalGetChannelData.apply(this, arguments);
                for (let i = 0; i < data.length; i += 100) { 
                    data[i] += (Math.sin(audioSeed + i) * 0.00001);
                }
                return data;
            };

            const originalCreateAnalyser = AudioContext.prototype.createAnalyser;
            AudioContext.prototype.createAnalyser = function() {
                const analyser = originalCreateAnalyser.apply(this, arguments);
                const originalGetFloatFrequencyData = analyser.getFloatFrequencyData;

                analyser.getFloatFrequencyData = function(array) {
                    originalGetFloatFrequencyData.apply(this, [array]);
                    for (let i = 0; i < array.length; i++) {
                        array[i] += (Math.sin(audioSeed + i) * 0.05);
                    }
                };

                return analyser;
            };

            console.log('[Fingerprint Defender] Audio fingerprint modified');
        })();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    public async Task EnableWebRTCAndBatteryAndSpeechProtectionAsync()
    {
        string script = @"
        (function() {
            'use strict';
            console.log('[Fingerprint Defender] WebRTC, Battery API and Speech Synthesis protection enabled');

            // WebRTC - Block IP leaks
            const originalRTCPeerConnection = window.RTCPeerConnection || window.webkitRTCPeerConnection || window.mozRTCPeerConnection;
            if (originalRTCPeerConnection) {
                window.RTCPeerConnection = function(config) {
                    if (config && config.iceServers) {
                        config.iceServers = [];
                    }
                    return new originalRTCPeerConnection(config);
                };
                window.RTCPeerConnection.prototype = originalRTCPeerConnection.prototype;
            }

            // MediaDevices - limit the enumeration
            if (navigator.mediaDevices && navigator.mediaDevices.enumerateDevices) {
                const originalEnumerateDevices = navigator.mediaDevices.enumerateDevices;
                navigator.mediaDevices.enumerateDevices = function() {
                    return originalEnumerateDevices.apply(this, arguments).then(devices => {
                        return devices.map(device => ({
                            deviceId: 'default',
                            kind: device.kind,
                            label: '',
                            groupId: 'default'
                        }));
                    });
                };
            }

            // Battery API Protection
            if (navigator.getBattery) {
                navigator.getBattery = function() {
                    return Promise.resolve({
                        charging: true,
                        chargingTime: 0,
                        dischargingTime: Infinity,
                        level: 1,
                        addEventListener: function() {},
                        removeEventListener: function() {},
                        dispatchEvent: function() { return true; }
                    });
                };
            }

            // Speech Synthesis Protection
            if (window.speechSynthesis) {
                const originalGetVoices = window.speechSynthesis.getVoices;
                window.speechSynthesis.getVoices = function() {
                    return [
                        { name: 'Google US English', lang: 'en-US', localService: false, voiceURI: 'Google US English', default: true }
                    ];
                };
            }

            console.log('[Fingerprint Defender] WebRTC/Battery/Speech modified');
        })();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    public async Task EnableAdvancedFingerprintProtectionAsync()
    {
        string script = @"
        (function() {
            'use strict';
            console.log('[Fingerprint Defender] Advanced Fingerprint Protection Enabled');

            // Protect CSS media query properties
            const originalMatchMedia = window.matchMedia;
            window.matchMedia = function(query) {
                const result = originalMatchMedia.apply(this, arguments);
                // Certains fingerprinters utilisent les media queries
                if (query.includes('prefers-color-scheme')) {
                    return { matches: false, media: query };
                }
                return result;
            };

            // Protect Keyboard Layout
            const protectKeyboardLayoutAdvanced = () => {
                if (!navigator.keyboard) return;
                
                try {
                    const keyboardHandler = {
                        get(target, prop) {
                            if (prop === 'getLayoutMap') {
                                return () => Promise.resolve(new Map([
                                    ['key1', 'layout1'],
                                    ['key2', 'layout2']
                                ]));
                            }
                            
                            const value = target[prop];
                            return typeof value === 'function' ? value.bind(target) : value;
                        }
                    };
                    
                    navigator.keyboard = new Proxy(navigator.keyboard, keyboardHandler);
                    
                } catch (error) {
                    // Fallback simple
                    try {
                        navigator.keyboard.getLayoutMap = () => Promise.resolve(new Map());
                    } catch (e) {
                        console.warn('Keyboard layout protection failed');
                    }
                }
            };

            // Protecting performance properties
            const protectPerformanceProperties = () => {
                if (!window.performance) return;
                
                try {
                    const performanceHandler = {
                        get(target, prop) {
                            // Intercept access to 'memory'
                            if (prop === 'memory') {
                                return {
                                    jsHeapSizeLimit: 2172649472,
                                    totalJSHeapSize: 10000000,
                                    usedJSHeapSize: 10000000
                                };
                            }
                            
                            // For other properties, return the original value
                            const value = target[prop];
                            // If it's a function, bind the context
                            return typeof value === 'function' ? value.bind(target) : value;
                        }
                    };
                    
                    // Replace performance with the proxy
                    window.performance = new Proxy(window.performance, performanceHandler);
                    
                } catch (error) {
                    console.warn('Performance properties protection not available');
                }
            };
            
            protectPerformanceProperties();

            // Hide automation-specific properties
            delete Object.getPrototypeOf(navigator).webdriver;

            console.log('[Fingerprint Defender] Advanced protections applied');
        })();
        ";

        await webView.AddScriptToExecuteOnDocumentCreatedAsync(script);
    }

    string GenerateRandomUserAgent()
    {
        string[] userAgents =
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.1 Safari/605.1.15",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0"
        };

        return userAgents[random.Next(userAgents.Length)];
    }

    (int, int) GenerateRandomResolution()
    {
        (int, int)[] resolutions =
        {
            (1920, 1080), (1366, 768), (1440, 900), (1536, 864), (1280, 720),
            (2560, 1440), (1600, 900), (1280, 1024), (1680, 1050)
        };

        return resolutions[random.Next(resolutions.Length)];
    }

    string GenerateRandomPlatform()
    {
        string[] platforms =
        {
            "Win32", "MacIntel", "Linux x86_64"
        };

        return platforms[random.Next(platforms.Length)];
    }

    int GenerateRandomTimezoneOffset()
    {
        int[] offsets = { -480, -420, -360, -300, -240, -180, 0, 60, 120, 180, 330, 480, 540 };
        return offsets[random.Next(offsets.Length)];
    }

    string GenerateRandomLanguage()
    {
        string[] languages = { "en-US", "en-GB", "fr-FR", "de-DE", "es-ES", "ja-JP", "pt-BR", "it-IT" };
        return languages[random.Next(languages.Length)];
    }

    int GenerateRandomHardwareConcurrency()
    {
        int[] cores = { 2, 4, 6, 8, 12, 16, 24 };
        return cores[random.Next(cores.Length)];
    }

    float GenerateRandomDeviceMemory()
    {
        float[] memories = { 4, 8, 16 }; // GB
        return memories[random.Next(memories.Length)];
    }

    int GenerateRandomMaxTouchPoints()
    {
        // Desktop = 0, Mobile = 5 or 10
        return 0; // desktop only
    }

    string GenerateRandomConnectionType()
    {
        string[] types = { "wifi", "4g", "3g", "ethernet" };
        return types[random.Next(types.Length)];
    }

    static readonly string[] Renderers = new string[]
    {
        // Intel Graphics
        "ANGLE (Intel, Intel(R) HD Graphics Direct3D11 vs_5_0 ps_5_0, D3D11)",
        "ANGLE (Intel, Intel(R) UHD Graphics 620 Direct3D11 vs_5_0 ps_5_0, D3D11)",
        "ANGLE (Intel, Intel(R) UHD Graphics 630 Direct3D11 vs_5_0 ps_5_0, D3D11)",
        "ANGLE (Intel, Intel(R) Iris(R) Xe Graphics Direct3D11 vs_5_0 ps_5_0, D3D11)",
        "ANGLE (Intel, Intel(R) HD Graphics 4000 Direct3D11 vs_5_0 ps_5_0, D3D11)",
        
        // NVIDIA Graphics
        "ANGLE (NVIDIA, NVIDIA GeForce GTX 1050 Ti Direct3D11 vs_5_0 ps_5_0, D3D11-27.21.14.5671)",
        "ANGLE (NVIDIA, NVIDIA GeForce GTX 1060 Direct3D11 vs_5_0 ps_5_0, D3D11-27.21.14.5671)",
        "ANGLE (NVIDIA, NVIDIA GeForce GTX 1650 Direct3D11 vs_5_0 ps_5_0, D3D11-30.0.14.9649)",
        "ANGLE (NVIDIA, NVIDIA GeForce RTX 2060 Direct3D11 vs_5_0 ps_5_0, D3D11-31.0.15.1659)",
        "ANGLE (NVIDIA, NVIDIA GeForce RTX 3060 Direct3D11 vs_5_0 ps_5_0, D3D11-31.0.15.4601)",
        
        // AMD Graphics
        "ANGLE (AMD, AMD Radeon(TM) Graphics Direct3D11 vs_5_0 ps_5_0, D3D11)",
        "ANGLE (AMD, Radeon RX 580 Series Direct3D11 vs_5_0 ps_5_0, D3D11-30.0.13031.5003)",
        "ANGLE (AMD, AMD Radeon RX 5600 XT Direct3D11 vs_5_0 ps_5_0, D3D11-30.0.15002.92)",
        "ANGLE (AMD, AMD Radeon RX 6600 Direct3D11 vs_5_0 ps_5_0, D3D11-31.0.14051.5006)",
        "ANGLE (AMD, AMD Radeon(TM) Vega 8 Graphics Direct3D11 vs_5_0 ps_5_0, D3D11)"
    };

    public string GenerateRandomRenderer()
    {
        return Renderers[random.Next(Renderers.Length)];
    }

    public string GenerateVendorForRenderer(string renderer)
    {
        if (renderer.Contains("Intel"))
            return "Google Inc. (Intel)";
        else if (renderer.Contains("NVIDIA"))
            return "Google Inc. (NVIDIA)";
        else if (renderer.Contains("AMD"))
            return "Google Inc. (AMD)";

        return "Google Inc. (Intel)";
    }
}