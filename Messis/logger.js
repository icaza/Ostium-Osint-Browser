const fs = require('fs');
const path = require('path');
const config = require('./config');

class Logger {
    constructor() {
        this.config = config.logging || { level: 'info', file: { enabled: false } };
        this.levels = {
            error: 0,
            warn: 1,
            info: 2,
            debug: 3
        };

        // Initialize log file if enabled
        if (this.config.file && this.config.file.enabled) {
            this.initializeLogFile();
        }
    }

    initializeLogFile() {
        try {
            const logPath = this.config.file.path || './logs/messis.log';
            const logDir = path.dirname(logPath);

            if (!fs.existsSync(logDir)) {
                fs.mkdirSync(logDir, { recursive: true });
            }

            // Test write permission
            fs.appendFileSync(logPath, '', 'utf8');
        } catch (error) {
            console.error('Failed to initialize log file:', error);
            // Disable file logging if initialization fails
            this.config.file.enabled = false;
        }
    }

    formatMessage(level, message, ...args) {
        const timestamp = new Date().toISOString();
        const formattedArgs = args.map(arg =>
            typeof arg === 'object' ? JSON.stringify(arg) : arg
        ).join(' ');

        return `[${timestamp}] [${level.toUpperCase()}] ${message} ${formattedArgs}`.trim();
    }

    log(level, message, ...args) {
        // Always log to console
        const consoleMethod = console[level] || console.log;
        consoleMethod(message, ...args);

        // Log to file if enabled and level is sufficient
        // Note: Simple level check, can be improved
        if (this.config.file && this.config.file.enabled) {
            const logPath = this.config.file.path;
            const logEntry = this.formatMessage(level, message, ...args) + '\n';

            try {
                fs.appendFileSync(logPath, logEntry, 'utf8');
            } catch (error) {
                console.error('Failed to write to log file:', error);
            }
        }
    }

    error(message, ...args) { this.log('error', message, ...args); }
    warn(message, ...args) { this.log('warn', message, ...args); }
    info(message, ...args) { this.log('info', message, ...args); }
    debug(message, ...args) { this.log('debug', message, ...args); }
}

module.exports = new Logger();
