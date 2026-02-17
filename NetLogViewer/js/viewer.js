class NetLogViewer {
    constructor() {
        this.currentData = null;
        this.dnsEntries = [];
        this.filteredEntries = [];
        this.currentFilter = 'all';
        this.currentSort = { field: 'expiration', order: 'desc' };
        this.currentPage = 1;
        this.entriesPerPage = 25;
        this.isProcessing = false;
        this.chunkSize = 50000;
        
        this.settings = this.loadSettings();
        
        this.initElements();
        this.bindEventListeners();
        this.initChart();
        this.applySettings();
        
        console.log('NetLogViewer initialisé');
    }
    
    loadSettings() {
        const saved = localStorage.getItem('netlogViewerSettings');
        return saved ? JSON.parse(saved) : {
            entriesPerPage: 25,
            timeFormat: 'absolute',
            autoRefresh: true,
            highlightErrors: true,
            showProgress: true
        };
    }
    
    saveSettings() {
        localStorage.setItem('netlogViewerSettings', JSON.stringify(this.settings));
    }
    
    applySettings() {
        if (this.elements.entriesPerPage) {
            this.elements.entriesPerPage.value = this.settings.entriesPerPage;
        }
        if (this.elements.timeFormat) {
            this.elements.timeFormat.value = this.settings.timeFormat;
        }
        if (this.elements.autoRefresh) {
            this.elements.autoRefresh.checked = this.settings.autoRefresh;
        }
        if (this.elements.highlightErrors) {
            this.elements.highlightErrors.checked = this.settings.highlightErrors;
        }
        
        this.entriesPerPage = this.settings.entriesPerPage;
    }
    
    initElements() {
        this.elements = {
            fileInput: document.getElementById('fileInput'),
            fileInputBtn: document.getElementById('fileInputBtn'),
            dropZone: document.getElementById('dropZone'),
            fileInfo: document.getElementById('fileInfo'),
            exportCsv: document.getElementById('exportCsv'),
            clearData: document.getElementById('clearData'),
            toggleAdvanced: document.getElementById('toggleAdvanced'),
            advancedModal: document.getElementById('advancedModal'),
            saveSettings: document.getElementById('saveSettings'),
            modalClose: document.querySelector('.modal-close'),
            
            // DNS Table
            dnsTableBody: document.getElementById('dnsTableBody'),
            dnsSearch: document.getElementById('dnsSearch'),
            advancedSearchBtn: document.getElementById('advancedSearchBtn'),
            filterBtns: document.querySelectorAll('.filter-btn'),
            pagination: document.getElementById('dnsPagination'),
            prevPage: document.getElementById('prevPage'),
            nextPage: document.getElementById('nextPage'),
            pageInfo: document.getElementById('pageInfo'),
            
            // Stats
            totalEntries: document.getElementById('totalEntries'),
            activeEntries: document.getElementById('activeEntries'),
            expiredEntries: document.getElementById('expiredEntries'),
            
            // Configuration
            dnsInsecureStatus: document.getElementById('dnsInsecureStatus'),
            dnsSecureStatus: document.getElementById('dnsSecureStatus'),
            cacheCapacity: document.getElementById('cacheCapacity'),
            dnsServersList: document.getElementById('dnsServersList'),
            configTableBody: document.getElementById('configTableBody'),
            
            // Timeline
            totalRequests: document.getElementById('totalRequests'),
            dohRequests: document.getElementById('dohRequests'),
            dnsErrors: document.getElementById('dnsErrors'),
            
            // Settings
            entriesPerPage: document.getElementById('entriesPerPage'),
            timeFormat: document.getElementById('timeFormat'),
            autoRefresh: document.getElementById('autoRefresh'),
            highlightErrors: document.getElementById('highlightErrors'),
            
            // Tabs
            tabBtns: document.querySelectorAll('.tab-btn'),
            tabContents: document.querySelectorAll('.tab-content'),
            
            // JSON Viewer
            jsonViewer: document.getElementById('jsonViewer')
        };
    }
    
    bindEventListeners() {
        if (this.elements.fileInputBtn && this.elements.fileInput) {
            this.elements.fileInputBtn.addEventListener('click', (e) => {
                e.stopPropagation();
                this.elements.fileInput.click();
            });
        }
        
        if (this.elements.fileInput) {
            this.elements.fileInput.addEventListener('change', (e) => {
                if (e.target.files.length > 0) {
                    this.handleFileSelect(e.target.files[0]);
                }
            });
        }
        
        if (this.elements.dropZone) {
            this.elements.dropZone.addEventListener('dragover', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.elements.dropZone.classList.add('dragover');
            });
            
            this.elements.dropZone.addEventListener('dragleave', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.elements.dropZone.classList.remove('dragover');
            });
            
            this.elements.dropZone.addEventListener('drop', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.elements.dropZone.classList.remove('dragover');
                
                if (e.dataTransfer.files.length > 0) {
                    this.handleFileSelect(e.dataTransfer.files[0]);
                }
            });
            
            this.elements.dropZone.addEventListener('click', (e) => {
                if (!e.target.closest('#fileInputBtn')) {
                    this.elements.fileInput.click();
                }
            });
        }
        
        if (this.elements.exportCsv) {
            this.elements.exportCsv.addEventListener('click', () => this.exportToCSV());
        }
        
        if (this.elements.clearData) {
            this.elements.clearData.addEventListener('click', () => this.clearData());
        }
        
        if (this.elements.toggleAdvanced) {
            this.elements.toggleAdvanced.addEventListener('click', () => {
                this.elements.advancedModal.classList.add('active');
            });
        }
        
        if (this.elements.modalClose) {
            this.elements.modalClose.addEventListener('click', () => {
                this.elements.advancedModal.classList.remove('active');
            });
        }
        
        if (this.elements.advancedModal) {
            this.elements.advancedModal.addEventListener('click', (e) => {
                if (e.target === this.elements.advancedModal) {
                    this.elements.advancedModal.classList.remove('active');
                }
            });
        }
        
        if (this.elements.saveSettings) {
            this.elements.saveSettings.addEventListener('click', () => {
                this.saveCurrentSettings();
                this.applySettings();
                this.elements.advancedModal.classList.remove('active');
            });
        }
        
        if (this.elements.dnsSearch) {
            this.elements.dnsSearch.addEventListener('input', () => {
                this.filterEntries();
            });
        }

        if (this.elements.advancedSearchBtn) {
            this.elements.advancedSearchBtn.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.toggleAdvancedSearch();
            });
        }     
        
        if (this.elements.filterBtns) {
            this.elements.filterBtns.forEach(btn => {
                btn.addEventListener('click', (e) => {
                    this.elements.filterBtns.forEach(b => b.classList.remove('active'));
                    e.target.classList.add('active');
                    this.currentFilter = e.target.dataset.filter;
                    this.filterEntries();
                });
            });
        }
        
        const sortableHeaders = document.querySelectorAll('#dnsTable th[data-sort]');
        if (sortableHeaders) {
            sortableHeaders.forEach(th => {
                th.addEventListener('click', () => {
                    const field = th.dataset.sort;
                    this.sortEntries(field);
                });
            });
        }
        
        if (this.elements.prevPage) {
            this.elements.prevPage.addEventListener('click', () => {
                if (this.currentPage > 1) {
                    this.currentPage--;
                    this.renderTable();
                }
            });
        }
        
        if (this.elements.nextPage) {
            this.elements.nextPage.addEventListener('click', () => {
                const totalPages = Math.ceil(this.filteredEntries.length / this.entriesPerPage);
                if (this.currentPage < totalPages) {
                    this.currentPage++;
                    this.renderTable();
                    this.scrollToTop();
                }
            });
        }
        
        if (this.elements.tabBtns) {
            this.elements.tabBtns.forEach(btn => {
                btn.addEventListener('click', (e) => {
                    const tabId = e.target.dataset.tab;
                    this.switchTab(tabId);
                });
            });
        }
        
        if (this.elements.entriesPerPage) {
            this.elements.entriesPerPage.addEventListener('change', () => {
                this.entriesPerPage = parseInt(this.elements.entriesPerPage.value);
                this.currentPage = 1;
                this.renderTable();
                this.scrollToTop();
            });
        }
    }

    toggleAdvancedSearch() {
    const modal = this.createAdvancedSearchModal();
    
    if (this.advancedSearchOptions) {
        const regexInput = document.getElementById('regexSearch');
        const searchField = document.getElementById('searchField');
        const caseSensitive = document.getElementById('caseSensitive');
        const wholeWord = document.getElementById('wholeWord');
        const invertMatch = document.getElementById('invertMatch');
        
        if (regexInput && this.advancedSearchOptions.regex) {
            const regexString = this.advancedSearchOptions.regex.toString();
            const pattern = regexString.substring(1, regexString.lastIndexOf('/'));
            regexInput.value = pattern;
        }
        
        if (searchField && this.advancedSearchOptions.field) {
            searchField.value = this.advancedSearchOptions.field;
        }
        
        if (caseSensitive) {
            caseSensitive.checked = this.advancedSearchOptions.caseSensitive || false;
        }
        
        if (wholeWord) {
            wholeWord.checked = this.advancedSearchOptions.wholeWord || false;
        }
        
        if (invertMatch) {
            invertMatch.checked = this.advancedSearchOptions.invertMatch || false;
        }
    }
    
    modal.classList.add('active');
    }    

    saveCurrentSettings() {
        this.settings = {
            entriesPerPage: parseInt(this.elements.entriesPerPage.value),
            timeFormat: this.elements.timeFormat.value,
            autoRefresh: this.elements.autoRefresh.checked,
            highlightErrors: this.elements.highlightErrors.checked,
            showProgress: document.getElementById('showProgress') ? document.getElementById('showProgress').checked : true
        };
        this.saveSettings();
    }
    
    initChart() {
        const ctx = document.getElementById('timelineChart');
        if (!ctx) return;
        
        this.chart = new Chart(ctx.getContext('2d'), {
            type: 'line',
            data: {
                labels: [],
                datasets: [{
                    label: 'Requêtes DNS',
                    data: [],
                    borderColor: '#667eea',
                    backgroundColor: 'rgba(102, 126, 234, 0.1)',
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        mode: 'index',
                        intersect: false,
                        callbacks: {
                            label: function(context) {
                                return `Requêtes: ${context.parsed.y}`;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Number of requests'
                        },
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)'
                        }
                    },
                    x: {
                        title: {
                            display: true,
                            text: 'Hour'
                        },
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)'
                        }
                    }
                },
                interaction: {
                    intersect: false,
                    mode: 'nearest'
                }
            }
        });
    }
    
    // ============ FILE MANAGEMENT ============
    async handleFileSelect(file) {
        if (!file || !file.name.toLowerCase().endsWith('.json')) {
            alert('Please select a NetLog JSON file');
            return;
        }
        
        console.log(`Loading file: ${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)`);
        
        this.showLoadingState(file);
        this.isProcessing = true;
        
        try {
            const data = await this.readFileWithProgress(file);
            this.processCompleteFile(data, file);
        } catch (error) {
            console.error('Error during processing:', error);
            this.showError('Error: ' + error.message);
            this.isProcessing = false;
        }
    }
    
    readFileWithProgress(file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            
            reader.onload = (e) => {
                try {
                    const data = JSON.parse(e.target.result);
                    resolve(data);
                } catch (error) {
                    reject(new Error('Invalid JSON: ' + error.message));
                }
            };
            
            reader.onerror = () => {
                reject(new Error('File read error'));
            };
            
            reader.onprogress = (e) => {
                if (e.lengthComputable) {
                    const percent = (e.loaded / e.total) * 100;
                    this.updateProgress(percent, `Load: ${Math.round(percent)}%`);
                }
            };
            
            reader.onloadstart = () => {
                this.updateProgress(0, 'Start loading...');
            };
            
            reader.readAsText(file);
        });
    }
    
    processCompleteFile(data, file) {
        console.log('=== START OF FULL TREATMENT ===');
        console.log('File:', file.name);
        console.log('Data structure:');
        console.log('- hasEvents:', !!data.events, 'count:', data.events?.length);
        console.log('- hasHostResolverInfo:', !!data.hostResolverInfo);
        console.log('- hasDNSConfig:', !!data.hostResolverInfo?.dns_config);
        console.log('- hasCache:', !!data.hostResolverInfo?.cache);
        console.log('- Cache entries:', data.hostResolverInfo?.cache?.entries?.length);
        
        if (data.events && data.events.length > 0) {
            console.log('Sample of events:');
            data.events.slice(0, 5).forEach((event, i) => {
                console.log(`  ${i}: type=${event.type}, source.type=${event.source?.type}, params=`, event.params);
            });
        }
        
        this.currentData = data;
        
        if (data.hostResolverInfo?.cache?.entries) {
            console.log(`DNS cache extraction: ${data.hostResolverInfo.cache.entries.length} entrances`);
            this.dnsEntries = this.extractDNSEntriesFromCache(data.hostResolverInfo.cache);
        } else if (data.events && data.events.length > 0) {
            console.log(`Analysis of events: ${data.events.length} events`);
            this.dnsEntries = this.processEventsWithProgress(data.events);
        } else {
            console.warn('No DNS data found');
            this.dnsEntries = [];
        }
        
        console.log(`${this.dnsEntries.length} Extracted DNS entries`);
        
        this.filteredEntries = this.dnsEntries;
        this.updateStats();
        this.filterEntries();
        this.updateConfig(data);
        this.updateTimeline(data);
        this.updateJsonView(data);
        
        this.elements.exportCsv.disabled = false;
        this.elements.clearData.disabled = false;
        this.isProcessing = false;
        
        this.showFileSummary(file, data);
        this.switchTab('dns');
    }
    
    processEventsWithProgress(events) {
        const dnsEntries = [];
        const hostMap = new Map();
        
        if (!events || !Array.isArray(events)) {
            return dnsEntries;
        }
        
        console.log(`Treatment of ${events.length} events`);
        
        const maxEvents = Math.min(events.length, 200000);
        const chunkSize = 5000;
        const totalChunks = Math.ceil(maxEvents / chunkSize);
        
        let processed = 0;
        
        for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
            const start = chunkIndex * chunkSize;
            const end = Math.min(start + chunkSize, maxEvents);
            const chunk = events.slice(start, end);
            
            chunk.forEach(event => {
                try {
                    if (this.isDNSEvent(event)) {
                        this.processEventForDNS(event, hostMap);
                    }
                } catch (e) {
                }
            });
            
            processed += chunk.length;
            
            if (chunkIndex % 10 === 0 || chunkIndex === totalChunks - 1) {
                const progress = (processed / maxEvents) * 100;
                this.updateProgress(progress, `Analyse DNS: ${Math.round(progress)}%`);
            }
        }
        
        hostMap.forEach(entry => {
            dnsEntries.push(entry);
        });
        
        return dnsEntries;
    }
    
    isDNSEvent(event) {
        const params = event.params || {};
        return (
            event.source?.type === 13 || // HOST_RESOLVER_IMPL_JOB
            event.source?.type === 43 || // DNS_TRANSACTION
            params.host !== undefined ||
            params.dns_query_type !== undefined ||
            params.addresses !== undefined ||
            params.dns_transaction !== undefined
        );
    }
    
    processEventForDNS(event, hostMap) {
    const params = event.params || {};
    
    if (params.results && Array.isArray(params.results)) {
        params.results.forEach(result => {
            if (!result.domain_name) return;
            
            const hostname = result.domain_name;
            const entryKey = `${hostname}_${result.query_type || 'UNSPECIFIED'}`;
            
            let expiration = 0;
            if (result.timed_expiration) {
                expiration = this.convertNetLogTimestamp(result.timed_expiration);
            } else if (event.time) {
                expiration = this.convertNetLogTimestamp(event.time) + (300 * 1000); // 5 minutes by default
            }
            
            const existingEntry = hostMap.get(entryKey);
            
            if (!existingEntry) {
                const addresses = [];
                if (result.endpoints && Array.isArray(result.endpoints)) {
                    result.endpoints.forEach(ep => {
                        if (ep.address) {
                            addresses.push(`${ep.address}${ep.port ? ':' + ep.port : ''}`);
                        }
                    });
                }
                
                hostMap.set(entryKey, {
                    hostname: hostname,
                    family: this.addressFamilyToStringFromQueryType(result.query_type),
                    addresses: addresses,
                    ttl: this.calculateTTLFromExpiration(expiration, event.time),
                    expiration: expiration,
                    network_changes: 0,
                    network_key: params.network_anonymization_key || params.network_isolation_key || '',
                    query_count: 1,
                    last_query: this.convertNetLogTimestamp(event.time) || 0,
                    doh: params.secure || params.secure_dns_policy > 0 || false,
                    expired: expiration > 0 && expiration < Date.now(),
                    error: result.error ? {
                        code: result.error,
                        message: this.netErrorToString(result.error)
                    } : null
                });
            } else {
                existingEntry.query_count++;
                if (expiration > existingEntry.expiration) {
                    existingEntry.expiration = expiration;
                }
                
                if (result.endpoints && Array.isArray(result.endpoints)) {
                    result.endpoints.forEach(ep => {
                        if (ep.address) {
                            const addr = `${ep.address}${ep.port ? ':' + ep.port : ''}`;
                            if (!existingEntry.addresses.includes(addr)) {
                                existingEntry.addresses.push(addr);
                            }
                        }
                    });
                }
            }
        });
    }
    }

    addressFamilyToStringFromQueryType(queryType) {
    const queryTypes = {
        'A': 'IPv4',
        'AAAA': 'IPv6',
        'UNSPECIFIED': 'UNSPEC',
        'UNSPECIFIED': 'IPv4 IPv6'
    };
    return queryTypes[queryType] || queryType || 'UNSPEC';
    }

    calculateTTLFromExpiration(expirationMs, eventTimeMs) {
    if (!expirationMs || !eventTimeMs) return 300;
    
    const eventMs = this.convertNetLogTimestamp(eventTimeMs);
    return Math.max(1, Math.round((expirationMs - eventMs) / 1000));
    }
    
    extractAddressesFromParams(params) {
        const addresses = [];
        
        try {
            if (params.addresses && Array.isArray(params.addresses)) {
                addresses.push(...params.addresses);
            }
            
            if (params.ip_endpoints && Array.isArray(params.ip_endpoints)) {
                params.ip_endpoints.forEach(ep => {
                    if (ep.address) {
                        addresses.push(`${ep.address}${ep.port ? ':' + ep.port : ''}`);
                    }
                });
            }
        } catch (e) {
        }
        
        return addresses;
    }
    
    extractDNSEntriesFromCache(cache) {
        const entries = [];
        const now = Date.now() / 1000;
        
        if (!cache.entries || !Array.isArray(cache.entries)) {
            return entries;
        }
        
        console.log(`Extraction of ${cache.entries.length} cache entries`);
        
        cache.entries.forEach(entry => {
            try {
                const expirationTime = entry.expiration || 0;
                const isExpired = entry.network_changes < cache.network_changes || now > expirationTime;
                
                entries.push({
                    hostname: entry.hostname || 'unknown',
                    family: this.addressFamilyToString(entry.address_family),
                    addresses: this.extractAddresses(entry),
                    ttl: entry.ttl || 0,
                    expiration: expirationTime,
                    network_changes: entry.network_changes || 0,
                    network_key: entry.network_anonymization_key || entry.network_isolation_key || '',
                    query_count: 1,
                    last_query: 0,
                    doh: false,
                    expired: isExpired,
                    error: entry.net_error ? {
                        code: entry.net_error,
                        message: this.netErrorToString(entry.net_error)
                    } : null
                });
            } catch (e) {
                console.warn('Cache extraction error:', e);
            }
        });
        
        return entries;
    }
    
    extractAddresses(entry) {
        const addresses = [];
        
        try {
            if (entry.addresses && Array.isArray(entry.addresses)) {
                addresses.push(...entry.addresses);
            }
            
            if (entry.ip_endpoints && Array.isArray(entry.ip_endpoints)) {
                entry.ip_endpoints.forEach(ep => {
                    if (ep.address) {
                        addresses.push(`${ep.address}${ep.port ? ':' + ep.port : ''}`);
                    }
                });
            }
        } catch (e) {
        }
        
        return addresses;
    }
    
    calculateExpiration(eventTime, ttl) {
        if (!eventTime) return 0;
        
        let timestamp = eventTime;
        
        if (timestamp > 1000000000000) {
            timestamp = timestamp / 1000000;
        }
        else if (timestamp > 10000000000) {
            timestamp = timestamp / 1000;
        }
        
        return eventTime + (ttl * 1000000); // TTL in seconds → microseconds
    }
    
    convertNetLogTimestamp(timestamp) {
    if (!timestamp) return 0;
    
    if (typeof timestamp === 'number' || !isNaN(timestamp)) {
        const numTimestamp = Number(timestamp);       
        
        if (numTimestamp > 10000000000000000) {
            const windowsEpochDiff = 11644473600000000;
            const unixMicroseconds = numTimestamp - windowsEpochDiff;
            return unixMicroseconds / 1000; // Convert to milliseconds
        }
        
        if (numTimestamp > 1000000000000) {
            return numTimestamp / 1000; // Microsecondes → millisecondes
        } else if (numTimestamp > 10000000000) {
            return numTimestamp;
        } else {
            return numTimestamp * 1000; // Secondes → millisecondes
        }
    }
    
    return 0;
    }

    addressFamilyToString(family) {
        const families = {
            0: 'UNSPEC',
            1: 'IPv4',
            2: 'IPv6',
            3: 'INET6',
            23: 'IPv4 IPv6',
            24: 'IPv6 IPv4'
        };
        return families[family] || `Type ${family}`;
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
            '-300': 'NAME_NOT_RESOLVED',
            '-301': 'DNS_TIMED_OUT',
            '-400': 'CACHE_MISS',
        };
        return errors[errorCode?.toString()] || `Erreur ${errorCode}`;
    }
    
    // ============ USER INTERFACE ============
    showLoadingState(file) {
        this.elements.fileInfo.innerHTML = `
            <div class="loading-container">
                <div class="loading-spinner"></div>
                <p><i class="fas fa-file"></i> <strong>${file.name}</strong></p>
                <p class="small">${(file.size / 1024 / 1024).toFixed(2)} MB</p>
                <div class="progress-container">
                    <div class="progress-bar">
                        <div class="progress-fill" id="progressFill" style="width: 0%"></div>
                    </div>
                    <div class="progress-text" id="progressText">Initialization...</div>
                </div>
            </div>
        `;
    }
    
    updateProgress(percent, message) {
        const progressFill = document.getElementById('progressFill');
        const progressText = document.getElementById('progressText');
        
        if (progressFill && progressText) {
            progressFill.style.width = `${percent}%`;
            progressText.textContent = message;
        }
    }
    
    showFileSummary(file, data) {
        const eventsCount = data.events ? data.events.length : 0;
        const cacheCount = data.hostResolverInfo?.cache?.entries?.length || 0;
        
        this.elements.fileInfo.innerHTML = `
            <div class="file-info-success">
                <p><i class="fas fa-check-circle success-text"></i> <strong>${file.name}</strong></p>
                <p class="small">${(file.size / 1024 / 1024).toFixed(2)} MB</p>
                <div class="file-stats">
                    <p class="small"><i class="fas fa-database"></i> ${this.dnsEntries.length.toLocaleString()} DNS entries</p>
                    <p class="small"><i class="fas fa-list"></i> ${eventsCount.toLocaleString()} events</p>
                    ${cacheCount > 0 ? `<p class="small"><i class="fas fa-cache"></i> ${cacheCount.toLocaleString()} cache entries</p>` : ''}
                </div>
            </div>
        `;
    }
    
    showError(message) {
        this.elements.fileInfo.innerHTML = `
            <div class="error-message">
                <p><i class="fas fa-exclamation-triangle"></i> Error</p>
                <p class="small">${message}</p>
            </div>
        `;
    }
    
    // ============ FILTERING AND SORTING ============
    filterEntries() {
    try {
        const searchTerm = this.elements.dnsSearch ? this.elements.dnsSearch.value.toLowerCase() : '';
        const advancedOptions = this.advancedSearchOptions || {};
        
        this.filteredEntries = this.dnsEntries.filter(entry => {
            // 1. Filter by status (active/expired/error)
            switch (this.currentFilter) {
                case 'active':
                    if (entry.expired) return false;
                    break;
                case 'expired':
                    if (!entry.expired) return false;
                    break;
                case 'error':
                    if (!entry.error) return false;
                    break;
            }
            
            // 2. Advanced search using regular expression
            if (advancedOptions.regex) {
                let searchText = '';
                
                switch (advancedOptions.field) {
                    case 'hostname':
                        searchText = entry.hostname || '';
                        break;
                    case 'addresses':
                        searchText = entry.addresses?.join(' ') || '';
                        break;
                    case 'network_key':
                        searchText = entry.network_key || '';
                        break;
                    default: // 'all'
                        searchText = [
                            entry.hostname,
                            entry.addresses?.join(' '),
                            entry.network_key
                        ].filter(Boolean).join(' ');
                        break;
                }
                
                let regexMatches = advancedOptions.regex.test(searchText);
                
                if (advancedOptions.invertMatch) {
                    regexMatches = !regexMatches;
                }
                
                if (!regexMatches) {
                    return false;
                }
            }
            
            // 3. Simple text search
            if (searchTerm) {
                const searchIn = [
                    entry.hostname?.toLowerCase() || '',
                    entry.addresses?.join(' ').toLowerCase() || '',
                    entry.network_key?.toLowerCase() || ''
                ].join(' ');
                
                let textMatches = false;
                
                if (advancedOptions.wholeWord) {
                    const words = searchIn.split(/\s+/);
                    textMatches = words.some(word => word === searchTerm);
                } else {
                    textMatches = searchIn.includes(searchTerm);
                }
                
                if (!textMatches) {
                    return false;
                }
            }
            
            return true;
        });
        
        console.log(`Filtrage terminé: ${this.filteredEntries.length} entrées sur ${this.dnsEntries.length}`);
        
        this.currentPage = 1;
        this.sortEntries(this.currentSort.field);
        
    } catch (error) {
        console.error('Erreur lors du filtrage:', error);
        alert(`Erreur de filtrage: ${error.message}`);
    }
    }

    // ============ ADVANCED SEARCH ============
    createAdvancedSearchModal() {
    let modal = document.getElementById('advancedSearchModal');
    if (modal) {
        return modal;
    }
    
    modal = document.createElement('div');
    modal.id = 'advancedSearchModal';
    modal.className = 'modal';
    
    modal.innerHTML = `
        <div class="modal-content" style="max-width: 550px;">
            <div class="modal-header">
                <h3><i class="fas fa-search-plus"></i> Advanced search</h3>
                <button class="modal-close">&times;</button>
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label for="regexSearch">
                        <i class="fas fa-code"></i> Regular expression
                    </label>
                    <input type="text" id="regexSearch" class="form-control" 
                           placeholder="Ex: ^www\.|\.com$">
                    <small class="form-text">Use valid JavaScript regular expressions</small>
                </div>
                
                <div class="form-group">
                    <label for="searchField">
                        <i class="fas fa-filter"></i> Search field
                    </label>
                    <select id="searchField" class="form-control">
                        <option value="all">All fields</option>
                        <option value="hostname">Domain only</option>
                        <option value="addresses">IP addresses only</option>
                        <option value="network_key">Network key only</option>
                    </select>
                </div>
                
                <div class="form-group">
                    <label>
                        <input type="checkbox" id="caseSensitive">
                        <span>Respect the breakage</span>
                    </label>
                </div>
                
                <div class="form-group">
                    <label>
                        <input type="checkbox" id="wholeWord">
                        <span>Whole word only</span>
                    </label>
                </div>
                
                <div class="form-group">
                    <label>
                        <input type="checkbox" id="invertMatch">
                        <span>Reverse the correspondence</span>
                    </label>
                </div>
                
                <div class="advanced-search-examples">
                    <h4><i class="fas fa-lightbulb"></i> Examples of regular expressions</h4>
                    <ul>
                        <li><code>^www\.</code> - Domains beginning with "www."</li>
                        <li><code>\.(com|net)$</code> - Domains .com ou .net</li>
                        <li><code>google</code> - Anything containing "google"</li>
                        <li><code>192\.168\.</code> - IP addresses in 192.168.*</li>
                        <li><code>^[a-z0-9]+$</code> - Letters and numbers only</li>
                    </ul>
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" id="applyAdvancedSearch">
                    <i class="fas fa-check"></i> Apply the search
                </button>
                <button class="btn btn-secondary" id="resetAdvancedSearch">
                    <i class="fas fa-times"></i> Reset
                </button>
                <button class="btn" id="testRegex">
                    <i class="fas fa-vial"></i> Test the expression
                </button>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
    
    modal.querySelector('.modal-close').addEventListener('click', () => {
        modal.classList.remove('active');
    });
    
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            modal.classList.remove('active');
        }
    });
    
    modal.querySelector('#applyAdvancedSearch').addEventListener('click', () => {
        this.applyAdvancedSearch();
        modal.classList.remove('active');
    });
    
    modal.querySelector('#resetAdvancedSearch').addEventListener('click', () => {
        this.resetAdvancedSearch();
        modal.classList.remove('active');
    });
    
    modal.querySelector('#testRegex').addEventListener('click', () => {
        this.testRegexExpression();
    });
    
    return modal;
    }    

    applyAdvancedSearch() {
    try {
        const regexInput = document.getElementById('regexSearch').value.trim();
        const searchField = document.getElementById('searchField').value;
        const caseSensitive = document.getElementById('caseSensitive').checked;
        const wholeWord = document.getElementById('wholeWord').checked;
        const invertMatch = document.getElementById('invertMatch').checked;
        
        let regex = null;
        if (regexInput) {
            regex = new RegExp(regexInput, caseSensitive ? '' : 'i');
        }
        
        this.advancedSearchOptions = {
            regex: regex,
            field: searchField,
            caseSensitive: caseSensitive,
            wholeWord: wholeWord,
            invertMatch: invertMatch
        };
        
        const searchBox = this.elements.dnsSearch.closest('.search-box');
        if (searchBox) {
            searchBox.classList.add('advanced-active');
        }
        
        if (this.elements.dnsSearch) {
            this.elements.dnsSearch.placeholder = "Active advanced search...";
            this.elements.dnsSearch.title = "Advanced search active. Click on ⚙ to edit.";
        }
        
        this.filterEntries();
        
        console.log('Advanced Applied Research:', this.advancedSearchOptions);
        
    } catch (error) {
        console.error('Error in advanced search:', error);
        alert(`Error in the regular expression:\n${error.message}`);
    }
    }    

    resetAdvancedSearch() {
    this.advancedSearchOptions = null;
    
    const searchBox = this.elements.dnsSearch.closest('.search-box');
    if (searchBox) {
        searchBox.classList.remove('advanced-active');
    }
    
    if (this.elements.dnsSearch) {
        this.elements.dnsSearch.placeholder = "Search for a domain...";
        this.elements.dnsSearch.title = "";
        this.elements.dnsSearch.value = "";
    }
    
    this.filterEntries();
    
    console.log('Advanced search reset');
    }    

    testRegexExpression() {
    const regexInput = document.getElementById('regexSearch').value.trim();
    const caseSensitive = document.getElementById('caseSensitive').checked;
    
    if (!regexInput) {
        alert("Please enter a regular expression to test.");
        return;
    }
    
    try {
        const regex = new RegExp(regexInput, caseSensitive ? '' : 'i');
        const testString = "www.example.com";
        const matches = regex.test(testString);
        
        alert(`Regular expression test:\n\n` +
              `Expression: /${regexInput}/${caseSensitive ? '' : 'i'}\n` +
              `Test chain: "${testString}"\n` +
              `Result: ${matches ? '✓ MATCH' : '✗ DOES NOT MATCH'}\n\n` +
              `Note: This test uses a fixed string. Test it with your own data to verify..`);
    } catch (error) {
        alert(`Syntax error in the regular expression:\n\n${error.message}`);
    }
    }
    
    sortEntries(field) {
        try {
            if (this.currentSort.field === field) {
                this.currentSort.order = this.currentSort.order === 'asc' ? 'desc' : 'asc';
            } else {
                this.currentSort.field = field;
                this.currentSort.order = 'desc';
            }
            
            this.filteredEntries.sort((a, b) => {
                let valA = a[field];
                let valB = b[field];
                
                if (field === 'expiration') {
                    valA = a.expiration || 0;
                    valB = b.expiration || 0;
                } else if (field === 'network') {
                    valA = a.network_changes || 0;
                    valB = b.network_changes || 0;
                } else if (field === 'hostname') {
                    valA = (a.hostname || '').toLowerCase();
                    valB = (b.hostname || '').toLowerCase();
                } else if (field === 'addresses') {
                    valA = a.addresses?.length || 0;
                    valB = b.addresses?.length || 0;
                }
                
                if (valA < valB) return this.currentSort.order === 'asc' ? -1 : 1;
                if (valA > valB) return this.currentSort.order === 'asc' ? 1 : -1;
                return 0;
            });
            
            this.renderTable();
        } catch (e) {
            console.error('Sorting error:', e);
        }
    }
    
    // ============ RENDERING TABLE ============
    renderTable() {
        try {
            if (!this.elements.dnsTableBody) {
                console.error('Table body not found');
                return;
            }
            
            const startIndex = (this.currentPage - 1) * this.entriesPerPage;
            const endIndex = startIndex + this.entriesPerPage;
            const pageEntries = this.filteredEntries.slice(startIndex, endIndex);
            
            this.elements.dnsTableBody.innerHTML = '';
            
            if (pageEntries.length === 0) {
                const row = this.elements.dnsTableBody.insertRow();
                row.className = 'empty-row';
                const cell = row.insertCell();
                cell.colSpan = 7;
                cell.innerHTML = `
                    <i class="fas fa-info-circle"></i>
                    ${this.dnsEntries.length === 0 ? 
                        'No DNS data found in the file' : 
                        'No data matching the filters'}
                `;
                return;
            }
            
            pageEntries.forEach((entry, index) => {
                const row = this.elements.dnsTableBody.insertRow();
                
                if (entry.expired) row.classList.add('expired');
                if (entry.error) row.classList.add('error');
                if (index % 2 === 0) row.classList.add('even');
                
                // Hostname
                const hostnameCell = row.insertCell();
                hostnameCell.textContent = entry.hostname;
                hostnameCell.title = entry.hostname;
                
                // Family
                const familyCell = row.insertCell();
                familyCell.textContent = entry.family;
                
                // Addresses
                const addressesCell = row.insertCell();
                if (entry.error) {
                    addressesCell.innerHTML = `
                        <span class="warning-text">
                            <i class="fas fa-exclamation-triangle"></i>
                            ${entry.error.message}
                        </span>
                    `;
                } else if (entry.addresses && entry.addresses.length > 0) {
                    const container = document.createElement('div');
                    container.className = 'address-container';
                    
                    entry.addresses.slice(0, 3).forEach(addr => {
                        const addrDiv = document.createElement('div');
                        addrDiv.className = 'address-item';
                        addrDiv.textContent = addr;
                        container.appendChild(addrDiv);
                    });
                    
                    if (entry.addresses.length > 3) {
                        const moreDiv = document.createElement('div');
                        moreDiv.className = 'address-more';
                        moreDiv.textContent = `+ ${entry.addresses.length - 3} others`;
                        container.appendChild(moreDiv);
                    }
                    
                    addressesCell.appendChild(container);
                } else {
                    addressesCell.textContent = '-';
                }
                
                // TTL
                const ttlCell = row.insertCell();
                ttlCell.textContent = entry.ttl || '0';
                
                // Expiration
                const expiresCell = row.insertCell();
                expiresCell.className = 'date-cell';
                if (entry.expiration && entry.expiration > 0) {
    try {
        const timestampMs = this.convertNetLogTimestamp(entry.expiration);
        const expiresDate = new Date(timestampMs);
        
        if (!isNaN(expiresDate.getTime())) {
            expiresCell.textContent = expiresDate.toLocaleString('fr-FR', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit'
            });
            
            const now = Date.now();
            if (timestampMs < now) {
                expiresCell.innerHTML += ' <span class="expired-badge">Expiré</span>';
                entry.expired = true;
            }
        } else {
            expiresCell.textContent = `Timestamp: ${entry.expiration}`;
            console.log('Invalid timestamp:', entry.expiration, '→', timestampMs);
        }
    } catch (e) {
        console.warn('Date formatting error:', e, entry.expiration);
        expiresCell.textContent = `Timestamp: ${entry.expiration}`;
    }
                } else {
                    expiresCell.textContent = '-';
                }
                
                // Network changes
                const networkCell = row.insertCell();
                networkCell.textContent = entry.network_changes || '0';
                
                // Network key
                const keyCell = row.insertCell();
                keyCell.textContent = entry.network_key || '-';
            });
            
            this.updatePagination();
            
        } catch (e) {
            console.error('Error rendered table:', e);
        }
    }
    
    updatePagination() {
    try {
        const totalPages = Math.max(1, Math.ceil(this.filteredEntries.length / this.entriesPerPage));
        
        if (this.elements.prevPage) {
            this.elements.prevPage.disabled = this.currentPage <= 1;
        }
        if (this.elements.nextPage) {
            this.elements.nextPage.disabled = this.currentPage >= totalPages;
        }
        if (this.elements.pageInfo) {
            this.elements.pageInfo.textContent = `Page ${this.currentPage} to ${totalPages}`;
        }
        
        const tableContainer = document.querySelector('.table-container');
        if (tableContainer) {
            tableContainer.scrollTop = 0;
        }
        
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
        
    } catch (e) {
        console.error('Erreur pagination:', e);
    }
    }
    
    // ============ DNS CONFIGURATION ============
    updateConfig(data) {
        try {
            const config = data.hostResolverInfo?.dns_config || this.extractDNSConfigFromEvents(data.events || []);
            const cache = data.hostResolverInfo?.cache;
            const constants = data.constants || {};
            
            console.log('Configuration DNS trouvée:', config);
            
            if (this.elements.dnsInsecureStatus) {
                const insecure = config?.can_use_insecure_dns_transactions;
                this.elements.dnsInsecureStatus.textContent = insecure ? 'Activated' : 'Disabled';
                this.elements.dnsInsecureStatus.className = insecure ? 'success-text' : 'warning-text';
            }

            if (this.elements.dnsSecureStatus) {
                const secure = config?.can_use_secure_dns_transactions;
                this.elements.dnsSecureStatus.textContent = secure ? 'Activated' : 'Disabled';
                this.elements.dnsSecureStatus.className = secure ? 'success-text' : 'warning-text';
            }
            
            if (this.elements.dnsInsecureStatus) {
                const insecure = config?.can_use_insecure_dns_transactions;
                this.elements.dnsInsecureStatus.textContent = insecure ? 'Activated' : 'Disabled';
                this.elements.dnsInsecureStatus.className = insecure ? 'success-text' : 'warning-text';
            }
            
            if (this.elements.dnsSecureStatus) {
                const secure = config?.can_use_secure_dns_transactions;
                this.elements.dnsSecureStatus.textContent = secure ? 'Activated' : 'Disabled';
                this.elements.dnsSecureStatus.className = secure ? 'success-text' : 'warning-text';
            }
            
            if (this.elements.dnsServersList) {
                this.elements.dnsServersList.innerHTML = '';
                
                let servers = [];
                
                if (config?.nameservers && config.nameservers.length > 0) {
                    servers = config.nameservers;
                } else {
                    servers = this.extractDNSServersFromEvents(data.events || []);
                }
                
                if (servers.length > 0) {
                    servers.forEach(server => {
                        const li = document.createElement('li');
                        li.textContent = this.formatDNSServer(server);
                        this.elements.dnsServersList.appendChild(li);
                    });
                } else {
                    const li = document.createElement('li');
                    li.textContent = 'No DNS server detected';
                    li.className = 'empty-item';
                    this.elements.dnsServersList.appendChild(li);
                }
            }
            
            if (this.elements.configTableBody) {
                this.elements.configTableBody.innerHTML = '';
                
                const configData = this.extractAllConfigData(data);
                
                if (configData.length === 0) {
                    const row = this.elements.configTableBody.insertRow();
                    const cell = row.insertCell();
                    cell.colSpan = 2;
                    cell.textContent = 'No configuration available';
                    cell.className = 'empty-cell';
                } else {
                    configData.forEach(item => {
                        const row = this.elements.configTableBody.insertRow();
                        const keyCell = row.insertCell();
                        const valueCell = row.insertCell();
                        
                        keyCell.textContent = item.key;
                        keyCell.className = 'config-key';
                        
                        this.renderConfigValue(valueCell, item);
                    });
                }
            }
            
            if (this.elements.cacheCapacity) {
                if (cache) {
                    this.elements.cacheCapacity.textContent = `${cache.entries?.length || 0} entrances`;
                    if (cache.capacity) {
                        this.elements.cacheCapacity.textContent += ` (ability: ${cache.capacity})`;
                    }
                } else {
                    this.elements.cacheCapacity.textContent = 'Not available';
                }
            }
            
        } catch (e) {
            console.error('Error updateConfig:', e);
        }
    }

    extractDNSConfigFromEvents(events) {
    const config = {
        nameservers: new Set(),
        search_domains: new Set(),
        dns_over_https_servers: new Set(),
        can_use_insecure_dns_transactions: false,
        can_use_secure_dns_transactions: false,
        secure_dns_mode: 0,
        use_local_ipv6: false
    };
    
    console.log('Analysis of', events.length, 'events for configuration');
    
    events.forEach((event, index) => {
        const params = event.params || {};
        
        if (params.dns_config || params.host || params.dns_query_type) {
            console.log(`Événement ${index} type ${event.type}:`, params);
        }
        
        if (event.source?.type === 30) {
            if (params.dns_config) {
                const dnsConfig = params.dns_config;
                console.log('DNS Config found:', dnsConfig);
                
                if (dnsConfig.nameservers && Array.isArray(dnsConfig.nameservers)) {
                    dnsConfig.nameservers.forEach(server => {
                        if (server && typeof server === 'string') {
                            config.nameservers.add(server.trim());
                        }
                    });
                }
                
                if (dnsConfig.search && Array.isArray(dnsConfig.search)) {
                    dnsConfig.search.forEach(domain => {
                        if (domain && typeof domain === 'string') {
                            config.search_domains.add(domain.trim());
                        }
                    });
                }
                
                if (dnsConfig.can_use_insecure_dns_transactions !== undefined) {
                    config.can_use_insecure_dns_transactions = dnsConfig.can_use_insecure_dns_transactions;
                }
                
                if (dnsConfig.can_use_secure_dns_transactions !== undefined) {
                    config.can_use_secure_dns_transactions = dnsConfig.can_use_secure_dns_transactions;
                }
                
                if (dnsConfig.secure_dns_mode !== undefined) {
                    config.secure_dns_mode = dnsConfig.secure_dns_mode;
                }
                
                if (dnsConfig.use_local_ipv6 !== undefined) {
                    config.use_local_ipv6 = dnsConfig.use_local_ipv6;
                }
            }
            
            if (params.secure_dns_policy !== undefined) {
                config.can_use_secure_dns_transactions = params.secure_dns_policy > 0;
            }
            
            if (params.host && params.host.includes('cloudflare-dns.com')) {
                config.dns_over_https_servers.add(params.host);
            }
        }
        
        if (params.nameservers && Array.isArray(params.nameservers)) {
            params.nameservers.forEach(server => {
                if (server && typeof server === 'string') {
                    config.nameservers.add(server.trim());
                }
            });
        }
        
        if (params.doh_servers && Array.isArray(params.doh_servers)) {
            params.doh_servers.forEach(server => {
                if (typeof server === 'string') {
                    config.dns_over_https_servers.add(server);
                } else if (server && server.server) {
                    config.dns_over_https_servers.add(JSON.stringify(server));
                }
            });
        }
        
        if (params.headers && Array.isArray(params.headers)) {
            params.headers.forEach(header => {
                if (header.includes('server:') && header.includes('cloudflare')) {
                    config.dns_over_https_servers.add('Cloudflare DNS');
                }
            });
        }
    });
    
    console.log('Configuration extraite:', {
        nameservers: Array.from(config.nameservers),
        dohServers: Array.from(config.dns_over_https_servers)
    });
    
    return {
        nameservers: Array.from(config.nameservers),
        search_domains: Array.from(config.search_domains),
        dns_over_https_servers: Array.from(config.dns_over_https_servers),
        can_use_insecure_dns_transactions: config.can_use_insecure_dns_transactions,
        can_use_secure_dns_transactions: config.can_use_secure_dns_transactions,
        secure_dns_mode: config.secure_dns_mode,
        use_local_ipv6: config.use_local_ipv6
    };
    }

    extractAllConfigData(data) {
    const config = data.hostResolverInfo?.dns_config || this.extractDNSConfigFromEvents(data.events || []);
    const cache = data.hostResolverInfo?.cache || {};
    const clientInfo = data.clientInfo || {};
    const constants = data.constants || {};
    
    const configData = [];
    
    configData.push({
        key: 'General Information',
        value: [
            `NetLog format: ${constants.logFormatVersion || 'Unknown'}`,
            `Browser version: ${clientInfo.version || 'Unknown'}`,
            `System: ${clientInfo.os_type || 'Unknown'}`,
            `Architecture: ${clientInfo.os_arch || 'Unknown'}`,
            `Date of creation: ${this.formatTimestamp(data.creationTime)}`,
            `Number of events: ${data.events?.length || 0}`,
            `DNS cache entries: ${cache.entries?.length || 0}`
        ],
        type: 'array'
    });
    
    if (config.nameservers && config.nameservers.length > 0) {
        configData.push({
            key: 'DNS Servers',
            value: config.nameservers.map(s => this.formatDNSServer(s)),
            type: 'array'
        });
    } else {
        const serversFromEvents = this.extractDNSServersFromEvents(data.events || []);
        if (serversFromEvents.length > 0) {
            configData.push({
                key: 'DNS servers (event logs)',
                value: serversFromEvents.map(s => this.formatDNSServer(s)),
                type: 'array'
            });
        }
    }
    
    // DNS over HTTPS
    if (config.dns_over_https_servers && config.dns_over_https_servers.length > 0) {
        configData.push({
            key: 'DNS over HTTPS',
            value: config.dns_over_https_servers,
            type: 'array'
        });
    }
    
    // Configuration DNS
    const dnsConfig = [];
    dnsConfig.push(`Standard DNS: ${config.can_use_insecure_dns_transactions ? 'Activated' : 'Disabled'}`);
    dnsConfig.push(`Secure DNS: ${config.can_use_secure_dns_transactions ? 'Activated' : 'Disabled'}`);
    
    if (config.secure_dns_mode !== undefined) {
        const modes = {
            0: 'Off',
            1: 'Automatic',
            2: 'Secure'
        };
        dnsConfig.push(`Mode DNS sécurisé: ${modes[config.secure_dns_mode] || config.secure_dns_mode}`);
    }
    
    if (config.use_local_ipv6 !== undefined) {
        dnsConfig.push(`IPv6 local: ${config.use_local_ipv6 ? 'Activated' : 'Disabled'}`);
    }
    
    configData.push({
        key: 'DNS configuration',
        value: dnsConfig,
        type: 'array'
    });
    
    if (config.search_domains && config.search_domains.length > 0) {
        configData.push({
            key: 'Research areas',
            value: config.search_domains,
            type: 'array'
        });
    }
    
    return configData;
    }
    
    extractDNSServersFromEvents(events) {
        const servers = new Set();
        
        events.forEach(event => {
            const params = event.params || {};
            
            if (params.nameservers && Array.isArray(params.nameservers)) {
                params.nameservers.forEach(server => {
                    if (server && typeof server === 'string') {
                        servers.add(server);
                    }
                });
            }
            
            if (params.dns_config?.nameservers) {
                params.dns_config.nameservers.forEach(server => {
                    if (server && typeof server === 'string') {
                        servers.add(server);
                    }
                });
            }
        });
        
        return Array.from(servers);
    }
    
    extractAllConfigData(data) {
        const config = data.hostResolverInfo?.dns_config || this.extractDNSConfigFromEvents(data.events || []);
        const cache = data.hostResolverInfo?.cache || {};
        const clientInfo = data.clientInfo || {};
        const constants = data.constants || {};
        
        const configData = [];
        
        const generalInfo = [
            `NetLog format: ${constants.logFormatVersion || 'Unknown'}`,
            `Browser version: ${clientInfo.version || 'Unknown'}`,
            `System: ${clientInfo.os_type || 'Unknown'}`,
            `Architecture: ${clientInfo.os_arch || 'Unknown'}`,
            `Date of creation: ${this.formatTimestamp(data.creationTime)}`,
            `Number of events: ${data.events?.length || 0}`,
            `DNS cache entries: ${cache.entries?.length || 0}`
        ];
        
        configData.push({
            key: 'General Information',
            value: [
                `NetLog format: ${constants.logFormatVersion || 'Unknown'}`,
                `Browser version: ${clientInfo.version || 'Unknown'}`,
                `System: ${clientInfo.os_type || 'Unknown'}`,
                `Architecture: ${clientInfo.os_arch || 'Unknown'}`,
                `Date of creation: ${this.formatTimestamp(data.creationTime)}`,
                `Number of events: ${data.events?.length || 0}`,
                `DNS cache entries: ${cache.entries?.length || 0}`
            ],
            type: 'array'
        });
        
        const servers = config.nameservers || this.extractDNSServersFromEvents(data.events || []);
        configData.push({
            key: 'DNS Servers',
            value: servers,
            type: 'array'
        });
        
        if (config.search_domains && config.search_domains.length > 0) {
            configData.push({
                key: 'Research areas',
                value: config.search_domains,
                type: 'array'
            });
        }
        
        if (config.dns_over_https_servers && config.dns_over_https_servers.length > 0) {
            configData.push({
                key: 'DNS over HTTPS',
                value: config.dns_over_https_servers.map(s => 
                    typeof s === 'string' ? s : JSON.stringify(s)
                ),
                type: 'array'
            });
        }
        
        const dnsConfig = [];
        if (config.can_use_secure_dns_transactions !== undefined) {
            dnsConfig.push(`Secure DNS: ${config.can_use_secure_dns_transactions ? 'Activated' : 'Disabled'}`);
        }
        if (config.can_use_insecure_dns_transactions !== undefined) {
            dnsConfig.push(`Standard DNS: ${config.can_use_insecure_dns_transactions ? 'Activatedv' : 'Disabled'}`);
        }
        if (config.secure_dns_mode !== undefined) {
            dnsConfig.push(`Secure DNS mode: ${config.secure_dns_mode}`);
        }
        if (config.use_local_ipv6 !== undefined) {
            dnsConfig.push(`IPv6 local: ${config.use_local_ipv6 ? 'Activated' : 'Disabled'}`);
        }
        
        if (dnsConfig.length > 0) {
            configData.push({
                key: 'DNS configuration',
                value: dnsConfig,
                type: 'array'
            });
        }
        
        const cacheConfig = [];
        if (cache.network_changes !== undefined) {
            cacheConfig.push(`Network modifications: ${cache.network_changes}`);
        }
        if (cache.capacity !== undefined) {
            cacheConfig.push(`Capability: ${cache.capacity}`);
        }
        if (cache.type !== undefined) {
            cacheConfig.push(`Type: ${cache.type}`);
        }
        
        if (cacheConfig.length > 0) {
            configData.push({
                key: 'DNS cache',
                value: cacheConfig,
                type: 'array'
            });
        }
        
        return configData;
    }
    
    renderConfigValue(cell, item) {
        cell.className = 'config-value';
        
        if (item.type === 'array' && Array.isArray(item.value)) {
            if (item.value.length === 0) {
                cell.textContent = 'None';
                cell.className = 'empty-value';
            } else {
                const container = document.createElement('div');
                container.className = 'config-value-array';
                
                item.value.forEach(value => {
                    const div = document.createElement('div');
                    div.textContent = value;
                    container.appendChild(div);
                });
                
                cell.appendChild(container);
            }
        } else if (typeof item.value === 'boolean') {
            cell.innerHTML = item.value ? 
                '<span class="success-text"><i class="fas fa-check"></i> Yes</span>' : 
                '<span class="warning-text"><i class="fas fa-times"></i> No</span>';
        } else if (item.value === null || item.value === undefined) {
            cell.textContent = 'Non disponible';
            cell.className = 'empty-value';
        } else {
            cell.textContent = String(item.value);
        }
    }
    
    formatDNSServer(server) {
        const knownServers = {
            '8.8.8.8': 'Google DNS',
            '8.8.4.4': 'Google DNS',
            '1.1.1.1': 'Cloudflare DNS',
            '1.0.0.1': 'Cloudflare DNS',
            '9.9.9.9': 'Quad9 DNS',
            '149.112.112.112': 'Quad9 DNS'
        };
        
        const description = knownServers[server];
        return description ? `${server} (${description})` : server;
    }
    
    formatTimestamp(timestamp) {
        if (!timestamp) return 'Unknown';
        
        try {
            let timestampMs = timestamp;
            if (timestamp < 10000000000) {
                timestampMs = timestamp * 1000;
            } else if (timestamp < 1000000000000) {
                timestampMs = timestamp;
            } else {
                timestampMs = timestamp / 1000;
            }
            
            const date = new Date(timestampMs);
            
            if (isNaN(date.getTime())) {
                return 'Date invalide';
            }
            
            return date.toLocaleString('fr-FR', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit'
            });
        } catch (e) {
            console.warn('Timestamp formatting error:', e);
            return 'Date error';
        }
    }
    
    // ============ TIMELINE ============
    updateTimeline(data) {
    try {
        if (!this.chart) return;
        
        const events = data.events || [];
        if (events.length === 0) return;
        
        const dnsEvents = events.filter(event => this.isDNSEvent(event));
        
        if (dnsEvents.length === 0) {
            console.log('No DNS events for the timeline');
            return;
        }
        
        const firstTimestamp = this.findFirstValidTimestamp(dnsEvents);
        
        const timeData = this.groupEventsByTimeCorrected(dnsEvents, firstTimestamp);
        
        this.chart.data.labels = timeData.labels;
        this.chart.data.datasets[0].data = timeData.data;
        this.chart.data.datasets[0].label = `DNS queries (${dnsEvents.length})`;
        this.chart.update();
        
        if (this.elements.totalRequests) {
            this.elements.totalRequests.textContent = dnsEvents.length.toLocaleString();
        }
        
        if (this.elements.dohRequests) {
            const dohCount = dnsEvents.filter(e => e.params?.secure).length;
            this.elements.dohRequests.textContent = dohCount.toLocaleString();
        }
        
        if (this.elements.dnsErrors) {
            const errorCount = dnsEvents.filter(e => e.params?.net_error !== undefined).length;
            this.elements.dnsErrors.textContent = errorCount.toLocaleString();
        }
        
    } catch (e) {
        console.error('updateTimeline error:', e);
    }
    }

    updateTimeline(data) {
    try {
        if (!this.chart) return;
        
        const events = data.events || [];
        if (events.length === 0) return;
        
        const dnsEvents = events.filter(event => this.isDNSEvent(event));
        
        if (dnsEvents.length === 0) {
            console.log('No DNS events for the timeline');
            return;
        }
        
        const firstTimestamp = this.findFirstValidTimestamp(dnsEvents);
        
        const timeData = this.groupEventsByTimeCorrected(dnsEvents, firstTimestamp);
        
        this.chart.data.labels = timeData.labels;
        this.chart.data.datasets[0].data = timeData.data;
        this.chart.data.datasets[0].label = `DNS queries (${dnsEvents.length})`;
        this.chart.update();
        
        if (this.elements.totalRequests) {
            this.elements.totalRequests.textContent = dnsEvents.length.toLocaleString();
        }
        
        if (this.elements.dohRequests) {
            const dohCount = dnsEvents.filter(e => e.params?.secure).length;
            this.elements.dohRequests.textContent = dohCount.toLocaleString();
        }
        
        if (this.elements.dnsErrors) {
            const errorCount = dnsEvents.filter(e => e.params?.net_error !== undefined).length;
            this.elements.dnsErrors.textContent = errorCount.toLocaleString();
        }
        
    } catch (e) {
        console.error('Error updateTimeline:', e);
    }
    }

    findFirstValidTimestamp(events) {
    for (const event of events) {
        if (event.time && event.time > 0) {
            return event.time;
        }
    }
    return Date.now() * 1000;
    }

    groupEventsByTimeCorrected(events, baseTimestamp, maxPoints = 50) {
    if (events.length === 0) {
        return { labels: [], data: [] };
    }
    
    const sortedEvents = [...events].sort((a, b) => (a.time || 0) - (b.time || 0));
    
    const firstEventTime = sortedEvents[0].time || baseTimestamp;
    const normalizedEvents = sortedEvents.map(event => ({
        ...event,
        normalizedTime: this.normalizeTimestamp(event.time, firstEventTime)
    }));
    
    const normalizedTimes = normalizedEvents.map(e => e.normalizedTime);
    // Utiliser reduce au lieu du spread operator pour éviter le dépassement de pile sur de grands tableaux
    const minTime = normalizedTimes.reduce((min, val) => Math.min(min, val), Infinity);
    const maxTime = normalizedTimes.reduce((max, val) => Math.max(max, val), -Infinity);
    const timeRange = Math.max(1, maxTime - minTime);
    
    const intervals = Math.min(maxPoints, Math.ceil(normalizedEvents.length / 10));
    const intervalSize = timeRange / intervals;
    
    const labels = [];
    const data = new Array(intervals).fill(0);
    
    normalizedEvents.forEach(event => {
        const normalizedTime = event.normalizedTime;
        const intervalIndex = Math.min(
            intervals - 1,
            Math.max(0, Math.floor((normalizedTime - minTime) / intervalSize))
        );
        
        if (intervalIndex >= 0 && intervalIndex < intervals) {
            data[intervalIndex]++;
        }
    });
    
    const baseDate = new Date(firstEventTime / 1000);
    
    for (let i = 0; i < intervals; i++) {
        const intervalTime = minTime + (i * intervalSize);
        const date = new Date(baseDate.getTime() + (intervalTime * 1000)); // secondes → millisecondes
        
        labels.push(
            date.toLocaleTimeString('fr-FR', { 
                hour: '2-digit', 
                minute: '2-digit',
                hour12: false 
            })
        );
    }
    
    return { labels, data };
    }

    normalizeTimestamp(timestamp, baseTimestamp) {
    // Conversion directe sans appeler convertToMs pour éviter la récursion
    let timestampMs = timestamp;
    let baseMs = baseTimestamp;
    
    // Convertir timestamp en millisecondes si nécessaire
    if (timestamp < 10000000000) {
        timestampMs = timestamp * 1000;
    } else if (timestamp >= 1000000000000) {
        timestampMs = timestamp / 1000;
    }
    
    // Convertir baseTimestamp en millisecondes si nécessaire
    if (baseTimestamp < 10000000000) {
        baseMs = baseTimestamp * 1000;
    } else if (baseTimestamp >= 1000000000000) {
        baseMs = baseTimestamp / 1000;
    }
    
    // Retourner la différence en secondes
    return (timestampMs - baseMs) / 1000;
    }    

    groupEventsByTime(events, maxPoints = 50) {
        if (events.length === 0) {
            return { labels: [], data: [] };
        }
        
        const sortedEvents = [...events].sort((a, b) => (a.time || 0) - (b.time || 0));
        
        const firstTime = this.convertToMs(sortedEvents[0].time || 0);
        const lastTime = this.convertToMs(sortedEvents[sortedEvents.length - 1].time || firstTime);
        const timeRange = Math.max(1, lastTime - firstTime);
        
        const intervals = Math.min(maxPoints, Math.ceil(sortedEvents.length / 10));
        const intervalSize = timeRange / intervals;
        
        const labels = [];
        const data = new Array(intervals).fill(0);
        
        sortedEvents.forEach(event => {
            const eventTime = this.convertToMs(event.time || 0);
            const intervalIndex = Math.min(
                intervals - 1,
                Math.max(0, Math.floor((eventTime - firstTime) / intervalSize))
            );
            
            if (intervalIndex >= 0 && intervalIndex < intervals) {
                data[intervalIndex]++;
            }
        });
        
        for (let i = 0; i < intervals; i++) {
            const intervalTime = firstTime + (i * intervalSize);
            const date = new Date(intervalTime);
            
            labels.push(
                date.toLocaleTimeString('fr-FR', { 
                    hour: '2-digit', 
                    minute: '2-digit',
                    hour12: false 
                })
            );
        }
        
        return { labels, data };
    }
    
    convertToMs(timestamp) {
        if (timestamp < 10000000000) {
            return timestamp * 1000;
        } else if (timestamp < 1000000000000) {
            return timestamp;
        } else {
            return timestamp / 1000;
        }
    }
    
    // ============ JSON VIEW ============
    updateJsonView(data) {
        try {
            if (!this.elements.jsonViewer) return;
            
            const preview = {
                constants: data.constants,
                clientInfo: data.clientInfo,
                creationTime: data.creationTime,
                logFormatVersion: data.logFormatVersion,
                fileInfo: {
                    events: data.events?.length || 0,
                    cacheEntries: data.hostResolverInfo?.cache?.entries?.length || 0,
                    hasConfig: !!data.hostResolverInfo?.dns_config
                },
                sample: {
                    events: data.events?.slice(0, 5) || [],
                    cache: data.hostResolverInfo?.cache?.entries?.slice(0, 3) || []
                }
            };
            
            const formattedJson = JSON.stringify(preview, null, 2);
            
            const highlightedJson = this.highlightJsonSyntax(formattedJson);
            
            this.elements.jsonViewer.innerHTML = `<pre><code class="language-json">${highlightedJson}</code></pre>`;
            
        } catch (e) {
            console.error('Error updateJsonView:', e);
        }
    }
    
    highlightJsonSyntax(json) {
        return json
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?)/g, (match) => {
                let cls = 'number';
                if (/^"/.test(match)) {
                    if (/:$/.test(match)) {
                        cls = 'key';
                    } else {
                        cls = 'string';
                    }
                } else if (/true|false/.test(match)) {
                    cls = 'boolean';
                } else if (/null/.test(match)) {
                    cls = 'null';
                }
                return `<span class="${cls}">${match}</span>`;
            })
            .replace(/\b-?\d+(\.\d+)?\b/g, '<span class="number">$&</span>');
    }
    
    // ============ NAVIGATION ============
    switchTab(tabId) {
        try {
            if (this.elements.tabBtns) {
                this.elements.tabBtns.forEach(btn => btn.classList.remove('active'));
            }
            if (this.elements.tabContents) {
                this.elements.tabContents.forEach(content => content.classList.remove('active'));
            }
            
            const tabBtn = document.querySelector(`.tab-btn[data-tab="${tabId}"]`);
            const tabContent = document.getElementById(`${tabId}Tab`);
            
            if (tabBtn && tabContent) {
                tabBtn.classList.add('active');
                tabContent.classList.add('active');
            }
        } catch (e) {
            console.error('Error switch tab:', e);
        }
    }
    
    // ============ EXPORT ============
    exportToCSV() {
        try {
            if (this.filteredEntries.length === 0) {
                alert('No data to export');
                return;
            }
            
            console.log(`Export CSV of ${this.filteredEntries.length} entry`);
            
            const headers = ['Domain', 'Family', 'IP Addresses', 'TTL', 'Expiry', 'Status', 'Requests', 'DoH', 'Network Key'];
            const rows = this.filteredEntries.map(entry => {
                const addresses = entry.addresses?.join('; ') || '';
                const expiration = entry.expiration ? this.formatTimestamp(entry.expiration) : '';
                const status = entry.expired ? 'Expired' : 'Active';
                const queryCount = entry.query_count || 1;
                const doh = entry.doh ? 'Yes' : 'No';
                
                return [
                    `"${(entry.hostname || '').replace(/"/g, '""')}"`,
                    `"${entry.family || ''}"`,
                    `"${addresses.replace(/"/g, '""')}"`,
                    entry.ttl || 0,
                    `"${expiration}"`,
                    `"${status}"`,
                    queryCount,
                    `"${doh}"`,
                    `"${(entry.network_key || '').replace(/"/g, '""')}"`
                ];
            });
            
            const csvContent = [
                headers.join(','),
                ...rows.map(row => row.join(','))
            ].join('\n');
            
            const blob = new Blob(['\ufeff' + csvContent], { type: 'text/csv;charset=utf-8' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            
            const dateStr = new Date().toISOString().slice(0, 19).replace(/[:]/g, '-');
            a.href = url;
            a.download = `netlog-dns-${dateStr}-${this.filteredEntries.length}.csv`;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            
            setTimeout(() => URL.revokeObjectURL(url), 100);
            
            console.log('CSV export complete');
            
        } catch (error) {
            console.error('CSV export error:', error);
            alert('Error during export: ' + error.message);
        }
    }
    
    // ============ Clean ============
    clearData() {
        if (confirm('Are you sure you want to erase all the data?')) {
            this.currentData = null;
            this.dnsEntries = [];
            this.filteredEntries = [];
            this.currentPage = 1;
            
            if (this.elements.dnsTableBody) {
                this.elements.dnsTableBody.innerHTML = `
                    <tr class="empty-row">
                        <td colspan="7">
                            <i class="fas fa-info-circle"></i>
                            Load a NetLog file to view the DNS data
                        </td>
                    </tr>
                `;
            }
            
            if (this.elements.fileInfo) {
                this.elements.fileInfo.innerHTML = '<p>No files loaded</p>';
            }
            
            if (this.elements.configTableBody) {
                this.elements.configTableBody.innerHTML = `
                    <tr>
                        <td colspan="2" class="empty-cell">No Data</td>
                    </tr>
                `;
            }
            
            if (this.elements.jsonViewer) {
                this.elements.jsonViewer.innerHTML = `
                    <pre><code>// The JSON data will appear here after loading</code></pre>
                `;
            }
            
            if (this.chart) {
                this.chart.data.labels = [];
                this.chart.data.datasets[0].data = [];
                this.chart.update();
            }
            
            this.elements.exportCsv.disabled = true;
            this.elements.clearData.disabled = true;
            
            this.updateStats();
        }
    }
    
    updateStats() {
        try {
            const total = this.dnsEntries.length;
            const expired = this.dnsEntries.filter(e => e.expired).length;
            const active = total - expired;
            const errors = this.dnsEntries.filter(e => e.error).length;
            
            this.elements.totalEntries.textContent = total.toLocaleString();
            this.elements.activeEntries.textContent = active.toLocaleString();
            this.elements.expiredEntries.textContent = expired.toLocaleString();
            
        } catch (e) {
            console.error('Error updateStats:', e);
        }
    }
    
    // ============ SCROLL UTILITY ============
    scrollToTop() {
        try {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        } catch (e) {
            console.error('Error scrollToTop:', e);
        }
    }
}

document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM loaded, NetLogViewer initialization...');
    
    try {
        window.netLogViewer = new NetLogViewer();
        console.log('NetLogViewer initialized successfully');
        
    } catch (error) {
        console.error('Error during initialization:', error);
        alert('Initialization error: ' + error.message);
    }
});