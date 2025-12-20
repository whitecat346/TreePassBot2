
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public class BotApiImpl(MessageArchiveService archiveService, ICommunicationService communicationService) : IBotApi
{
    public required ulong GroupId { get; init; }
    public required ulong UserId { get; init; }

    /// <param name="memberId"></param>
    /// <inheritdoc />
    public Task<MemberInfo?> GetMemberInfoAsync(ulong memberId) =>
        communicationService.GetGroupMemberInfoAsync(GroupId, memberId);

    /// <inheritdoc />
    public Task<List<MemberInfo>?> GetGroupMemberListAsync() =>
        communicationService.GetGroupMemberListAsync(GroupId);

    /// <inheritdoc />
    public Task WithdrawMessageAsync(long messageId) =>
        communicationService.WithdrawMessageAsync(messageId);

    /// <inheritdoc />
    public Task KickMemberAsync(ulong memberId, bool rejectRequest = false) =>
        communicationService.KickMemberAsync(GroupId, memberId, rejectRequest);

    /// <inheritdoc />
    public Task<List<Infrastructure.MakabakaAdaptor.Models.MessageSegments.Message>?>
        GetForwardMessageAsync(string forwardId) =>
        communicationService.GetForwardMessageAsync(forwardId);

    /// <inheritdoc />
    public Task ArchiveMessageAsync(long startMessageId, string reason, TimeSpan lookBackTime) =>
        archiveService.ArchiveUserMessageAsync(GroupId, UserId, startMessageId, reason, lookBackTime);
}
