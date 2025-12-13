using Microsoft.EntityFrameworkCore;
using TreePassBot2.BotEngine.Extensions;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Data;

namespace BackManager.Server.Extensions;

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

        services.AddBotEngineServices();

        services.AddHostedService<BotHost>();

        return services;
    }
}
