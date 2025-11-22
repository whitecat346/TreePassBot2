namespace TreePassBot2.PluginSdk.Interfaces;

public interface IBotPlugin
{
    PluginMeta Meta { get; }
    Task OnLoadedAsyn(IPluginContext context);
    Task OnUnloadedAsync();
}