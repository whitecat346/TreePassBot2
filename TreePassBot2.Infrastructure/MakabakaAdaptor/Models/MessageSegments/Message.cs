namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

/// <summary>
/// 消息类，包含多个消息段
/// </summary>
public class Message : List<MessageSegment>
{
    public Message()
    {
    }

    public Message(IEnumerable<MessageSegment> segments) : base(segments)
    {
    }

    /// <summary>
    /// 将消息转换为字符串表示
    /// </summary>
    /// <returns>消息的字符串表示</returns>
    public override string ToString() =>
        string.Join(" ", this.Select(segment => segment switch
        {
            TextSegment textSegment => $"{textSegment.Text}",
            AtSegment atSegment => $"@<{atSegment.UserId}>",
            ForwardSegment forwardSegment => $"{forwardSegment.ForwardId}",
            ReplySegment replySegment => $"{replySegment.MessageId}",
            ImageSegment imageSegment => $"{imageSegment.Url}",
            VideoSegment videoSegment => $"{videoSegment.Url}",
            AudioSegment audioSegment => $"{audioSegment.File}",
            FaceSegment faceSegment => $"{faceSegment.FaceId}",
            PokeSegment pokeSegment => $"{pokeSegment.PokeType}",
            _ => string.Empty
        }));
}
