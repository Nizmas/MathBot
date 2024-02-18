using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MathBot.Dal.Interfaces;

namespace MathBot.Dal.Implementations.Extensions;

public static class MathBotDalExtension
{
    public static IServiceCollection RegistrationMathBotDal(this IServiceCollection services/*, IConfiguration config*/)
    {
        services.TryAddScoped<IScriptsProvider, ScriptsProvider>();
        
        return services;
    }
    
}