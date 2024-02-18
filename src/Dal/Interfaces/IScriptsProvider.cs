namespace MathBot.Dal.Interfaces;

public interface IScriptsProvider
{
    /// <summary>
    /// Получить все имеющиеся пары ключ-значение
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<IDictionary<string, string>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить скрипт по ключу
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<string> GetValueAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Присвоить значение по ключу
    /// </summary>
    /// <param name="keyValuePair">Пара ключ-значение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SetValueAsync(KeyValuePair<string, string> keyValuePair, CancellationToken cancellationToken = default);
}