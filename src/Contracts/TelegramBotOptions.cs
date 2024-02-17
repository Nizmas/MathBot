namespace MathBot.Contracts;

/// <summary>
/// Опции для подключения клиента телеграм-бота
/// </summary>
public class TelegramBotOptions
{
    /// <summary>
    /// Имя бота (в данном проекте не актуально)
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Ключ доступа к API телеграм-бота
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Список доверенных пользователей
    /// (по-хорошему новые команды должны добавлять только администраторы)
    /// </summary>
    public long[] Admins { get; set; }
}