@echo off

echo ======================================================
echo       OSTIUM Bookmark Explorer by ICAZA MEDIA
echo ======================================================
echo.

REM Check for the presence of deno.exe in the current directory.
if not exist "%~dp0deno.exe" (
    echo [ERROR] deno.exe not found in the application directory!
    echo Download it from https://deno.com and place deno.exe here.
    pause
    exit
)

"%~dp0deno.exe" run --allow-net --allow-read --allow-write --allow-env app.ts
