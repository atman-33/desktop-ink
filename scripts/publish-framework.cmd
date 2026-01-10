@echo off
setlocal

pushd "%~dp0.." || exit /b 1

echo Publishing framework-dependent version (requires .NET 10)...
echo This creates a smaller package that requires .NET 10 runtime installed.
echo.

set OUTPUT_DIR=publish\win-x64-framework
set PROJECT=src\DesktopInk\DesktopInk.csproj

rem Clean previous publish output
if exist "%OUTPUT_DIR%" (
    echo Cleaning previous output...
    rmdir /s /q "%OUTPUT_DIR%"
)

rem Publish as framework-dependent
dotnet publish "%PROJECT%" ^
    -c Release ^
    -r win-x64 ^
    --self-contained false ^
    -p:PublishSingleFile=true ^
    -p:PublishReadyToRun=false ^
    -o "%OUTPUT_DIR%" ^
    %*

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ Publish successful!
    echo.
    echo Output: %OUTPUT_DIR%\DesktopInk.exe
    echo.
    echo This executable requires .NET 10 Desktop Runtime to be installed.
    echo Typical size: 1-2 MB
    echo.
    echo Users can download .NET 10 Runtime from:
    echo https://dotnet.microsoft.com/download/dotnet/10.0
) else (
    echo.
    echo ✗ Publish failed!
    exit /b 1
)

popd
