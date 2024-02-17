using MathBot.Contracts;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.Extensions.Options;

namespace MathBot.Bll.Implementations;

public class BotCommands : IDisposable
{
    private const string NewLine = "\n";
    private static IOptions<BotCommandsMenuOptions> _commandsAndScript { get; set; }
    
    public BotCommands(IOptions<BotCommandsMenuOptions> commandMenuOptions)
    {
        _commandsAndScript ??= commandMenuOptions ?? throw new ArgumentNullException(nameof(commandMenuOptions));
    }

    public async Task<string> ExecuteAsync(string commandToExecute)
    {
        var args = commandToExecute.Split(" ");

        var command = args[0].Replace("/", string.Empty);
        if (string.IsNullOrEmpty(command))
        {
            return "Не удалось";
        }
        
        if (!_commandsAndScript.Value.CommandAndScript.TryGetValue(command, out var script))
        {
            return "Не удалось";
        }

        var parameters = args.Skip(1);
        var result = await CSharpScript.EvaluateAsync(script, globals: new ScriptParams(parameters));
        return result.ToString();

        // string script = "string TryUpper(string str) { return str.ToUpper(); } return TryUpper(args);";
        
        //string script = "return double.Parse(args[0]) + double.Parse(args[1]);\n";
    }

    public async Task<string> AddAsync(string commandToAdd)
    {
        var endOfFirstLine = commandToAdd.IndexOf(NewLine, StringComparison.Ordinal);
        if(endOfFirstLine == default)
            return "Не удалось";
        
        var commandName = commandToAdd.Substring(0, endOfFirstLine);
        var commandScript = commandToAdd.Substring(endOfFirstLine + NewLine.Length);
        
        _commandsAndScript.Value.CommandAndScript.TryAdd($"{commandName}", commandScript);
        return "Удалось";
    }
    
    public void Dispose()
    {
        //throw new NotImplementedException();
    }
}
