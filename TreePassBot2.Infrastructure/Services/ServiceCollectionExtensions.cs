using Microsoft.Extensions.DependencyInjection;
using TreePassBot2.Infrastructure.MakabakaAdaptor;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace TreePassBot2.Infrastructure.Services;

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
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/></exception>
    public static IServiceCollection AddMakabakaAdapter(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.AddSingleton<IMakabakaAdapter, MakabakaAdapter>();

        return services;
    }

    /// <summary>
    /// 添加TreePassBot通信服务（基于Makabaka）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/></exception>
    public static IServiceCollection AddTreePassBotMakabakaService(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.AddSingleton<ITreePassBotCommunicationService, MakabakaService>();

        return services;
    }
}