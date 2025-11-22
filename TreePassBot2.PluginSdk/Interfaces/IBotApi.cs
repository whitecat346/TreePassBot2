using TreePassBot2.PluginSdk.Entities;

namespace TreePassBot2.PluginSdk.Interfaces;

public interface IBotApi
{
    Task SendGroupMessageAsync(ulong groupId, string message);
    Task SendProvateMessageAsync(ulong userId, string message);
    Task<MemberInfoDto?> GetGroupMemberAsync(ulong groupId, ulong userId);
}