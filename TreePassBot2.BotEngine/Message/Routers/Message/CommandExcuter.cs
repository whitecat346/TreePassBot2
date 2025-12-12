using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Infrastructure.MakabakaAdaptor;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

namespace TreePassBot2.BotEngine.Message.Routers.Message;

public class CommandExcuter(
    PluginManagerService pluginManagerService,
    CommandContextImplFactory factory,
    ILogger<CommandExcuter> logger) : IMessageHandler
{
    private readonly PluginManagerService _pluginManagerService = pluginManagerService;
    private readonly ILogger<CommandExcuter> _logger = logger;

    /// <inheritdoc />
    public Task HandleMessageAsync(MessageEventData data)
    {
        if (data.Message[0] is not TextSegment firSeg)
        {
            return Task.CompletedTask;
        }

        var trigger = firSeg.Text.Trim([' ']);

        var context = factory.Create(data.Sender.NickName, data.Sender.Id, data.GroupId, data.MessageId, data.Message);

        return _pluginManagerService.DispatchCommandAsync(trigger, context);
    }
}
