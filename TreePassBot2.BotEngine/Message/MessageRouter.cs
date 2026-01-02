using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

namespace TreePassBot2.BotEngine.Message;

public partial class MessageRouter(
    IServiceProvider serviceProvider,
    UserManageService userManage,
    ILogger<MessageRouter> logger)
{
    private async Task<bool> EnsureMetadataAsync(ulong groupId, ulong userId)
    {
        try
        {
            _ = await userManage.GetGroupInfoAsync(groupId).ConfigureAwait(false);
            _ = await userManage.GetMemberInfoAsync(groupId, userId).ConfigureAwait(false);
        }
        catch (InvalidDataException)
        {
            return false;
        }

        return true;
    }

    public async Task ExecuteMessageHandlerAsync(MessageEventData context)
    {
        var success = await EnsureMetadataAsync(context.GroupId, context.Sender.Id).ConfigureAwait(false);
        if (!success)
        {
            return;
        }

        using var scope = serviceProvider.CreateScope();

        var handlers = scope.ServiceProvider.GetServices<IMessageHandler>();

        LogTryToHandleMessage(logger, context.MessageId);

        var tasks = handlers.Select(h => h.HandleMessageAsync(context));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    [LoggerMessage(LogLevel.Trace, "Try to handle message: {messageId}")]
    static partial void LogTryToHandleMessage(ILogger<MessageRouter> logger, long messageId);
}
