using System.Text;
using Consul;
using MathBot.Bll.Implementations;
using MathBot.Bll.Interfaces;
using MathBot.Dal.Implementations;
using MathBot.Dal.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NUnit.Framework;
using SomeCompany.Common;
using Telegram.Bots;
using Telegram.Bots.Extensions.Polling;
using Telegram.Bots.Requests;

namespace MathBot.IntegrationTests;

[TestFixture]
public class UpdateHandlerTest
{
    private const string Prefix = "MathBot";
    private const string FirstKey = "firstkey";
    private const string SecondKey = "secondkey";
    private const string Result = "RESULT";
    private const string CorrectValue = $"return \"{Result}\";";
    private readonly IServiceProvider _serviceProvider;
    private readonly KVPair[] _kvPairs;
    private Mock<IConsulClient> _consulClientMock;
    private Mock<IBotClient> _botClientMock;
    private Mock<IKVEndpoint> _kvEndpointMock;
    
    public UpdateHandlerTest()
    {
        var serviceCollection = TestsHelpers.GetDefaultServiceCollection();
        
        serviceCollection.Configure<ConsulOptions>(cO => cO.Prefix = "MathBot" );
        
        serviceCollection.TryAddSingleton<IUpdateHandler, UpdateHandler>();
        serviceCollection.TryAddScoped<IScriptsService, ScriptsService>();
        serviceCollection.TryAddScoped<IScriptsProvider, ScriptsProvider>();
        
        serviceCollection.AddScopedMock<IBotClient>(bM => _botClientMock = bM);
        serviceCollection.AddScopedMock<IConsulClient>(cM => _consulClientMock = cM);
        serviceCollection.AddScopedMock<IKVEndpoint>(kvE => _kvEndpointMock = kvE);
        
        _serviceProvider = serviceCollection.BuildServiceProvider();

        _consulClientMock!.SetupGet(cM => cM.KV)
            .Returns(_kvEndpointMock!.Object);
        
        _kvPairs = new KVPair[]
        {
            new KVPair(Prefix)
            {
                Value = Encoding.UTF8.GetBytes(CorrectValue),
            },
            new KVPair(FirstKey)
            {
                Value = Encoding.UTF8.GetBytes(CorrectValue),
            },
            new KVPair(SecondKey)
            {
                Value = Encoding.UTF8.GetBytes(CorrectValue),
            }
        };
    }

    [Test]
    public void Constructor_ThreeKeyValue_FirstSkipped()
    {
        // Arrange
        _kvEndpointMock.Setup(kvM => kvM.List(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync((string _, CancellationToken _) => new QueryResult<KVPair[]>() {Response = _kvPairs});
        
        // Act
        _serviceProvider.GetRequiredService<IUpdateHandler>();
        
        // Assert
        _botClientMock.Verify(
            c => c.HandleAsync(
                It.Is<SetMyCommands>(smC => _kvPairs.Length == smC.Commands.Count() + 1 && !smC.Commands.Any(c => c.Command == Prefix)),
                It.IsAny<CancellationToken>()),
            Times.Once());
    }
}