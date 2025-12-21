using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

namespace TreePassBot2.BotEngine.Message;

public partial class MessageRouter(
    IServiceProvider serviceProvider,
    ILogger<MessageRouter> logger)
{
    private readonly ILogger<MessageRouter> _logger = logger;

    public async Task ExecuteMessageHandlerAsync(MessageEventData context)
    {
        using var scope = serviceProvider.CreateScope();

        var handlers = scope.ServiceProvider.GetServices<IMessageHandler>();

        LogTryToHandleMessage(context.MessageId);

        var tasks = handlers.Select(h => h.HandleMessageAsync(context));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    [LoggerMessage(LogLevel.Trace, "Try to handle message: {messageId}")]
    partial void LogTryToHandleMessage(long messageId);
}
