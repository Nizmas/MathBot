using MathBot.Bll.Interfaces;
using MathBot.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telegram.Bots;
using Telegram.Bots.Extensions.Polling;

namespace MathBot.Bll.Implementations.Extensions;

public static class MathBotBllExtension
{
    public static IServiceCollection RegistrationMathBotBll(this IServiceCollection services, IConfiguration config)
    {
        var tBotOptions = config.GetSection(nameof(TelegramBotOptions)).Get<TelegramBotOptions>() ?? throw new KeyNotFoundException("Отсутствуют настройки бота");
        
        services.AddPolling<UpdateHandler>()
                .AddBotClient(tBotOptions.Key);
        
        services.TryAddScoped<IScriptsService, ScriptsService>();
        
        return services;
    }
}