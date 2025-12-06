<#
.SYNOPSIS
    Docker部署脚本
.DESCRIPTION
    此脚本用于构建和运行Docker镜像
    确保已安装Docker Desktop
.EXAMPLE
    .\docker-deploy.ps1
#>

# 设置颜色
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "开始Docker部署" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# 检查Docker是否运行
$dockerRunning = docker info 2>$null
if (-not $dockerRunning) {
    Write-Host "Docker未运行！请先启动Docker Desktop" -ForegroundColor Red
    exit 1
}

# 构建Docker镜像
Write-Host "正在构建Docker镜像..." -ForegroundColor Yellow
& docker build -t backmanager:latest -f .\BackManager.Server\Dockerfile ..

if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker构建失败！" -ForegroundColor Red
    exit $LASTEXITCODE
}

# 停止旧容器
Write-Host "正在停止旧容器..." -ForegroundColor Yellow
& docker stop backmanager 2>$null
& docker rm backmanager 2>$null

# 运行新容器
Write-Host "正在启动新容器..." -ForegroundColor Yellow
& docker run -d -p 8080:8080 -p 8081:8081 --name backmanager backmanager:latest

if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker运行失败！" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Docker部署成功！" -ForegroundColor Green
Write-Host "访问地址: http://localhost:8080" -ForegroundColor Green
Write-Host "HTTPS地址: https://localhost:8081" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "查看日志: docker logs -f backmanager" -ForegroundColor Yellow
Write-Host "停止容器: docker stop backmanager" -ForegroundColor Yellow
Write-Host "=========================================" -ForegroundColor Cyan