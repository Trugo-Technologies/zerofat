# Run the locally published ZeroFat.WebAPIs executable with env vars from .env.
param(
    [string]$PublishPath = "publish/local",
    [string]$Url
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$publishDir = Join-Path $repoRoot $PublishPath
$exe = Join-Path $publishDir "ZeroFat.WebAPIs.exe"
$envFile = Join-Path $repoRoot ".env"

if (-not (Test-Path $exe)) {
    Write-Error "Executable not found: $exe. Publish first: .\scripts\publish-local.ps1"
}

if (Test-Path $envFile) {
    Get-Content $envFile | ForEach-Object {
        $line = $_.Trim()
        if ($line -eq "" -or $line.StartsWith("#")) { return }

        $eq = $line.IndexOf("=")
        if ($eq -lt 1) { return }

        $name = $line.Substring(0, $eq).Trim()
        $value = $line.Substring($eq + 1).Trim().Trim('"')
        if ($name -and $value) {
            Set-Item -Path "Env:$name" -Value $value
        }
    }
}
else {
    Write-Warning ".env not found at $envFile - set DatabaseOptions__* and HangfireSettings__* manually."
}

if ($Url) {
    $env:ASPNETCORE_URLS = $Url
}
elseif (-not $env:ASPNETCORE_URLS) {
    $env:ASPNETCORE_URLS = "http://0.0.0.0:5000"
}

$listenUrl = $env:ASPNETCORE_URLS

Write-Host "Starting $exe"
Write-Host "URL: $listenUrl"
Write-Host ""

Push-Location $publishDir
try {
    & $exe
}
finally {
    Pop-Location
}
