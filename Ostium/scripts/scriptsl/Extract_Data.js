/**
 * @file Extract_Data.js
 * @author ICAZA <github.wildly512@passinbox.com>
 * @version 1.3
 * @description Extract Data Enhanced.
 */

(function () {
    'use strict';

    console.log('Extract Data - Script injected');
    
    if (window.__OSINT_EXTRACTOR_LOADED__) {
        console.warn('Extract Data already loaded');
        alert('Extract Data is already active on this page.');
        return;
    }
    window.__OSINT_EXTRACTOR_LOADED__ = true;

    const SECURITY_CONFIG = {
        MAX_TEXT_LENGTH: 10000,
        MAX_URL_LENGTH: 2048,
        MAX_ITEMS_PER_CATEGORY: 5000,
        ALLOWED_PROTOCOLS: ['http:', 'https:', 'data:', 'ftp:'],
        BLOCKED_DOMAINS: ['javascript:', 'vbscript:', 'file:'],
        DEBOUNCE_DELAY: 300
    };

    console.log('Loading DOMPurify...');

    const domPurifyScript = document.createElement('script');
    domPurifyScript.src = 'https://cdnjs.cloudflare.com/ajax/libs/dompurify/3.2.4/purify.min.js';
    domPurifyScript.crossOrigin = 'anonymous';

    const loadTimeout = setTimeout(() => {
        console.error('DOMPurify loading timeout - Starting without it');
        window.DOMPurify = {
            sanitize: (text) => {
                if (typeof text !== 'string') return text;
                return text.replace(/[<>]/g, '');
            }
        };
        initScript();
    }, 5000);
    
    domPurifyScript.onload = () => {
        clearTimeout(loadTimeout);
        console.log('DOMPurify loaded successfully');
        initScript();
    };
    
    domPurifyScript.onerror = (error) => {
        clearTimeout(loadTimeout);
        console.error('Failed to load DOMPurify:', error);
        alert('Error loading DOMPurify. Extraction will continue with reduced security.');
        
        window.DOMPurify = {
            sanitize: (text) => {
                if (typeof text !== 'string') return text;
                return text.replace(/[<>]/g, '');
            }
        };
        initScript();
    };
    
    document.head.appendChild(domPurifyScript);

    function initScript() {
        if (typeof DOMPurify === 'undefined') {
            console.error('DOMPurify not available');
            return;
        }

        const sanitizeConfig = {
            ALLOWED_URI_REGEXP: /^(https?|ftp|data):/i,
            FORBID_TAGS: ['script', 'iframe', 'object', 'embed', 'applet', 'link'],
            FORBID_ATTR: ['onclick', 'onload', 'onerror', 'onmouseover', 'onfocus', 'onblur']
        };

        const collectedData = {
            metadata: {
                url: window.location.href,
                title: document.title,
                timestamp: new Date().toISOString(),
                userAgent: navigator.userAgent,
                language: document.documentElement.lang || 'unknown'
            },
            texts: [],
            images: [],
            videos: [],
            links: [],
            emails: [],
            phones: [],
            metadata_tags: [],
            social_media: []
        };

        const SecurityUtils = {
            isValidUrl(url) {
                try {
                    const parsed = new URL(url);
                    return SECURITY_CONFIG.ALLOWED_PROTOCOLS.includes(parsed.protocol) &&
                           !SECURITY_CONFIG.BLOCKED_DOMAINS.some(blocked => 
                               url.toLowerCase().includes(blocked)
                           ) &&
                           url.length <= SECURITY_CONFIG.MAX_URL_LENGTH;
                } catch {
                    return false;
                }
            },

            sanitizeText(text) {
                if (!text || typeof text !== 'string') return '';
                const cleaned = DOMPurify.sanitize(text.trim(), { ALLOWED_TAGS: [] });
                return cleaned.substring(0, SECURITY_CONFIG.MAX_TEXT_LENGTH);
            },

            debounce(func, wait) {
                let timeout;
                return function executedFunction(...args) {
                    clearTimeout(timeout);
                    timeout = setTimeout(() => func(...args), wait);
                };
            },

            extractEmails(text) {
                const emailRegex = /\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b/g;
                return [...new Set(text.match(emailRegex) || [])];
            },

            extractPhones(text) {
                const phoneRegex = /(\+?\d{1,3}[-.\s]?)?\(?\d{2,4}\)?[-.\s]?\d{2,4}[-.\s]?\d{2,4}[-.\s]?\d{0,4}/g;
                const matches = text.match(phoneRegex) || [];
                return [...new Set(matches.filter(p => p.replace(/\D/g, '').length >= 8))];
            },

            isSocialMediaUrl(url) {
                const socialDomains = [
                    'facebook.com', 'twitter.com', 'x.com', 'instagram.com', 
                    'linkedin.com', 'youtube.com', 'tiktok.com', 'reddit.com',
                    'pinterest.com', 'snapchat.com', 'whatsapp.com', 'telegram.org'
                ];
                return socialDomains.some(domain => url.toLowerCase().includes(domain));
            }
        };

        // User interface
        const container = document.createElement('div');
        container.id = 'osint-extractor-container';
        container.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100vw;
            height: 100vh;
            background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
            overflow-y: auto;
            z-index: 2147483647;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Arial, sans-serif;
            padding: 20px;
            box-sizing: border-box;
            color: white;
        `;

        const styles = {
            table: `
                width: 100%;
                margin-bottom: 20px;
                border-collapse: separate;
                border-spacing: 0;
                background: white;
                color: black;
                border-radius: 12px;
                overflow: hidden;
                box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            `,
            th: `
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: white;
                border: none;
                padding: 14px;
                text-align: left;
                font-weight: 600;
                cursor: pointer;
                transition: background 0.3s;
            `,
            td: `
                border: 1px solid #e0e0e0;
                padding: 12px;
                word-break: break-word;
                max-width: 400px;
                vertical-align: top;
                color: #333 !important;
                background: white;
            `,
            button: `
                padding: 12px 24px;
                font-size: 14px;
                font-weight: 600;
                border: none;
                border-radius: 8px;
                cursor: pointer;
                transition: all 0.3s;
                box-shadow: 0 2px 4px rgba(0,0,0,0.2);
                margin: 5px;
            `
        };

        const sectionColors = {
            metadata: '#e3f2fd',
            text: '#d1ecf1',
            images: '#d4edda',
            videos: '#fff3cd',
            links: '#f8d7da',
            emails: '#e1bee7',
            phones: '#ffccbc',
            social: '#c5e1a5'
        };

        // Creating sections
        function createSection(title, color, id) {
            const section = document.createElement('div');
            section.id = `section-${id}`;
            section.style.marginBottom = '40px';
            
            const header = document.createElement('h2');
            header.innerHTML = `<span style="margin-right: 10px;">📊</span>${SecurityUtils.sanitizeText(title)}`;
            header.style.cssText = `
                background: ${color};
                padding: 18px;
                border-radius: 12px;
                color: #1a1a1a;
                margin-bottom: 15px;
                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                font-size: 1.4em;
            `;
            
            const table = document.createElement('table');
            table.style.cssText = styles.table;
            table.dataset.category = id;
            
            section.appendChild(header);
            section.appendChild(table);
            container.appendChild(section);
            return table;
        }

        // Creating the tables
        const metadataTable = createSection('Metadata', sectionColors.metadata, 'metadata');
        const textTable = createSection('Texts', sectionColors.text, 'texts');
        const imagesTable = createSection('Images', sectionColors.images, 'images');
        const videosTable = createSection('Videos', sectionColors.videos, 'videos');
        const linksTable = createSection('Links', sectionColors.links, 'links');
        const emailsTable = createSection('Emails', sectionColors.emails, 'emails');
        const phonesTable = createSection('Phones', sectionColors.phones, 'phones');
        const socialTable = createSection('Social Media', sectionColors.social, 'social');

        function addTableHeaders(table, headers) {
            const thead = document.createElement('thead');
            const row = document.createElement('tr');
            headers.forEach((headerText, index) => {
                const th = document.createElement('th');
                th.textContent = SecurityUtils.sanitizeText(headerText);
                th.style.cssText = styles.th;
                th.dataset.column = index;
                th.addEventListener('click', () => sortTable(table, index));
                th.addEventListener('mouseenter', () => {
                    th.style.background = 'linear-gradient(135deg, #764ba2 0%, #667eea 100%)';
                });
                th.addEventListener('mouseleave', () => {
                    th.style.background = 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)';
                });
                row.appendChild(th);
            });
            thead.appendChild(row);
            table.appendChild(thead);
            const tbody = document.createElement('tbody');
            table.appendChild(tbody);
        }

        addTableHeaders(metadataTable, ['Property', 'Value']);
        addTableHeaders(textTable, ['Title', 'Tag', 'Text', 'Length']);
        addTableHeaders(imagesTable, ['Preview', 'URL', 'Alt Text', 'Size']);
        addTableHeaders(videosTable, ['Preview', 'URL', 'Duration']);
        addTableHeaders(linksTable, ['Text', 'URL', 'Type']);
        addTableHeaders(emailsTable, ['Email', 'Context']);
        addTableHeaders(phonesTable, ['Phone', 'Context']);
        addTableHeaders(socialTable, ['Platform', 'URL', 'Type']);

        function addTableRow(table, cells, dataCategory, rawData = {}) {
            const category = collectedData[dataCategory];
            if (category && category.length >= SECURITY_CONFIG.MAX_ITEMS_PER_CATEGORY) {
                return;
            }

            const tbody = table.querySelector('tbody') || table;
            const row = document.createElement('tr');
            row.style.transition = 'background 0.2s';
            row.addEventListener('mouseenter', () => {
                row.style.background = '#f5f5f5';
            });
            row.addEventListener('mouseleave', () => {
                row.style.background = 'white';
            });

            cells.forEach((cellData) => {
                const td = document.createElement('td');
                td.style.cssText = styles.td;

                if (cellData instanceof HTMLElement) {
                    const safeElement = cellData.cloneNode(true);
                    if (['IMG', 'VIDEO'].includes(safeElement.tagName)) {
                        const src = safeElement.src;
                        if (SecurityUtils.isValidUrl(src)) {
                            safeElement.src = src;
                            td.appendChild(safeElement);
                        }
                    } else {
                        td.appendChild(safeElement);
                    }
                } else if (typeof cellData === 'string') {
                    if (SecurityUtils.isValidUrl(cellData)) {
                        const link = document.createElement('a');
                        link.href = cellData;
                        link.target = '_blank';
                        link.rel = 'noopener noreferrer';
                        link.textContent = cellData.length > 60 ? cellData.substring(0, 60) + '...' : cellData;
                        link.style.wordBreak = 'break-all';
                        link.style.color = '#1976d2';
                        td.appendChild(link);
                    } else {
                        td.textContent = SecurityUtils.sanitizeText(cellData);
                    }
                } else {
                    td.textContent = String(cellData);
                }

                row.appendChild(td);
            });

            tbody.appendChild(row);
            
            if (dataCategory && collectedData[dataCategory]) {
                collectedData[dataCategory].push(rawData);
            }
        }

        // Metadata extraction
        function extractMetadata() {
            const metaTags = document.querySelectorAll('meta');
            const metadata = { ...collectedData.metadata };

            metaTags.forEach(tag => {
                const name = tag.getAttribute('name') || tag.getAttribute('property');
                const content = tag.getAttribute('content');
                if (name && content) {
                    metadata[SecurityUtils.sanitizeText(name)] = SecurityUtils.sanitizeText(content);
                }
            });

            Object.entries(metadata).forEach(([key, value]) => {
                addTableRow(metadataTable, [key, value], 'metadata_tags', { key, value });
            });
        }

        function traverseDOM(node, title = '', depth = 0) {
            if (depth > 50) return; // Protection against deep recursion

            if (node.nodeType === Node.ELEMENT_NODE) {
                // Extract title
                if (/^H[1-6]$/i.test(node.tagName)) {
                    title = SecurityUtils.sanitizeText(node.textContent);
                }

                // Extract text
                if (node.childNodes.length === 1 && node.childNodes[0].nodeType === Node.TEXT_NODE) {
                    const text = SecurityUtils.sanitizeText(node.textContent);
                    if (text && text.length > 3) {
                        addTableRow(textTable, [
                            title || '(no title)',
                            node.tagName,
                            text.length > 100 ? text.substring(0, 100) + '...' : text,
                            text.length
                        ], 'texts', { title, tag: node.tagName, text, length: text.length });

                        // Extract emails and phone numbers from text
                        SecurityUtils.extractEmails(text).forEach(email => {
                            addTableRow(emailsTable, [email, text.substring(0, 50)], 'emails', { email, context: text });
                        });

                        SecurityUtils.extractPhones(text).forEach(phone => {
                            addTableRow(phonesTable, [phone, text.substring(0, 50)], 'phones', { phone, context: text });
                        });
                    }
                }

                // Extract images
                if (node.tagName === 'IMG' && node.src && SecurityUtils.isValidUrl(node.src)) {
                    const img = new Image();
                    img.src = node.src;
                    img.style.width = '120px';
                    img.style.borderRadius = '8px';
                    img.loading = 'lazy';
                    const alt = SecurityUtils.sanitizeText(node.alt || '');
                    const size = node.naturalWidth && node.naturalHeight ? 
                                `${node.naturalWidth}x${node.naturalHeight}` : 'Unknown';
                    addTableRow(imagesTable, [img, node.src, alt, size], 'images', 
                        { url: node.src, alt, size });
                }

                // Extract videos
                if (node.tagName === 'VIDEO' && node.src && SecurityUtils.isValidUrl(node.src)) {
                    const video = document.createElement('video');
                    video.src = node.src;
                    video.controls = true;
                    video.style.width = '120px';
                    video.style.borderRadius = '8px';
                    const duration = node.duration ? `${Math.round(node.duration)}s` : 'Unknown';
                    addTableRow(videosTable, [video, node.src, duration], 'videos', 
                        { url: node.src, duration });
                }

                // Extract links
                if (node.tagName === 'A' && node.href && SecurityUtils.isValidUrl(node.href)) {
                    const text = SecurityUtils.sanitizeText(node.textContent) || '(no text)';
                    const type = node.href.includes('#') ? 'Internal' : 'External';
                    addTableRow(linksTable, [text, node.href, type], 'links', 
                        { text, url: node.href, type });

                    // Social media verification
                    if (SecurityUtils.isSocialMediaUrl(node.href)) {
                        const platform = new URL(node.href).hostname.split('.').slice(-2, -1)[0];
                        addTableRow(socialTable, [
                            platform.charAt(0).toUpperCase() + platform.slice(1),
                            node.href,
                            'Profile/Page'
                        ], 'social_media', { platform, url: node.href });
                    }
                }

                // Recursion
                Array.from(node.childNodes).forEach(child => traverseDOM(child, title, depth + 1));
            }
        }

        // Table sorting
        function sortTable(table, columnIndex) {
            const tbody = table.querySelector('tbody');
            if (!tbody) return;

            const rows = Array.from(tbody.querySelectorAll('tr'));
            let ascending = table.dataset.sortAscending !== 'true';

            rows.sort((a, b) => {
                const aText = (a.children[columnIndex]?.textContent || '').toLowerCase();
                const bText = (b.children[columnIndex]?.textContent || '').toLowerCase();
                const result = aText.localeCompare(bText, undefined, { numeric: true });
                return ascending ? result : -result;
            });

            table.dataset.sortAscending = ascending;
            rows.forEach(row => tbody.appendChild(row));
        }

        // Search bar
        function createSearchBar() {
            const searchContainer = document.createElement('div');
            searchContainer.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 2147483648;
                display: flex;
                gap: 10px;
                align-items: center;
            `;

            const searchInput = document.createElement('input');
            searchInput.type = 'text';
            searchInput.placeholder = '🔍 Rechercher...';
            searchInput.style.cssText = `
                padding: 12px 16px;
                font-size: 15px;
                border-radius: 8px;
                border: 2px solid #667eea;
                width: 300px;
                box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            `;

            const clearBtn = document.createElement('button');
            clearBtn.textContent = '✕';
            clearBtn.style.cssText = styles.button + `
                background: #f44336;
                color: white;
                padding: 12px 16px;
            `;

            const debouncedSearch = SecurityUtils.debounce((searchTerm) => {
                const allRows = container.querySelectorAll('tbody tr');
                let visibleCount = 0;

                allRows.forEach(row => {
                    const rowText = Array.from(row.children)
                        .map(cell => cell.textContent.toLowerCase())
                        .join(' ');
                    const isVisible = rowText.includes(searchTerm.toLowerCase());
                    row.style.display = isVisible ? '' : 'none';
                    if (isVisible) visibleCount++;
                });

                searchInput.style.borderColor = visibleCount === 0 ? '#f44336' : '#667eea';
            }, SECURITY_CONFIG.DEBOUNCE_DELAY);

            searchInput.addEventListener('input', (e) => debouncedSearch(e.target.value));

            clearBtn.addEventListener('click', () => {
                searchInput.value = '';
                debouncedSearch('');
            });

            searchContainer.appendChild(searchInput);
            searchContainer.appendChild(clearBtn);
            container.appendChild(searchContainer);
        }

        // Statistics
        function createStats() {
            const statsContainer = document.createElement('div');
            statsContainer.id = 'stats-container';
            statsContainer.style.cssText = `
                position: fixed;
                bottom: 20px;
                left: 20px;
                background: rgba(255, 255, 255, 0.95);
                padding: 20px;
                border-radius: 12px;
                color: #333;
                z-index: 2147483647;
                box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                min-width: 200px;
            `;

            const stats = {
                texts: collectedData.texts.length,
                images: collectedData.images.length,
                videos: collectedData.videos.length,
                links: collectedData.links.length,
                emails: collectedData.emails.length,
                phones: collectedData.phones.length,
                social: collectedData.social_media.length
            };

            statsContainer.innerHTML = `
                <strong style="font-size: 1.2em; display: block; margin-bottom: 10px;">📈 Statistics</strong>
                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 8px;">
                    <div>📝 Texts:</div><div><strong>${stats.texts}</strong></div>
                    <div>🖼️ Images:</div><div><strong>${stats.images}</strong></div>
                    <div>🎥 Videos:</div><div><strong>${stats.videos}</strong></div>
                    <div>🔗 Links:</div><div><strong>${stats.links}</strong></div>
                    <div>📧 Emails:</div><div><strong>${stats.emails}</strong></div>
                    <div>📱 Phones:</div><div><strong>${stats.phones}</strong></div>
                    <div>🌐 Social:</div><div><strong>${stats.social}</strong></div>
                </div>
            `;

            container.appendChild(statsContainer);
        }

        // Export buttons
        function createActionButtons() {
            const buttonContainer = document.createElement('div');
            buttonContainer.style.cssText = `
                position: fixed;
                top: 80px;
                right: 20px;
                display: flex;
                flex-direction: column;
                gap: 10px;
                z-index: 2147483648;
            `;

            const buttons = [
                {
                    text: '📄 CSV',
                    color: '#6c757d',
                    action: exportCSV
                },
                {
                    text: '📋 JSON',
                    color: '#007bff',
                    action: exportJSON
                },
                {
                    text: '🖨️ PDF',
                    color: '#28a745',
                    action: exportPDF
                },
                {
                    text: '📊 HTML',
                    color: '#17a2b8',
                    action: exportHTML
                },
                {
                    text: '❌ Close',
                    color: '#dc3545',
                    action: closeExtractor
                }
            ];

            buttons.forEach(btn => {
                const button = document.createElement('button');
                button.textContent = btn.text;
                button.style.cssText = styles.button + `
                    background: ${btn.color};
                    color: white;
                    width: 140px;
                `;
                button.addEventListener('click', btn.action);
                button.addEventListener('mouseenter', () => {
                    button.style.transform = 'translateY(-2px)';
                    button.style.boxShadow = '0 4px 8px rgba(0,0,0,0.3)';
                });
                button.addEventListener('mouseleave', () => {
                    button.style.transform = 'translateY(0)';
                    button.style.boxShadow = '0 2px 4px rgba(0,0,0,0.2)';
                });
                buttonContainer.appendChild(button);
            });

            container.appendChild(buttonContainer);
        }

        // Export functions
        function exportCSV() {
            try {
                let csvContent = `Data Export - ${collectedData.metadata.url}\n`;
                csvContent += `Generated: ${collectedData.metadata.timestamp}\n\n`;

                Object.entries(collectedData).forEach(([category, data]) => {
                    if (category === 'metadata' || !Array.isArray(data) || data.length === 0) return;
                    
                    csvContent += `\n${category.toUpperCase()}\n`;
                    const headers = Object.keys(data[0] || {}).join(',');
                    csvContent += headers + '\n';
                    
                    data.forEach(item => {
                        const row = Object.values(item).map(val => {
                            const str = String(val).replace(/"/g, '""');
                            return `"${str}"`;
                        }).join(',');
                        csvContent += row + '\n';
                    });
                });

                downloadFile(csvContent, 'text/csv', 'csv');
            } catch (err) {
                console.error('CSV export error:', err);
                alert('Error exporting CSV');
            }
        }

        function exportJSON() {
            try {
                const json = JSON.stringify(collectedData, null, 2);
                downloadFile(json, 'application/json', 'json');
            } catch (err) {
                console.error('JSON export error:', err);
                alert('Error exporting JSON');
            }
        }

        function exportHTML() {
            try {
                const html = `
<!DOCTYPE html>
<html lang="fr">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Report - ${SecurityUtils.sanitizeText(collectedData.metadata.title)}</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; background: #f5f5f5; }
        .container { max-width: 1200px; margin: 0 auto; background: white; padding: 30px; border-radius: 12px; }
        h1 { color: #1976d2; }
        table { width: 100%; border-collapse: collapse; margin: 20px 0; }
        th { background: #1976d2; color: white; padding: 12px; text-align: left; }
        td { border: 1px solid #ddd; padding: 10px; }
        tr:nth-child(even) { background: #f9f9f9; }
        img, video { max-width: 150px; border-radius: 8px; }
    </style>
</head>
<body>
    <div class="container">
        <h1>🔍 Data Report</h1>
        ${DOMPurify.sanitize(container.innerHTML, sanitizeConfig)}
    </div>
</body>
</html>`;
                downloadFile(html, 'text/html', 'html');
            } catch (err) {
                console.error('HTML export error:', err);
                alert('Error exporting HTML');
            }
        }

        function exportPDF() {
            try {
                const printWindow = window.open('', '_blank');
                const content = DOMPurify.sanitize(container.innerHTML, sanitizeConfig);
                
                printWindow.document.write(`
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>Report - ${SecurityUtils.sanitizeText(collectedData.metadata.title)}</title>
                        <style>
                            body { padding: 20px; font-family: Arial, sans-serif; }
                            table { width: 100%; border-collapse: collapse; page-break-inside: avoid; }
                            th { background: #1976d2; color: white; padding: 10px; }
                            td { border: 1px solid #ddd; padding: 8px; }
                            img, video { max-width: 150px; }
                            h2 { color: #1976d2; page-break-before: always; }
                            h2:first-of-type { page-break-before: avoid; }
                        </style>
                    </head>
                    <body>${content}</body>
                    </html>
                `);
                printWindow.document.close();
                setTimeout(() => printWindow.print(), 500);
            } catch (err) {
                console.error('PDF export error:', err);
                alert('Error exporting PDF');
            }
        }

        function downloadFile(content, mimeType, extension) {
            const blob = new Blob([content], { type: `${mimeType};charset=utf-8;` });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            const hostname = SecurityUtils.sanitizeText(window.location.hostname.replace(/[^a-z0-9]/gi, '_'));
            a.href = url;
            a.download = `osint_${hostname}_${Date.now()}.${extension}`;
            a.click();
            setTimeout(() => URL.revokeObjectURL(url), 100);
        }

        function closeExtractor() {
            if (confirm('Do you really want to close Extract Data?')) {
                container.remove();
                delete window.__OSINT_EXTRACTOR_LOADED__;
            }
        }

        // Export functions
        function init() {
            console.log('Extract Data - Starting extraction...');
            
            // Saving original content
            const originalBody = document.body.cloneNode(true);
            
            try {
                extractMetadata();
                traverseDOM(originalBody);

                while (document.body.firstChild) {
                    document.body.removeChild(document.body.firstChild);
                }
                
                // Adding the interface
                document.body.appendChild(container);
                
                // Creating UI components
                createSearchBar();
                createStats();
                createActionButtons();
                
                console.log('Extraction completed successfully');
                console.log('Data collected:', {
                    texts: collectedData.texts.length,
                    images: collectedData.images.length,
                    videos: collectedData.videos.length,
                    links: collectedData.links.length,
                    emails: collectedData.emails.length,
                    phones: collectedData.phones.length,
                    social: collectedData.social_media.length
                });
            } catch (error) {
                console.error('Extraction error:', error);
                alert('An error occurred during extraction. Check the console for details..');
            }
        }

        init();
    }
})();