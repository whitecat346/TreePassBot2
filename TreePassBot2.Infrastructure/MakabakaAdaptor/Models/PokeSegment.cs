namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Models;

/// <summary>
/// 戳一戳消息段
/// </summary>
public class PokeSegment : MessageSegment
{
    /// <summary>
    /// 被戳用户ID
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// 戳一戳类型
    /// </summary>
    public string PokeType { get; set; }

    /// <summary>
    /// 戳一戳ID
    /// </summary>
    public string PokeId { get; set; }

    /// <summary>
    /// 消息段类型
    /// </summary>
    public override string Type => "poke";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userId">被戳用户ID</param>
    /// <param name="pokeType">戳一戳类型，默认为"poke"</param>
    /// <param name="pokeId">戳一戳ID，默认为"1"</param>
    public PokeSegment(ulong userId, string pokeType = "poke", string pokeId = "1")
    {
        UserId = userId;
        PokeType = pokeType;
        PokeId = pokeId;
    }
}