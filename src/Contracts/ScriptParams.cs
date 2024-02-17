namespace MathBot.Contracts;

/// <summary>
/// Класс для передачи параметров в скрипты бота
/// </summary>
public class ScriptParams
{
    private ScriptParams() {}
    
    /// <summary>
    /// Массив аргументов для скрипта
    /// По аналогии с Main принимает массив строк (нейминг нарушен с той же целью)
    /// </summary>
    public string[] args { get; set; }
    
    /// <summary>
    /// Конструктор для инициализации <see cref="ScriptParams" />
    /// </summary>
    /// <param name="args">Коллекция параметров для выполнения скрипта</param>
    public ScriptParams(IEnumerable<string> args)
    {
        this.args = args.ToArray();
    }
}