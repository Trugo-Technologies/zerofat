## ZeroFat WebAPIs (.NET 9)

ZeroFat is a modular monolith Web API solution built on **.NET 9**. It is structured as a single deployable host that composes multiple business modules (each with its own Domain/Application/Infrastructure layers) while sharing cross-cutting building blocks.

Typical use-cases include:

- Identity & access (users/roles/permissions)
- Client portal domain
- Nutrition planning domain
- Workouts/gym domain

The host exposes HTTP APIs (Swagger enabled in development) and runs background processing (Hangfire) and health checks.

The entry point is the Host project:

- `src/Host/ZeroFat.WebAPIs/ZeroFat.WebAPIs.csproj`

On startup, the host loads module configuration files and runs module DB initializers (migrations + seed).

## Solution structure (high level)

- `src/Host/ZeroFat.WebAPIs` – host Web API
- `src/BuildingBlocks` – shared domain/application/infrastructure
- `src/Modules/*` – modules (Users, Workouts, NutriPlan, ClientPortal, ...)
- `src/Migrations/Migrators.*` – EF Core migrations assemblies per database provider

## Architecture

- **Modular monolith**: modules are developed as separate projects but deployed together.
- **Layered modules**: each module typically follows `Domain` → `Application` → `Infrastructure` → `Api`.
- **Shared building blocks**: cross-cutting concerns live under `src/BuildingBlocks`.
- **Database per host (schema-per-module pattern)**: EF Core migrations are organized per module and database provider via `src/Migrations/Migrators.*`.

## Tech stack

- .NET 9 / ASP.NET Core
- EF Core (provider selected via configuration)
- PostgreSQL (default in repo configs), plus optional SQL Server / MySQL / SQLite
- Hangfire (background jobs)
- Serilog (logging)
- HealthChecks UI
- JWT-based security (configured in `security.json`)

## Modules

The solution is organized into modules under `src/Modules`:

- **Users**: identity, roles, permissions, authentication support
- **ClientPortal**: client-facing workflows and settings
- **NutriPlan**: nutrition planning workflows
- **Workouts (GymUp)**: workout/training workflows

## Prerequisites

- .NET SDK **9.0**
- A database supported by the project (configured via `DatabaseOptions:Provider`):
  - PostgreSQL (default in configs)
  - SQL Server
  - MySQL
  - SQLite

If you run locally with HTTPS, ensure a dev certificate is available:

- `dotnet dev-certs https --trust`

## Build

From the repo root:

- `dotnet restore ZeroFat.sln`
- `dotnet build ZeroFat.sln`

## Configuration

The host loads configuration from these files (in order), plus environment variables:

- `src/Host/ZeroFat.WebAPIs/appsettings.json`
- `src/Host/ZeroFat.WebAPIs/appsettings.{Environment}.json` (optional)
- `src/Host/ZeroFat.WebAPIs/Configurations/*.json`
- `src/Host/ZeroFat.WebAPIs/Configurations/*.{Environment}.json` (optional)

The main configuration files are:

- `src/Host/ZeroFat.WebAPIs/Configurations/modules.json` (DatabaseOptions, module settings)
- `src/Host/ZeroFat.WebAPIs/Configurations/security.json` (JWT/security settings)
- `src/Host/ZeroFat.WebAPIs/Configurations/hangfire.json` (jobs dashboard + storage)
- `src/Host/ZeroFat.WebAPIs/Configurations/storage.json` (file storage provider)
- `src/Host/ZeroFat.WebAPIs/Configurations/cors.json`
- `src/Host/ZeroFat.WebAPIs/Configurations/logger.json`
- `src/Host/ZeroFat.WebAPIs/Configurations/healthcheck.json`
- `src/Host/ZeroFat.WebAPIs/Configurations/localization.json`
- `src/Host/ZeroFat.WebAPIs/Configurations/paymob.json`
- `src/Host/ZeroFat.WebAPIs/Configurations/stripe.json`
- `src/Host/ZeroFat.WebAPIs/Configurations/sms.json`

### Feature flags / enabled modules

Module toggles live in `src/Host/ZeroFat.WebAPIs/appsettings.json` under `FeatureManagement` (e.g. `UsersModule`, `WorkoutModule`, ...).

### Recommended local setup (do not commit secrets)

Prefer overriding secrets using environment variables (ASP.NET Core configuration). Examples:

- `DatabaseOptions__Provider=postgresql`
- `DatabaseOptions__ConnectionString=Host=localhost;Port=5433;Database=zerofatdb;Username=postgres;Password=<PASSWORD>;`
- `HangfireSettings__Storage__ConnectionString=Host=localhost;Port=5433;Database=zerofatdb;Username=postgres;Password=<PASSWORD>;`
- `SecuritySettings__JwtSettings__Key=<JWT_SIGNING_KEY>`
- `HangfireSettings__Credentials__User=<DASHBOARD_USER>`
- `HangfireSettings__Credentials__Password=<DASHBOARD_PASSWORD>`

For module-specific settings, use the same `__` nesting convention.

If you use the checked-in JSON configuration files as a starting point, ensure you replace any credentials/keys with your own local values.

Production note (current AWS setup): configuration is shipped inside the container image and the deployed image tag is kept as `v1`.

## Running locally

### Visual Studio

1. Open `ZeroFat.sln`
2. Set startup project to `ZeroFat.WebAPIs`
3. Run using the `http` profile

By default the host listens on:

- `http://localhost:5000`
- `https://localhost:7000`

### .NET CLI

From the repo root:

- `dotnet restore ZeroFat.sln`
- `dotnet build ZeroFat.sln`
- `dotnet run --project src/Host/ZeroFat.WebAPIs/ZeroFat.WebAPIs.csproj`

## Testing

If the repo includes tests in your branch/clone:

- `dotnet test ZeroFat.sln`

### Docker

- Build: `docker build -t zerofat.webapis .`
- Run (example): `docker run --rm -p 8080:8080 -e ASPNETCORE_URLS=http://+:8080 zerofat.webapis`

If you override configuration, pass environment variables to the container (recommended) or mount configuration files.

## Documentation

Additional handover-oriented docs:

- `docs/INDEX.md`
- `docs/SETUP.md`
- `docs/CONFIGURATION.md`
- `docs/ARCHITECTURE.md`
- `docs/DEPLOYMENT.md`
- `docs/AWS_ECS_ECR.md`
- `docs/OPERATIONS.md`
- `docs/HANDOVER.md`

Project-wide policies:

- `SECURITY.md`
- `CHANGELOG.md`

## Database, migrations, and seeding

On application startup, each module's `IDbInitializer`:

- applies pending EF Core migrations
- seeds initial data

This means the DB user must have permissions to create/update schema objects.

If you want to run migrations explicitly (instead of relying on startup), use the provider-specific migrator project for your selected database provider (e.g. `src/Migrations/Migrators.PostgreSQL`).

### Example: PostgreSQL (local)

If you want a quick local database using Docker:

- `docker run --name zerofat-postgres --rm -e POSTGRES_PASSWORD=<PASSWORD> -e POSTGRES_DB=zerofatdb -p 5433:5432 postgres:16`

Then set `DatabaseOptions__ConnectionString` accordingly.

## Useful endpoints

- Swagger: `/swagger`
- Hangfire dashboard: `/jobs`
- Health (simple): `/healthcheck`
- Health UI: `/health-ui`
- Health API: `/health-api`

## Troubleshooting

### App fails on startup due to database connectivity

- Verify `DatabaseOptions__Provider` and `DatabaseOptions__ConnectionString`.
- Ensure the database is reachable from where the app runs (localhost vs container networking).
- Ensure the DB user has permissions to apply migrations.

### Hangfire dashboard is not accessible

- Confirm the app is running and you are hitting the correct base URL.
- Check `HangfireSettings` configuration (route, credentials, storage connection string).

### HTTPS certificate issues

- Run `dotnet dev-certs https --trust`.
- If you run in Docker, prefer HTTP inside the container and terminate TLS at a reverse proxy.

## Creating EF Core migrations (for developers)

The solution includes `Commands.txt` with migration examples. Typical workflow:

- Choose the correct DbContext (e.g. `ZeroFat.Users.Infrastructure.Persistence.Context.UsersContext`)
- Output migrations into the provider-specific migrator project (e.g. `src/Migrations/Migrators.PostgreSQL/Migrations/Users`)

The exact command depends on your tooling (`dotnet ef` or Package Manager Console) and your selected provider.

## Contributing

- See `CONTRIBUTING.md` for guidelines.
- Keep module boundaries intact (avoid leaking module types across layers).

## License

This project is licensed with the [MIT license](LICENSE).