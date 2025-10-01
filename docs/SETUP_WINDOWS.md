# Windows Setup

## Prerequisites
- .NET 8 SDK with Windows Desktop SDK
  - Easiest: Visual Studio 2022 with “.NET desktop development” workload
- Optional: PowerShell 7+ for better scripting experience

Verify:
- `dotnet --info` should list `Microsoft.WindowsDesktop.App 8.*`
- `dotnet --list-sdks` should include `8.*`

## Quick Start (Script)
- In PowerShell:
  - `cd C:\\Users\\eMich\\source\\synopsis_codex`
  - `powershell -ExecutionPolicy Bypass -File scripts\\setup-windows.ps1`

What the script does:
- Validates SDKs and Desktop runtime
- Restores, builds, and runs tests
- Launches the WPF app (`ReportGenerator.App`)

## Manual Steps
- `cd C:\\Users\\eMich\\source\\synopsis_codex`
- `dotnet restore`
- `dotnet build`
- `dotnet test`
- Run app: `dotnet run --project src\\ReportGenerator.App`

## First Run Notes
- SQLite DB `templates.db` is created on first start
- Logs are written to `Logs\\app-<date>.log`
- Main window lists templates and allows adding a new one

## Troubleshooting
- If build fails with WindowsDesktop targets missing, install the VS 2022 “.NET desktop development” workload or the Windows Desktop SDK for .NET 8.
- If NuGet restore fails, run `dotnet nuget locals all --clear` and retry `dotnet restore`.
