@echo off

if not exist Mechabellum.exe echo Error: Uninstall script is not in game folder
if not exist Mechabellum.exe pause
if not exist Mechabellum.exe exit 1

del /f /q changelog.txt 
del /f /q winhttp.dll
del /f /q .doorstop_version
del /f /q doorstop_config.ini
del /f /q output_log.txt
del /f /q /s BepInEx
del /f /q /s dotnet
rmdir BepInEx /S /Q
rmdir dotnet /S /Q
del "%~f0"
