<#
.SYNOPSIS
    一键构建前后端项目
.DESCRIPTION
    此脚本用于同时构建后端ASP.NET Core项目和前端Vue项目
    确保已安装.NET SDK和Node.js
.EXAMPLE
    .\build-all.ps1
#>

# 设置颜色
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "开始构建前后端项目" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# 构建后端项目
Write-Host "正在构建后端项目..." -ForegroundColor Yellow
Push-Location .\BackManager.Server
& dotnet build -c Release
Pop-Location

if ($LASTEXITCODE -ne 0) {
    Write-Host "后端构建失败！" -ForegroundColor Red
    exit $LASTEXITCODE
}

# 构建前端项目
Write-Host "正在构建前端项目..." -ForegroundColor Yellow
Push-Location .\backmanager.client
& npm install
& npm run build
Pop-Location

if ($LASTEXITCODE -ne 0) {
    Write-Host "前端构建失败！" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "前后端项目构建成功！" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan