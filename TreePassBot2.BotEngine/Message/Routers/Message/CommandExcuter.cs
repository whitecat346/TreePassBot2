using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Infrastructure.MakabakaAdaptor;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;

namespace TreePassBot2.BotEngine.Message.Routers.Message;

public class CommandExcuter(
    PluginManager pluginManager,
    CommandContextImplFactory factory,
    ILogger<CommandExcuter> logger) : IMessageHandler
{
    private readonly PluginManager _pluginManager = pluginManager;
    private readonly ILogger<CommandExcuter> _logger = logger;

    /// <inheritdoc />
    public Task HandleMessageAsync(MessageEventData data)
    {
        var firSeg = data.Message[0] as TextSegment;

        if (firSeg == null)
        {
            return Task.CompletedTask;
        }

        var trigger = firSeg.Text.Trim([' ']);

        var context = factory.Create(data.Sender.NickName, data.Sender.Id, data.GroupId, data.MessageId, data.Message);

        return _pluginManager.DispatchCommandAsync(trigger, context);
    }
}
