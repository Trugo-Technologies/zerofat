#!/usr/bin/env bash
# Deploy ZeroFat.WebAPIs to DEV or QC on AWS ECS.
#
# Usage:
#   ./scripts/deploy-ecs.sh <dev|qc> <commit_sha> [--verify]
#   ./scripts/deploy-ecs.sh <dev|qc> --rollback <revision>

set -euo pipefail

REGION="${AWS_DEFAULT_REGION:-ap-southeast-1}"
ECR_REPO="${AWS_ECR_REPOSITORY:-zerofat-api}"
CONTAINER_NAME="${ECS_CONTAINER_NAME:-zerofat-api}"
TASK_FAMILY="${ECS_TASK_FAMILY:-zerofat-api-task}"
VERIFY_TIMEOUT="${DEPLOY_VERIFY_TIMEOUT:-600}"

usage() {
  echo "Usage:"
  echo "  $0 <dev|qc> <commit_sha> [--verify]"
  echo "  $0 <dev|qc> --rollback <revision>"
  exit 1
}

require_cmd() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Error: '$1' is required but not installed." >&2
    exit 1
  fi
}

map_environment() {
  local env="$1"
  case "$env" in
    dev)
      CLUSTER="zerofat-dev-cluster"
      SERVICE="zerofat-dev-api-service"
      TAG_PREFIX="dev"
      ;;
    qc)
      CLUSTER="zerofat-qc-cluster"
      SERVICE="zerofat-qc-api-service"
      TAG_PREFIX="qc"
      ;;
    *)
      echo "Error: environment must be 'dev' or 'qc' (got '$env')." >&2
      exit 1
      ;;
  esac
}

require_ecr_registry() {
  if [[ -z "${AWS_ECR_REGISTRY:-}" ]]; then
    echo "Error: AWS_ECR_REGISTRY is not set." >&2
    exit 1
  fi
}

deploy_image() {
  local commit_sha="$1"
  local verify="${2:-false}"
  local image="${AWS_ECR_REGISTRY}/${ECR_REPO}:${TAG_PREFIX}-${commit_sha}"

  echo "Deploying ${image} to ${CLUSTER}/${SERVICE} (${REGION})"

  local task_def_arn
  task_def_arn=$(aws ecs describe-services \
    --cluster "$CLUSTER" \
    --services "$SERVICE" \
    --region "$REGION" \
    --query 'services[0].taskDefinition' \
    --output text)

  if [[ -z "$task_def_arn" || "$task_def_arn" == "None" ]]; then
    echo "Error: could not resolve task definition for service $SERVICE." >&2
    exit 1
  fi

  echo "Current task definition: $task_def_arn"

  local task_file new_task_file
  task_file=$(mktemp)
  new_task_file=$(mktemp)
  trap 'rm -f "$task_file" "$new_task_file"' RETURN

  aws ecs describe-task-definition \
    --task-definition "$task_def_arn" \
    --region "$REGION" \
    --query taskDefinition \
    --output json > "$task_file"

  jq --arg img "$image" --arg name "$CONTAINER_NAME" '
    del(.taskDefinitionArn, .revision, .status, .requiresAttributes, .compatibilities, .registeredAt, .registeredBy)
    | .containerDefinitions = (
        .containerDefinitions
        | map(if .name == $name then .image = $img else . end)
      )
  ' "$task_file" > "$new_task_file"

  local new_revision
  new_revision=$(aws ecs register-task-definition \
    --region "$REGION" \
    --cli-input-json "file://${new_task_file}" \
    --query 'taskDefinition.revision' \
    --output text)

  echo "Registered task definition ${TASK_FAMILY}:${new_revision}"

  aws ecs update-service \
    --cluster "$CLUSTER" \
    --service "$SERVICE" \
    --task-definition "${TASK_FAMILY}:${new_revision}" \
    --region "$REGION" \
    --output text >/dev/null

  echo "Service update initiated: ${TASK_FAMILY}:${new_revision}"

  if [[ "$verify" == "true" ]]; then
    verify_deployment
  fi
}

rollback_revision() {
  local revision="$1"
  echo "Rolling back ${CLUSTER}/${SERVICE} to ${TASK_FAMILY}:${revision}"

  aws ecs update-service \
    --cluster "$CLUSTER" \
    --service "$SERVICE" \
    --task-definition "${TASK_FAMILY}:${revision}" \
    --region "$REGION" \
    --output text >/dev/null

  echo "Rollback initiated: ${TASK_FAMILY}:${revision}"
  verify_deployment
}

verify_deployment() {
  echo "Waiting for deployment to complete (timeout: ${VERIFY_TIMEOUT}s)..."
  local elapsed=0
  local interval=15

  while [[ $elapsed -lt $VERIFY_TIMEOUT ]]; do
    local rollout_state running desired
    rollout_state=$(aws ecs describe-services \
      --cluster "$CLUSTER" \
      --services "$SERVICE" \
      --region "$REGION" \
      --query 'services[0].deployments[0].rolloutState' \
      --output text)
    running=$(aws ecs describe-services \
      --cluster "$CLUSTER" \
      --services "$SERVICE" \
      --region "$REGION" \
      --query 'services[0].runningCount' \
      --output text)
    desired=$(aws ecs describe-services \
      --cluster "$CLUSTER" \
      --services "$SERVICE" \
      --region "$REGION" \
      --query 'services[0].desiredCount' \
      --output text)

    echo "  rolloutState=${rollout_state} running=${running} desired=${desired}"

    if [[ "$rollout_state" == "COMPLETED" && "$running" == "$desired" ]]; then
      echo "Deployment completed successfully."
      return 0
    fi

    if [[ "$rollout_state" == "FAILED" ]]; then
      echo "Error: deployment rollout failed." >&2
      exit 1
    fi

    sleep "$interval"
    elapsed=$((elapsed + interval))
  done

  echo "Error: deployment verification timed out after ${VERIFY_TIMEOUT}s." >&2
  exit 1
}

main() {
  require_cmd aws
  require_cmd jq
  require_ecr_registry

  [[ $# -ge 1 ]] || usage

  local env="$1"
  shift
  map_environment "$env"

  if [[ $# -ge 1 && "$1" == "--rollback" ]]; then
    [[ $# -eq 2 ]] || usage
    rollback_revision "$2"
    return
  fi

  [[ $# -ge 1 ]] || usage
  local commit_sha="$1"
  shift

  local verify=false
  if [[ $# -ge 1 && "$1" == "--verify" ]]; then
    verify=true
  fi

  deploy_image "$commit_sha" "$verify"
}

main "$@"
