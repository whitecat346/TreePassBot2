using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.PluginSdk.Exceptions;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public class PluginManager(IServiceProvider services, ILogger<PluginManager> logger) : IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, PluginSupervisor> _activePlugins = [];

    private readonly ConcurrentDictionary<string, (PluginSupervisor Supervisor, IBotCommand Command)>
        _commandRouteTable = [];

    /// <summary>
    /// Load a plugin from plugin file path.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot load this dll that not implemnet IBotPlugin interface.</exception>
    /// <exception cref="FailedToActivatePluginException">Throws if failed to activate a plugin.</exception>
    public async Task LoadPluginAsync(string dllPath)
    {
        var alc = new PluginLoadContext(dllPath);
        var assembly = alc.LoadFromAssemblyPath(dllPath);
        var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IBotPlugin).IsAssignableFrom(t))
                      ?? throw new InvalidOperationException(
                             "Cannot load this dll that not implemnet IBotPlugin interface.");

        if (Activator.CreateInstance(pluginType) is not IBotPlugin pluginInstance)
        {
            throw new FailedToActivatePluginException($"Fialed to activate plugin: {pluginType}");
        }

        var context = new PluginContextImpl(pluginInstance.Meta.Id, services);

        try
        {
            await pluginInstance.OnLoadedAsync(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fialed to load plugin {Id}", pluginInstance.Meta.Id);
            alc.Unload();
            return;
        }

        var supervisor = new PluginSupervisor(pluginInstance, alc, logger);
        supervisor.OnPluginException += SupervisorOnOnPluginException;

        _activePlugins[pluginInstance.Meta.Id] = supervisor;

        foreach (var cmd in context.RegisteredCommands)
        {
            RegisterRoute(cmd.Trigger, cmd, supervisor);

            foreach (var alias in cmd.Aliases)
            {
                RegisterRoute(alias, cmd, supervisor);
            }
        }

        logger.LogInformation("Have loaded plugin: {Name}; Registered {Count} commands",
                              pluginInstance.Meta.Name, context.RegisteredCommands.Count);
    }

    private void RegisterRoute(string trigger, IBotCommand cmd, PluginSupervisor supervisor)
    {
        if (_commandRouteTable.ContainsKey(trigger))
        {
            logger.LogWarning("Trigger '{Trigger}' is already registered by {NewPlugin}",
                              trigger, supervisor.Meta.Id);
            return;
        }

        _commandRouteTable[trigger] = (supervisor, cmd);
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

            logger.LogInformation("Unload plugin {Id} successfully", pluginId);
        }
    }

    /// <summary>
    /// Dispatch command to appropriate plugin.
    /// </summary>
    /// <param name="cmdTrigger">Matched command.</param>
    /// <param name="msgCtx">Message context.</param>
    public async Task DispatchCommandAsync(string cmdTrigger, ICommandContext msgCtx)
    {
        if (!_commandRouteTable.TryGetValue(cmdTrigger, out var value))
        {
            var replyMsg = new MessageBuilder().AddText("Command not found");

            await msgCtx.ReplyAsync(replyMsg).ConfigureAwait(false);
            return;
        }

        await value.Supervisor.SafeExecuteCommandAsync(value.Command, msgCtx).ConfigureAwait(false);

        logger.LogTrace("Issued command {CommandName} from {GroupId} by {UserId}",
                        value.Command.Trigger, msgCtx.GroupId, msgCtx.SenderId);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        foreach (var activePlugin in _activePlugins)
        {
            await activePlugin.Value.UnloadAsync().ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
    }
}