using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.PluginSdk.Exceptions;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public partial class PluginManagerService(IServiceProvider services, ILogger<PluginManagerService> logger)
    : IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, PluginSupervisor> _activePlugins = [];

    private readonly ConcurrentDictionary<string, (PluginSupervisor Supervisor, IBotCommand Command)>
        _commandRouteTable = [];

    public IReadOnlyList<PluginSupervisor> ActivePlugins => _activePlugins.Values.ToList();

    /// <summary>
    /// Load a plugin from plugin file path.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot load this dll that not implemnet IBotPlugin interface.</exception>
    /// <exception cref="FailedToActivatePluginException">Throws if failed to activate a plugin.</exception>
    public async Task<bool> LoadPluginAsync(string dllPath)
    {
        var alc = new PluginLoadAssemblyContext(dllPath);
        var assembly = alc.LoadFromAssemblyPath(dllPath);
        var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IBotPlugin).IsAssignableFrom(t))
                      ?? throw new InvalidOperationException(
                             "Cannot load this dll that not implemnet IBotPlugin interface.");

        if (Activator.CreateInstance(pluginType) is not IBotPlugin pluginInstance)
        {
            throw new FailedToActivatePluginException($"Fialed to activate plugin: {pluginType}");
        }

        var context = new PluginLoadingLoadingContextImpl(pluginInstance.Meta.Id, services);

        try
        {
            await pluginInstance.OnLoadedAsync(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogFialedToLoadPluginId(logger, ex, pluginInstance.Meta.Id);
            alc.Unload();
            return false;
        }

        var supervisor = new PluginSupervisor(pluginInstance, alc, logger);
        supervisor.OnPluginException += SupervisorOnOnPluginException;

        var pluginId = pluginInstance.Meta.Id;

        // uload if already loaded
        if (_activePlugins.TryGetValue(pluginId, out var activePlugin))
        {
            await activePlugin.UnloadAsync().ConfigureAwait(false);
            _activePlugins.TryRemove(pluginId, out _);
        }

        // add active plugin
        _activePlugins[pluginId] = supervisor;

        // register all commands
        foreach (var cmd in context.RegisteredCommands)
        {
            RegisterRoute(cmd.Trigger, cmd, supervisor);

            foreach (var alias in cmd.Aliases)
            {
                RegisterRoute(alias, cmd, supervisor);
            }
        }

        LogHaveLoadedPluginNameRegisteredCountCommands(logger, pluginInstance.Meta.Name,
                                                       context.RegisteredCommands.Count);

        return true;
    }

    private void RegisterRoute(string trigger, IBotCommand cmd, PluginSupervisor supervisor)
    {
        if (_commandRouteTable.ContainsKey(trigger))
        {
            LogTriggerTriggerIsAlreadyRegisteredByNewplugin(logger, trigger, supervisor.Meta.Id);
            return;
        }

        _commandRouteTable[trigger] = (supervisor, cmd);
    }

    /// <summary>
    /// Dispatch command to appropriate plugin.
    /// </summary>
    /// <param name="cmdTrigger">Matched command.</param>
    /// <param name="cmdCtx">Message context.</param>
    public async Task DispatchCommandAsync(string cmdTrigger, ICommandContext cmdCtx)
    {
        if (!_commandRouteTable.TryGetValue(cmdTrigger, out var value))
        {
            var replyMsg = new MessageBuilder().AddText("Command not found");

            await cmdCtx.ReplyAsync(replyMsg).ConfigureAwait(false);
            return;
        }

        await value.Supervisor.SafeExecuteCommandAsync(value.Command, cmdCtx).ConfigureAwait(false);

        LogIssuedCommandCommandnameFromGroupidByUserid(logger, value.Command.Trigger, cmdCtx.GroupId, cmdCtx.SenderId);
    }

    private void SupervisorOnOnPluginException(string pluginId)
    {
        var tk = UnloadPluginAsync(pluginId);
        Task.WhenAny(tk);
    }

    private async Task UnloadPluginAsync(string pluginId)
    {
        if (_activePlugins.TryRemove(pluginId, out var supervisor))
        {
            await supervisor.UnloadAsync().ConfigureAwait(false);

            var keysToRemove = _commandRouteTable
                              .Where(kvp => kvp.Value.Supervisor == supervisor)
                              .Select(kvp => kvp.Key)
                              .ToList();

            foreach (var key in keysToRemove)
            {
                _commandRouteTable.TryRemove(key, out _);
            }

            LogUnloadPluginIdSuccessfully(logger, pluginId);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        foreach (var activePlugin in _activePlugins)
        {
            await activePlugin.Value.UnloadAsync().ConfigureAwait(false);
        }
    }

    #region LogMethod

    [LoggerMessage(LogLevel.Error, "Fialed to load plugin {Id}")]
    static partial void LogFialedToLoadPluginId(ILogger<PluginManagerService> logger, Exception ex, string id);

    [LoggerMessage(LogLevel.Information, "Have loaded plugin: {name}; Registered {count} commands")]
    static partial void LogHaveLoadedPluginNameRegisteredCountCommands(ILogger<PluginManagerService> logger,
                                                                       string name, int count);

    [LoggerMessage(LogLevel.Warning, "Trigger '{trigger}' is already registered by {newPlugin}")]
    static partial void LogTriggerTriggerIsAlreadyRegisteredByNewplugin(ILogger<PluginManagerService> logger,
                                                                        string trigger, string newPlugin);

    [LoggerMessage(LogLevel.Trace, "Issued command {commandName} from {groupId} by {userId}")]
    static partial void LogIssuedCommandCommandnameFromGroupidByUserid(ILogger<PluginManagerService> logger,
                                                                       string commandName, ulong groupId, ulong userId);

    [LoggerMessage(LogLevel.Information, "Unload plugin {id} successfully")]
    static partial void LogUnloadPluginIdSuccessfully(ILogger<PluginManagerService> logger, string id);

    #endregion
}
