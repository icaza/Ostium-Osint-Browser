let openedFiles = [];

const searchInput = document.getElementById("search");
const resultsDiv = document.getElementById("results");
const timelineDiv = document.getElementById("timeline");
const graphSvg = document.getElementById("graph");

let graphNodes = [];
let graphLinks = [];
let currentLayout = 'force';
let selectedExtensions = new Set();
let zoom = 1;
let panX = 0;
let panY = 0;
let animationId = null;

searchInput.addEventListener("input", async () => {
    const q = encodeURIComponent(searchInput.value);
    try {
        const res = await fetch(`/api/search?q=${q}`);
        if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
        const data = await res.json();
        renderResults(data);
    } catch (error) {
        console.error("Search error:", error);
        resultsDiv.innerHTML = `<div class="error">Error loading search results</div>`;
    }
});

function renderResults(files) {
    if (files.length === 0) {
        resultsDiv.innerHTML = '<div class="no-results">No files found</div>';
        return;
    }

    resultsDiv.innerHTML = files.map(f => `
        <div class="item ${openedFiles.includes(f.Sha256) ? 'opened' : ''}" 
             onclick="openFile('${escapeHtml(f.Path.replace(/\\/g, '/')).replace(/'/g, "\\'")}', '${f.Sha256}', '${escapeHtml(f.Name).replace(/'/g, "\\'")}')">
            <b>${escapeHtml(f.Name)}</b>
            <small>${f.Extension} | ${f.Sha256.slice(0, 16)}...</small>
            <div class="meta">Size: ${formatBytes(f.Size)} | Modified: ${new Date(f.Modified).toLocaleString()}</div>
        </div>
    `).join("");
}

function openFile(path, sha, fileName) {
    // Normalize path to use forward slashes
    path = path.replace(/\\/g, '/');
    const ext = (fileName || path).split('.').pop().toLowerCase();

    console.log('Opening file:', fileName, 'Path:', path, 'Extension:', ext);

    const directOpenExtensions = [
        // Documents
        '.pdf', '.docx', '.doc', '.txt', '.csv', '.rtf',
        '.xlsx', '.xls', '.pptx', '.ppt',

        // Images
        '.jpg', '.jpeg', '.png', '.gif', '.bmp', '.svg', '.ico',

        // Web files
        '.html', '.htm', '.css', '.js', '.json', '.xml', '.md',

        // Archives
        '.zip', '.rar', '.7z',

        // Email
        '.eml', '.msg',

        // Code files (read-only viewing)
        '.py', '.java', '.cpp', '.c', '.h', '.cs', '.php', '.sql',
        '.sh', '.bat', '.ps1', '.vbs',

        // Config files
        '.yml', '.yaml', '.ini', '.cfg', '.log',

        // Fonts
        '.woff', '.woff2', '.ttf'];

    // Encode the path properly for URL usage
    const encodedPath = encodeURIComponent(path);

    if (directOpenExtensions.includes(ext)) {
        console.log('Opening directly:', `/files/${encodedPath}`);
        window.open(`/files/${encodedPath}`, "_blank");
    } else {
        const viewerUrl = `/viewer.html?file=${encodedPath}&name=${encodeURIComponent(fileName || path)}`;
        console.log('Opening in viewer:', viewerUrl);
        window.open(viewerUrl, "_blank");
    }

    if (sha && !openedFiles.includes(sha)) {
        openedFiles.push(sha);
        const items = document.querySelectorAll('.item');
        items.forEach(item => {
            const onclickStr = item.getAttribute('onclick') || '';
            if (onclickStr.includes(sha)) {
                item.classList.add('opened');
            }
        });
    }
}

async function loadTimeline() {
    try {
        const res = await fetch("/api/timeline");
        if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
        const data = await res.json();

        if (data.length === 0) {
            timelineDiv.innerHTML = '<div class="no-results">No files in timeline</div>';
            return;
        }

        timelineDiv.innerHTML = data.map(f => `
            <div class="timeline-item" onclick="openFile('${escapeHtml(f.Path.replace(/\\/g, '/')).replace(/'/g, "\\'")}', '', '${escapeHtml(f.Name).replace(/'/g, "\\'")}')">
                <span class="date">${new Date(f.Created).toLocaleString()}</span>
                <span class="name">${escapeHtml(f.Name)}</span>
            </div>
        `).join("");
    } catch (error) {
        console.error("Timeline error:", error);
        timelineDiv.innerHTML = '<div class="error">Error loading timeline</div>';
    }
}

async function loadGraph() {
    try {
        const res = await fetch("/api/graph");
        if (!res.ok) throw new Error(`HTTP error! status: ${res.status} `);
        const data = await res.json();

        if (data.nodes.length === 0) {
            graphSvg.innerHTML = '<text x="400" y="200" text-anchor="middle" fill="#888">No data to display</text>';
            return;
        }

        graphNodes = data.nodes.map(n => ({ ...n }));
        graphLinks = data.links.map(l => ({ ...l }));

        createGraphControls();

        const extensions = [...new Set(graphNodes.map(n => n.ext))];
        selectedExtensions = new Set(extensions);

        createExtensionFilter(extensions);

        renderGraph();

    } catch (error) {
        console.error("Graph error:", error);
        graphSvg.innerHTML = '<text x="400" y="200" text-anchor="middle" fill="#f44">Error loading graph</text>';
    }
}

function createGraphControls() {
    console.log('Graph controls already in HTML');
}

function createExtensionFilter(extensions) {
    const chipsContainer = document.getElementById('extension-chips');
    if (!chipsContainer) {
        console.error('extension-chips container not found');
        return;
    }

    chipsContainer.innerHTML = extensions.map(ext => `
        <label class="extension-chip" style="background: ${getColorByExtension(ext)}22; border: 1px solid ${getColorByExtension(ext)};">
            <input type="checkbox" value="${ext}" checked onchange="filterByExtension()">
            <span style="color: ${getColorByExtension(ext)};">${ext}</span>
        </label>
    `).join('');
}

function filterByExtension() {
    const checkboxes = document.querySelectorAll('#extension-filter input[type="checkbox"]');
    selectedExtensions.clear();
    checkboxes.forEach(cb => {
        if (cb.checked) selectedExtensions.add(cb.value);
    });
    renderGraph();
}

function changeLayout(layout) {
    currentLayout = layout;
    renderGraph();
}

function resetLayout() {
    zoom = 1;
    panX = 0;
    panY = 0;
    renderGraph();
}

function zoomIn() {
    zoom = Math.min(zoom * 1.2, 5);
    updateTransform();
}

function zoomOut() {
    zoom = Math.max(zoom / 1.2, 0.2);
    updateTransform();
}

function resetZoom() {
    zoom = 1;
    panX = 0;
    panY = 0;
    updateTransform();
}

function toggleGroups(show) {
    const groups = document.querySelectorAll('.extension-group');
    groups.forEach(g => g.style.display = show ? 'block' : 'none');
}

function updateTransform() {
    const mainGroup = document.getElementById('main-graph-group');
    if (mainGroup) {
        mainGroup.setAttribute('transform', `translate(${panX}, ${panY}) scale(${zoom})`);
    }
}

function renderGraph() {
    const width = 800;
    const height = 400;

    const nodes = graphNodes
        .filter(n => selectedExtensions.has(n.ext))
        .map((n, i) => ({
            ...n,
            x: n.x || 0,
            y: n.y || 0,
            vx: 0,
            vy: 0,
            index: i
        }));

    const links = graphLinks
        .map(l => ({
            source: nodes.find(n => n.id === l.source),
            target: nodes.find(n => n.id === l.target)
        }))
        .filter(l => l.source && l.target);

    applyLayout(nodes, links, width, height);

    graphSvg.innerHTML = "";

    const mainGroup = document.createElementNS("http://www.w3.org/2000/svg", "g");
    mainGroup.id = 'main-graph-group';
    mainGroup.setAttribute('transform', `translate(${panX}, ${panY}) scale(${zoom})`);
    graphSvg.appendChild(mainGroup);

    const showGroups = document.getElementById('show-groups')?.checked !== false;
    if (showGroups) {
        createExtensionGroups(mainGroup, nodes);
    }

    const linkGroup = document.createElementNS("http://www.w3.org/2000/svg", "g");
    mainGroup.appendChild(linkGroup);

    const linkElems = links.map(l => {
        const line = document.createElementNS("http://www.w3.org/2000/svg", "line");
        line.setAttribute("stroke", "#444");
        line.setAttribute("stroke-width", "1.5");
        line.setAttribute("x1", l.source.x);
        line.setAttribute("y1", l.source.y);
        line.setAttribute("x2", l.target.x);
        line.setAttribute("y2", l.target.y);
        linkGroup.appendChild(line);
        return line;
    });

    const nodeGroup = document.createElementNS("http://www.w3.org/2000/svg", "g");
    mainGroup.appendChild(nodeGroup);

    const nodeElems = nodes.map(n => {
        const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
        group.style.cursor = "grab";
        group.setAttribute("transform", `translate(${n.x}, ${n.y})`);

        const circle = document.createElementNS("http://www.w3.org/2000/svg", "circle");
        circle.setAttribute("r", 8);
        circle.setAttribute("fill", getColorByExtension(n.ext));
        circle.setAttribute("stroke", "#fff");
        circle.setAttribute("stroke-width", "2");

        const text = document.createElementNS("http://www.w3.org/2000/svg", "text");
        text.textContent = n.label.length > 15 ? n.label.substring(0, 12) + "..." : n.label;
        text.setAttribute("font-size", "10");
        text.setAttribute("fill", "#eee");
        text.setAttribute("text-anchor", "middle");
        text.setAttribute("y", "20");

        group.appendChild(circle);
        group.appendChild(text);

        group.addEventListener("dblclick", (e) => {
            e.stopPropagation();
            openFile(n.path.replace(/\\/g, '/'), n.id, n.label);
        });

        const title = document.createElementNS("http://www.w3.org/2000/svg", "title");
        title.textContent = `${n.label} (${n.ext}) \nDouble - click to open`;
        group.appendChild(title);

        let drag = false;
        group.addEventListener("mousedown", e => {
            if (e.button !== 0) return;
            drag = true;
            group.style.cursor = "grabbing";
            e.stopPropagation();
            e.preventDefault();
        });

        graphSvg.addEventListener("mousemove", e => {
            if (!drag) return;
            const rect = graphSvg.getBoundingClientRect();
            const x = (e.clientX - rect.left - panX) / zoom;
            const y = (e.clientY - rect.top - panY) / zoom;
            n.x = x;
            n.y = y;
            n.vx = 0;
            n.vy = 0;
            updatePositions(nodes, links, nodeElems, linkElems);
        });

        graphSvg.addEventListener("mouseup", () => {
            if (drag) {
                drag = false;
                group.style.cursor = "grab";
            }
        });

        nodeGroup.appendChild(group);
        return group;
    });

    graphSvg.addEventListener("wheel", (e) => {
        e.preventDefault();
        if (e.ctrlKey) {
            const delta = e.deltaY > 0 ? 0.9 : 1.1;
            zoom = Math.max(0.2, Math.min(5, zoom * delta));
        } else {
            panX -= e.deltaX;
            panY -= e.deltaY;
        }
        updateTransform();
    });

    if (currentLayout === 'force') {
        if (animationId) cancelAnimationFrame(animationId);
        let frameCount = 0;
        function animate() {
            if (frameCount < 300) {
                applyForces(nodes, links);
                updatePositions(nodes, links, nodeElems, linkElems);
                frameCount++;
                animationId = requestAnimationFrame(animate);
            }
        }
        animate();
    }
}

function createExtensionGroups(parent, nodes) {
    const groups = {};
    nodes.forEach(n => {
        if (!groups[n.ext]) groups[n.ext] = [];
        groups[n.ext].push(n);
    });

    Object.entries(groups).forEach(([ext, groupNodes]) => {
        if (groupNodes.length < 2) return;

        const xs = groupNodes.map(n => n.x);
        const ys = groupNodes.map(n => n.y);
        const minX = Math.min(...xs) - 20;
        const minY = Math.min(...ys) - 20;
        const maxX = Math.max(...xs) + 20;
        const maxY = Math.max(...ys) + 20;

        const rect = document.createElementNS("http://www.w3.org/2000/svg", "rect");
        rect.setAttribute("class", "extension-group");
        rect.setAttribute("x", minX);
        rect.setAttribute("y", minY);
        rect.setAttribute("width", maxX - minX);
        rect.setAttribute("height", maxY - minY);
        rect.setAttribute("fill", getColorByExtension(ext) + "15");
        rect.setAttribute("stroke", getColorByExtension(ext));
        rect.setAttribute("stroke-width", "2");
        rect.setAttribute("stroke-dasharray", "5,5");
        rect.setAttribute("rx", "10");

        parent.insertBefore(rect, parent.firstChild);

        const label = document.createElementNS("http://www.w3.org/2000/svg", "text");
        label.setAttribute("class", "extension-group");
        label.textContent = `${ext} (${groupNodes.length})`;
        label.setAttribute("x", minX + 5);
        label.setAttribute("y", minY + 15);
        label.setAttribute("fill", getColorByExtension(ext));
        label.setAttribute("font-size", "12");
        label.setAttribute("font-weight", "600");
        parent.appendChild(label);
    });
}

function applyLayout(nodes, links, width, height) {
    switch (currentLayout) {
        case 'circular':
            applyCircularLayout(nodes, width, height);
            break;
        case 'hierarchical':
            applyHierarchicalLayout(nodes, links, width, height);
            break;
        case 'grid':
            applyGridLayout(nodes, width, height);
            break;
        default:
            applyForceLayout(nodes, width, height);
    }
}

function applyForceLayout(nodes, width, height) {
    nodes.forEach(n => {
        if (!n.x || !n.y) {
            n.x = Math.random() * (width - 40) + 20;
            n.y = Math.random() * (height - 40) + 20;
        }
    });
}

function applyCircularLayout(nodes, width, height) {
    const radius = Math.min(width, height) / 2.5;
    const centerX = width / 2;
    const centerY = height / 2;
    const angleStep = (2 * Math.PI) / nodes.length;

    nodes.forEach((n, i) => {
        n.x = centerX + radius * Math.cos(i * angleStep);
        n.y = centerY + radius * Math.sin(i * angleStep);
    });
}

function applyHierarchicalLayout(nodes, links, width, height) {
    const levels = new Map();
    const visited = new Set();

    function assignLevel(nodeId, level) {
        if (visited.has(nodeId)) return;
        visited.add(nodeId);

        if (!levels.has(level)) levels.set(level, []);
        levels.get(level).push(nodes.find(n => n.id === nodeId));

        links.filter(l => l.source.id === nodeId).forEach(l => {
            assignLevel(l.target.id, level + 1);
        });
    }

    if (nodes.length > 0) {
        assignLevel(nodes[0].id, 0);
    }

    nodes.filter(n => !visited.has(n.id)).forEach((n, i) => {
        const level = Math.max(...levels.keys()) + 1;
        if (!levels.has(level)) levels.set(level, []);
        levels.get(level).push(n);
    });

    const levelHeight = height / (levels.size || 1);
    levels.forEach((levelNodes, level) => {
        const levelWidth = width / (levelNodes.length + 1);
        levelNodes.forEach((n, i) => {
            n.x = levelWidth * (i + 1);
            n.y = levelHeight * (level + 0.5);
        });
    });
}

function applyGridLayout(nodes, width, height) {
    const cols = Math.ceil(Math.sqrt(nodes.length));
    const cellWidth = width / cols;
    const cellHeight = height / Math.ceil(nodes.length / cols);

    nodes.forEach((n, i) => {
        const col = i % cols;
        const row = Math.floor(i / cols);
        n.x = cellWidth * (col + 0.5);
        n.y = cellHeight * (row + 0.5);
    });
}

function applyForces(nodes, links) {
    links.forEach(l => {
        const dx = l.target.x - l.source.x;
        const dy = l.target.y - l.source.y;
        const dist = Math.sqrt(dx * dx + dy * dy) || 1;
        const force = (dist - 100) * 0.01;
        const fx = force * dx / dist;
        const fy = force * dy / dist;

        l.source.vx += fx;
        l.source.vy += fy;
        l.target.vx -= fx;
        l.target.vy -= fy;
    });

    for (let i = 0; i < nodes.length; i++) {
        for (let j = i + 1; j < nodes.length; j++) {
            const dx = nodes[j].x - nodes[i].x;
            const dy = nodes[j].y - nodes[i].y;
            const dist = Math.sqrt(dx * dx + dy * dy) || 1;

            if (dist < 50) {
                const force = (50 - dist) * 0.05;
                const fx = force * dx / dist;
                const fy = force * dy / dist;

                nodes[i].vx -= fx;
                nodes[i].vy -= fy;
                nodes[j].vx += fx;
                nodes[j].vy += fy;
            }
        }
    }

    nodes.forEach(n => {
        n.vx *= 0.85;
        n.vy *= 0.85;
        n.x += n.vx;
        n.y += n.vy;
        n.x = Math.max(20, Math.min(780, n.x));
        n.y = Math.max(20, Math.min(380, n.y));
    });
}

function updatePositions(nodes, links, nodeElems, linkElems) {
    links.forEach((l, i) => {
        if (linkElems[i]) {
            linkElems[i].setAttribute("x1", l.source.x);
            linkElems[i].setAttribute("y1", l.source.y);
            linkElems[i].setAttribute("x2", l.target.x);
            linkElems[i].setAttribute("y2", l.target.y);
        }
    });

    nodes.forEach((n, i) => {
        if (nodeElems[i]) {
            nodeElems[i].setAttribute("transform", `translate(${n.x}, ${n.y})`);
        }
    });
}

function exportGraphSVG() {
    const svgData = graphSvg.outerHTML;
    const blob = new Blob([svgData], { type: 'image/svg+xml' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `graph_${Date.now()}.svg`;
    a.click();
    URL.revokeObjectURL(url);
}

function exportGraphPNG() {
    const svgData = new XMLSerializer().serializeToString(graphSvg);
    const canvas = document.createElement('canvas');
    canvas.width = 800;
    canvas.height = 400;
    const ctx = canvas.getContext('2d');

    const img = new Image();
    const blob = new Blob([svgData], { type: 'image/svg+xml' });
    const url = URL.createObjectURL(blob);

    img.onload = () => {
        ctx.fillStyle = '#111421';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(img, 0, 0);

        canvas.toBlob(blob => {
            const pngUrl = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = pngUrl;
            a.download = `graph_${Date.now()}.png`;
            a.click();
            URL.revokeObjectURL(pngUrl);
            URL.revokeObjectURL(url);
        });
    };

    img.src = url;
}

// --- Export ---
function exportData(format) {
    window.open(`/ api /export?f = ${format} `, "_blank");
}

// --- Utilities ---
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function formatBytes(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
}

function getColorByExtension(ext) {
    const colors = {
        '.pdf': '#e74c3c',
        '.docx': '#3498db',
        ".doc": '#3498db',
        '.txt': '#95a5a6',
        '.csv': '#2ecc71',
        ".xlsx": '#ffffffff',
        ".xls": '#ffffffff',
        ".pptx": '#252d75ff',
        ".ppt": '#252d75ff',
        '.jpg': '#f39c12',
        '.jpeg': '#f39c12',
        '.png': '#9b59b6',
        ".gif": '#c70c8fff',
        ".bmp": '#0c8c91ff',
        '.zip': '#34495e',
        ".rar": '#ff0000ff',
        ".7z": '#ff0000ff',
        '.json': '#1abc9c',
        '.xml': '#21077eff',
        ".html": '#08bbd3ff',
        ".htm": '#08bbd3ff',
        ".md": '#a7f7bbff',
        ".rtf": '#3498db',
        ".eml": '#263358ff',
        ".msg": '#4a5205ff',
        ".py": '#db4451ff',
        ".java": '#044d1aff',
        ".cpp": '#e26613ff',
        ".c": '#e26613ff',
        ".h": '#fffb00ff',
        ".cs": '#580202ff',
        ".php": '#3d042fff',
        ".sql": '#7bfc85ff',
        ".sh": '#962151ff',
        ".bat": '#5900ffff',
        ".ps1": '#213530ff',
        ".vbs": '#ffffffff',
        ".yml": '#7bfc85ff',
        ".yaml": '#962151ff',
        ".ini": '#a7f7bbff',
        ".cfg": '#a7f7bbff',
        ".log": '#a7f7bbff',
        ".svg": '#9b59b6'
    };
    return colors[ext] || '#0af';
}

// --- Initialization ---
loadTimeline();
loadGraph();
searchInput.dispatchEvent(new Event('input'));