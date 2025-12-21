using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TreePassBot2.BotEngine.Interfaces;
using TreePassBot2.BotEngine.Message;
using TreePassBot2.BotEngine.Message.Routers.Event;
using TreePassBot2.BotEngine.Plugins;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.Services;

namespace TreePassBot2.BotEngine.Extensions;

public static class DiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBotEngineServices()
        {
            services.AddSingleton<PluginManagerService>()
                    .AddSingleton<CommandContextImplFactory>()
                    .AddSingleton<MessageRouter>()
                    .AddSingleton<MessageArchiveService>()
                    .AddSingleton<BotEventRouter>()
                    .AddSingleton<AppRuntimeInfo>()
                    .AddSingleton<UserManageService>()
                    .AddSingleton<ICommunicationService, MakabakaService>();

            services.AddScoped<WithdrawMessageFlagger>()
                    .AddScoped<AuditManagerService>();

            services.AddBotEngineMessageHandlers();

            return services;
        }

        private IServiceCollection AddBotEngineMessageHandlers()
        {
            var handlerTypes = Assembly.GetExecutingAssembly().GetTypes()
                                       .Where(t => typeof(IMessageHandler).IsAssignableFrom(t) &&
                                                   t is { IsInterface: false, IsAbstract: false });

            foreach (var type in handlerTypes)
            {
                Console.WriteLine($"Add message handler: {type.Name}");

                services.AddScoped(typeof(IMessageHandler), type);
            }

            return services;
        }
    }
}
