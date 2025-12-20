using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using TreePassBot2.BotEngine.Services;

// ReSharper disable ComplexConditionExpression

namespace BackManager.Server.Controllers;

/// <summary>
/// 成员管理控制器
/// </summary>
/// <remarks>
/// 成员管理控制器构造函数
/// </remarks>
[ApiController]
[Route("api/groups")]
public partial class MembersController(
    UserManageService userManage,
    ILogger<MembersController> logger) : ControllerBase
{
    /// <summary>
    /// 获取群组成员列表
    /// </summary>
    /// <param name="groupId">群组ID</param>
    /// <param name="limit">Take count</param>
    /// <param name="skip">Skip count</param>
    /// <returns>成员列表</returns>
    [HttpGet("{groupId}/members")]
    public async Task<IActionResult> GetGroupMembers(
        string groupId,
        [FromQuery] int limit,
        [FromQuery] int skip)
    {
        LogGettingGroupMembersForGroupidGroupidLimitLimitSkipSkip(logger, groupId, limit, skip);

        try
        {
            var groupIdLong = ulong.Parse(groupId);

            var fetched = await userManage.GetMemberListFromApiAsync(groupIdLong).ConfigureAwait(false);

            var users = fetched.Skip(skip).Take(limit).ToList();

            var members = users.Select((user, index) => new
            {
                GroupId = groupId,
                UserId = user.QqId.ToString(),
                Username = user.UserName ?? $"用户{user.QqId}",
                Nickname = user.UserName ?? $"用户{user.QqId}",
                Role = user.Role.ToString(),
                JoinedAt = user.JoinedAt.ToString("O")
            }).ToList();

            return Ok(ApiResponse<object>.Ok(members, "获取群组成员列表成功"));
        }
        catch (Exception ex)
        {
            LogFailedToGetGroupMemberListError(logger, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取群组成员列表失败: {ex.Message}"));
        }
    }

    [HttpGet("{groupId}/count")]
    public async Task<IActionResult> GetGroupMemberCount(string groupId)
    {
        try
        {
            var groupIdLong = ulong.Parse(groupId);
            var fetched = await userManage.GetMemberListFromApiAsync(groupIdLong).ConfigureAwait(false);
            var count = fetched.Count;
            return Ok(ApiResponse<object>.Ok(new { Count = count }, "获取群组成员数量成功"));
        }
        catch (Exception ex)
        {
            LogFailedToGetGroupMemberCountError(logger, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取群组成员数量失败: {ex.Message}"));
        }
    }

    [HttpGet("totalMemberCount")]
    public async Task<IActionResult> GetTotalMemberCount()
    {
        try
        {
            var groups = await userManage.GetGroupListFromApiAsync().ConfigureAwait(false);
            var totalCount = 0;
            foreach (var group in groups)
            {
                var members = await userManage.GetMemberListFromApiAsync(group.GroupId).ConfigureAwait(false);
                totalCount += members.Count;
            }

            return Ok(ApiResponse<object>.Ok(new { TotalCount = totalCount }, "获取所有群组成员总数成功"));
        }
        catch (Exception ex)
        {
            LogFailedToGetGroupMemberCountError(logger, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取所有群组成员总数失败: {ex.Message}"));
        }
    }

    [LoggerMessage(LogLevel.Debug, "Getting group members for groupId: {groupId}, limit: {limit}, skip: {skip}")]
    static partial void LogGettingGroupMembersForGroupidGroupidLimitLimitSkipSkip(
        ILogger<MembersController> logger, string groupId, int limit, int skip);

    [LoggerMessage(LogLevel.Error, "Failed to get group member list: {error}")]
    static partial void LogFailedToGetGroupMemberListError(ILogger<MembersController> logger, string error);

    [LoggerMessage(LogLevel.Error, "Failed to get group member count: {error}")]
    static partial void LogFailedToGetGroupMemberCountError(ILogger<MembersController> logger, string error);
}
