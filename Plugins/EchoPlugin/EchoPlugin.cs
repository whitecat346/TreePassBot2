using Microsoft.Extensions.Logging;
using TreePassBot2.PluginSdk;
using TreePassBot2.PluginSdk.Interfaces;

namespace EchoPlugin;

public class EchoPlugin : IBotPlugin
{
    /// <inheritdoc />
    public PluginMeta Meta { get; } =
        new PluginMeta("com.text.whitecat", "Echo", "1.0.0", "whitecat346", "A simple echo test plugin.");

    /// <inheritdoc />
    public Task OnLoadedAsync(IPluginLoadingContext loadingContext)
    {
        loadingContext.RegisterCommand(new EchoCommand());
        loadingContext.Logger.LogInformation("Load echo plugin");

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnUnloadedAsync()
    {
        return Task.CompletedTask;
    }
}
