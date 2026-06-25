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

### Seed data / testing mode

Test seed data (roles, test users, and JSON demo data for all modules) is controlled by `SeedOptions`:

- `SeedOptions:EnableTestingMode` in `src/Host/ZeroFat.WebAPIs/appsettings.json` (default: `false`)
- `src/Host/ZeroFat.WebAPIs/appsettings.Development.json` can override this for local development

When `EnableTestingMode` is `true`, the host runs `SeedAsync` for every module after migrations. When `false`, only migrations run.

Environment variable override:

- `SeedOptions__EnableTestingMode=true`

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
- `email.json`

### Email (subscription payment links)

- `EmailSettings` in `src/Host/ZeroFat.WebAPIs/Configurations/email.json`

When `EmailSettings:Enabled` is `false` (default), payment-link emails are logged only. Set `Enabled` to `true` and configure SMTP to send real messages.

Environment variable examples:

- `EmailSettings__Enabled=true`
- `EmailSettings__SmtpHost=smtp.example.com`
- `EmailSettings__SmtpUsername=<USERNAME>`
- `EmailSettings__SmtpPassword=<PASSWORD>`

## Environment variable overrides

ASP.NET Core supports overriding nested configuration using `__`.

Examples:

- `DatabaseOptions__Provider=postgresql`
- `DatabaseOptions__ConnectionString=Host=localhost;Port=5433;Database=zerofatdb;Username=postgres;Password=<PASSWORD>;`
- `SeedOptions__EnableTestingMode=true`
- `SecuritySettings__JwtSettings__Key=<JWT_SIGNING_KEY>`

## Secrets

Do not commit real credentials, API keys, or encryption keys.

Recommended approaches:

- Use environment variables for local/dev.
- Use a secret store (CI/CD secrets, Kubernetes secrets, etc.) in non-dev environments.
