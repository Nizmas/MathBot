using System.ComponentModel.DataAnnotations;
using MathBot.Bll.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SomeCompany.Common;
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
        UpdateBotMenuAsync();
    }

    public async Task HandleAsync(IBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update is MessageUpdate { Data: TextMessage textMsg })
        {
            // _logger.LogInformation($"Принято сообщение от {textMsg.From.Username}: {textMsg.Text}");
            string response;
            
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var scriptsService = scope.ServiceProvider.GetRequiredService<IScriptsService>();
                var msgEntity = textMsg.Entities?.FirstOrDefault();

                response = msgEntity?.Type switch
                {
                    MessageEntityType.BotCommand =>
                        await scriptsService.ExecuteAsync(textMsg.Text),
                    
                    MessageEntityType.Pre when msgEntity.Language == DefaultLang => 
                        await UpdateBotMenuAsync(botClient, scriptsService, textMsg.Text),
                    
                    _ => "Проверьте правильность отправленных данных.",
                };
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ValidationException)
            {
                // _logger.LogWarning(ex);
                response = ex.Message;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex);
                response = ex.Message;
            }
            
            await botClient.HandleAsync(new SendText(textMsg.Chat.Id, response), cancellationToken);
        }
    }

    private async Task<string> UpdateBotMenuAsync(IBotClient? botClient = null, IScriptsService? scriptsService = null, string? commandConfig = null)
    {
        using var scope = _serviceProvider.CreateScope();
        scriptsService ??= scope.ServiceProvider.GetRequiredService<IScriptsService>();
        botClient ??= scope.ServiceProvider.GetRequiredService<IBotClient>();
        
        var response = string.Empty;
        if (!string.IsNullOrEmpty(commandConfig))
        {
            response = await scriptsService.AddAsync(commandConfig);
        }

        var botCommands = new List<BotCommand>();
        var commandScripts = await scriptsService.GetAllAsync();
        foreach (var commandScript in commandScripts)
        {
            var value = commandScript.Value;
            var endOfFirstLine = value.IndexOf(Separators.NewLine, StringComparison.Ordinal);
            var description = endOfFirstLine == -1 ?
                            value :
                            value.Substring(0, endOfFirstLine).Replace("//", string.Empty);
            
            botCommands.Add(new BotCommand()
            {
                Command = commandScript.Key,
                Description = description,
            });
        }
        
        await botClient.HandleAsync
        (
            new SetMyCommands(botCommands)
        );

        return response;
    }
}