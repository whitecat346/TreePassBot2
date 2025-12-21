namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

/// <summary>
/// 视频消息段
/// </summary>
public class VideoSegment : MessageSegment
{
    /// <summary>
    /// 消息段类型
    /// </summary>
    public override string Type => "video";

    /// <summary>
    /// 视频文件路径
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 初始化视频消息段
    /// </summary>
    /// <param name="url">视频文件路径</param>
    public VideoSegment(string url)
    {
        Url = url;
    }
}