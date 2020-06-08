using ConsoleApp.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace ConsoleApp
{
    public static class ConsoleApp
    {
        public static IConsoleAppBuilder<TApplication> DefaultBuilder<TApplication, TImplementation>(string[] args)
            where TApplication : class where TImplementation : class, TApplication
        {
            var builder = new ConsoleAppBuilder<TApplication, TImplementation>();

            builder.ConfigureApp(config =>
            {
                config.AddEnvironmentVariables();
                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            }).ConfigureLogging((logging) =>
            {
                var logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(
                        theme: AnsiConsoleTheme.Code)
                    .CreateLogger();

                logging.AddSerilog(logger, true);
                logging.AddEventSourceLogger();
            });

            return builder;
        }

        public static IConsoleAppBuilder<TApplication> DefaultBuilder<TApplication, TImplementation>()
            where TApplication : class where TImplementation : class, TApplication
        {
            return DefaultBuilder<TApplication, TImplementation>(null);
        }

        public static IConsoleAppBuilder<TApplication> DefaultBuilder<TApplication>(string[] args)
            where TApplication : class
        {
            return DefaultBuilder<TApplication, TApplication>(args);
        }

        public static IConsoleAppBuilder<TApplication> DefaultBuilder<TApplication>()
            where TApplication : class
        {
            return DefaultBuilder<TApplication, TApplication>(null);
        }
    }
}
