using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Handler;
using SereneApi.Core.Options.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SereneApi.Core.Configuration
{
    public static class ConfigurationManager
    {
        private static readonly Dictionary<string, IHandlerConfigurationBuilder> _configurationStore = new();

        static ConfigurationManager()
        {
            GetConfigurationProviders(AppDomain.CurrentDomain.GetAssemblies());

            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
            {
                GetConfigurationProviders(args.LoadedAssembly);
            };
        }

        public static ApiOptionsFactory<TApiHandler> BuildApiOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler
        {
            string providerName = GetProviderName<TApiHandler>();

            return _configurationStore[providerName].BuildOptionsFactory<TApiHandler>();
        }

        public static void AmendConfiguration<TApiHandler>(Action<IHandlerConfigurationFactory> factory) where TApiHandler : IApiHandler
        {
            string providerName = GetProviderName<TApiHandler>();

            IHandlerConfigurationFactory configurationFactory = (IHandlerConfigurationFactory)_configurationStore[providerName];

            factory.Invoke(configurationFactory);
        }

        private static void GetConfigurationProviders(params Assembly[] assemblies)
        {
            Type providerType = typeof(ConfigurationProvider);

            List<Type> types = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(providerType) && !t.IsAbstract)
                .ToList();

            foreach (Type type in types)
            {
                ConfigurationProvider provider = (ConfigurationProvider)Activator.CreateInstance(type);

                _configurationStore.Add(type.Name, provider);
            }
        }

        private static string GetProviderName<TApiHandler>() where TApiHandler : IApiHandler
        {
            Type apiHandlerType = typeof(TApiHandler);

            ConfigurationProviderAttribute attribute = (ConfigurationProviderAttribute)Attribute
                .GetCustomAttribute(apiHandlerType, typeof(ConfigurationProviderAttribute));

            return attribute.ProviderName;
        }
    }
}
