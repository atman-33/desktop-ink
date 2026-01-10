@echo off
setlocal

pushd "%~dp0.." || exit /b 1

echo Publishing both versions...
echo.
echo ========================================
echo 1/2: Self-Contained Version
echo ========================================
call scripts\publish.cmd
if %ERRORLEVEL% NEQ 0 exit /b 1

echo.
echo.
echo ========================================
echo 2/2: Framework-Dependent Version
echo ========================================
call scripts\publish-framework.cmd
if %ERRORLEVEL% NEQ 0 exit /b 1

echo.
echo.
echo ========================================
echo ✓ All versions published successfully!
echo ========================================
echo.
echo Self-Contained:      publish\win-x64-self-contained\DesktopInk.exe
echo Framework-Dependent: publish\win-x64-framework\DesktopInk.exe
echo.

popd
