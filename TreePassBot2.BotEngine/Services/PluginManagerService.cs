using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.PluginSdk.Exceptions;
using TreePassBot2.PluginSdk.Interfaces;

// ReSharper disable FlagArgument

namespace TreePassBot2.BotEngine.Services;

public partial class PluginManagerService : IAsyncDisposable
{
    private readonly IServiceProvider _services;
    private readonly ILogger<PluginManagerService> _logger;
    private readonly ConcurrentDictionary<string, PluginSupervisor> _activePlugins = [];

    private readonly ConcurrentDictionary<string, (PluginSupervisor Supervisor, IBotCommand Command)>
        _commandRouteTable = [];

    public IReadOnlyList<PluginSupervisor> ActivePlugins => _activePlugins.Values.ToList();

    private readonly string _currentRootPath = Directory.GetCurrentDirectory();
    private readonly string _shadowCachePath;

    public PluginManagerService(IServiceProvider services, ILogger<PluginManagerService> logger)
    {
        _services = services;
        _logger = logger;

        _shadowCachePath = Path.Combine(_currentRootPath, "shadow_cache");
        if (Directory.Exists(_shadowCachePath))
        {
            Directory.Delete(_shadowCachePath, true);
        }

        Directory.CreateDirectory(_shadowCachePath);
    }

    /// <summary>
    /// Load a plugin from plugin file path.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot load this dll that not implemnet IBotPlugin interface.</exception>
    /// <exception cref="FailedToActivatePluginException">Throws if failed to activate a plugin.</exception>
    public async Task<bool> LoadPluginAsync(string dllPath)
    {
        if (!Path.IsPathRooted(dllPath))
        {
            dllPath = Path.Combine(_currentRootPath, dllPath);
        }

        var shadowPath = Path.Combine(
            _shadowCachePath,
            $"{Path.GetFileNameWithoutExtension(dllPath)}_{Guid.NewGuid()}.dll"
        );

        await CopyFileAsync(dllPath, shadowPath).ConfigureAwait(false);

        var alc = new PluginLoadAssemblyContext(shadowPath);
        var assembly = alc.LoadFromAssemblyPath(shadowPath);
        var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IBotPlugin).IsAssignableFrom(t))
                      ?? throw new InvalidOperationException(
                             "Cannot load this dll that not implemnet IBotPlugin interface.");

        if (Activator.CreateInstance(pluginType) is not IBotPlugin pluginInstance)
        {
            throw new FailedToActivatePluginException($"Fialed to activate plugin: {pluginType}");
        }

        var context = new PluginLoadingLoadingContextImpl(pluginInstance.Meta.Id, _services);

        try
        {
            await pluginInstance.OnLoadedAsync(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogFialedToLoadPluginId(_logger, ex, pluginInstance.Meta.Id);
            alc.Unload();
            return false;
        }

        var supervisor = new PluginSupervisor(alc, _logger)
        {
            Plugin = pluginInstance,
            ShadowPluginFilePath = shadowPath,
            RealPluginFilePath = dllPath
        };
        supervisor.OnPluginException += SupervisorOnOnPluginException;

        var pluginId = pluginInstance.Meta.Id;

        // unload and delete if already loaded
        if (_activePlugins.TryGetValue(pluginId, out var activePlugin))
        {
            await UnloadPluginAsync(pluginId).ConfigureAwait(false);
            _activePlugins.Remove(pluginId, out _);

            try
            {
                File.Delete(activePlugin.RealPluginFilePath);
            }
            catch (Exception ex)
            {
                LogFialedToDeleteOldPlugin(_logger, activePlugin.Meta.Id, activePlugin.ShadowPluginFilePath, ex);

                return false;
            }

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

        LogHaveLoadedPlugin(_logger, pluginInstance.Meta.Name, context.RegisteredCommands.Count);

        return true;
    }

    private async Task CopyFileAsync(string source, string target)
    {
        await using var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        await using var targetStream = new FileStream(target, FileMode.Create);
        await sourceStream.CopyToAsync(targetStream).ConfigureAwait(false);
    }

    private void RegisterRoute(string trigger, IBotCommand cmd, PluginSupervisor supervisor)
    {
        if (_commandRouteTable.ContainsKey(trigger))
        {
            LogTriggerIsAlreadyRegistered(_logger, trigger, supervisor.Meta.Id);
            return;
        }

        _commandRouteTable[trigger] = (supervisor, cmd);
    }

    public Task<bool> TryGetPluginIdAsync(string cmdTrigger, [NotNullWhen(true)] out string? id)
    {
        id = null;

        if (!_commandRouteTable.TryGetValue(cmdTrigger, out var value))
        {
            id = null;
            return Task.FromResult(false);
        }

        id = value.Supervisor.Meta.Id;
        return Task.FromResult(true);
    }

    /// <summary>
    /// Dispatch command to appropriate plugin.
    /// </summary>
    /// <param name="cmdTrigger">Matched command.</param>
    /// <param name="cmdCtx">Message context.</param>
    public async Task DispatchCommandAsync(string cmdTrigger, ICommandContext cmdCtx, bool ignoreNotFound = false)
    {
        _logger.LogInformation("Dispatch command: {Trigger}", cmdTrigger);

        PluginSupervisor supervisor;
        IBotCommand command;
        if (ignoreNotFound)
        {
            (supervisor, command) = _commandRouteTable[cmdTrigger];
        }
        else
        {
            if (!_commandRouteTable.TryGetValue(cmdTrigger, out var value))
            {
                var replyMsg = new MessageBuilder()
                              .AddAt(cmdCtx.SenderId)
                              .AddText("Command not found");

                await cmdCtx.ReplyAsync(replyMsg).ConfigureAwait(false);
                return;
            }

            supervisor = value.Supervisor;
            command = value.Command;
        }

        await supervisor.SafeExecuteCommandAsync(command, cmdCtx).ConfigureAwait(false);

        LogIssuedCommand(_logger, command.Trigger, cmdCtx.GroupId, cmdCtx.SenderId);
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

            LogUnloadPluginIdSuccessfully(_logger, pluginId);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        foreach (var activePlugin in _activePlugins)
        {
            activePlugin.Value.OnPluginException -= SupervisorOnOnPluginException;
            await activePlugin.Value.UnloadAsync().ConfigureAwait(false);
        }

        _activePlugins.Clear();
    }

    #region LogMethod

    [LoggerMessage(LogLevel.Error, "Fialed to load plugin {Id}")]
    static partial void LogFialedToLoadPluginId(ILogger<PluginManagerService> logger,
                                                Exception ex, string id);

    [LoggerMessage(LogLevel.Information, "Have loaded plugin: {name}; Registered {count} commands")]
    static partial void LogHaveLoadedPlugin(ILogger<PluginManagerService> logger,
                                            string name, int count);

    [LoggerMessage(LogLevel.Warning, "Trigger '{trigger}' is already registered by {newPlugin}")]
    static partial void LogTriggerIsAlreadyRegistered(ILogger<PluginManagerService> logger,
                                                      string trigger, string newPlugin);

    [LoggerMessage(LogLevel.Trace, "Issued command {commandName} from {groupId} by {userId}")]
    static partial void LogIssuedCommand(ILogger<PluginManagerService> logger,
                                         string commandName, ulong groupId, ulong userId);

    [LoggerMessage(LogLevel.Information, "Unload plugin {id} successfully")]
    static partial void LogUnloadPluginIdSuccessfully(ILogger<PluginManagerService> logger, string id);

    #endregion

    [LoggerMessage(LogLevel.Error, "Fialed to delete old plugin file: {id}; {path}")]
    static partial void LogFialedToDeleteOldPlugin(ILogger<PluginManagerService> logger,
                                                   string id, string path, Exception exception);
}
