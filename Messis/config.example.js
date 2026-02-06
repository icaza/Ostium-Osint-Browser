// Advanced Messis configuration
// Copy this file to config.js and modify it as needed

module.exports = {
    // Server configuration
    server: {
        port: 3000,
        host: 'localhost',
        
        // Enable HTTPS (requires SSL certificates)
        https: {
            enabled: false,
            keyPath: './certs/key.pem',
            certPath: './certs/cert.pem'
        }
    },
    
    // Database
    database: {
        filename: 'messis.db',
        
        // SQLite3 Options
        options: {
            // Read-only mode
            readonly: false,
            
            // Timeout for operations (ms)
            timeout: 5000
        }
    },
    
    // Security
    security: {
        // Rate limiting
        rateLimit: {
            windowMs: 15 * 60 * 1000, // 15 minutes
            max: 1000 // max queries per window
        },
        
        // CORS (Cross-Origin Resource Sharing)
        cors: {
            enabled: false,
            origin: '*' // or specific area
        },
        
        // Content Security Policy
        csp: {
            enabled: true,
            directives: {
                defaultSrc: ["'self'"],
                scriptSrc: ["'self'", "'unsafe-inline'", "https://unpkg.com", "https://cdnjs.cloudflare.com"],
                styleSrc: ["'self'", "'unsafe-inline'", "https://cdnjs.cloudflare.com"],
                fontSrc: ["'self'", "https://cdnjs.cloudflare.com"]
            }
        }
    },
    
    // Export
    export: {
        // Maximum export size (bytes)
        maxSize: 50 * 1024 * 1024, // 50 MB
        
        // Enabled formats
        formats: {
            json: true,
            csv: true,
            markdown: true,
            html: true,
            txt: true,
            pdf: true
        },
        
        // PDF Options
        pdf: {
            format: 'A4',
            margin: {
                top: '20mm',
                right: '20mm',
                bottom: '20mm',
                left: '20mm'
            }
        }
    },
    
    // User interface
    ui: {
        // Default theme
        theme: 'dark', // 'dark' ou 'light'
        
        // Default language
        language: 'en',
        
        // Number of entities per page (data view)
        itemsPerPage: 20,
        
        // Graph options
        graph: {
            defaultLayout: 'force', // 'force', 'hierarchical', 'circular'
            
            physics: {
                enabled: true,
                stabilization: {
                    iterations: 100
                }
            },
            
            // Custom colors by type
            colors: {
                'Personne': '#2563eb',
                'Organisation': '#dc2626',
                'Localisation': '#16a34a',
                'Document': '#ea580c',
                'Événement': '#ca8a04',
                'Site Web': '#7c3aed',
                'Email': '#0891b2',
                'Téléphone': '#059669',
                'Véhicule': '#4f46e5',
                'Autre': '#9333ea'
            }
        }
    },
    
    // Logging
    logging: {
        // Log level : 'error', 'warn', 'info', 'debug'
        level: 'info',
        
        // Save to a file
        file: {
            enabled: false,
            path: './logs/messis.log'
        }
    },
    
    // Automatic backup
    backup: {
        enabled: false,
        
        // Backup interval (minutes)
        interval: 60,
        
        // Backup folder
        path: './backups',
        
        // Number of backups to keep
        keep: 10
    }
};
