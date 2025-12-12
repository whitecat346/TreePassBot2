using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models;

/// <summary>
/// 消息构建器，用于构建复杂的消息内容
/// </summary>
public class MessageBuilder
{
    private readonly List<MessageSegment> _segments = [];

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
        _segments.Add(new FaceSegment(faceId));
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
    /// <param name="userId">被戳用户ID</param>
    /// <returns>消息构建器实例</returns>
    public MessageBuilder AddPoke(ulong userId)
    {
        _segments.Add(new PokeSegment(userId));
        return this;
    }

    /// <summary>
    /// 添加合并转发消息段
    /// </summary>
    /// <param name="forwardId">合并转发ID</param>
    /// <returns>消息构建器</returns>
    public MessageBuilder AddForward(string forwardId)
    {
        _segments.Add(new ForwardSegment(forwardId));
        return this;
    }

    /// <summary>
    /// 添加视频消息段
    /// </summary>
    /// <param name="file">视频文件路径</param>
    /// <returns>消息构建器</returns>
    public MessageBuilder AddVideo(string file)
    {
        _segments.Add(new VideoSegment(file));
        return this;
    }

    /// <summary>
    /// 添加音频消息段
    /// </summary>
    /// <param name="file">音频文件路径</param>
    /// <returns>消息构建器</returns>
    public MessageBuilder AddAudio(string file)
    {
        _segments.Add(new AudioSegment(file));
        return this;
    }

    /// <summary>
    /// 构建消息对象
    /// </summary>
    /// <returns>消息对象</returns>
    public Message Build()
    {
        var message = new Message();
        message.AddRange(_segments);
        return message;
    }
}
