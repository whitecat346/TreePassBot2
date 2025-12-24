using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
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
    /// <summary>
    /// 尝试解析消息，提取命令触发词
    /// </summary>
    /// <param name="data">消息事件数据</param>
    /// <param name="secSeg">第二个文本段</param>
    /// <returns>是否解析成功</returns>
    private bool TryParseMessage(MessageEventData data, [NotNullWhen(true)] out TextSegment? secSeg)
    {
        secSeg = null;

        // check message segment length
        if (data.Message.Count < 2)
        {
            return false;
        }

        var isForwardMessage = data.Message[0] is ForwardSegment;
        var atIndex = isForwardMessage ? 1 : 0;
        var textIndex = isForwardMessage ? 2 : 1;

        // check ping segment
        if (data.Message[atIndex] is not AtSegment atSegment)
        {
            LogNotRightFormat(logger, "Not start with At");
            return false;
        }

        if (atSegment.UserId != options.Value.BotQqId)
        {
            LogNotRightFormat(logger, $"At target not Bot-Self with {atSegment.UserId}");
            return false;
        }

        // check text segment
        if (data.Message.Count <= textIndex || data.Message[textIndex] is not TextSegment textSeg)
        {
            LogNotRightFormat(logger, "Second part not TextSegment");
            return false;
        }

        secSeg = textSeg;
        return true;
    }

    /// <inheritdoc />
    public async Task HandleMessageAsync(MessageEventData data)
    {
        if (!TryParseMessage(data, out var secSeg))
        {
            return;
        }

        var trigger = secSeg.Text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[0].Trim([' ']);

        LogTryToMatchTrigger(logger, trigger);

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
    static partial void LogNotRightFormat(ILogger<CommandExcuter> logger, string message);

    [LoggerMessage(LogLevel.Information, "Try to match trigger: {trigger}")]
    static partial void LogTryToMatchTrigger(ILogger<CommandExcuter> logger, string trigger);
}
