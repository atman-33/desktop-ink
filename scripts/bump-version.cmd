@echo off
REM Wrapper script for bump-version.ps1
REM Usage: bump-version.cmd <version> [-Push]
REM Example: bump-version.cmd 1.1.0
REM Example: bump-version.cmd 1.1.0 -Push

cd /d "%~dp0"
powershell -ExecutionPolicy Bypass -File "%~dp0bump-version.ps1" %*
