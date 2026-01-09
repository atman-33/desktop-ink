@echo off
setlocal

pushd "%~dp0.." || exit /b 1

rem Run DesktopInk (Release)
dotnet run --project src\DesktopInk\DesktopInk.csproj -c Release %*

popd
