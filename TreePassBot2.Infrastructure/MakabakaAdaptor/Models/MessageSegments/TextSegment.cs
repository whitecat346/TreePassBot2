namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

/// <summary>
/// 文本消息段
/// </summary>
public class TextSegment : MessageSegment
{
    /// <summary>
    /// 文本内容
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 消息段类型
    /// </summary>
    public override string Type => "text";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="text">文本内容</param>
    public TextSegment(string text)
    {
        Text = text;
    }
}