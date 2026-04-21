@echo off
setlocal
cd /d "%~dp0"
echo Running flip update (仪器固定，翻转式)...
powershell -ExecutionPolicy Bypass -File "%~dp0apply_flip_update.ps1"
echo.
echo If there were no errors, the update is complete.
echo Press any key to close.
pause >nul
