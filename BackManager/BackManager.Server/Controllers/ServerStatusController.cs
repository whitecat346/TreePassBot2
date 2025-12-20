using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using TreePassBot2.BotEngine.Services;

namespace BackManager.Server.Controllers;

/// <summary>
/// 服务器状态控制器
/// </summary>
[ApiController]
[Route("api/server")]
public class ServerStatusController(
    AppRuntimeInfo runtimeInfo,
    ILogger<ServerStatusController> logger) : ControllerBase
{
    /// <summary>
    /// 获取服务器状态
    /// </summary>
    /// <returns>服务器状态</returns>
    [HttpGet("status")]
    public async Task<IActionResult> GetServerStatus()
    {
        try
        {
            var serverStatus = new
            {
                CpuUsage = await runtimeInfo.GetCpuUsageAsync().ConfigureAwait(false),
                MemoryUsage = runtimeInfo.GetMemoryUsage(),
                DiskUsage = runtimeInfo.GetDiskUsage(),
                Uptime = runtimeInfo.Uptime.ToString("g"),
                Timestamp = DateTime.UtcNow.ToString("s")
            };

            return Ok(ApiResponse<object>.Ok(serverStatus, "获取服务器状态成功"));
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to  get server status: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取服务器状态失败: {ex.Message}"));
        }
    }
}
