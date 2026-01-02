using Microsoft.Extensions.DependencyInjection;
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

    /// <summary>
    /// Gets a read-only list of active plugins.
    /// </summary>
    public IReadOnlyList<PluginSupervisor> ActivePlugins => _activePlugins.Values.ToList().AsReadOnly();

    private readonly string _currentRootPath = Directory.GetCurrentDirectory();
    private readonly string _shadowCachePath;

    public PluginManagerService(IServiceProvider services, ILogger<PluginManagerService> logger)
    {
        _services = services;
        _logger = logger;

        _shadowCachePath = Path.Combine(_currentRootPath, "shadow_cache");
        // Create shadow cache directory if it doesn't exist, don't delete existing contents
        if (!Directory.Exists(_shadowCachePath))
        {
            Directory.CreateDirectory(_shadowCachePath);
        }
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
        var pluginTypes = assembly.GetTypes().Where(t => typeof(IBotPlugin).IsAssignableFrom(t) && !t.IsAbstract)
                                  .ToList();

        if (pluginTypes.Count == 0)
        {
            throw new InvalidOperationException(
                "Cannot load this dll that not implement IBotPlugin interface.");
        }

        var loadedPlugins = 0;

        foreach (var pluginType in pluginTypes)
        {
            if (await LoadSinglePluginAsync(alc, dllPath, shadowPath, pluginType).ConfigureAwait(false))
            {
                loadedPlugins++;
            }
        }

        // If no plugins were loaded successfully, unload the ALC and return false
        if (loadedPlugins == 0)
        {
            alc.Unload();
            return false;
        }

        return true;
    }


    /// <summary>
    /// Copies a file asynchronously with appropriate stream management.
    /// </summary>
    /// <param name="source">The source file path.</param>
    /// <param name="target">The target file path.</param>
    /// <returns>A task representing the asynchronous copy operation.</returns>
    private static async Task CopyFileAsync(string source, string target)
    {
        await using var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        await using var targetStream = new FileStream(target, FileMode.Create);
        await sourceStream.CopyToAsync(targetStream).ConfigureAwait(false);
    }

    /// <summary>
    /// Loads a single plugin from the specified type.
    /// </summary>
    /// <param name="alc">The assembly load context.</param>
    /// <param name="realPath">The real path to the plugin DLL.</param>
    /// <param name="shadowPath">The shadow path to the plugin DLL.</param>
    /// <param name="pluginType">The plugin type to load.</param>
    /// <returns><c>true</c> if the plugin was loaded successfully; otherwise, <c>false</c>.</returns>
    private async Task<bool> LoadSinglePluginAsync(PluginLoadAssemblyContext alc, string realPath, string shadowPath,
                                                   Type pluginType)
    {
        try
        {
            // Create plugin instance with dependency injection support
            var pluginInstance = (IBotPlugin)ActivatorUtilities.CreateInstance(_services, pluginType);
            if (pluginInstance is null)
            {
                throw new FailedToActivatePluginException($"Failed to activate plugin: {pluginType}");
            }

            // Initialize plugin
            var context = new PluginLoadingLoadingContextImpl(pluginInstance.Meta.Id, _services);
            try
            {
                await pluginInstance.OnLoadedAsync(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogFailedToLoadPluginId(_logger, ex, pluginInstance.Meta.Id);
                return false;
            }

            // Create supervisor
            var supervisor = new PluginSupervisor(alc, _logger)
            {
                Plugin = pluginInstance,
                ShadowPluginFilePath = shadowPath,
                RealPluginFilePath = realPath
            };
            supervisor.OnPluginException += SupervisorOnOnPluginException;

            // Cleanup existing plugin if it exists
            var pluginId = pluginInstance.Meta.Id;
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
                    LogFailedToDeleteOldPlugin(_logger, activePlugin.Meta.Id, activePlugin.ShadowPluginFilePath, ex);
                    return false;
                }
            }

            // Register plugin and commands
            _activePlugins[pluginId] = supervisor;
            RegisterPluginCommands(pluginInstance, supervisor, context);

            return true;
        }
        catch (Exception ex)
        {
            LogFailedToProcessPluginType(_logger, ex, pluginType.FullName ?? "Unknown");
            return false;
        }
    }

    /// <summary>
    /// Registers all commands for a plugin.
    /// </summary>
    /// <param name="pluginInstance">The plugin instance.</param>
    /// <param name="supervisor">The plugin supervisor.</param>
    /// <param name="context">The plugin loading context.</param>
    private void RegisterPluginCommands(IBotPlugin pluginInstance, PluginSupervisor supervisor,
                                        PluginLoadingLoadingContextImpl context)
    {
        foreach (var cmd in context.RegisteredCommands)
        {
            RegisterRoute(cmd.Trigger, cmd, supervisor);

            foreach (var alias in cmd.Aliases)
            {
                RegisterRoute(alias, cmd, supervisor);
            }
        }

        LogHaveLoadedPlugin(_logger, pluginInstance.Meta.Name, context.RegisteredCommands.Count);
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

    /// <summary>
    /// Get plugin id from trigger
    /// </summary>
    /// <param name="cmdTrigger">Trigger</param>
    /// <param name="id">Got plugin id</param>
    /// <returns>Is success.</returns>
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
    /// Toggles the active state of a plugin.
    /// </summary>
    /// <param name="pluginId">The ID of the plugin to toggle.</param>
    /// <returns><c>true</c> if the plugin state was toggled; otherwise, <c>false</c> if the plugin was not found.</returns>
    public Task<bool> TogglePluginStateAsync(string pluginId)
    {
        // Use case-insensitive lookup for plugin ID
        var plugin = _activePlugins.Values
                                   .SingleOrDefault(p => p.Meta.Id.Equals(
                                                        pluginId, StringComparison.OrdinalIgnoreCase));

        if (plugin is null)
        {
            return Task.FromResult(false);
        }

        plugin.IsActive = !plugin.IsActive;

        return Task.FromResult(true);
    }

    /// <summary>
    /// Dispatch command to appropriate plugin.
    /// </summary>
    /// <param name="cmdTrigger">Matched command.</param>
    /// <param name="cmdCtx">Message context.</param>
    /// <param name="ignoreNotFound">Ignore not found command if <see langword="true"/>. (Default <see langword="false"/>)</param>
    /// <exception cref="KeyNotFoundException">Throws if command executer not found when <see cref="ignoreNotFound"/> is <see langword="true"/></exception>
    public async Task DispatchCommandAsync(string cmdTrigger, ICommandContext cmdCtx, bool ignoreNotFound = false)
    {
        LogDispatchCommand(_logger, cmdTrigger);

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

    /// <summary>
    /// Unloads a plugin by its ID.
    /// </summary>
    /// <param name="pluginId">The ID of the plugin to unload.</param>
    public async Task UnloadPluginAsync(string pluginId)
    {
        if (_activePlugins.TryRemove(pluginId, out var supervisor))
        {
            // Remove event handler to prevent memory leaks
            supervisor.OnPluginException -= SupervisorOnOnPluginException;

            try
            {
                // Unload the plugin
                await supervisor.UnloadAsync().ConfigureAwait(false);

                // Remove all command routes associated with this plugin
                RemovePluginCommandRoutes(supervisor);

                LogUnloadPluginIdSuccessfully(_logger, pluginId);
            }
            catch (Exception ex)
            {
                LogErrorUnloadingPlugin(_logger, ex, pluginId);
            }
        }
    }

    /// <summary>
    /// Removes all command routes associated with a plugin supervisor.
    /// </summary>
    /// <param name="supervisor">The supervisor whose command routes to remove.</param>
    private void RemovePluginCommandRoutes(PluginSupervisor supervisor)
    {
        // Find all keys to remove using a more efficient approach
        var keysToRemove = _commandRouteTable
                          .Where(kvp => kvp.Value.Supervisor == supervisor)
                          .Select(kvp => kvp.Key)
                          .ToList();

        // Remove all found keys
        foreach (var key in keysToRemove)
        {
            _commandRouteTable.TryRemove(key, out _);
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

    /// <summary>
    /// Handles plugin exception events by unloading the problematic plugin.
    /// </summary>
    /// <param name="pluginId">The ID of the plugin that encountered an exception.</param>
    private async void SupervisorOnOnPluginException(string pluginId)
    {
        await UnloadPluginAsync(pluginId).ConfigureAwait(false);
    }

    #region LogMethod

    [LoggerMessage(LogLevel.Error, "Failed to load plugin {Id}")]
    static partial void LogFailedToLoadPluginId(ILogger<PluginManagerService> logger,
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


    [LoggerMessage(LogLevel.Error, "Failed to delete old plugin file: {id}; {path}")]
    static partial void LogFailedToDeleteOldPlugin(ILogger<PluginManagerService> logger,
                                                   string id, string path, Exception exception);

    [LoggerMessage(LogLevel.Trace, "Dispatch command: {trigger}")]
    static partial void LogDispatchCommand(ILogger<PluginManagerService> logger, string trigger);


    [LoggerMessage(LogLevel.Error, "Failed to process plugin type {pluginType}")]
    static partial void LogFailedToProcessPluginType(ILogger<PluginManagerService> logger,
                                                     Exception ex, string pluginType);

    [LoggerMessage(LogLevel.Error, "Error unloading plugin {pluginId}")]
    static partial void LogErrorUnloadingPlugin(ILogger<PluginManagerService> logger,
                                                Exception ex, string pluginId);

    #endregion
}
