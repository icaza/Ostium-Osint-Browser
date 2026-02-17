@echo off

echo ======================================================
echo    MESSIS - OSINT Investigation Tool by ICAZA MEDIA
echo ======================================================
echo.

REM Check Node.js
where node >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Node.js is not installed!
    echo Download it from https://nodejs.org/
    pause
    exit /b 1
)

echo Select mode:
echo.
echo [D] Development (nodemon - auto restart)
echo [P] Production (node - stable)
echo.
set /p CHOICE="Your choice (D/P): "

REM Convert to lowercase
set CHOICE=%CHOICE:D=d%
set CHOICE=%CHOICE:P=p%

if "%CHOICE%"=="d" (
    echo.
    echo [INFO] DEVELOPMENT mode selected
    set INSTALL_CMD=npm install
    set START_CMD=npm run dev
) else (
    echo.
    echo [INFO] PRODUCTION mode selected
    set INSTALL_CMD=npm install --omit=dev
    set START_CMD=npm start
)

REM Installation
if not exist "node_modules" (
    echo [INFO] Installing dependencies...
    call %INSTALL_CMD%
    if %ERRORLEVEL% NEQ 0 (
        echo [ERROR] Installation failed
        pause
        exit /b 1
    )
    echo [SUCCESS] Installation completed
)

echo.
echo [INFO] Messis starting...
echo [INFO] The application will be accessible at http://localhost:{portselect}
echo [INFO] Press Ctrl+C to stop the server
echo.

REM Start
call %START_CMD%

echo.
echo [INFO] Server stopped.
pause