/**
 * @file Extract_Images.js
 * @author ICAZA <github.wildly512@passinbox.com>
 * @version 1.1
 * @description Extract images from a web page.
 */

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
                td.style.textAlign = "center";

                td.appendChild(cellData);
                row.appendChild(td);
            });
            tbody.appendChild(row);
        });
        table.appendChild(tbody);
        return table;
    }

    let images = Array.from(document.querySelectorAll("img"));
    console.log("Images found :", images.length);
    let imageData = images.map(img => {
        let linkElement = document.createElement("a");
        linkElement.href = img.src;
        linkElement.target = "_blank";

        let imgElement = document.createElement("img");
        imgElement.src = img.src;
        imgElement.style.maxWidth = "100px";
        imgElement.style.height = "auto";
        imgElement.style.border = "1px solid black";

        linkElement.appendChild(imgElement);
        return [linkElement];
    });
    let imageTable = createTable(["Images found"], imageData);

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
    container.appendChild(imageTable);
    document.body.appendChild(container);
})();
