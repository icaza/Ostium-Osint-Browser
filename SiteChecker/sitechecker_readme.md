# SiteChecker

A high-performance .NET Framework console application for bulk website availability verification with parallel processing and detailed reporting.

## Features

- **Parallel Processing**: Utilizes async/await and SemaphoreSlim for efficient concurrent checks (10 simultaneous requests)
- **Smart File Input**: Supports command-line arguments, interactive console input, or default file detection
- **Real-time Progress**: Live percentage-based progress indicator
- **Detailed Reporting**: Generates three output files with comprehensive results
- **Bot Detection Prevention**: Uses realistic browser User-Agent to avoid being flagged
- **Thread-Safe Operations**: Implements ConcurrentBag and Interlocked operations for data integrity
- **Automatic URL Normalization**: Adds HTTPS protocol if not specified
- **Timeout Management**: 30-second timeout per request with proper error handling

## Requirements

- .NET Framework 4.8.1
- Windows OS
- Optional: Microsoft.Web.WebView2 NuGet package (if WebView2 features are needed)

## Installation

1. Clone or download the project
2. Open in Visual Studio
3. Build the solution in Release mode
4. The executable will be generated in `bin/Release/`

## Usage

### Method 1: Command Line Argument
```bash
SiteChecker.exe sitelist.txt
```

### Method 2: Default File
Place a `sites.txt` file in the same directory as the executable. The application will detect and prompt to use it.

```bash
SiteChecker.exe
> File 'sites.txt' found.
> Use this file (Y/n): Y
```

### Method 3: Interactive Input
Run the application without arguments and enter the file path when prompted.

```bash
SiteChecker.exe
> Please enter the path to the file containing sites:
> C:\MyFiles\websites.txt
```

## Input File Format

Create a text file with one URL per line. Lines starting with `#` are treated as comments.

**Example (sites.txt):**
```
# Production servers
https://www.example.com
https://api.example.com

# Third-party services
github.com
stackoverflow.com
www.google.com

# Sites without protocol (https:// will be added automatically)
microsoft.com
```

## Output Files

### 1. in.txt
Contains all successfully responding websites (HTTP status 2xx), sorted alphabetically.

**Example:**
```
https://www.example.com
https://www.github.com
https://www.google.com
```

### 2. out.txt
Contains all non-responding or error-generating websites, sorted alphabetically.

**Example:**
```
https://sitedoesnotexist123456.com
https://www.brokensite.com
```

### 3. report.txt
Comprehensive report with statistics and detailed error information.

**Example:**
```
=== SITE VERIFICATION REPORT ===
Date: 10/06/2025 14:32:15

Online sites: 3
Offline sites: 2
Total: 5

=== ERROR DETAILS ===

URL: https://sitedoesnotexist123456.com
Error: Connection error: No such host is known

URL: https://www.brokensite.com
Error: HTTP 404 - Not Found
```

## Configuration

### Adjust Concurrent Requests
Modify the SemaphoreSlim initialization to change simultaneous request limit:

```csharp
private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(10); // Change 10 to desired value
```

### Adjust Timeout Duration
Modify the HttpClient timeout setting:

```csharp
httpClient.Timeout = TimeSpan.FromSeconds(30); // Change 30 to desired seconds
```

### Modify User-Agent
Update the User-Agent string to simulate different browsers:

```csharp
httpClient.DefaultRequestHeaders.Add("User-Agent", 
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36...");
```

## Performance

- **Threading Model**: Asynchronous I/O-bound operations with controlled parallelism
- **Memory Efficiency**: Thread-safe collections with minimal locking overhead
- **Scalability**: Tested with lists of 100+ websites
- **Speed**: Typically processes 50 websites in under 30 seconds (network-dependent)

## Error Handling

The application gracefully handles:
- **HTTP Errors**: Non-2xx status codes (404, 500, 503, etc.)
- **Connection Errors**: DNS failures, refused connections, network timeouts
- **Timeout Errors**: Sites that don't respond within 30 seconds
- **Unexpected Errors**: Catches and logs all exceptions with details

## Example Session

```
=== SiteChecker - Website Status Verification ===

File 'sites.txt' found.
Use this file (Y/n): Y

Selected file: sites.txt
Checking 5 site(s)...

Progress: 100% (5/5 sites checked)

=== Summary ===
Online sites: 3
Offline sites: 2
Execution time: 8.45s

Generated files:
   - in.txt (online sites)
   - out.txt (offline sites)
   - report.txt (detailed report)

Press any key to exit...
```

## Technical Details

### Architecture
- **Language**: C# (.NET Framework 4.8.1)
- **Pattern**: Async/Await with concurrent execution
- **Collections**: ConcurrentBag for thread-safe operations
- **Synchronization**: SemaphoreSlim for throttling, Interlocked for atomic operations

### HTTP Configuration
- **Protocol**: Automatic HTTPS enforcement
- **Redirects**: Follows up to 5 automatic redirections
- **Timeout**: 30 seconds per request
- **User-Agent**: Chrome browser simulation

## Troubleshooting

### Application doesn't start
- Ensure .NET Framework 4.8.1 is installed
- Run as Administrator if file permissions are restrictive

### Sites showing as offline incorrectly
- Check firewall/proxy settings
- Increase timeout value if network is slow
- Verify URLs are correctly formatted in input file

### High memory usage
- Reduce SemaphoreSlim concurrent request limit
- Check for extremely large input files (>1000 sites)

## License

This project is provided as-is for educational and commercial use.

## Version

Current Version: 1.0.0  
Last Updated: October 2025

## Author

Created as a professional website monitoring solution with enterprise-grade reliability and performance.