using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Plugins;

public class CommandContextImplFactory(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Activate <see cref="CommandContextImpl"/>.
    /// </summary>
    public ICommandContext Create(
        string nickName,
        ulong senderId,
        ulong groupId,
        long msgId,
        Infrastructure.MakabakaAdaptor.Models.Message rawMessage)
    {
        var communicationService = serviceProvider.GetRequiredService<ICommunicationService>();
        var logger = serviceProvider.GetRequiredService<ILogger<CommandContextImpl>>();

        var instance = new CommandContextImpl(communicationService)
        {
            SenderNickName = nickName,
            SenderId = senderId,
            GroupId = groupId,
            MessageId = msgId,
            RawMessage = rawMessage
        };

        return instance;
    }
}
