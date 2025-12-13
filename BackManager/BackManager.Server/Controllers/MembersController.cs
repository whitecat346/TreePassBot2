using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreePassBot2.Data;

// ReSharper disable ComplexConditionExpression

namespace BackManager.Server.Controllers;

/// <summary>
/// 成员管理控制器
/// </summary>
/// <remarks>
/// 成员管理控制器构造函数
/// </remarks>
/// <param name="dbContext">数据库上下文</param>
[ApiController]
[Route("api/groups/{groupId}/members")]
public class MembersController(
    BotDbContext dbContext,
    ILogger<MembersController> logger) : ControllerBase
{
    /// <summary>
    /// 获取群组成员列表
    /// </summary>
    /// <param name="groupId">群组ID</param>
    /// <returns>成员列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetGroupMembers(string groupId)
    {
        try
        {
            var users = await dbContext.Users.ToListAsync().ConfigureAwait(false);

            var members = users.Select((user, index) => new
            {
                Id = user.Id.ToString(),
                GroupId = groupId,
                UserId = user.QqId.ToString(),
                Username = user.UserName ?? $"用户{user.QqId}",
                Role = index == 0 ? "管理员" : "成员"
            }).ToList();

            return Ok(ApiResponse<object>.Ok(members, "获取群组成员列表成功"));
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get group member list: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取群组成员列表失败: {ex.Message}"));
        }
    }
}
