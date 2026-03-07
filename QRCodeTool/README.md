# QR Code Tool

A lightweight, user-friendly WinForms application to **generate** and **read/decode** QR codes.

---

## Features

### ✏ Generate Tab
- Enter any text, URL, vCard, Wi-Fi config, etc.
- Choose image size (100–2000 px)
- Select error correction level: L / M (default) / Q / H
- Customize **foreground** and **background** colors
- Live preview of the generated QR code
- Save as **PNG**, **JPEG**, or **BMP**

### 📷 Scan / Read Tab
- Load any image file (PNG, JPG, BMP, GIF, TIFF…)
- **Drag & drop** an image directly onto the preview panel
- Decoded content is displayed in a read-only text box
- **Copy to Clipboard** button
- **Open URL** button (auto-enabled when content is a URL)

---

## Requirements

- Windows 7 / 10 / 11
- [.NET Framework 4.8.1 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net481)
- Visual Studio 2019 or later (for building)

---

## How to Build

### Option A — Visual Studio

1. Open `QRCodeTool.csproj` in Visual Studio.
2. Right-click the project → **Manage NuGet Packages** → Restore.
   - Or run: `nuget restore QRCodeTool.csproj`
3. Build → **Build Solution** (`Ctrl+Shift+B`).
4. Run the `.exe` from `bin\Debug\` or `bin\Release\`.

### Option B — Command Line (MSBuild + NuGet CLI)

```cmd
nuget restore QRCodeTool.csproj
msbuild QRCodeTool.csproj /p:Configuration=Release
```

---

## Dependencies (NuGet)

| Package | Version | Purpose |
|---|---|---|
| `ZXing.Net` | 0.16.9 | QR encode & decode engine |
| `ZXing.Net.Bindings.Windows.Compatibility` | 0.16.9 | `Bitmap`-based renderer/reader for WinForms |

---

## Architecture Notes

- **Single-file UI** — all form logic in `MainForm.cs`, no Designer files needed.
- **Resource-light** — bitmaps are disposed as soon as they are replaced.
- **No unsafe code** — uses only managed `System.Drawing` and ZXing.Net.
- `ErrorCorrectionLevel` exposed via a dropdown so users can balance capacity vs. resilience.

---

## Project Structure

```
QRCodeTool/
├── Program.cs          ← STAThread entry point
├── MainForm.cs         ← All UI + logic
├── QRCodeTool.csproj   ← .NET Framework 4.8.1 project
├── packages.config     ← NuGet references
└── README.md
```
