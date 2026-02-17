const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const helmet = require('helmet');
const rateLimit = require('express-rate-limit');
const sanitizeHtml = require('sanitize-html');
const path = require('path');
const fs = require('fs');
const PDFDocument = require('pdfkit');
const { marked } = require('marked');
const config = require('./config');
const logger = require('./logger');

const app = express();
const PORT = config.server.port || 3000;

// Security middleware
const helmetOptions = {};

if (config.security.csp && config.security.csp.enabled) {
    helmetOptions.contentSecurityPolicy = {
        directives: config.security.csp.directives
    };
}

app.use(helmet(helmetOptions));

// Rate limiting
const limiter = rateLimit({
    windowMs: config.security.rateLimit.windowMs,
    max: config.security.rateLimit.max
});
app.use(limiter);

// Middleware
app.use(express.json({ limit: '50mb' }));
app.use(express.static('public'));

// Database initialization
const dbPath = config.database.filename || './db/messis.db';
const db = new sqlite3.Database(dbPath, (err) => {
    if (err) {
        logger.error('Database connection error:', err);
    } else {
        logger.info('✓ Connected to SQLite database');
        initializeDatabase();
    }
});

// Initialize database schema
function initializeDatabase() {
    db.serialize(() => {
        // Projects table
        db.run(`CREATE TABLE IF NOT EXISTS projects (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            description TEXT,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
        )`);

        // Entities table (main subjects of investigation)
        db.run(`CREATE TABLE IF NOT EXISTS entities (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            project_id INTEGER NOT NULL,
            type TEXT NOT NULL,
            name TEXT NOT NULL,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (project_id) REFERENCES projects(id) ON DELETE CASCADE
        )`);

        // Attributes table (flexible data storage)
        db.run(`CREATE TABLE IF NOT EXISTS attributes (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            entity_id INTEGER NOT NULL,
            category TEXT NOT NULL,
            label TEXT NOT NULL,
            value TEXT NOT NULL,
            notes TEXT,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (entity_id) REFERENCES entities(id) ON DELETE CASCADE
        )`);

        // Relationship types table
        db.run(`CREATE TABLE IF NOT EXISTS relationship_types (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            description TEXT,
            color TEXT DEFAULT '#64748b',
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP
        )`);

        // Relationships table (connections between entities)
        db.run(`CREATE TABLE IF NOT EXISTS relationships (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            project_id INTEGER NOT NULL,
            source_entity_id INTEGER NOT NULL,
            target_entity_id INTEGER NOT NULL,
            relationship_type_id INTEGER NOT NULL,
            description TEXT,
            strength INTEGER DEFAULT 5 CHECK (strength BETWEEN 1 AND 10),
            bidirectional INTEGER DEFAULT 0, -- SQLite uses INTEGER for booleans (0=false, 1=true)
            evidence TEXT,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (project_id) REFERENCES projects(id) ON DELETE CASCADE,
            FOREIGN KEY (source_entity_id) REFERENCES entities(id) ON DELETE CASCADE,
            FOREIGN KEY (target_entity_id) REFERENCES entities(id) ON DELETE CASCADE,
            FOREIGN KEY (relationship_type_id) REFERENCES relationship_types(id) ON DELETE CASCADE
        )`);

        // Insert default relationship types
        db.run(`INSERT OR IGNORE INTO relationship_types (id, name, description, color) VALUES
            (1, 'Knows', 'Personal knowledge', '#3b82f6'),
            (2, 'Works with', 'Professional relationship', '#10b981'),
            (3, 'Family', 'Family ties', '#8b5cf6'),
            (4, 'Communication', 'Regular communication', '#f59e0b'),
            (5, 'Finance', 'Financial link', '#ef4444'),
            (6, 'Property', 'Ownership relationship', '#6366f1'),
            (7, 'Location', 'Same location', '#84cc16'),
            (8, 'Document', 'Mention in document', '#ec4899')`);

        logger.info('✓ Database schema initialized');
    });
}

// Sanitize input
function sanitize(input) {
    if (typeof input === 'string') {
        return sanitizeHtml(input, {
            allowedTags: [],
            allowedAttributes: {}
        });
    }
    return input;
}

// ==================== PROJECTS ROUTES ====================

// Get configuration (UI settings only)
app.get('/api/config', (req, res) => {
    res.json(config.ui);
});

// Get all projects
app.get('/api/projects', (req, res) => {
    db.all('SELECT * FROM projects ORDER BY updated_at DESC', [], (err, rows) => {
        if (err) {
            res.status(500).json({ error: err.message });
            return;
        }
        res.json(rows);
    });
});

// Get single project
app.get('/api/projects/:id', (req, res) => {
    const id = parseInt(req.params.id);
    db.get('SELECT * FROM projects WHERE id = ?', [id], (err, row) => {
        if (err) {
            res.status(500).json({ error: err.message });
            return;
        }
        if (!row) {
            res.status(404).json({ error: 'Project not found' });
            return;
        }
        res.json(row);
    });
});

// Create project
app.post('/api/projects', (req, res) => {
    const { name, description } = req.body;
    const safeName = sanitize(name);
    const safeDesc = sanitize(description);

    db.run(
        'INSERT INTO projects (name, description) VALUES (?, ?)',
        [safeName, safeDesc],
        function (err) {
            if (err) {
                res.status(500).json({ error: err.message });
                return;
            }
            res.json({ id: this.lastID, name: safeName, description: safeDesc });
        }
    );
});

// Update project
app.put('/api/projects/:id', (req, res) => {
    const id = parseInt(req.params.id);
    const { name, description } = req.body;
    const safeName = sanitize(name);
    const safeDesc = sanitize(description);

    db.run(
        'UPDATE projects SET name = ?, description = ?, updated_at = CURRENT_TIMESTAMP WHERE id = ?',
        [safeName, safeDesc, id],
        function (err) {
            if (err) {
                res.status(500).json({ error: err.message });
                return;
            }
            res.json({ success: true, changes: this.changes });
        }
    );
});

// Delete project
app.delete('/api/projects/:id', (req, res) => {
    const id = parseInt(req.params.id);
    db.run('DELETE FROM projects WHERE id = ?', [id], function (err) {
        if (err) {
            res.status(500).json({ error: err.message });
            return;
        }
        res.json({ success: true, changes: this.changes });
    });
});

// ==================== ENTITIES ROUTES ====================

// Get entities for project
app.get('/api/projects/:projectId/entities', (req, res) => {
    const projectId = parseInt(req.params.projectId);
    db.all('SELECT * FROM entities WHERE project_id = ?', [projectId], (err, rows) => {
        if (err) {
            res.status(500).json({ error: err.message });
            return;
        }
        res.json(rows);
    });
});

// Create entity
app.post('/api/entities', (req, res) => {
    const { project_id, type, name } = req.body;
    const safeName = sanitize(name);
    const safeType = sanitize(type);

    db.run(
        'INSERT INTO entities (project_id, type, name) VALUES (?, ?, ?)',
        [project_id, safeType, safeName],
        function (err) {
            if (err) {
                res.status(500).json({ error: err.message });
                return;
            }
            res.json({ id: this.lastID, project_id, type: safeType, name: safeName });
        }
    );
});

// Update entity
app.put('/api/entities/:id', (req, res) => {
    const id = parseInt(req.params.id);
    const { type, name } = req.body;
    const safeName = sanitize(name);
    const safeType = sanitize(type);

    db.run(
        'UPDATE entities SET type = ?, name = ? WHERE id = ?',
        [safeType, safeName, id],
        function (err) {
            if (err) {
                res.status(500).json({ error: err.message });
                return;
            }
            res.json({ success: true, changes: this.changes });
        }
    );
});

// Delete entity
app.delete('/api/entities/:id', (req, res) => {
    const id = parseInt(req.params.id);
    db.run('DELETE FROM entities WHERE id = ?', [id], function (err) {
        if (err) {
            res.status(500).json({ error: err.message });
            return;
        }
        res.json({ success: true, changes: this.changes });
    });
});

// ==================== ATTRIBUTES ROUTES ====================

// Get attributes for entity
app.get('/api/entities/:entityId/attributes', (req, res) => {
    const entityId = parseInt(req.params.entityId);
    db.all('SELECT * FROM attributes WHERE entity_id = ?', [entityId], (err, rows) => {
        if (err) {
            res.status(500).json({ error: err.message });
            return;
        }
        res.json(rows);
    });
});

// Create attribute
app.post('/api/attributes', (req, res) => {
    const { entity_id, category, label, value, notes } = req.body;
    const safeCategory = sanitize(category);
    const safeLabel = sanitize(label);
    const safeValue = sanitize(value);
    const safeNotes = sanitize(notes);

    db.run(
        'INSERT INTO attributes (entity_id, category, label, value, notes) VALUES (?, ?, ?, ?, ?)',
        [entity_id, safeCategory, safeLabel, safeValue, safeNotes],
        function (err) {
            if (err) {
                res.status(500).json({ error: err.message });
                return;
            }
            res.json({
                id: this.lastID,
                entity_id,
                category: safeCategory,
                label: safeLabel,
                value: safeValue,
                notes: safeNotes
            });
        }
    );
});

// Update attribute
app.put('/api/attributes/:id', (req, res) => {
    const id = parseInt(req.params.id);
    const { category, label, value, notes } = req.body;
    const safeCategory = sanitize(category);
    const safeLabel = sanitize(label);
    const safeValue = sanitize(value);
    const safeNotes = sanitize(notes);

    db.run(
        'UPDATE attributes SET category = ?, label = ?, value = ?, notes = ? WHERE id = ?',
        [safeCategory, safeLabel, safeValue, safeNotes, id],
        function (err) {
            if (err) {
                res.status(500).json({ error: err.message });
                return;
            }
            res.json({ success: true, changes: this.changes });
        }
    );
});

// Delete attribute
app.delete('/api/attributes/:id', (req, res) => {
    const id = parseInt(req.params.id);
    db.run('DELETE FROM attributes WHERE id = ?', [id], function (err) {
        if (err) {
            res.status(500).json({ error: err.message });
            return;
        }
        res.json({ success: true, changes: this.changes });
    });
});

// ==================== RELATIONSHIPS ROUTES ====================

// Get relationships for project
app.get('/api/projects/:projectId/relationships', (req, res) => {
    const projectId = parseInt(req.params.projectId);
    db.all('SELECT * FROM relationships WHERE project_id = ?', [projectId], (err, rows) => {
        if (err) {
            res.status(500).json({ error: err.message });
            return;
        }
        res.json(rows);
    });
});

// Create relationship
app.post('/api/relationships', (req, res) => {
    const { project_id, source_entity_id, target_entity_id, relationship_type_id, description, strength, evidence, bidirectional } = req.body;
    const safeDesc = sanitize(description);
    const safeEvidence = sanitize(evidence);

    db.run(
        'INSERT INTO relationships (project_id, source_entity_id, target_entity_id, relationship_type_id, description, strength, evidence, bidirectional) VALUES (?, ?, ?, ?, ?, ?, ?, ?)',
        [project_id, source_entity_id, target_entity_id, relationship_type_id, safeDesc, strength, safeEvidence, bidirectional ? 1 : 0],
        function (err) {
            if (err) {
                res.status(500).json({ error: err.message });
                return;
            }
            res.json({ id: this.lastID });
        }
    );
});

// Update relationship
app.put('/api/relationships/:id', (req, res) => {
    const id = parseInt(req.params.id);
    const { source_entity_id, target_entity_id, relationship_type_id, description, strength, evidence, bidirectional } = req.body;
    const safeDesc = sanitize(description);
    const safeEvidence = sanitize(evidence);

    db.run(
        'UPDATE relationships SET source_entity_id = ?, target_entity_id = ?, relationship_type_id = ?, description = ?, strength = ?, evidence = ?, bidirectional = ?, updated_at = CURRENT_TIMESTAMP WHERE id = ?',
        [source_entity_id, target_entity_id, relationship_type_id, safeDesc, strength, safeEvidence, bidirectional ? 1 : 0, id],
        function (err) {
            if (err) {
                res.status(500).json({ error: err.message });
                return;
            }
            res.json({ success: true, changes: this.changes });
        }
    );
});

// Delete relationship
app.delete('/api/relationships/:id', (req, res) => {
    const id = parseInt(req.params.id);
    db.run('DELETE FROM relationships WHERE id = ?', [id], function (err) {
        if (err) {
            res.status(500).json({ error: err.message });
            return;
        }
        res.json({ success: true, changes: this.changes });
    });
});

// ==================== EXPORT ROUTES ====================

// Get complete project data
app.get('/api/projects/:projectId/export-data', async (req, res) => {
    const projectId = parseInt(req.params.projectId);

    try {
        const project = await new Promise((resolve, reject) => {
            db.get('SELECT * FROM projects WHERE id = ?', [projectId], (err, row) => {
                if (err) reject(err);
                else resolve(row);
            });
        });

        const entities = await new Promise((resolve, reject) => {
            db.all('SELECT * FROM entities WHERE project_id = ?', [projectId], (err, rows) => {
                if (err) reject(err);
                else resolve(rows);
            });
        });

        const attributes = await new Promise((resolve, reject) => {
            db.all(`
                SELECT a.* FROM attributes a
                INNER JOIN entities e ON a.entity_id = e.id
                WHERE e.project_id = ?
            `, [projectId], (err, rows) => {
                if (err) reject(err);
                else resolve(rows);
            });
        });

        const relationships = await new Promise((resolve, reject) => {
            db.all('SELECT * FROM relationships WHERE project_id = ?', [projectId], (err, rows) => {
                if (err) reject(err);
                else resolve(rows);
            });
        });

        res.json({
            project,
            entities,
            attributes,
            relationships
        });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
});

// Export as PDF
app.get('/api/projects/:projectId/export/pdf', async (req, res) => {
    const projectId = parseInt(req.params.projectId);

    try {
        const data = await new Promise((resolve, reject) => {
            db.get('SELECT * FROM projects WHERE id = ?', [projectId], async (err, project) => {
                if (err) {
                    reject(err);
                    return;
                }

                const entities = await new Promise((res2, rej2) => {
                    db.all('SELECT * FROM entities WHERE project_id = ?', [projectId], (err2, rows) => {
                        if (err2) rej2(err2);
                        else res2(rows);
                    });
                });

                const attributes = await new Promise((res2, rej2) => {
                    db.all(`
                        SELECT a.* FROM attributes a
                        INNER JOIN entities e ON a.entity_id = e.id
                        WHERE e.project_id = ?
                    `, [projectId], (err2, rows) => {
                        if (err2) rej2(err2);
                        else res2(rows);
                    });
                });

                const relationships = await new Promise((res2, rej2) => {
                    db.all(`
                        SELECT r.* FROM relationships r
                        INNER JOIN entities e ON (r.source_entity_id = e.id OR r.target_entity_id = e.id)
                        WHERE e.project_id = ?
                        GROUP BY r.id
                    `, [projectId], (err2, rows) => {
                        if (err2) rej2(err2);
                        else res2(rows);
                    });
                });

                resolve({ project, entities, attributes, relationships });
            });
        });

        const doc = new PDFDocument({ margin: 40 });
        res.setHeader('Content-Type', 'application/pdf');
        res.setHeader('Content-Disposition', `attachment; filename=messis_${projectId}.pdf`);

        doc.pipe(res);

        // Title page
        doc.fontSize(24).font('Helvetica-Bold').text(data.project.name, { align: 'center' });
        doc.moveDown(0.5);
        doc.fontSize(12).font('Helvetica').text('OSINT Investigation Report', { align: 'center' });
        doc.moveDown(2);

        if (data.project.description) {
            doc.fontSize(11).text(data.project.description, { align: 'center' });
            doc.moveDown(2);
        }

        // Statistics
        doc.fontSize(10).fillColor('#666666');
        doc.text(`Total Entities: ${data.entities.length}`, { indent: 20 });
        doc.text(`Total Relationships: ${data.relationships.length}`, { indent: 20 });
        doc.text(`Total Attributes: ${data.attributes.length}`, { indent: 20 });
        doc.fillColor('black');
        doc.moveDown(2);

        // Entities Section
        doc.fontSize(16).font('Helvetica-Bold').text('ENTITIES', { underline: true });
        doc.moveDown(1);

        data.entities.forEach((entity, index) => {
            // Check if we need a new page
            if (doc.y > doc.page.height - 100) {
                doc.addPage();
            }

            doc.fontSize(13).font('Helvetica-Bold').fillColor('#2563eb').text(`${entity.type}: ${entity.name}`);
            doc.fillColor('black').font('Helvetica');
            doc.moveDown(0.3);

            const entityAttrs = data.attributes.filter(a => a.entity_id === entity.id);
            
            if (entityAttrs.length > 0) {
                doc.fontSize(10).fillColor('#666666').text('Attributes:', { indent: 10 });
                entityAttrs.forEach(attr => {
                    doc.fontSize(9).text(`* ${attr.label}: ${attr.value}`, { indent: 20 });
                    if (attr.category) {
                        doc.fontSize(8).fillColor('#999999').text(`  Category: ${attr.category}`, { indent: 25 });
                    }
                    if (attr.notes) {
                        doc.fontSize(8).text(`  Notes: ${attr.notes}`, { indent: 25 });
                    }
                });
            }

            // Relationships for this entity
            const entityRels = data.relationships.filter(r => 
                r.source_entity_id === entity.id || r.target_entity_id === entity.id
            );

            if (entityRels.length > 0) {
                doc.fillColor('black').fontSize(10).text('Relations:', { indent: 10 });
                entityRels.forEach(rel => {
                    // Check if we need a new page
                    if (doc.y > doc.page.height - 50) {
                        doc.addPage();
                    }

                    const isSource = rel.source_entity_id === entity.id;
                    const otherEntityId = isSource ? rel.target_entity_id : rel.source_entity_id;
                    const otherEntity = data.entities.find(e => e.id === otherEntityId);
                    const arrow = rel.bidirectional ? '<->' : (isSource ? '->' : '<-');

                    if (otherEntity) {
                        doc.fontSize(9).fillColor('#10b981');
                        doc.text(`* ${entity.name} ${arrow} ${otherEntity.name} (${rel.relationship_type})`, 
                                { indent: 20 });
                        doc.fillColor('black').fontSize(8);
                        if (rel.description) {
                            doc.text(`Description: ${rel.description}`, { indent: 25 });
                        }
                        doc.text(`Strength: ${rel.strength || 5}/10`, { indent: 25 });
                        if (rel.evidence) {
                            doc.text(`Evidence: ${rel.evidence}`, { indent: 25 });
                        }
                        if (rel.bidirectional) {
                            doc.fillColor('#f59e0b').text('Bidirectional: Yes', { indent: 25 });
                        }
                        doc.fillColor('black');
                    }
                });
            }

            doc.moveDown(0.5);
        });

        // Relationships Summary Table
        if (data.relationships && data.relationships.length > 0) {
            doc.addPage();
            doc.fontSize(16).font('Helvetica-Bold').fillColor('#2563eb').text('RELATIONSHIPS SUMMARY', { underline: true });
            doc.fillColor('black');
            doc.moveDown(1);

            // Create a simple table with relationship details
            const tableTop = doc.y;
            const col1 = 40;
            const col2 = 140;
            const col3 = 240;
            const col4 = 340;
            const col5 = 400;
            const col6 = 450;
            const rowHeight = 25;

            // Table header
            doc.fontSize(8).font('Helvetica-Bold');
            doc.rect(col1, tableTop, 100, rowHeight).stroke();
            doc.text('Source', col1 + 5, tableTop + 7, { width: 90 });
            
            doc.rect(col2, tableTop, 100, rowHeight).stroke();
            doc.text('Relation', col2 + 5, tableTop + 7, { width: 90 });
            
            doc.rect(col3, tableTop, 100, rowHeight).stroke();
            doc.text('Target', col3 + 5, tableTop + 7, { width: 90 });
            
            doc.rect(col4, tableTop, 50, rowHeight).stroke();
            doc.text('Force', col4 + 5, tableTop + 7, { width: 40 });
            
            doc.rect(col5, tableTop, 50, rowHeight).stroke();
            doc.text('Evid', col5 + 5, tableTop + 7, { width: 40 });

            // Table rows
            let currentY = tableTop + rowHeight;
            doc.font('Helvetica');
            doc.fontSize(7);

            data.relationships.forEach(rel => {
                const sourceEntity = data.entities.find(e => e.id === rel.source_entity_id);
                const targetEntity = data.entities.find(e => e.id === rel.target_entity_id);

                // Check if we need a new page
                if (currentY > doc.page.height - 50) {
                    doc.addPage();
                    currentY = 50;
                    
                    // Redraw table header on new page
                    doc.fontSize(8).font('Helvetica-Bold');
                    doc.rect(col1, currentY, 100, rowHeight).stroke();
                    doc.text('Source', col1 + 5, currentY + 7, { width: 90 });
                    
                    doc.rect(col2, currentY, 100, rowHeight).stroke();
                    doc.text('Relation', col2 + 5, currentY + 7, { width: 90 });
                    
                    doc.rect(col3, currentY, 100, rowHeight).stroke();
                    doc.text('Target', col3 + 5, currentY + 7, { width: 90 });
                    
                    doc.rect(col4, currentY, 50, rowHeight).stroke();
                    doc.text('Force', col4 + 5, currentY + 7, { width: 40 });
                    
                    doc.rect(col5, currentY, 50, rowHeight).stroke();
                    doc.text('Evid', col5 + 5, currentY + 7, { width: 40 });

                    currentY += rowHeight;
                    doc.fontSize(7).font('Helvetica');
                }

                doc.rect(col1, currentY, 100, rowHeight).stroke();
                doc.text((sourceEntity?.name || '?').substring(0, 12), col1 + 5, currentY + 7, { width: 90 });

                doc.rect(col2, currentY, 100, rowHeight).stroke();
                doc.text((rel.relationship_type || '?').substring(0, 12), col2 + 5, currentY + 7, { width: 90 });

                doc.rect(col3, currentY, 100, rowHeight).stroke();
                doc.text((targetEntity?.name || '?').substring(0, 12), col3 + 5, currentY + 7, { width: 90 });

                doc.rect(col4, currentY, 50, rowHeight).stroke();
                doc.text(((rel.strength || 5) + '/10').substring(0, 6), col4 + 5, currentY + 7, { width: 40 });

                doc.rect(col5, currentY, 50, rowHeight).stroke();
                doc.text(rel.evidence ? 'Yes' : 'No', col5 + 10, currentY + 7, { width: 30 });

                currentY += rowHeight;
            });
        }

        doc.end();
    } catch (err) {
        logger.error('PDF export error:', err);
        res.status(500).json({ error: err.message });
    }
});

// Start server
app.listen(PORT, () => {
    logger.info(`\nMessis OSINT Tool by ICAZA MEDIA`);
    logger.info(`✓ Server running on http://localhost:${PORT}`);
    logger.info(`✓ Database: messis.db`);
    logger.info(`\nPress Ctrl+C to stop\n`);
});

// Graceful shutdown
process.on('SIGINT', () => {
    db.close((err) => {
        if (err) {
            logger.error('Error closing database:', err);
        } else {
            logger.info('\n✓ Database connection closed');
        }
        process.exit(0);
    });
});
