using TreePassBot2.Core.Entities.Enums;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor;

public record MessageEventData(SenderData Sender, Message Message, long MessageId, ulong GroupId);

public record SenderData(ulong Id, string NickName, bool IsAnonymous, UserRole Role);
