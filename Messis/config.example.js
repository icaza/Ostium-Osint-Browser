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
        filename: './db/messis.db',

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
            windowMs: 60 * 1000, // 1 minutes
            max: 1000 
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
                'Person': '#2563eb',
                'Identification': '#8b5cf6',
                'Contact': '#0891b2',
                'Location': '#16a34a',
                'Social Networks': '#ec4899',
                'Professional': '#f59e0b',
                'Financial': '#10b981',
                'Technical': '#6366f1',
                'Organization': '#ff0000',
                'Document': '#ea580c',
                'Media': '#fc38e2',
                'Event': '#ca8a04',
                'Website': '#00fff2',
                'E-mail': '#0891b2',
                'Phone': '#059669',
                'Vehicle': '#00ff40',
                'Other': '#c8ff00'                
            }
        }
    },
    
    // Logging
    logging: {
        // Log level : 'error', 'warn', 'info', 'debug'
        level: 'info',
        
        // Save to a file
        file: {
            enabled: true,
            path: './logs/messis.log'
        }
    },
    
    // Automatic backup
    backup: {
        enabled: false,
        
        // Backup interval (minutes)
        interval: 3,
        
        // Backup folder
        path: './backups',
        
        // Number of backups to keep
        keep: 10
    }
};
