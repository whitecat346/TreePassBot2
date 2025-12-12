namespace TreePassBot2.PluginSdk.Interfaces;

public interface IBotApi
{
    Task GetGroupMembersAsync();
    Task GetMemberInfoAsync(ulong memberId);
    Task WithdrawMessageAsync(long messageId);
    Task KickMemberAsync(ulong memberId);

    Task ArchiveMessageAsync(long startMessageId, string reason, TimeSpan lookBackTime);
}
