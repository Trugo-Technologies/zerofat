# Copilot Instructions

## Project Guidelines
- Deployment practice: configuration is baked inside the container image (no ECS env vars/secrets), images are tagged with a mutable 'v1' tag, and ECS deployments are done manually after pushing to ECR.