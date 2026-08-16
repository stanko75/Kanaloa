@echo off
setlocal

set "ROOT=%~dp0"
set "LIB_SOLUTION=%ROOT%lib\KanaloaLibrary.sln"
set "WEB_PROJECT=%ROOT%webApi\Kanaloa\Kanaloa.csproj"
set "PUBLISH_DIR=%ROOT%publish"

echo.
echo ========================================
echo Cleaning publish directory...
echo ========================================

if exist "%PUBLISH_DIR%" (
    rmdir /s /q "%PUBLISH_DIR%"
)

if exist "%PUBLISH_DIR%" (
    echo ERROR: Could not delete publish directory.
    exit /b 1
)

echo.
echo ========================================
echo Building Kanaloa libraries...
echo ========================================

dotnet build "%LIB_SOLUTION%" -c Release

if errorlevel 1 (
    echo.
    echo ERROR: Library build failed.
    exit /b 1
)

echo.
echo ========================================
echo Publishing Kanaloa Web API...
echo ========================================

dotnet publish "%WEB_PROJECT%" -c Release -o "%PUBLISH_DIR%"

if errorlevel 1 (
    echo.
    echo ERROR: Web API publish failed.
    exit /b 1
)

echo.
echo ========================================
echo Publish completed successfully.
echo ========================================
echo Output: %PUBLISH_DIR%
echo.

endlocal