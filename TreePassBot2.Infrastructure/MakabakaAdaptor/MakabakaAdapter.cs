using Makabaka;
using Makabaka.Exceptions;
using Makabaka.Messages;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor;

/// <summary>
/// Makabaka适配器实现类
/// </summary>
public class MakabakaAdapter : IMakabakaAdapter
{
    private readonly IBotContext _botContext;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="botContext">机器人上下文</param>
    public MakabakaAdapter(IBotContext botContext)
    {
        _botContext = botContext ?? throw new ArgumentNullException(nameof(botContext));
    }

    /// <summary>
    /// 发送带格式的群消息
    /// </summary>
    /// <param name="groupId">群号</param>
    /// <param name="messageBuilder">消息构建器</param>
    /// <returns>任务</returns>
    public Task SendGroupMessageAsync(ulong groupId, MessageBuilder messageBuilder)
    {
        if (messageBuilder == null)
            throw new ArgumentNullException(nameof(messageBuilder));

        var message = messageBuilder.ToMessage();
        return SendGroupMessageInternalAsync(groupId, message);
    }

    /// <summary>
    /// 发送带格式的私聊消息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="messageBuilder">消息构建器</param>
    /// <returns>任务</returns>
    public Task SendPrivateMessageAsync(ulong userId, MessageBuilder messageBuilder)
    {
        if (messageBuilder == null)
            throw new ArgumentNullException(nameof(messageBuilder));

        var message = messageBuilder.ToMessage();
        return SendPrivateMessageInternalAsync(userId, message);
    }

    /// <summary>
    /// 内部方法：发送群消息
    /// </summary>
    private async Task SendGroupMessageInternalAsync(ulong groupId, Message message)
    {
        try
        {
            var response = await _botContext.SendGroupMessageAsync(groupId, message).ConfigureAwait(false);
            // 使用EnsureSuccess方法确保请求成功
            response.EnsureSuccess();
        }
        catch (APIException ex)
        {
            // API异常处理
            throw new Exception($"发送群消息失败: {ex.Message}", ex);
        }
        catch (APITimeoutException ex)
        {
            // API超时异常处理
            throw new TimeoutException($"发送群消息超时: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            // 其他异常处理
            throw new Exception($"发送群消息时发生异常: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 内部方法：发送私聊消息
    /// </summary>
    private async Task SendPrivateMessageInternalAsync(ulong userId, Message message)
    {
        try
        {
            var response = await _botContext.SendPrivateMessageAsync(userId, message).ConfigureAwait(false);
            // 使用EnsureSuccess方法确保请求成功
            response.EnsureSuccess();
        }
        catch (APIException ex)
        {
            // API异常处理
            throw new Exception($"发送私聊消息失败: {ex.Message}", ex);
        }
        catch (APITimeoutException ex)
        {
            // API超时异常处理
            throw new TimeoutException($"发送私聊消息超时: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            // 其他异常处理
            throw new Exception($"发送私聊消息时发生异常: {ex.Message}", ex);
        }
    }
}