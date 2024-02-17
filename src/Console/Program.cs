using SomeCompany.Common.Extensions;
using MathBot.Console.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace MathBot.Console;

static class Program
{
    private static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    /// <summary>
    /// Host builder method
    /// </summary>
    /// <param name="args">Аргументы передаваемые при запуске приложения</param>
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                const string prefix = "MATH_";
                var env = hostingContext.HostingEnvironment;
                config.AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json")
                    .AddEnvironmentVariables(prefix)
                    .AddConsul(env.ApplicationName, prefix);

                if (args.Any())
                {
                    config.AddCommandLine(args);
                }
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddNLog();
            })
            .ConfigureServices((hostContext, sc) =>
            {
                hostContext.UseStartup(sc);
            });
}