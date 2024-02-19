using System.Text;
using Consul;
using MathBot.Dal.Interfaces;
using Microsoft.Extensions.Options;
using SomeCompany.Common;

namespace MathBot.Dal.Implementations;

public class ScriptsProvider : IScriptsProvider
{
    private const string FolderName = "BotScripts";
    private static readonly Encoding Encoder = Encoding.UTF8;
    
    private readonly IConsulClient _consulClient;
    private readonly ConsulOptions _consulOptions;
    
    public ScriptsProvider(IConsulClient consulClient, IOptions<ConsulOptions> consulOptions)
    {
        _consulClient = consulClient ?? throw new ArgumentNullException(nameof(consulClient));
        _consulOptions = consulOptions?.Value ?? throw new ArgumentNullException(nameof(consulOptions));
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, string>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var consulKey = $"{_consulOptions.Prefix}/{FolderName}";
        var readResult = await _consulClient.KV.List(consulKey, cancellationToken);

        if (readResult?.Response == null || !readResult.Response.Any())
        {
            throw new KeyNotFoundException($"Ключ-папка {consulKey} не найдена или пуста");
        }

        var keyValDict = new Dictionary<string, string>();
        foreach (var kvPair in readResult.Response.Skip(1))
        {
            var key = kvPair.Key.Replace(consulKey, string.Empty);
            var value = Encoder.GetString(kvPair.Value, 0, kvPair.Value.Length);
            keyValDict.Add(key, value);
        }

        return keyValDict;
    }

    /// <inheritdoc/>
    public async Task<string> GetValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var readResult = await _consulClient.KV.Get($"{_consulOptions.Prefix}/{FolderName}/{key}", cancellationToken);

        if (readResult?.Response == null)
        {
            throw new KeyNotFoundException($"Скрипт с ключом {key} не найден");
        }

        var kvPairVal = readResult.Response.Value;
        return Encoder.GetString(kvPairVal, 0, kvPairVal.Length);
    }
    
    /// <inheritdoc/>
    public async Task SetValueAsync(KeyValuePair<string, string> keyValuePair, CancellationToken cancellationToken = default)
    {
        var key = $"{_consulOptions.Prefix}/{FolderName}/{keyValuePair.Key}";
        
        var keyVal = new KVPair(key)
        {
            Value = Encoder.GetBytes(keyValuePair.Value),
        };
        
        var writeResult = await _consulClient.KV.Put(keyVal, cancellationToken);
        if (!writeResult.Response)
        {
            throw new KeyNotFoundException($"Не удалось сохранить команду {keyValuePair.Key}");
        }
    }
}