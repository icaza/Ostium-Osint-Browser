class MemoryManager {
    constructor() {
        this.cache = new Map();
        this.maxCacheSize = 100 * 1024 * 1024; // 100MB
        this.currentCacheSize = 0;
        this.cleanupInterval = null;
        
        this.startCleanupInterval();
    }
    
    startCleanupInterval() {
        this.cleanupInterval = setInterval(() => {
            this.cleanup();
        }, 30000);
    }
    
    store(key, data, ttl = 300000) { // 5 default minutes
        const size = this.calculateSize(data);
        
        if (this.currentCacheSize + size > this.maxCacheSize) {
            this.evictOldEntries();
        }
        
        const entry = {
            data,
            size,
            timestamp: Date.now(),
            ttl,
            accessCount: 0
        };
        
        this.cache.set(key, entry);
        this.currentCacheSize += size;
        
        return key;
    }
    
    get(key) {
        const entry = this.cache.get(key);
        if (!entry) return null;
        
        if (Date.now() - entry.timestamp > entry.ttl) {
            this.cache.delete(key);
            this.currentCacheSize -= entry.size;
            return null;
        }
        
        entry.accessCount++;
        entry.lastAccess = Date.now();
        
        return entry.data;
    }
    
    calculateSize(data) {
        try {
            const jsonString = JSON.stringify(data);
            return new Blob([jsonString]).size;
        } catch (error) {
            return 1024;
        }
    }
    
    evictOldEntries() {
        const entries = Array.from(this.cache.entries());
        
        // default minutes
        entries.sort((a, b) => {
            const aLastAccess = a[1].lastAccess || a[1].timestamp;
            const bLastAccess = b[1].lastAccess || b[1].timestamp;
            return aLastAccess - bLastAccess;
        });
        
        // Delete the oldest ones until you have enough space.
        let freedSpace = 0;
        const targetFreeSpace = this.maxCacheSize * 0.2; // Release 20%
        
        for (const [key, entry] of entries) {
            if (freedSpace >= targetFreeSpace) break;
            
            this.cache.delete(key);
            freedSpace += entry.size;
            this.currentCacheSize -= entry.size;
        }
    }
    
    cleanup() {
        const now = Date.now();
        
        for (const [key, entry] of this.cache.entries()) {
            if (now - entry.timestamp > entry.ttl) {
                this.cache.delete(key);
                this.currentCacheSize -= entry.size;
            }
        }
    }
    
    clear() {
        this.cache.clear();
        this.currentCacheSize = 0;
    }
    
    getStats() {
        return {
            entries: this.cache.size,
            size: this.currentCacheSize,
            maxSize: this.maxCacheSize,
            usage: ((this.currentCacheSize / this.maxCacheSize) * 100).toFixed(2) + '%'
        };
    }
    
    destroy() {
        if (this.cleanupInterval) {
            clearInterval(this.cleanupInterval);
        }
        this.clear();
    }
}