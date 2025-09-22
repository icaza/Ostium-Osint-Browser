@echo off
:: ===========================================
:: Uninstall_SecureDelete_ContextMenu.bat
:: Removes "Secure deletion" from context menu
:: Requires Administrator
:: ===========================================

echo.
echo === Removing "Secure deletion" from context menu ===
echo.

reg delete "HKCR\*\shell\SecureDeletion" /f
reg delete "HKCR\Directory\shell\SecureDeletion" /f

echo.
echo Uninstallation complete.
pause
