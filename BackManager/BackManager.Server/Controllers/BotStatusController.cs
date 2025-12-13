using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace BackManager.Server.Controllers;

/// <summary>
/// 机器人状态控制器
/// </summary>
/// <remarks>
/// 机器人状态控制器构造函数
/// </remarks>
/// <param name="communicationService">通信服务</param>
/// <param name="botHost">机器人主机服务</param>
[ApiController]
[Route("api/bot")]
public class BotStatusController(
    ICommunicationService communicationService,
    BotHost botHost,
    ILogger<BotStatusController> logger) : ControllerBase
{
    private DateTimeOffset? _startTime;
    private string _status = "Stopped";

    /// <summary>
    /// 获取机器人状态
    /// </summary>
    /// <returns>机器人状态</returns>
    [HttpGet("status")]
    public IActionResult GetBotStatus()
    {
        try
        {
            // TODO; impl runtime info get
            var botStatus = new
            {
                Status = _status,
                StartTime = _startTime?.ToString("O"),
                Version = "1.0.0",
                Protocol = "OneBot 11"
            };

            return Ok(ApiResponse<object>.Ok(botStatus, "获取机器人状态成功"));
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get bot status: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取机器人状态失败: {ex.Message}"));
        }
    }
}
