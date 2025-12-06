using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackManager.Server.Controllers
{
    /// <summary>
    /// 服务器状态控制器
    /// </summary>
    [ApiController]
    [Route("api/server")]
    public class ServerStatusController : ControllerBase
    {
        /// <summary>
        /// 服务器状态模型
        /// </summary>
        private record ServerStatus
        {
            public double CpuUsage { get; set; }
            public double MemoryUsage { get; set; }
            public double DiskUsage { get; set; }
            public string Uptime { get; set; } = "0 days 00:00:00";
            public required string Timestamp { get; set; }
        }

        /// <summary>
        /// 获取服务器状态
        /// </summary>
        /// <returns>服务器状态</returns>
        [HttpGet("status")]
        public IActionResult GetServerStatus()
        {
            // 模拟服务器状态数据
            var serverStatus = new ServerStatus
            {
                CpuUsage = 45.2,
                MemoryUsage = 67.8,
                DiskUsage = 82.3,
                Uptime = "5 days 12:34:56",
                Timestamp = DateTime.UtcNow.ToString("o")
            };

            return Ok(ApiResponse<ServerStatus>.Ok(serverStatus, "获取服务器状态成功"));
        }
    }
}