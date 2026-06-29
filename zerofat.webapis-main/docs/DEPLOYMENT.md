# Deployment

This repo is designed to be deployed as a single host application (`ZeroFat.WebAPIs`) that composes multiple modules.

## Local publish and run (Windows exe)

To publish and run `ZeroFat.WebAPIs.exe` on your machine (outside Docker/ECS), see [LOCAL_DEPLOYMENT.md](LOCAL_DEPLOYMENT.md).

## Primary deployment target: AWS (ECS + ECR + RDS PostgreSQL)

The current deployment model is:

- GitLab CI builds the Docker image and pushes it to **Amazon ECR**.
- **Amazon ECS** runs the image.
- Database is **Amazon RDS (Aurora PostgreSQL / PostgreSQL-compatible)**.

See: `docs/AWS_ECS_ECR.md`.

## Container (Docker)

A `Dockerfile` is included at repo root.

### Build

- `docker build -t zerofat.webapis .`

### Run (example)

- `docker run --rm -p 8080:8080 -e ASPNETCORE_URLS=http://+:8080 zerofat.webapis`

### Provide configuration

Recommended: pass configuration via environment variables:

- `-e DatabaseOptions__Provider=postgresql`
- `-e DatabaseOptions__ConnectionString=...`
- `-e HangfireSettings__Storage__ConnectionString=...`
- `-e SecuritySettings__JwtSettings__Key=...`

If you run in containers, ensure the DB host is reachable from the container network.

## Migrations in production

The app applies migrations on startup (via module `IDbInitializer`).

In production you may prefer a controlled migration step (CI/CD job) before updating the ECS service.

Whether to keep "migrate on startup" depends on your change-management and permissions model.

## Observability

- Logs are configured via Serilog (see `src/Host/ZeroFat.WebAPIs/Configurations/logger.json`).
- Health endpoints are available (see `/healthcheck`, `/health-api`, `/health-ui`).
