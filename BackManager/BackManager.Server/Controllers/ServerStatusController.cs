using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackManager.Server.Controllers;

/// <summary>
/// 服务器状态控制器
/// </summary>
[ApiController]
[Route("api/server")]
public class ServerStatusController(ILogger<ServerStatusController> logger) : ControllerBase
{
    /// <summary>
    /// 获取服务器状态
    /// </summary>
    /// <returns>服务器状态</returns>
    [HttpGet("status")]
    public IActionResult GetServerStatus()
    {
        try
        {
            var serverStatus = new
            {
                CpuUsage = 45.2,
                MemoryUsage = 67.8,
                DiskUsage = 82.3,
                Uptime = "5 days 12:34:56",
                Timestamp = DateTime.UtcNow.ToString("o")
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
