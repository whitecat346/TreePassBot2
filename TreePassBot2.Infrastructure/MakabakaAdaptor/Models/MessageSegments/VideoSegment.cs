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
    public string File { get; set; }

    /// <summary>
    /// 初始化视频消息段
    /// </summary>
    /// <param name="file">视频文件路径</param>
    public VideoSegment(string file)
    {
        File = file;
    }
}