class FileValidator {
    static validateNetLogFile(file, content) {
        const errors = [];
        const warnings = [];
        
        if (!file.name.endsWith('.json')) {
            errors.push('The file must be in JSON format');
        }
        
        const maxSize = 500 * 1024 * 1024; // 500MB
        if (file.size > maxSize) {
            errors.push(`The file is too large (max: ${maxSize / 1024 / 1024}MB)`);
        }
        
        try {
            const data = JSON.parse(content);
            
            if (!data.events && !data.hostResolverInfo) {
                warnings.push('The file does not contain a standard NetLog structure');
            }
            
            if (content.length > 10000000) { // 10MB
                this.validateLargeJson(content);
            }
            
            this.detectMaliciousContent(data);
            
        } catch (error) {
            errors.push(`Invalid JSON: ${error.message}`);
        }
        
        return { errors, warnings, isValid: errors.length === 0 };
    }
    
    static validateLargeJson(content) {
        let depth = 0;
        let maxDepth = 0;
        
        for (let char of content) {
            if (char === '{' || char === '[') depth++;
            if (char === '}' || char === ']') depth--;
            maxDepth = Math.max(maxDepth, depth);
            
            if (maxDepth > 100) {
                throw new Error('JSON too deep');
            }
        }
        
        const longStringRegex = /"[^"]{10000,}"/g;
        if (longStringRegex.test(content)) {
            throw new Error('JSON strings too long detected');
        }
    }
    
    static detectMaliciousContent(data) {
        const dangerousPatterns = [
            /<script/i,
            /javascript:/i,
            /data:/i,
            /onload/i,
            /onerror/i,
            /eval\(/i,
            /document\./i,
            /window\./i,
            /localStorage/i,
            /sessionStorage/i
        ];
        
        const jsonString = JSON.stringify(data);
        
        dangerousPatterns.forEach(pattern => {
            if (pattern.test(jsonString)) {
                console.warn('Potentially dangerous pattern detected:', pattern);
            }
        });
    }
    
    static sanitizeEntry(entry) {
        const sanitized = { ...entry };
        
        const escapeHtml = (str) => {
            if (typeof str !== 'string') return str;
            return str
                .replace(/&/g, '&amp;')
                .replace(/</g, '&lt;')
                .replace(/>/g, '&gt;')
                .replace(/"/g, '&quot;')
                .replace(/'/g, '&#x27;')
                .replace(/\//g, '&#x2F;');
        };
        
        ['hostname', 'network_key'].forEach(field => {
            if (sanitized[field]) {
                sanitized[field] = escapeHtml(sanitized[field]);
            }
        });
        
        if (sanitized.addresses) {
            sanitized.addresses = sanitized.addresses.map(addr => {
                const cleanAddr = this.validateIPAddress(addr);
                return escapeHtml(cleanAddr);
            });
        }
        
        return sanitized;
    }
    
    static validateIPAddress(addr) {
        const ipv4Regex = /^(\d{1,3}\.){3}\d{1,3}(:\d+)?$/;
        const ipv6Regex = /^([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$/;
        
        if (ipv4Regex.test(addr) || ipv6Regex.test(addr)) {
            return addr;
        }
        
        return addr.replace(/[^a-zA-Z0-9.:\-]/g, '');
    }
}