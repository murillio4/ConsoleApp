using ConsoleApp.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    public static class ConsoleAppBuilderExtensions
    {
        public static IConsoleAppBuilder<T> ConfigureLogging<T>(this IConsoleAppBuilder<T> builder, Action<IHostEnvironment, ILoggingBuilder> configureLogging)
            where T : class
        {
            return builder.ConfigureServices((config, env, collection) => collection.AddLogging(builder => configureLogging(env, builder)));
        }

        public static IConsoleAppBuilder<T> ConfigureLogging<T>(this IConsoleAppBuilder<T> builder, Action<ILoggingBuilder> configureLogging)
            where T : class
        {
            return builder.ConfigureServices((config, env, collection) => collection.AddLogging(builder => configureLogging(builder)));
        }

        public static IConsoleAppBuilder<T> UseEnvironment<T>(this IConsoleAppBuilder<T> hostBuilder, string environment)
            where T : class
        {
            return hostBuilder.ConfigureApp(configBuilder =>
            {
                configBuilder.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>(HostDefaults.EnvironmentKey,
                        environment  ?? throw new ArgumentNullException(nameof(environment)))
                });
            });
        }
    }
}
