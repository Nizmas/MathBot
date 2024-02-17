using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Winton.Extensions.Configuration.Consul;

namespace SomeCompany.Common.Extensions;

public static class ConsulExtensions
{
    private const string ConsulEnv = "CONSUL_URI";
    
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