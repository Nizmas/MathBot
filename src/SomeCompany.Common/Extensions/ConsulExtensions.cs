using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Winton.Extensions.Configuration.Consul;

namespace SomeCompany.Common.Extensions;

/// <summary>
/// Расширения для работы с consul
/// </summary>
public static class ConsulExtensions
{
    /// <summary>
    /// Добавление клиента в качестве хранителя данных во время работы приложения
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="consulPrefix">Префикс-папка в consul на случай доступа из разных сервисов</param>
    /// <param name="config">Конфигурации для работы приложения</param>
    public static IServiceCollection AddConsulClient(this IServiceCollection services, string consulPrefix, IConfiguration config)
    {
        services.Configure<ConsulOptions>(cO => cO.Prefix = consulPrefix );
        
        var consulServerPath = config.GetSection(ConsulConsts.ConsulEnvName).Value;
        if (string.IsNullOrEmpty(consulServerPath))
        {
            throw new ArgumentNullException(ConsulConsts.ConsulEnvName);
        }

        services.AddScoped<IConsulClient>(consul => 
            new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(consulServerPath);
            }));

        return services;
    }

    /// <summary>
    /// Добавление консула в качестве источника конфигов
    /// </summary>
    /// <param name="config">Конфигурации для работы приложения</param>
    /// <param name="consulPrefix">Префикс-папка в consul на случай доступа из разных сервисов</param>
    /// <param name="envPrefix">Префикс переменных env для конкретного сервиса</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static IConfigurationBuilder AddConsul(this IConfigurationBuilder config, string consulPrefix, string? envPrefix)
    {
        var consulServerPath = Environment.GetEnvironmentVariable($"{envPrefix}{ConsulConsts.ConsulEnvName}");

        if (string.IsNullOrEmpty(consulServerPath))
        {
            throw new ArgumentNullException(ConsulConsts.ConsulEnvName);
        }

        config.AddConsul(consulPrefix, op =>
            {
                op.ConsulConfigurationOptions = cco =>
                {
                    cco.Address = new Uri(consulServerPath);
                };
            });

        return config;
    }
}