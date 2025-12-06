using Makabaka;
using Microsoft.EntityFrameworkCore;
using TreePassBot2.BotEngine.Extensions;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Data;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.Services;

namespace TreePassBot2.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBotServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connString))
        {
            connString = "Host=pgsql;Database=treepass_bot;Username=postgres;Password=rtyw3p4f";
        }

        services.AddDbContextPool<BotDbContext>(options =>
                                                    options.UseNpgsql(connString));

        var builder = new MakabakaAppBuilder();
        var makabakaApp = builder.Build();

        services.AddSingleton<MakabakaApp>(_ => makabakaApp);
        services.AddSingleton<IBotContext>(_ => makabakaApp.BotContext);

        services.AddSingleton<ICommunicationService, MakabakaService>();

        services.AddBotEngineServices();

        services.AddHostedService<BotHost>();

        return services;
    }
}