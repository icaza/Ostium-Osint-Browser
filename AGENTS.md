# AGENTS.md

## Project

Ostium OSINT Browser

Ostium is an open-source OSINT investigation browser designed to assist researchers, analysts, journalists and cybersecurity professionals in collecting and analyzing publicly available information on the web.

The browser is built using:

* C# (.NET)
* Windows Forms
* Microsoft Edge WebView2 (Chromium engine)

The application integrates multiple investigation and analysis tools such as:

* link extraction
* text and semantic analysis
* script injection
* OSINT utilities
* file and document analysis

All processing must remain **local and privacy-respecting**. No telemetry or data collection should be added.

---

# Core Principles

Agents modifying this repository MUST respect the following principles:

1. Privacy First
   Ostium never collects or transmits user data.

2. Local Processing
   All analysis must run locally on the user's machine.

3. Transparency
   Code must remain readable and auditable.

4. Stability
   Do not introduce breaking changes without justification.

5. OSINT Ethics
   Only publicly available data analysis tools should be implemented.

---

# Technology Stack

Primary language:

* C#

Framework:

* .NET

UI:

* Windows Forms

Browser engine:

* Microsoft Edge WebView2

Other components may include:

* HTML / CSS / JavaScript
* JSON parsing
* local file analysis tools
* PlantUML integration
* embedded OSINT utilities

---

# Repository Structure (Conceptual)

Agents should preserve the logical structure of the project.

Example structure:

/src
/Browser
/OSINTTools
/Analysis
/UI
/Utils

/tools
/scripts

/docs

Do NOT arbitrarily reorganize the project structure.

---

# Coding Standards

## C#

Follow standard .NET conventions:

* PascalCase for classes and methods
* camelCase for local variables
* private fields should use `_camelCase`

Example:

```csharp
public class LinkExtractor
{
    private string _url;

    public void ExtractLinks()
    {
    }
}
```

Rules:

* prefer clear and explicit code
* avoid unnecessary abstractions
* avoid reflection unless required
* avoid unsafe code

---

# WebView2 Rules

When modifying browser features:

* do not break WebView2 initialization
* ensure navigation stability
* do not disable browser security features without justification
* JavaScript injection must remain controlled and auditable

---

# Security Rules

Agents must NOT:

* add telemetry
* add remote tracking
* transmit browsing data
* add hidden network requests

Agents must avoid:

* executing arbitrary remote code
* loading external scripts without validation

---

# Performance Guidelines

Because OSINT analysis can process large pages or documents:

* prefer streaming over full memory loading
* avoid blocking the UI thread
* use async operations when possible

Example:

```csharp
await Task.Run(() => AnalyzeDocument(file));
```

---

# Adding New Tools

New OSINT tools should follow these rules:

1. Modular architecture
2. UI clearly separated from logic
3. Reusable components
4. Documentation required

Each tool should be placed in:

/src/OSINTTools/

---

# File Analysis Tools

Ostium may support analysis of formats such as:

* txt
* json
* xml
* csv
* html
* log
* md

Agents adding analyzers should:

* avoid loading extremely large files fully in memory
* sanitize user inputs
* ensure safe parsing

---

# Dependencies

Agents must:

* minimize external dependencies
* prefer standard .NET libraries
* avoid unnecessary packages

All dependencies must be documented.

---

# Pull Request Rules

Before submitting a PR:

1. Ensure project builds successfully
2. Verify no warnings
3. Keep commits small and clear
4. Provide a short explanation of changes

Commit message example:

```
feat: add semantic analyzer for HTML documents
fix: improve JSON parsing performance
refactor: cleanup WebView2 initialization
```

---

# Things Agents Must Never Modify

Unless explicitly required:

* licensing files
* security policies
* build scripts
* release configuration

---

# Documentation

Any new feature must include:

* inline comments
* documentation update if needed

---

# Philosophy

Ostium is designed as a powerful and ethical OSINT research platform.

Contributions should always aim to:

* improve investigation capabilities
* improve privacy
* improve reliability
* keep the tool accessible to researchers and professionals.

End of AGENTS.md
