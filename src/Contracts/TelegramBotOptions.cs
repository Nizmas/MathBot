namespace MathBot.Contracts;

public class TelegramBotOptions
{
    public string Name { get; set; }
    
    public string Key { get; set; }

    public long ChatId { get; set; }

    public long[] Admins { get; set; }
}