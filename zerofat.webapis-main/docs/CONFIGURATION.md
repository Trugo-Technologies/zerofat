# Configuration

The host project loads configuration from JSON files and environment variables.

## Where configuration lives

- Host base settings:
  - `src/Host/ZeroFat.WebAPIs/appsettings.json`
  - `src/Host/ZeroFat.WebAPIs/appsettings.{Environment}.json` (optional)
- Host configuration directory:
  - `src/Host/ZeroFat.WebAPIs/Configurations/*.json`
  - `src/Host/ZeroFat.WebAPIs/Configurations/*.{Environment}.json` (optional)

## Important configuration sections

### Database

Database selection and connection settings are configured through `DatabaseOptions`:

- `DatabaseOptions:Provider`
- `DatabaseOptions:ConnectionString`

The provider determines EF Core configuration and the migrations assembly used (see `src/Migrations/Migrators.*`).

### Feature flags / enabled modules

Module toggles:

- `FeatureManagement:*` in `src/Host/ZeroFat.WebAPIs/appsettings.json`

### Security / JWT

- `SecuritySettings` in `src/Host/ZeroFat.WebAPIs/Configurations/security.json`

When delivering to another party, provide a new JWT signing key and do not reuse any keys from development.

### Hangfire

- `HangfireSettings` in `src/Host/ZeroFat.WebAPIs/Configurations/hangfire.json`

### External integrations

These are configured under `src/Host/ZeroFat.WebAPIs/Configurations/`:

- `paymob.json`
- `stripe.json`
- `sms.json`
- `storage.json`

## Environment variable overrides

ASP.NET Core supports overriding nested configuration using `__`.

Examples:

- `DatabaseOptions__Provider=postgresql`
- `DatabaseOptions__ConnectionString=Host=localhost;Port=5433;Database=zerofatdb;Username=postgres;Password=<PASSWORD>;`
- `SecuritySettings__JwtSettings__Key=<JWT_SIGNING_KEY>`

## Secrets

Do not commit real credentials, API keys, or encryption keys.

Recommended approaches:

- Use environment variables for local/dev.
- Use a secret store (CI/CD secrets, Kubernetes secrets, etc.) in non-dev environments.
