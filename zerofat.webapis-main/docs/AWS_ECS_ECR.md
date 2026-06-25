# AWS deployment (ECS + ECR + RDS PostgreSQL)

This document describes the current/expected deployment model for **ZeroFat.WebAPIs** on AWS.

## Overview

- **Container image** built from the repo `Dockerfile`
- **GitLab CI** builds the image and pushes it to **Amazon ECR**
- **Amazon ECS** runs the image (service + task definition)
- **Amazon RDS (Aurora PostgreSQL / PostgreSQL-compatible)** is used for the application database

Current runtime (as provided):

- Region: `ap-southeast-1`
- Launch type: **Fargate**
- Cluster: `zerofat-cluster`
- Service: `zerofat-api-service`
- Task definition family: `zerofat-api-task`
- Task definition revision (currently running): `28`

Deployment process (as provided):

- GitLab CI **builds** and **pushes** the image to ECR
- ECS is updated **manually** after the image is pushed

## Container image

- Dockerfile: `Dockerfile`
- Container listens on port **8080** (see `EXPOSE 8080`)
- Recommended runtime env var:
  - `ASPNETCORE_URLS=http://+:8080`

Currently running image (as provided):

- Container name: `zerofat-api`
- Image: `013364996232.dkr.ecr.ap-southeast-1.amazonaws.com/zerofat-api:v1`

## GitLab CI → ECR

The repo already includes a pipeline that builds and pushes to ECR:

- `.gitlab-ci.yml`

GitLab variables that must be configured (in GitLab UI / CI variables):

- `AWS_ECR_REGISTRY` (example: `013364996232.dkr.ecr.ap-southeast-1.amazonaws.com`)
- `AWS_DEFAULT_REGION` (example: `ap-southeast-1`)
- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`

Image naming variables in `.gitlab-ci.yml`:

- `AWS_ECR_REPOSITORY` (e.g. `zerofat-api`)
- `IMAGE_TAG`

### Image tagging strategy

Current approach (as provided):

- Use a **mutable** tag: `v1`

Optional (recommended for traceability):

To make rollbacks and traceability easier, prefer:

- `IMAGE_TAG=$CI_COMMIT_SHORT_SHA` (or `$CI_COMMIT_SHA`)

## ECS service configuration

At minimum you need:

- An ECS cluster
- A task definition (Fargate or EC2)
- A service running the task definition
- Networking (VPC subnets + security groups)

### Task definition (high level)

- Container image: `${AWS_ECR_REGISTRY}/${AWS_ECR_REPOSITORY}:${IMAGE_TAG}`
- Container port mapping: `8080`
- Environment variables / secrets (see below)

### Fargate sizing (current)

- CPU / Memory: **1 vCPU / 3 GiB**
- Platform version: `1.4.0`
- Network mode: `awsvpc`

### Application configuration in ECS

Current approach (as provided): configuration is baked **inside the container image** (no ECS environment variables / secrets configured in the task definition).

Optional (recommended): use **ECS task definition** environment variables + AWS secret integration.

Common variables:

- `ASPNETCORE_ENVIRONMENT=Production`
- `DatabaseOptions__Provider=postgresql`
- `DatabaseOptions__ConnectionString=<RDS_CONNECTION_STRING>`
- `HangfireSettings__Storage__StorageProvider=postgresql`
- `HangfireSettings__Storage__ConnectionString=<RDS_CONNECTION_STRING>`
- `SecuritySettings__JwtSettings__Key=<JWT_SIGNING_KEY>`

Store secrets in one of:

- AWS Secrets Manager
- AWS SSM Parameter Store

…and reference them from the ECS task definition as secrets (do not commit values).

## Load balancer (NLB)

The environment uses an **internet-facing Network Load Balancer**:

- Name: `zerofat-nlb`
- Type: Network
- Scheme: Internet-facing
- Listener: **TLS 443**
- Default action: forward to target group `zerofat-api-tg`
- NLB ARN: `arn:aws:elasticloadbalancing:ap-southeast-1:013364996232:loadbalancer/net/zerofat-nlb/3c3f4b80be1273fd`
- DNS name: `zerofat-nlb-3c3f4b80be1273fd.elb.ap-southeast-1.amazonaws.com`

### Target group

Target group details (as provided):

- Target group: `zerofat-api-tg`
- Target type: `IP`
- Protocol/port: `TCP:80`
- Registered target (current task IP): `10.0.151.230:8080`

### Health checks

Current health check configuration (as provided):

- Protocol: `HTTP`
- Port: `traffic port` (effective port is the registered target port, e.g. `8080`)
- Path: `/healthcheck`
- Success codes: `200-399`
- Interval: `30s`
- Timeout: `6s`
- Healthy threshold: `5`
- Unhealthy threshold: `2`

Note: the application also exposes a health check API endpoint at `/health-api` (HealthChecks UI response). The NLB uses `/healthcheck`.

## RDS (PostgreSQL)

Current environment details (provided for handover/context):

- DB instance ID: `zerofat-db`
- Engine: Aurora PostgreSQL 17.x (engine version `17.4`)
- Region: `ap-southeast-1`
- Database name: `zerofatdb`
- ARN: `arn:aws:rds:ap-southeast-1:013364996232:db:zerofat-db`

### Connectivity

Ensure ECS tasks can reach the DB:

- Security groups allow inbound PostgreSQL (5432) from the ECS task security group
- Use the correct cluster/instance endpoint in the connection string

## Updating ECS on new image push

Pushing to ECR alone does **not** update running tasks unless you force a new deployment.

Common options:

1. **Immutable tags** (recommended): update task definition with the new `${IMAGE_TAG}` and redeploy service.
2. **Mutable tag** (e.g. `latest`): keep the tag constant and force a new deployment on the ECS service.

### Recommended approach for this project

Use **immutable tags** (commit SHA) and update the ECS task definition to reference the new image tag, then update the ECS service.

At a high level:

1. Push a new image tag to ECR
2. Register a new task definition revision with updated image
3. Update ECS service to the new task definition revision (or force a new deployment)

### Manual deployment options

Since deployment is manual after pushing to ECR, you typically do one of:

1. **Immutable image tags** (recommended):
   - Register a new task definition revision with the new image tag
   - Update `zerofat-api-service` to use the new revision
2. **Mutable image tag** (current CI default appears to be `IMAGE_TAG=v1`):
   - Push the updated image under the same tag
   - In ECS, force a new deployment of `zerofat-api-service` so tasks pull the latest image for that tag

#### Manual steps (AWS Console)

Option A (immutable tags):

1. Push image to ECR with a new tag (recommended: commit SHA)
2. Create a new revision of task definition `zerofat-api-task` updating the image for container `zerofat-api`
3. Update service `zerofat-api-service` to use the new task definition revision

Option B (mutable tag `v1`):

1. Push image to ECR with tag `v1`
2. In ECS service `zerofat-api-service`, choose **Force new deployment**

### GitLab CI deployment stage (optional)

If you want GitLab to also update ECS after pushing to ECR, add a `deploy` stage that:

- Registers a new task definition revision (or updates the image)
- Updates the ECS service to use the new revision
- Forces a new deployment

Exact commands depend on your ECS launch type (Fargate/EC2) and how task definitions are managed (JSON in repo vs generated).

For this environment (Fargate), a common pattern is:

- `aws ecs describe-task-definition` (get the current JSON)
- update the `containerDefinitions[].image` value
- `aws ecs register-task-definition`
- `aws ecs update-service --force-new-deployment`

If you later decide to automate ECS updates from GitLab, a deploy job can be added, but it will require the task definition **container name** and a decision on immutable vs mutable tags.

## Information needed to finalize a production-ready guide

To turn this into an exact runbook (copy/paste commands), provide:

1. ECS launch type: **Fargate** or **EC2**
2. ECS cluster name, service name, task definition family
3. NLB target group protocol/port and health check type (TCP vs HTTP/HTTPS) and path if applicable (e.g. `/health-api`)
4. Where secrets are stored (Secrets Manager vs SSM) and naming convention
5. Whether migrations should run at startup in production or as a separate CI/CD step
6. ECS task definition container name (needed for scripted updates)
