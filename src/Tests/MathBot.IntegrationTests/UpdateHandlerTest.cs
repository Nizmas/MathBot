using Consul;
using MathBot.Bll.Implementations;
using MathBot.Bll.Interfaces;
using MathBot.Dal.Implementations;
using MathBot.Dal.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SomeCompany.Common;
using Telegram.Bots;
using Telegram.Bots.Extensions.Polling;
using Telegram.Bots.Http;
using Telegram.Bots.Types;

namespace MathBot.IntegrationTests;

[TestFixture]
public class UpdateHandlerTest
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MessageUpdate _messageUpdate;
    private Mock<IConsulClient> _consulClientMock;
    private Mock<IBotClient> _botClientMock;
    
    public UpdateHandlerTest()
    {
        _messageUpdate = new MessageUpdate() { Data = new TextMessage() { Text = "Ok" } };
        
        var serviceCollection = TestsHelpers.GetDefaultServiceCollection();
        
        serviceCollection.Configure<ConsulOptions>(cO => cO.Prefix = "MathBot" );
        serviceCollection.TryAddSingleton<IUpdateHandler, UpdateHandler>();
        serviceCollection.TryAddScoped<IScriptsService, ScriptsService>();
        serviceCollection.TryAddScoped<IScriptsProvider, ScriptsProvider>();
        
        serviceCollection.AddScopedMock<IBotClient>(bM => _botClientMock = bM);
        serviceCollection.AddScopedMock<IConsulClient>(cM => _consulClientMock = cM);
        
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Test]
    public async Task Test1()
    {
        var updateHandler = _serviceProvider.GetRequiredService<IUpdateHandler>();
        //var updateMsg = _serviceProvider.GetRequiredService<Update>();
        // TODO: настройка _consulClient.KV.List()

        await updateHandler.HandleAsync(_botClientMock.Object, _messageUpdate, CancellationToken.None);
        Assert.Pass();
    }
}