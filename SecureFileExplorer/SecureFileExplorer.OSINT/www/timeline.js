fetch("/api/timeline").then(r => r.json()).then(data => {
    const root = document.getElementById("timeline");
    data.forEach(f => {
        const d = document.createElement("div");
        d.textContent = `${f.created} - ${f.name}`;
        root.appendChild(d);
    });
});
