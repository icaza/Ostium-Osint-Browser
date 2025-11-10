@echo off
rd /s /q "Ostium.exe.WebView2" 2>nul
if %errorlevel% neq 0 (
    echo Error deleting directory Ostium.exe.WebView2
)
del /s "sourcepage" 2>nul
if %errorlevel% neq 0 (
    echo Error deleting sourcepage
)
del /s "sourcepagelst" 2>nul
if %errorlevel% neq 0 (
    echo Error deleting sourcepagelst
)
del /s "Archive-DB-FILES-FEED.bat" 2>nul
if %errorlevel% neq 0 (
    echo Error deleting Archive-DB-FILES-FEED.bat
)
del /s "tempItemAdd.txt" 2>nul
if %errorlevel% neq 0 (
    echo Error deleting tempItemAdd.txt
)
del /s "scripts\temp.js.min" 2>nul
if %errorlevel% neq 0 (
    echo Error deleting scripts\temp.js.min
)
del /s "tmp.txt" 2>nul
if %errorlevel% neq 0 (
    echo Error deleting tmp.txt
)
exit
