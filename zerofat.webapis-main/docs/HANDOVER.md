# Handover checklist

Use this checklist when delivering the project to another party.

## Source code

- [ ] Provide the repository URL and the target branch/tag/commit.
- [ ] Provide build instructions (see `README.md`).
- [ ] Provide module overview and ownership contacts.

## Environment configuration

- [ ] Document the target environment (Docker/K8s/IIS/etc.).
- [ ] Provide non-secret configuration defaults.
- [ ] Provide a list of required secrets (without values) and where to set them.

## Infrastructure dependencies

- [ ] Database provider + version + connection requirements.
- [ ] Who runs migrations (app-on-start vs CI/CD step).
- [ ] External services required (Stripe/Paymob/SMS/Storage).

### AWS-specific (current deployment)

- [ ] ECR registry/repository name(s)
- [ ] ECS cluster/service/task definition names
- [ ] ECS container name inside the task definition (needed for automated task definition updates)
- [ ] VPC/subnets/security groups used by ECS tasks
- [ ] Load balancer details (NLB/ALB), listener ports, target group name(s), health check type/path
- [ ] RDS endpoint/port and allowed security group rules
- [ ] Where secrets are stored (Secrets Manager vs SSM) and required secret names

Known current values (from AWS console):

- NLB: `zerofat-nlb` (TLS 443 → target group `zerofat-api-tg`)
- Target group: `zerofat-api-tg` (TCP:80, IP target `:8080`, health check HTTP `/healthcheck`)
- ECS: cluster `zerofat-cluster`, service `zerofat-api-service`, task definition `zerofat-api-task:28`, container `zerofat-api`

Delivery conventions (as provided):

- Configuration is shipped inside the container image
- Image tag is kept as a mutable `v1`
- ECS rollout is performed manually after pushing to ECR

## Operational readiness

- [ ] Logging destination and retention expectations.
- [ ] Health check URLs and expected statuses.
- [ ] Background jobs (Hangfire) access and credentials.

## Security

- [ ] Confirm no real secrets are committed.
- [ ] Provide key rotation guidance and environment separation.
