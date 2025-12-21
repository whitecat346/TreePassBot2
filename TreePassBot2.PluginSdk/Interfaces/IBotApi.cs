using TreePassBot2.Core.Options;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;
using MemberInfo = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo.MemberInfo;

namespace TreePassBot2.PluginSdk.Interfaces;

public interface IBotApi
{
    BotOptions BotConfig { get; }

    Task<MemberInfo?> GetMemberInfoAsync(ulong memberId);
    Task<List<MemberInfo>?> GetGroupMemberListAsync();
    Task WithdrawMessageAsync(long messageId);
    Task KickMemberAsync(ulong memberId, bool rejectRequest = false);
    Task<List<Message>?> GetForwardMessageAsync(string forwardId);
    Task ArchiveMessageAsync(long startMessageId, string reason, TimeSpan lookBackTime);

    Task ApproveAuditAsync(ulong targetUserId);
    Task RejectAuditAsync(ulong targetUserId);
    Task AddAuditRequestAsync(ulong targetUserId);
    Task RemoveAuditRequestAsync(ulong targetUserId);
}
