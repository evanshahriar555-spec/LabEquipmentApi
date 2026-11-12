@echo off
setlocal
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Publish-LabEquipmentApi.ps1"
if errorlevel 1 (
  echo.
  echo Publishing did not complete. Read the error above.
)
pause
