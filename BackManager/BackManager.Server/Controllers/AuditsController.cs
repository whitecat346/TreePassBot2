using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Core.Entities.Enums;
using TreePassBot2.Data;

// ReSharper disable RedundantAnonymousTypePropertyName

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
public partial class AuditsController(
    BotDbContext dbContext,
    AuditManagerService auditManagerService,
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
                UserId = audit.UserId.ToString(),
                GroupId = audit.GroupId.ToString(),
                Status = audit.Status.ToString(),
                VerificationCode = audit.VerificationCode,
                EnteredGroup = audit.Status == AuditStatus.Approved,
                CreatedAt = audit.CreatedAt.ToString("O"),
                ProcessedAt = audit.ProcessedAt.ToString("O"),
                ProcessedBy = audit.ProcessedBy
            }).ToImmutableList();

            return Ok(ApiResponse<object>.Ok(auditRecords, "获取审核记录成功"));
        }
        catch (Exception ex)
        {
            LogFailedToGetAuditRecordsError(logger, ex.Message);
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
            if (!Guid.TryParse(auditId, out var requestId))
            {
                return BadRequest(ApiResponse<object>.Error("无效的审核ID"));
            }

            var (isSuccess, _) =
                await auditManagerService.ApproveAuditAsync(requestId, 0).ConfigureAwait(false);
            if (!isSuccess)
            {
                return NotFound(ApiResponse<object>.Error("审核记录不存在"));
            }

            return Ok(ApiResponse<object>.Ok(null, $"审核 {auditId} 已批准"));
        }
        catch (Exception ex)
        {
            LogFailedToApproveAuditError(logger, ex.Message);
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

            var (isSuccess, _) = await auditManagerService.DenyAuditAsync(id, 0).ConfigureAwait(false);
            if (!isSuccess)
            {
                return NotFound(ApiResponse<object>.Error("审核记录不存在"));
            }

            return Ok(ApiResponse<object>.Ok(null, $"审核 {auditId} 已拒绝"));
        }
        catch (Exception ex)
        {
            LogFailedToRejectAuditError(logger, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"拒绝审核失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 拒绝审核
    /// </summary>
    /// <param name="auditId">审核ID</param>
    /// <returns>操作结果</returns>
    [HttpPost("${auditId}/reset")]
    public async Task<IActionResult> ResetAudit(string auditId)
    {
        try
        {
            if (!Guid.TryParse(auditId, out var id))
            {
                return BadRequest(ApiResponse<object>.Error("无效的审核ID"));
            }

            var isSuccess = await auditManagerService.ResetAuditRequestsAsync(id).ConfigureAwait(false);
            if (!isSuccess)
            {
                return NotFound(ApiResponse<object>.Error("审核记录不存在"));
            }

            return Ok(ApiResponse<object>.Ok(null, $"审核 {auditId} 已重置"));
        }
        catch (Exception ex)
        {
            LogFailedToRejectAuditError(logger, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"重置审核失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 拒绝审核
    /// </summary>
    /// <param name="auditId">审核ID</param>
    /// <returns>操作结果</returns>
    [HttpPost("${auditId}/regenerate-code")]
    public async Task<IActionResult> RegeneratedVerificationCode(string auditId)
    {
        try
        {
            if (!Guid.TryParse(auditId, out var id))
            {
                return BadRequest(ApiResponse<object>.Error("无效的审核ID"));
            }

            var isSuccess = await auditManagerService.RegenerateVerificationCodeAsync(id).ConfigureAwait(false);
            if (!isSuccess)
            {
                return NotFound(ApiResponse<object>.Error("审核记录不存在"));
            }

            return Ok(ApiResponse<object>.Ok(null, $"审核 {auditId} 已重新生成验证码"));
        }
        catch (Exception ex)
        {
            LogFailedToRejectAuditError(logger, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"重新生成验证码失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 拒绝审核
    /// </summary>
    /// <param name="auditId">审核ID</param>
    /// <returns>操作结果</returns>
    [HttpPost("${auditId}/enteredGroup")]
    public async Task<IActionResult> CheckEnteredGroup(string auditId)
    {
        try
        {
            if (!Guid.TryParse(auditId, out var id))
            {
                return BadRequest(ApiResponse<object>.Error("无效的审核ID"));
            }

            var isJoined = await auditManagerService.IsJoinedGroupAsync(id).ConfigureAwait(false);
            if (!isJoined)
            {
                return NotFound(ApiResponse<object>.Error("审核记录不存在"));
            }

            return Ok(ApiResponse<object>.Ok(new { EnteredGroup = isJoined }, $"获取审核 {auditId} 状态成功"));
        }
        catch (Exception ex)
        {
            LogFailedToRejectAuditError(logger, ex.Message);
            return StatusCode(500, ApiResponse<object>.Error($"获取状态失败: {ex.Message}"));
        }
    }


    [LoggerMessage(LogLevel.Error, "Failed to get audit records: {error}")]
    static partial void LogFailedToGetAuditRecordsError(ILogger<AuditsController> logger, string error);

    [LoggerMessage(LogLevel.Error, "Failed to approve audit: {error}")]
    static partial void LogFailedToApproveAuditError(ILogger<AuditsController> logger, string error);

    [LoggerMessage(LogLevel.Error, "Failed to reject audit: {error}")]
    static partial void LogFailedToRejectAuditError(ILogger<AuditsController> logger, string error);
}
