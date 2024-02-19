using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace SomeCompany.Common;

public static class TestsHelpers
{
    public static IServiceCollection GetDefaultServiceCollection()
    {
        var serviceCollection = new ServiceCollection();
        var loggerFactoryMoq = new Mock<ILoggerFactory>();
        loggerFactoryMoq.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(Mock.Of<ILogger>);

        serviceCollection.AddSingleton(loggerFactoryMoq.Object);
        serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        return serviceCollection;
    }
    
    public static IServiceCollection AddScopedMock<T>(this IServiceCollection sc, Action<Mock<T>>? cfg = null)
        where T : class
    {
        var mock = new Mock<T>();
        cfg?.Invoke(mock);

        sc.AddScoped((p) => mock.Object);
        return sc;
    }
}