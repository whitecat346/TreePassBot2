using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Utils;
using TreePassBot2.Core.Entities;
using TreePassBot2.Core.Entities.Enums;
using TreePassBot2.Data;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public partial class AuditManagerService(
    BotDbContext db,
    ICommunicationService communicationService,
    UserManageService userManage,
    ILogger<AuditManagerService> logger)
{
    #region Approve/Reject Action

    public async Task<(bool, string)> ApproveAuditRequestAsync(Guid auditId, ulong processorId)
    {
        var auditRequest = await GetAuditRequestAsync(auditId).ConfigureAwait(false);
        if (auditRequest == null)
        {
            LogAuditNotFound(logger, auditId.ToString());
            return (false, "Not found.");
        }

        auditRequest.Status = AuditStatus.Approved;
        auditRequest.ProcessedAt = DateTimeOffset.UtcNow;
        auditRequest.ProcessedBy = processorId;

        var verificationCode = VerificationCodeGenerator.GenerateNumericCode(10);
        auditRequest.VerificationCode = verificationCode;

        db.AuditRequests.Update(auditRequest);
        await db.SaveChangesAsync().ConfigureAwait(false);

        await communicationService
             .AnnounceApprovedAuditActionAsync(auditRequest.UserId, auditRequest.GroupId, "您的申请已通过！", verificationCode)
             .ConfigureAwait(false);

        LogProcessed(logger, auditId.ToString(), processorId, auditRequest.Status);

        return (true, "Success.");
    }

    public async Task<(bool, string)> ApproveAuditRequestAsync(ulong userId, ulong processorId)
    {
        var auditRequest =
            await GetAuditRequestAsync(userId).ConfigureAwait(false);
        if (auditRequest == null)
        {
            LogAuditNotFound(logger, userId.ToString());
            return (false, "Not found.");
        }

        auditRequest.Status = AuditStatus.Approved;
        auditRequest.ProcessedAt = DateTimeOffset.UtcNow;
        auditRequest.ProcessedBy = processorId;

        var verificationCode = VerificationCodeGenerator.GenerateNumericCode(10);
        auditRequest.VerificationCode = verificationCode;

        db.AuditRequests.Update(auditRequest);
        await db.SaveChangesAsync().ConfigureAwait(false);

        await communicationService
             .AnnounceApprovedAuditActionAsync(auditRequest.UserId, auditRequest.GroupId, "您的申请已通过！", verificationCode)
             .ConfigureAwait(false);

        LogProcessed(logger, userId.ToString(), processorId, auditRequest.Status);

        return (true, "Success.");
    }

    public async Task<(bool, string)> DenyAuditRequestAsync(Guid userId, ulong processorId)
    {
        var auditRequest = await GetAuditRequestAsync(userId).ConfigureAwait(false);
        if (auditRequest == null)
        {
            LogAuditNotFound(logger, userId.ToString());
            return (false, "Not found.");
        }

        auditRequest.Status = AuditStatus.Rejected;
        auditRequest.ProcessedAt = DateTimeOffset.UtcNow;
        auditRequest.ProcessedBy = processorId;

        db.AuditRequests.Update(auditRequest);
        await db.SaveChangesAsync().ConfigureAwait(false);

        await communicationService
             .AnnounceRejectedAuditActionAsync(auditRequest.UserId, auditRequest.GroupId, "您的申请未通过，请重新申请。")
             .ConfigureAwait(false);

        LogProcessed(logger, userId.ToString(), processorId, auditRequest.Status);
        return (true, "Success.");
    }

    public async Task<(bool, string)> DenyAuditRequestAsync(ulong userId, ulong processorId)
    {
        var auditRequest =
            await GetAuditRequestAsync(userId).ConfigureAwait(false);
        if (auditRequest == null)
        {
            LogAuditNotFound(logger, userId.ToString());
            return (false, "Not found.");
        }

        auditRequest.Status = AuditStatus.Rejected;
        auditRequest.ProcessedAt = DateTimeOffset.UtcNow;
        auditRequest.ProcessedBy = processorId;

        db.AuditRequests.Update(auditRequest);
        await db.SaveChangesAsync().ConfigureAwait(false);

        await communicationService
             .AnnounceRejectedAuditActionAsync(auditRequest.UserId, auditRequest.GroupId, "您的申请未通过，请重新申请。")
             .ConfigureAwait(false);

        LogProcessed(logger, userId.ToString(), processorId, auditRequest.Status);
        return (true, "Success.");
    }

    #endregion

    public async Task<(bool, string)> CheckVerificationCodeAsync(ulong userId, string verificationCode)
    {
        var auditRequest = await GetAuditRequestAsync(userId).ConfigureAwait(false);
        if (auditRequest == null)
        {
            LogAuditNotFound(logger, userId.ToString());
            return (false, "You are not exist in database.");
        }

        var isRight =
            auditRequest.VerificationCode.Equals(verificationCode, StringComparison.CurrentCultureIgnoreCase) &&
            auditRequest.Status == AuditStatus.Approved;

        var offset = DateTimeOffset.UtcNow - auditRequest.ProcessedAt;

        var isExpired = offset > TimeSpan.FromMinutes(30);

        var isAllowed = isRight && !isExpired;

        string msg;
        if (isAllowed)
        {
            if (isExpired)
            {
                msg = "Verification code is expired.";
            }
            else
            {
                msg = string.Empty;
            }
        }
        else
        {
            msg = "Verification code is incorrect.";
        }

        return (isAllowed, msg);
    }

    public async Task MarkJoinedAsync(ulong userId)
    {
        var auditRequest = await GetAuditRequestAsync(userId).ConfigureAwait(false);
        if (auditRequest == null)
        {
            LogAuditNotFound(logger, userId.ToString());
            return;
        }

        auditRequest.IsJoinedGroup = true;
        db.AuditRequests.Update(auditRequest);
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    #region Add/Remove Audit

    public async Task<bool> AddAuditRequestAsync(ulong userId, ulong groupId)
    {
        var auditRequest = new AuditRequestData
        {
            UserId = userId,
            GroupId = groupId,
            GroupName = await userManage.GetGroupNameAsync(groupId).ConfigureAwait(false)
        };

        var data = await db.AuditRequests.SingleOrDefaultAsync(data => data.UserId == userId).ConfigureAwait(false);
        if (data != null)
        {
            db.AuditRequests.Remove(data);
        }

        db.AuditRequests.Add(auditRequest);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }

    public async Task<bool> RemoveAuditRequestAsync(ulong auditId)
    {
        var auditRequest = await GetAuditRequestAsync(auditId).ConfigureAwait(false);
        if (auditRequest == null)
        {
            LogAuditNotFound(logger, auditId.ToString());
            return false;
        }

        db.AuditRequests.Remove(auditRequest);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }

    #endregion

    private Task<AuditRequestData?> GetAuditRequestAsync(Guid auditId) =>
        db.AuditRequests.SingleOrDefaultAsync(data => data.Id == auditId);

    private Task<AuditRequestData?> GetAuditRequestAsync(ulong userId) =>
        db.AuditRequests.SingleOrDefaultAsync(data => data.UserId == userId);

    #region LogMethod

    [LoggerMessage(LogLevel.Warning, "Audit request with ID {auditId} not found")]
    static partial void LogAuditNotFound(ILogger<AuditManagerService> logger, string auditId);

    [LoggerMessage(LogLevel.Information, "Processed audit request {auditId} by processor {processorId} with {state}")]
    static partial void LogProcessed(ILogger<AuditManagerService> logger, string auditId, ulong processorId,
                                     AuditStatus state);

    #endregion
}
