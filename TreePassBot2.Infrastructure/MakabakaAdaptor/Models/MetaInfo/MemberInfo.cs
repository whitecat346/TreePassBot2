using TreePassBot2.Core.Entities.Enums;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

public record MemberInfo(
    ulong QqId,
    string UserName,
    string NickName,
    UserRole Role,
    DateTimeOffset JoinedAt
);
