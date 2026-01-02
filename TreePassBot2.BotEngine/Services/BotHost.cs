using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;
using TreePassBot2.BotEngine.Message;
using TreePassBot2.Core.Options;
using TreePassBot2.Data;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public class BotHost(
    ICommunicationService communicationService,
    BotEventRouter router,
    AppRuntimeInfo runtimeInfo,
    PluginManagerService pluginManager,
    UserManageService userManage,
    IServiceScopeFactory scopeFactory,
    IOptions<BotOptions> config) : IHostedService
{
    private readonly BotOptions _config = config.Value;

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();
        await db.Database.MigrateAsync(ct).ConfigureAwait(false);

        var pluginDir = _config.PluginDir;
        await LoadPluginsAsync(pluginDir).ConfigureAwait(false);
        await communicationService.ConnectAsync().ConfigureAwait(false);
        await userManage.InitInfoAsync().ConfigureAwait(false);
        router.StartRoute();

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
        await communicationService.DisconnectAsync().ConfigureAwait(false);
    }
}
