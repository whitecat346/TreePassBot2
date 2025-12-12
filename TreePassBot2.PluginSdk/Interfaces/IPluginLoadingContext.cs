using Microsoft.Extensions.Logging;

namespace TreePassBot2.PluginSdk.Interfaces;

public interface IPluginLoadingContext
{
    void RegisterCommand(IBotCommand command);
    IReadOnlyList<IBotCommand> RegisteredCommands { get; }
    ILogger Logger { get; }
}
