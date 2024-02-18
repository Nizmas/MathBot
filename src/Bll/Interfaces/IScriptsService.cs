namespace MathBot.Bll.Interfaces;

public interface IScriptsService
{
    /// <summary>
    /// Получить все пары ключ-значение
    /// </summary>
    /// <param name="cancellationToken">Ключ отмены</param>
    Task<IDictionary<string, string>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все пары ключ-значение
    /// </summary>
    /// <param name="commandToExecute">Команда для запуска соответствующего скрипта</param>
    /// <param name="cancellationToken">Ключ отмены</param>
    Task<string> ExecuteAsync(string commandToExecute, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все пары ключ-значение
    /// </summary>
    /// <param name="commandAndScriptToAdd">Текс с командой и скриптом</param>
    /// <param name="cancellationToken">Ключ отмены</param>
    Task<string> AddAsync(string commandAndScriptToAdd, CancellationToken cancellationToken = default);
}