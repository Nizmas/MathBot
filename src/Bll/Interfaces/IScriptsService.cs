namespace MathBot.Bll.Interfaces;

public interface IScriptsService
{
    Task<IDictionary<string, string>> GetAllAsync();
    
    Task<string> ExecuteAsync(string commandToExecute);

    Task<string> AddAsync(string commandToAdd);
}