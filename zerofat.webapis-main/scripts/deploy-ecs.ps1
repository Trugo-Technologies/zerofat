# Deploy ZeroFat.WebAPIs to DEV or QC on AWS ECS.
#
# Usage:
#   .\scripts\deploy-ecs.ps1 -Environment dev -CommitSha <sha> [-Verify]
#   .\scripts\deploy-ecs.ps1 -Environment qc -CommitSha <sha> [-Verify]
#   .\scripts\deploy-ecs.ps1 -Environment dev -RollbackRevision 27

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("dev", "qc")]
    [string]$Environment,

    [string]$CommitSha,

    [int]$RollbackRevision = 0,

    [switch]$Verify
)

$ErrorActionPreference = "Stop"

$Region = if ($env:AWS_DEFAULT_REGION) { $env:AWS_DEFAULT_REGION } else { "ap-southeast-1" }
$EcrRepo = if ($env:AWS_ECR_REPOSITORY) { $env:AWS_ECR_REPOSITORY } else { "zerofat-api" }
$ContainerName = if ($env:ECS_CONTAINER_NAME) { $env:ECS_CONTAINER_NAME } else { "zerofat-api" }
$TaskFamily = if ($env:ECS_TASK_FAMILY) { $env:ECS_TASK_FAMILY } else { "zerofat-api-task" }
$VerifyTimeout = if ($env:DEPLOY_VERIFY_TIMEOUT) { [int]$env:DEPLOY_VERIFY_TIMEOUT } else { 600 }

if (-not $env:AWS_ECR_REGISTRY) {
    Write-Error "AWS_ECR_REGISTRY is not set."
}

switch ($Environment) {
    "dev" {
        $Cluster = "zerofat-dev-cluster"
        $Service = "zerofat-dev-api-service"
        $TagPrefix = "dev"
    }
    "qc" {
        $Cluster = "zerofat-qc-cluster"
        $Service = "zerofat-qc-api-service"
        $TagPrefix = "qc"
    }
}

function Invoke-AwsJson {
    param([string[]]$Arguments)
    $output = & aws @Arguments --output json 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($output | Out-String) }
    return $output | ConvertFrom-Json
}

function Wait-ForDeployment {
    Write-Host "Waiting for deployment to complete (timeout: ${VerifyTimeout}s)..."
    $elapsed = 0
    $interval = 15

    while ($elapsed -lt $VerifyTimeout) {
        $svc = Invoke-AwsJson @(
            "ecs", "describe-services",
            "--cluster", $Cluster,
            "--services", $Service,
            "--region", $Region
        )
        $deployment = $svc.services[0].deployments[0]
        $running = $svc.services[0].runningCount
        $desired = $svc.services[0].desiredCount

        Write-Host "  rolloutState=$($deployment.rolloutState) running=$running desired=$desired"

        if ($deployment.rolloutState -eq "COMPLETED" -and $running -eq $desired) {
            Write-Host "Deployment completed successfully."
            return
        }
        if ($deployment.rolloutState -eq "FAILED") {
            throw "Deployment rollout failed."
        }

        Start-Sleep -Seconds $interval
        $elapsed += $interval
    }

    throw "Deployment verification timed out after ${VerifyTimeout}s."
}

if ($RollbackRevision -gt 0) {
    Write-Host "Rolling back ${Cluster}/${Service} to ${TaskFamily}:${RollbackRevision}"
    & aws ecs update-service `
        --cluster $Cluster `
        --service $Service `
        --task-definition "${TaskFamily}:${RollbackRevision}" `
        --region $Region | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "aws ecs update-service failed." }
    Wait-ForDeployment
    return
}

if (-not $CommitSha) {
    Write-Error "CommitSha is required unless -RollbackRevision is specified."
}

$Image = "$($env:AWS_ECR_REGISTRY)/${EcrRepo}:${TagPrefix}-${CommitSha}"
Write-Host "Deploying $Image to ${Cluster}/${Service} ($Region)"

$svc = Invoke-AwsJson @(
    "ecs", "describe-services",
    "--cluster", $Cluster,
    "--services", $Service,
    "--region", $Region
)
$taskDefArn = $svc.services[0].taskDefinition
if (-not $taskDefArn) { throw "Could not resolve task definition for service $Service." }

Write-Host "Current task definition: $taskDefArn"

$taskDef = Invoke-AwsJson @(
    "ecs", "describe-task-definition",
    "--task-definition", $taskDefArn,
    "--region", $Region
).taskDefinition

$readOnly = @(
    "taskDefinitionArn", "revision", "status", "requiresAttributes",
    "compatibilities", "registeredAt", "registeredBy"
)
foreach ($key in $readOnly) {
    $taskDef.PSObject.Properties.Remove($key)
}

$updated = $false
foreach ($container in $taskDef.containerDefinitions) {
    if ($container.name -eq $ContainerName) {
        $container.image = $Image
        $updated = $true
    }
}
if (-not $updated) {
    throw "Container '$ContainerName' not found in task definition."
}

$taskJson = $taskDef | ConvertTo-Json -Depth 20 -Compress
$tempFile = [System.IO.Path]::GetTempFileName()
try {
    [System.IO.File]::WriteAllText($tempFile, $taskJson)
    $fileUri = "file://" + ($tempFile -replace '\\', '/')

    $registerResult = Invoke-AwsJson @(
        "ecs", "register-task-definition",
        "--region", $Region,
        "--cli-input-json", $fileUri
    )
    $newRevision = $registerResult.taskDefinition.revision
}
finally {
    Remove-Item -Path $tempFile -Force -ErrorAction SilentlyContinue
}

Write-Host "Registered task definition ${TaskFamily}:${newRevision}"

& aws ecs update-service `
    --cluster $Cluster `
    --service $Service `
    --task-definition "${TaskFamily}:${newRevision}" `
    --region $Region | Out-Null
if ($LASTEXITCODE -ne 0) { throw "aws ecs update-service failed." }

Write-Host "Service update initiated: ${TaskFamily}:${newRevision}"

if ($Verify) {
    Wait-ForDeployment
}
