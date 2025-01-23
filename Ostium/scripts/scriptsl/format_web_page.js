// JS script to reformat a web page with classified tables Icaza Media OOB
(function () {
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
        border-collapse: collapse;
        background: white;
        color: black;
    `;

    const thStyle = `
        background-color: #f4f4f4;
        border: 1px solid #ddd;
        padding: 8px;
        text-align: left;
    `;

    const tdStyle = `
        border: 1px solid #ddd;
        padding: 8px;
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
            background: ${color};
            padding: 10px;
            border-radius: 5px;
            color: #333;
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
    const videosTable = createSection('Videoss', sectionColors.videos);
    const linksTable = createSection('Links', sectionColors.links);

    function addTableHeaders(table, headers) {
        const thead = document.createElement('thead');
        const row = document.createElement('tr');

        headers.forEach(headerText => {
            const th = document.createElement('th');
            th.textContent = headerText;
            th.style.cssText = thStyle;
            row.appendChild(th);
        });

        thead.appendChild(row);
        table.appendChild(thead);
    }

    addTableHeaders(textTable, ['Title', 'Tag', 'Text']);
    addTableHeaders(imagesTable, ['Overview', 'URL']);
    addTableHeaders(videosTable, ['Overview', 'URL']);
    addTableHeaders(linksTable, ['Text', 'URL']);

    function addTableRow(table, cells, dataCategory) {
        const row = document.createElement('tr');
        const rowData = {};

        cells.forEach((cellData, index) => {
            const td = document.createElement('td');
            td.style.cssText = tdStyle;

            if (cellData instanceof HTMLElement) {
                td.appendChild(cellData);
                if (cellData.tagName === 'IMG' || cellData.tagName === 'VIDEO') {
                    rowData[index === 0 ? 'preview' : 'url'] = cellData.src;
                }
            } else if (table === linksTable) {
                const link = document.createElement('a');
                link.href = cellData;
                link.target = '_blank';
                link.textContent = cellData;
                td.appendChild(link);
                rowData['url'] = cellData;
                rowData['text'] = link.textContent;
            } else {
                td.textContent = cellData;
                rowData[index === 0 ? 'title' : index === 1 ? 'tag' : 'text'] = cellData;
            }

            row.appendChild(td);
        });

        table.appendChild(row);
        collectedData[dataCategory].push(rowData);
    }

    function traverseDOM(node, title = '') {
        if (node.nodeType === Node.ELEMENT_NODE) {

            if (/^H[1-6]$/.test(node.tagName)) {
                title = node.textContent.trim();
            }

            if (node.childNodes.length === 1 && node.childNodes[0].nodeType === Node.TEXT_NODE) {
                const text = node.textContent.trim();
                if (text) {
                    addTableRow(textTable, [title, node.tagName, text], 'texts');
                }
            }

            if (node.tagName === 'IMG' && node.src) {
                const img = document.createElement('img');
                img.src = node.src;
                img.style.width = '100px';
                addTableRow(imagesTable, [img, node.src], 'images');
            }

            if (node.tagName === 'VIDEO' && node.src) {
                const video = document.createElement('video');
                video.src = node.src;
                video.controls = true;
                video.style.width = '100px';
                addTableRow(videosTable, [video, node.src], 'videos');
            }

            if (node.tagName === 'A' && node.href) {
                addTableRow(linksTable, [node.textContent.trim() || '(no text)', node.href], 'links');
            }

            Array.from(node.childNodes).forEach(child => traverseDOM(child, title));
        }
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
            const blob = new Blob([JSON.stringify(collectedData, null, 2)], { type: 'application/json' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'collected_data.json';
            a.click();
            URL.revokeObjectURL(url);
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
            const pdfContent = container.innerHTML;
            const pdfWindow = window.open('', '_blank');
            pdfWindow.document.write(`
                    <html>
                        <head>
                            <title>Données collectées</title>
                        </head>
                        <body>
                            ${pdfContent}
                        </body>
                    </html>
                `);
            pdfWindow.document.close();
            pdfWindow.print();
        });

        container.appendChild(button);
    }

    traverseDOM(document.body);

    document.body.innerHTML = '';
    document.body.appendChild(container);

    createDownloadButton();
    createDownloadPDFButton()
})();
