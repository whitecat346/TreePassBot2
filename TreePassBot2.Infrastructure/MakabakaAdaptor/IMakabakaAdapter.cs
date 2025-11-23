namespace TreePassBot2.Infrastructure.MakabakaAdaptor;

/// <summary>
/// Makabaka适配器接口
/// </summary>
public interface IMakabakaAdapter
{
    /// <summary>
    /// 发送带格式的群消息
    /// </summary>
    /// <param name="groupId">群号</param>
    /// <param name="messageBuilder">消息构建器</param>
    /// <returns>任务</returns>
    Task SendGroupMessageAsync(ulong groupId, MessageBuilder messageBuilder);

    /// <summary>
    /// 发送带格式的私聊消息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="messageBuilder">消息构建器</param>
    /// <returns>任务</returns>
    Task SendPrivateMessageAsync(ulong userId, MessageBuilder messageBuilder);
}