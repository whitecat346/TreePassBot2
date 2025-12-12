namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

/// <summary>
/// 回复消息段
/// </summary>
public class ReplySegment : MessageSegment
{
    /// <summary>
    /// 要回复的消息ID
    /// </summary>
    public long MessageId { get; }

    /// <summary>
    /// 消息段类型
    /// </summary>
    public override string Type => "reply";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="messageId">要回复的消息ID</param>
    public ReplySegment(long messageId)
    {
        MessageId = messageId;
    }
}