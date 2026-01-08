/**
 * @file Extract_Article_and_Link.js
 * @author ICAZA <github.wildly512@passinbox.com>
 * @version 2.1
 * @description Extract articles and links from a web page with enhanced features
 * @license MIT
 */

(function () {
    'use strict';

    // Configuration
    const CONFIG = {
        maxResults: 1000,
        minTextLength: 10,
        containerWidth: '500px',
        containerMaxHeight: '85vh'
    };

    // Utility: Sanitize text to prevent XSS
    function sanitizeText(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Utility: Validate URL
    function isValidUrl(url) {
        try {
            const parsed = new URL(url, window.location.href);
            return parsed.protocol === 'http:' || parsed.protocol === 'https:';
        } catch {
            return false;
        }
    }

    // Create styled table with sorting and filtering
    function createTable(headers, data, tableType) {
        const wrapper = document.createElement('div');
        wrapper.style.marginBottom = '20px';

        // Title
        const title = document.createElement('h3');
        title.textContent = tableType === 'paragraphs' ? 
            `📄 Paragraphes (${data.length})` : 
            `🔗 Liens (${data.length})`;
        title.style.margin = '10px 0';
        title.style.fontSize = '16px';
        title.style.color = '#333';
        wrapper.appendChild(title);

        // Search/Filter input
        const filterInput = document.createElement('input');
        filterInput.type = 'text';
        filterInput.placeholder = `Filtrer les ${tableType === 'paragraphs' ? 'paragraphes' : 'liens'}...`;
        filterInput.style.width = '100%';
        filterInput.style.padding = '8px';
        filterInput.style.marginBottom = '10px';
        filterInput.style.border = '1px solid #ccc';
        filterInput.style.borderRadius = '4px';
        filterInput.style.boxSizing = 'border-box';
        wrapper.appendChild(filterInput);

        // Table
        const table = document.createElement('table');
        table.style.width = '100%';
        table.style.border = '1px solid #ddd';
        table.style.borderCollapse = 'collapse';
        table.style.backgroundColor = 'white';
        table.style.fontSize = '13px';

        // Header
        const thead = document.createElement('thead');
        const headerRow = document.createElement('tr');
        headers.forEach(headerText => {
            const th = document.createElement('th');
            th.textContent = headerText;
            th.style.border = '1px solid #ddd';
            th.style.padding = '8px';
            th.style.backgroundColor = '#f5f5f5';
            th.style.fontWeight = 'bold';
            th.style.textAlign = 'left';
            th.style.position = 'sticky';
            th.style.top = '0';
            headerRow.appendChild(th);
        });
        thead.appendChild(headerRow);
        table.appendChild(thead);

        // Body
        const tbody = document.createElement('tbody');
        const allRows = [];

        data.forEach((rowData, index) => {
            const row = document.createElement('tr');
            row.style.backgroundColor = index % 2 === 0 ? 'white' : '#f9f9f9';
            row.dataset.searchText = '';

            rowData.forEach(cellData => {
                const td = document.createElement('td');
                td.style.border = '1px solid #ddd';
                td.style.padding = '8px';
                td.style.wordBreak = 'break-word';

                if (typeof cellData === 'string') {
                    td.textContent = cellData;
                    row.dataset.searchText += cellData.toLowerCase() + ' ';
                } else if (cellData instanceof HTMLElement) {
                    td.appendChild(cellData);
                    row.dataset.searchText += cellData.textContent.toLowerCase() + ' ';
                }
                row.appendChild(td);
            });

            tbody.appendChild(row);
            allRows.push(row);
        });

        table.appendChild(tbody);

        // Filter functionality
        filterInput.addEventListener('input', (e) => {
            const searchTerm = e.target.value.toLowerCase();
            let visibleCount = 0;

            allRows.forEach(row => {
                if (row.dataset.searchText.includes(searchTerm)) {
                    row.style.display = '';
                    visibleCount++;
                } else {
                    row.style.display = 'none';
                }
            });

            title.textContent = tableType === 'paragraphs' ? 
                `📄 Paragraphes (${visibleCount}/${data.length})` : 
                `🔗 Liens (${visibleCount}/${data.length})`;
        });

        wrapper.appendChild(table);
        return wrapper;
    }

    // Extract article URL with improved logic
    function getArticleUrl(element) {
        const searchDepth = 5;
        let current = element;

        for (let i = 0; i < searchDepth && current; i++) {
            // Check for article/section containers
            if (current.matches('article, section, [role="article"]')) {
                const link = current.querySelector('a[href]:not([href^="#"]):not([href^="javascript:"])');
                if (link && isValidUrl(link.href)) {
                    return link.href;
                }
            }

            // Check for heading links
            const heading = current.querySelector('h1, h2, h3, h4');
            if (heading) {
                const headingLink = heading.closest('a') || heading.querySelector('a');
                if (headingLink && isValidUrl(headingLink.href)) {
                    return headingLink.href;
                }
            }

            current = current.parentElement;
        }

        return null;
    }

    // Export functionality
    function exportData(paragraphs, links, format) {
        let content = '';
        const timestamp = new Date().toISOString().split('T')[0];

        if (format === 'csv') {
            content = 'Type,Contenu,URL\n';
            paragraphs.forEach(p => {
                const text = p.text.replace(/"/g, '""');
                const url = p.url || '';
                content += `"Paragraphe","${text}","${url}"\n`;
            });
            links.forEach(l => {
                const url = l.url.replace(/"/g, '""');
                content += `"Lien","${url}","${url}"\n`;
            });
        } else if (format === 'json') {
            content = JSON.stringify({
                extracted_date: new Date().toISOString(),
                source_url: window.location.href,
                paragraphs: paragraphs,
                links: links
            }, null, 2);
        }

        const blob = new Blob([content], { type: format === 'csv' ? 'text/csv' : 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `extract_${timestamp}.${format}`;
        a.click();
        URL.revokeObjectURL(url);
    }

    // Main extraction logic
    function extractContent() {
        // Extract paragraphs
        const paragraphs = Array.from(document.querySelectorAll('p'))
            .map(p => {
                const text = p.textContent.trim();
                if (text.length < CONFIG.minTextLength) return null;
                
                return {
                    text: text,
                    url: getArticleUrl(p)
                };
            })
            .filter(p => p !== null)
            .slice(0, CONFIG.maxResults);

        // Extract links
        const uniqueLinks = new Set();
        const links = Array.from(document.querySelectorAll('a[href]'))
            .map(a => {
                const href = a.href;
                const normalizedHref = href.trim().toLowerCase();
                if (
                    !isValidUrl(href) ||
                    normalizedHref.startsWith('javascript:') ||
                    normalizedHref.startsWith('data:') ||
                    normalizedHref.startsWith('vbscript:') ||
                    href.startsWith('#')
                ) {
                    return null;
                }
                if (uniqueLinks.has(href)) return null;
                
                uniqueLinks.add(href);
                return {
                    url: href,
                    text: a.textContent.trim() || href
                };
            })
            .filter(l => l !== null)
            .slice(0, CONFIG.maxResults);

        return { paragraphs, links };
    }

    // Create UI
    function createUI(paragraphs, links) {
        // Container
        const container = document.createElement('div');
        container.id = 'content-extractor-panel';
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            width: ${CONFIG.containerWidth};
            max-height: ${CONFIG.containerMaxHeight};
            background: white;
            border: 2px solid #333;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.3);
            z-index: 2147483647;
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
        `;

        // Header
        const header = document.createElement('div');
        header.style.cssText = `
            padding: 15px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-radius: 6px 6px 0 0;
            display: flex;
            justify-content: space-between;
            align-items: center;
        `;

        const headerTitle = document.createElement('h2');
        headerTitle.textContent = '📊 Extracteur de Contenu';
        headerTitle.style.cssText = 'margin: 0; font-size: 18px; font-weight: 600;';
        header.appendChild(headerTitle);

        // Buttons container
        const buttonGroup = document.createElement('div');
        buttonGroup.style.display = 'flex';
        buttonGroup.style.gap = '5px';

        // Export buttons
        const exportCsvBtn = document.createElement('button');
        exportCsvBtn.textContent = '📥 CSV';
        exportCsvBtn.style.cssText = `
            padding: 5px 10px;
            background: white;
            color: #333;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 12px;
            font-weight: 500;
        `;
        exportCsvBtn.onclick = () => exportData(paragraphs, links, 'csv');

        const exportJsonBtn = document.createElement('button');
        exportJsonBtn.textContent = '📥 JSON';
        exportJsonBtn.style.cssText = exportCsvBtn.style.cssText;
        exportJsonBtn.onclick = () => exportData(paragraphs, links, 'json');

        // Close button
        const closeBtn = document.createElement('button');
        closeBtn.textContent = '✕';
        closeBtn.style.cssText = `
            padding: 5px 10px;
            background: rgba(255,255,255,0.2);
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 16px;
            font-weight: bold;
        `;
        closeBtn.onclick = () => container.remove();

        buttonGroup.appendChild(exportCsvBtn);
        buttonGroup.appendChild(exportJsonBtn);
        buttonGroup.appendChild(closeBtn);
        header.appendChild(buttonGroup);
        container.appendChild(header);

        // Content area
        const content = document.createElement('div');
        content.style.cssText = `
            padding: 15px;
            overflow-y: auto;
            max-height: calc(${CONFIG.containerMaxHeight} - 70px);
        `;

        // Prepare table data
        const paragraphData = paragraphs.map(p => {
            if (p.url) {
                const link = document.createElement('a');
                link.href = p.url;
                link.textContent = p.text;
                link.target = '_blank';
                link.rel = 'noopener noreferrer';
                link.style.cssText = 'color: #667eea; text-decoration: none;';
                link.onmouseover = () => link.style.textDecoration = 'underline';
                link.onmouseout = () => link.style.textDecoration = 'none';
                return [link];
            }
            return [p.text];
        });

        const linkData = links.map(l => {
            const link = document.createElement('a');
            link.href = l.url;
            link.textContent = l.text.length > 80 ? l.text.substring(0, 77) + '...' : l.text;
            link.title = l.url;
            link.target = '_blank';
            link.rel = 'noopener noreferrer';
            link.style.cssText = 'color: #667eea; text-decoration: none;';
            link.onmouseover = () => link.style.textDecoration = 'underline';
            link.onmouseout = () => link.style.textDecoration = 'none';
            return [link];
        });

        content.appendChild(createTable(['Contenu'], paragraphData, 'paragraphs'));
        content.appendChild(createTable(['Lien'], linkData, 'links'));

        container.appendChild(content);
        document.body.appendChild(container);

        // Make draggable
        let isDragging = false;
        let currentX, currentY, initialX, initialY;

        header.style.cursor = 'move';
        header.onmousedown = (e) => {
            if (e.target === closeBtn || e.target === exportCsvBtn || e.target === exportJsonBtn) return;
            isDragging = true;
            initialX = e.clientX - container.offsetLeft;
            initialY = e.clientY - container.offsetTop;
        };

        document.onmousemove = (e) => {
            if (isDragging) {
                e.preventDefault();
                currentX = e.clientX - initialX;
                currentY = e.clientY - initialY;
                container.style.left = currentX + 'px';
                container.style.top = currentY + 'px';
                container.style.right = 'auto';
            }
        };

        document.onmouseup = () => {
            isDragging = false;
        };
    }

    // Initialize
    try {
        // Check if already exists
        const existing = document.getElementById('content-extractor-panel');
        if (existing) {
            existing.remove();
            console.log('Extractor closed');
            return;
        }

        console.log('Extraction in progress...');
        const { paragraphs, links } = extractContent();
        
        console.log(`${paragraphs.length} paragraphs and ${links.length} links found`);
        
        createUI(paragraphs, links);
    } catch (error) {
        console.error('Error during extraction:', error);
        alert('An error occurred while extracting the content.');
    }
})();