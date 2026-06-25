# Setup (Local Development)

This document describes how to build and run **ZeroFat.WebAPIs** locally.

## Prerequisites

- .NET SDK 9.0
- A supported database (configured via `DatabaseOptions:Provider`)
  - PostgreSQL (commonly used in this repo)
  - SQL Server / MySQL / SQLite (optional)
- (Optional) Docker Desktop (for running the database and/or the API)

If you run locally with HTTPS:

- `dotnet dev-certs https --trust`

## Quick start

From the repo root:

- `dotnet restore ZeroFat.sln`
- `dotnet build ZeroFat.sln`
- `dotnet run --project src/Host/ZeroFat.WebAPIs/ZeroFat.WebAPIs.csproj`

Default URLs (Development profile):

- `http://localhost:5000`
- `https://localhost:7000`

## Database

The host applies EF Core migrations and runs seeders on startup via module `IDbInitializer` implementations.

### PostgreSQL via Docker (example)

- `docker run --name zerofat-postgres --rm -e POSTGRES_PASSWORD=<PASSWORD> -e POSTGRES_DB=zerofatdb -p 5433:5432 postgres:16`

Then set environment variables (PowerShell example):

- `$env:DatabaseOptions__Provider = "postgresql"`
- `$env:DatabaseOptions__ConnectionString = "Host=localhost;Port=5433;Database=zerofatdb;Username=postgres;Password=<PASSWORD>;"`

## Common endpoints

- Swagger: `/swagger`
- Hangfire dashboard: `/jobs`
- Health UI: `/health-ui`
- Health API: `/health-api`
