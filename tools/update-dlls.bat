@echo off
setlocal enabledelayedexpansion

set "SCRIPT_DIR=%~dp0.."
set "DLL_DIR=%SCRIPT_DIR%\referencedlls"

set "sourceDir=C:\Other\Games\Steam\steamapps\common\Mechabellum\BepInEx\interop"
set "destDir=!DLL_DIR!"

cd "!sourceDir!"

for /r %%i in (*) do (
    set "sourceFile=%%~nxi"

    if exist "!destDir!\!sourceFile!" (
        fc "%%i" "!destDir!\!sourceFile!" > nul
        if !errorlevel! equ 0 (
            echo !sourceFile! OK
        ) else (
            copy "%%i" "!destDir!\" > nul
            echo !sourceFile! UPDATED
        )
    )
)

echo Done.

endlocal
