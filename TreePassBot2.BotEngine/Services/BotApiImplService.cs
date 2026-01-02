
using Microsoft.Extensions.Options;
using TreePassBot2.Core.Options;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;
using TreePassBot2.PluginSdk.Interfaces;
using MessageSeg = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments.Message;

namespace TreePassBot2.BotEngine.Services;

public class BotApiImplService(
    MessageArchiveService archiveService,
    ICommunicationService communicationService,
    AuditManagerService auditManagerService,
    IOptions<BotOptions> options) : IBotApi
{
    public required ulong GroupId { get; init; }
    public required ulong UserId { get; init; }

    /// <inheritdoc />
    public BotOptions BotConfig { get; } = options.Value;

    /// <param name="memberId"></param>
    /// <inheritdoc />
    public Task<MemberInfo?> GetMemberInfoAsync(ulong memberId) =>
        communicationService.GetGroupMemberInfoAsync(GroupId, memberId);

    /// <inheritdoc />
    public Task<List<MemberInfo>?> GetGroupMemberListAsync() =>
        communicationService.GetGroupMemberListAsync(GroupId);

    /// <inheritdoc />
    public Task<List<MemberInfo>?> GetGroupMemberListAsync(ulong groupId) =>
        communicationService.GetGroupMemberListAsync(groupId);

    /// <inheritdoc />
    public Task WithdrawMessageAsync(long messageId) =>
        communicationService.WithdrawMessageAsync(messageId);

    /// <inheritdoc />
    public Task KickMemberAsync(ulong memberId, bool rejectRequest = false) =>
        communicationService.KickMemberAsync(GroupId, memberId, rejectRequest);

    /// <inheritdoc />
    public Task<List<MessageSeg>?>
        GetForwardMessageAsync(string forwardId) =>
        communicationService.GetForwardMessageAsync(forwardId);

    /// <inheritdoc />
    public Task ArchiveMessageAsync(long startMessageId, string reason, TimeSpan lookBackTime) =>
        archiveService.ArchiveUserMessageAsync(GroupId, UserId, startMessageId, reason, lookBackTime);

    /// <inheritdoc />
    public Task ApproveAuditAsync(ulong targetUserId) =>
        auditManagerService.ApproveAuditAsync(targetUserId, UserId);

    /// <inheritdoc />
    public Task RejectAuditAsync(ulong targetUserId) =>
        auditManagerService.DenyAuditAsync(targetUserId, UserId);

    /// <inheritdoc />
    public Task AddAuditRequestAsync(ulong targetUserId) =>
        auditManagerService.AddAuditRequestAsync(targetUserId, GroupId);

    /// <inheritdoc />
    public Task RemoveAuditRequestAsync(ulong targetUserId) =>
        auditManagerService.RemoveAuditRequestAsync(targetUserId, GroupId);

    /// <inheritdoc />
    public Task<IReadOnlyList<ulong>> GetAuditListAsync() =>
        auditManagerService.GetAllAuditQqIdAsync();
}
