namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

/// <summary>
/// 图片消息段
/// </summary>
public class ImageSegment : MessageSegment
{
    /// <summary>
    /// 图片文件路径或URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 消息段类型
    /// </summary>
    public override string Type => "image";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="url">图片文件路径或URL</param>
    public ImageSegment(string url)
    {
        Url = url;
    }
}
