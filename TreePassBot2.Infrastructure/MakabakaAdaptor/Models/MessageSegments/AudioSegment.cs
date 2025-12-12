namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

/// <summary>
/// 音频消息段
/// </summary>
public class AudioSegment : MessageSegment
{
    /// <summary>
    /// 消息段类型
    /// </summary>
    public override string Type => "audio";

    /// <summary>
    /// 音频文件路径
    /// </summary>
    public string File { get; set; }

    /// <summary>
    /// 初始化音频消息段
    /// </summary>
    /// <param name="file">音频文件路径</param>
    public AudioSegment(string file)
    {
        File = file;
    }
}