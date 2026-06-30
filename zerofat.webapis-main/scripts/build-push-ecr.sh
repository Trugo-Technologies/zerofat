#!/usr/bin/env bash
# Build ZeroFat.WebAPIs Docker image and push to ECR with prod + DEV + QC tags.
#
# Usage:
#   ./scripts/build-push-ecr.sh <commit_sha>

set -euo pipefail

REGION="${AWS_DEFAULT_REGION:-ap-southeast-1}"
ECR_REPO="${AWS_ECR_REPOSITORY:-zerofat-api}"
IMAGE_NAME="${IMAGE_NAME:-zerofat-api}"

usage() {
  echo "Usage: $0 <commit_sha>"
  exit 1
}

[[ $# -eq 1 ]] || usage
COMMIT_SHA="$1"

if [[ -z "${AWS_ECR_REGISTRY:-}" ]]; then
  echo "Error: AWS_ECR_REGISTRY is not set." >&2
  exit 1
fi

REGISTRY="${AWS_ECR_REGISTRY}/${ECR_REPO}"

echo "Building ${IMAGE_NAME}..."
docker build -t "${IMAGE_NAME}:latest" .

echo "Logging in to ECR (${REGION})..."
aws ecr get-login-password --region "$REGION" | \
  docker login --username AWS --password-stdin "$AWS_ECR_REGISTRY"

tags=(
  "v1"
  "${COMMIT_SHA}"
  "dev-${COMMIT_SHA}"
  "qc-${COMMIT_SHA}"
)

for tag in "${tags[@]}"; do
  echo "Tagging and pushing ${REGISTRY}:${tag}"
  docker tag "${IMAGE_NAME}:latest" "${REGISTRY}:${tag}"
  docker push "${REGISTRY}:${tag}"
done

echo ""
echo "Pushed tags:"
for tag in "${tags[@]}"; do
  echo "  ${REGISTRY}:${tag}"
done
