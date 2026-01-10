@echo off
setlocal

pushd "%~dp0.." || exit /b 1

rem Build solution (Debug and Release)
dotnet build desktop-ink.slnx -c Debug %*
dotnet build desktop-ink.slnx -c Release %*

popd
