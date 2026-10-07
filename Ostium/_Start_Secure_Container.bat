@echo off
setlocal

set VERACRYPT=C:\...\VeraCrypt-x64.exe
set CONTAINER=C:\...\EnvironmentWebview.hc
set DRIVE=X:\\
set OSTIUM=C:\...\Ostium.exe

echo.
echo ================================================================
echo                   OPEN ENCRYPTED VIRTUAL DISK
echo ================================================================
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
    echo.
    echo To start a new session on the virtual disk, use the RestartSession tool, select the EnvironmentWebview directory 
    echo on the virtual disk, then choose an existing session or create a new one; then click the RESTART SESSION button.
    echo Finally, click the New Session button on the Ostium toolbar or restart Ostium if the program is not running.
    echo.
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

cls
echo ================================================================
echo                 CLOSING ENCRYPTED VIRTUAL DISK
echo ================================================================
echo.
echo   This session created the encrypted virtual disk %DRIVE%
echo.
echo   Do you want to dismount it now?
echo.
echo   [Y] YES : if no other Ostium Osint session is open.
echo.
echo   [N] NO  : if other sessions are still using this disk.
echo       Dismounting it would make them malfunction.
echo.
echo ----------------------------------------------------------------
echo.
echo   AT THE END OF YOUR LAST SESSION, dismount it yourself:
echo.
echo   Method 1: in VeraCrypt, select %DRIVE% then click "Dismount".
echo.
echo   Method 2: from the command line:
echo     "%VERACRYPT%" /q /d %DRIVE%
echo.
echo   While it stays mounted, your data remains accessible.
echo.
choice /c YN /n /m "Your choice: [Y] Yes, dismount / [N] No, keep mounted: "

if errorlevel 2 goto KEEP_MOUNTED
if errorlevel 1 goto UNMOUNT_VOLUME

:UNMOUNT_VOLUME

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

goto END

:KEEP_MOUNTED

echo.
echo Volume %DRIVE% will remain mounted.
echo Close all other Ostium sessions before unmounting the volume.
echo.
echo Operation complete.

:END
endlocal
exit /b 0