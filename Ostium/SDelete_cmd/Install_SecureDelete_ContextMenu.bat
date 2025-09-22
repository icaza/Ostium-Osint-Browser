@echo off
:: ===========================================
:: Install_SecureDelete_ContextMenu.bat
:: Adds "Secure deletion" to file and folder context menu
:: Requires Administrator
:: ===========================================

:: Full paths (adjust if different)
set "ICO=C:\Users\icaza\Documents\Ostium\SDelete_cmd\sdsd.ico"
set "SCRIPT=C:\Users\icaza\Documents\Ostium\SDelete_cmd\SDelete_cmd.bat"

echo.
echo === Installing "Secure deletion" into context menu (files & folders) ===
echo.

:: Files
reg add "HKCR\*\shell\SecureDeletion" /ve /t REG_EXPAND_SZ /d "Secure deletion" /f
reg add "HKCR\*\shell\SecureDeletion" /v Icon /t REG_EXPAND_SZ /d "%ICO%" /f
reg add "HKCR\*\shell\SecureDeletion\command" /ve /t REG_EXPAND_SZ /d "\"%SCRIPT%\" \"%%1\"" /f

:: Folders
reg add "HKCR\Directory\shell\SecureDeletion" /ve /t REG_EXPAND_SZ /d "Secure deletion" /f
reg add "HKCR\Directory\shell\SecureDeletion" /v Icon /t REG_EXPAND_SZ /d "%ICO%" /f
reg add "HKCR\Directory\shell\SecureDeletion\command" /ve /t REG_EXPAND_SZ /d "\"%SCRIPT%\" \"%%1\"" /f

echo.
echo Installation complete.
echo If the new menu does not appear, restart Explorer or log off/log on.
pause
