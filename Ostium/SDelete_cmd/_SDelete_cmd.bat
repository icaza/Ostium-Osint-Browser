@echo off
:: ================================
:: SDelete_cmd.bat
:: Secure deletion with SDelete
:: Drag and drop a file or folder onto it
:: ================================

setlocal enabledelayedexpansion

:: Full paths (adjust if different)
set SDELETE=\Ostium\SDelete_cmd\SDelete\sdelete64.exe

if not exist "%SDELETE%" (
    echo [ERROR] Cannot find %SDELETE%
    echo Check the path in the script.
    pause
    exit /b
)

if "%~1"=="" (
    echo Drag and drop a file or folder onto this script to securely delete it.
    pause
    exit /b
)

if exist "%~1\" (
    echo Securely delete the folder : "%~1"
    "%SDELETE%" -p 3 -s -q "%~1"
) else (
    echo Secure file deletion : "%~1"
    "%SDELETE%" -p 3 "%~1"
)

echo.
echo Secure deletion complete.
pause
