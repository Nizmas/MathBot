using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MathBot.Console.Extensions;

internal static class HostExtension
{
    internal static void UseStartup(this HostBuilderContext builderContext, IServiceCollection serviceCollection)
    {
        var startup = new Startup(builderContext.Configuration);
        startup.ConfigureServices(serviceCollection);
    }
}