using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MathBot.Console.Extensions;

/// <summary>
/// Расширения для запуска приложения
/// </summary>
internal static class HostExtension
{
    /// <summary>
    /// Использование класса Startup
    /// </summary>
    /// <param name="builderContext">Билдер приложения</param>
    /// <param name="serviceCollection">Коллекция сервисов</param>
    internal static void UseStartup(this HostBuilderContext builderContext, IServiceCollection serviceCollection)
    {
        var startup = new Startup(builderContext.Configuration);
        startup.ConfigureServices(serviceCollection);
    }
}