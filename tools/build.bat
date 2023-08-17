@echo off

set PROJECT_NAME="mechabellum-modding"
set SRC_DIR="%~dp0\..\%PROJECT_NAME%"
set DIST_DIR="%~dp0\..\dist"
set BUILD_DIR="%~dp0\..\%PROJECT_NAME%\bin\Debug\net6.0"
set TOOLS_DIR="%~dp0\..\tools"
set RELEASE_DIR="%~dp0\..\release"

if not exist %DIST_DIR% mkdir %DIST_DIR%
if not exist %RELEASE_DIR% mkdir %RELEASE_DIR%

cd %SRC_DIR% || exit 1

dotnet build

xcopy %BUILD_DIR%\com.github.toasterparty.%PROJECT_NAME%.dll %DIST_DIR% /y /q || exit 1

echo.
echo Successfully built %PROJECT_NAME%
echo.
