@echo off
echo ======================================================
echo    MESSIS - OSINT Investigation Tool by ICAZA MEDIA
echo ======================================================
echo.

REM
where node >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Node.js is not installed!
    echo Download it on https://nodejs.org/
    pause
    exit /b 1
)

REM
if not exist "node_modules" (
    echo [INFO] Installation of outbuildings...
    call npm install
    if %ERRORLEVEL% NEQ 0 (
        echo [ERROR] Failure to install outbuildings
        pause
        exit /b 1
    )
    echo.
)

echo [INFO] Messis start...
echo [INFO] The application will be accessible on http://localhost:{portselect}
echo [INFO] Press Ctrl+C to stop the server
echo.

REM
call npm start

pause
