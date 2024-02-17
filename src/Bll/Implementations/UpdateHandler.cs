using Microsoft.Extensions.DependencyInjection;
using Telegram.Bots;
using Telegram.Bots.Extensions.Polling;
using Telegram.Bots.Requests;
using Telegram.Bots.Types;

namespace MathBot.Bll.Implementations;

/// <summary>
/// Прослушиватель сообщений телеграм-бота
/// </summary>
public class UpdateHandler : IUpdateHandler
{
    private const string DefaultLang = "csharp";
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Инициализация <see cref="UpdateHandler" />
    /// </summary>
    /// <param name="serviceProvider">Провайдер сервисов для IoC</param>
    public UpdateHandler(
        IServiceProvider serviceProvider
    )
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task HandleAsync(IBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update is MessageUpdate { Data: TextMessage textMsg })
        {
            // _logger.LogInformation($"Принято сообщение от {textMsg.From.Username}: {textMsg.Text}");
            using var scope = _serviceProvider.CreateScope();
            var botCommands = scope.ServiceProvider.GetRequiredService<BotCommands>();
            var msgEntity = textMsg.Entities?.FirstOrDefault();
                
            var response = msgEntity?.Type switch
            {
                MessageEntityType.BotCommand =>
                    await botCommands.QueryAsync(textMsg.Text),
                MessageEntityType.Pre when msgEntity.Language == DefaultLang => await UpdateBotMenuAsync(),
                    //await botCommands.AddAsync(textMsg.Text),
                _ => "Ясно",
            };
            await bot.HandleAsync(new SendText(textMsg.Chat.Id, response), cancellationToken);
            
            async Task<string> UpdateBotMenuAsync()
            {
                return await botCommands.CommandAsync(textMsg.Text);
            }
        }
        
    }
}