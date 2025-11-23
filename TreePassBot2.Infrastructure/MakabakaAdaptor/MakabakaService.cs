using Makabaka.Exceptions;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Converters;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor;

/// <summary>
/// Makabaka通信服务实现
/// </summary>
public class MakabakaService : ITreePassBotCommunicationService
{
    private readonly Makabaka.IBotContext _botContext;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="botContext">Makabaka机器人上下文</param>
    public MakabakaService(Makabaka.IBotContext botContext)
    {
        _botContext = botContext ?? throw new ArgumentNullException(nameof(botContext));
    }

    /// <summary>
    /// 发送群消息
    /// </summary>
    /// <param name="groupId">群号</param>
    /// <param name="message">消息对象</param>
    /// <returns>任务</returns>
    public async Task SendGroupMessageAsync(ulong groupId, Message message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            // 转换消息
            var makabakaMessage = MessageConverter.ConvertToMakabakaMessage(message);

            // 发送消息
            var response = await _botContext.SendGroupMessageAsync(groupId, makabakaMessage).ConfigureAwait(false);

            // 确保请求成功
            response.EnsureSuccess();
        }
        catch (APIException ex)
        {
            throw new Exception($"发送群消息失败: {ex.Message}", ex);
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
    public async Task SendPrivateMessageAsync(ulong userId, Message message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            // 转换消息
            var makabakaMessage = MessageConverter.ConvertToMakabakaMessage(message);

            // 发送消息
            var response = await _botContext.SendPrivateMessageAsync(userId, makabakaMessage).ConfigureAwait(false);

            // 确保请求成功
            response.EnsureSuccess();
        }
        catch (APIException ex)
        {
            throw new Exception($"发送私聊消息失败: {ex.Message}", ex);
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
    public Task SendGroupMessageAsync(ulong groupId, Models.MessageBuilder messageBuilder)
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
    public Task SendPrivateMessageAsync(ulong userId, Models.MessageBuilder messageBuilder)
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
    public async Task<List<Message>> GetForwardMessageAsync(string forwardId)
    {
        if (string.IsNullOrEmpty(forwardId))
            throw new ArgumentNullException(nameof(forwardId));

        try
        {
            // 获取合并转发消息内容
            var response = await _botContext.GetForwardMessageAsync(forwardId).ConfigureAwait(false);

            // 确保请求成功
            response.EnsureSuccess();

            // 转换消息列表
            var result = new List<Message>();
            if (response.Data?.Message is { Count: > 0 })
            {
                // 处理合并转发消息（这里简化处理，直接将整个消息作为列表中的一项）
                var message = MessageConverter.ConvertToTreePassBotMessage(response.Data.Message);
                result.Add(message);
            }

            return result;
        }
        catch (APIException ex)
        {
            throw new Exception($"获取合并转发消息失败: {ex.Message}", ex);
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