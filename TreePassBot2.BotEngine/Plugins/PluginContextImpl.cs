using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.Services;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Plugins;

public class PluginContextImpl : IPluginContext
{
    private readonly IServiceProvider _rootService;
    private readonly string _pluginId;
    private readonly MakabakaService _botApi;

    public PluginContextImpl(string pluginId, IServiceProvider rootService)
    {
        _pluginId = pluginId;
        _rootService = rootService;

        _botApi = _rootService.GetRequiredService<MakabakaService>();

        var scopeFactory = _rootService.GetRequiredService<IServiceScopeFactory>();
        State = new PluginStateStorageImpl(pluginId, scopeFactory);

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

    /// <inheritdoc />
    public ITreePassBotCommunicationService BotApi => _botApi;

    /// <inheritdoc />
    public IPluginStateStorage State { get; }

    public ILogger Logger { get; }
}