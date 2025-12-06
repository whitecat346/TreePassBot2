using Microsoft.Extensions.Logging;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace TreePassBot2.PluginSdk.Interfaces;

public interface IPluginContext
{
    void RegisterCommand(IBotCommand command);

    ICommunicationService BotApi { get; }
    IPluginStateStorage State { get; }
    ILogger Logger { get; }
    IReadOnlyList<IBotCommand> RegisteredCommands { get; }
}