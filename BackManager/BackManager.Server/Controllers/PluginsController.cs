using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Data;

namespace BackManager.Server.Controllers;

/// <summary>
/// 插件管理控制器
/// </summary>
/// <remarks>
/// 插件管理控制器构造函数
/// </remarks>
/// <param name="pluginManager">插件管理服务</param>
/// <param name="dbContext">数据库上下文</param>
[ApiController]
[Route("api/[controller]")]
public class PluginsController(
    PluginManagerService pluginManager,
    BotDbContext dbContext,
    ILogger<PluginsController> logger) : ControllerBase
{
    /// <summary>
    /// 获取插件列表
    /// </summary>
    /// <returns>插件列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetPlugins()
    {
        try
        {
            // 从数据库获取插件状态
            var pluginStates = dbContext.PluginStates
                                        .ToList();

            // 目前PluginManagerService没有提供获取插件列表的方法，这里使用模拟数据
            // 实际项目中应该扩展PluginManagerService，添加获取插件列表的方法
            var plugins = new List<object>
            {
                new
                {
                    Id = "1",
                    Name = "测试插件1",
                    Description = "这是一个测试插件",
                    IsEnabled = true,
                    Status = "Running",
                    Version = "1.0.0"
                },
                new
                {
                    Id = "2",
                    Name = "测试插件2",
                    Description = "这是另一个测试插件",
                    IsEnabled = false,
                    Status = "Disabled",
                    Version = "2.0.0"
                },
                new
                {
                    Id = "3",
                    Name = "测试插件3",
                    Description = "这是第三个测试插件",
                    IsEnabled = true,
                    Status = "Warning",
                    Version = "1.5.0"
                }
            };

            foreach (var plugin in plugins)
            {
                var pluginDict = (System.Collections.Generic.IDictionary<string, object>)plugin;
                var pluginId = pluginDict["Id"].ToString()!;
                var pluginState = pluginStates.FirstOrDefault(p => p.PluginId == pluginId);
                if (pluginState != null)
                {
                    pluginDict["IsEnabled"] = true;
                }
            }

            return Ok(ApiResponse<object>.Ok(plugins, "获取插件列表成功"));
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get plugin list: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取插件列表失败: {ex.Message}"));
        }
    }
}
