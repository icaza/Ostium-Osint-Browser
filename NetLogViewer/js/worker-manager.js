class WorkerManager {
    constructor() {
        this.workers = new Map();
        this.activeWorkers = new Set();
        this.maxWorkers = navigator.hardwareConcurrency || 4;
        this.taskQueue = [];
        this.isProcessingQueue = false;
        
        this.precreateWorkers(2);
    }
    
    precreateWorkers(count) {
        for (let i = 0; i < Math.min(count, this.maxWorkers); i++) {
            this.createWorker();
        }
    }
    
    createWorker() {
        try {
            const worker = new Worker('netlog-worker.js', {
                name: `netlog-worker-${Date.now()}`
            });
            
            const workerId = Date.now() + Math.random();
            this.workers.set(workerId, {
                id: workerId,
                worker,
                busy: false,
                tasksCompleted: 0,
                lastUsed: Date.now()
            });
            
            setTimeout(() => this.cleanupInactiveWorkers(), 300000);
            
            return workerId;
        } catch (error) {
            console.error('Failed to create worker:', error);
            return null;
        }
    }
    
    async executeTask(task) {
        return new Promise((resolve, reject) => {
            this.taskQueue.push({
                task,
                resolve,
                reject,
                timestamp: Date.now()
            });
            
            this.processQueue();
        });
    }
    
    async processQueue() {
        if (this.isProcessingQueue || this.taskQueue.length === 0) {
            return;
        }
        
        this.isProcessingQueue = true;
        
        while (this.taskQueue.length > 0) {
            const { task, resolve, reject } = this.taskQueue.shift();
            
            try {
                const workerId = await this.getAvailableWorker();
                const result = await this.executeWithWorker(workerId, task);
                resolve(result);
                
                const workerInfo = this.workers.get(workerId);
                if (workerInfo) {
                    workerInfo.busy = false;
                    workerInfo.lastUsed = Date.now();
                    workerInfo.tasksCompleted++;
                }
            } catch (error) {
                reject(error);
            }
            
            await new Promise(res => setTimeout(res, 0));
        }
        
        this.isProcessingQueue = false;
    }
    
    async getAvailableWorker() {
        for (const [id, info] of this.workers) {
            if (!info.busy) {
                info.busy = true;
                return id;
            }
        }
        
        if (this.workers.size < this.maxWorkers) {
            const newWorkerId = this.createWorker();
            if (newWorkerId) {
                const workerInfo = this.workers.get(newWorkerId);
                if (workerInfo) {
                    workerInfo.busy = true;
                }
                return newWorkerId;
            }
        }
        
        return new Promise((resolve) => {
            const checkWorker = () => {
                for (const [id, info] of this.workers) {
                    if (!info.busy) {
                        info.busy = true;
                        resolve(id);
                        return;
                    }
                }
                setTimeout(checkWorker, 100);
            };
            checkWorker();
        });
    }
    
    executeWithWorker(workerId, task) {
        return new Promise((resolve, reject) => {
            const workerInfo = this.workers.get(workerId);
            if (!workerInfo) {
                reject(new Error('Worker not found'));
                return;
            }
            
            const { worker } = workerInfo;
            const timeoutId = setTimeout(() => {
                this.terminateWorker(workerId);
                reject(new Error('Worker timeout after 60 seconds'));
            }, 60000);
            
            const messageHandler = (event) => {
                const { type, data } = event.data;
                
                switch (type) {
                    case 'PROCESSING_COMPLETE':
                    case 'FILE_STRUCTURE':
                    case 'FILTERED_RESULTS':
                    case 'EXPORT_COMPLETE':
                        clearTimeout(timeoutId);
                        worker.removeEventListener('message', messageHandler);
                        worker.removeEventListener('error', errorHandler);
                        
                        workerInfo.busy = false;
                        workerInfo.lastUsed = Date.now();
                        
                        resolve({ type, data });
                        break;
                        
                    case 'ERROR':
                        clearTimeout(timeoutId);
                        worker.removeEventListener('message', messageHandler);
                        worker.removeEventListener('error', errorHandler);
                        reject(new Error(data.message));
                        break;
                        
                    case 'PROGRESS':
                        if (task.onProgress) {
                            task.onProgress(data);
                        }
                        break;
                }
            };
            
            const errorHandler = (error) => {
                clearTimeout(timeoutId);
                worker.removeEventListener('message', messageHandler);
                worker.removeEventListener('error', errorHandler);
                reject(error);
            };
            
            worker.addEventListener('message', messageHandler);
            worker.addEventListener('error', errorHandler);
            
            worker.postMessage({
                type: task.type,
                data: task.data
            });
        });
    }
    
    terminateWorker(workerId) {
        const workerInfo = this.workers.get(workerId);
        if (workerInfo) {
            workerInfo.worker.terminate();
            this.workers.delete(workerId);
        }
    }
    
    cleanupInactiveWorkers() {
        const now = Date.now();
        const maxAge = 5 * 60 * 1000; // 5 minutes
        
        for (const [id, info] of this.workers) {
            if (!info.busy && now - info.lastUsed > maxAge) {
                this.terminateWorker(id);
            }
        }
    }
    
    terminateAll() {
        for (const [id] of this.workers) {
            this.terminateWorker(id);
        }
        this.workers.clear();
        this.taskQueue = [];
    }
    
    getStats() {
        return {
            totalWorkers: this.workers.size,
            busyWorkers: Array.from(this.workers.values()).filter(w => w.busy).length,
            queuedTasks: this.taskQueue.length,
            maxWorkers: this.maxWorkers
        };
    }
}

window.WorkerManager = new WorkerManager();