@echo off
echo ========================================
echo          Ostium cleanup script
echo ========================================
echo.

set /a ErrorCount=0

echo [1/7] Deleting the folder Ostium.exe.WebView2...
if exist "Ostium.exe.WebView2" (
    rd /s /q "Ostium.exe.WebView2" 2>nul
    if errorlevel 1 (
        echo [ERROR] Unable to delete the folder
        set /a ErrorCount=%ErrorCount%+1
    ) else (
        if not exist "Ostium.exe.WebView2" (
            echo [OK] File deleted
        ) else (
            echo [ERROR] The file still exists
            set /a ErrorCount=%ErrorCount%+1
        )
    )
) else (
    echo [INFO] File not found
)

echo [2/7] sourcepage removal...
if exist "sourcepage" (
    del /s /q "sourcepage" 2>nul
    if not exist "sourcepage" (
        echo [OK] File deleted
    ) else (
        echo [ERROR] Unable to delete
        set /a ErrorCount=%ErrorCount%+1
    )
) else (
    echo [INFO] File not found
)

echo [3/7] sourcepagelst removal...
if exist "sourcepagelst" (
    del /s /q "sourcepagelst" 2>nul
    if not exist "sourcepagelst" (
        echo [OK] File deleted
    ) else (
        echo [ERROR] Unable to delete
        set /a ErrorCount=%ErrorCount%+1
    )
) else (
    echo [INFO] File not found
)

echo [4/7] Archive-DB-FILES-FEED.bat removal...
if exist "Archive-DB-FILES-FEED.bat" (
    del /s /q "Archive-DB-FILES-FEED.bat" 2>nul
    if not exist "Archive-DB-FILES-FEED.bat" (
        echo [OK] File deleted
    ) else (
        echo [ERROR] Unable to delete
        set /a ErrorCount=%ErrorCount%+1
    )
) else (
    echo [INFO] File not found
)

echo [5/7] tempItemAdd.txt removal...
if exist "tempItemAdd.txt" (
    del /s /q "tempItemAdd.txt" 2>nul
    if not exist "tempItemAdd.txt" (
        echo [OK] File deleted
    ) else (
        echo [ERROR] Unable to delete
        set /a ErrorCount=%ErrorCount%+1
    )
) else (
    echo [INFO] File not found
)

echo [6/7] removal scripts\temp.js.min removal...
if exist "scripts\temp.js.min" (
    del /s /q "scripts\temp.js.min" 2>nul
    if not exist "scripts\temp.js.min" (
        echo [OK] File deleted
    ) else (
        echo [ERROR] Unable to delete
        set /a ErrorCount=%ErrorCount%+1
    )
) else (
    echo [INFO] File not found
)

echo [7/7] tmp.txt removal...
if exist "tmp.txt" (
    del /s /q "tmp.txt" 2>nul
    if not exist "tmp.txt" (
        echo [OK] File deleted
    ) else (
        echo [ERROR] Unable to delete
        set /a ErrorCount=%ErrorCount%+1
    )
) else (
    echo [INFO] File not found
)

echo.
echo ========================================
echo Cleaning results
echo ========================================
echo.
echo Number of errors: %ErrorCount%
echo.

if "%ErrorCount%"=="0" (
    echo [SUCCESS] All items have been cleaned or not exists
) else (
    echo [ATTENTION] Errors have been detected
)

echo.
pause