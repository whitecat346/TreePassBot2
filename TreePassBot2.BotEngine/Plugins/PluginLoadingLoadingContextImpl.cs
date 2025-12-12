using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Plugins;

public class PluginLoadingLoadingContextImpl : IPluginLoadingContext
{
    public PluginLoadingLoadingContextImpl(string pluginId, IServiceProvider rootService)
    {
        var loggerFactory = rootService.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger($"Plugin.{pluginId}");
    }

    private readonly List<IBotCommand> _commands = [];

    public IReadOnlyList<IBotCommand> RegisteredCommands => _commands.AsReadOnly();

    /// <inheritdoc />
    public void RegisterCommand(IBotCommand command)
    {
        _commands.Add(command);
        Logger.LogDebug("Command registered: {Trigger}", command.Trigger);
    }

    public ILogger Logger { get; }
}
