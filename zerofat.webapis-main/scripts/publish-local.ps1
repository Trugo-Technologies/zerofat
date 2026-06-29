# Publish ZeroFat.WebAPIs for local execution (framework-dependent).
param(
    [string]$OutputPath = "publish/local",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "src/Host/ZeroFat.WebAPIs/ZeroFat.WebAPIs.csproj"
$output = Join-Path $repoRoot $OutputPath

Push-Location $repoRoot
try {
    dotnet publish $project -c $Configuration -o $output
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    $exe = Join-Path $output "ZeroFat.WebAPIs.exe"
    if (-not (Test-Path $exe)) {
        Write-Error "Publish completed but executable was not found: $exe"
    }

    Write-Host ""
    Write-Host "Published to: $output"
    Write-Host "Executable:   $exe"
    Write-Host ""
    Write-Host "Run with: .\scripts\run-published.ps1"
}
finally {
    Pop-Location
}
