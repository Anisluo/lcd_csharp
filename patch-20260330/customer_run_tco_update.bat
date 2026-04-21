@echo off
setlocal
cd /d "%~dp0"
echo Running customer TCO minimal update...
powershell -ExecutionPolicy Bypass -File "%~dp0apply_tco_minimal_update_v3.ps1"
echo.
echo If there were no errors, the customer source update is complete.
echo Press any key to close.
pause >nul
