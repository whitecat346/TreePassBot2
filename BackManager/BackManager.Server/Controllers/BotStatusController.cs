using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackManager.Server.Controllers
{
    /// <summary>
    /// 机器人状态控制器
    /// </summary>
    [ApiController]
    [Route("api/bot")]
    public class BotStatusController : ControllerBase
    {
        /// <summary>
        /// 机器人状态模型
        /// </summary>
        private class BotStatus
        {
            public string Status { get; set; } = "Running";
            public string? StartTime { get; set; }
            public string Version { get; set; } = "1.0.0";
            public string Protocol { get; set; } = "OneBot 11";
        }

        /// <summary>
        /// 获取机器人状态
        /// </summary>
        /// <returns>机器人状态</returns>
        [HttpGet("status")]
        public IActionResult GetBotStatus()
        {
            // 模拟机器人状态数据
            var botStatus = new BotStatus
            {
                Status = "Running",
                StartTime = DateTime.UtcNow.AddHours(-2).ToString("o"),
                Version = "1.0.0",
                Protocol = "OneBot 11"
            };

            return Ok(ApiResponse<BotStatus>.Ok(botStatus, "获取机器人状态成功"));
        }

        /// <summary>
        /// 启动机器人
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpPost("start")]
        public IActionResult StartBot()
        {
            // 模拟启动机器人
            return Ok(ApiResponse<object>.Ok(null, "机器人启动成功"));
        }

        /// <summary>
        /// 停止机器人
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpPost("stop")]
        public IActionResult StopBot()
        {
            // 模拟停止机器人
            return Ok(ApiResponse<object>.Ok(null, "机器人停止成功"));
        }
    }
}