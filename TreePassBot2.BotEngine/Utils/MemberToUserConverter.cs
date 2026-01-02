using TreePassBot2.Core.Entities;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

namespace TreePassBot2.BotEngine.Utils;

public static class MemberToUserConverter
{
    public static QqUserInfo ConverToQqUserInfo(MemberInfo info, ulong groupId)
        => new()
        {
            QqId = info.QqId,
            GroupId = groupId,
            UserName = info.UserName,
            NickName = info.NickName,
            Role = info.Role,
            JoinedAt = info.JoinedAt.UtcDateTime
        };
}
