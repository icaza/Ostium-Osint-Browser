(function () {
    function createTable(headers, data) {
        let table = document.createElement("table");
        table.style.border = "1px solid black";
        table.style.borderCollapse = "collapse";
        table.style.margin = "10px 0";
        table.style.backgroundColor = "white";

        let thead = document.createElement("thead");
        let headerRow = document.createElement("tr");

        headers.forEach(headerText => {
            let th = document.createElement("th");
            th.textContent = headerText;
            th.style.border = "1px solid black";
            th.style.padding = "5px";
            th.style.backgroundColor = "#ddd";
            headerRow.appendChild(th);
        });
        thead.appendChild(headerRow);
        table.appendChild(thead);

        let tbody = document.createElement("tbody");
        data.forEach(rowData => {
            let row = document.createElement("tr");
            rowData.forEach(cellData => {
                let td = document.createElement("td");
                td.style.border = "1px solid black";
                td.style.padding = "5px";

                if (typeof cellData === "string") {
                    td.textContent = cellData;
                } else {
                    td.appendChild(cellData);
                }
                row.appendChild(td);
            });
            tbody.appendChild(row);
        });
        table.appendChild(tbody);
        return table;
    }

    function getArticleUrl(paragraph) {
        let parent = paragraph.closest("article, section, div");
        if (!parent) return null;

        let link = parent.querySelector("a[href]");
        if (link) return link.href;

        let titleElement = parent.querySelector("h1, h2, h3");
        if (titleElement) {
            let titleLink = titleElement.closest("a") || titleElement.querySelector("a");
            if (titleLink) return titleLink.href;
        }

        return null;
    }

    let paragraphs = Array.from(document.querySelectorAll("p"));
    console.log("Paragraphs found:", paragraphs.length);

    let paragraphData = paragraphs.map(p => {
        let paragraphText = p.textContent.trim();
        let articleUrl = getArticleUrl(p);

        if (articleUrl) {
            let linkElement = document.createElement("a");
            linkElement.href = articleUrl;
            linkElement.textContent = paragraphText;
            linkElement.target = "_blank";
            linkElement.style.textDecoration = "none";
            linkElement.style.color = "inherit";
            return [linkElement];
        } else {
            return [paragraphText];
        }
    });

    let paragraphTable = createTable(["Paragraph text"], paragraphData);

    let links = Array.from(document.querySelectorAll("a"));
    console.log("Links found:", links.length);

    let linkData = links.map(a => {
        let linkElement = document.createElement("a");
        linkElement.href = a.href;
        linkElement.textContent = a.href;
        linkElement.target = "_blank";
        return [linkElement];
    });

    let linkTable = createTable(["Links found"], linkData);

    let container = document.createElement("div");
    container.style.position = "fixed";
    container.style.top = "10px";
    container.style.right = "10px";
    container.style.width = "400px";
    container.style.maxHeight = "80vh";
    container.style.overflowY = "auto";
    container.style.backgroundColor = "white";
    container.style.border = "2px solid black";
    container.style.padding = "10px";
    container.style.zIndex = "99999";

    let closeButton = document.createElement("button");
    closeButton.textContent = "Close";
    closeButton.style.display = "block";
    closeButton.style.marginBottom = "10px";
    closeButton.onclick = function () {
        container.remove();
    };

    container.appendChild(closeButton);
    container.appendChild(paragraphTable);
    container.appendChild(linkTable);

    document.body.appendChild(container);
})();
