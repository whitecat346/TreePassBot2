using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Core.Options;

namespace BackManager.Server.Controllers;

/// <summary>
/// 插件管理控制器
/// </summary>
/// <remarks>
/// 插件管理控制器构造函数
/// </remarks>
/// <param name="pluginManager">插件管理服务</param>
/// <param name="logger">日志记录器</param>
/// <param name="config">配置</param>
[ApiController]
[Route("api/[controller]")]
public partial class PluginsController(
    PluginManagerService pluginManager,
    ILogger<PluginsController> logger,
    IOptions<BotOptions> config) : ControllerBase
{
    private readonly BotOptions _config = config.Value;

    /// <summary>
    /// 获取插件列表
    /// </summary>
    /// <returns>插件列表</returns>
    [HttpGet]
    public Task<IActionResult> GetPlugins()
    {
        try
        {
            var activativePlugins = pluginManager.ActivePlugins;

            var plugins = activativePlugins.Select(p => new
            {
                p.Meta.Name,
                p.Meta.Version,
                p.Meta.Author,
                p.Meta.Description,
                p.Meta.Id,
                IsActive = p.IsAlive
            }).ToList();

            return Task.FromResult<IActionResult>(Ok(ApiResponse<object>.Ok(plugins, "获取插件列表成功")));
        }
        catch (Exception ex)
        {
            LogFailedToGetPluginListError(logger, ex.Message);
            return Task.FromResult<IActionResult>(
                StatusCode(500, ApiResponse<object>.Error($"获取插件列表失败: {ex.Message}")));
        }
    }

    [HttpGet("enabledCount")]
    public Task<IActionResult> GetEnabledPluginCount()
    {
        try
        {
            var count = pluginManager.ActivePlugins.Count(p => p.IsAlive);
            return Task.FromResult<IActionResult>(Ok(ApiResponse<int>.Ok(count, "获取启用的插件数量成功")));
        }
        catch (Exception ex)
        {
            LogFailedToGetEnabledPluginCountExmessage(logger, ex.Message);
            return Task.FromResult<IActionResult>(
                StatusCode(500, ApiResponse<object>.Error($"获取启用的插件数量失败: {ex.Message}")));
        }
    }

    /// <summary>
    /// 上传新插件
    /// </summary>
    /// <param name="pluginFile">插件文件</param>
    /// <returns>上传结果</returns>
    [HttpPost("upload")]
    [RequestFormLimits(MultipartBodyLengthLimit = 20971520)] // 20MB
    [RequestSizeLimit(20971520)]
    public async Task<IActionResult> UploadPlugin(IFormFile pluginFile)
    {
        try
        {
            if (pluginFile.Length == 0)
            {
                LogUploadedPluginFileIsEmpty(logger);
                return BadRequest(ApiResponse<object>.Error("请选择要上传的插件文件", 400));
            }

            var fileExtension = Path.GetExtension(pluginFile.FileName);
            if (fileExtension != ".dll") // ensure right file type
            {
                LogInvalidFileTypeUploadedFilename(logger, pluginFile.FileName);
                return BadRequest(ApiResponse<object>.Error("只允许上传 .dll 格式的插件文件", 400));
            }

            var pluginDir = _config.PluginDir;
            if (!Path.IsPathRooted(pluginDir))
            {
                var contentRootPath = Directory.GetCurrentDirectory();
                pluginDir = Path.Combine(contentRootPath, pluginDir);
            }

            // create plugin directory if not exists
            if (!Directory.Exists(pluginDir))
            {
                LogCreatingPluginDirectoryDirectory(logger, pluginDir);
                Directory.CreateDirectory(pluginDir);
            }

            // save plugin file
            var uniqueFileName = $"{Guid.NewGuid()}_{pluginFile.FileName}";
            var filePath = Path.Combine(pluginDir, uniqueFileName);

            LogSavingPluginFileToFilepath(logger, filePath);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await pluginFile.CopyToAsync(stream).ConfigureAwait(false);
            }

            LogPluginFileUploadedSuccessfullyFilename(logger, pluginFile.FileName);

            _ = Task.Run(() => pluginManager.LoadPluginAsync(filePath));

            return Ok(ApiResponse<object>.Ok(
                          new { name = pluginFile.FileName, path = filePath },
                          $"插件 {pluginFile.FileName} 上传成功"));
        }
        catch (Exception ex)
        {
            LogFailedToUploadPluginFileError(logger, ex, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"上传插件失败: {ex.Message}"));
        }
    }

    #region LoggerMethods

    [LoggerMessage(LogLevel.Error, "Failed to get plugin list: {error}")]
    static partial void LogFailedToGetPluginListError(ILogger<PluginsController> logger, string error);

    [LoggerMessage(LogLevel.Warning, "Uploaded plugin file is empty")]
    static partial void LogUploadedPluginFileIsEmpty(ILogger<PluginsController> logger);

    [LoggerMessage(LogLevel.Warning, "Invalid file type uploaded: {fileName}")]
    static partial void LogInvalidFileTypeUploadedFilename(ILogger<PluginsController> logger, string fileName);

    [LoggerMessage(LogLevel.Information, "Creating plugin directory: {directory}")]
    static partial void LogCreatingPluginDirectoryDirectory(ILogger<PluginsController> logger, string directory);

    [LoggerMessage(LogLevel.Information, "Saving plugin file to: {filePath}")]
    static partial void LogSavingPluginFileToFilepath(ILogger<PluginsController> logger, string filePath);

    [LoggerMessage(LogLevel.Information, "Plugin file uploaded successfully: {fileName}")]
    static partial void LogPluginFileUploadedSuccessfullyFilename(ILogger<PluginsController> logger, string fileName);

    [LoggerMessage(LogLevel.Error, "Failed to upload plugin file: {msg}")]
    static partial void LogFailedToUploadPluginFileError(ILogger<PluginsController> logger, Exception error,
                                                         string msg);

    #endregion

    [LoggerMessage(LogLevel.Error, "Failed to get enabled plugin count: {ExMessage}")]
    static partial void LogFailedToGetEnabledPluginCountExmessage(ILogger<PluginsController> logger, string ExMessage);
}
