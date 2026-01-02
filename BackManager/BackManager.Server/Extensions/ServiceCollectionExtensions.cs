using Microsoft.EntityFrameworkCore;
using TreePassBot2.BotEngine.Extensions;
using TreePassBot2.BotEngine.Services;
using TreePassBot2.Core.Options;
using TreePassBot2.Data;

namespace BackManager.Server.Extensions;

/// <summary>
/// 服务集合扩展类
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加机器人服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置信息</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddBotServices(this IServiceCollection services, IConfiguration configuration)
    {
        var onebotConnStr = configuration.GetSection("Bot:ForwardWebSocket:Url").Value;
        if (string.IsNullOrEmpty(onebotConnStr))
        {
            throw new InvalidDataException("Unknown onebot connection url");
        }

        var connString = configuration.GetConnectionString("DefaultConnection");

        // skip aspire default connection string
        if (connString?.Contains("Server=localhost;Username=postgres;Database=postgres") == true)
        {
            var configBuilder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                               .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

            var fileConfig = configBuilder.Build();
            var fileConnString = fileConfig.GetConnectionString("DefaultConnection");

            if (!string.IsNullOrEmpty(fileConnString))
            {
                connString = fileConnString;
            }
        }

        services.Configure<BotOptions>(configuration.GetSection("BotSettings"));

        services.AddDbContextPool<BotDbContext>(options =>
                                                    options.UseNpgsql(connString));
        services.AddLazyCache();

        services.AddBotEngineServices();

        services.AddHostedService<BotHost>();

        return services;
    }
}
