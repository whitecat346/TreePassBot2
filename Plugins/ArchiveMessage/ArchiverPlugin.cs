using TreePassBot2.PluginSdk;
using TreePassBot2.PluginSdk.Interfaces;

namespace ArchiveMessage;

public class ArchiverPlugin : IBotPlugin
{
    /// <inheritdoc />
    public PluginMeta Meta { get; } = new PluginMeta("com.message.archiver.whitecat",
                                                     "MessageArchiver",
                                                     "v1.0.0",
                                                     "whitecat346",
                                                     "Archive message by command.");

    /// <inheritdoc />
    public Task OnLoadedAsync(IPluginLoadingContext loadingContext)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnUnloadedAsync()
    {
        return Task.CompletedTask;
    }
}
