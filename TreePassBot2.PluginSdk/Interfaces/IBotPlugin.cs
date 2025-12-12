namespace TreePassBot2.PluginSdk.Interfaces;

public interface IBotPlugin
{
    PluginMeta Meta { get; }

    Task OnLoadedAsync(IPluginLoadingContext loadingContext);

    Task OnUnloadedAsync();
}
