using Makabaka;
using Microsoft.EntityFrameworkCore;
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

        services.AddSingleton<MakabakaApp>(app =>
            {
                var builder = new MakabakaAppBuilder();
                return builder.Build();
            }
        );

        services.AddSingleton<ICommunicationService, MakabakaService>();

        return services;
    }
}