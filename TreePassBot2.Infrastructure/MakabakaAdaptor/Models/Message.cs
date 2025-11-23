namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models;

/// <summary>
/// 消息类，包含多个消息段
/// </summary>
public class Message : List<MessageSegment>
{
    /// <summary>
    /// 将消息转换为字符串表示
    /// </summary>
    /// <returns>消息的字符串表示</returns>
    public override string ToString()
    {
        return string.Join("", this.Select(segment =>
        {
            if (segment is TextSegment textSegment)
                return textSegment.Text;
            return $"<{segment.Type}>";
        }));
    }
}