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
/// <param name="runtimeInfo">机器人运行时信息</param>
[ApiController]
[Route("api/bot")]
public partial class BotStatusController(
    AppRuntimeInfo runtimeInfo,
    ICommunicationService communicationService,
    ILogger<BotStatusController> logger) : ControllerBase
{
    private readonly string _status = "Running";

    /// <summary>
    /// 获取机器人状态
    /// </summary>
    /// <returns>机器人状态</returns>
    [HttpGet("status")]
    public IActionResult GetBotStatus()
    {
        try
        {
            var botStatus = new
            {
                Status = _status,
                StartTime = runtimeInfo.Uptime.ToString("g"),
                Version = runtimeInfo.CurrentVersion,
                Protocol = "OneBot 11"
            };

            return Ok(ApiResponse<object>.Ok(botStatus, "获取机器人状态成功"));
        }
        catch (Exception ex)
        {
            LogFailedToGetBotStatusErrormessage(logger, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取机器人状态失败: {ex.Message}"));
        }
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartBot()
    {
        await communicationService.ConnectAsync().ConfigureAwait(false);
        return Ok(ApiResponse<object>.Ok(null, "启动机器人成功"));
    }

    [HttpPost("stop")]
    public async Task<IActionResult> StopBot()
    {
        await communicationService.DisconnectAsync().ConfigureAwait(false);
        return Ok(ApiResponse<object>.Ok(null, "停止机器人成功"));
    }

    [LoggerMessage(LogLevel.Error, "Failed to get bot status: {ErrorMessage}")]
    static partial void LogFailedToGetBotStatusErrormessage(ILogger<BotStatusController> logger, string ErrorMessage);
}
