namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models;

/// <summary>
/// @消息段
/// </summary>
public class AtSegment : MessageSegment
{
    /// <summary>
    /// 用户ID，0表示@全体成员
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// 自定义文本
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// 消息段类型
    /// </summary>
    public override string Type => "at";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userId">用户ID，0表示@全体成员</param>
    /// <param name="text">自定义文本</param>
    public AtSegment(ulong userId, string? text = null)
    {
        UserId = userId;
        Text = text;
    }
}