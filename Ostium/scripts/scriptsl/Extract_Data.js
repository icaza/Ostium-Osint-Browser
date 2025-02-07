(function () {
    // Extract_Data_v3
    const domPurifyScript = document.createElement('script');
    domPurifyScript.src = 'https://cdnjs.cloudflare.com/ajax/libs/dompurify/3.0.5/purify.min.js';
    domPurifyScript.onload = initScript;
    document.head.appendChild(domPurifyScript);

    function initScript() {
        const sanitizeConfig = {
            ALLOWED_URI_REGEXP: /^(https?|ftp|data):/i,
            FORBID_TAGS: ['script', 'iframe', 'object', 'embed'],
            FORBID_ATTR: ['onclick', 'onload', 'onerror']
        };

        const container = document.createElement('div');
        container.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100vw;
            height: 100vh;
            background: rgb(41, 44, 51);
            overflow-y: auto;
            z-index: 9999;
            font-family: Arial, sans-serif;
            padding: 20px;
            box-sizing: border-box;
            color: white;
        `;

        const tableStyle = `
            width: 100%;
            margin-bottom: 20px;
            border-collapse: separate;
            border-spacing: 0;
            background: white;
            color: black;
            border-radius: 8px;
            overflow: hidden;
        `;

        const thStyle = `
            background-color: #f4f4f4;
            border: 1px solid #ddd;
            padding: 12px;
            text-align: left;
            font-weight: bold;
            cursor: pointer;
        `;

        const tdStyle = `
            border: 1px solid #ddd;
            padding: 12px;
            word-break: break-word;
            max-width: 300px;
            vertical-align: top;
            color: #333 !important;
        `;

        const sectionColors = {
            text: '#d1ecf1',
            images: '#d4edda',
            videos: '#fff3cd',
            links: '#f8d7da'
        };

        const collectedData = {
            texts: [],
            images: [],
            videos: [],
            links: []
        };

        function createSection(title, color) {
            const section = document.createElement('div');
            section.style.marginBottom = '40px';
            const header = document.createElement('h2');
            header.textContent = title;
            header.style.cssText = `
                background: ${CSS.escape(color)};
                padding: 15px;
                border-radius: 8px;
                color: #333;
                margin-bottom: 10px;
            `;
            const table = document.createElement('table');
            table.style.cssText = tableStyle;
            section.appendChild(header);
            section.appendChild(table);
            container.appendChild(section);
            return table;
        }

        const textTable = createSection('Texts', sectionColors.text);
        const imagesTable = createSection('Images', sectionColors.images);
        const videosTable = createSection('Videos', sectionColors.videos);
        const linksTable = createSection('Links', sectionColors.links);

        function addTableHeaders(table, headers) {
            const thead = document.createElement('thead');
            const row = document.createElement('tr');
            headers.forEach(headerText => {
                const th = document.createElement('th');
                th.textContent = DOMPurify.sanitize(headerText);
                th.style.cssText = thStyle;
                th.addEventListener('click', () => sortTable(table, headers.indexOf(headerText)));
                row.appendChild(th);
            });
            thead.appendChild(row);
            table.appendChild(thead);
        }

        addTableHeaders(textTable, ['Title', 'Tag', 'Text']);
        addTableHeaders(imagesTable, ['Preview', 'URL']);
        addTableHeaders(videosTable, ['Preview', 'URL']);
        addTableHeaders(linksTable, ['Text', 'URL']);

        function addTableRow(table, cells, dataCategory) {
            const row = document.createElement('tr');
            const rowData = {};
            cells.forEach((cellData, index) => {
                const td = document.createElement('td');
                td.style.cssText = tdStyle;

                if (cellData instanceof HTMLElement) {
                    const safeElement = cellData.cloneNode(true);
                    if (['IMG', 'VIDEO'].includes(safeElement.tagName)) {
                        const src = DOMPurify.sanitize(safeElement.src, sanitizeConfig);
                        if (src) {
                            safeElement.src = src;
                            rowData[index === 0 ? 'preview' : 'url'] = src;
                            td.appendChild(safeElement);
                        }
                    } else {
                        td.appendChild(safeElement);
                    }
                } else if (table === linksTable) {
                    if (index === 1) {
                        const link = document.createElement('a');
                        const url = DOMPurify.sanitize(cellData, sanitizeConfig);
                        link.href = url;
                        link.target = '_blank';
                        link.rel = 'noopener noreferrer';
                        link.textContent = url;
                        link.style.wordBreak = 'break-all';
                        td.appendChild(link);
                        rowData.url = url;
                    } else {
                        td.textContent = DOMPurify.sanitize(cellData);
                        rowData.text = cellData;
                    }
                } else {
                    td.textContent = DOMPurify.sanitize(cellData);
                    const keys = ['title', 'tag', 'text'];
                    rowData[keys[index]] = cellData;
                }

                row.appendChild(td);
            });
            table.appendChild(row);
            collectedData[dataCategory].push(rowData);
        }

        function traverseDOM(node, title = '') {
            if (node.nodeType === Node.ELEMENT_NODE) {
                if (/^H[1-6]$/i.test(node.tagName)) {
                    title = DOMPurify.sanitize(node.textContent.trim());
                }

                if (node.childNodes.length === 1 && node.childNodes[0].nodeType === Node.TEXT_NODE) {
                    const text = DOMPurify.sanitize(node.textContent.trim());
                    if (text) {
                        addTableRow(textTable, [title, node.tagName, text], 'texts');
                    }
                }

                if (node.tagName === 'IMG' && node.src) {
                    const img = new Image();
                    img.src = DOMPurify.sanitize(node.src, sanitizeConfig);
                    img.style.width = '100px';
                    addTableRow(imagesTable, [img, img.src], 'images');
                }

                if (node.tagName === 'VIDEO' && node.src) {
                    const video = document.createElement('video');
                    video.src = DOMPurify.sanitize(node.src, sanitizeConfig);
                    video.controls = true;
                    video.style.width = '100px';
                    addTableRow(videosTable, [video, video.src], 'videos');
                }

                if (node.tagName === 'A' && node.href) {
                    const text = DOMPurify.sanitize(node.textContent.trim()) || '(no text)';
                    const href = DOMPurify.sanitize(node.href, sanitizeConfig);
                    addTableRow(linksTable, [text, href], 'links');
                }

                Array.from(node.childNodes).forEach(child => traverseDOM(child, title));
            }
        }

        function sortTable(table, columnIndex) {
            const rows = Array.from(table.querySelectorAll('tr:not(:first-child)'));
            rows.sort((a, b) => {
                const aText = a.children[columnIndex].textContent.toLowerCase();
                const bText = b.children[columnIndex].textContent.toLowerCase();
                return aText.localeCompare(bText);
            });
            rows.forEach(row => table.appendChild(row));
        }

        function createSearchBar() {
            const searchContainer = document.createElement('div');
            searchContainer.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 10001;
            `;

            const searchInput = document.createElement('input');
            searchInput.type = 'text';
            searchInput.placeholder = 'Rechercher...';
            searchInput.style.cssText = `
                padding: 10px;
                font-size: 16px;
                border-radius: 5px;
                border: 1px solid #ddd;
            `;

            searchInput.addEventListener('input', () => {
                const searchTerm = searchInput.value.toLowerCase();
                const allRows = container.querySelectorAll('tr:not(:first-child)');
                allRows.forEach(row => {
                    const rowText = Array.from(row.children)
                        .map(cell => cell.textContent.toLowerCase())
                        .join(' ');
                    row.style.display = rowText.includes(searchTerm) ? '' : 'none';
                });
            });

            searchContainer.appendChild(searchInput);
            container.appendChild(searchContainer);
        }

        function createStats() {
            const statsContainer = document.createElement('div');
            statsContainer.style.cssText = `
                position: fixed;
                bottom: 180px;
                right: 20px;
                background: rgba(255, 255, 255, 0.9);
                padding: 15px;
                border-radius: 8px;
                color: #333;
                z-index: 10000;
            `;

            const stats = {
                texts: collectedData.texts.length,
                images: collectedData.images.length,
                videos: collectedData.videos.length,
                links: collectedData.links.length
            };

            statsContainer.innerHTML = `
                <strong>Statistics :</strong><br>
                Texts: ${stats.texts}<br>
                Images: ${stats.images}<br>
                Videos: ${stats.videos}<br>
                Links: ${stats.links}
            `;

            container.appendChild(statsContainer);
        }

        function createCSVButton() {
            const button = document.createElement('button');
            button.textContent = 'Download data in CSV';
            button.style.cssText = `
                position: fixed;
                bottom: 125px;
                right: 20px;
                padding: 10px 20px;
                font-size: 16px;
                background: #6c757d;
                color: white;
                border: none;
                border-radius: 5px;
                cursor: pointer;
                z-index: 10000;
            `;

            button.addEventListener('click', () => {
                const csvContent = Object.entries(collectedData)
                    .map(([category, data]) => {
                        const headers = Object.keys(data[0] || {}).join(',');
                        const rows = data.map(item =>
                            Object.values(item).map(val =>
                                typeof val === 'string' ? `"${val.replace(/"/g, '""')}"` : val
                            ).join(',')
                        ).join('\n');
                        return `${category}\n${headers}\n${rows}`;
                    })
                    .join('\n\n');

                const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
                const url = URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `${window.location.host}_collected_data.csv`;
                a.click();
                setTimeout(() => URL.revokeObjectURL(url), 100);
            });

            container.appendChild(button);
        }

        function createDownloadButton() {
            const button = document.createElement('button');
            button.textContent = 'Download data in JSON';
            button.style.cssText = `
                position: fixed;
                bottom: 70px;
                right: 20px;
                padding: 10px 20px;
                font-size: 16px;
                background: #007bff;
                color: white;
                border: none;
                border-radius: 5px;
                cursor: pointer;
                z-index: 10000;
            `;

            button.addEventListener('click', () => {
                const blob = new Blob([JSON.stringify(collectedData, null, 2)], {
                    type: 'application/json'
                });
                const url = URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = DOMPurify.sanitize(`${window.location.host}_collected_data.json`);
                a.click();
                setTimeout(() => URL.revokeObjectURL(url), 100);
            });

            container.appendChild(button);
        }

        function createDownloadPDFButton() {
            const button = document.createElement('button');
            button.textContent = 'Download data in PDF';
            button.style.cssText = `
                position: fixed;
                bottom: 20px;
                right: 20px;
                padding: 10px 20px;
                font-size: 16px;
                background: #28a745;
                color: white;
                border: none;
                border-radius: 5px;
                cursor: pointer;
                z-index: 10000;
            `;

            button.addEventListener('click', () => {
                const styles = Array.from(document.querySelectorAll('style, link[rel="stylesheet"]'))
                    .map(el => el.outerHTML)
                    .join('');

                const sanitizedContent = DOMPurify.sanitize(container.innerHTML, {
                    ADD_TAGS: ['video', 'source'],
                    ADD_ATTR: ['controls', 'src'],
                    ...sanitizeConfig
                });

                const pdfWindow = window.open('', '_blank');
                pdfWindow.document.write(`
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>${DOMPurify.sanitize(window.location.host)}</title>
                        ${styles}
                        <style>
                            body { 
                                padding: 20px !important;
                                background: #fff !important;
                                color: #000 !important; 
                            }
                            img, video { max-width: 200px !important; }
                        </style>
                    </head>
                    <body>
                        ${sanitizedContent}
                    </body>
                    </html>
                `);
                pdfWindow.document.close();
                pdfWindow.print();
            });

            container.appendChild(button);
        }

        traverseDOM(document.body);

        while (document.body.firstChild) {
            document.body.removeChild(document.body.firstChild);
        }
        document.body.appendChild(container);

        createSearchBar();
        createStats();
        createCSVButton();
        createDownloadButton();
        createDownloadPDFButton();
    }
})();