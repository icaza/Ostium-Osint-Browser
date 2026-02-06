// ==================== STATE MANAGEMENT ====================
const state = {
    config: null, // Will hold the UI configuration
    currentProject: null,
    currentEntity: null,
    projects: [],
    entities: [],
    attributes: [],
    relationships: [],
    network: null,
    currentView: 'graph',
    editingProjectId: null
};

// ==================== INITIALIZATION ====================
document.addEventListener('DOMContentLoaded', () => {
    initializeApp();
});

async function initializeApp() {
    try {
        // Load configuration first
        await loadConfig();

        // Load projects
        await loadProjects();

        // Setup event listeners
        setupEventListeners();

        // Initialize network graph
        initializeNetwork();

        console.log('✓ Messis OSINT Tool initialized');
    } catch (error) {
        console.error('Initialization error:', error);
        showNotification('Error during initialization', 'error');
    }
}

async function loadConfig() {
    try {
        const response = await fetch('/api/config');
        state.config = await response.json();

        // Apply theme
        if (state.config && state.config.theme === 'light') {
            document.body.classList.add('light-theme');
        } else {
            document.body.classList.remove('light-theme');
        }
    } catch (error) {
        console.error('Error loading config:', error);
        // Fallback or critical error? Let's use defaults if fails or just log
        showNotification('Configuration loading error', 'error');
    }
}

// ==================== EVENT LISTENERS ====================
function setupEventListeners() {
    // Project buttons
    document.getElementById('newProjectBtn').addEventListener('click', () => openProjectModal());
    document.getElementById('startProjectBtn').addEventListener('click', () => openProjectModal());
    document.getElementById('refreshProjectsBtn').addEventListener('click', () => loadProjects());
    document.getElementById('saveProjectBtn').addEventListener('click', () => saveProject());
    document.getElementById('editProjectBtn').addEventListener('click', () => editCurrentProject());
    document.getElementById('backToProjectsBtn').addEventListener('click', () => backToProjects());

    // Entity buttons
    document.getElementById('addEntityBtn').addEventListener('click', () => openEntityModal());
    document.getElementById('saveEntityBtn').addEventListener('click', () => saveEntity());
    document.getElementById('deleteEntityBtn').addEventListener('click', () => deleteEntity());

    // Single event listener for addAttributeBtn
    document.getElementById('addAttributeBtn').addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        addAttributeField();
    });

    // View toggle
    document.querySelectorAll('.toggle-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const view = e.currentTarget.dataset.view;
            switchView(view);
        });
    });

    // Graph controls
    document.getElementById('fitGraphBtn').addEventListener('click', () => fitGraph());
    document.getElementById('zoomInBtn').addEventListener('click', () => zoomGraph(1.2));
    document.getElementById('zoomOutBtn').addEventListener('click', () => zoomGraph(0.8));
    document.getElementById('resetGraphBtn').addEventListener('click', () => resetGraph());
    document.getElementById('layoutSelect').addEventListener('change', (e) => updateLayout(e.target.value));
    document.getElementById('toggleLegendBtn').addEventListener('click', () => toggleLegend());

    // Légende
    const legend = document.getElementById('graphLegend');
    const legendCloseBtn = legend.querySelector('.btn-legend-close');
    legendCloseBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        legend.classList.add('hidden');
    });

    // Export dropdown
    setupExportDropdown();

    // Modal close buttons
    document.querySelectorAll('[data-modal]').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const modalId = e.currentTarget.dataset.modal;
            closeModal(modalId);
        });
    });

    // Close modals on outside click
    document.querySelectorAll('.modal').forEach(modal => {
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                closeModal(modal.id);
            }
        });
    });

    // Relations panel search
    const searchRelationsInput = document.getElementById('searchRelations');
    if (searchRelationsInput) {
        searchRelationsInput.addEventListener('input', (e) => {
            const searchTerm = e.target.value.toLowerCase();
            filterRelationsTable(searchTerm);
        });
    }
}

function toggleLegend() {
    const legend = document.getElementById('graphLegend');
    legend.classList.toggle('hidden');
}

function setupExportDropdown() {
    const dropdown = document.querySelector('.dropdown');
    const toggle = dropdown.querySelector('.dropdown-toggle');
    const items = dropdown.querySelectorAll('.dropdown-item');

    toggle.addEventListener('click', (e) => {
        e.stopPropagation();
        dropdown.classList.toggle('active');
    });

    items.forEach(item => {
        item.addEventListener('click', async (e) => {
            e.stopPropagation();
            const format = e.currentTarget.dataset.export;
            await exportProject(format);
            dropdown.classList.remove('active');
        });
    });

    document.addEventListener('click', () => {
        dropdown.classList.remove('active');
    });
}

function filterRelationsTable(searchTerm) {
    const tbody = document.querySelector('#relationsTable tbody');
    if (!tbody) return;

    const rows = tbody.querySelectorAll('tr');
    let hasVisibleRows = false;

    rows.forEach(row => {
        if (row.classList.contains('no-data-row')) {
            // Handling the "no data" message
            row.style.display = searchTerm ? 'none' : '';
            return;
        }

        const cells = row.querySelectorAll('td');
        let shouldShow = false;

        if (searchTerm.trim() === '') {
            shouldShow = true;
        } else {
            // Check the text content of each relevant cell
            cells.forEach((cell, index) => {
                if (index < 3) { // Only the first 3 columns (Source, Relationship, Target)
                    const text = cell.textContent.toLowerCase();
                    if (text.includes(searchTerm)) {
                        shouldShow = true;
                    }
                }
            });
        }

        row.style.display = shouldShow ? '' : 'none';
        if (shouldShow) hasVisibleRows = true;
    });

    // Display a message if no results are found.
    let noResultsRow = tbody.querySelector('.no-results-row');
    
    if (!hasVisibleRows && searchTerm) {
        if (!noResultsRow) {
            noResultsRow = document.createElement('tr');
            noResultsRow.className = 'no-results-row';

            const td = document.createElement('td');
            td.colSpan = 5;
            td.className = 'text-center';

            const container = document.createElement('div');
            container.className = 'no-data-message';

            const icon = document.createElement('i');
            icon.className = 'fas fa-search';

            const message = document.createElement('p');
            message.textContent = 'No relationship matches "' + searchTerm + '"';

            container.appendChild(icon);
            container.appendChild(message);
            td.appendChild(container);
            noResultsRow.appendChild(td);

            tbody.appendChild(noResultsRow);
        }
        noResultsRow.style.display = '';
    } else if (noResultsRow) {
        noResultsRow.style.display = 'none';
    }
}

// ==================== PROJECTS ====================
async function loadProjects() {
    try {
        const response = await fetch('/api/projects');
        state.projects = await response.json();
        renderProjects();
    } catch (error) {
        console.error('Error loading projects:', error);
        showNotification('Error loading projects', 'error');
    }
}

function renderProjects() {
    const container = document.getElementById('projectsList');

    if (state.projects.length === 0) {
        container.innerHTML = `
            <div class="loading">
                <i class="fas fa-folder-open"></i>
                <p>No project</p>
            </div>
        `;
        return;
    }

    container.innerHTML = state.projects.map(project => `
        <div class="project-item" data-id="${project.id}">
            <div class="project-header">
                <h3>
                    <i class="fas fa-folder"></i>
                    ${escapeHtml(project.name)}
                </h3>
                <button class="btn-icon btn-delete-project" data-id="${project.id}" title="Delete the project">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
            <p>${escapeHtml(project.description || 'No description')}</p>
            <div class="project-meta">
                <span><i class="fas fa-calendar"></i> ${formatDate(project.created_at)}</span>
                <span><i class="fas fa-edit"></i> ${formatDate(project.updated_at)}</span>
            </div>
        </div>
    `).join('');

    // Add click listeners
    container.querySelectorAll('.project-item').forEach(item => {
        const projectId = parseInt(item.dataset.id);

        item.addEventListener('click', (e) => {
            if (!e.target.closest('.btn-delete-project')) {
                openProject(projectId);
            }
        });
    });

    container.querySelectorAll('.btn-delete-project').forEach(btn => {
        btn.addEventListener('click', async (e) => {
            e.stopPropagation(); // Prevent the project from opening
            const projectId = parseInt(btn.dataset.id);
            await deleteProject(projectId);
        });
    });
}

async function openProject(id) {
    try {
        state.currentProject = state.projects.find(p => p.id === id);

        if (!state.currentProject) {
            showNotification('Project not found', 'error');
            return;
        }

        // Load project data
        await loadProjectData(id);

        // Update UI
        document.getElementById('welcomeScreen').classList.add('hidden');
        document.getElementById('projectView').classList.remove('hidden');
        document.getElementById('projectTitle').textContent = state.currentProject.name;

        // Update active state in sidebar
        document.querySelectorAll('.project-item').forEach(item => {
            item.classList.toggle('active', parseInt(item.dataset.id) === id);
        });

        // Render data
        renderGraph();
        renderDataView();

    } catch (error) {
        console.error('Error opening project:', error);
        showNotification('Error opening the project', 'error');
    }
}

async function loadProjectData(projectId) {
    try {
        // Load in parallel to optimize
        const [entitiesResponse, relsResponse] = await Promise.all([
            fetch(`/api/projects/${projectId}/entities`),
            fetch(`/api/projects/${projectId}/relationships`)
        ]);

        state.entities = await entitiesResponse.json();
        state.relationships = await relsResponse.json();

        // Load attributes in batches to optimize
        state.attributes = [];
        const batchSize = 5;

        for (let i = 0; i < state.entities.length; i += batchSize) {
            const batch = state.entities.slice(i, i + batchSize);
            const batchPromises = batch.map(async (entity) => {
                const attrResponse = await fetch(`/api/entities/${entity.id}/attributes`);
                const attrs = await attrResponse.json();
                return { entityId: entity.id, attrs };
            });

            const batchResults = await Promise.all(batchPromises);
            batchResults.forEach(({ entityId, attrs }) => {
                attrs.forEach(attr => {
                    attr.entity_id = entityId;
                    state.attributes.push(attr);
                });
            });
        }

    } catch (error) {
        console.error('Error loading project data:', error);
        throw error;
    }
}

function openProjectModal(project = null) {
    const modal = document.getElementById('projectModal');
    const title = document.getElementById('projectModalTitle');
    const nameInput = document.getElementById('projectName');
    const descInput = document.getElementById('projectDescription');

    if (project) {
        title.innerHTML = '<i class="fas fa-edit"></i> Modify the Project';
        nameInput.value = project.name;
        descInput.value = project.description || '';
        state.editingProjectId = project.id;
    } else {
        title.innerHTML = '<i class="fas fa-folder-plus"></i> New Project';
        nameInput.value = '';
        descInput.value = '';
        state.editingProjectId = null;
    }

    modal.classList.add('active');
    nameInput.focus();
}

async function saveProject() {
    const name = document.getElementById('projectName').value.trim();
    const description = document.getElementById('projectDescription').value.trim();

    if (!name) {
        showNotification('The project name is required', 'error');
        return;
    }

    try {
        let response;
        if (state.editingProjectId) {
            // Update
            response = await fetch(`/api/projects/${state.editingProjectId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name, description })
            });
        } else {
            // Create
            response = await fetch('/api/projects', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name, description })
            });
        }

        const result = await response.json();

        if (response.ok) {
            closeModal('projectModal');
            await loadProjects();

            if (!state.editingProjectId) {
                // Open newly created project
                openProject(result.id);
            } else {
                // Update current project if editing
                if (state.currentProject && state.currentProject.id === state.editingProjectId) {
                    state.currentProject.name = name;
                    state.currentProject.description = description;
                    document.getElementById('projectTitle').textContent = name;
                }
            }

            showNotification('Project successfully registered', 'success');
        } else {
            showNotification('Error during registration', 'error');
        }
    } catch (error) {
        console.error('Error saving project:', error);
        showNotification('Error during registration', 'error');
    }
}

function editCurrentProject() {
    if (state.currentProject) {
        openProjectModal(state.currentProject);
    }
}

function backToProjects() {
    state.currentProject = null;
    state.currentEntity = null;
    state.entities = [];
    state.attributes = [];
    state.relationships = [];

    // Reset the network if active
    if (state.network) {
        state.network.setData({ nodes: [], edges: [] });
    }

    document.getElementById('projectView').classList.add('hidden');
    document.getElementById('welcomeScreen').classList.remove('hidden');

    loadProjects();
}

async function deleteProject(projectId) {
    const project = state.projects.find(p => p.id === projectId);
    if (!project) return;

    const confirmMessage = `Are you sure you want to delete the project? "${project.name}" ?\n\nThis action will also remove :\n• All entities\n• Tous les attributs\n• All relationships\n\nThis action is irreversible.`;

    if (!confirm(confirmMessage)) {
        return;
    }

    try {
        const response = await fetch(`/api/projects/${projectId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            if (state.currentProject && state.currentProject.id === projectId) {
                backToProjects();
            }

            await loadProjects();

            showNotification('Project successfully deleted', 'success');
        } else {
            showNotification('Error deleting project', 'error');
        }
    } catch (error) {
        console.error('Error deleting project:', error);
        showNotification('Error during deletion', 'error');
    }
}

// ==================== ENTITIES ====================
function openEntityModal(entity = null) {
    const modal = document.getElementById('entityModal');
    const title = document.getElementById('entityModalTitle');
    const typeInput = document.getElementById('entityType');
    const nameInput = document.getElementById('entityName');
    const deleteBtn = document.getElementById('deleteEntityBtn');

    // Clear attributes list
    const attributesList = document.getElementById('attributesList');
    attributesList.innerHTML = '';

    // Reset no attributes message
    updateNoAttributesMessage();

    if (entity) {
        title.innerHTML = '<i class="fas fa-user-edit"></i> Modify the Entity';
        typeInput.value = entity.type;
        nameInput.value = entity.name;
        state.currentEntity = entity;
        deleteBtn.style.display = 'block';

        // Load attributes
        const entityAttrs = state.attributes.filter(a => a.entity_id === entity.id);
        entityAttrs.forEach(attr => {
            addAttributeField(attr);
        });
    } else {
        title.innerHTML = '<i class="fas fa-user-plus"></i> New Entity';
        typeInput.value = 'Person';
        nameInput.value = '';
        state.currentEntity = null;
        deleteBtn.style.display = 'none';

        // Add one empty attribute field
        addAttributeField();
    }

    modal.classList.add('active');

    setTimeout(() => {
        nameInput.focus();
    }, 100);
}

function addAttributeField(attribute = null) {
    const template = document.getElementById('attributeTemplate');
    const clone = template.content.cloneNode(true);
    const item = clone.querySelector('.attribute-item');

    // By default, the notes are folded.
    item.dataset.collapsed = "true";

    if (attribute) {
        item.dataset.id = attribute.id;
        item.querySelector('.attr-category').value = attribute.category;
        item.querySelector('.attr-label').value = attribute.label;
        item.querySelector('.attr-value').value = attribute.value;
        item.querySelector('.attr-notes').value = attribute.notes || '';
    }

    // Collapse toggle
    const collapseBtn = item.querySelector('.btn-collapse');
    collapseBtn.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        const isCollapsed = item.dataset.collapsed === 'true';
        item.dataset.collapsed = (!isCollapsed).toString();
        collapseBtn.title = isCollapsed ? 'Hide notes' : 'Show ratings';
    });

    // Remove button
    const removeBtn = item.querySelector('.btn-remove');
    removeBtn.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();

        if (attribute && attribute.id) {
            // Mark for deletion (soft delete)
            item.classList.add('to-delete');
            item.style.opacity = '0.5';
        } else {
            // Deletion animation
            item.classList.add('removing');
            setTimeout(() => {
                if (item.parentNode) {
                    item.remove();
                    updateNoAttributesMessage();
                }
            }, 300);
        }
        updateNoAttributesMessage();
    });

    const attributesList = document.getElementById('attributesList');
    attributesList.appendChild(item);

    updateNoAttributesMessage();

    setTimeout(() => {
        item.querySelector('.attr-label').focus();
    }, 50);

    return item;
}

function updateNoAttributesMessage() {
    const attributesList = document.getElementById('attributesList');
    const noAttributesMessage = document.getElementById('noAttributesMessage');

    if (!attributesList || !noAttributesMessage) return;

    // Count the visible elements (not those marked for deletion).
    const visibleItems = Array.from(attributesList.querySelectorAll('.attribute-item:not(.to-delete):not(.removing)'));
    const hasAttributes = visibleItems.length > 0;

    noAttributesMessage.style.display = hasAttributes ? 'none' : 'flex';
}

async function saveEntity() {
    const type = document.getElementById('entityType').value;
    const name = document.getElementById('entityName').value.trim();

    if (!name) {
        showNotification('The entity name is required', 'error');
        return;
    }

    try {
        let entityId;

        if (state.currentEntity) {
            // Update entity
            const response = await fetch(`/api/entities/${state.currentEntity.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ type, name })
            });

            if (!response.ok) throw new Error('Failed to update entity');
            entityId = state.currentEntity.id;
        } else {
            // Create entity
            const response = await fetch('/api/entities', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    project_id: state.currentProject.id,
                    type,
                    name
                })
            });

            if (!response.ok) throw new Error('Failed to create entity');
            const result = await response.json();
            entityId = result.id;
        }

        // Save attributes
        await saveAttributes(entityId);

        // Reload data
        await loadProjectData(state.currentProject.id);

        // Update views
        renderGraph();
        renderDataView();

        closeModal('entityModal');
        showNotification('Entity successfully registered', 'success');

    } catch (error) {
        console.error('Error saving entity:', error);
        showNotification('Error during registration', 'error');
    }
}

async function saveAttributes(entityId) {
    const attributeItems = document.querySelectorAll('.attribute-item:not(.to-delete)');
    const itemsToDelete = document.querySelectorAll('.attribute-item.to-delete');

    // Use Promise.all for parallel queries
    const requests = [];

    for (const item of itemsToDelete) {
        const attrId = item.dataset.id;
        if (attrId) {
            requests.push(
                fetch(`/api/attributes/${attrId}`, {
                    method: 'DELETE'
                })
            );
        }
    }

    for (const item of attributeItems) {
        const category = item.querySelector('.attr-category').value;
        const label = item.querySelector('.attr-label').value.trim();
        const value = item.querySelector('.attr-value').value.trim();
        const notes = item.querySelector('.attr-notes').value.trim();

        // Skip empty attributes
        if (!label && !value) continue;

        const attrId = item.dataset.id;

        if (attrId) {
            // Update existing
            requests.push(
                fetch(`/api/attributes/${attrId}`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ category, label, value, notes })
                })
            );
        } else {
            // Create new
            requests.push(
                fetch('/api/attributes', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        entity_id: entityId,
                        category,
                        label,
                        value,
                        notes
                    })
                })
            );
        }
    }

    try {
        await Promise.all(requests);
    } catch (error) {
        console.error('Error saving attributes:', error);
        throw error;
    }
}

async function deleteEntity() {
    if (!state.currentEntity) return;

    if (!confirm(`Are you sure you want to delete "${state.currentEntity.name}" ?`)) {
        return;
    }

    try {
        const response = await fetch(`/api/entities/${state.currentEntity.id}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            await loadProjectData(state.currentProject.id);
            renderGraph();
            renderDataView();
            closeModal('entityModal');
            showNotification('Entity successfully deleted', 'success');
        } else {
            showNotification('Error during deletion', 'error');
        }
    } catch (error) {
        console.error('Error deleting entity:', error);
        showNotification('Error during deletion', 'error');
    }
}

// ==================== GRAPH VISUALIZATION ====================
function initializeNetwork() {
    const container = document.getElementById('network');

    const config = state.config?.graph || {};
    const defaultLayout = config.defaultLayout || 'force';

    const options = {
        nodes: {
            shape: 'dot',
            size: 25,
            font: {
                size: 14,
                color: '#f1f5f9',
                face: 'Arial'
            },
            borderWidth: 2,
            shadow: {
                enabled: true,
                color: 'rgba(0,0,0,0.5)',
                size: 10,
                x: 5,
                y: 5
            }
        },
        edges: {
            width: 2,
            color: {
                color: '#64748b',
                highlight: '#2563eb',
                hover: '#3b82f6'
            },
            smooth: {
                type: 'continuous'
            },
            arrows: {
                to: {
                    enabled: true,
                    scaleFactor: 0.5
                }
            },
            font: {
                size: 12,
                color: '#cbd5e1',
                strokeWidth: 0
            }
        },
        physics: {
            enabled: config.physics?.enabled ?? true,
            barnesHut: {
                gravitationalConstant: -30000,
                centralGravity: 0.3,
                springLength: 200,
                springConstant: 0.04,
                damping: 0.09,
                avoidOverlap: 0.5
            },
            stabilization: {
                iterations: config.physics?.stabilization?.iterations || 100
            }
        },
        interaction: {
            hover: true,
            tooltipDelay: 100,
            zoomView: true,
            dragView: true
        },
        layout: {
            improvedLayout: true,
            hierarchical: {
                enabled: defaultLayout === 'hierarchical'
            }
        }
    };

    // Set initial layout selection in UI
    const layoutSelect = document.getElementById('layoutSelect');
    if (layoutSelect) {
        layoutSelect.value = defaultLayout;
    }

    state.network = new vis.Network(container, { nodes: [], edges: [] }, options);

    // Event listeners
    state.network.on('doubleClick', (params) => {
        if (params.nodes.length > 0) {
            const nodeId = params.nodes[0];
            const entity = state.entities.find(e => e.id === nodeId);
            if (entity) {
                openEntityModal(entity);
            }
        }
    });
}

function renderGraph() {
    if (!state.network) return;

    const nodes = new vis.DataSet();
    const edges = new vis.DataSet();

    // Create nodes for entities
    state.entities.forEach(entity => {
        const color = getEntityColor(entity.type);
        const icon = getEntityIcon(entity.type);

        nodes.add({
            id: entity.id,
            label: `${icon} ${entity.name}`,
            color: {
                background: color,
                border: darkenColor(color, 20),
                highlight: {
                    background: lightenColor(color, 20),
                    border: color
                }
            },
            title: `${entity.type}: ${entity.name}`,
            size: 30
        });

        // Create nodes for important attributes and connect them
        const entityAttrs = state.attributes.filter(a => a.entity_id === entity.id);
        entityAttrs.forEach(attr => {
            const attrId = `attr-${attr.id}`;
            const categoryColor = getCategoryColor(attr.category);
            const categoryIcon = getCategoryIcon(attr.category);

            // Add attribute node (smaller) with category-based styling
            nodes.add({
                id: attrId,
                label: `${categoryIcon} ${attr.value}`,
                color: {
                    background: categoryColor,
                    border: darkenColor(categoryColor, 20),
                    highlight: {
                        background: lightenColor(categoryColor, 20),
                        border: categoryColor
                    }
                },
                size: 20,
                font: { size: 11, color: '#f1f5f9' },
                title: `${attr.category} - ${attr.label}: ${attr.value}${attr.notes ? '\n' + attr.notes : ''}`
            });

            // Connect to entity
            edges.add({
                from: entity.id,
                to: attrId,
                label: attr.label,
                length: 150,
                dashes: true,
                color: {
                    color: categoryColor,
                    opacity: 0.6
                }
            });
        });
    });

    // Create edges for relationships - ONLY from state.relationships
    // Do not use relationshipsData to avoid duplication
    state.relationships.forEach(rel => {
        edges.add({
            id: `rel-${rel.id}`, // Add a unique ID
            from: rel.source_entity_id,
            to: rel.target_entity_id,
            label: rel.relationship_type,
            title: rel.description,
            length: 250
        });
    });

    state.network.setData({ nodes, edges });

    // Fit to screen after stabilization
    setTimeout(() => {
        state.network.fit({
            animation: {
                duration: 1000,
                easingFunction: 'easeInOutQuad'
            }
        });
    }, 500);
}

function getEntityColor(type) {
    if (state.config && state.config.graph && state.config.graph.colors) {
        const configColor = state.config.graph.colors[type];
        if (configColor) return configColor;
    }

    const colors = {
        'Person': '#2563eb',
        'Organization': '#dc2626',
        'Location': '#16a34a',
        'Document': '#ea580c',
        'Event': '#ca8a04',
        'Website': '#7c3aed',
        'E-mail': '#0891b2',
        'Phone': '#059669',
        'Vehicle': '#4f46e5',
        'Other': '#9333ea'
    };
    return colors[type] || '#9333ea';
}

function getEntityIcon(type) {
    const icons = {
        'Person': '👤',
        'Organization': '🏢',
        'Location': '📍',
        'Document': '📄',
        'Event': '📅',
        'Website': '🌐',
        'E-mail': '📧',
        'Phone': '📞',
        'Vehicle': '🚗',
        'Other': '🔹'
    };
    return icons[type] || '🔹';
}

function getCategoryColor(category) {
    const colors = {
        'Person': '#2563eb',
        'Identification': '#8b5cf6',
        'Contact': '#0891b2',
        'Location': '#16a34a',
        'Social Networks': '#ec4899',
        'Professional': '#f59e0b',
        'Financial': '#10b981',
        'Technical': '#6366f1',
        'Organization': '#dc2626',
        'Document': '#ea580c',
        'Event': '#ca8a04',
        'Website': '#7c3aed',
        'E-mail': '#0891b2',
        'Phone': '#059669',
        'Vehicle': '#4f46e5',
        'Other': '#64748b'
    };
    return colors[category] || '#64748b';
}

function getCategoryIcon(category) {
    const icons = {
        'Person': '👤',
        'Identification': '🆔',
        'Contact': '📞',
        'Location': '📍',
        'Social Networks': '💬',
        'Professional': '💼',
        'Financial': '💰',
        'Technical': '⚙️',
        'Organization': '🏢',
        'Document': '📄',
        'Event': '📅',
        'Website': '🌐',
        'E-mail': '📧',
        'Phone': '📞',
        'Vehicle': '🚗',
        'Other': '🔹'
    };
    return icons[category] || '🔹';
}

function fitGraph() {
    if (state.network) {
        state.network.fit({
            animation: {
                duration: 500,
                easingFunction: 'easeInOutQuad'
            }
        });
    }
}

function zoomGraph(scale) {
    if (state.network) {
        const currentScale = state.network.getScale();
        state.network.moveTo({
            scale: currentScale * scale,
            animation: {
                duration: 300,
                easingFunction: 'easeInOutQuad'
            }
        });
    }
}

function resetGraph() {
    renderGraph();
}

function updateLayout(layoutType) {
    if (!state.network) return;

    let options = {
        layout: {
            hierarchical: {
                enabled: false
            }
        },
        physics: {
            enabled: true
        }
    };

    if (layoutType === 'hierarchical') {
        options.layout = {
            hierarchical: {
                enabled: true,
                direction: 'UD',
                sortMethod: 'directed',
                levelSeparation: 150,
                nodeSpacing: 200
            }
        };
        options.physics = { enabled: false };
    } else if (layoutType === 'circular') {
        // Circular layout will be handled by physics (using repulsion solver)
        options.physics = {
            enabled: true,
            solver: 'repulsion',
            repulsion: {
                nodeDistance: 100,
                centralGravity: 0.2,
                springLength: 200,
                springConstant: 0.05,
                damping: 0.09
            }
        };
    } else if (layoutType === 'force') {
        options.physics = {
            enabled: true,
            solver: 'barnesHut',
            barnesHut: {
                gravitationalConstant: -30000,
                centralGravity: 0.3,
                springLength: 200,
                springConstant: 0.04,
                damping: 0.09,
                avoidOverlap: 0.5
            }
        };
    }

    state.network.setOptions(options);

    // Force stabilization to ensure clean transition
    if (layoutType !== 'hierarchical') {
        state.network.stabilize(500);
    }
}

// ==================== DATA VIEW ====================
function renderDataView() {
    const container = document.getElementById('entitiesGrid');

    if (state.entities.length === 0) {
        container.innerHTML = `
            <div class="loading" style="grid-column: 1/-1;">
                <i class="fas fa-database"></i>
                <p>No entity. Click "Add Entity" to begin.</p>
            </div>
        `;
        return;
    }

    container.innerHTML = state.entities.map(entity => {
        const entityAttrs = state.attributes.filter(a => a.entity_id === entity.id);
        const color = getEntityColor(entity.type);

        return `
            <div class="entity-card" data-id="${entity.id}" style="border-left: 4px solid ${color};">
                <div class="entity-header">
                    <div>
                        <div class="entity-type" style="background: ${color}20; color: ${color};">
                            ${getEntityIcon(entity.type)} ${entity.type}
                        </div>
                        <div class="entity-name">${escapeHtml(entity.name)}</div>
                    </div>
                </div>
                <div class="entity-attributes">
                    ${entityAttrs.length === 0 ? '<p class="text-muted">No attributes</p>' : ''}
                    ${entityAttrs.map(attr => `
                        <div class="attribute-row">
                            <div class="attribute-label">
                                <span class="attribute-category">${escapeHtml(attr.category)}</span>
                                ${escapeHtml(attr.label)}
                            </div>
                            <div class="attribute-value">${escapeHtml(attr.value)}</div>
                            ${attr.notes ? `<div class="text-muted" style="font-size: 0.8rem; margin-top: 0.25rem;">${escapeHtml(attr.notes)}</div>` : ''}
                        </div>
                    `).join('')}
                </div>
            </div>
        `;
    }).join('');

    // Add click listeners
    container.querySelectorAll('.entity-card').forEach(card => {
        card.addEventListener('click', () => {
            const id = parseInt(card.dataset.id);
            const entity = state.entities.find(e => e.id === id);
            if (entity) {
                openEntityModal(entity);
            }
        });
    });
}

// ==================== VIEW SWITCHING ====================
function switchView(view) {
    state.currentView = view;

    // Update toggle buttons
    document.querySelectorAll('.toggle-btn').forEach(btn => {
        btn.classList.toggle('active', btn.dataset.view === view);
    });

    // Show/hide views
    document.getElementById('graphView').classList.toggle('hidden', view !== 'graph');
    document.getElementById('dataView').classList.toggle('hidden', view !== 'data');

    // Redraw graph if switching to it
    if (view === 'graph' && state.network) {
        setTimeout(() => {
            state.network.redraw();
            state.network.fit();
        }, 100);
    }
}

// ==================== EXPORT ====================
async function exportProject(format) {
    if (!state.currentProject) return;

    try {
        showNotification('Exportation en cours...', 'info');

        const response = await fetch(`/api/projects/${state.currentProject.id}/export-data`);
        const data = await response.json();
        
        const relsResponse = await fetch(`/api/projects/${state.currentProject.id}/relationships`);
        const relationships = await relsResponse.json();
        
        data.relationships = relationships;

        let content, filename, mimeType;

        switch (format) {
            case 'json':
                content = JSON.stringify(data, null, 2);
                filename = `messis_${state.currentProject.id}.json`;
                mimeType = 'application/json';
                break;

            case 'csv':
                content = exportToCSV(data);
                filename = `messis_${state.currentProject.id}.csv`;
                mimeType = 'text/csv';
                break;

            case 'markdown':
                content = exportToMarkdown(data);
                filename = `messis_${state.currentProject.id}.md`;
                mimeType = 'text/markdown';
                break;

            case 'html':
                content = exportToHTML(data);
                filename = `messis_${state.currentProject.id}.html`;
                mimeType = 'text/html';
                break;

            case 'txt':
                content = exportToText(data);
                filename = `messis_${state.currentProject.id}.txt`;
                mimeType = 'text/plain';
                break;

            case 'pdf':
                window.open(`/api/projects/${state.currentProject.id}/export/pdf`, '_blank');
                showNotification('Export PDF generated', 'success');
                return;

            case 'svg':
                if (state.currentView !== 'graph') {
                    showNotification('Switch to Graph View to export to SVG', 'warning');
                    return;
                }
                content = exportToSVG(data);
                filename = `messis_${state.currentProject.id}.svg`;
                mimeType = 'image/svg+xml';
                break;
        }

        downloadFile(content, filename, mimeType);
        showNotification(`Export ${format.toUpperCase()} successful`, 'success');

    } catch (error) {
        console.error('Export error:', error);
        showNotification('Error during export', 'error');
    }
}

function exportToCSV(data) {
    let csv = 'Type,Name,Category,Label,Value,Notes\n';

    data.entities.forEach(entity => {
        const attrs = data.attributes.filter(a => a.entity_id === entity.id);

        if (attrs.length === 0) {
            csv += `"${entity.type}","${entity.name}","","","",""\n`;
        } else {
            attrs.forEach(attr => {
                csv += `"${entity.type}","${entity.name}","${attr.category}","${attr.label}","${attr.value}","${attr.notes || ''}"\n`;
            });
        }
    });

    csv += '\n\n=== RELATIONSHIPS ===\n';
    csv += 'Source,Relationship Type,Target,Strength,Description,Evidence,Bidirectional\n';

    if (data.relationships && data.relationships.length > 0) {
        data.relationships.forEach(rel => {
            const sourceEntity = data.entities.find(e => e.id === rel.source_entity_id);
            const targetEntity = data.entities.find(e => e.id === rel.target_entity_id);
            const type = getRelationshipType(rel.relationship_type_id);

            csv += `"${sourceEntity?.name || 'Unknown'} (${sourceEntity?.type || '?'})",`;
            csv += `"${type.name}",`;
            csv += `"${targetEntity?.name || 'Unknown'} (${targetEntity?.type || '?'})",`;
            csv += `"${rel.strength || 5}/10",`;
            csv += `"${rel.description || ''}",`;
            csv += `"${rel.evidence || ''}",`;
            csv += `"${rel.bidirectional ? 'Yes' : 'No'}"\n`;
        });
    } else {
        csv += '"No defined relationship","","","","","",""\n';
    }

    return csv;
}

function exportToMarkdown(data) {
    let md = `# ${data.project.name}\n\n`;

    if (data.project.description) {
        md += `${data.project.description}\n\n`;
    }

    md += `**Created on :** ${formatDate(data.project.created_at)}\n`;
    md += `**Last modified :** ${formatDate(data.project.updated_at)}\n\n`;
    
    // Statistics
    const entityCount = data.entities.length;
    const relationCount = data.relationships ? data.relationships.length : 0;
    md += `**Statistics :** ${entityCount} entity(ies), ${relationCount} relationship(s)\n\n`;
    
    md += `---\n\n`;

    md += `## 📊 Entities (${entityCount})\n\n`;

    data.entities.forEach((entity, index) => {
        md += `### ${index + 1}. ${getEntityIcon(entity.type)} ${entity.name}\n\n`;
        md += `**Type :** ${entity.type}\n\n`;

        const attrs = data.attributes.filter(a => a.entity_id === entity.id);

        if (attrs.length > 0) {
            md += `#### Attributes\n\n`;

            attrs.forEach(attr => {
                md += `- **${attr.label}** (${attr.category}) : ${attr.value}\n`;
                if (attr.notes) {
                    md += `  - *Notes :* ${attr.notes}\n`;
                }
            });
            md += `\n`;
        }

        // Relationships of this entity
        const entityRelations = data.relationships ? 
            data.relationships.filter(r => 
                r.source_entity_id === entity.id || r.target_entity_id === entity.id
            ) : [];
        
        if (entityRelations.length > 0) {
            md += `#### Relationships\n\n`;
            
            entityRelations.forEach(rel => {
                const isSource = rel.source_entity_id === entity.id;
                const otherEntityId = isSource ? rel.target_entity_id : rel.source_entity_id;
                const otherEntity = data.entities.find(e => e.id === otherEntityId);
                const type = getRelationshipType(rel.relationship_type_id);
                
                if (otherEntity) {
                    const direction = isSource ? '→' : '←';
                    const otherDirection = isSource ? '←' : '→';
                    
                    md += `- **${isSource ? 'Source' : 'Target'}** : ${otherEntity.name} (${otherEntity.type})\n`;
                    md += `  - **Kind :** ${type.name} ${rel.bidirectional ? '↔' : direction}\n`;
                    md += `  - **Strength :** ${rel.strength || 5}/10\n`;
                    if (rel.description) {
                        md += `  - **Description :** ${rel.description}\n`;
                    }
                    if (rel.evidence) {
                        md += `  - **Evidence :** ${rel.evidence}\n`;
                    }
                    md += `\n`;
                }
            });
        }

        md += `---\n\n`;
    });

    if (data.relationships && data.relationships.length > 0) {
        md += `## 🔗 Relationships (${relationCount})\n\n`;
        
        md += `### Table of Relationships\n\n`;
        md += `| Source | Relationship | Target | Strength | Description |\n`;
        md += `|--------|--------------|--------|----------|-------------|\n`;
        
        data.relationships.forEach(rel => {
            const sourceEntity = data.entities.find(e => e.id === rel.source_entity_id);
            const targetEntity = data.entities.find(e => e.id === rel.target_entity_id);
            const type = getRelationshipType(rel.relationship_type_id);
            
            md += `| ${sourceEntity?.name || '?'} | ${type.name} ${rel.bidirectional ? '↔' : '→'} | ${targetEntity?.name || '?'} | ${rel.strength || 5}/10 | ${rel.description || ''} |\n`;
        });
        
        md += `\n`;
        
        md += `### Relational Diagram\n\n`;
        md += '```\n';
        
        data.entities.forEach(entity => {
            const outgoingRels = data.relationships.filter(r => r.source_entity_id === entity.id);
            const incomingRels = data.relationships.filter(r => r.target_entity_id === entity.id);
            
            if (outgoingRels.length > 0 || incomingRels.length > 0) {
                md += `${getEntityIcon(entity.type)} ${entity.name}\n`;
                
                outgoingRels.forEach(rel => {
                    const targetEntity = data.entities.find(e => e.id === rel.target_entity_id);
                    const type = getRelationshipType(rel.relationship_type_id);
                    const arrow = rel.bidirectional ? '↔' : '→';
                    
                    if (targetEntity) {
                        md += `  ${arrow} ${getEntityIcon(targetEntity.type)} ${targetEntity.name} [${type.name}]\n`;
                    }
                });
                
                md += `\n`;
            }
        });
        
        md += '```\n\n';
    }

    return md;
}

function exportToHTML(data) {
    let html = `<!DOCTYPE html>
<html lang="fr">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>${escapeHtml(data.project.name)} - OSINT Report</title>
    <style>
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            max-width: 1200px;
            margin: 0 auto;
            padding: 2rem;
            background: #f8fafc;
            color: #1e293b;
            line-height: 1.6;
        }
        
        .header {
            background: linear-gradient(135deg, #2563eb, #3b82f6);
            color: white;
            padding: 2rem;
            border-radius: 12px;
            margin-bottom: 2rem;
            box-shadow: 0 10px 15px -3px rgba(37, 99, 235, 0.3);
        }
        
        h1 { 
            font-size: 2.5rem;
            margin-bottom: 0.5rem;
            color: white;
        }
        
        .subtitle {
            font-size: 1.1rem;
            opacity: 0.9;
            margin-bottom: 1.5rem;
        }
        
        .stats {
            display: flex;
            gap: 2rem;
            margin-top: 1rem;
        }
        
        .stat-card {
            background: rgba(255, 255, 255, 0.2);
            padding: 1rem 1.5rem;
            border-radius: 8px;
            backdrop-filter: blur(10px);
        }
        
        h2 { 
            color: #2563eb; 
            border-bottom: 3px solid #2563eb; 
            padding-bottom: 0.5rem;
            margin-top: 3rem;
            font-size: 1.8rem;
        }
        
        h3 { 
            color: #334155; 
            margin-top: 2rem; 
            font-size: 1.4rem;
        }
        
        .entity-section {
            background: white;
            padding: 1.5rem;
            margin: 1.5rem 0;
            border-radius: 12px;
            box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1);
            border-left: 4px solid #2563eb;
        }
        
        .entity-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 1rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid #f1f5f9;
        }
        
        .entity-type {
            display: inline-block;
            background: #e0e7ff;
            color: #3730a3;
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-size: 0.875rem;
            font-weight: 600;
        }
        
        .attributes-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 1rem;
            margin: 1rem 0;
        }
        
        .attribute-card {
            background: #f8fafc;
            padding: 1rem;
            border-radius: 8px;
            border-left: 3px solid #3b82f6;
        }
        
        .attribute-label {
            font-weight: 600;
            color: #475569;
            font-size: 0.9rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .attribute-value {
            color: #1e293b;
            margin-top: 0.5rem;
            font-size: 1.1rem;
        }
        
        .attribute-category {
            display: inline-block;
            background: #e2e8f0;
            color: #475569;
            padding: 0.25rem 0.75rem;
            border-radius: 4px;
            font-size: 0.75rem;
            margin-left: 0.5rem;
        }
        
        .notes {
            color: #64748b;
            font-size: 0.875rem;
            font-style: italic;
            margin-top: 0.5rem;
            padding: 0.5rem;
            background: #f1f5f9;
            border-radius: 4px;
        }
        
        .relations-section {
            background: white;
            padding: 1.5rem;
            margin: 1.5rem 0;
            border-radius: 12px;
            box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1);
            border-left: 4px solid #10b981;
        }
        
        .relation-item {
            background: #f0fdf4;
            padding: 1rem;
            margin: 1rem 0;
            border-radius: 8px;
            border: 1px solid #bbf7d0;
        }
        
        .relation-header {
            display: flex;
            align-items: center;
            gap: 1rem;
            margin-bottom: 0.5rem;
        }
        
        .relation-type {
            display: inline-block;
            background: #10b981;
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 4px;
            font-size: 0.875rem;
            font-weight: 600;
        }
        
        .relation-strength {
            display: inline-block;
            background: linear-gradient(90deg, #10b981, #3b82f6, #ef4444);
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 4px;
            font-size: 0.875rem;
            font-weight: 600;
        }
        
        .bidirectional {
            display: inline-flex;
            align-items: center;
            gap: 0.25rem;
            background: #fef3c7;
            color: #92400e;
            padding: 0.25rem 0.75rem;
            border-radius: 4px;
            font-size: 0.875rem;
        }
        
        .evidence {
            background: #fefce8;
            padding: 0.75rem;
            margin-top: 0.75rem;
            border-radius: 6px;
            border-left: 3px solid #f59e0b;
            font-size: 0.875rem;
        }
        
        .table-container {
            overflow-x: auto;
            margin: 1rem 0;
        }
        
        table {
            width: 100%;
            border-collapse: collapse;
            margin: 1rem 0;
        }
        
        th {
            background: #f1f5f9;
            padding: 1rem;
            text-align: left;
            font-weight: 600;
            color: #475569;
            border-bottom: 2px solid #e2e8f0;
        }
        
        td {
            padding: 1rem;
            border-bottom: 1px solid #e2e8f0;
            vertical-align: top;
        }
        
        tr:hover {
            background: #f8fafc;
        }
        
        .graph-summary {
            background: #f8fafc;
            padding: 1.5rem;
            border-radius: 8px;
            margin: 1rem 0;
        }
        
        .meta {
            color: #64748b;
            font-size: 0.875rem;
            margin-top: 0.5rem;
        }
        
        @media print {
            body {
                padding: 1rem;
            }
            
            .header {
                box-shadow: none;
            }
            
            .entity-section, .relations-section {
                box-shadow: none;
                border: 1px solid #e2e8f0;
                page-break-inside: avoid;
            }
        }
        
        @media (max-width: 768px) {
            .stats {
                flex-direction: column;
                gap: 1rem;
            }
            
            .attributes-grid {
                grid-template-columns: 1fr;
            }
            
            .entity-header {
                flex-direction: column;
                gap: 1rem;
            }
        }
    </style>
</head>
<body>
    <div class="header">
        <h1>${escapeHtml(data.project.name)}</h1>
        ${data.project.description ? `<div class="subtitle">${escapeHtml(data.project.description)}</div>` : ''}
        
        <div class="stats">
            <div class="stat-card">
                <div style="font-size: 0.875rem; opacity: 0.9;">Entities</div>
                <div style="font-size: 1.5rem; font-weight: bold;">${data.entities.length}</div>
            </div>
            <div class="stat-card">
                <div style="font-size: 0.875rem; opacity: 0.9;">Relationships</div>
                <div style="font-size: 1.5rem; font-weight: bold;">${data.relationships ? data.relationships.length : 0}</div>
            </div>
            <div class="stat-card">
                <div style="font-size: 0.875rem; opacity: 0.9;">Created on</div>
                <div style="font-size: 1rem; font-weight: bold;">${formatDate(data.project.created_at)}</div>
            </div>
        </div>
    </div>
`;

    html += `<h2>📊 Entities (${data.entities.length})</h2>`;

    data.entities.forEach((entity, index) => {
        const attrs = data.attributes.filter(a => a.entity_id === entity.id);
        const entityRelations = data.relationships ? 
            data.relationships.filter(r => 
                r.source_entity_id === entity.id || r.target_entity_id === entity.id
            ) : [];

        html += `
    <div class="entity-section">
        <div class="entity-header">
            <div>
                <div style="font-size: 0.875rem; color: #64748b;">Entité #${index + 1}</div>
                <div style="font-size: 1.5rem; font-weight: bold; margin: 0.5rem 0;">
                    ${getEntityIcon(entity.type)} ${escapeHtml(entity.name)}
                </div>
                <span class="entity-type">${escapeHtml(entity.type)}</span>
            </div>
        </div>`;

        if (attrs.length > 0) {
            html += `<h3>Attributs (${attrs.length})</h3>
        <div class="attributes-grid">`;

            attrs.forEach(attr => {
                html += `
            <div class="attribute-card">
                <div class="attribute-label">
                    ${escapeHtml(attr.label)}
                    <span class="attribute-category">${escapeHtml(attr.category)}</span>
                </div>
                <div class="attribute-value">${escapeHtml(attr.value)}</div>
                ${attr.notes ? `<div class="notes">${escapeHtml(attr.notes)}</div>` : ''}
            </div>`;
            });

            html += `</div>`;
        }

        // Relationships of this entity
        if (entityRelations.length > 0) {
            html += `<h3>Relations (${entityRelations.length})</h3>`;

            entityRelations.forEach(rel => {
                const isSource = rel.source_entity_id === entity.id;
                const otherEntityId = isSource ? rel.target_entity_id : rel.source_entity_id;
                const otherEntity = data.entities.find(e => e.id === otherEntityId);
                const type = getRelationshipType(rel.relationship_type_id);
                
                if (otherEntity) {
                    const directionIcon = rel.bidirectional ? '↔' : (isSource ? '→' : '←');
                    
                    html += `
            <div class="relation-item">
                <div class="relation-header">
                    <div style="flex: 1;">
                        <div style="display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.5rem;">
                            <span style="font-weight: bold;">${escapeHtml(entity.name)}</span>
                            <span style="font-size: 1.2rem;">${directionIcon}</span>
                            <span style="font-weight: bold;">${escapeHtml(otherEntity.name)}</span>
                            <span style="font-size: 0.875rem; color: #64748b;">(${escapeHtml(otherEntity.type)})</span>
                        </div>
                    </div>
                    <span class="relation-type">${type.name}</span>
                    <span class="relation-strength">${rel.strength || 5}/10</span>
                    ${rel.bidirectional ? '<span class="bidirectional"><i class="fas fa-exchange-alt"></i> Bidirectional</span>' : ''}
                </div>
                
                ${rel.description ? `<div style="margin: 0.5rem 0; padding: 0.5rem; background: #f1f5f9; border-radius: 4px;">${escapeHtml(rel.description)}</div>` : ''}
                
                ${rel.evidence ? `<div class="evidence"><strong>Evidence/Sources :</strong> ${escapeHtml(rel.evidence)}</div>` : ''}
            </div>`;
                }
            });
        }

        html += `</div>`;
    });

    if (data.relationships && data.relationships.length > 0) {
        html += `<h2>🔗 Relationships (${data.relationships.length})</h2>`;

        html += `<div class="relations-section">
            <h3>Tableau des Relations</h3>
            <div class="table-container">
                <table>
                    <thead>
                        <tr>
                            <th>Source</th>
                            <th>Type of Relationship</th>
                            <th>Target</th>
                            <th>Strenght</th>
                            <th>Description</th>
                            <th>Bidirectional</th>
                        </tr>
                    </thead>
                    <tbody>`;

        data.relationships.forEach(rel => {
            const sourceEntity = data.entities.find(e => e.id === rel.source_entity_id);
            const targetEntity = data.entities.find(e => e.id === rel.target_entity_id);
            const type = getRelationshipType(rel.relationship_type_id);

            html += `
                        <tr>
                            <td>
                                <strong>${escapeHtml(sourceEntity?.name || '?')}</strong><br>
                                <small style="color: #64748b;">${escapeHtml(sourceEntity?.type || '')}</small>
                            </td>
                            <td>
                                <span class="relation-type" style="background: ${type.color || '#64748b'}">${type.name}</span>
                            </td>
                            <td>
                                <strong>${escapeHtml(targetEntity?.name || '?')}</strong><br>
                                <small style="color: #64748b;">${escapeHtml(targetEntity?.type || '')}</small>
                            </td>
                            <td>
                                <div style="background: linear-gradient(90deg, #10b981, #3b82f6, #ef4444); 
                                            width: ${(rel.strength || 5) * 10}%; 
                                            height: 20px; 
                                            border-radius: 4px;
                                            display: flex;
                                            align-items: center;
                                            justify-content: center;
                                            color: white;
                                            font-size: 0.75rem;
                                            font-weight: bold;">
                                    ${rel.strength || 5}/10
                                </div>
                            </td>
                            <td>${escapeHtml(rel.description || '')}</td>
                            <td>${rel.bidirectional ? '<span style="color: #10b981; font-weight: bold;">✓ Yes</span>' : '<span style="color: #64748b;">No</span>'}</td>
                        </tr>`;
        });

        html += `</tbody>
                </table>
            </div>
            
            <h3 style="margin-top: 2rem;">Summary of Connections</h3>
            <div class="graph-summary">`;

        // Connection statistics
        const connectionStats = {};
        data.entities.forEach(entity => {
            const connections = data.relationships.filter(r => 
                r.source_entity_id === entity.id || r.target_entity_id === entity.id
            ).length;
            connectionStats[entity.name] = connections;
        });

        // Sort by number of connections
        const sortedConnections = Object.entries(connectionStats)
            .sort((a, b) => b[1] - a[1]);

        if (sortedConnections.length > 0) {
            html += `<p><strong>Most connected entities :</strong></p>
                <ul>`;
            
            sortedConnections.slice(0, 5).forEach(([name, count]) => {
                html += `<li><strong>${escapeHtml(name)}</strong> : ${count} connection(s)</li>`;
            });
            
            html += `</ul>`;
        }

        html += `</div>
        </div>`;
    }

    html += `
    <div style="margin-top: 3rem; padding-top: 2rem; border-top: 2px solid #e2e8f0; text-align: center; color: #64748b; font-size: 0.875rem;">
        <p>Report generated by Messis OSINT Tool on ${new Date().toLocaleDateString('fr-FR', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        })}</p>
        <p>© ${new Date().getFullYear()} Messis - OSINT Investigation Platform</p>
    </div>
</body>
</html>`;

    return html;
}

function exportToText(data) {
    let txt = `========================================================================\n`;
    txt += `OSINT REPORT - ${data.project.name}\n`;
    txt += `========================================================================\n\n`;

    if (data.project.description) {
        txt += `Description : ${data.project.description}\n\n`;
    }

    txt += `Date of creation : ${formatDate(data.project.created_at)}\n`;
    txt += `Last modified : ${formatDate(data.project.updated_at)}\n`;
    txt += `Statistics : ${data.entities.length} entity(ies), ${data.relationships ? data.relationships.length : 0} relationship(s)\n\n`;

    txt += `========================================================================\n`;
    txt += `ENTITIES (${data.entities.length})\n`;
    txt += `========================================================================\n\n`;

    data.entities.forEach((entity, index) => {
        txt += `${index + 1}. [${entity.type}] ${entity.name}\n`;
        txt += `   ${'─'.repeat(50)}\n`;

        const attrs = data.attributes.filter(a => a.entity_id === entity.id);
        const entityRelations = data.relationships ? 
            data.relationships.filter(r => 
                r.source_entity_id === entity.id || r.target_entity_id === entity.id
            ) : [];

        // Attributes
        if (attrs.length > 0) {
            txt += `   Attributes :\n`;
            attrs.forEach(attr => {
                txt += `   • ${attr.label} (${attr.category}) : ${attr.value}\n`;
                if (attr.notes) {
                    txt += `     Notes : ${attr.notes}\n`;
                }
            });
            txt += `\n`;
        }

        // Relationships
        if (entityRelations.length > 0) {
            txt += `   Relationships :\n`;
            entityRelations.forEach(rel => {
                const isSource = rel.source_entity_id === entity.id;
                const otherEntityId = isSource ? rel.target_entity_id : rel.source_entity_id;
                const otherEntity = data.entities.find(e => e.id === otherEntityId);
                const type = getRelationshipType(rel.relationship_type_id);
                
                if (otherEntity) {
                    const direction = rel.bidirectional ? '↔' : (isSource ? '→' : '←');
                    
                    txt += `   • ${isSource ? 'Source →' : '← Target'} ${otherEntity.name} [${otherEntity.type}]\n`;
                    txt += `     Kind : ${type.name} ${direction}\n`;
                    txt += `     Strength : ${rel.strength || 5}/10\n`;
                    if (rel.description) {
                        txt += `     Description : ${rel.description}\n`;
                    }
                    if (rel.evidence) {
                        txt += `     Evidence : ${rel.evidence}\n`;
                    }
                    txt += `\n`;
                }
            });
        }

        txt += `\n`;
    });

    if (data.relationships && data.relationships.length > 0) {
        txt += `========================================================================\n`;
        txt += `RELATIONSHIPS (${data.relationships.length})\n`;
        txt += `========================================================================\n\n`;

        txt += `Table of Relationships :\n`;
        txt += `┌────────────────┬──────────────────┬────────────────┬──────────────┬────────────────────────────────┐\n`;
        txt += `│ Source          │ Relationship        │ Target         │ Strength       │ Description                       │\n`;
        txt += `├────────────────┼──────────────────┼────────────────┼──────────────┼────────────────────────────────┤\n`;

        data.relationships.forEach(rel => {
            const sourceEntity = data.entities.find(e => e.id === rel.source_entity_id);
            const targetEntity = data.entities.find(e => e.id === rel.target_entity_id);
            const type = getRelationshipType(rel.relationship_type_id);
            
            const sourceName = (sourceEntity?.name || '?').padEnd(14).substring(0, 14);
            const relationName = type.name.padEnd(16).substring(0, 16);
            const targetName = (targetEntity?.name || '?').padEnd(14).substring(0, 14);
            const strength = `${rel.strength || 5}/10`.padEnd(6).substring(0, 6);
            const description = (rel.description || '').padEnd(27).substring(0, 27);
            
            const direction = rel.bidirectional ? '↔' : '→';
            
            txt += `│ ${sourceName} │ ${relationName} ${direction} │ ${targetName} │ ${strength} │ ${description} │\n`;
        });

        txt += `└────────────────┴──────────────────┴────────────────┴──────────────┴────────────────────────────────┘\n\n`;

        txt += `Relational Diagram :\n\n`;
        
        data.entities.forEach(entity => {
            const outgoingRels = data.relationships.filter(r => r.source_entity_id === entity.id);
            const incomingRels = data.relationships.filter(r => r.target_entity_id === entity.id);
            
            if (outgoingRels.length > 0 || incomingRels.length > 0) {
                const entityIcon = getEntityIcon(entity.type);
                txt += `${entityIcon} ${entity.name}\n`;
                
                outgoingRels.forEach(rel => {
                    const targetEntity = data.entities.find(e => e.id === rel.target_entity_id);
                    const type = getRelationshipType(rel.relationship_type_id);
                    
                    if (targetEntity) {
                        const arrow = rel.bidirectional ? '↔' : '→';
                        const targetIcon = getEntityIcon(targetEntity.type);
                        txt += `  ${arrow} ${targetIcon} ${targetEntity.name} [${type.name}]\n`;
                    }
                });
                
                txt += `\n`;
            }
        });
    }

    txt += `========================================================================\n`;
    txt += `END OF REPORT\n`;
    txt += `========================================================================\n`;
    txt += `Generated on : ${new Date().toLocaleDateString('fr-FR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    })}\n`;
    txt += `© ${new Date().getFullYear()} Messis - OSINT Investigation Platform\n`;

    return txt;
}

function exportToSVG(data) {
    if (!state.network) return '';

    // Get all node positions and bounding box
    const nodePositions = state.network.getPositions();
    let minX = Infinity, minY = Infinity, maxX = -Infinity, maxY = -Infinity;

    Object.values(nodePositions).forEach(pos => {
        minX = Math.min(minX, pos.x);
        minY = Math.min(minY, pos.y);
        maxX = Math.max(maxX, pos.x);
        maxY = Math.max(maxY, pos.y);
    });

    // Add padding
    const padding = 50;
    minX -= padding;
    minY -= padding;
    maxX += padding;
    maxY += padding;

    const width = maxX - minX;
    const height = maxY - minY;

    let svg = `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="${minX} ${minY} ${width} ${height}" style="background-color: #0f172a;">
    <style>
        text { font-family: Arial, sans-serif; }
    </style>`;

    // Edges
    state.relationships.forEach(rel => {
        const fromPos = nodePositions[rel.source_entity_id];
        const toPos = nodePositions[rel.target_entity_id];
        
        if (fromPos && toPos) {
            const type = getRelationshipType(rel.relationship_type_id);
            svg += `<line x1="${fromPos.x}" y1="${fromPos.y}" x2="${toPos.x}" y2="${toPos.y}" stroke="${type.color}" stroke-width="${Math.max(1, rel.strength/3)}" opacity="0.6" />`;
            
            // Note: Advanced curved edges and arrows are complex in raw SVG, simplifying to lines for MVP
            // Ideally we'd calculate the Bezier curves used by VisJS
        }
    });

    // Entity Attributes Edges
    state.entities.forEach(entity => {
        const fromPos = nodePositions[entity.id];
        if(!fromPos) return;

         const entityAttrs = state.attributes.filter(a => a.entity_id === entity.id);
         entityAttrs.forEach(attr => {
             const attrId = `attr-${attr.id}`;
             const toPos = nodePositions[attrId];
             if(toPos) {
                const categoryColor = getCategoryColor(attr.category);
                svg += `<line x1="${fromPos.x}" y1="${fromPos.y}" x2="${toPos.x}" y2="${toPos.y}" stroke="${categoryColor}" stroke-dasharray="5,5" stroke-width="1" opacity="0.6" />`;
             }
         });
    });


    // Entities (Nodes)
    state.entities.forEach(entity => {
        const pos = nodePositions[entity.id];
        if (pos) {
            const color = getEntityColor(entity.type);
            const icon = getEntityIcon(entity.type);
            
            // Circle
            svg += `<circle cx="${pos.x}" cy="${pos.y}" r="15" fill="${color}" stroke="#1e293b" stroke-width="2" />`;
            
            // Label
            svg += `<text x="${pos.x}" y="${pos.y + 30}" text-anchor="middle" fill="#f1f5f9" font-size="12">${escapeHtml(entity.name)}</text>`;
            
            // Icon
            svg += `<text x="${pos.x}" y="${pos.y + 5}" text-anchor="middle" fill="#ffffff" font-size="14">${icon}</text>`;
        }
    });

    // Attributes (Nodes)
     state.entities.forEach(entity => {
         const entityAttrs = state.attributes.filter(a => a.entity_id === entity.id);
         entityAttrs.forEach(attr => {
             const attrId = `attr-${attr.id}`;
             const pos = nodePositions[attrId];
             if (pos) {
                  const categoryColor = getCategoryColor(attr.category);
                  const categoryIcon = getCategoryIcon(attr.category);

                  svg += `<circle cx="${pos.x}" cy="${pos.y}" r="10" fill="${categoryColor}" stroke="#1e293b" stroke-width="1" />`;
                  svg += `<text x="${pos.x}" y="${pos.y + 25}" text-anchor="middle" fill="#cbd5e1" font-size="10">${escapeHtml(attr.value)}</text>`;
                   svg += `<text x="${pos.x}" y="${pos.y + 4}" text-anchor="middle" fill="#ffffff" font-size="10">${categoryIcon}</text>`;
             }
         });
    });


    svg += `</svg>`;
    return svg;
}

// ==================== UTILITIES ====================
function closeModal(modalId) {
    document.getElementById(modalId).classList.remove('active');
}

function showNotification(message, type = 'info') {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 1rem 1.5rem;
        background: ${type === 'success' ? '#16a34a' : type === 'error' ? '#dc2626' : '#0891b2'};
        color: white;
        border-radius: 8px;
        box-shadow: 0 10px 15px -3px rgba(0,0,0,0.3);
        z-index: 10000;
        animation: slideIn 0.3s ease;
        max-width: 400px;
    `;

    const icon = type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle';
    notification.innerHTML = `<i class="fas fa-${icon}"></i> ${escapeHtml(message)}`;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

function downloadFile(content, filename, mimeType) {
    const blob = new Blob([content], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    URL.revokeObjectURL(url);
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
}

function darkenColor(color, percent) {
    const num = parseInt(color.replace('#', ''), 16);
    const amt = Math.round(2.55 * percent);
    const R = (num >> 16) - amt;
    const G = (num >> 8 & 0x00FF) - amt;
    const B = (num & 0x0000FF) - amt;
    return '#' + (
        0x1000000 +
        (R < 255 ? R < 1 ? 0 : R : 255) * 0x10000 +
        (G < 255 ? G < 1 ? 0 : G : 255) * 0x100 +
        (B < 255 ? B < 1 ? 0 : B : 255)
    ).toString(16).slice(1);
}

function lightenColor(color, percent) {
    const num = parseInt(color.replace('#', ''), 16);
    const amt = Math.round(2.55 * percent);
    const R = (num >> 16) + amt;
    const G = (num >> 8 & 0x00FF) + amt;
    const B = (num & 0x0000FF) + amt;
    return '#' + (
        0x1000000 +
        (R < 255 ? R < 1 ? 0 : R : 255) * 0x10000 +
        (G < 255 ? G < 1 ? 0 : G : 255) * 0x100 +
        (B < 255 ? B < 1 ? 0 : B : 255)
    ).toString(16).slice(1);
}

const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

document.addEventListener('DOMContentLoaded', () => {
    setTimeout(() => {
        updateNoAttributesMessage();
    }, 100);
});

// ==================== RELATIONS MANAGEMENT ====================
let relationshipsData = [];

async function loadRelationships() {
    if (!state.currentProject) return;

    try {
        const response = await fetch(`/api/projects/${state.currentProject.id}/relationships`);
        relationshipsData = await response.json();

        // Sync with global state to ensure consistency across views
        // AVOID duplication: replace the table rather than adding
        state.relationships = [...relationshipsData];

        // Update graph with relationships - ONLY if you are in graph view
        if (state.currentView === 'graph') {
            updateGraphWithRelationships();
        }
    } catch (error) {
        console.error('Error loading relationships:', error);
    }
}

function updateGraphWithRelationships() {
    if (!state.network) return;

    const edges = state.network.body.data.edges;

    // First, clean up all existing relationships
    const currentEdges = edges.get();
    const relationEdges = currentEdges.filter(edge => edge.id && edge.id.startsWith('rel-'));
    
    if (relationEdges.length > 0) {
        edges.remove(relationEdges.map(edge => edge.id));
    }

    // Add only the new relationships
    relationshipsData.forEach(rel => {
        const type = getRelationshipType(rel.relationship_type_id);

        edges.add({
            id: `rel-${rel.id}`,
            from: rel.source_entity_id,
            to: rel.target_entity_id,
            label: type.name,
            title: `${rel.description || ''}\nForce: ${rel.strength}/10`,
            color: {
                color: type.color,
                highlight: lightenColor(type.color, 20),
                hover: type.color
            },
            width: Math.max(1, rel.strength / 3),
            arrows: {
                to: { enabled: true, scaleFactor: 0.5 }
            },
            dashes: rel.strength < 5,
            smooth: {
                type: 'curvedCCW',
                roundness: 0.2
            }
        });

        // If bidirectional, add reverse edge
        if (rel.bidirectional) {
            edges.add({
                id: `rel-rev-${rel.id}`,
                from: rel.target_entity_id,
                to: rel.source_entity_id,
                label: type.name,
                title: `${rel.description || ''}\n(Bidirectionnel)\nForce: ${rel.strength}/10`,
                color: {
                    color: type.color,
                    highlight: lightenColor(type.color, 20),
                    hover: type.color
                },
                width: Math.max(1, rel.strength / 3),
                arrows: {
                    to: { enabled: true, scaleFactor: 0.5 }
                },
                dashes: rel.strength < 5,
                smooth: {
                    type: 'curvedCW',
                    roundness: 0.2
                }
            });
        }
    });
}

function getRelationshipType(typeId) {
    const defaultTypes = {
        1: { id: 1, name: 'Knows', color: '#3b82f6', description: 'Personal knowledge' },
        2: { id: 2, name: 'Works with', color: '#10b981', description: 'Professional relationship' },
        3: { id: 3, name: 'Family', color: '#8b5cf6', description: 'Family ties' },
        4: { id: 4, name: 'Communication', color: '#f59e0b', description: 'Regular communication' },
        5: { id: 5, name: 'Finance', color: '#ef4444', description: 'Financial link' },
        6: { id: 6, name: 'Property', color: '#6366f1', description: 'Ownership relationship' },
        7: { id: 7, name: 'Location', color: '#84cc16', description: 'Same location' },
        8: { id: 8, name: 'Document', color: '#ec4899', description: 'Mention in document' }
    };

    return defaultTypes[typeId] || { id: typeId, name: 'Relationship', color: '#64748b' };
}

function openRelationModal(relation = null) {
    const modal = document.getElementById('relationModal');
    const title = document.getElementById('relationModalTitle');

    // Populate dropdowns first so we can set values later
    populateEntityDropdowns();
    populateRelationTypes();

    if (relation) {
        title.innerHTML = '<i class="fas fa-edit"></i> Modify the Relationship';
        // Populate form with relation data
        document.getElementById('sourceEntity').value = relation.source_entity_id;
        document.getElementById('targetEntity').value = relation.target_entity_id;
        document.getElementById('relationType').value = relation.relationship_type_id;
        document.getElementById('relationStrength').value = relation.strength || 5;
        document.getElementById('relationDescription').value = relation.description || '';
        document.getElementById('relationEvidence').value = relation.evidence || '';
        document.getElementById('bidirectional').checked = relation.bidirectional || false;

        state.editingRelationId = relation.id;
    } else {
        title.innerHTML = '<i class="fas fa-link"></i> New Relationship';
        // Clear form
        document.getElementById('sourceEntity').value = '';
        document.getElementById('targetEntity').value = '';
        document.getElementById('relationType').value = '';
        document.getElementById('relationStrength').value = 5;
        document.getElementById('relationDescription').value = '';
        document.getElementById('relationEvidence').value = '';
        document.getElementById('bidirectional').checked = false;

        state.editingRelationId = null;
    }

    modal.classList.add('active');
}

function populateRelationTypes() {
    const typeSelect = document.getElementById('relationType');
    const currentStructure = typeSelect.innerHTML;

    // Only repopulate if empty or just has placeholder (simple check)
    // Actually simpler to just rebuild options based on known types

    typeSelect.innerHTML = '<option value="">Select a type...</option>';

    // We can use getRelationshipType helper to get standard types
    // Since getRelationshipType takes an ID and returns object, we need source of truth for all types
    // Based on app.js analysis, `getRelationshipType` has a hardcoded list inside. 
    // We should extract that or reuse it. 
    // For now, let's replicate the keys 1-8 which are hardcoded in getRelationshipType

    const types = [
        { id: 1, name: 'Knows' },
        { id: 2, name: 'Works with' },
        { id: 3, name: 'Family' },
        { id: 4, name: 'Communication' },
        { id: 5, name: 'Finance' },
        { id: 6, name: 'Property' },
        { id: 7, name: 'Location' },
        { id: 8, name: 'Document' }
    ];

    types.forEach(type => {
        typeSelect.innerHTML += `<option value="${type.id}">${type.name}</option>`;
    });
}

function populateEntityDropdowns() {
    const sourceSelect = document.getElementById('sourceEntity');
    const targetSelect = document.getElementById('targetEntity');

    sourceSelect.innerHTML = '<option value="">Select an entity...</option>';
    targetSelect.innerHTML = '<option value="">Select an entity...</option>';

    state.entities.forEach(entity => {
        const option = `<option value="${entity.id}">${entity.name} (${entity.type})</option>`;
        sourceSelect.innerHTML += option;
        targetSelect.innerHTML += option;
    });
}

async function saveRelation() {
    const sourceId = document.getElementById('sourceEntity').value;
    const targetId = document.getElementById('targetEntity').value;
    const typeId = document.getElementById('relationType').value;
    const strength = document.getElementById('relationStrength').value;
    const description = document.getElementById('relationDescription').value.trim();
    const evidence = document.getElementById('relationEvidence').value.trim();
    const bidirectional = document.getElementById('bidirectional').checked;

    if (!sourceId || !targetId || !typeId) {
        showNotification('Please fill in all required fields', 'error');
        return;
    }

    if (sourceId === targetId) {
        showNotification('The source and target entities cannot be identical', 'error');
        return;
    }

    try {
        const relationData = {
            source_entity_id: sourceId,
            target_entity_id: targetId,
            relationship_type_id: typeId,
            strength: parseInt(strength),
            description,
            evidence,
            bidirectional
        };

        let response;
        if (state.editingRelationId) {
            response = await fetch(`/api/relationships/${state.editingRelationId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(relationData)
            });
        } else {
            relationData.project_id = state.currentProject.id;
            response = await fetch('/api/relationships', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(relationData)
            });
        }

        if (response.ok) {
            await loadRelationships();

            // Reload project data to ensure everything is in sync (useful for bidirectional or side effects)
            // But loadRelationships updates local state.relationships.

            // Force redraw of table if modal is likely open (or just always if it exists)
            renderRelationsTable();

            closeModal('relationModal');
            showNotification('Relationship successfully registered', 'success');

            // Update graph always, so if we are in data view and switch back, it's ready.
            // But wait, renderGraph uses state.entities and state.relationships.
            // renderGraph() rebuilds nodes and edges.
            if (state.network) {
                renderGraph();
            }
        } else {
            showNotification('Error during registration', 'error');
        }
    } catch (error) {
        console.error('Error saving relation:', error);
        showNotification('Error during registration', 'error');
    }
}

async function openRelationsPanel() {
    const modal = document.getElementById('relationsPanelModal');
    modal.classList.add('active');

    const searchInput = document.getElementById('searchRelations');
    if (searchInput) {
        searchInput.value = '';
    }

    // Show loading state
    const tbody = document.querySelector('#relationsTable tbody');
    if (tbody) {
        tbody.innerHTML = '<tr><td colspan="5" class="text-center"><div class="loading"><i class="fas fa-spinner fa-spin"></i> Loading...</div></td></tr>';
    }

    await loadRelationships();
    renderRelationsTable();
}

function renderRelationsTable() {
    const tbody = document.querySelector('#relationsTable tbody');
    if (!tbody) return;

    tbody.innerHTML = '';

    if (relationshipsData.length === 0) {
        tbody.innerHTML = `
            <tr class="no-data-row">
                <td colspan="5" class="text-center">
                    <div class="no-data-message">
                        <i class="fas fa-link"></i>
                        <p>No defined relationship</p>
                    </div>
                </td>
            </tr>
        `;
        return;
    }

    relationshipsData.forEach(rel => {
        const sourceEntity = state.entities.find(e => e.id === rel.source_entity_id);
        const targetEntity = state.entities.find(e => e.id === rel.target_entity_id);
        const type = getRelationshipType(rel.relationship_type_id);

        const row = document.createElement('tr');
        row.innerHTML = `
            <td data-search="${sourceEntity?.name?.toLowerCase() || ''} ${sourceEntity?.type?.toLowerCase() || ''}">
                <div class="entity-cell">
                    <span class="entity-badge" style="background: ${getEntityColor(sourceEntity?.type)}">
                        ${getEntityIcon(sourceEntity?.type)} ${sourceEntity?.name || 'Unknown'}
                    </span>
                </div>
            </td>
            <td data-search="${type.name?.toLowerCase() || ''} ${rel.description?.toLowerCase() || ''}">
                <div class="relation-cell">
                    <span class="relation-badge" style="background: ${type.color}">
                        ${type.name}
                    </span>
                    ${rel.description ? `<small class="text-muted">${rel.description}</small>` : ''}
                </div>
            </td>
            <td data-search="${targetEntity?.name?.toLowerCase() || ''} ${targetEntity?.type?.toLowerCase() || ''}">
                <div class="entity-cell">
                    <span class="entity-badge" style="background: ${getEntityColor(targetEntity?.type)}">
                        ${getEntityIcon(targetEntity?.type)} ${targetEntity?.name || 'Unknown'}
                    </span>
                </div>
            </td>
            <td>
                <div class="strength-indicator">
                    <div class="strength-bar" style="width: ${rel.strength * 10}%"></div>
                    <span>${rel.strength}/10</span>
                </div>
            </td>
            <td>
                <div class="action-buttons">
                    <button class="btn-icon btn-sm btn-edit-relation" data-id="${rel.id}" title="To modify">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn-icon btn-sm btn-danger btn-delete-relation" data-id="${rel.id}" title="DELETE">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </td>
        `;
        tbody.appendChild(row);
    });

    // Add event listeners
    tbody.querySelectorAll('.btn-edit-relation').forEach(btn => {
        btn.addEventListener('click', (e) => {
            e.stopPropagation();
            const id = parseInt(btn.dataset.id);
            editRelation(id);
        });
    });

    tbody.querySelectorAll('.btn-delete-relation').forEach(btn => {
        btn.addEventListener('click', (e) => {
            e.stopPropagation();
            const id = parseInt(btn.dataset.id);
            deleteRelation(id);
        });
    });
}

async function editRelation(relationId) {
    const relation = relationshipsData.find(r => r.id === relationId);
    if (relation) {
        closeModal('relationsPanelModal');
        setTimeout(() => openRelationModal(relation), 300);
    }
}

async function deleteRelation(relationId) {
    if (!confirm('Are you sure you want to end this relationship?')) return;

    try {
        const response = await fetch(`/api/relationships/${relationId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            await loadRelationships();
            renderRelationsTable();
            showNotification('Relationship successfully deleted', 'success');

            // Update graph
            if (state.currentView === 'graph') {
                renderGraph();
            }
        }
    } catch (error) {
        console.error('Error deleting relation:', error);
        showNotification('Error during deletion', 'error');
    }
}

document.addEventListener('DOMContentLoaded', () => {
    // Add these to your existing setupEventListeners function
    document.getElementById('manageRelationsBtn')?.addEventListener('click', openRelationsPanel);
    document.getElementById('addNewRelationBtn')?.addEventListener('click', () => openRelationModal());
    document.getElementById('saveRelationBtn')?.addEventListener('click', saveRelation);

    // Strength slider value display
    const strengthSlider = document.getElementById('relationStrength');
    const strengthValue = document.getElementById('strengthValue');
    if (strengthSlider && strengthValue) {
        strengthSlider.addEventListener('input', (e) => {
            strengthValue.textContent = e.target.value;
        });
    }
});