using System.ComponentModel.DataAnnotations;
using MathBot.Bll.Interfaces;
using MathBot.Contracts;
using MathBot.Dal.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using SomeCompany.Common;

namespace MathBot.Bll.Implementations;

public class ScriptsService : IScriptsService
{
    private const string Return = "return";
    private const string NoResult = "Скрипт не вернул результат";
    private readonly IScriptsProvider _scriptsProvider;
    
    public ScriptsService(IScriptsProvider scriptsProvider)
    {
        _scriptsProvider = scriptsProvider ?? throw new ArgumentNullException(nameof(scriptsProvider));
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, string>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _scriptsProvider.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<string> ExecuteAsync(string commandToExecute, CancellationToken cancellationToken = default)
    {
        var args = commandToExecute.Split(Separators.Whitespace);

        var command = args[0].Replace("/", string.Empty);
        if (string.IsNullOrEmpty(command))
        {
            throw new KeyNotFoundException($"Скрипт с ключом {command} не найден");
        }
        
        var script = await _scriptsProvider.GetValueAsync(command, cancellationToken);
        var parameters = args.Skip(1);
        var result = await CSharpScript.EvaluateAsync(script, globals: new ScriptParams(parameters), cancellationToken: cancellationToken);
        var response = result?.ToString() ?? NoResult;
        
        return response;
    }

    /// <inheritdoc/>
    public async Task<string> AddAsync(string commandToAdd, CancellationToken cancellationToken = default)
    {
        var endOfFirstLine = commandToAdd.IndexOf(Separators.NewLine, StringComparison.Ordinal);
        if (endOfFirstLine == -1)
        {
            throw new ValidationException("Не удалось распознать команду");
        }
        
        var commandName = commandToAdd.Substring(0, endOfFirstLine).ToLower();
        var commandScript = commandToAdd.Substring(endOfFirstLine + Separators.NewLine.Length);
        
        if (string.IsNullOrEmpty(commandScript))
        {
            throw new ValidationException("Скрипт пуст, либо не распознан");
        }
        
        if (!commandScript.Contains(Return))
        {
            throw new ValidationException("Скрипт не возвращает результат");
        }

        var keyVal = new KeyValuePair<string, string>(commandName, commandScript);
        await _scriptsProvider.SetValueAsync(keyVal, cancellationToken);
        
        return $"Команда {commandName} успешно добавлена";
    }
}
