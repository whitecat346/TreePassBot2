namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models;

/// <summary>
/// 合并转发消息段
/// </summary>
public class ForwardSegment : MessageSegment
{
    /// <summary>
    /// 合并转发ID
    /// </summary>
    public string ForwardId { get; set; }

    /// <summary>
    /// 消息段类型
    /// </summary>
    public override string Type => "forward";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="forwardId">合并转发ID</param>
    public ForwardSegment(string forwardId)
    {
        ForwardId = forwardId;
    }
}