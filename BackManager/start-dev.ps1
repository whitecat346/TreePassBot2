<#
.SYNOPSIS
    一键启动前后端开发环境
.DESCRIPTION
    此脚本用于同时启动后端ASP.NET Core项目和前端Vue项目
    确保已安装.NET SDK和Node.js
.EXAMPLE
    .\start-dev.ps1
#>

# 设置颜色
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "开始启动前后端开发环境" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# 启动后端项目
Write-Host "正在启动后端项目..." -ForegroundColor Yellow
$backendProcess = Start-Process -FilePath "dotnet" -ArgumentList "watch run" -WorkingDirectory ".\BackManager.Server" -PassThru

# 等待后端启动
Start-Sleep -Seconds 5

# 启动前端项目
Write-Host "正在启动前端项目..." -ForegroundColor Yellow
$frontendProcess = Start-Process -FilePath "npm" -ArgumentList "run dev" -WorkingDirectory ".\backmanager.client" -PassThru

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "前后端开发环境已启动" -ForegroundColor Green
Write-Host "后端地址: https://localhost:7248" -ForegroundColor Green
Write-Host "前端地址: https://localhost:5201" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "按任意键停止服务..." -ForegroundColor Yellow

# 等待用户输入
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# 停止进程
Write-Host "正在停止服务..." -ForegroundColor Yellow
$backendProcess.Kill()
$frontendProcess.Kill()

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "服务已停止" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan