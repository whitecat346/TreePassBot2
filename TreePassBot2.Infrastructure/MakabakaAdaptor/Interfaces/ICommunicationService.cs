using Makabaka;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

/// <summary>
/// TreePassBot通信服务接口
/// </summary>
public interface ICommunicationService
{
    IBotContext BotContext { get; }

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
    Task<List<Message>?> GetForwardMessageAsync(string forwardId);

    /// <summary>
    /// Get group member info.
    /// </summary>
    /// <param name="groupId">Target group id.</param>
    /// <param name="userId">Target user id.</param>
    /// <returns>User info. Will return <see langword="null"/> if not found.</returns>
    Task<MemberInfo?> GetGroupMemberInfoAsync(ulong groupId, ulong userId);

    /// <summary>
    /// Get group member info list.
    /// </summary>
    /// <param name="groupId">Target group id.</param>
    /// <returns>User info list. Will return <see langword="null"/> if not found.</returns>
    Task<IEnumerable<MemberInfo>?> GetGroupMemberListAsync(ulong groupId);

    /// <summary>
    /// Kick a member from group.
    /// </summary>
    /// <param name="groupId">Target group id.</param>
    /// <param name="userId">Target user id.</param>
    /// <param name="rejectRequest"></param>
    Task KickMemberAsync(ulong groupId, ulong userId, bool rejectRequest = false);

    /// <summary>
    /// Withdraw a message in group.
    /// </summary>
    /// <param name="messageId">Target message id.</param>
    Task WithdrawMessageAsync(long messageId);

    /// <summary>
    /// Connect to Websocket Server.
    /// </summary>
    Task ConnectAsync();

    /// <summary>
    /// Disconnect from Websocket Server.
    /// </summary>
    Task DisconnectAsync();
}
