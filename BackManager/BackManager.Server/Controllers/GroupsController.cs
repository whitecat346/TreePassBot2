using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using TreePassBot2.Data;

// ReSharper disable ComplexConditionExpression

namespace BackManager.Server.Controllers;

/// <summary>
/// 群组管理控制器
/// </summary>
/// <remarks>
/// 群组管理控制器构造函数
/// </remarks>
/// <param name="dbContext">数据库上下文</param>
[ApiController]
[Route("api/[controller]")]
public class GroupsController(
    BotDbContext dbContext,
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
            var groups = new List<object>
            {
                new
                {
                    Id = "1",
                    Name = "测试群组1",
                    MemberCount = 100,
                    OwnerId = "owner1",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2).ToString("o")
                },
                new
                {
                    Id = "2",
                    Name = "测试群组2",
                    MemberCount = 200,
                    OwnerId = "owner2",
                    CreatedAt = DateTime.UtcNow.AddMonths(-1).ToString("o")
                },
                new
                {
                    Id = "3",
                    Name = "测试群组3",
                    MemberCount = 50,
                    OwnerId = "owner3",
                    CreatedAt = DateTime.UtcNow.AddDays(-10).ToString("o")
                }
            };

            return Ok(ApiResponse<object>.Ok(groups, "获取群组列表成功"));
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get group list: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取群组列表失败: {ex.Message}"));
        }
    }
}
