using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Utils;
using TreePassBot2.Core.Entities;
using TreePassBot2.Core.Entities.Enums;
using TreePassBot2.Data;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

// ReSharper disable FlagArgument

namespace TreePassBot2.BotEngine.Services;

public partial class AuditManagerService(
    BotDbContext db,
    ICommunicationService commuService,
    ILogger<AuditManagerService> logger)
{
    private const int VerificationExpiryMinutes = 30;

    #region Public API

    public Task<ServiceResult> ApproveAuditAsync(ulong identifier, ulong processorId) =>
        ProcessAuditWorkflowAsycAsync(identifier, processorId, AuditStatus.Approved);

    public Task<ServiceResult> ApproveAuditAsync(Guid auditId, ulong processorId) =>
        ProcessAuditWorkflowAsycAsync(auditId, processorId, AuditStatus.Approved);

    public Task<ServiceResult> DenyAuditAsync(ulong identifier, ulong processorId) =>
        ProcessAuditWorkflowAsycAsync(identifier, processorId, AuditStatus.Rejected);

    public Task<ServiceResult> DenyAuditAsync(Guid identifier, ulong processorId) =>
        ProcessAuditWorkflowAsycAsync(identifier, processorId, AuditStatus.Rejected);

    #endregion

    private async Task<ServiceResult> ProcessAuditWorkflowAsycAsync(object identifier,
                                                                    ulong processorId,
                                                                    AuditStatus targetStatus)
    {
        var auditRequest = identifier switch
        {
            Guid auditId => await GetAuditRequestAsync(auditId).ConfigureAwait(false),
            ulong userId => await GetAuditRequestAsync(userId).ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(identifier), identifier, "Invalied user identifier.")
        };

        if (auditRequest is null)
        {
            LogAuditNotFound(logger, identifier.ToString()!);
            return new(false, "Audit request not found.");
        }

        auditRequest.Status = targetStatus;
        auditRequest.ProcessedAt = DateTimeOffset.UtcNow;
        auditRequest.ProcessedBy = processorId;

        string? vCode = null;
        if (targetStatus is AuditStatus.Approved)
        {
            vCode = VerificationCodeGenerator.GenerateNumericCode(10);
            auditRequest.VerificationCode = vCode;
        }

        db.AuditRequests.Update(auditRequest);
        await db.SaveChangesAsync().ConfigureAwait(false);

        await SendNotificationAsync(auditRequest, targetStatus, vCode).ConfigureAwait(false);

        LogProcessed(logger, identifier.ToString()!, processorId, targetStatus);
        return new(true, "Success");
    }

    private async Task SendNotificationAsync(AuditRequestData request, AuditStatus status, string? code)
    {
        if (status is AuditStatus.Approved)
        {
            await commuService.AnnounceApprovedAuditActionAsync(
                                   request.UserId, request.GroupId, "您的申请已通过！", code!)
                              .ConfigureAwait(false);
        }
        else
        {
            await commuService.AnnounceRejectedAuditActionAsync(
                                   request.UserId, request.GroupId, "您的申请未通过，请重新申请。")
                              .ConfigureAwait(false);
        }
    }


    public async Task<(bool IsAllowed, string Message)> CheckVerificationCodeAsync(
        ulong userId, string verificationCode)
    {
        var auditRequest = await GetAuditRequestAsync(userId).ConfigureAwait(false);

        if (auditRequest == null)
        {
            LogAuditNotFound(logger, userId.ToString());
            return (false, "User does not exist in database.");
        }

        if (auditRequest.Status is not AuditStatus.Approved)
        {
            return (false, "Audit request is not approved.");
        }

        var isExpired = (DateTimeOffset.UtcNow - auditRequest.ProcessedAt)
                      > TimeSpan.FromMinutes(VerificationExpiryMinutes);
        if (isExpired)
        {
            return (false, "Verification code has expired.");
        }

        var isMatch = string.Equals(auditRequest.VerificationCode, verificationCode,
                                    StringComparison.OrdinalIgnoreCase);
        if (!isMatch)
        {
            return (false, "Verification code is incorrect.");
        }

        return (true, string.Empty);
    }

    #region Basic CRUD

    public async Task<bool> AddAuditRequestAsync(ulong userId, ulong groupId)
    {
        await RemoveAuditRequestAsync(userId, groupId).ConfigureAwait(false);

        var auditRequest = new AuditRequestData
        {
            UserId = userId,
            GroupId = groupId
        };

        await db.AuditRequests.AddAsync(auditRequest).ConfigureAwait(false);
        await db.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }

    public Task RemoveAuditRequestAsync(ulong userId, ulong groupId) =>
        db.AuditRequests
          .Where(user => user.UserId == userId)
          .Where(user => user.GroupId == groupId)
          .ExecuteDeleteAsync();

    public async Task MarkJonedAsync(ulong userId)
    {
        var request = await GetAuditRequestAsync(userId).ConfigureAwait(false);
        if (request is null)
        {
            return;
        }

        request.IsJoinedGroup = true;
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private Task<AuditRequestData?> GetAuditRequestAsync(Guid auditId) =>
        db.AuditRequests.SingleOrDefaultAsync(audit => audit.Id == auditId);

    private Task<AuditRequestData?> GetAuditRequestAsync(ulong userId) =>
        db.AuditRequests.SingleOrDefaultAsync(audit => audit.UserId == userId);

    #endregion

    #region LogMethod

    [LoggerMessage(LogLevel.Warning, "Audit request with ID {auditId} not found")]
    static partial void LogAuditNotFound(ILogger<AuditManagerService> logger, string auditId);

    [LoggerMessage(LogLevel.Information, "Processed audit request {auditId} by processor {processorId} with {state}")]
    static partial void LogProcessed(ILogger<AuditManagerService> logger, string auditId, ulong processorId,
                                     AuditStatus state);

    #endregion
}
