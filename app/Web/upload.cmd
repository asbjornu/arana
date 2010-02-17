@echo off
python D:\Programs\Google\google_appengine\appcfg.py update aranalibtest/

if %errorlevel% NEQ 0 (
  echo.
  echo.Upload failed!
  echo.
  pause
)