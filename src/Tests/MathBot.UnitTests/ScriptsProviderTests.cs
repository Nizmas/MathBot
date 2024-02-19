using System.Text;
using Consul;
using MathBot.Dal.Implementations;
using MathBot.Dal.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SomeCompany.Common;

namespace MathBot.UnitTests;

[TestFixture]
public class ScriptsProviderTests
{
    private const string Prefix = "MathBot";
    private const string CorrectValue = "return \"RESULT\";";
    private const string FirstKey = "FirstKey";
    private const string SecondKey = "SecondKey";
    private readonly IScriptsProvider _scriptsProvider;
    private readonly Mock<IConsulClient> _consulClientMock = new ();
    private readonly Mock<IOptions<ConsulOptions>> _consulOptionsMock = new ();
    
    public ScriptsProviderTests()
    {
        _consulOptionsMock.Setup(cO => cO.Value).Returns(new ConsulOptions() { Prefix = Prefix });
        _scriptsProvider = new ScriptsProvider(_consulClientMock.Object, _consulOptionsMock.Object);
    }
    
    [Test]
    public void GetAllAsync_NullResponse_Exception()
    {
        // Arrange
        _consulClientMock.Setup(cC => cC.KV.List(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((string _, CancellationToken _) => new QueryResult<KVPair[]>());
        
        // Act, Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _scriptsProvider.GetAllAsync(CancellationToken.None));
    }
    
    [Test]
    public async Task GetAllAsync_TwoCommands_SkippedFirstCommand()
    {
        // Arrange
        var kvPairs = new KVPair[]
        {
            new KVPair(FirstKey),
            new KVPair(SecondKey)
            {
                Value = Encoding.UTF8.GetBytes(CorrectValue),
            }
        };
        
        _consulClientMock.Setup(cC => cC.KV.List(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, CancellationToken _) => new QueryResult<KVPair[]>()
            {
                Response = kvPairs
            });
        
        // Act
        var commandsDict = await _scriptsProvider.GetAllAsync(CancellationToken.None);
        var skippedKv = commandsDict.FirstOrDefault(cD => cD.Key == FirstKey);
        
        // Assert
        ClassicAssert.AreEqual(kvPairs.Length, commandsDict.Count + 1);
        ClassicAssert.IsNull(skippedKv.Key);
    }

    [Test]
    public void GetValueAsync_NullResponse_Exception()
    {
        // Arrange
        _consulClientMock.Setup(cC => cC.KV.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((string _, CancellationToken _) => new QueryResult<KVPair>());
        
        // Act, Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _scriptsProvider.GetValueAsync(FirstKey, CancellationToken.None));
    }
    
    [Test]
    public async Task GetValueAsync_Key_Script()
    {
        // Arrange
        _consulClientMock.Setup(cC => cC.KV.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, CancellationToken _) => new QueryResult<KVPair>()
            {
                Response = new KVPair(FirstKey)
                {
                    Value = Encoding.UTF8.GetBytes(CorrectValue),
                }
            });
        
        // Act
        var resScript = await _scriptsProvider.GetValueAsync(FirstKey, CancellationToken.None);
        
        // Assert
        ClassicAssert.AreEqual(CorrectValue, resScript);
    }

    [Test]
    public void SetValueAsync_FalseResponse_Exception()
    {
        // Arrange
        _consulClientMock.Setup(cC => cC.KV.Put(It.IsAny<KVPair>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((KVPair _, CancellationToken _) => new WriteResult<bool>() { Response = false });
        
        // Act, Assert
        Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _scriptsProvider.SetValueAsync(new KeyValuePair<string,  string>(FirstKey, CorrectValue), CancellationToken.None));
    }
    
    [Test]
    public async Task SetValueAsync_Key_Script()
    {
        // Arrange
        var setValue = string.Empty;
        _consulClientMock.Setup(cC => cC.KV.Put(It.IsAny<KVPair>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((KVPair kvPair, CancellationToken _) =>
            {
                setValue = Encoding.UTF8.GetString(kvPair.Value, 0, kvPair.Value.Length);
                return new WriteResult<bool>() { Response = true };
            });
        
        // Act
        await _scriptsProvider.SetValueAsync(new KeyValuePair<string,  string>(FirstKey, CorrectValue), CancellationToken.None);
        
        // Assert
        ClassicAssert.AreEqual(CorrectValue, setValue);
    }
}