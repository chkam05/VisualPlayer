@echo off
setlocal

if "%~1"=="--help" goto usage
if "%~1"=="-h" goto usage
if not "%~2"=="" goto invalid

set "BUILD_ARCH=%PROCESSOR_ARCHITECTURE%"
if defined PROCESSOR_ARCHITEW6432 set "BUILD_ARCH=%PROCESSOR_ARCHITEW6432%"
set "BUILD_RUNTIME=win-x64"
if /I "%BUILD_ARCH%"=="ARM64" set "BUILD_RUNTIME=win-arm64"
if /I "%BUILD_ARCH%"=="x86" set "BUILD_RUNTIME=win-x86"
if not "%~1"=="" set "BUILD_RUNTIME=%~1"

if "%BUILD_RUNTIME%"=="win-x64" goto publish
if "%BUILD_RUNTIME%"=="win-arm64" goto publish
if "%BUILD_RUNTIME%"=="win-x86" goto publish
goto invalid

:publish
where dotnet >nul 2>&1
if errorlevel 1 (
    echo ERROR: Install the .NET 10 SDK and restart your terminal.
    exit /b 1
)

set "BUILD_OUTPUT=%~dp0artifacts\publish\%BUILD_RUNTIME%"
echo Publishing VisualPlayer Release for %BUILD_RUNTIME%...
dotnet publish "%~dp0VisualPlayer\VisualPlayer.csproj" --configuration Release --runtime "%BUILD_RUNTIME%" --self-contained true --output "%BUILD_OUTPUT%" -p:PublishTrimmed=false -p:PublishSingleFile=false -p:UseAppHost=true
if errorlevel 1 exit /b 1

echo.
echo Ready: "%BUILD_OUTPUT%\VisualPlayer.exe"
echo Copy the entire output folder when distributing the application.
exit /b 0

:invalid
echo ERROR: Invalid arguments.
call :usage
exit /b 2

:usage
echo Usage: build_windown.bat [win-x64^|win-arm64^|win-x86]
echo Default: the architecture of this Windows computer.
exit /b 0
