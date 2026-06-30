# ZeroFat.WebAPIs – Multi-Environment Deployment (DEV & QC)

This guide provides **exact AWS CLI commands** and **automation scripts** to deploy updates separately to **DEV** and **QC** environments using ECS + ECR.

Production (`zerofat-cluster` / `zerofat-api-service`, image tag `v1`) is **out of scope** for automated deploys here. CI continues to push `v1` for the existing production workflow.

---

## 1. Environment configuration

| Environment | Cluster             | Service                 | Task definition family | Image tag pattern   |
| ----------- | ------------------- | ----------------------- | ---------------------- | ------------------- |
| DEV         | `zerofat-dev-cluster` | `zerofat-dev-api-service` | `zerofat-api-task`   | `dev-<commit_sha>`  |
| QC          | `zerofat-qc-cluster`  | `zerofat-qc-api-service`  | `zerofat-api-task`   | `qc-<commit_sha>`   |
| PROD (manual) | `zerofat-cluster`   | `zerofat-api-service`     | `zerofat-api-task`   | `v1` (mutable)      |

- **Region:** `ap-southeast-1`
- **ECR:** `013364996232.dkr.ecr.ap-southeast-1.amazonaws.com/zerofat-api`
- **Container name:** `zerofat-api`

---

## 2. Recommended workflow (scripts)

### GitLab CI (after push)

1. Pipeline builds and pushes images (including `dev-<sha>` and `qc-<sha>`).
2. Manually trigger **`deploy_dev`** or **`deploy_qc`** in GitLab.

### Local / manual

```bash
# Build and push (optional — CI does this on merge)
./scripts/build-push-ecr.sh <commit_sha>

# Deploy to DEV
./scripts/deploy-ecs.sh dev <commit_sha> --verify

# Deploy to QC
./scripts/deploy-ecs.sh qc <commit_sha> --verify

# Rollback DEV to task revision 27
./scripts/deploy-ecs.sh dev --rollback 27
```

Windows (PowerShell):

```powershell
.\scripts\deploy-ecs.ps1 -Environment dev -CommitSha <commit_sha> -Verify
.\scripts\deploy-ecs.ps1 -Environment qc -CommitSha <commit_sha> -Verify
.\scripts\deploy-ecs.ps1 -Environment dev -RollbackRevision 27
```

### Required environment variables

| Variable | Example |
| -------- | ------- |
| `AWS_DEFAULT_REGION` | `ap-southeast-1` |
| `AWS_ECR_REGISTRY` | `013364996232.dkr.ecr.ap-southeast-1.amazonaws.com` |
| `AWS_ECR_REPOSITORY` | `zerofat-api` (default in scripts) |
| `AWS_ACCESS_KEY_ID` / `AWS_SECRET_ACCESS_KEY` | From IAM user or CI variables |

---

## 3. Build & push image (common)

### 3.1 Login to ECR

```bash
aws ecr get-login-password \
  --region ap-southeast-1 | \
  docker login \
  --username AWS \
  --password-stdin 013364996232.dkr.ecr.ap-southeast-1.amazonaws.com
```

### 3.2 Build image

```bash
docker build -t zerofat-api .
```

### 3.3 Tag image (environment-based)

```bash
COMMIT_SHA=<commit_sha>
REGISTRY=013364996232.dkr.ecr.ap-southeast-1.amazonaws.com/zerofat-api

docker tag zerofat-api:latest ${REGISTRY}:dev-${COMMIT_SHA}
docker tag zerofat-api:latest ${REGISTRY}:qc-${COMMIT_SHA}
docker tag zerofat-api:latest ${REGISTRY}:v1
```

### 3.4 Push image

```bash
docker push ${REGISTRY}:dev-${COMMIT_SHA}
docker push ${REGISTRY}:qc-${COMMIT_SHA}
docker push ${REGISTRY}:v1
```

Or use `./scripts/build-push-ecr.sh <commit_sha>` to build and push all tags.

---

## 4. Deploy to DEV

### 4.1 Get current task definition (from running service)

**Important:** base the new task definition on the revision **currently running on the target service**, not the latest family revision (DEV and QC share family `zerofat-api-task`).

```bash
TASK_DEF=$(aws ecs describe-services \
  --cluster zerofat-dev-cluster \
  --services zerofat-dev-api-service \
  --region ap-southeast-1 \
  --query 'services[0].taskDefinition' --output text)

aws ecs describe-task-definition \
  --task-definition "$TASK_DEF" \
  --region ap-southeast-1 \
  --query taskDefinition > dev-task.json
```

### 4.2 Update image (edit file)

```json
"image": "013364996232.dkr.ecr.ap-southeast-1.amazonaws.com/zerofat-api:dev-<commit_sha>"
```

Remove read-only fields before registering:

- `taskDefinitionArn`, `revision`, `status`, `requiresAttributes`, `compatibilities`, `registeredAt`, `registeredBy`

### 4.3 Register new revision

```bash
aws ecs register-task-definition \
  --cli-input-json file://dev-task.json \
  --region ap-southeast-1
```

### 4.4 Update DEV service

```bash
aws ecs update-service \
  --cluster zerofat-dev-cluster \
  --service zerofat-dev-api-service \
  --task-definition zerofat-api-task \
  --region ap-southeast-1
```

---

## 5. Deploy to QC

Same steps as DEV, using QC cluster/service and `qc-<commit_sha>` image tag.

```bash
TASK_DEF=$(aws ecs describe-services \
  --cluster zerofat-qc-cluster \
  --services zerofat-qc-api-service \
  --region ap-southeast-1 \
  --query 'services[0].taskDefinition' --output text)

aws ecs describe-task-definition \
  --task-definition "$TASK_DEF" \
  --region ap-southeast-1 \
  --query taskDefinition > qc-task.json
```

Update image to `.../zerofat-api:qc-<commit_sha>`, register, then:

```bash
aws ecs update-service \
  --cluster zerofat-qc-cluster \
  --service zerofat-qc-api-service \
  --task-definition zerofat-api-task \
  --region ap-southeast-1
```

---

## 6. Quick deployment (mutable `v1` tag — production only)

```bash
aws ecs update-service \
  --cluster zerofat-cluster \
  --service zerofat-api-service \
  --force-new-deployment \
  --region ap-southeast-1
```

DEV/QC should use immutable `dev-<sha>` / `qc-<sha>` tags via the deploy script.

---

## 7. Verify deployment

```bash
aws ecs describe-services \
  --cluster zerofat-dev-cluster \
  --services zerofat-dev-api-service \
  --region ap-southeast-1

aws ecs describe-services \
  --cluster zerofat-qc-cluster \
  --services zerofat-qc-api-service \
  --region ap-southeast-1
```

Check `deployments[0].rolloutState` is `COMPLETED` and `runningCount` matches `desiredCount`.

Health endpoint: `GET /healthcheck` (expect HTTP 200).

---

## 8. Rollback

```bash
aws ecs update-service \
  --cluster zerofat-dev-cluster \
  --service zerofat-dev-api-service \
  --task-definition zerofat-api-task:27 \
  --region ap-southeast-1
```

Or: `./scripts/deploy-ecs.sh dev --rollback 27`

---

## 9. Tagging best practice

| Environment | Tag pattern   | Notes |
| ------------- | ------------- | ----- |
| DEV           | `dev-<sha>`   | Immutable; traceable to commit |
| QC            | `qc-<sha>`    | Immutable; traceable to commit |
| PROD          | `v1` (current) | Mutable; migrate to `prod-<sha>` when ready |

Never reuse generic tags like `latest` for production rollouts.

---

## 10. Deployment flow

```text
Build → Tag (v1 + dev-sha + qc-sha) → Push → Update Task → Deploy → Verify
```

GitLab CI automates build/push; `deploy_dev` / `deploy_qc` are **manual** jobs that run `scripts/deploy-ecs.sh`.

---

## Related docs

- [DEPLOYMENT.md](DEPLOYMENT.md) — overview
- [AWS_ECS_ECR.md](AWS_ECS_ECR.md) — infrastructure details (NLB, RDS, legacy single-cluster reference)
