using Microsoft.Extensions.DependencyInjection;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor;

/// <summary>
/// 服务集合扩展类
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加Makabaka适配器
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMakabakaAdapter(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        // 注册适配器接口和实现类
        services.AddSingleton<IMakabakaAdapter, MakabakaAdapter>();

        return services;
    }

    /// <summary>
    /// 添加TreePassBot通信服务（基于Makabaka）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddTreePassBotMakabakaService(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        // 注册TreePassBot通信服务为单例服务
        services.AddSingleton<ITreePassBotCommunicationService, MakabakaService>();

        return services;
    }
}