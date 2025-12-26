using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using TreePassBot2.BotEngine.Services;

// ReSharper disable ComplexConditionExpression

namespace BackManager.Server.Controllers;

/// <summary>
/// 群组管理控制器
/// </summary>
/// <remarks>
/// 群组管理控制器构造函数
/// </remarks>
/// <param name="userManage">用户管理服务</param>
[ApiController]
[Route("api/[controller]")]
public partial class GroupsController(
    UserManageService userManage,
    ILogger<GroupsController> logger) : ControllerBase
{
    /// <summary>
    /// 获取群组列表
    /// </summary>
    /// <returns>群组列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        try
        {
            var groups = await userManage.GetGroupListAsync().ConfigureAwait(false);

            return Ok(ApiResponse<object>.Ok(groups, "获取群组列表成功"));
        }
        catch (Exception ex)
        {
            LogFailedToGetGroupListError(logger, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取群组列表失败: {ex.Message}"));
        }
    }

    [HttpGet("totalGroupCount")]
    public async Task<IActionResult> GetTotalGroupCount()
    {
        try
        {
            var groups = await userManage.GetGroupListAsync().ConfigureAwait(false);
            var totalCount = groups.Count;
            return Ok(ApiResponse<int>.Ok(totalCount, "获取群组总数成功"));
        }
        catch (Exception ex)
        {
            LogFailedToGetTotalGroupCountError(logger, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取群组总数失败: {ex.Message}"));
        }
    }

    [LoggerMessage(LogLevel.Error, "Failed to get group list: {Error}")]
    static partial void LogFailedToGetGroupListError(ILogger<GroupsController> logger, string Error);

    [LoggerMessage(LogLevel.Error, "Failed to get total group count: {Error}")]
    static partial void LogFailedToGetTotalGroupCountError(ILogger<GroupsController> logger, string Error);
}
