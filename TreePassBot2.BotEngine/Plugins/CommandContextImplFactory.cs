using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Core.Options;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Plugins;

public class CommandContextImplFactory(IServiceProvider serviceProvider)
{
    /// <summary>
    /// 解析消息参数
    /// </summary>
    /// <param name="eventData">消息事件数据</param>
    /// <returns>解析后的参数数组</returns>
    private static string[] ParseArgs(MessageEventData eventData)
    {
        var isForwardMessage = eventData.Message[0] is ForwardSegment;
        var skipCount = isForwardMessage ? 3 : 2;

        return
        [
            .. eventData.Message.ToString()
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Skip(skipCount)
        ];
    }

    /// <summary>
    /// 获取引用消息ID
    /// </summary>
    /// <param name="eventData">消息事件数据</param>
    /// <returns>引用消息ID，不存在则返回0</returns>
    private static long GetReferId(MessageEventData eventData)
    {
        if (eventData.Message[0] is ForwardSegment forwardSegment &&
            !string.IsNullOrEmpty(forwardSegment.ForwardId))
        {
            return long.TryParse(forwardSegment.ForwardId, out var refId) ? refId : 0;
        }

        return 0;
    }

    /// <summary>
    /// Activate <see cref="CommandContextImpl"/>.
    /// </summary>
    public ICommandContext Create(
        string pluginId,
        MessageEventData eventData)
    {
        var communicationService = serviceProvider.GetRequiredService<ICommunicationService>();
        var stateStorage = new PluginStateStorageImpl(pluginId, serviceProvider);

        using var scope = serviceProvider.CreateScope();

        var archiveManager = serviceProvider.GetRequiredService<MessageArchiveService>();
        var auditManager = scope.ServiceProvider.GetRequiredService<AuditManagerService>();
        var botOptions = serviceProvider.GetRequiredService<IOptions<BotOptions>>();
        var botApi = new BotApiImplService(archiveManager, communicationService, auditManager, botOptions)
        {
            GroupId = eventData.GroupId,
            UserId = eventData.Sender.Id,
        };
        var logger = serviceProvider.GetRequiredService<ILogger<ICommandContext>>();

        var args = ParseArgs(eventData);
        var referId = GetReferId(eventData);

        return new CommandContextImpl(communicationService)
        {
            SenderName = eventData.Sender.NickName,
            SenderId = eventData.Sender.Id,
            GroupId = eventData.GroupId,
            MessageId = eventData.MessageId,
            RawMessage = eventData.Message,
            ReferMessage = referId,
            Args = args,
            State = stateStorage,
            BotApi = botApi,
            Logger = logger
        };
    }
}
