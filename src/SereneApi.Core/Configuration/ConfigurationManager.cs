using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Extensions;
using SereneApi.Core.Handler;
using SereneApi.Core.Options.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SereneApi.Core.Configuration
{
    public class ConfigurationManager : IConfigurationManager, IDisposable
    {
        private readonly Dictionary<string, ConfigurationFactory> _configurationStore = new();

        public ConfigurationManager()
        {
            GetConfigurationProviders(AppDomain.CurrentDomain.GetAssemblies());

            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;
        }

        public void AmendConfiguration<TConfigurationFactory>(Action<IHandlerConfigurationFactory> factory) where TConfigurationFactory : ConfigurationFactory
        {
            string providerName = typeof(TConfigurationFactory).FullName;

            IHandlerConfigurationFactory configurationFactory = _configurationStore[providerName];

            factory.Invoke(configurationFactory);
        }

        public ApiOptionsFactory<TApiHandler> BuildApiOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler
        {
            string factoryName = GetFactoryName<TApiHandler>();

            return _configurationStore[factoryName].BuildOptionsFactory<TApiHandler>();
        }

        private void GetConfigurationProviders(params Assembly[] assemblies)
        {
            Type providerType = typeof(ConfigurationFactory);

            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetLoadedTypes())
                .Where(t => t.IsSubclassOf(providerType) && !t.IsAbstract);

            foreach (Type type in types)
            {
                ConfigurationFactory factory = (ConfigurationFactory)Activator.CreateInstance(type);

                factory.Dependencies.AddSingleton<IConfigurationManager>(() => this);

                _configurationStore.Add(type.FullName, factory);
            }
        }

        private string GetFactoryName<TApiHandler>() where TApiHandler : IApiHandler
        {
            Type apiHandlerType = typeof(TApiHandler);

            UseConfigurationFactoryAttribute useConfigurationFactory = apiHandlerType.GetCustomAttribute<UseConfigurationFactoryAttribute>();

            return useConfigurationFactory.Name;
        }

        private void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            GetConfigurationProviders(args.LoadedAssembly);
        }

        #region IDisposable

        private volatile bool _disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoaded;

                foreach (ConfigurationFactory factory in _configurationStore.Values)
                {
                    factory.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion IDisposable
    }
}