(function () {
    'use strict';

    // Configuration optimisée pour OSINT
    const CONFIG = {
        minParagraphLength: 20,
        minTotalLength: 50,
        preserveFormatting: true,
        autoExecute: true,

        // Performance
        maxDOMDepth: 500,  // Augmenté pour pages complexes
        maxRetries: 3,     // Retry en cas d'échec
        extractionTimeout: 30000, // 30 secondes max

        // Debug
        verbose: false  // Activer pour debug détaillé
    };

    // Cache pour optimiser les appels répétés
    const cache = {
        visibility: new WeakMap(),
        computedStyles: new WeakMap()
    };

    // Métriques de performance
    const metrics = {
        startTime: 0,
        endTime: 0,
        methodUsed: '',
        charactersExtracted: 0,
        domNodesProcessed: 0
    };

    // Fonction principale optimisée
    async function extractAndCopyToClipboard() {
        metrics.startTime = performance.now();
        let attemptCount = 0;

        while (attemptCount < CONFIG.maxRetries) {
            try {
                attemptCount++;
                log(`Tentative ${attemptCount}/${CONFIG.maxRetries}`);

                // Timeout pour éviter les blocages
                const extractedText = await Promise.race([
                    extractReadableContentAsync(),
                    new Promise((_, reject) =>
                        setTimeout(() => reject(new Error('Timeout')), CONFIG.extractionTimeout)
                    )
                ]);

                if (!extractedText || extractedText.trim().length < CONFIG.minTotalLength) {
                    throw new Error('Contenu insuffisant');
                }

                log(`Texte extrait: ${extractedText.length} caractères`);

                // Copier dans le presse-papiers
                const copySuccess = await copyToClipboard(extractedText);

                if (!copySuccess) {
                    throw new Error('Échec copie presse-papiers');
                }

                metrics.endTime = performance.now();
                metrics.charactersExtracted = extractedText.length;

                // Notifier avec métriques complètes
                notifyApplication({
                    success: true,
                    length: extractedText.length,
                    textPreview: extractedText.substring(0, 500),
                    url: window.location.href,
                    title: document.title,
                    domain: window.location.hostname,
                    method: metrics.methodUsed,
                    performance: {
                        duration: Math.round(metrics.endTime - metrics.startTime),
                        nodesProcessed: metrics.domNodesProcessed,
                        attempt: attemptCount
                    },
                    timestamp: new Date().toISOString()
                });

                log('✓ Extraction réussie', 'success');
                return true;

            } catch (error) {
                logError(`Tentative ${attemptCount} échouée: ${error.message}`);

                if (attemptCount >= CONFIG.maxRetries) {
                    // Échec définitif
                    notifyApplication({
                        success: false,
                        error: error.message,
                        url: window.location.href,
                        title: document.title,
                        attempts: attemptCount,
                        timestamp: new Date().toISOString()
                    });
                    return false;
                }

                // Attendre avant retry
                await sleep(1000 * attemptCount);
            }
        }
    }

    // Extraction asynchrone avec yield pour ne pas bloquer
    async function extractReadableContentAsync() {
        const strategies = [
            { name: 'Schema.org Article', fn: extractFromSchemaOrg, weight: 100 },
            { name: 'Main Content', fn: extractFromMainContent, weight: 90 },
            { name: 'Article Element', fn: extractFromArticle, weight: 80 },
            { name: 'Readability Heuristic', fn: extractByReadability, weight: 70 },
            { name: 'Body Filtered', fn: extractFromBodyWithFilter, weight: 60 },
            { name: 'All Paragraphs', fn: extractFromAllParagraphs, weight: 50 },
            { name: 'Full Body', fn: extractFullBodyText, weight: 10 }
        ];

        let bestResult = { text: '', score: 0, method: '' };

        for (const strategy of strategies) {
            try {
                // Yield pour ne pas bloquer
                await sleep(0);

                log(`Essai: ${strategy.name}`);
                const text = strategy.fn();

                if (text && text.length > 0) {
                    // Calculer un score de qualité
                    const score = calculateQualityScore(text) * strategy.weight;

                    log(`${strategy.name}: ${text.length} chars, score: ${score.toFixed(2)}`);

                    if (score > bestResult.score) {
                        bestResult = { text, score, method: strategy.name };
                    }

                    // Si score excellent, early return
                    if (score > 8000) {
                        log(`Score excellent atteint avec ${strategy.name}`);
                        break;
                    }
                }
            } catch (e) {
                logError(`${strategy.name} échoué: ${e.message}`);
            }
        }

        metrics.methodUsed = bestResult.method;
        return cleanExtractedText(bestResult.text);
    }

    // NOUVELLE: Extraction via Schema.org (très fiable pour articles)
    function extractFromSchemaOrg() {
        const scripts = document.querySelectorAll('script[type="application/ld+json"]');

        for (const script of scripts) {
            try {
                const data = JSON.parse(script.textContent);
                const article = findArticleInSchema(data);

                if (article && article.articleBody) {
                    log('Schema.org trouvé !');
                    return article.articleBody;
                }
            } catch (e) {
                // JSON invalide, ignorer
            }
        }
        return '';
    }

    function findArticleInSchema(data) {
        if (Array.isArray(data)) {
            for (const item of data) {
                const found = findArticleInSchema(item);
                if (found) return found;
            }
        } else if (data && typeof data === 'object') {
            if (data['@type'] === 'Article' || data['@type'] === 'NewsArticle') {
                return data;
            }
            for (const key in data) {
                const found = findArticleInSchema(data[key]);
                if (found) return found;
            }
        }
        return null;
    }

    // OPTIMISÉE: Heuristique de densité de texte (comme Readability)
    function extractByReadability() {
        const candidates = [];

        // Chercher les divs/sections avec beaucoup de texte
        const containers = document.querySelectorAll('div, section, article, main');

        for (const container of containers) {
            if (!isElementVisibleCached(container)) continue;

            const text = container.innerText || '';
            const links = container.querySelectorAll('a');
            const paragraphs = container.querySelectorAll('p');

            // Calculer densité de texte
            const textLength = text.length;
            const linkLength = Array.from(links).reduce((sum, a) => sum + (a.innerText?.length || 0), 0);
            const linkDensity = textLength > 0 ? linkLength / textLength : 1;

            // Score basé sur:
            // - Longueur de texte (plus = mieux)
            // - Faible densité de liens (moins = mieux)
            // - Nombre de paragraphes (plus = mieux)
            const score = textLength * (1 - linkDensity) * Math.log(paragraphs.length + 1);

            if (score > 100) {
                candidates.push({ element: container, score, text });
            }
        }

        // Trier par score
        candidates.sort((a, b) => b.score - a.score);

        if (candidates.length > 0) {
            log(`Readability: meilleur score = ${candidates[0].score.toFixed(2)}`);
            return candidates[0].text;
        }

        return '';
    }

    // Calcul de score de qualité du texte extrait
    function calculateQualityScore(text) {
        if (!text) return 0;

        const length = text.length;
        const lines = text.split('\n').filter(l => l.trim().length > 20).length;
        const words = text.split(/\s+/).length;
        const avgWordLength = length / Math.max(words, 1);

        // Pénalités
        let penalty = 0;

        // Trop de répétitions (menus, footers)
        const uniqueLines = new Set(text.split('\n').map(l => l.trim()));
        const repetitionRatio = uniqueLines.size / Math.max(lines, 1);
        if (repetitionRatio < 0.5) penalty += 2000;

        // Trop court
        if (length < 500) penalty += 1000;

        // Mots trop courts (probablement des menus)
        if (avgWordLength < 4) penalty += 1000;

        // Score final
        return Math.max(0, length + (lines * 50) + (words * 2) - penalty);
    }

    // Méthodes d'extraction existantes - OPTIMISÉES
    function extractFromMainContent() {
        const selectors = [
            'main', 'article', '[role="main"]',
            '.content', '#content', '.main-content',
            '.post-content', '.article-content', '.entry-content',
            '[itemprop="articleBody"]'
        ];

        for (const selector of selectors) {
            const element = document.querySelector(selector);
            if (element && isElementVisibleCached(element)) {
                const text = getTextFromElement(element);
                if (text && text.length > CONFIG.minTotalLength * 2) {
                    return text;
                }
            }
        }
        return '';
    }

    function extractFromArticle() {
        const articles = document.querySelectorAll('article');
        const texts = [];

        for (const article of articles) {
            if (isElementVisibleCached(article)) {
                const text = getTextFromElement(article);
                if (text && text.length > CONFIG.minParagraphLength) {
                    texts.push(text);
                }
            }
        }

        return texts.join('\n\n');
    }

    function extractFromBodyWithFilter() {
        const unwanted = 'script,style,noscript,iframe,nav,header,footer,aside,.ad,.advertisement,svg,canvas';
        const textParts = [];

        const walker = document.createTreeWalker(
            document.body,
            NodeFilter.SHOW_TEXT,
            {
                acceptNode: (node) => {
                    if (node.parentElement?.closest(unwanted)) {
                        return NodeFilter.FILTER_REJECT;
                    }
                    if (!isElementVisibleCached(node.parentElement)) {
                        return NodeFilter.FILTER_REJECT;
                    }
                    return NodeFilter.FILTER_ACCEPT;
                }
            }
        );

        let count = 0;
        while (walker.nextNode() && count < CONFIG.maxDOMDepth) {
            const text = walker.currentNode.textContent?.trim();
            if (text && text.length > 10) {
                textParts.push(text);
            }
            count++;
        }

        metrics.domNodesProcessed = count;
        return textParts.join(' ');
    }

    function extractFromAllParagraphs() {
        const selectors = 'p, h1, h2, h3, h4, h5, h6, li, blockquote';
        const elements = document.querySelectorAll(selectors);
        const texts = new Set(); // Éviter doublons

        for (const el of elements) {
            if (isElementVisibleCached(el)) {
                const text = (el.innerText || el.textContent || '').trim();
                if (text.length > CONFIG.minParagraphLength) {
                    texts.add(text);
                }
            }
        }

        return Array.from(texts).join('\n\n');
    }

    function extractFullBodyText() {
        return document.body.innerText || document.body.textContent || '';
    }

    // OPTIMISÉE: Cache de visibilité avec WeakMap
    function isElementVisibleCached(element) {
        if (!element) return false;

        if (cache.visibility.has(element)) {
            return cache.visibility.get(element);
        }

        const style = getComputedStyleCached(element);
        const isVisible = style.display !== 'none' &&
            style.visibility !== 'hidden' &&
            parseFloat(style.opacity) > 0;

        cache.visibility.set(element, isVisible);
        return isVisible;
    }

    function getComputedStyleCached(element) {
        if (cache.computedStyles.has(element)) {
            return cache.computedStyles.get(element);
        }

        const style = window.getComputedStyle(element);
        cache.computedStyles.set(element, style);
        return style;
    }

    function getTextFromElement(element) {
        if (!element) return '';
        return (element.innerText || element.textContent || '').trim();
    }

    // Nettoyage optimisé
    function cleanExtractedText(text) {
        if (!text) return '';

        return text
            .replace(/\r\n/g, '\n')           // Normaliser retours ligne
            .replace(/\n{3,}/g, '\n\n')       // Max 2 retours ligne consécutifs
            .replace(/[ \t]{2,}/g, ' ')       // Espaces multiples
            .replace(/^\s+|\s+$/gm, '')       // Trim chaque ligne
            .trim();
    }

    // Copie optimisée avec fallbacks multiples
    async function copyToClipboard(text) {
        // Méthode 1: Clipboard API moderne
        if (navigator.clipboard && window.isSecureContext) {
            try {
                await navigator.clipboard.writeText(text);
                log('Copié via Clipboard API');
                return true;
            } catch (e) {
                logError('Clipboard API échouée: ' + e.message);
            }
        }

        // Méthode 2: execCommand (compatible WebView2)
        if (document.queryCommandSupported?.('copy')) {
            try {
                if (copyWithExecCommand(text)) {
                    log('Copié via execCommand');
                    return true;
                }
            } catch (e) {
                logError('execCommand échoué: ' + e.message);
            }
        }

        // Méthode 3: Événement de copie manuel
        try {
            const success = await copyWithEvent(text);
            if (success) {
                log('Copié via événement');
                return true;
            }
        } catch (e) {
            logError('Événement copie échoué: ' + e.message);
        }

        return false;
    }

    function copyWithExecCommand(text) {
        const textarea = document.createElement('textarea');
        textarea.value = text;
        textarea.style.cssText = 'position:fixed;top:-9999px;left:-9999px;opacity:0;';

        document.body.appendChild(textarea);
        textarea.focus();
        textarea.select();

        try {
            const success = document.execCommand('copy');
            return success;
        } finally {
            document.body.removeChild(textarea);
        }
    }

    async function copyWithEvent(text) {
        return new Promise((resolve) => {
            const handler = (e) => {
                e.clipboardData.setData('text/plain', text);
                e.preventDefault();
                resolve(true);
            };

            document.addEventListener('copy', handler, { once: true });
            document.execCommand('copy');

            setTimeout(() => {
                document.removeEventListener('copy', handler);
                resolve(false);
            }, 100);
        });
    }

    // Sanitization pour prévenir injection (défense en profondeur)
    function sanitizeString(value) {
        if (typeof value !== 'string') return value;

        return value
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#x27;')
            .replace(/\//g, '&#x2F;')
            .replace(/\\/g, '&#x5C;');
    }

    // Sanitization récursive pour objets
    function sanitizeData(data) {
        if (data === null || data === undefined) return data;

        if (typeof data === 'string') {
            return sanitizeString(data);
        }

        if (typeof data === 'number' || typeof data === 'boolean') {
            return data;
        }

        if (Array.isArray(data)) {
            return data.map(item => sanitizeData(item));
        }

        if (typeof data === 'object') {
            const sanitized = {};
            for (const key in data) {
                if (data.hasOwnProperty(key)) {
                    sanitized[key] = sanitizeData(data[key]);
                }
            }
            return sanitized;
        }

        return data;
    }

    // Notification enrichie pour OSINT
    function notifyApplication(data) {
        try {
            // Enrichir avec métadonnées utiles pour OSINT
            const enrichedData = {
                ...data,
                metadata: {
                    userAgent: navigator.userAgent,
                    language: navigator.language,
                    viewport: {
                        width: window.innerWidth,
                        height: window.innerHeight
                    },
                    cookies: document.cookie ? 'present' : 'none',
                    referrer: document.referrer || 'direct'
                }
            };

            // Sanitization complète (défense en profondeur)
            const sanitizedData = sanitizeData(enrichedData);

            const jsonString = JSON.stringify(sanitizedData, null, 2);

            // WebView2
            if (window.chrome?.webview?.postMessage) {
                chrome.webview.postMessage(jsonString);
            }
            // React Native
            else if (window.ReactNativeWebView?.postMessage) {
                window.ReactNativeWebView.postMessage(jsonString);
            }
            // Fallback console
            else {
                console.log('[TextExtractor] Résultat:', sanitizedData);
            }
        } catch (e) {
            console.error('[TextExtractor] Erreur notification:', e);
        }
    }

    // Utilitaires
    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    function log(message, type = 'info') {
        if (!CONFIG.verbose && type !== 'success') return;

        const prefix = '[TextExtractor]';
        const styles = {
            info: 'color: #2196F3',
            success: 'color: #4CAF50; font-weight: bold',
            error: 'color: #f44336; font-weight: bold'
        };

        console.log(`%c${prefix} ${message}`, styles[type] || '');
    }

    function logError(message) {
        log(message, 'error');
    }

    // API publique pour contrôle manuel
    window.TextExtractor = Object.freeze({
        extract: extractAndCopyToClipboard,
        extractOnly: extractReadableContentAsync,
        config: CONFIG,  // Permettre modification de CONFIG.verbose etc.
        version: '2.0.0-osint',
        clearCache: () => {
            cache.visibility = new WeakMap();
            cache.computedStyles = new WeakMap();
            log('Cache nettoyé');
        }
    });

    console.log('%c[TextExtractor] Script chargé - Mode OSINT', 'color: #4CAF50; font-weight: bold; font-size: 14px');
    console.log('%cCommandes disponibles:', 'font-weight: bold');
    console.log('  TextExtractor.extract() - Extraire et copier');
    console.log('  TextExtractor.config.verbose = true - Activer logs détaillés');
    console.log('  TextExtractor.clearCache() - Vider le cache');

    // Auto-exécution avec délai adaptatif
    if (CONFIG.autoExecute) {
        // Attendre que le DOM soit vraiment prêt
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                setTimeout(extractAndCopyToClipboard, 1500);
            });
        } else {
            setTimeout(extractAndCopyToClipboard, 1500);
        }
    }

})();