using MathBot.Bll.Implementations.Handlers;
using MathBot.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bots;
using Telegram.Bots.Extensions.Polling;
using Telegram.Bots.Requests;
using Telegram.Bots.Types;

namespace MathBot.Bll.Implementations.Extensions;

public static class MathBotBllExtension
{
    public static IServiceCollection RegistrationMathBotBll(this IServiceCollection services, IConfiguration config)
    {
        var tBotOptions = config.GetSection(nameof(TelegramBotOptions)).Get<TelegramBotOptions>();
        
        var provider = services.AddPolling<UpdateHandler>()
            .AddBotClient(tBotOptions.Key)
            .Services
            .BuildServiceProvider();

        services.Configure<BotCommandsMenuOptions>(config.GetSection(nameof(BotCommandsMenuOptions)));
        
        // TODO: где интерфейс?
        services.AddTransient<BotCommands>();
        // services.TryAddTransient<ITelegramClient, TelegramClient>();
        return services;
    }
}