# Architecture

## Overview

ZeroFat is a **modular monolith**: modules are developed as separate projects (with clear boundaries) but deployed together under a single Host.

## Key concepts

- **Host**: `src/Host/ZeroFat.WebAPIs` boots the app, loads configuration, registers modules, and wires middleware.
- **Modules**: located in `src/Modules/*`.
- **Building blocks**: shared abstractions and cross-cutting concerns in `src/BuildingBlocks/*`.

## Typical module layering

Most modules follow:

- `*.Domain`: entities, aggregates, domain rules
- `*.Application`: use-cases, contracts, application services
- `*.Infrastructure`: EF Core persistence, integrations, implementation details
- `*.Api`: module endpoints (controllers, minimal APIs, etc.)

## Persistence & migrations

- EF Core is used for persistence.
- The active provider is selected via `DatabaseOptions:Provider`.
- Migrations live in provider-specific migrator projects:
  - `src/Migrations/Migrators.PostgreSQL`
  - `src/Migrations/Migrators.MSSQL`
  - (others if present)

## Startup flow

At startup the host:

1. Loads configuration JSON files from `src/Host/ZeroFat.WebAPIs/Configurations`.
2. Registers shared infrastructure.
3. Registers modules (Users, Workouts, NutriPlan, ClientPortal, ...).
4. Runs module database initializers (`IDbInitializer`) to apply migrations and seed.

If you need a diagram-oriented handover, share the audience (developers vs ops) and the desired depth.
