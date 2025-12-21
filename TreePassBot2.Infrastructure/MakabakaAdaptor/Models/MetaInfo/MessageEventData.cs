using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

public record MessageEventData(SenderData Sender, Message Message, long MessageId, ulong GroupId);
