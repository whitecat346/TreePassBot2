using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackManager.Server.Controllers
{
    /// <summary>
    /// 插件管理控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PluginsController : ControllerBase
    {
        /// <summary>
        /// 插件模型
        /// </summary>
        private record Plugin
        {
            public required string Id { get; set; }
            public required string Name { get; set; }
            public required string Description { get; set; }
            public bool IsEnabled { get; set; }
            public required string Status { get; set; }
            public required string Version { get; set; }
        }

        /// <summary>
        /// 获取插件列表
        /// </summary>
        /// <returns>插件列表</returns>
        [HttpGet]
        public IActionResult GetPlugins()
        {
            // 模拟插件数据
            var plugins = new List<Plugin>
            {
                new()
                {
                    Id = "1",
                    Name = "测试插件1",
                    Description = "这是一个测试插件",
                    IsEnabled = true,
                    Status = "Running",
                    Version = "1.0.0"
                },
                new()
                {
                    Id = "2",
                    Name = "测试插件2",
                    Description = "这是另一个测试插件",
                    IsEnabled = false,
                    Status = "Disabled",
                    Version = "2.0.0"
                },
                new()
                {
                    Id = "3",
                    Name = "测试插件3",
                    Description = "这是第三个测试插件",
                    IsEnabled = true,
                    Status = "Warning",
                    Version = "1.5.0"
                }
            };

            return Ok(ApiResponse<List<Plugin>>.Ok(plugins, "获取插件列表成功"));
        }

        /// <summary>
        /// 切换插件状态
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{pluginId}/toggle")]
        public IActionResult TogglePlugin(string pluginId)
        {
            // 模拟切换插件状态
            return Ok(ApiResponse<object>.Ok(null, $"插件 {pluginId} 状态已更新"));
        }

        /// <summary>
        /// 上传插件
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpPost("upload")]
        public IActionResult UploadPlugin()
        {
            // 模拟上传插件
            var plugin = new Plugin
            {
                Id = "4",
                Name = "新上传的插件",
                Description = "这是一个新上传的插件",
                IsEnabled = false,
                Status = "Disabled",
                Version = "1.0.0"
            };

            return Ok(ApiResponse<Plugin>.Ok(plugin, "插件上传成功"));
        }
    }
}
