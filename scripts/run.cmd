@echo off
setlocal

pushd "%~dp0.." || exit /b 1

rem Run DesktopInk (Debug)
dotnet run --project src\DesktopInk\DesktopInk.csproj -c Debug %*

popd
