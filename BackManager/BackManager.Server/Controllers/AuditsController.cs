using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TreePassBot2.Core.Entities.Enums;
using TreePassBot2.Data;

// ReSharper disable ComplexConditionExpression

namespace BackManager.Server.Controllers;

/// <summary>
/// 审核记录控制器
/// </summary>
/// <remarks>
/// 审核记录控制器构造函数
/// </remarks>
/// <param name="dbContext">数据库上下文</param>
[ApiController]
[Route("api/[controller]")]
public class AuditsController(
    BotDbContext dbContext,
    ILogger<AuditsController> logger) : ControllerBase
{
    /// <summary>
    /// 获取审核记录列表
    /// </summary>
    /// <returns>审核记录列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetAuditRecords()
    {
        try
        {
            var auditRequests =
                await dbContext.AuditRequests
                               .OrderByDescending(a => a.CreatedAt)
                               .ToListAsync().ConfigureAwait(false);

            var auditRecords = auditRequests.Select(audit => new
            {
                Id = audit.Id.ToString(),
                UserId = audit.RequestQqId.ToString(),
                GroupId = audit.TargetGroupId.ToString(),
                Status = audit.Status.ToString(),
                VerificationCode = audit.Passcode,
                EnteredGroup = audit.Status == AuditStatus.Approved,
                CreatedAt = audit.CreatedAt.ToString("O"),
                ProcessedAt = audit.ProcessedAt.ToString("O"),
                audit.ProcessedBy
            }).ToImmutableList();

            return Ok(ApiResponse<object>.Ok(auditRecords, "获取审核记录成功"));
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get audit records: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取审核记录失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 批准审核
    /// </summary>
    /// <param name="auditId">审核ID</param>
    /// <returns>操作结果</returns>
    [HttpPost("{auditId}/approve")]
    public async Task<IActionResult> ApproveAudit(string auditId)
    {
        try
        {
            if (!Guid.TryParse(auditId, out var id))
            {
                return BadRequest(ApiResponse<object>.Error("无效的审核ID"));
            }

            var auditRequest = await dbContext.AuditRequests.FindAsync(id).ConfigureAwait(false);
            if (auditRequest == null)
            {
                return NotFound(ApiResponse<object>.Error("审核记录不存在"));
            }

            auditRequest.Status = AuditStatus.Approved;
            auditRequest.ProcessedBy = 0;
            auditRequest.ProcessedAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return Ok(ApiResponse<object>.Ok(null, $"审核 {auditId} 已批准"));
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to approve audit: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"批准审核失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 拒绝审核
    /// </summary>
    /// <param name="auditId">审核ID</param>
    /// <returns>操作结果</returns>
    [HttpPost("{auditId}/reject")]
    public async Task<IActionResult> RejectAudit(string auditId)
    {
        try
        {
            if (!Guid.TryParse(auditId, out var id))
            {
                return BadRequest(ApiResponse<object>.Error("无效的审核ID"));
            }

            var auditRequest = await dbContext.AuditRequests.FindAsync(id).ConfigureAwait(false);
            if (auditRequest == null)
            {
                return NotFound(ApiResponse<object>.Error("审核记录不存在"));
            }

            auditRequest.Status = AuditStatus.Rejected;
            auditRequest.ProcessedBy = 0;
            auditRequest.ProcessedAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return Ok(ApiResponse<object>.Ok(null, $"审核 {auditId} 已拒绝"));
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to reject audit: {Error}", ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"拒绝审核失败: {ex.Message}"));
        }
    }
}
