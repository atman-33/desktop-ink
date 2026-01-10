@echo off
setlocal

pushd "%~dp0.." || exit /b 1

echo Running all tests...
echo.

rem Run tests with normal verbosity
dotnet test src\DesktopInk.Tests\DesktopInk.Tests.csproj --verbosity normal %*

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ All tests passed successfully!
) else (
    echo.
    echo ✗ Some tests failed!
    exit /b 1
)

popd
