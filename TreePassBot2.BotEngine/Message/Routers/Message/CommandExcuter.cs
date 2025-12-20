using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Infrastructure.MakabakaAdaptor;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

namespace TreePassBot2.BotEngine.Message.Routers.Message;

public partial class CommandExcuter(
    PluginManagerService pluginManagerService,
    CommandContextImplFactory factory,
    ILogger<CommandExcuter> logger) : IMessageHandler
{
    /// <inheritdoc />
    public Task HandleMessageAsync(MessageEventData data)
    {
        if (data.Message.Count < 2)
        {
            return Task.CompletedTask;
        }

        if (data.Message[0] is not AtSegment)
        {
            return Task.CompletedTask;
        }

        if (data.Message[1] is not TextSegment secSeg)
        {
            return Task.CompletedTask;
        }

        var trigger = secSeg.Text.Trim([' ']);

        var context = factory.Create(data.Sender.NickName, data.Sender.Id, data.GroupId, data.MessageId, data.Message);

        LogDispatchTracer(logger, trigger);

        return pluginManagerService.DispatchCommandAsync(trigger, context);
    }

    [LoggerMessage(LogLevel.Trace, "Try to dispatch message to command dispatcher: {trigger}")]
    static partial void LogDispatchTracer(ILogger<CommandExcuter> logger, string trigger);
}
