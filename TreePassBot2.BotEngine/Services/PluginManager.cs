using Microsoft.Extensions.Logging;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.PluginSdk.Exceptions;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public class PluginManager(IServiceProvider services, ILogger<PluginManager> logger)
{
    private readonly Dictionary<string, PluginSupervisor> _activePlugins = [];

    /// <exception cref="InvalidOperationException">Cannot load this dll that not implemnet IBotPlugin interface.</exception>
    /// <exception cref="FailedToActivatePluginException">Throws if failed to activate a plugin.</exception>
    public async Task LoadPluginAsync(string dllPath)
    {
        var alc = new PluginLoadContext(dllPath);

        var assembly = alc.LoadFromAssemblyPath(dllPath);

        var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IBotPlugin).IsAssignableFrom(t));

        if (pluginType == null)
        {
            throw new InvalidOperationException("Cannot load this dll that not implemnet IBotPlugin interface.");
        }

        var pluginInstance = Activator.CreateInstance(pluginType) as IBotPlugin;

        if (pluginInstance == null)
        {
            throw new FailedToActivatePluginException($"Fialed to activate plugin: {pluginType}");
        }

        var context = new PluginContextImpl(pluginInstance.Meta.Id, services);

        await pluginInstance.OnLoadedAsync(context).ConfigureAwait(false);

        var supervisor = new PluginSupervisor(pluginInstance, alc, logger);
        _activePlugins.Add(pluginInstance.Meta.Id, supervisor);

        logger.LogInformation("Have loadded plugin: {Name}", pluginInstance.Meta.Name);
    }

    public async Task DispatchCommandAsync(string cmdTrigger, ICommandContext msgCtx)
    {
        foreach (var supervisor in _activePlugins.Values)
        {
            // TODO: impl this
            // 假设 PluginContextImpl 里存储了该插件注册的 commands 列表
            // 这里找到匹配的命令并调用 Supervisor 的 SafeExecuteCommandAsync
        }
    }
}