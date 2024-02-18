namespace MathBot.Dal.Interfaces;

public interface IScriptsProvider
{
    /// <summary>
    /// Получить все имеющиеся пары ключ-значение
    /// </summary>
    Task<IDictionary<string, string>> GetAllAsync();
    
    /// <summary>
    /// Получить скрипт по ключу
    /// </summary>
    /// <param name="key">Ключ</param>
    Task<string> GetValueAsync(string key);
    
    /// <summary>
    /// Присвоить значение по ключу
    /// </summary>
    /// <param name="keyValuePair">Пара ключ-значение</param>
    Task SetValueAsync(KeyValuePair<string, string> keyValuePair);
}