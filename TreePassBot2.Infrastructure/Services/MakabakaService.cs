using Makabaka;
using Makabaka.Exceptions;
using TreePassBot2.Infrastructure.Exceptions;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Converters;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using MessageBuilder = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageBuilder;

namespace TreePassBot2.Infrastructure.Services;

/// <summary>
/// Makabaka通信服务实现
/// </summary>
public class MakabakaService : ITreePassBotCommunicationService
{
    public IBotContext BotContext { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="botContext">Makabaka机器人上下文</param>
    public MakabakaService(Makabaka.IBotContext botContext)
    {
        BotContext = botContext ?? throw new ArgumentNullException(nameof(botContext));
    }

    /// <summary>
    /// 发送群消息
    /// </summary>
    /// <param name="groupId">群号</param>
    /// <param name="message">消息对象</param>
    /// <returns>任务</returns>
    /// <exception cref="Exception">Throws if failed to get forward message.</exception>
    /// <exception cref="FailedToExcuteApiException">Throws if failed to execute api.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/></exception>
    /// <exception cref="TimeoutException">Throws if timed out when execute api.</exception>
    public async Task SendGroupMessageAsync(ulong groupId, Message message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            // 转换消息
            var makabakaMessage = MessageConverter.ConvertToMakabakaMessage(message);

            // 发送消息
            var response = await BotContext.SendGroupMessageAsync(groupId, makabakaMessage).ConfigureAwait(false);

            // 确保请求成功
            response.EnsureSuccess();
        }
        catch (APIException ex)
        {
            throw new FailedToExcuteApiException($"发送群消息失败: {ex.Message}", ex);
        }
        catch (APITimeoutException ex)
        {
            throw new TimeoutException($"发送群消息超时: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"发送群消息时发生异常: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="message">消息对象</param>
    /// <returns>任务</returns>
    /// <exception cref="Exception">Throws if failed to get forward message.</exception>
    /// <exception cref="FailedToExcuteApiException">Throws if failed to execute api.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/></exception>
    /// <exception cref="TimeoutException">Throws if timed out when execute api.</exception>
    public async Task SendPrivateMessageAsync(ulong userId, Message message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            // 转换消息
            var makabakaMessage = MessageConverter.ConvertToMakabakaMessage(message);

            // 发送消息
            var response = await BotContext.SendPrivateMessageAsync(userId, makabakaMessage).ConfigureAwait(false);

            // 确保请求成功
            response.EnsureSuccess();
        }
        catch (APIException ex)
        {
            throw new FailedToExcuteApiException($"发送私聊消息失败: {ex.Message}", ex);
        }
        catch (APITimeoutException ex)
        {
            throw new TimeoutException($"发送私聊消息超时: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"发送私聊消息时发生异常: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 发送群消息（使用消息构建器）
    /// </summary>
    /// <param name="groupId">群号</param>
    /// <param name="messageBuilder">消息构建器</param>
    /// <returns>任务</returns>
    /// <exception cref="ArgumentNullException"><paramref name="messageBuilder"/> is <see langword="null"/></exception>
    /// <exception cref="Exception">Throws if failed to get forward message.</exception>
    /// <exception cref="FailedToExcuteApiException">Throws if failed to execute api.</exception>
    /// <exception cref="TimeoutException">Throws if timed out when execute api.</exception>
    public Task SendGroupMessageAsync(ulong groupId, MessageBuilder messageBuilder)
    {
        if (messageBuilder == null)
            throw new ArgumentNullException(nameof(messageBuilder));

        var message = messageBuilder.Build();
        return SendGroupMessageAsync(groupId, message);
    }

    /// <summary>
    /// 发送私聊消息（使用消息构建器）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="messageBuilder">消息构建器</param>
    /// <returns>任务</returns>
    /// <exception cref="ArgumentNullException"><paramref name="messageBuilder"/> is <see langword="null"/></exception>
    /// <exception cref="Exception">Throws if failed to get forward message.</exception>
    /// <exception cref="FailedToExcuteApiException">Throws if failed to execute api.</exception>
    /// <exception cref="TimeoutException">Throws if timed out when execute api.</exception>
    public Task SendPrivateMessageAsync(ulong userId, MessageBuilder messageBuilder)
    {
        if (messageBuilder == null)
            throw new ArgumentNullException(nameof(messageBuilder));

        var message = messageBuilder.Build();
        return SendPrivateMessageAsync(userId, message);
    }

    /// <summary>
    /// 获取合并转发消息内容
    /// </summary>
    /// <param name="forwardId">合并转发ID</param>
    /// <returns>合并转发的消息列表</returns>
    /// <exception cref="Exception">Throws if failed to get forward message.</exception>
    /// <exception cref="FailedToExcuteApiException">Throws if failed to execute api.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="forwardId"/> is <see langword="null"/></exception>
    /// <exception cref="TimeoutException">Throws if timed out when execute api.</exception>
    public async Task<List<Message>> GetForwardMessageAsync(string forwardId)
    {
        if (string.IsNullOrEmpty(forwardId))
            throw new ArgumentNullException(nameof(forwardId));

        try
        {
            var response = await BotContext.GetForwardMessageAsync(forwardId).ConfigureAwait(false);

            response.EnsureSuccess();

            var result = new List<Message>();
            if (response.Data?.Message is { Count: > 0 })
            {
                var message = MessageConverter.ConvertToTreePassBotMessage(response.Data.Message);
                result.AddRange(message);
            }

            return result;
        }
        catch (APIException ex)
        {
            throw new FailedToExcuteApiException($"获取合并转发消息失败: {ex.Message}", ex);
        }
        catch (APITimeoutException ex)
        {
            throw new TimeoutException($"获取合并转发消息超时: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"获取合并转发消息时发生异常: {ex.Message}", ex);
        }
    }
}