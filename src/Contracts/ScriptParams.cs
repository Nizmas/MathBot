namespace MathBot.Contracts;

public class ScriptParams
{
    public string[] args;

    public ScriptParams(IEnumerable<string> args)
    {
        this.args = args.ToArray();
    }
}