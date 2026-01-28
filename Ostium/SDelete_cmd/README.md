# _SDelete_cmd.bat

Added script to add SDelete to Windows context menu and/or drag and drop a file or directory onto the SDelete_cmd.bat script icon to permanently delete it. 
SDelete is part of Microsoft's Sysinternals suite, learn about its use and the difference between SSD/HDD disk before using it.

## Video tutorial
▶️ https://youtu.be/roN_alnacM4

## Usage

_Change the path according to your configuration in the file_ SDelete_cmd.bat

set SDELETE=\Ostium\SDelete_cmd\SDelete\sdelete64.exe

# _Install_SecureDelete_ContextMenu.bat

_Change the path according to your configuration in the file_ Install_SecureDelete_ContextMenu.bat

set "ICO=\Ostium\SDelete_cmd\sdsd.ico"
set "SCRIPT=\Ostium\SDelete_cmd\SDelete_cmd.bat"

Script to add the registry keys needed to add a Secure Deletion button to the Windows context menu.
! Must be run in Admin mode

# Uninstall_SecureDelete_ContextMenu

Script to uninstall registry keys.
! Must be run in Admin mode

# SDelete

v2.05 (September 29, 2023)
Securely overwrite your sensitive files and cleanse your free space of previously deleted files using this DoD-compliant secure delete program. 
[Sysinternals Utilities Index](https://learn.microsoft.com/en-us/sysinternals/downloads/)