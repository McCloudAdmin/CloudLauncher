@echo off
setlocal

REM === CONFIG ===
set PROJECT=CloudLauncher.csproj
set OUTPUT_DIR=publish
set CONFIG=Release

REM === ARCHITECTURES TO BUILD ===
set RIDS=win-x86 win-x64 win-arm64

echo Publishing project %PROJECT% for all Windows architectures...

for %%R in (%RIDS%) do (
    echo.
    echo ===== Publishing for %%R =====
    dotnet publish %PROJECT% -c %CONFIG% -r %%R --self-contained true ^
        /p:PublishSingleFile=true ^
        --output %OUTPUT_DIR%\%%R
)

echo.
echo ✅ All builds finished.
endlocal
pause
