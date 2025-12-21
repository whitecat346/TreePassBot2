using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Core.Options;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

namespace TreePassBot2.BotEngine.Message.Routers.Message;

public partial class CommandExcuter(
    PluginManagerService pluginManagerService,
    CommandContextImplFactory factory,
    ICommunicationService communicationService,
    ILogger<CommandExcuter> logger,
    IOptions<BotOptions> options) : IMessageHandler
{
    /// <inheritdoc />
    public async Task HandleMessageAsync(MessageEventData data)
    {
        if (data.Message.Count < 2)
        {
            return;
        }

        if (data.Message[0] is AtSegment atSegment)
        {
            if (atSegment.UserId != options.Value.BotQqId)
            {
                LogIgnoringMessageBecauseNotRightFormat(logger, $"At target not Bot-Self with {atSegment.UserId}");
                logger.LogInformation("Right format: {Qq}", options.Value.BotQqId);
                return;
            }
        }
        else
        {
            LogIgnoringMessageBecauseNotRightFormat(logger, "Not start with At");
            return;
        }

        if (data.Message[1] is not TextSegment secSeg)
        {
            LogIgnoringMessageBecauseNotRightFormat(logger, "Second part not TextSegment");
            return;
        }

        var trigger = secSeg.Text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[0].Trim([' ']);

        LogTryToMatchTriggerTrigger(logger, trigger);

        if (!await pluginManagerService.TryGetPluginIdAsync(trigger, out var id).ConfigureAwait(false))
        {
            var replyMsg = new MessageBuilder().AddAt(data.Sender.Id).AddText("Command not found");

            await communicationService.SendGroupMessageAsync(data.GroupId, replyMsg).ConfigureAwait(false);
            return;
        }

        var context = factory.Create(id!, data);

        LogDispatchTracer(logger, trigger);

        await pluginManagerService.DispatchCommandAsync(trigger, context).ConfigureAwait(false);
    }

    [LoggerMessage(LogLevel.Trace, "Try to dispatch message to command dispatcher: {trigger}")]
    static partial void LogDispatchTracer(ILogger<CommandExcuter> logger, string trigger);

    [LoggerMessage(LogLevel.Information, "Ignoring message because not right format: {message}")]
    static partial void LogIgnoringMessageBecauseNotRightFormat(ILogger<CommandExcuter> logger, string message);

    [LoggerMessage(LogLevel.Information, "Try to match trigger: {Trigger}")]
    static partial void LogTryToMatchTriggerTrigger(ILogger<CommandExcuter> logger, string Trigger);
}
