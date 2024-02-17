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
        var tBotOptions = config.GetSection(nameof(TelegramBotOptions)).Get<TelegramBotOptions>();
        
        services.AddPolling<UpdateHandler>()
                .AddBotClient(tBotOptions.Key);

        services.Configure<BotCommandsMenuOptions>(config.GetSection(nameof(BotCommandsMenuOptions)));
        
        // TODO: где интерфейс?
        services.TryAddScoped<BotCommands>();
        
        return services;
    }
}