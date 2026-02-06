# Messis OSINT Investigation Platform

<div align="center">

![Messis](https://img.shields.io/badge/Version-1.0.0-blue)
![Node](https://img.shields.io/badge/Node.js-18+-green)
![License](https://img.shields.io/badge/License-MIT-yellow)

**Messis** is a OSINT investigation platform for collecting, organizing, and visualizing investigation data in a structured and secure manner.

</div>

## Features

### 📊 Project Management
- **Unlimited creation** of investigation projects
- **Organization** by projects with detailed descriptions
- **Complete history** with creation and modification dates
- **Automatic backup** in a local SQLite database

### 🔍 Data Collection
- **Multiple entities**: People, Organizations, Locations, Documents, etc.
- **Customizable attributes**: Email, phone, address, social networks, etc.
- **Flexible categories**: Identification, Contact, Professional, Technical, etc.
- **Easy add/modify/delete** of all data
- **Notes and annotations** for each attribute

### 📈 Advanced Visualization
- **Interactive graph** with vis.js Network
- **Dynamic layouts**: Hierarchical, Force-Directed, Circular
- **Smooth zoom and navigation**
- **Double-click** to edit directly from the graph
- **Color-coded legend** by entity type
- **Data view** in grid with detailed cards

### 📤 Multi-Format Export
- **JSON**: Complete structured format
- **CSV**: Excel and database compatible
- **Markdown**: Readable documentation
- **HTML**: Standalone web report
- **TXT**: Simple text format
- **PDF**: Report

### 🔒 Security
- **Validation** of all user inputs
- **XSS protection** with HTML sanitization
- **Prepared SQL statements** (injection protection)
- **Rate limiting** to prevent abuse
- **Helmet.js** for HTTP security headers

### ⚡ Performance
- **Optimized code** with efficient memory management
- **Asynchronous data loading**
- **Intelligent caching**
- **Minimized resources** (no unnecessary dependencies)
- **Ultra-fast SQLite database**

## 📋 Prerequisites

- **Node.js** 18+ ([Download](https://nodejs.org/))
- **npm** (included with Node.js)

---

## 📄 License

MIT License - Free to use for any purpose.

## 🎯 OSINT Best Practices

1. **Document everything**: Add notes for each attribute
2. **Timestamp**: Timestamps are automatic
3. **Verify**: Cross-reference your sources
4. **Organize**: Use appropriate categories
5. **Export regularly**: Back up your research
6. **Respect privacy**: Use this tool ethically and legally

---

## ⭐ Support the Project

If Ostium OSINT Browser helps you in your work, feel free to:

- ⭐ **Star** the project on GitHub
- 🔄 **Share** with your colleagues
- 💬 **Contribute** to development
- 📝 **Suggest** improvements
- 🐛 **Report** bugs

---

<div align="center">

**Developed by [ICAZA](https://github.com/icaza)**

© 2026 Icaza Media - All rights reserved

[⬆ Back to top](#messis-osint-investigation-platform)

</div>