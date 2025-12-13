using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TreePassBot2.Data;

namespace BackManager.Server.Controllers;

/// <summary>
/// 消息日志控制器
/// </summary>
/// <remarks>
/// 消息日志控制器构造函数
/// </remarks>
/// <param name="dbContext">数据库上下文</param>
[ApiController]
[Route("api/messages")]
public class MessageLogsController(
    BotDbContext dbContext,
    ILogger<MessageLogsController> logger) : ControllerBase
{
    /// <summary>
    /// 获取消息日志列表
    /// </summary>
    /// <param name="groupId">群组ID（可选）</param>
    /// <param name="startTime">开始时间（可选）</param>
    /// <param name="endTime">结束时间（可选）</param>
    /// <param name="beforeId">分页游标（可选）</param>
    /// <param name="limit">每页数量（可选，默认20）</param>
    /// <returns>消息日志列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetMessageLogs(
        [FromQuery] string? groupId,
        [FromQuery] string? startTime,
        [FromQuery] string? endTime,
        [FromQuery] string? beforeId,
        [FromQuery] int limit = 20)
    {
        try
        {
            var query = dbContext.MessageLogs.AsQueryable();

            if (!string.IsNullOrEmpty(groupId) && ulong.TryParse(groupId, out var groupIdValue))
            {
                query = query.Where(m => m.GroupId == groupIdValue);
            }

            if (!string.IsNullOrEmpty(startTime) && DateTimeOffset.TryParse(startTime, out var startTimeValue))
            {
                query = query.Where(m => m.SendAt >= startTimeValue);
            }

            if (!string.IsNullOrEmpty(endTime) && DateTimeOffset.TryParse(endTime, out var endTimeValue))
            {
                query = query.Where(m => m.SendAt <= endTimeValue);
            }

            if (!string.IsNullOrEmpty(beforeId) && long.TryParse(beforeId, out var beforeIdValue))
            {
                query = query.Where(m => m.Id < beforeIdValue);
            }

            var messageLogs = await query
                                   .OrderByDescending(m => m.Id)
                                   .Take(limit + 1) // for check has more
                                   .ToListAsync().ConfigureAwait(false);

            var hasMore = messageLogs.Count > limit;
            if (hasMore)
            {
                messageLogs.RemoveAt(messageLogs.Count - 1);
            }

            var logs = messageLogs.Select(m => new
            {
                Id = m.Id.ToString(),
                GroupId = m.GroupId,
                GroupName = m.GroupId,
                UserId = m.UserId.ToString(),
                Username = m.UserNickName ?? $"用户{m.UserId}",
                Content = m.ContentText,
                SendTime = m.SendAt.ToString("O"),
                IsRecalled = m.IsWithdrawed,
                RecalledBy = m.WithdrawedBy?.ToString(),
                RecalledAt = string.Empty
            }).ToImmutableList();

            var response = new
            {
                items = logs,
                hasMore = hasMore,
                nextCursor = hasMore ? logs[^1].Id : string.Empty,
                total = await dbContext.MessageLogs.CountAsync().ConfigureAwait(false)
            };

            return Ok(ApiResponse<object>.Ok(response, "获取消息日志成功"));
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get message log: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取消息日志失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 获取消息详情
    /// </summary>
    /// <param name="messageId">消息ID</param>
    /// <returns>消息详情</returns>
    [HttpGet("{messageId}")]
    public async Task<IActionResult> GetMessageDetail(string messageId)
    {
        try
        {
            if (!long.TryParse(messageId, out var id))
            {
                return BadRequest(ApiResponse<object>.Error("无效的消息ID"));
            }

            var messageLog = await dbContext.MessageLogs.FindAsync(id).ConfigureAwait(false);
            if (messageLog == null)
            {
                return NotFound(ApiResponse<object>.Error("消息不存在"));
            }

            var log = new
            {
                Id = messageLog.Id.ToString(),
                GroupId = messageLog.GroupId.ToString(),
                GroupName = $"群组{messageLog.GroupId}",
                UserId = messageLog.UserId.ToString(),
                Username = messageLog.UserNickName ?? $"用户{messageLog.UserId}",
                Content = messageLog.ContentText,
                SendTime = messageLog.SendAt.ToString("o"),
                IsRecalled = messageLog.IsWithdrawed,
                RecalledBy = messageLog.WithdrawedBy?.ToString(),
                RecalledAt = string.Empty
            };

            return Ok(ApiResponse<object>.Ok(log, "获取消息详情成功"));
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get message datails: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取消息详情失败: {ex.Message}"));
        }
    }
}
