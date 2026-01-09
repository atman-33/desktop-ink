@echo off
setlocal

pushd "%~dp0.." || exit /b 1

rem Build solution (Release)
dotnet build desktop-ink.slnx -c Release %*

popd
