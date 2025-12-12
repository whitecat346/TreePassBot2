using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public class BotApiImpl(MessageArchiveService archiveService, ICommunicationService communicationService) : IBotApi
{
    public required ulong GroupId { get; init; }
    public required ulong UserId { get; init; }

    /// <inheritdoc />
    public Task GetGroupMembersAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task GetMemberInfoAsync(ulong memberId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task WithdrawMessageAsync(long messageId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task KickMemberAsync(ulong memberId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task ArchiveMessageAsync(long startMessageId, string reason, TimeSpan lookBackTime)
    {
        archiveService.ArchiveUserMessageAsync(GroupId, UserId, startMessageId, reason, lookBackTime);
    }
}
