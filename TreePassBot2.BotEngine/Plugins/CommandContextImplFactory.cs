using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Core.Options;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Plugins;

public class CommandContextImplFactory(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Activate <see cref="CommandContextImpl"/>.
    /// </summary>
    public ICommandContext Create(
        string pluginId,
        MessageEventData eventData)
    {
        var communicationService = serviceProvider.GetRequiredService<ICommunicationService>();
        var stateStorage = new PluginStateStorageImpl(pluginId, serviceProvider);

        // ctor botapi
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

        var args = eventData.Message.ToString()
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Skip(2)
                            .ToArray();

        var instance = new CommandContextImpl(communicationService)
        {
            SenderName = eventData.Sender.NickName,
            SenderId = eventData.Sender.Id,
            GroupId = eventData.GroupId,
            MessageId = eventData.MessageId,
            RawMessage = eventData.Message,
            Args = args,
            State = stateStorage,
            BotApi = botApi,
            Logger = logger
        };

        return instance;
    }
}
