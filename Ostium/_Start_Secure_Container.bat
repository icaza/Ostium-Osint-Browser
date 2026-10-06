@echo off
setlocal

set VERACRYPT=C:\...\VeraCrypt-x64.exe
set CONTAINER=C:\...\renamefile.hc
set DRIVE=X:\\
set OSTIUM=C:\...\Ostium.exe

echo.
echo ==========================================
echo      Ostium - EnvironmentWebview
echo ==========================================
echo.

if not exist "%VERACRYPT%" (
    echo ERROR : VeraCrypt-x64.exe not found.
    pause
    exit /b 1
)

if not exist "%CONTAINER%" (
    echo ERROR : Container VeraCrypt not found :
    echo %CONTAINER%
    pause
    exit /b 1
)

if not exist "%OSTIUM%" (
    echo ERROR : Ostium.exe not found :
    echo %OSTIUM%
    pause
    exit /b 1
)

REM Check that X is not already in use
if exist "%DRIVE%\" (
    echo ERROR : The drive %DRIVE% is already in use.
    pause
    exit /b 1
)

echo Mounting EnvironmentWebview on %DRIVE%...
echo.

"%VERACRYPT%" /q /v "%CONTAINER%" /l %DRIVE%

echo Awaiting assembly...

set /a COUNT=0

:WAIT_MOUNT
if exist "%DRIVE%\" goto MOUNT_OK

set /a COUNT+=1

if %COUNT% GEQ 30 (
    echo.
    echo ERROR: The volume could not be mounted.
    pause
    exit /b 1
)

timeout /t 1 /nobreak >nul
goto WAIT_MOUNT

:MOUNT_OK

echo Volume successfully mounted on %DRIVE%.
echo.
echo Launch of Ostium...
echo.

REM Launching Ostium and waiting for it to close
start "" /wait "%OSTIUM%" %DRIVE%

echo.
echo Ostium is finished..
echo.
echo Unmount %DRIVE%...

"%VERACRYPT%" /q /d %DRIVE%

REM Waiting for the reader to disappear
set /a COUNT=0

:WAIT_DISMOUNT
if not exist "%DRIVE%\" goto DISMOUNT_OK

set /a COUNT+=1

if %COUNT% GEQ 15 (
    echo.
    echo WARNING: Drive %DRIVE% still appears to be mounted..
    echo Manual disassembly required.
    pause
    exit /b 1
)

timeout /t 1 /nobreak >nul
goto WAIT_DISMOUNT

:DISMOUNT_OK

echo.
echo Volume successfully unmounted.
echo.
echo Operation complete.

endlocal
exit /b 0