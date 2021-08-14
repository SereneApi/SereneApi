using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration.Attributes;
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
        private readonly Dictionary<string, ConfigurationProvider> _configurationStore = new();

        public ConfigurationManager()
        {
            GetConfigurationProviders(AppDomain.CurrentDomain.GetAssemblies());

            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;
        }

        public ApiOptionsFactory<TApiHandler> BuildApiOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler
        {
            string providerName = GetProviderName<TApiHandler>();

            return _configurationStore[providerName].BuildOptionsFactory<TApiHandler>();
        }

        public void AmendConfiguration<TApiHandler>(Action<IHandlerConfigurationFactory> factory) where TApiHandler : ConfigurationProvider
        {
            string providerName = typeof(TApiHandler).Name;

            IHandlerConfigurationFactory configurationFactory = _configurationStore[providerName];

            factory.Invoke(configurationFactory);
        }

        private void GetConfigurationProviders(params Assembly[] assemblies)
        {
            Type providerType = typeof(ConfigurationProvider);

            List<Type> types = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(providerType) && !t.IsAbstract)
                .ToList();

            foreach (Type type in types)
            {
                ConfigurationProvider provider = (ConfigurationProvider)Activator.CreateInstance(type);

                provider.AddDependency<IConfigurationManager>(() => this, Lifetime.Singleton, Binding.Unbound);

                _configurationStore.Add(type.Name, provider);
            }
        }

        private void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            GetConfigurationProviders(args.LoadedAssembly);
        }

        private string GetProviderName<TApiHandler>() where TApiHandler : IApiHandler
        {
            Type apiHandlerType = typeof(TApiHandler);

            ConfigurationProviderAttribute attribute = (ConfigurationProviderAttribute)Attribute
                .GetCustomAttribute(apiHandlerType, typeof(ConfigurationProviderAttribute));

            return attribute.ProviderName;
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
            }

            _disposed = true;
        }

        #endregion
    }
}
