using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor;

namespace TreePassBot2.BotEngine.Message;

public class MessageRouter
{
    private readonly List<IMessageHandler> _handlers;

    public MessageRouter(
        IServiceProvider serviceProvider,
        ILogger<MessageRouter> logger)
    {
        var am = Assembly.GetExecutingAssembly()
                         .GetTypes()
                         .Where(ty => typeof(IMessageHandler).IsAssignableFrom(ty))
                         .Select(ty => ActivatorUtilities.CreateInstance(serviceProvider, ty) as IMessageHandler)
                         .Where(ty => ty is not null);

        // there is never will be null; but roslyn analysis... fk u!
        _handlers = am.ToList()!;
    }

    public Task ExecuteMessageHandlerAsync(MessageEventData context)
    {
        var handlerTasks = _handlers.Select(handler => handler.HandleMessageAsync(context));
        return Task.WhenAll(handlerTasks);
    }
}
