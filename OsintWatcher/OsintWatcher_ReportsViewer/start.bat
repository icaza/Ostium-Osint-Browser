@echo off
setlocal
cd /d "%~dp0"

where node >nul 2>nul
if errorlevel 1 (
  echo [ERROR] Node.js is not installed or not in PATH.
  echo         Download it from https://nodejs.org/
  pause
  exit /b 1
)

echo Starting OSINT Watcher - Reports Viewer...
echo.
echo Once started, open this URL in the browser of your choice:
echo     http://127.0.0.1:(portselect)
echo.
node server.js