# Windows Setup: Visual Studio & VS Code

## Prerequisites
- Windows 10/11, .NET 8 SDK + Windows Desktop SDK
- Git, PowerShell
- WPF requires Windows (cannot run on Linux/WSL)

Verify:
- `dotnet --info` shows `Microsoft.WindowsDesktop.App 8.*`
- `dotnet --list-sdks` includes `8.*`

## Visual Studio 2022
- Open `ReportGenerator.sln`.
- Set startup project: right‑click `ReportGenerator.App` → Set as Startup Project.
- Select configuration: `Debug | Any CPU`.
- Restore/build: Build → Clean Solution, then Build Solution.
- Run: F5 (Debug) or Ctrl+F5 (Run without debugging).
- Tests: Test → Run All Tests (uses xUnit). View results in Test Explorer.
- NuGet: Tools → NuGet Package Manager → Restore/Manage if restore fails.

## VS Code
- Install extensions:
  - `C# Dev Kit` (ms-dotnettools.csdevkit)
  - `C#` (ms-dotnettools.csharp) if prompted
- Terminal commands (from repo root):
  - `dotnet restore`
  - `dotnet build`
  - `dotnet test`
- Debug WPF app:
  1) Press F5 → choose “.NET” to generate assets.
  2) Update `.vscode/launch.json` (example):
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Launch ReportGenerator.App",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/src/ReportGenerator.App/bin/Debug/net8.0-windows/ReportGenerator.App.exe",
      "cwd": "${workspaceFolder}",
      "console": "internalConsole"
    }
  ]
}
```
- Optional build task (`.vscode/tasks.json`):
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "args": ["build"],
      "type": "process",
      "problemMatcher": "$msCompile"
    }
  ]
}
```

## First Run
- DB `templates.db` is created automatically.
- Logs: `Logs/app-<date>.log`.

## Troubleshooting
- Missing Windows Desktop SDK: install VS 2022 “.NET desktop development”.
- Stale caches: `dotnet nuget locals all --clear` then `dotnet restore`.
