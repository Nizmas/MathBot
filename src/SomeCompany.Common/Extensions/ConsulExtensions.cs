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
    /// Значение env для приложения по-умолчанию.
    /// </summary>
    private const string ConsulEnv = "CONSUL_URI";
    
    /// <summary>
    /// Добавление клиента в качестве хранителя данных во время работы приложения
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="config">Конфигурации для работы приложения</param>
    public static IServiceCollection AddConsulClient(this IServiceCollection services, IConfiguration config)
    {
        var consulServerPath = config.GetSection(ConsulEnv).Value;
        if (string.IsNullOrEmpty(consulServerPath))
        {
            throw new ArgumentNullException(ConsulEnv);
        }

        services.AddSingleton<IConsulClient>(consul => 
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
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IConfigurationBuilder AddConsul(this IConfigurationBuilder config, string consulPrefix, string? envPrefix)
    {
        var consulServerPath = Environment.GetEnvironmentVariable($"{envPrefix}{ConsulEnv}");

        if (string.IsNullOrEmpty(consulServerPath))
        {
            throw new ArgumentNullException(ConsulEnv);
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