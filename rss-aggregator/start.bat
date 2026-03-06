@echo off

echo ======================================================
echo    rss-aggregator Tool by ICAZA MEDIA
echo ======================================================
echo.

REM Check Node.js
where node >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Node.js is not installed!
    echo Download it from https://nodejs.org/
    pause
    exit
)

node server.js