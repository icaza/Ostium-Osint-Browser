# SecureFileExplorer

[![License](https://img.shields.io/badge/license-MIT-green.svg)](https://github.com/icaza/Ostium-Osint-Browser/blob/feature%2314/StylometryAnonymizer/LICENSE.txt)

## Overview

SecureFileExplorer is a web application for file analysis. It allows you to index, search, view, and analyze document collections with a secure interface.

## Features

### Analysis and Indexing
- ✅ **Automatic indexing** of files with complete metadata
- ✅ **Real-time search** with highlighted results
- ✅ **Chronological timeline** of files
- ✅ **SHA-256 hashing** for file integrity

### Advanced Viewing
- ✅ **Interactive graph** with 4 layout algorithms (Force, Circular, Hierarchical, Grid)
- ✅ **Zoom and Pan** on the graph with mouse controls
- ✅ **Dynamic filtering** by extension type
- ✅ **Visual groups** by file type
- ✅ **Graph export** to high-quality SVG and PNG

### File Visualization
- ✅ **Direct visualization** of files in the browser
- ✅ **Custom viewer** for CSV, XML, JSON, archives
- ✅ **Syntax highlighting** for code and structured data
- ✅ **Hexadecimal view** for binary files

### Interface and UX
- ✅ **Modern interface** with professional dark theme
- ✅ **Intuitive controls** (double-click to open, drag to move)
- ✅ **Data export** to JSON and CSV
- ✅ **Responsive design** for mobile
- ✅ **Enhanced security** with file sandbox

## Graph Features

### Layout Algorithms
- **Force-Directed**: Physical simulation with natural forces
- **Circular**: Uniform circular distribution
- **Hierarchical**: Organization into hierarchical levels  
- **Grid**: Regular grid layout

### Zoom and Navigation
- Zoom from 20% to 500% (0.2x to 5x)
- Scroll wheel for vertical/horizontal panning
- Ctrl + scroll wheel for progressive zoom
- Reset, Zoom In, Zoom Out, Fit buttons

### Filtering and Groups
- Real-time filtering by extension
- Colored chips for each type
- Visual groups with dotted rectangles
- Automatic file counting by group

### Export
- Vector SVG export (editable)
- Raster PNG export (800x400px)
- File name with timestamp
- Color and style preservation

### Security

- 🔒 Strict file path validation
- 🔒 Protection against path traversal attacks
- 🔒 Verification of authorized extensions
- 🔒 Sandbox for static files
- 🔒 Comprehensive error handling

### UX/UI

- 🎨 Modernized interface with animations
- 🎨 Visual indicators (files open in green)
- 🎨 Clear error messages
- 🎨 Tooltips and contextual information
- 🎨 Responsive design
- 🎨 Custom scrollbars

## Advanced configuration config.json

Edit `config.json`:

### Change the port
```json
{
  “Port”: 9000
}
```

### Add allowed extensions
```json
{
  “AllowedExtensions”: [
    “.pdf”,
    “.docx”,
    “.xlsx”,
    “.pptx”
  ]
}
```

### Change the data directory
```json
{
  “RootDirectory”: “D:\\MyDocuments”
}
```

# Usage

**Create the data directory**
```bash
mkdir C:\OSINT_DATA
```

**Place your files to be analyzed**

Copy your documents to `C:\OSINT_DATA` or its subfolders.

Open your browser to: **http://localhost:PORT**

### Search
- Type in the search bar to filter files in real time
- Click on a file to open it in a new tab
- Open files are marked in green

### Timeline
- Displays all files in chronological order
- Click on an entry to open the file

### Graph
- Visualizes the relationships between files (grouped by extension)
- **Click** on a node to open the file
- **Drag** a node to rearrange the graph
- Colors indicate the file type:
  - 🔴 Red: PDF
  - 🔵 Blue: DOCX
  - 🟢 Green: CSV
  - 🟠 Orange: JPG/PNG images
  - 🟣 Purple: PNG
  - ⚫ Gray: Others

### Export
- **Export JSON**: All data and metadata
- **Export CSV**: Tabular format for Excel/analysis
