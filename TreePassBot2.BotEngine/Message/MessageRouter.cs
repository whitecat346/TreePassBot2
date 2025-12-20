using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor;

namespace TreePassBot2.BotEngine.Message;

public partial class MessageRouter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageRouter> _logger;

    public MessageRouter(
        IServiceProvider serviceProvider,
        ILogger<MessageRouter> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ExecuteMessageHandlerAsync(MessageEventData context)
    {
        using var scope = _serviceProvider.CreateScope();

        var handlers = scope.ServiceProvider.GetServices<IMessageHandler>();

        var _ = handlers.Select(h => h.HandleMessageAsync(context));
        //await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
