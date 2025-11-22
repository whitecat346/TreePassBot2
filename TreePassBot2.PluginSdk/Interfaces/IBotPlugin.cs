namespace TreePassBot2.PluginSdk.Interfaces;

public interface IBotPlugin
{
    PluginMeta Meta { get; }
    Task OnLoadedAsync(IPluginContext context);
    Task OnUnloadedAsync();
}