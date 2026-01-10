@echo off
setlocal

pushd "%~dp0.." || exit /b 1

echo Publishing self-contained single-file executable (win-x64)...
echo This will create a standalone .exe with .NET runtime included.
echo.

set OUTPUT_DIR=publish\win-x64-self-contained
set PROJECT=src\DesktopInk\DesktopInk.csproj

rem Clean previous publish output
if exist "%OUTPUT_DIR%" (
    echo Cleaning previous output...
    rmdir /s /q "%OUTPUT_DIR%"
)

rem Publish as self-contained single file
dotnet publish "%PROJECT%" ^
    -c Release ^
    -r win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:PublishReadyToRun=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -p:PublishTrimmed=false ^
    -o "%OUTPUT_DIR%" ^
    %*

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ Publish successful!
    echo.
    echo Output: %OUTPUT_DIR%\DesktopInk.exe
    echo.
    echo This executable includes .NET runtime and can run on any Windows x64 machine.
    echo Typical size: 100-120 MB
) else (
    echo.
    echo ✗ Publish failed!
    exit /b 1
)

popd
