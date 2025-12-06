using Microsoft.Extensions.DependencyInjection;
using TreePassBot2.BotEngine.Message;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.BotEngine.Services;

namespace TreePassBot2.BotEngine.Extensions;

public static class DiExtensions
{
    public static IServiceCollection AddBotEngineServices(this IServiceCollection services)
    {
        services.AddSingleton<PluginManager>()
                .AddSingleton<CommandContextImplFactory>()
                .AddSingleton<MessageRouter>()
                .AddSingleton<MessageArchiveService>()
                .AddSingleton<BotEventRouter>();

        return services;
    }
}
