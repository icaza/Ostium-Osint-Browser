/**
 * @file Extract_Content_Simple.js
 * @author ICAZA <github.wildly512@passinbox.com>
 * @version 2.3
 * @description Extract web page content and return as plain text
 */

(function () {
    'use strict';

    const DEFAULT_CONFIG = Object.freeze({
        minParagraphLength: 20,
        minTotalLength: 50,
        maxDOMDepth: 500,
        maxElementsProcessed: 500,
        maxTotalChars: 200_000,
        maxJSONLDSize: 100 * 1024,
        maxLDItemsParsed: 10,
        verbose: false
    });

    const _window = window;
    const _document = document;
    const safeJSON = _window.JSON;
    const safeJSONParse = safeJSON.parse.bind(safeJSON);
    const safeArrayFrom = Array.from.bind(Array);
    const safeSet = Set;
    const safeGetComputedStyle = _window.getComputedStyle.bind(_window);
    const safeConsole = _window.console || { log: () => { }, warn: () => { }, error: () => { } };
    const safeNodeFilter = typeof NodeFilter !== 'undefined' ? NodeFilter : null;

    function mergeConfig(cfg) {
        return Object.assign({}, DEFAULT_CONFIG, cfg || {});
    }

    let currentConfig = mergeConfig();

    function safeLog(msg, type = 'log') {
        if (!currentConfig.verbose) return;
        if (type === 'error') safeConsole.error('[TextExtractor]', msg);
        else if (type === 'warn') safeConsole.warn('[TextExtractor]', msg);
        else safeConsole.log('[TextExtractor]', msg);
    }

    function truncateIfNeeded(text) {
        if (!text) return text;
        if (text.length <= currentConfig.maxTotalChars) return text;
        safeLog(`Truncating extracted text from ${text.length} -> ${currentConfig.maxTotalChars}`, 'warn');
        return text.slice(0, currentConfig.maxTotalChars);
    }

    function cleanExtractedText(text) {
        if (!text) return '';
        return truncateIfNeeded(
            text
                .replace(/\r\n/g, '\n')
                .replace(/\n{3,}/g, '\n\n')
                .replace(/[ \t]{2,}/g, ' ')
                .replace(/^\s+|\s+$/gm, '')
                .trim()
        );
    }

    function getElementText(element) {
        if (!element) return '';
        let text = element.textContent || '';
        if ((!text || text.trim().length === 0) && typeof element.innerText === 'string') {
            try {
                text = element.innerText;
            } catch (e) {
                text = '';
            }
        }
        return (text || '').trim();
    }

    const visibilityCache = new WeakMap();
    const VIS_CACHE_TTL_MS = 2000;

    function isElementVisible(element) {
        if (!element || element.nodeType !== 1) return false;

        const cached = visibilityCache.get(element);
        const now = Date.now();
        if (cached && (now - cached.t) < VIS_CACHE_TTL_MS) {
            return cached.v;
        }

        try {
            if (element.hasAttribute('aria-hidden') && element.getAttribute('aria-hidden') === 'true') {
                visibilityCache.set(element, { v: false, t: now });
                return false;
            }

            const style = safeGetComputedStyle(element);
            if (!style) {
                visibilityCache.set(element, { v: false, t: now });
                return false;
            }

            if (style.display === 'none' || style.visibility === 'hidden' || parseFloat(style.opacity || '1') === 0) {
                visibilityCache.set(element, { v: false, t: now });
                return false;
            }

            const rect = element.getBoundingClientRect();
            if (rect.width === 0 || rect.height === 0) {
                visibilityCache.set(element, { v: false, t: now });
                return false;
            }

            const inViewport = rect.bottom >= 0 && rect.right >= 0 && rect.top <= (_window.innerHeight || _document.documentElement.clientHeight) && rect.left <= (_window.innerWidth || _document.documentElement.clientWidth);
            if (!inViewport) {
                visibilityCache.set(element, { v: false, t: now });
                return false;
            }

            visibilityCache.set(element, { v: true, t: now });
            return true;
        } catch (e) {
            visibilityCache.set(element, { v: false, t: now });
            return false;
        }
    }

    function extractFromSchemaOrg() {
        const scripts = _document.querySelectorAll('script[type="application/ld+json"]');
        let parsedCount = 0;

        for (const script of scripts) {
            try {
                const content = script.textContent || '';
                if (!content) continue;
                if (content.length > currentConfig.maxJSONLDSize) {
                    safeLog('Skipping large LD+JSON script block', 'warn');
                    continue;
                }
                if (parsedCount >= currentConfig.maxLDItemsParsed) {
                    safeLog('LD+JSON parse limit reached', 'warn');
                    break;
                }

                let data;
                try {
                    data = safeJSONParse(content);
                } catch (e) {
                    continue;
                }

                const found = findArticleInSchema(data, 0);
                if (found && typeof found.articleBody === 'string' && found.articleBody.trim().length > 0) {
                    safeLog('Found articleBody via JSON-LD');
                    return found.articleBody;
                }
                parsedCount++;
            } catch (e) {
                safeLog('JSON-LD processing error: ' + (e && e.message), 'warn');
            }
        }
        return '';
    }

    function findArticleInSchema(node, depth) {
        if (depth > 8) return null;
        if (!node) return null;
        if (Array.isArray(node)) {
            for (const item of node) {
                const r = findArticleInSchema(item, depth + 1);
                if (r) return r;
            }
        } else if (typeof node === 'object') {
            const type = node['@type'] || node['type'];
            if (type === 'Article' || type === 'NewsArticle' || type === 'ScholarlyArticle') {
                return node;
            }
            for (const k in node) {
                try {
                    const r = findArticleInSchema(node[k], depth + 1);
                    if (r) return r;
                } catch (e) {
                    continue;
                }
            }
        }
        return null;
    }

    function extractByReadability() {
        const candidates = [];
        const containers = _document.querySelectorAll('div, section, article, main');
        let processed = 0;

        for (const container of containers) {
            if (processed >= currentConfig.maxElementsProcessed) break;
            processed++;

            if (!isElementVisible(container)) continue;

            const text = getElementText(container);
            if (!text || text.length < currentConfig.minTotalLength) continue;

            const links = container.getElementsByTagName('a');
            let linkLength = 0;
            for (const a of safeArrayFrom(links)) {
                linkLength += (a.textContent || '').length;
            }

            const paragraphs = container.getElementsByTagName('p');
            const pCount = paragraphs.length || 1;
            const textLength = text.length;
            const linkDensity = textLength > 0 ? linkLength / textLength : 1;

            const score = textLength * (1 - Math.min(0.99, linkDensity)) * Math.log(pCount + 1);

            if (score > 100) candidates.push({ element: container, score, text });
        }

        candidates.sort((a, b) => b.score - a.score);

        if (candidates.length > 0) {
            safeLog(`Readability: best score = ${candidates[0].score.toFixed(1)}`);
            return candidates[0].text;
        }
        return '';
    }

    function extractFromMainContent() {
        const selectors = [
            'main', 'article', '[role="main"]',
            '.content', '#content', '.main-content',
            '.post-content', '.article-content', '.entry-content',
            '[itemprop="articleBody"]'
        ];

        for (const selector of selectors) {
            try {
                const element = _document.querySelector(selector);
                if (element && isElementVisible(element)) {
                    const text = getElementText(element);
                    if (text && text.length > currentConfig.minTotalLength * 2) {
                        safeLog(`Found main content using selector ${selector}`);
                        return text;
                    }
                }
            } catch (e) {
                continue;
            }
        }
        return '';
    }

    function extractFromArticleElements() {
        const articles = _document.querySelectorAll('article');
        const texts = [];
        for (const article of articles) {
            if (!isElementVisible(article)) continue;
            const text = getElementText(article);
            if (text && text.length > currentConfig.minParagraphLength) {
                texts.push(text);
            }
            if (texts.join('\n\n').length > currentConfig.maxTotalChars) break;
        }
        return texts.join('\n\n');
    }

    function extractFromBodyWithFilter() {
        try {
            const unwantedSelector = 'script,style,noscript,iframe,nav,header,footer,aside,[role="navigation"],.ad,.advertisement,svg,canvas';
            const textParts = [];
            const walker = _document.createTreeWalker(
                _document.body,
                NodeFilter.SHOW_TEXT,
                {
                    acceptNode: (node) => {
                        if (!node || !node.parentElement) return safeNodeFilter ? safeNodeFilter.FILTER_REJECT : 0;
                        try {
                            if (node.parentElement.closest && node.parentElement.closest(unwantedSelector)) {
                                return safeNodeFilter ? safeNodeFilter.FILTER_REJECT : 0;
                            }
                            if (!isElementVisible(node.parentElement)) {
                                return safeNodeFilter ? safeNodeFilter.FILTER_REJECT : 0;
                            }
                            const t = (node.textContent || '').trim();
                            if (!t || t.length < 10) return safeNodeFilter ? safeNodeFilter.FILTER_REJECT : 0;
                            return safeNodeFilter ? safeNodeFilter.FILTER_ACCEPT : 1;
                        } catch (e) {
                            return safeNodeFilter ? safeNodeFilter.FILTER_REJECT : 0;
                        }
                    }
                }
            );

            let count = 0;
            let node;
            while ((node = walker.nextNode()) && count < currentConfig.maxDOMDepth) {
                const txt = (walker.currentNode && walker.currentNode.textContent) ? walker.currentNode.textContent.trim() : '';
                if (txt && txt.length > 10) {
                    textParts.push(txt);
                }
                count++;
                if (textParts.join(' ').length > currentConfig.maxTotalChars) break;
            }
            return textParts.join(' ');
        } catch (e) {
            safeLog('Body filter extraction failed: ' + e.message, 'warn');
            return '';
        }
    }

    function extractFromAllParagraphs() {
        const selectors = 'p, h1, h2, h3, h4, h5, h6, li, blockquote';
        const elements = _document.querySelectorAll(selectors);
        const texts = new safeSet();
        let processed = 0;
        for (const el of elements) {
            if (processed >= currentConfig.maxElementsProcessed) break;
            processed++;
            if (!isElementVisible(el)) continue;
            const text = getElementText(el);
            if (text.length > currentConfig.minParagraphLength) {
                texts.add(text);
            }
            if (Array.from(texts).join('\n\n').length > currentConfig.maxTotalChars) break;
        }
        return Array.from(texts).join('\n\n');
    }

    function extractFullBodyText() {
        try {
            return getElementText(_document.body) || '';
        } catch (e) {
            return '';
        }
    }

    function calculateQualityScore(text) {
        if (!text) return 0;
        const length = text.length;
        const lines = text.split('\n').filter(l => l.trim().length > 20).length;
        const words = text.split(/\s+/).filter(Boolean).length;
        const avgWordLength = length / Math.max(words, 1);

        let penalty = 0;
        const uniqueLines = new safeSet(text.split('\n').map(l => l.trim()));
        const repetitionRatio = uniqueLines.size / Math.max(lines, 1);
        if (repetitionRatio < 0.5) penalty += 2000;
        if (length < 500) penalty += 1000;
        if (avgWordLength < 4) penalty += 1000;

        const base = Math.max(0, length + (lines * 50) + (words * 2) - penalty);
        return base;
    }

    function extractContentSync() {
        try {
            const strategies = [
                { name: 'Schema.org Article', fn: extractFromSchemaOrg, weight: 100 },
                { name: 'Main Content', fn: extractFromMainContent, weight: 90 },
                { name: 'Article Element', fn: extractFromArticleElements, weight: 80 },
                { name: 'Readability Heuristic', fn: extractByReadability, weight: 70 },
                { name: 'Body Filtered', fn: extractFromBodyWithFilter, weight: 60 },
                { name: 'All Paragraphs', fn: extractFromAllParagraphs, weight: 50 },
                { name: 'Full Body', fn: extractFullBodyText, weight: 10 }
            ];

            let bestResult = { text: '', score: 0, method: '' };

            for (const strategy of strategies) {
                try {
                    safeLog(`Attempting strategy: ${strategy.name}`);
                    const text = strategy.fn();
                    if (!text || text.length === 0) continue;

                    const effectiveText = text.length > currentConfig.maxTotalChars ? text.slice(0, currentConfig.maxTotalChars) : text;
                    const score = calculateQualityScore(effectiveText) * strategy.weight;

                    safeLog(`${strategy.name}: ${effectiveText.length} chars, score: ${score.toFixed(2)}`);

                    if (score > bestResult.score) {
                        bestResult = { text: effectiveText, score, method: strategy.name };
                    }

                    if (score > 8000) {
                        safeLog(`Excellent score with ${strategy.name}`, 'log');
                        break;
                    }
                } catch (e) {
                    safeLog(`${strategy.name} failed: ${e && e.message}`, 'warn');
                }
            }

            const cleaned = cleanExtractedText(bestResult.text);
            safeLog(`Extraction done: ${cleaned.length} characters via ${bestResult.method}`);

            return cleaned;
        } catch (e) {
            safeLog('Extraction error: ' + e.message, 'error');
            return '';
        }
    }

    function createSafeTextExtractor(options = {}) {
        options = options || {};
        const conf = mergeConfig(Object.assign({}, currentConfig, options.config || {}));
        if (typeof options.verbose === 'boolean') conf.verbose = options.verbose;

        async function extract() {
            // yield once to reduce reentrancy impact
            await new Promise((res) => setTimeout(res, 0));
            // use the same bounded synchronous extraction
            return extractContentSync();
        }

        return {
            extract,
            config: conf
        };
    }

    // --- FIX: return the extracted string synchronously for callers that expect a string ---
    // Use the bounded synchronous extractor and return the cleaned string.
    try {
        const finalText = cleanExtractedText(extractContentSync());
        // optionally log a short summary when verbose
        if (currentConfig.verbose) {
            safeConsole.log('%c[TextExtractor] Extraction finished - returning string result', 'color: #4CAF50; font-weight: bold');
        }
        return finalText;
    } catch (e) {
        safeLog('Final extraction failed: ' + e.message, 'error');
        return '';
    }

})();