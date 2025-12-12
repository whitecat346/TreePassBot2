using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.PluginSdk;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public class PluginSupervisor(
    IBotPlugin plugin,
    PluginLoadAssemblyContext loadAssemblyCtx,
    ILogger logger)
{
    private int _errorCount;
    private const int MaxErrors = 5;
    private const int ErrorWindowSeconds = 60;

    public PluginMeta Meta => plugin.Meta;
    public bool IsAlive { get; private set; } = true;

    /// <summary>
    /// Invoke when plugin occurred exception and unloaded.
    /// </summary>
    /// <remarks>
    /// - First parameter is unloaded plugin ID.
    /// </remarks>
    public event Action<string>? OnPluginException;

    /// <summary>
    /// Execute plugin command safely.
    /// </summary>
    public async Task SafeExecuteCommandAsync(IBotCommand cmd, ICommandContext ctx)
    {
        if (!IsAlive)
        {
            return;
        }

        try
        {
            await cmd.ExecuteAsync(ctx).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, "ExecuteCommand").ConfigureAwait(false);
        }
    }

    private async Task HandleExceptionAsync(Exception ex, string action)
    {
        _errorCount++;
        logger.LogError(ex, "Plugin {Plugin} occurred a/an exception: {Action}. Current error count: {Count}",
                        Meta.Name, action, _errorCount);

        if (_errorCount >= MaxErrors)
        {
            logger.LogCritical("Plugin {Plugin} occurred too many error, unloading...", Meta.Name);
            IsAlive = false;
            await UnloadAsync().ConfigureAwait(false);
            OnPluginException?.Invoke(Meta.Id);
        }
    }

    /// <summary>
    /// Unload loaded plugin.
    /// </summary>
    public async Task UnloadAsync()
    {
        try
        {
            await plugin.OnUnloadedAsync().ConfigureAwait(false);
        }
        catch
        {
            // ignore
        }
        finally
        {
            loadAssemblyCtx.Unload();
        }
    }
}
