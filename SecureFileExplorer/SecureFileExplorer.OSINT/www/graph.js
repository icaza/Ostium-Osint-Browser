fetch("/api/graph").then(r => r.json()).then(g => {
    const svg = document.querySelector("svg");
    g.links.forEach(l => {/* draw lines */ });
    g.nodes.forEach(n => {/* draw nodes */ });
});
