namespace MathBot.Contracts;

/// <summary>
/// Объект для хранения команд
/// </summary>
public class BotCommandsMenuOptions
{
    /// <summary>
    /// Словарь, где ключом является команда для бота (без знака /)
    /// в качестве значений присваиваются скрипты на языке csharp
    /// </summary>
    public Dictionary<string, string> CommandAndScript { get; set; }
}