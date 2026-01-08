/**
 * @file Domain_Infos.js
 * @author ICAZA <github.wildly512@passinbox.com>
 * @version 2.1
 * @description Display secure domain information with improved safety and performance
 * @license MIT
 */

; (function () {
    'use strict';

    /**
     * Configuration object
     */
    const CONFIG = {
        CONTAINER_ID: 'domainInfoContainer',
        TABLE_CLASS: 'domain-info-table',
        HIDDEN_VALUE: '[Hidden for security]',
        SENSITIVE_KEYS: ['Cookies', 'URL parameters', 'Anchor URL'],
        EXCLUDED_SUBDOMAINS: ['www', 'mail', 'ftp', 'blog', 'shop']
    };

    /**
     * Security utility functions
     */
    const SecurityUtils = {
        /**
         * Sanitize string to prevent XSS
         * @param {string} str - Input string
         * @return {string} Sanitized string
         */
        sanitize: function (str) {
            if (typeof str !== 'string') return '';

            const div = document.createElement('div');
            div.textContent = str;
            return div.innerHTML;
        },

        /**
         * Mask sensitive information
         * @param {string} key - Property key
         * @param {string} value - Property value
         * @return {string} Masked or original value
         */
        maskSensitiveData: function (key, value) {
            if (!value || value === 'N/A') return value;

            if (CONFIG.SENSITIVE_KEYS.includes(key)) {
                if (key === 'Cookies' && value.trim() === '') return 'No cookies';
                return CONFIG.HIDDEN_VALUE;
            }

            return value;
        },

        /**
         * Parse domain parts correctly
         * @param {string} hostname - Full hostname
         * @return {Object} Domain parts
         */
        parseDomainParts: function (hostname) {
            try {
                const parts = hostname.split('.');

                if (parts.length < 2) {
                    return { subdomain: '', domain: hostname, tld: '' };
                }

                // Handle complex TLDs (co.uk, com.au, etc.)
                let tld = parts.pop();
                let domain = parts.pop();

                // Check for second-level TLDs
                const secondLevelTLDs = ['co', 'com', 'org', 'net', 'gov', 'edu', 'ac'];
                if (parts.length > 0 && secondLevelTLDs.includes(parts[parts.length - 1])) {
                    const secondLevel = parts.pop();
                    tld = `${secondLevel}.${tld}`;
                    if (parts.length > 0) {
                        domain = parts.pop();
                    }
                }

                // Get subdomain (excluding common ones)
                let subdomain = parts.join('.');
                if (CONFIG.EXCLUDED_SUBDOMAINS.includes(subdomain)) {
                    subdomain = '';
                }

                return {
                    subdomain: subdomain || 'None',
                    domain: domain || hostname,
                    tld: tld || 'Unknown'
                };
            } catch (error) {
                console.error('Error parsing domain:', error);
                return { subdomain: 'Error', domain: hostname, tld: 'Error' };
            }
        }
    };

    /**
     * Data collection functions
     */
    const DataCollector = {
        /**
         * Collect all domain information safely
         * @return {Object} Collected data
         */
        collectData: function () {
            try {
                const { location } = window;
                const hostname = location.hostname;
                const domainParts = SecurityUtils.parseDomainParts(hostname);

                const data = {
                    "Full domain name": hostname,
                    "Protocol": location.protocol.replace(':', ''),
                    "Domain (without TLD)": domainParts.domain,
                    "TLD": domainParts.tld,
                    "Subdomain": domainParts.subdomain,
                    "Port": this.getPort(location),
                    "Path": location.pathname || '/',
                    "URL parameters": location.search,
                    "Anchor URL": location.hash,
                    "Domain Origin": location.origin,
                    "Cookies": document.cookie,
                    "HTTPS Secure": location.protocol === 'https:',
                    "Page Title": document.title,
                    "User Agent": navigator.userAgent.substring(0, 50) + '...',
                    "Timestamp": new Date().toISOString()
                };

                return data;
            } catch (error) {
                console.error('Error collecting domain data:', error);
                return { "Error": "Unable to collect domain information" };
            }
        },

        /**
         * Get correct port information
         * @param {Location} location - Window location object
         * @return {string} Port number or default
         */
        getPort: function (location) {
            if (location.port) return location.port;

            const defaultPorts = {
                'http:': '80',
                'https:': '443',
                'ftp:': '21',
                'ws:': '80',
                'wss:': '443'
            };

            return defaultPorts[location.protocol] || 'N/A';
        }
    };

    /**
     * UI rendering functions
     */
    const UIRenderer = {
        /**
         * Create table row safely
         * @param {string} key - Property name
         * @param {string} value - Property value
         * @return {string} HTML table row
         */
        createTableRow: function (key, value) {
            const sanitizedKey = SecurityUtils.sanitize(key);
            const sanitizedValue = SecurityUtils.sanitize(
                SecurityUtils.maskSensitiveData(key, value)
            );

            return `
                <tr>
                    <td class="property">${sanitizedKey}</td>
                    <td class="value">${sanitizedValue || 'N/A'}</td>
                </tr>
            `;
        },

        /**
         * Create complete table
         * @param {Object} data - Data to display
         * @return {string} HTML table
         */
        createTable: function (data) {
            let rows = '';

            for (const [key, value] of Object.entries(data)) {
                rows += this.createTableRow(key, value);
            }

            return `
                <table class="${CONFIG.TABLE_CLASS}">
                    <thead>
                        <tr>
                            <th>Property</th>
                            <th>Value</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${rows}
                    </tbody>
                </table>
            `;
        },

        /**
         * Inject styles into document
         */
        injectStyles: function () {
            const styles = `
                #${CONFIG.CONTAINER_ID} {
                    margin: 20px auto;
                    max-width: 1000px;
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background: #f8f9fa;
                    border-radius: 8px;
                    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                    padding: 20px;
                }
                
                #${CONFIG.CONTAINER_ID} h2 {
                    color: #2c3e50;
                    border-bottom: 2px solid #3498db;
                    padding-bottom: 10px;
                    margin-top: 0;
                    display: flex;
                    align-items: center;
                    gap: 10px;
                }
                
                #${CONFIG.CONTAINER_ID} h2::before {
                    content: '??';
                    font-size: 1.2em;
                }
                
                .${CONFIG.TABLE_CLASS} {
                    width: 98%;
                    border-collapse: collapse;
                    margin-top: 20px;
                    background: white;
                    border-radius: 6px;
                    overflow: hidden;
                }
                
                .${CONFIG.TABLE_CLASS} th {
                    background: #3498db;
                    color: white;
                    font-weight: 600;
                    padding: 12px 15px;
                    text-align: left;
                }
                
                .${CONFIG.TABLE_CLASS} td {
                    padding: 10px 15px;
                    border-bottom: 1px solid #e1e1e1;
                }
                
                .${CONFIG.TABLE_CLASS} tr:last-child td {
                    border-bottom: none;
                }
                
                .${CONFIG.TABLE_CLASS} tr:hover {
                    background: #f5f7fa;
                }
                
                .${CONFIG.TABLE_CLASS} .property {
                    font-weight: 600;
                    color: #2c3e50;
                    width: 30%;
                    vertical-align: top;
                }
                
                .${CONFIG.TABLE_CLASS} .value {
                    color: #34495e;
                    word-break: break-word;
                    font-family: 'Consolas', 'Monaco', monospace;
                }
                
                .secure-badge {
                    display: inline-block;
                    padding: 2px 8px;
                    border-radius: 4px;
                    font-size: 0.85em;
                    font-weight: bold;
                    margin-left: 8px;
                }
                
                .secure-yes {
                    background: #2ecc71;
                    color: white;
                }
                
                .secure-no {
                    background: #e74c3c;
                    color: white;
                }
            `;

            const styleElement = document.createElement('style');
            styleElement.textContent = styles;
            document.head.appendChild(styleElement);
        },

        /**
         * Create container and inject content
         * @param {Object} data - Data to display
         */
        render: function (data) {
            // Remove existing container if present
            const existingContainer = document.getElementById(CONFIG.CONTAINER_ID);
            if (existingContainer) {
                existingContainer.remove();
            }

            // Create container
            const container = document.createElement('div');
            container.id = CONFIG.CONTAINER_ID;

            // Create header with HTTPS status
            const isSecure = data['HTTPS Secure'];
            const secureBadge = isSecure ?
                '<span class="secure-badge secure-yes">SECURE</span>' :
                '<span class="secure-badge secure-no">NOT SECURE</span>';

            container.innerHTML = `
                <h2>Domain Information ${secureBadge}</h2>
                ${this.createTable(data)}
            `;

            // Insert at beginning of body
            document.body.insertBefore(container, document.body.firstChild);
        }
    };

    /**
     * Main initialization function
     */
    function init() {
        try {
            // Check if we're in a browser environment
            if (typeof window === 'undefined' || typeof document === 'undefined') {
                throw new Error('This script must run in a browser environment');
            }

            // Inject styles
            UIRenderer.injectStyles();

            // Collect and render data
            const data = DataCollector.collectData();
            UIRenderer.render(data);

            // Log for debugging (optional)
            console.log('Domain information collected:', {
                ...data,
                Cookies: data.Cookies === CONFIG.HIDDEN_VALUE ? '[Hidden]' : 'No cookies'
            });

        } catch (error) {
            console.error('Failed to initialize domain info:', error);

            // Graceful degradation
            const errorContainer = document.createElement('div');
            errorContainer.style.cssText = `
                padding: 20px;
                margin: 20px;
                background: #fee;
                border: 1px solid #f99;
                border-radius: 5px;
                color: #c00;
            `;
            errorContainer.textContent = 'Unable to display domain information.';
            document.body.insertBefore(errorContainer, document.body.firstChild);
        }
    }

    /**
     * Initialize when DOM is ready
     */
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    /**
     * Export for potential module usage
     */
    if (typeof module !== 'undefined' && module.exports) {
        module.exports = {
            collectData: DataCollector.collectData.bind(DataCollector),
            parseDomainParts: SecurityUtils.parseDomainParts.bind(SecurityUtils)
        };
    }
})();