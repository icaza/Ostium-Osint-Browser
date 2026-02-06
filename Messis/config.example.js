// Configuration avancée de Messis
// Copiez ce fichier en config.js et modifiez selon vos besoins

module.exports = {
    // Configuration du serveur
    server: {
        port: 3000,
        host: 'localhost',
        
        // Activer HTTPS (nécessite certificats SSL)
        https: {
            enabled: false,
            keyPath: './certs/key.pem',
            certPath: './certs/cert.pem'
        }
    },
    
    // Base de données
    database: {
        filename: 'messis.db',
        
        // Options SQLite3
        options: {
            // Mode lecture seule
            readonly: false,
            
            // Timeout pour les opérations (ms)
            timeout: 5000
        }
    },
    
    // Sécurité
    security: {
        // Rate limiting
        rateLimit: {
            windowMs: 15 * 60 * 1000, // 15 minutes
            max: 1000 // requêtes max par fenêtre
        },
        
        // CORS (Cross-Origin Resource Sharing)
        cors: {
            enabled: false,
            origin: '*' // ou domaine spécifique
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
        // Taille maximale des exports (bytes)
        maxSize: 50 * 1024 * 1024, // 50 MB
        
        // Formats activés
        formats: {
            json: true,
            csv: true,
            markdown: true,
            html: true,
            txt: true,
            pdf: true
        },
        
        // Options PDF
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
    
    // Interface utilisateur
    ui: {
        // Thème par défaut
        theme: 'dark', // 'dark' ou 'light'
        
        // Langue par défaut
        language: 'en',
        
        // Nombre d'entités par page (vue données)
        itemsPerPage: 20,
        
        // Options du graphe
        graph: {
            defaultLayout: 'force', // 'force', 'hierarchical', 'circular'
            
            physics: {
                enabled: true,
                stabilization: {
                    iterations: 100
                }
            },
            
            // Couleurs personnalisées par type
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
        // Niveau de log : 'error', 'warn', 'info', 'debug'
        level: 'info',
        
        // Enregistrer dans un fichier
        file: {
            enabled: false,
            path: './logs/messis.log'
        }
    },
    
    // Sauvegarde automatique
    backup: {
        enabled: false,
        
        // Intervalle de sauvegarde (minutes)
        interval: 60,
        
        // Dossier de sauvegarde
        path: './backups',
        
        // Nombre de sauvegardes à conserver
        keep: 10
    }
};
