# Security

## Reporting

If you discover a security issue, report it privately to the maintainers.

## Secrets and credentials

This repository contains configuration files under `src/Host/ZeroFat.WebAPIs/Configurations`.

When delivering this project to another party:

- **Do not** ship real production secrets in source control.
- Replace any development credentials/keys with:
  - environment variables, or
  - a secret store used by your deployment platform.

Examples of sensitive values to externalize:

- Database connection strings
- JWT signing keys
- Storage provider keys
- SMS provider credentials
- Payment provider credentials

## Recommended practices

- Rotate any leaked keys immediately.
- Use separate credentials per environment (dev/staging/prod).
- Restrict DB user permissions to least privilege needed.
