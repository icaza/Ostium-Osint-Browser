/**
 * @file Extract_Content_Simple.js
 * @author ICAZA <github.wildly512@passinbox.com>
 * @version 2.2
 * @description Extract web page content and return as plain text
 */

(function () {
    'use strict';

    const CONFIG = {
        minParagraphLength: 20,
        minTotalLength: 50,
        maxDOMDepth: 500,
        verbose: false
    };

    const cache = {
        visibility: new WeakMap(),
        computedStyles: new WeakMap()
    };

    function extractContent() {
        try {
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
                    log(`Essay: ${strategy.name}`);
                    const text = strategy.fn();

                    if (text && text.length > 0) {
                        const score = calculateQualityScore(text) * strategy.weight;

                        log(`${strategy.name}: ${text.length} chars, score: ${score.toFixed(2)}`);

                        if (score > bestResult.score) {
                            bestResult = { text, score, method: strategy.name };
                        }

                        if (score > 8000) {
                            log(`Excellent score achieved with ${strategy.name}`);
                            break;
                        }
                    }
                } catch (e) {
                    logError(`${strategy.name} failed: ${e.message}`);
                }
            }

            const cleanedText = cleanExtractedText(bestResult.text);

            log(`✓ Extraction successful: ${cleanedText.length} characters via ${bestResult.method}`, 'success');

            return cleanedText;

        } catch (error) {
            logError(`Extraction error: ${error.message}`);
            return '';
        }
    }

    function extractFromSchemaOrg() {
        const scripts = document.querySelectorAll('script[type="application/ld+json"]');

        for (const script of scripts) {
            try {
                const data = JSON.parse(script.textContent);
                const article = findArticleInSchema(data);

                if (article && article.articleBody) {
                    log('Schema.org find !');
                    return article.articleBody;
                }
            } catch (e) { }
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

    function extractByReadability() {
        const candidates = [];
        const containers = document.querySelectorAll('div, section, article, main');

        for (const container of containers) {
            if (!isElementVisibleCached(container)) continue;

            const text = container.innerText || '';
            const links = container.querySelectorAll('a');
            const paragraphs = container.querySelectorAll('p');

            const textLength = text.length;
            const linkLength = Array.from(links).reduce((sum, a) => sum + (a.innerText?.length || 0), 0);
            const linkDensity = textLength > 0 ? linkLength / textLength : 1;

            const score = textLength * (1 - linkDensity) * Math.log(paragraphs.length + 1);

            if (score > 100) {
                candidates.push({ element: container, score, text });
            }
        }

        candidates.sort((a, b) => b.score - a.score);

        if (candidates.length > 0) {
            log(`Readability: best score = ${candidates[0].score.toFixed(2)}`);
            return candidates[0].text;
        }

        return '';
    }

    function calculateQualityScore(text) {
        if (!text) return 0;

        const length = text.length;
        const lines = text.split('\n').filter(l => l.trim().length > 20).length;
        const words = text.split(/\s+/).length;
        const avgWordLength = length / Math.max(words, 1);

        let penalty = 0;

        const uniqueLines = new Set(text.split('\n').map(l => l.trim()));
        const repetitionRatio = uniqueLines.size / Math.max(lines, 1);
        if (repetitionRatio < 0.5) penalty += 2000;
        if (length < 500) penalty += 1000;
        if (avgWordLength < 4) penalty += 1000;

        return Math.max(0, length + (lines * 50) + (words * 2) - penalty);
    }

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

        return textParts.join(' ');
    }

    function extractFromAllParagraphs() {
        const selectors = 'p, h1, h2, h3, h4, h5, h6, li, blockquote';
        const elements = document.querySelectorAll(selectors);
        const texts = new Set();

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

    function cleanExtractedText(text) {
        if (!text) return '';

        return text
            .replace(/\r\n/g, '\n')
            .replace(/\n{3,}/g, '\n\n')
            .replace(/[ \t]{2,}/g, ' ')
            .replace(/^\s+|\s+$/gm, '')
            .trim();
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

    window.TextExtractor = Object.freeze({
        extract: extractContent,
        config: CONFIG,
        version: '2.2.0'
    });

    console.log('%c[TextExtractor] OOBai Script loaded - v2.1.0', 'color: #4CAF50; font-weight: bold; font-size: 14px');

    const result = extractContent();

    return result;

})();