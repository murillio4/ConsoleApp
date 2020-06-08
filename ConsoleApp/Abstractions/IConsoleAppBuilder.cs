using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Abstractions
{
    public interface IConsoleAppBuilder<TApplication>
        where TApplication : class
    {
        public TApplication Build();
        public IConsoleAppBuilder<TApplication> ConfigureApp(Action<IConfigurationBuilder> configureDelegate);
        public IConsoleAppBuilder<TApplication> ConfigureServices(Action<IConfiguration, IHostEnvironment, IServiceCollection> configureDelegate);
    }
}
