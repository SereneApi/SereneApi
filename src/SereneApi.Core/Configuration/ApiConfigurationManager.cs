using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Configuration.Exceptions;
using SereneApi.Core.Configuration.Provider;
using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SereneApi.Core.Configuration
{
    public sealed class ApiConfigurationManager : IDisposable
    {
        private readonly Dictionary<Type, ApiConfigurationScope> _configurationScope = new();
        private readonly object _configurationScopeLock = new();
        private readonly Dictionary<Type, ApiConfigurationFactory> _configurationStore = new();
        private readonly object _configurationStoreLock = new();
        private readonly Action<IApiConfiguration> _onConfigurationProviderRegistered;

        public ApiConfigurationManager(Action<IApiConfiguration> onConfigurationProviderRegistered = null)
        {
            _onConfigurationProviderRegistered = onConfigurationProviderRegistered;

            GetConfigurationProviders(AppDomain.CurrentDomain.GetAssemblies());

            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;
        }

        public IApiSettings<TApiHandler> BuildApiOptions<TApiHandler>() where TApiHandler : IApiHandler
        {
            Type handlerType = typeof(TApiHandler);
            Type providerType = GetConfigurationProviderType<TApiHandler>();

            ApiConfigurationScope scope;

            lock (_configurationScopeLock)
            {
                if (!_configurationScope.TryGetValue(handlerType, out scope))
                {
                    scope = _configurationStore[providerType].CreateScope(typeof(TApiHandler));

                    _configurationScope.Add(handlerType, scope);
                }
            }

            return scope.BuildApiSettings<TApiHandler>();
        }

        public IApiSettings BuildApiOptions(Type handlerType)
        {
            Type providerType = GetConfigurationProviderType(handlerType);

            ApiConfigurationScope scope;

            lock (_configurationScopeLock)
            {
                if (!_configurationScope.TryGetValue(handlerType, out scope))
                {
                    scope = _configurationStore[providerType].CreateScope(handlerType);

                    _configurationScope.Add(handlerType, scope);
                }
            }

            return scope.BuildApiSettings(handlerType);
        }

        #region HandlerConfiguraion

        public void AmendConfigurationProvider<TProvider>(Action<IApiConfiguration> configuration) where TProvider : HandlerConfigurationProvider
        {
            ApiConfigurationFactory factory;

            lock (_configurationStoreLock)
            {
                if (!_configurationStore.TryGetValue(typeof(TProvider), out factory))
                {
                    throw new ProviderNotFoundException(typeof(TProvider));
                }
            }

            configuration.Invoke(factory);
        }

        #endregion HandlerConfiguraion

        #region ApiConfiguration

        public void AddApiConfiguration<TApiHandler>(Action<IApiConfiguration> configuration) where TApiHandler : IApiHandler
        {
            AddApiConfiguration(typeof(TApiHandler), configuration);
        }

        public void AddApiConfiguration(Type handlerType, Action<IApiConfiguration> configuration)
        {
            ApiConfigurationFactory factory = GetConfigurationFactory(handlerType);

            factory.AddApiConfiguration(handlerType, configuration);
        }

        public void AddApiPostConfiguration(Type handlerType, Action<IApiConfiguration> configuration)
        {
            ApiConfigurationFactory factory = GetConfigurationFactory(handlerType);

            factory.AddApiPostConfiguration(handlerType, configuration);
        }

        public void AddApiPostConfiguration<TApiHandler>(Action<IApiConfiguration> configuration) where TApiHandler : IApiHandler
        {
            AddApiPostConfiguration(typeof(TApiHandler), configuration);
        }

        public void AddApiPreConfiguration(Type handlerType, Action<IApiConfiguration> configuration)
        {
            ApiConfigurationFactory factory = GetConfigurationFactory(handlerType);

            factory.AddApiPreConfiguration(handlerType, configuration);
        }

        public void AddApiPreConfiguration<TApiHandler>(Action<IApiConfiguration> configuration) where TApiHandler : IApiHandler
        {
            AddApiPreConfiguration(typeof(TApiHandler), configuration);
        }

        public void AddOnConfigurationInitialization(Type handlerType, Action<IApiOnInitialization> configuration)
        {
            ApiConfigurationFactory factory = GetConfigurationFactory(handlerType);

            factory.AddOnScopeInitialization(handlerType, configuration);
        }

        public void AddOnConfigurationInitialization<TApiHandler>(Action<IApiOnInitialization> configuration) where TApiHandler : IApiHandler
        {
            AddOnConfigurationInitialization(typeof(TApiHandler), configuration);
        }

        #endregion ApiConfiguration

        #region Private Methods

        private static Type GetConfigurationProviderType(Type handlerType)
        {
            UseHandlerConfigurationProviderAttribute useProvider = handlerType.GetCustomAttribute<UseHandlerConfigurationProviderAttribute>();

            if (useProvider == null)
            {
                throw new HandlerNullProviderException(handlerType);
            }

            return useProvider.Type;
        }

        private static IEnumerable<Type> GetConfigurationProviderTypes(Assembly[] assemblies)
        {
            Type providerType = typeof(HandlerConfigurationProvider);

            return assemblies.SelectMany(a => a.GetLoadedTypes()).Where(t => t.IsSubclassOf(providerType) && !t.IsAbstract);
        }

        private ApiConfigurationFactory GetConfigurationFactory(Type handlerType)
        {
            Type providerType = GetConfigurationProviderType(handlerType);

            lock (_configurationStoreLock)
            {
                return _configurationStore[providerType];
            }
        }

        private void GetConfigurationProviders(params Assembly[] assemblies)
        {
            foreach (Type type in GetConfigurationProviderTypes(assemblies))
            {
                HandlerConfigurationProvider configurationProvider = (HandlerConfigurationProvider)Activator.CreateInstance(type);

                if (configurationProvider == null)
                {
                    throw new ProviderNotInstantiatedException(type);
                }

                _onConfigurationProviderRegistered?.Invoke(configurationProvider);

                configurationProvider.Dependencies.AddSingleton(() => this);

                lock (_configurationStoreLock)
                {
                    _configurationStore.Add(type, new ApiConfigurationFactory(configurationProvider));
                }
            }
        }

        private Type GetConfigurationProviderType<TApiHandler>() where TApiHandler : IApiHandler
        {
            return GetConfigurationProviderType(typeof(TApiHandler));
        }

        private void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            GetConfigurationProviders(args.LoadedAssembly);
        }

        #endregion Private Methods

        #region IDisposable

        private volatile bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoaded;

            foreach (ApiConfigurationScope scope in _configurationScope.Values)
            {
                scope.Dispose();
            }

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}