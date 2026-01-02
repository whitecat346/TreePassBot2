using TreePassBot2.PluginSdk;
using TreePassBot2.PluginSdk.Interfaces;

namespace AuditPlugin;

public class AuditPlugin : IBotPlugin
{
    /// <inheritdoc />
    public PluginMeta Meta { get; } = new("com.audit.whitecat346",
                                          "Audit",
                                          "1.0.0",
                                          "whitecat346",
                                          "A audit plugin.");

    /// <inheritdoc />
    public Task OnLoadedAsync(IPluginLoadingContext loadingContext)
    {
        loadingContext.RegisterCommand(new AddUnexistUsersCommand());
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnUnloadedAsync()
    {
        return Task.CompletedTask;
    }
}
