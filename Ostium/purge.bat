@echo off
echo ========================================
echo          Ostium cleanup script
echo ========================================
echo.
echo All temporary session files will be deleted.
echo.

set /a ErrorCount=0

timeout /t 3 /nobreak > NUL

echo [1/9] Deleting the folder EnvironmentWebview...
if exist "EnvironmentWebview" (
    rd /s /q "EnvironmentWebview" 2>nul
    if errorlevel 1 (
        echo [ERROR] Unable to delete the folder
        set /a ErrorCount=%ErrorCount%+1
    ) else (
        if not exist "EnvironmentWebview" (
            echo [OK] File deleted
        ) else (
            echo [ERROR] The file still exists
            set /a ErrorCount=%ErrorCount%+1
        )
    )
) else (
    echo [INFO] File not found
)

echo [2/9] sourcepage removal...
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

echo [3/9] sourcepagelst removal...
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

echo [4/9] Archive-DB-FILES-FEED.bat removal...
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

echo [5/9] tempItemAdd.txt removal...
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

echo [6/9] removal scripts\temp.js.min removal...
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

echo [7/9] tmp.txt removal...
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

echo [8/9] Deleting the folder UrlUnshortenWorker.exe.WebView2...
if exist "UrlUnshortenWorker.exe.WebView2" (
    rd /s /q "UrlUnshortenWorker.exe.WebView2" 2>nul
    if errorlevel 1 (
        echo [ERROR] Unable to delete the folder
        set /a ErrorCount=%ErrorCount%+1
    ) else (
        if not exist "UrlUnshortenWorker.exe.WebView2" (
            echo [OK] File deleted
        ) else (
            echo [ERROR] The file still exists
            set /a ErrorCount=%ErrorCount%+1
        )
    )
) else (
    echo [INFO] File not found
)

echo [9/9] Deletion of the secure EnvironmentWebview folder...
if exist "X:\EnvironmentWebview" (
    rd /s /q "X:\EnvironmentWebview" 2>nul
    if errorlevel 1 (
        echo [ERROR] Unable to delete the folder
        set /a ErrorCount=%ErrorCount%+1
    ) else (
        if not exist "X:\EnvironmentWebview" (
            echo [OK] File deleted
        ) else (
            echo [ERROR] The file still exists
            set /a ErrorCount=%ErrorCount%+1
        )
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
    echo [WARNING] Errors detected! Manually run purge.bat again to force the deletion of temporary files.
    start .
)

echo.
pause