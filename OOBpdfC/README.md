# 📄 PDF Converter Pro - iText Edition

A modern and powerful Windows application for converting PDF files to text and Markdown with advanced content extraction.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=.net)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=flat-square&logo=windows)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)
![iText](https://img.shields.io/badge/iText-8.0.5-red?style=flat-square)

## ✨ Features

### 🎯 Advanced Conversion
- **Multi-format**: Convert to TXT, Markdown, or both simultaneously
- **Batch Processing**: Handle multiple files in a single operation
- **Drag & Drop**: Intuitive interface with drag-and-drop support
- **Real-time Progress**: Visual tracking of each conversion

### 📊 Intelligent Extraction
- **Formatted Text**: Preserve original layout and formatting
- **Complete Metadata**: Extract title, author, subject, keywords, creator, producer
- **Images**: Automatic extraction and saving of images (PNG format)
- **Tables**: Automatic detection of tabular structures
- **Multi-page**: Individual processing of each page with statistics

### 🔒 Security and Validation
- Configurable file size limit (100 MB default)
- File format validation
- Robust error handling
- Protection against corrupted files

## 🖥️ Screenshots

### Main Interface
```
╔════════════════════════════════════════════════════════════╗
║  📄 PDF Converter Pro                                      ║
╠════════════════════════════════════════════════════════════╣
║  [➕ Add Files] [📁 Add Folder] [❌ Remove] [🗑️ Clear All]  ║
╠════════════════════════════════════════════════════════════╣
║  File List                   │  ⚙️ Conversion Options      ║
║  • document1.pdf (2.5 MB)    │  ○ 📝 Text only (.txt)      ║
║  • report.pdf (1.8 MB)       │  ○ 📋 Markdown only (.md)   ║
║  • invoice.pdf (500 KB)      │  ● 📑 Both formats          ║
║                              │                             ║
║                              │  ☑ ℹ️ Extract metadata      ║
║                              │  ☑ 🖼️ Extract images        ║
║                              │  ☑ 📊 Extract tables        ║
║                              │  ☑ ✨ Preserve formatting   ║
╠════════════════════════════════════════════════════════════╣
║  3 file(s) selected              [🚀 CONVERT]              ║
║  [████████████████░░░░] 75%                                ║
╚════════════════════════════════════════════════════════════╝
```

## 🚀 Installation

### Prerequisites
- Windows 10/11
- .NET 9.0 SDK or Runtime
- Visual Studio 2022 (for development)

### Clone the Repository
```bash
git clone https://github.com/your-username/pdf-converter-pro.git
cd pdf-converter-pro
```

### Build from Source
```bash
# Restore NuGet dependencies
dotnet restore

# Build the project
dotnet build --configuration Release

# Run the application
dotnet run --project PdfConverterWinForms
```

### Create Standalone Executable
```bash
# Publish with embedded runtime (standalone executable)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# The executable will be in: bin/Release/net9.0-windows/win-x64/publish/
```

## 📦 Dependencies

- **iText7** (v8.0.5) - Advanced PDF processing library
- **System.Drawing.Common** (v9.0.0) - Image extraction support

## 🎮 Usage

### Graphical Interface

1. **Add Files**
   - Click "➕ Add Files" to select individual PDFs
   - Click "📁 Add Folder" to import all PDFs from a directory
   - Drag and drop files directly into the window

2. **Configure Options**
   - Choose output format (TXT, MD, or both)
   - Enable/disable metadata extraction
   - Enable/disable image extraction
   - Enable/disable table detection
   - Enable/disable formatting preservation

3. **Convert**
   - Click "🚀 CONVERT" to start the conversion
   - Monitor progress in real-time
   - View status for each file (Pending, Processing, Completed, Error)

### Output Structure

```
input-folder/
├── document.pdf
├── document.txt          # Text output
├── document.md           # Markdown output
└── document_images/      # Extracted images folder
    ├── image_1.png
    ├── image_2.png
    └── image_3.png
```

### Markdown Output Example

```markdown
# document

*Document converted on 2026-01-21 at 14:30:00*

## 📋 Metadata

- **Title**: Annual Report 2024
- **Author**: John Doe
- **Subject**: Financial Analysis
- **Keywords**: finance, report, 2024
- **PDF Version**: 1.7
- **Total Pages**: 45

## 📊 Document Statistics

- **Total Pages**: 45
- **Extracted Images**: 12
- **Pages with Tables**: 8

---

## 📄 Content

### Page 1

> 🖼️ *This page contains 2 image(s)*

> 📊 *This page likely contains one or more tables*

[Page content here...]

---
```

## 🏗️ Project Structure

```
PdfConverterWinForms/
├── MainForm.cs              # Main UI logic
├── MainForm.Designer.cs     # UI designer code
├── PdfProcessor.cs          # PDF processing engine
├── Program.cs               # Application entry point
├── PdfConverterWinForms.csproj  # Project configuration
└── README.md                # This file
```

## 🔧 Advanced Features

### Custom Image Extraction
The application uses iText7's `ImageRenderListener` to extract images directly from the PDF rendering pipeline, ensuring high-quality output.

### Table Detection Algorithm
Tables are detected by analyzing text alignment patterns:
- Identifies multiple consecutive spaces (column separators)
- Calculates aligned line percentage
- Marks pages with >30% aligned content as containing tables

### Formatting Preservation
Uses `LocationTextExtractionStrategy` to maintain:
- Text positioning
- Line breaks
- Paragraph structure
- Indentation

## 📝 Configuration

Maximum file size can be modified in `PdfProcessor.cs`:

```csharp
public const int MaxFileSizeMb = 100;
public const long MaxFileSizeBytes = MaxFileSizeMb * 1024L * 1024L;
```

## 🐛 Troubleshooting

### Common Issues

**Error: "File exceeds maximum allowed size"**
- Solution: Increase `MaxFileSizeMb` in `PdfProcessor.cs`

**Error: "No text could be extracted"**
- Cause: PDF contains scanned images without OCR
- Solution: Use OCR software before conversion

**Warning: "Image extraction failed"**
- Cause: Encrypted or compressed images
- Solution: Images are skipped, text extraction continues

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Style
- Follow C# coding conventions
- Use meaningful variable names
- Add XML documentation for public methods
- Include error handling

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **iText7** - Powerful PDF manipulation library
- **.NET Team** - Excellent framework and tools
- **Community** - Contributors and users

## 📧 Contact

Project Link: [https://github.com/your-username/pdf-converter-pro](https://github.com/your-username/pdf-converter-pro)

## 🗺️ Roadmap

- [ ] OCR integration for scanned PDFs
- [ ] PDF preview panel
- [ ] Custom output templates
- [ ] Command-line interface
- [ ] Cloud storage integration
- [ ] Multi-language support
- [ ] Advanced table extraction with CSV export
- [ ] Batch renaming options
- [ ] Scheduled conversions

---

**Made with ❤️ using .NET 9.0 and iText7**