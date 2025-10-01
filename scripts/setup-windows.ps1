param(
  [switch]$NoRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Require-Command($name) {
  if (-not (Get-Command $name -ErrorAction SilentlyContinue)) {
    throw "Required command not found: $name"
  }
}

Write-Host "Checking prerequisites..." -ForegroundColor Cyan
Require-Command dotnet

$info = & dotnet --info
if ($LASTEXITCODE -ne 0) { throw "dotnet --info failed" }

$hasDesktop = (& dotnet --list-runtimes) -match 'Microsoft.WindowsDesktop.App 8\.'
if (-not $hasDesktop) {
  Write-Warning "Windows Desktop runtime for .NET 8 not found. Install VS 2022 '.NET desktop development' workload."
}

Write-Host "Restoring packages..." -ForegroundColor Cyan
& dotnet restore

Write-Host "Building solution..." -ForegroundColor Cyan
& dotnet build

Write-Host "Running tests..." -ForegroundColor Cyan
& dotnet test

if (-not $NoRun) {
  Write-Host "Launching WPF app..." -ForegroundColor Cyan
  & dotnet run --project src/ReportGenerator.App
}

Write-Host "Done." -ForegroundColor Green

