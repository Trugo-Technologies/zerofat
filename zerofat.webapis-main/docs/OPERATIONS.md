# Operations

## Logging

Logging is configured via Serilog (see `src/Host/ZeroFat.WebAPIs/Configurations/logger.json`).

Common log locations/sinks:

- Console
- File sink (JSON)

## Background jobs (Hangfire)

Hangfire is configured in `src/Host/ZeroFat.WebAPIs/Configurations/hangfire.json`.

- Dashboard route: `/jobs`
- Storage provider/connection string is configured under `HangfireSettings:Storage`.

## Health checks

Health endpoints are available:

- `/healthcheck`
- `/health-api`
- `/health-ui`

Configuration is in `src/Host/ZeroFat.WebAPIs/Configurations/healthcheck.json`.

## Common runbook checks

- Verify DB connectivity and that migrations can be applied.
- Verify JWT settings are configured and keys are present.
- Verify Hangfire storage points to a reachable DB.
- Verify external integrations are disabled or configured (Stripe/Paymob/SMS/Storage).
