using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Plugins;

public class PluginContextImpl : IPluginContext
{
    private readonly IServiceProvider _rootService;
    private readonly string _pluginId;
    private readonly BotApiImpl _botApi;

    public PluginContextImpl(string pluginId, IServiceProvider rootService)
    {
        _pluginId = pluginId;
        _rootService = rootService;

        _botApi = ActivatorUtilities.CreateInstance<BotApiImpl>(rootService);

        var scopeFactory = _rootService.GetRequiredService<IServiceScopeFactory>();
        State = new PluginStateStorageImpl(pluginId, scopeFactory);

        var loggerFactory = rootService.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger($"Plugin.{pluginId}");
    }

    internal List<IBotCommand> RegisteredCommands { get; } = [];

    /// <inheritdoc />
    public void RegisterCommand(IBotCommand command)
    {
        RegisteredCommands.Add(command);
        Logger.LogDebug("Command registered: {Trigger}", command.Trigger);
    }

    /// <inheritdoc />
    public IBotApi BotApi => _botApi;

    /// <inheritdoc />
    public IPluginStateStorage State { get; }

    public ILogger Logger { get; }
}