using Makabaka.Messages;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor;

/// <summary>
/// 消息构建器，用于构建复杂的消息内容
/// </summary>
public class MessageBuilder
{
    private readonly List<Segment> _segments = new();

    /// <summary>
    /// 添加文本消息
    /// </summary>
    /// <param name="text">文本内容</param>
    /// <returns>消息构建器实例</returns>
    public MessageBuilder AddText(string text)
    {
        _segments.Add(new TextSegment(text));
        return this;
    }

    /// <summary>
    /// 添加@消息
    /// </summary>
    /// <param name="userId">用户ID，0表示@全体成员</param>
    /// <param name="text">自定义文本</param>
    /// <returns>消息构建器实例</returns>
    public MessageBuilder AddAt(ulong userId, string? text = null)
    {
        _segments.Add(new AtSegment(userId, text));
        return this;
    }

    /// <summary>
    /// 添加表情消息
    /// </summary>
    /// <param name="faceId">表情ID</param>
    /// <returns>消息构建器实例</returns>
    public MessageBuilder AddFace(int faceId)
    {
        _segments.Add(new FaceSegment(faceId.ToString()));
        return this;
    }

    /// <summary>
    /// 添加图片消息
    /// </summary>
    /// <param name="file">图片文件路径或URL</param>
    /// <returns>消息构建器实例</returns>
    public MessageBuilder AddImage(string file)
    {
        _segments.Add(new ImageSegment(file));
        return this;
    }

    /// <summary>
    /// 添加回复消息
    /// </summary>
    /// <param name="messageId">要回复的消息ID</param>
    /// <returns>消息构建器实例</returns>
    public MessageBuilder AddReply(long messageId)
    {
        _segments.Add(new ReplySegment(messageId));
        return this;
    }

    /// <summary>
    /// 添加戳一戳消息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>消息构建器实例</returns>
    public MessageBuilder AddPoke(ulong userId)
    {
        // 使用mirai标准的戳一戳类型和ID
        // 对于普通戳一戳，type为"poke"，id为"1"
        _segments.Add(new PokeSegment("poke", "1"));
        return this;
    }

    /// <summary>
    /// 转换为Makabaka的Message对象
    /// </summary>
    /// <returns>Message对象</returns>
    public Message ToMessage()
    {
        var message = new Message();
        message.AddRange(_segments);
        return message;
    }
}