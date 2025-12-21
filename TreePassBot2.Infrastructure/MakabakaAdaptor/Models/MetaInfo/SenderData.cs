using TreePassBot2.Core.Entities.Enums;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

public record SenderData(ulong Id, string NickName, bool IsAnonymous, UserRole Role);
