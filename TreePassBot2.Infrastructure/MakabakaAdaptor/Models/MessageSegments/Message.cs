namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

/// <summary>
/// 消息类，包含多个消息段
/// </summary>
public class Message : List<MessageSegment>
{
    /// <summary>
    /// 将消息转换为字符串表示
    /// </summary>
    /// <returns>消息的字符串表示</returns>
    public override string ToString() =>
        string.Join(" ", this.Select(segment => segment switch
        {
            TextSegment textSegment => $"<text>{textSegment.Text}</text>",
            AtSegment atSegment => $"@<{atSegment.UserId}>",
            ForwardSegment forwardSegment => $"<forward>{forwardSegment.ForwardId}</forward>",
            ReplySegment replySegment => $"<reply>{replySegment.MessageId}<reply/>",
            ImageSegment imageSegment => $"<image>{imageSegment.File}</image>",
            VideoSegment videoSegment => $"<video>{videoSegment.File}</video>",
            AudioSegment audioSegment => $"<audio>{audioSegment.File}</audio>",
            FaceSegment faceSegment => $"<face>{faceSegment.FaceId}</face>",
            PokeSegment pokeSegment => $"<poke>{pokeSegment.PokeType}</poke>",
            _ => string.Empty
        }));
}
