using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.PluginSdk;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public partial class PluginSupervisor(
    PluginLoadAssemblyContext loadAssemblyCtx,
    ILogger logger) : IDisposable
{
    private int _errorCount;
    private const int MaxErrors = 5;
    //private const int ErrorWindowSeconds = 60; not in use yet

    public required IBotPlugin Plugin;
    public PluginMeta Meta => Plugin.Meta;
    public required string ShadowPluginFilePath;
    public required string RealPluginFilePath;
    public bool IsAlive { get; private set; } = true;
    public bool IsActive { get; internal set; } = true;

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
        if (!(IsActive && IsAlive))
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
        LogPluginException(logger, ex, Meta.Name, action, _errorCount);

        if (_errorCount >= MaxErrors)
        {
            LogPluginExceptionTooMany(logger, Meta.Name);
            IsAlive = false;
            IsActive = false;
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
            await Plugin.OnUnloadedAsync().ConfigureAwait(false);
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

    /// <inheritdoc />
    public void Dispose()
    {
        var tk = UnloadAsync();
        Task.WhenAll(tk);
    }

    [LoggerMessage(LogLevel.Error, "Plugin {plugin} occurred a/an exception: {action}. Current error count: {count}")]
    static partial void LogPluginException(ILogger logger, Exception ex, string plugin, string action, int count);

    [LoggerMessage(LogLevel.Critical, "Plugin {plugin} occurred too many error, unloading...")]
    static partial void LogPluginExceptionTooMany(ILogger logger, string plugin);
}
