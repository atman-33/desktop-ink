@echo off
setlocal

pushd "%~dp0.." || exit /b 1

echo Starting test watch mode...
echo Tests will run automatically when files change.
echo Press Ctrl+C to stop.
echo.

rem Run tests in watch mode
dotnet watch test src\DesktopInk.Tests\DesktopInk.Tests.csproj %*

popd
