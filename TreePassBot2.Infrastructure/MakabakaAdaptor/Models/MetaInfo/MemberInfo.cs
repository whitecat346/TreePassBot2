using TreePassBot2.Core.Entities.Enums;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

public record MemberInfo(
    ulong GroupId,
    ulong UserId,
    string NickName,
    UserRole Role
);
