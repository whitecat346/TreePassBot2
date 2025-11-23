namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models;

/// <summary>
/// 表情消息段
/// </summary>
public class FaceSegment : MessageSegment
{
    /// <summary>
    /// 表情ID
    /// </summary>
    public int FaceId { get; set; }

    /// <summary>
    /// 消息段类型
    /// </summary>
    public override string Type => "face";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="faceId">表情ID</param>
    public FaceSegment(int faceId)
    {
        FaceId = faceId;
    }
}