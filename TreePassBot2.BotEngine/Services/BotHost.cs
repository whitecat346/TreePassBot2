using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;
using TreePassBot2.Core.Options;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public class BotHost(
    ICommunicationService communicationService,
    AppRuntimeInfo runtimeInfo,
    PluginManagerService pluginManager,
    IOptions<BotOptions> config) : IHostedService
{
    private readonly BotOptions _config = config.Value;

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var pluginDir = _config.PluginDir;
        await LoadPluginsAsync(pluginDir).ConfigureAwait(false);
        //await communicationService.ConnectAsync().ConfigureAwait(false);

        runtimeInfo.StartTime = DateTimeOffset.UtcNow;
        runtimeInfo.CurrentVersion = Assembly.GetExecutingAssembly()
                                             .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                            ?.InformationalVersion ?? "Unknown";
    }

    private async Task LoadPluginsAsync(string pluginDir)
    {
        if (!Directory.Exists(pluginDir))
        {
            Directory.CreateDirectory(pluginDir);
            return;
        }

        var pluginFiles = Directory.EnumerateFiles(pluginDir, "*.dll", SearchOption.TopDirectoryOnly);
        foreach (var pluginF in pluginFiles)
        {
            await pluginManager.LoadPluginAsync(pluginF).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await pluginManager.DisposeAsync().ConfigureAwait(false);
        //await communicationService.DisconnectAsync().ConfigureAwait(false);
    }
}
