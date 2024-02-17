using MathBot.Bll.Implementations.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SomeCompany.Common.Extensions;

namespace MathBot.Console;

public class Startup
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="Startup" />.
    /// </summary>
    /// <param name="configuration">Configuration</param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// Configuration
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Configure services method
    /// </summary>
    /// <param name="services">DI контейнер</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.RegistrationMathBotBll(Configuration)
                .AddConsulClient(Configuration);
    }
}