using System.Text;
using System.Text.Json;
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

    public async Task<IDictionary<string, string>> GetAllAsync()
    {
        var readResult = await _consulClient.KV.List($"{_consulOptions.Prefix}/{FolderName}");

        if (readResult?.Response == null || !readResult.Response.Any())
        {
            throw new ConsulRequestException();
        }

        var keyValDict = new Dictionary<string, string>();
        foreach (var kvPair in readResult.Response.Skip(1))
        {
            var key = kvPair.Key.Replace($"{_consulOptions.Prefix}/{FolderName}", string.Empty);
            var value = Encoder.GetString(kvPair.Value, 0, kvPair.Value.Length);
            keyValDict.Add(key, value);
        }

        return keyValDict;
    }

    /// <inheritdoc/>
    public async Task<string> GetValueAsync(string key)
    {
        var readResult = await _consulClient.KV.Get($"{_consulOptions.Prefix}/{FolderName}/{key}");

        if (readResult?.Response == null)
        {
            throw new KeyNotFoundException($"Скрипт с ключом {key} не найден");
        }

        var kvPairVal = readResult.Response.Value;
        return Encoder.GetString(kvPairVal, 0, kvPairVal.Length);
    }
    
    /// <inheritdoc/>
    public async Task SetValueAsync(KeyValuePair<string, string> keyValuePair)
    {
        var key = $"{_consulOptions.Prefix}/{FolderName}/{keyValuePair.Key}";
        
        var keyVal = new KVPair(key)
        {
            Value = Encoder.GetBytes(keyValuePair.Value),
        };
        
        var writeResult = await _consulClient.KV.Put(keyVal);
        if (!writeResult.Response)
        {
            throw new ConsulRequestException();
        }
    }
}