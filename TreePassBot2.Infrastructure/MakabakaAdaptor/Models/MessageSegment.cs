namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models;

/// <summary>
/// 消息段基类
/// </summary>
public abstract class MessageSegment
{
    /// <summary>
    /// 消息段类型
    /// </summary>
    public abstract string Type { get; }
}