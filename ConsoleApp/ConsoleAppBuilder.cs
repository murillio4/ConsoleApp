using ConsoleApp.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ConsoleApp
{
    public class ConsoleAppBuilder<TApplication, TImplementation> : IConsoleAppBuilder<TApplication> 
        where TApplication : class where TImplementation : class, TApplication
    {
        private List<Action<IConfigurationBuilder>> _configureAppActions = new List<Action<IConfigurationBuilder>>();
        private List<Action<IConfiguration, IHostEnvironment, IServiceCollection>> _configureServicesActions = new List<Action<IConfiguration, IHostEnvironment, IServiceCollection>>();
        private IServiceProviderFactory<IServiceCollection> _serviceProviderFactory = new DefaultServiceProviderFactory();

        private bool _built;
        private IConfiguration _appConfiguration;
        private HostingEnvironment _hostingEnvironment;
        private IServiceProvider _appServices;

        public IConsoleAppBuilder<TApplication> ConfigureApp(Action<IConfigurationBuilder> configureDelegate)
        {
            _configureAppActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }
        public IConsoleAppBuilder<TApplication> ConfigureServices(Action<IConfiguration, IHostEnvironment, IServiceCollection> configureDelegate)
        {
            _configureServicesActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
            return this;
        }

        public TApplication Build()
        {
            if (_built)
            {
                throw new InvalidOperationException("Build can only be called once.");
            }
            _built = true;

            //ConfigureLogging();
            BuildAppConfiguration();
            CreateHostingEnvironment();
            CreateServiceProvider();

            return _appServices.GetRequiredService<TApplication>();
        }

        private void CreateHostingEnvironment()
        {
            _hostingEnvironment = new HostingEnvironment()
            {
                ApplicationName = _appConfiguration[HostDefaults.ApplicationKey],
                EnvironmentName = _appConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production,
                ContentRootPath = ResolveContentRootPath(_appConfiguration[HostDefaults.ContentRootKey], AppContext.BaseDirectory),
            };

            if (string.IsNullOrEmpty(_hostingEnvironment.ApplicationName))
            {
                _hostingEnvironment.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;
            }

            _hostingEnvironment.ContentRootFileProvider = new PhysicalFileProvider(_hostingEnvironment.ContentRootPath);
        }

        private string ResolveContentRootPath(string contentRootPath, string basePath)
        {
            if (string.IsNullOrEmpty(contentRootPath))
            {
                return basePath;
            }
            if (Path.IsPathRooted(contentRootPath))
            {
                return contentRootPath;
            }
            return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
        }

        private void BuildAppConfiguration()
        {
            var configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection();

            foreach (var buildAction in _configureAppActions)
            {
                buildAction(configBuilder);
            }
            _appConfiguration = configBuilder.Build();
        }

        private void CreateServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IHostEnvironment>(_hostingEnvironment);
            services.AddSingleton(_ => _appConfiguration);

            services.AddSingleton<TApplication, TImplementation>();
            services.AddOptions();
            services.AddLogging();

            foreach (var configureServicesAction in _configureServicesActions)
            {
                configureServicesAction(_appConfiguration, _hostingEnvironment, services);
            }

            _appServices = services.BuildServiceProvider();

            _ = _appServices.GetService<IConfiguration>();
        }
    }

    public class ConsoleAppBuilder<TApplication> : ConsoleAppBuilder<TApplication, TApplication>
        where TApplication : class
    {}
}
