@echo off
setlocal enabledelayedexpansion

pushd "%~dp0.." || exit /b 1

rem Check if --debug-version argument is provided
set DEBUG_VERSION=
:parse_args
if "%~1"=="" goto end_parse
if "%~1"=="--debug-version" (
    set DEBUG_VERSION=%~2
    shift
    shift
    goto parse_args
)
shift
goto parse_args
:end_parse

rem If no argument provided, ask interactively
if "%DEBUG_VERSION%"=="" (
    echo.
    echo ========================================
    echo   Desktop Ink - Debug Run
    echo ========================================
    echo.
    set /p USE_DEBUG="Use debug version for update check? (y/N): "
    
    if /i "!USE_DEBUG!"=="y" (
        set /p DEBUG_VERSION="Enter debug version (e.g., 1.0.0): "
    )
)

rem Set environment variable if debug version is specified
if not "%DEBUG_VERSION%"=="" (
    set DESKTOPINK_DEBUG_VERSION=%DEBUG_VERSION%
    echo.
    echo [DEBUG] Using custom version: %DEBUG_VERSION%
    echo.
)

rem Run DesktopInk (Debug)
dotnet run --project src\DesktopInk\DesktopInk.csproj -c Debug

popd
