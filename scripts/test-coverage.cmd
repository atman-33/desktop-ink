@echo off
setlocal

pushd "%~dp0.." || exit /b 1

echo Running tests with coverage...
echo.

rem Run tests with code coverage
dotnet test src\DesktopInk.Tests\DesktopInk.Tests.csproj ^
    --collect:"XPlat Code Coverage" ^
    --results-directory:.\TestResults ^
    --verbosity normal %*

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ Tests completed with coverage report!
    echo Coverage reports saved to: .\TestResults
) else (
    echo.
    echo ✗ Tests failed!
    exit /b 1
)

popd
