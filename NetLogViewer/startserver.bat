@echo off
echo Checking Python installation...

set PORT=8000

REM Check if python is accessible
python --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Python is not installed or not in PATH.
    echo.
    echo Please install Python from https://www.python.org/downloads/
    echo Make sure to check "Add Python to PATH" during installation.
    echo.
    pause
    exit /b 1
)

REM Get Python version
for /f "tokens=2" %%I in ('python --version 2^>^&1') do set "PYTHON_VERSION=%%I"
echo Python version %PYTHON_VERSION% detected.

REM Check if port 8000 is already in use
netstat -an | findstr ":%PORT%" | findstr "LISTENING" >nul
if not errorlevel 1 (
    echo Warning: Port %PORT% is already in use.
    echo.
    choice /c YN /n /m "Do you want to use a different port? (Y/N) "

    REM errorlevel 2 = N
    if errorlevel 2 (
        echo Server will not be started.
        pause
        exit /b 1
    )

    REM errorlevel 1 = Y
    set /p PORT=Enter port number: 
)

echo Starting HTTP server on port %PORT%...
echo Press Ctrl+C to stop the server.
echo.
python -m http.server %PORT%

if errorlevel 1 (
    echo.
    echo Error starting the server.
    pause
)