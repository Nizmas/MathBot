using System;
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
public class ScriptsServiceTestsTests
{
    private const string Prefix = "MathBot";
    private const string CorrectValue = "return \"RESULT\"";
    private const string IncorrectValueWoReturn = "throw";
    private const string FirstKey = "FirstKey";
    private const string SecondKey = "SecondKey";
    private readonly IScriptsProvider _scriptsProvider;
    private readonly Mock<IConsulClient> _consulClientMock = new ();
    private readonly Mock<IOptions<ConsulOptions>> _consulOptionsMock = new ();
    
    public ScriptsServiceTestsTests()
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
}