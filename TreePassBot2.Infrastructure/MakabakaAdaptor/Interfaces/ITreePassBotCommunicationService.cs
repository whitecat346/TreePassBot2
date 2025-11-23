using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

/// <summary>
/// TreePassBot通信服务接口
/// </summary>
public interface ITreePassBotCommunicationService
{
    /// <summary>
    /// 发送群消息
    /// </summary>
    /// <param name="groupId">群号</param>
    /// <param name="message">消息对象</param>
    /// <returns>任务</returns>
    Task SendGroupMessageAsync(ulong groupId, Message message);

    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="message">消息对象</param>
    /// <returns>任务</returns>
    Task SendPrivateMessageAsync(ulong userId, Message message);

    /// <summary>
    /// 发送群消息（使用消息构建器）
    /// </summary>
    /// <param name="groupId">群号</param>
    /// <param name="messageBuilder">消息构建器</param>
    /// <returns>任务</returns>
    Task SendGroupMessageAsync(ulong groupId, MessageBuilder messageBuilder);

    /// <summary>
    /// 发送私聊消息（使用消息构建器）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="messageBuilder">消息构建器</param>
    /// <returns>任务</returns>
    Task SendPrivateMessageAsync(ulong userId, MessageBuilder messageBuilder);

    /// <summary>
    /// 获取合并转发消息内容
    /// </summary>
    /// <param name="forwardId">合并转发ID</param>
    /// <returns>合并转发的消息列表</returns>
    Task<List<Message>> GetForwardMessageAsync(string forwardId);
}