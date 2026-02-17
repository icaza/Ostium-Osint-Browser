class NetLogWorker {
    constructor() {
        this.chunkSize = 1024 * 1024; // 1MB chunks
        this.entryBatchSize = 1000; // Batch processing
        this.currentData = null;
        
        self.addEventListener('message', this.handleMessage.bind(this));
    }

    /**
     * Determine if a string key is unsafe to use as a property name on a plain object.
     * This prevents prototype pollution via special keys like "__proto__".
     */
    isUnsafeKey(key) {
        return key === '__proto__' || key === 'constructor' || key === 'prototype';
    }
    
    handleMessage(event) {
        const { type, data } = event.data;
        
        switch (type) {
            case 'PROCESS_FILE':
                this.processFile(data);
                break;
            case 'ANALYZE_EVENTS':
                this.analyzeEvents(data);
                break;
            case 'EXTRACT_DNS_FROM_CACHE':
                this.extractDNSEntriesFromCache(data);
                break;
            case 'FILTER_AND_SORT':
                this.filterAndSort(data);
                break;
            case 'EXPORT_CSV':
                this.exportToCSV(data);
                break;
            default:
                console.warn('Unknown message type:', type);
        }
    }
    
    async processFile(fileData) {
        try {
            self.postMessage({ 
                type: 'PROGRESS', 
                data: { step: 'parsing', progress: 10 } 
            });
            
            const data = JSON.parse(fileData);
            
            const structure = this.analyzeFileStructure(data);
            self.postMessage({ 
                type: 'FILE_STRUCTURE', 
                data: structure 
            });
            
            let dnsEntries = [];
            let stats = {};
            
            // Method 1: DNS Cache
            if (data.hostResolverInfo?.cache?.entries) {
                const cacheResult = this.extractDNSEntriesFromCache(data.hostResolverInfo.cache);
                dnsEntries = cacheResult.entries;
                stats = cacheResult.stats;
                
                self.postMessage({ 
                    type: 'PROGRESS', 
                    data: { step: 'cache_processing', progress: 50 } 
                });
            }
            
            // Method 2: Analyze events (in batches)
            if (data.events && data.events.length > 0) {
                const eventResult = await this.processEventsInChunks(data.events, data);
                
                if (dnsEntries.length === 0) {
                    dnsEntries = eventResult.entries;
                } else {
                    // Merge results
                    dnsEntries = this.mergeEntries(dnsEntries, eventResult.entries);
                }
                
                stats = { ...stats, ...eventResult.stats };
                
                self.postMessage({ 
                    type: 'PROGRESS', 
                    data: { step: 'events_processing', progress: 90 } 
                });
            }
            
            // DNS configuration
            const config = this.extractDNSConfig(data);
            
            // Timeline events
            const timeline = this.extractTimelineEvents(data.events || []);
            
            self.postMessage({ 
                type: 'PROCESSING_COMPLETE', 
                data: { 
                    dnsEntries, 
                    stats, 
                    config, 
                    timeline,
                    structure 
                } 
            });
            
        } catch (error) {
            self.postMessage({ 
                type: 'ERROR', 
                data: { 
                    message: error.message, 
                    stack: error.stack 
                } 
            });
        }
    }
    
    async processEventsInChunks(events, fullData) {
        const chunkSize = 10000; // 10k events per batch
        const chunks = [];
        
        for (let i = 0; i < events.length; i += chunkSize) {
            chunks.push(events.slice(i, i + chunkSize));
        }
        
        let allEntries = [];
        let aggregatedStats = {
            totalEvents: events.length,
            hostResolverEvents: 0,
            dnsTransactionEvents: 0,
            successfulResolutions: 0,
            failedResolutions: 0,
            dohQueries: 0,
            byQueryType: {},
            byHost: {}
        };
        
        for (let i = 0; i < chunks.length; i++) {
            const chunk = chunks[i];
            const result = this.analyzeEventsChunk(chunk, fullData);
            
            allEntries = [...allEntries, ...result.entries];
            
            aggregatedStats.hostResolverEvents += result.stats.hostResolverEvents;
            aggregatedStats.dnsTransactionEvents += result.stats.dnsTransactionEvents;
            aggregatedStats.successfulResolutions += result.stats.successfulResolutions;
            aggregatedStats.failedResolutions += result.stats.failedResolutions;
            aggregatedStats.dohQueries += result.stats.dohQueries;
            
            Object.keys(result.stats.byQueryType).forEach(key => {
                aggregatedStats.byQueryType[key] = 
                    (aggregatedStats.byQueryType[key] || 0) + result.stats.byQueryType[key];
            });
            
            Object.keys(result.stats.byHost).forEach(host => {
                if (!aggregatedStats.byHost[host]) {
                    aggregatedStats.byHost[host] = result.stats.byHost[host];
                } else {
                    aggregatedStats.byHost[host].count += result.stats.byHost[host].count;
                    aggregatedStats.byHost[host].successes += result.stats.byHost[host].successes;
                    aggregatedStats.byHost[host].failures += result.stats.byHost[host].failures;
                    aggregatedStats.byHost[host].lastSeen = result.stats.byHost[host].lastSeen;
                }
            });
            
            const progress = 30 + ((i + 1) / chunks.length) * 40;
            self.postMessage({ 
                type: 'PROGRESS', 
                data: { 
                    step: `processing_chunk_${i + 1}_of_${chunks.length}`, 
                    progress: Math.round(progress) 
                } 
            });
            
            if (i % 5 === 0) {
                await new Promise(resolve => setTimeout(resolve, 0));
            }
        }
        
        return { entries: allEntries, stats: aggregatedStats };
    }
    
    analyzeEventsChunk(events, fullData) {
        const dnsEntries = [];
        const stats = {
            hostResolverEvents: 0,
            dnsTransactionEvents: 0,
            successfulResolutions: 0,
            failedResolutions: 0,
            dohQueries: 0,
            byQueryType: {},
            byHost: {}
        };
        
        const hostMap = new Map();
        
        events.forEach(event => {
            try {
                if (event.source?.type === 13) {
                    stats.hostResolverEvents++;
                    this.processEventForDNS(event, hostMap, stats, dnsEntries);
                }
                
                if (event.source?.type === 43) {
                    stats.dnsTransactionEvents++;
                    this.processEventForDNS(event, hostMap, stats, dnsEntries);
                }
                
                if (event.params?.host || event.params?.dns_query_type !== undefined) {
                    this.processEventForDNS(event, hostMap, stats, dnsEntries);
                }
                
            } catch (e) {
                console.warn('Error processing event in worker:', e);
            }
        });
        
        return { 
            entries: Array.from(hostMap.values()), 
            stats 
        };
    }
    
    processEventForDNS(event, hostMap, stats, dnsEntries) {
        const params = event.params || {};
        const hostname = params.host;
        
        if (!hostname) return;
        // Prevent prototype pollution via special property names.
        if (this.isUnsafeKey(hostname)) return;
        
        if (!stats.byHost[hostname]) {
            stats.byHost[hostname] = {
                count: 0,
                successes: 0,
                failures: 0,
                queryTypes: new Set(),
                firstSeen: event.time,
                lastSeen: event.time
            };
        }
        
        stats.byHost[hostname].count++;
        stats.byHost[hostname].lastSeen = event.time;
        
        if (params.net_error === undefined) {
            stats.byHost[hostname].successes++;
            stats.successfulResolutions++;
        } else {
            stats.byHost[hostname].failures++;
            stats.failedResolutions++;
        }
        
        const queryType = params.dns_query_type || 0;
        stats.byQueryType[queryType] = (stats.byQueryType[queryType] || 0) + 1;
        stats.byHost[hostname].queryTypes.add(queryType);
        
        if (params.secure) {
            stats.dohQueries++;
        }
        
        if (!hostMap.has(hostname)) {
            const entry = this.createDNSEntry(event, params);
            hostMap.set(hostname, entry);
        } else {
            const existing = hostMap.get(hostname);
            this.updateDNSEntry(existing, event, params);
        }
    }
    
    createDNSEntry(event, params) {
        return {
            hostname: params.host,
            family: this.addressFamilyToString(params.address_family || 0),
            addresses: this.extractAddressesFromParams(params),
            ttl: params.ttl || 300,
            expiration: this.calculateExpiration(event.time, params.ttl),
            network_changes: 0,
            network_key: params.network_isolation_key || params.network_anonymization_key || '',
            query_count: 1,
            last_query: event.time,
            doh: params.secure || false,
            error: params.net_error ? {
                code: params.net_error,
                message: this.netErrorToString(params.net_error)
            } : null,
            expired: false
        };
    }
    
    updateDNSEntry(existing, event, params) {
        existing.query_count++;
        existing.last_query = event.time;
        
        const newAddresses = this.extractAddressesFromParams(params);
        newAddresses.forEach(addr => {
            if (!existing.addresses.includes(addr)) {
                existing.addresses.push(addr);
            }
        });
        
        if (params.net_error && !existing.error) {
            existing.error = {
                code: params.net_error,
                message: this.netErrorToString(params.net_error)
            };
        }
    }
    
    extractDNSEntriesFromCache(cache) {
        const entries = [];
        const now = Date.now() / 1000;
        const stats = {
            totalEntries: cache.entries?.length || 0,
            expiredEntries: 0,
            ipv4Entries: 0,
            ipv6Entries: 0,
            entriesWithErrors: 0
        };
        
        if (!cache.entries || !Array.isArray(cache.entries)) {
            return { entries, stats };
        }
        
        cache.entries.forEach(entry => {
            try {
                const expirationTime = entry.expiration || 0;
                const isExpired = entry.network_changes < cache.network_changes || now > expirationTime;
                
                if (isExpired) stats.expiredEntries++;
                if (entry.address_family === 1) stats.ipv4Entries++;
                if (entry.address_family === 2) stats.ipv6Entries++;
                if (entry.net_error !== undefined) stats.entriesWithErrors++;
                
                entries.push({
                    hostname: entry.hostname || 'unknown',
                    family: this.addressFamilyToString(entry.address_family),
                    addresses: this.extractAddresses(entry),
                    ttl: entry.ttl || 0,
                    expiration: expirationTime,
                    network_changes: entry.network_changes || 0,
                    network_key: entry.network_anonymization_key || entry.network_isolation_key || '',
                    expired: isExpired,
                    error: entry.net_error ? {
                        code: entry.net_error,
                        message: this.netErrorToString(entry.net_error)
                    } : null
                });
            } catch (e) {
                console.warn('Error extracting cache entry in worker:', e);
            }
        });
        
        return { entries, stats };
    }
    
    analyzeFileStructure(data) {
        const structure = {
            hasHostResolverInfo: !!data.hostResolverInfo,
            hasCache: !!(data.hostResolverInfo?.cache),
            cacheEntries: data.hostResolverInfo?.cache?.entries?.length || 0,
            hasEvents: !!(data.events && Array.isArray(data.events)),
            eventCount: data.events?.length || 0,
            hasConstants: !!data.constants,
            constants: data.constants ? Object.keys(data.constants) : [],
            fileSize: JSON.stringify(data).length,
            estimatedDNSEvents: 0
        };
        
        if (data.events) {
            structure.estimatedDNSEvents = data.events.filter(e => 
                e.source?.type === 13 || e.source?.type === 43
            ).length;
        }
        
        return structure;
    }
    
    extractDNSConfig(data) {
        let config = data.hostResolverInfo?.dns_config;
        
        if (!config) {
            config = {
                nameservers: [],
                search_domains: [],
                dns_over_https_servers: [],
                can_use_insecure_dns_transactions: false,
                can_use_secure_dns_transactions: false
            };
            
            if (data.events) {
                data.events.forEach(event => {
                    const params = event.params || {};
                    
                    if (params.nameservers && Array.isArray(params.nameservers)) {
                        params.nameservers.forEach(server => {
                            if (!config.nameservers.includes(server)) {
                                config.nameservers.push(server);
                            }
                        });
                    }
                    
                    if (params.secure === true) {
                        config.can_use_secure_dns_transactions = true;
                    }
                    
                    if (params.secure === false && params.host) {
                        config.can_use_insecure_dns_transactions = true;
                    }
                });
            }
        }
        
        return config;
    }
    
    extractTimelineEvents(events) {
        const timelineEvents = [];
        const hourlyCounts = {};
        const dohCounts = {};
        const errorCounts = {};
        
        events.forEach(event => {
            if (event.time) {
                const hour = new Date(event.time * 1000).toLocaleTimeString('fr-FR', {
                    hour: '2-digit',
                    minute: '2-digit'
                });
                
                hourlyCounts[hour] = (hourlyCounts[hour] || 0) + 1;
                
                if (event.params?.secure) {
                    dohCounts[hour] = (dohCounts[hour] || 0) + 1;
                }
                
                if (event.params?.net_error !== undefined) {
                    errorCounts[hour] = (errorCounts[hour] || 0) + 1;
                }
            }
        });
        
        Object.keys(hourlyCounts).sort().forEach(hour => {
            timelineEvents.push({
                time: hour,
                count: hourlyCounts[hour],
                type: dohCounts[hour] > 0 ? 'doh' : 'regular',
                error: errorCounts[hour] > 0
            });
        });
        
        return timelineEvents;
    }
    
    filterAndSort({ entries, filter, searchTerm, sortField, sortOrder }) {
        let filtered = entries.filter(entry => {
            if (searchTerm && !entry.hostname.toLowerCase().includes(searchTerm.toLowerCase())) {
                return false;
            }
            
            switch (filter) {
                case 'active':
                    return !entry.expired && !entry.error;
                case 'expired':
                    return entry.expired;
                case 'error':
                    return entry.error !== null;
                default:
                    return true;
            }
        });
        
        filtered.sort((a, b) => {
            let valA = a[sortField];
            let valB = b[sortField];
            
            if (sortField === 'expiration') {
                valA = a.expiration;
                valB = b.expiration;
            } else if (sortField === 'network') {
                valA = a.network_changes;
                valB = b.network_changes;
            } else if (sortField === 'hostname') {
                valA = a.hostname.toLowerCase();
                valB = b.hostname.toLowerCase();
            }
            
            if (valA < valB) return sortOrder === 'asc' ? -1 : 1;
            if (valA > valB) return sortOrder === 'asc' ? 1 : -1;
            return 0;
        });
        
        return filtered;
    }
    
    exportToCSV({ entries }) {
        const headers = [
            'Domain',
            'Address family', 
            'IP addresses',
            'TTL (secondes)',
            'Expiration date',
            'Status',
            'Requests',
            'DNS over HTTPS',
            'Network key'
        ];
        
        const rows = entries.map(entry => {
            const addresses = entry.addresses && entry.addresses.length > 0 
                ? entry.addresses.join('; ') 
                : 'None';
            
            let expirationDate = 'Unknown';
            if (entry.expiration && entry.expiration > 0) {
                const date = new Date(entry.expiration * 1000);
                expirationDate = date.toISOString();
            }
            
            let status = 'Active';
            if (entry.expired) status = 'Expired';
            if (entry.error) status = 'Error: ' + entry.error.message;
            
            const queryCount = entry.query_count || 1;
            const doh = entry.doh ? 'Yes' : 'No';
            
            return [
                entry.hostname || '',
                entry.family || '',
                addresses,
                entry.ttl || '0',
                expirationDate,
                status,
                queryCount.toString(),
                doh,
                entry.network_key || ''
            ];
        });
        
        const csvContent = [
            headers.join(','),
            ...rows.map(row => row.map(cell => 
                `"${String(cell).replace(/"/g, '""')}"`
            ).join(','))
        ].join('\n');
        
        return csvContent;
    }
    
    mergeEntries(entries1, entries2) {
        const mergedMap = new Map();
        
        entries1.forEach(entry => {
            mergedMap.set(entry.hostname, { ...entry });
        });
        
        entries2.forEach(entry => {
            if (mergedMap.has(entry.hostname)) {
                const existing = mergedMap.get(entry.hostname);
                
                entry.addresses.forEach(addr => {
                    if (!existing.addresses.includes(addr)) {
                        existing.addresses.push(addr);
                    }
                });
                existing.query_count += entry.query_count || 1;
                if (entry.last_query > existing.last_query) {
                    existing.last_query = entry.last_query;
                }
            } else {
                mergedMap.set(entry.hostname, { ...entry });
            }
        });
        
        return Array.from(mergedMap.values());
    }
    
    extractAddresses(entry) {
        let addresses = [];
        
        if (entry.addresses && Array.isArray(entry.addresses)) {
            addresses = addresses.concat(entry.addresses);
        }
        
        if (entry.ip_endpoints && Array.isArray(entry.ip_endpoints)) {
            entry.ip_endpoints.forEach(ep => {
                if (ep.address) {
                    addresses.push(`${ep.address}${ep.port ? ':' + ep.port : ''}`);
                }
            });
        }
        
        return addresses;
    }
    
    extractAddressesFromParams(params) {
        const addresses = [];
        
        if (params.addresses && Array.isArray(params.addresses)) {
            addresses.push(...params.addresses);
        }
        
        if (params.ip_endpoints && Array.isArray(params.ip_endpoints)) {
            params.ip_endpoints.forEach(ep => {
                if (ep.address) {
                    addresses.push(ep.address + (ep.port ? ':' + ep.port : ''));
                }
            });
        }
        
        return addresses;
    }
    
    calculateExpiration(eventTime, ttl) {
        const nowInSeconds = Date.now() / 1000;
        return nowInSeconds + (ttl || 300);
    }
    
    addressFamilyToString(family) {
        const families = {
            0: 'UNSPEC',
            1: 'IPv4',
            2: 'IPv6',
            3: 'INET6',
        };
        return families[family] || `UNKNOWN (${family})`;
    }
    
    netErrorToString(errorCode) {
        const errors = {
            '-2': 'FAILED',
            '-100': 'CONNECTION_FAILED',
            '-101': 'CONNECTION_CLOSED',
            '-102': 'CONNECTION_RESET',
            '-103': 'CONNECTION_REFUSED',
            '-104': 'CONNECTION_ABORTED',
            '-105': 'CONNECTION_TIMED_OUT',
            '-106': 'ADDRESS_UNREACHABLE',
            '-107': 'ADDRESS_INVALID',
            '-200': 'CERTIFICATE_ERROR',
            '-201': 'CERTIFICATE_REVOKED',
            '-202': 'CERTIFICATE_INVALID',
            '-300': 'NAME_NOT_RESOLVED',
            '-301': 'NAME_RESOLUTION_FAILED',
            '-400': 'CACHE_MISS',
            '-501': 'ACCESS_DENIED',
        };
        return errors[errorCode?.toString()] || `Error ${errorCode}`;
    }
}

// Initialize the worker
new NetLogWorker();