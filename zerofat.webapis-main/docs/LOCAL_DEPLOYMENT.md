# Local deployment (publish and run)

This document describes how to **publish** `ZeroFat.WebAPIs` as a local executable and **run** it outside of `dotnet run`.

For day-to-day development, see [SETUP.md](SETUP.md). For container/AWS deployment, see [DEPLOYMENT.md](DEPLOYMENT.md).

## Prerequisites

- .NET SDK 9.0 (to publish)
- .NET Runtime 9.0 (for framework-dependent publish; included with the SDK)
- A reachable PostgreSQL database (or update `DatabaseOptions` for your provider)
- Repo-root `.env` file with local secrets (copy from `.env.example`)

## Publish

From the repo root:

### Framework-dependent (recommended for local Windows)

Smaller output; requires .NET 9 runtime on the machine.

```powershell
dotnet publish src/Host/ZeroFat.WebAPIs/ZeroFat.WebAPIs.csproj -c Release -o publish/local
```

Output:

- `publish/local/ZeroFat.WebAPIs.exe` — Windows app host
- `publish/local/ZeroFat.WebAPIs.dll` — main assembly
- `publish/local/Configurations/` — JSON config copied at publish time

Or use the helper script:

```powershell
.\scripts\publish-local.ps1
```

### Self-contained (optional)

Bundles the .NET runtime; larger folder, no separate runtime install required.

```powershell
dotnet publish src/Host/ZeroFat.WebAPIs/ZeroFat.WebAPIs.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -o publish/local-selfcontained
```

For Linux/macOS, change `-r` to `linux-x64` or `osx-x64`.

## Provide configuration

The published app reads JSON from `Configurations/` and **environment variables** (env vars override JSON).

Set secrets via environment variables before starting the exe. Minimum for PostgreSQL:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://0.0.0.0:5000"

$env:DatabaseOptions__Provider = "postgresql"
$env:DatabaseOptions__ConnectionString = "Host=localhost;Port=5432;Database=zerofatdb;Username=postgres;Password=<PASSWORD>;"

$env:HangfireSettings__Storage__ConnectionString = "Host=localhost;Port=5432;Database=zerofatdb;Username=postgres;Password=<PASSWORD>;"
```

Optional but commonly needed locally:

- `SecuritySettings__JwtSettings__Key` — JWT signing key (min 32 characters)
- `HangfireSettings__Credentials__Password` — Hangfire dashboard password

See [CONFIGURATION.md](CONFIGURATION.md) and `.env.example` for the full list.

## Run the published exe

From the repo root, after publishing:

```powershell
cd publish/local
.\ZeroFat.WebAPIs.exe
```

Or use the helper script (loads variables from repo-root `.env`):

```powershell
.\scripts\run-published.ps1
```

By default the helper listens on `http://0.0.0.0:5000` (set via `ASPNETCORE_URLS` in `.env`). Override with:

```powershell
.\scripts\run-published.ps1 -Url "http://localhost:5000"
```

## Verify it is running

1. Console should show `server booting up..` and then `Now listening on: http://0.0.0.0:5000` (or your chosen URL).
2. Open:
   - Health (this machine): `http://localhost:5000/healthcheck`
   - Health (LAN / phone): `http://<your-pc-ip>:5000/healthcheck` (e.g. `http://192.168.1.17:5000/healthcheck`)
   - Swagger: `http://localhost:5000/swagger`
   - Hangfire: `http://localhost:5000/jobs`

Use `http://0.0.0.0:5000` in `ASPNETCORE_URLS` if you need access from other devices on the same network. `http://localhost:5000` only works on the same PC.

Quick PowerShell check:

```powershell
Invoke-WebRequest -Uri "http://localhost:5000/healthcheck" -UseBasicParsing
```

Expected: HTTP `200` when the app and database are healthy.

## Migrations

Same as other deployment modes: EF Core migrations run on startup via module `IDbInitializer` implementations.

Ensure the database exists and credentials in `DatabaseOptions__ConnectionString` are correct before starting the exe.

## Troubleshooting

| Symptom | Likely cause |
|--------|----------------|
| `password authentication failed for user "postgres"` | Wrong DB password in env vars or `.env` |
| `ERR_CONNECTION_REFUSED` on `http://<ip>:5000` | App is bound to `localhost` only; set `ASPNETCORE_URLS=http://0.0.0.0:5000` and restart. Allow port 5000 in Windows Firewall if needed. |
| `address already in use` on startup | Another instance is already running on that port; stop it or change `ASPNETCORE_URLS` in `.env` |
| `Unable to connect to the remote server` on health check | App crashed during startup (check console output) or wrong port |
| `You must install or update .NET` | Framework-dependent publish used but .NET 9 runtime is missing; install runtime or use self-contained publish |
| `API key cannot be the empty string` (Stripe) | Set `StripeSettings__SecretKey` in `.env` to a Stripe test key, or remove the empty line so it is not loaded |
| Hangfire startup failure | `HangfireSettings__Storage__ConnectionString` missing or invalid |

## Publish output layout

```
publish/local/
  ZeroFat.WebAPIs.exe          # Windows executable entry point
  ZeroFat.WebAPIs.dll
  Configurations/              # hangfire.json, modules.json, security.json, ...
  appsettings.json
  appsettings.Development.json
  Localization/
  Files/
  Logs/                        # Serilog file logs (created at runtime)
```

The `publish/` folder is gitignored. Re-run `dotnet publish` after code or configuration changes.
