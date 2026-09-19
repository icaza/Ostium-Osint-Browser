# UrlDeduplicator

![Platform](https://img.shields.io/badge/platform-.NET%20Framework%204.8-512BD4)
![Language](https://img.shields.io/badge/language-C%23-239120)
![UI Language](https://img.shields.io/badge/UI-en--US-blue)
![License](https://img.shields.io/badge/license-MIT-green)

Lightweight C# console application that removes duplicate URLs from a large
text file (one URL per line) and produces a clean output file, without ever
loading the whole file into memory.

Built to handle lists ranging from a few dozen to several million URLs,
with live progress reporting and a minimal memory footprint.

## Table of contents

- [Features](#features)
- [Preview](#preview)
- [Performance](#performance)
- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)
- [How it works](#how-it-works)
- [Project structure](#project-structure)
- [License](#license)

## Features

- ⚡ **Fast** — `O(1)` lookup per URL via `HashSet<string>` with
  `StringComparer.Ordinal`.
- 🧠 **Memory-efficient** — streamed reading and writing, no full-file load
  into RAM.
- 📊 **Live progress** — progress bar, percentage, line count, and
  throughput (lines/second).
- 🧹 **Automatic cleanup** — blank or whitespace-only lines are skipped;
  every URL is trimmed of surrounding whitespace.
- 🔤 **Case-sensitive comparison** — two URLs that differ only by case are
  treated as distinct (the safest default, since case can point to
  different resources on the web).
- 🖱️ **Simple to use** — a single screen: point it at a file, press Enter,
  done. Drag-and-drop onto the executable also works.

## Preview

```
========================================
        URL Deduplicator (.NET)
========================================
Removes duplicate URLs from a large text file.

Enter the path of the URL file to process, then press Enter: urls.txt

Analyzing file...
File size:   110.20 MB
Total lines: 2,000,000

Processing...

[##############################] 100.0%  2,000,000/2,000,000 lines  1,450,000 lines/sec

Done.
Lines read:          2,000,000
Unique URLs written: 490,783
Duplicates removed:  1,509,217
Time elapsed:        00:01.37

Output file: urls_deduplicated.txt
```

## Performance

Measurements from a plain text file (indicative figures — actual timing
depends on disk and CPU):

| File            | Lines      | Size    | Time     | Memory (RSS) |
|-----------------|-----------:|--------:|---------:|--------------:|
| Medium test     | 150,003    | 6.4 MB  | ~0.09 s  | ~46 MB         |
| Large test      | 2,000,000  | 110 MB  | ~1.4 s   | ~130 MB        |

## Requirements

- Windows with [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)
  (preinstalled on an up-to-date Windows 10/11).
- To build: Visual Studio 2019/2022 (*.NET desktop development* workload)
  or MSBuild.

## Installation

### Option 1 — Build from source

```bash
git clone https://github.com/<your-account>/UrlDeduplicator.git
cd UrlDeduplicator
```

Open `UrlDeduplicator.sln` in Visual Studio, select the **Release**
configuration, then `Build > Build Solution` (`Ctrl+Shift+B`).

The executable is generated at
`UrlDeduplicator\bin\Release\UrlDeduplicator.exe`.

### Option 2 — Download a release

Grab the latest `UrlDeduplicator.exe` from the repository's
[Releases](../../releases) page.

## Usage

1. Launch `UrlDeduplicator.exe`.
2. Type (or paste) the path to the URL file to process, then press
   **Enter** — or drag and drop the file directly onto the executable.
3. Watch the live progress.
4. Pick up the output file created next to the original, with the
   `_deduplicated` suffix (e.g. `urls.txt` → `urls_deduplicated.txt`).

The original order is preserved: only the **first** occurrence of each URL
is kept in the output file.

## How it works

- **Streamed I/O** — buffered `StreamReader`/`StreamWriter` over
  `FileStream` (1 MB buffer), so the file is never loaded into memory in
  full.
- **Deduplication** — `HashSet<string>` with `StringComparer.Ordinal`, the
  fastest string comparison available in .NET.
- **Pre-count pass** — a fast byte scan (no string allocation) counts lines
  up front to display an exact percentage and to pre-size the `HashSet`,
  avoiding internal resizes during processing.
- **Garbage Collector** — *workstation* mode (not *server*) is set in
  `App.config`, favoring a small memory footprint over maximum multi-core
  throughput.

## Project structure

```
UrlDeduplicator/
├── UrlDeduplicator.sln
└── UrlDeduplicator/
    ├── Program.cs              # Full application logic
    ├── UrlDeduplicator.csproj  # Project targeting .NET Framework 4.8
    ├── App.config              # Runtime configuration (culture, GC)
    └── Properties/
        └── AssemblyInfo.cs
```

## License

Distributed under the [MIT](LICENSE) license.
