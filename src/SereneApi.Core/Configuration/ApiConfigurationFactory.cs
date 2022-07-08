using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Configuration.Exceptions;
using SereneApi.Core.Configuration.Provider;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SereneApi.Core.Configuration
{
    internal sealed class ApiConfigurationFactory
    {
        private readonly List<Action<IApiConfiguration>> _providerConfiguration = new();

        private readonly Dictionary<Type, List<Action<IApiConfiguration>>> _configuration = new();
        private readonly Dictionary<Type, List<Action<IApiOnInitialization>>> _onInitialization = new();
        private readonly Dictionary<Type, List<Action<IApiConfiguration>>> _postConfiguration = new();
        private readonly Dictionary<Type, List<Action<IApiConfiguration>>> _preConfiguration = new();
        //public IDependencyCollection Dependencies { get; } = new DependencyCollection();

        private readonly HandlerConfigurationProvider _configurationProvider;

        public ApiConfigurationFactory(HandlerConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void AmendConfiguration(Action<IApiConfiguration> configuration)
        {
            _providerConfiguration.Add(configuration);
        }

        public void AddApiConfiguration(Type handlerType, Action<IApiConfiguration> configuration)
        {
            ValidateHandler(handlerType);

            if (_configuration.TryGetValue(handlerType, out List<Action<IApiConfiguration>> configurations))
            {
                configurations.Add(configuration);
            }
            else
            {
                configurations = new List<Action<IApiConfiguration>> { configuration };

                _configuration.Add(handlerType, configurations);
            }
        }

        public void AddApiPostConfiguration(Type handlerType, Action<IApiConfiguration> configuration)
        {
            ValidateHandler(handlerType);

            if (_postConfiguration.TryGetValue(handlerType, out List<Action<IApiConfiguration>> configurations))
            {
                configurations.Add(configuration);
            }
            else
            {
                configurations = new List<Action<IApiConfiguration>> { configuration };

                _postConfiguration.Add(handlerType, configurations);
            }
        }

        public void AddApiPreConfiguration(Type handlerType, Action<IApiConfiguration> configuration)
        {
            ValidateHandler(handlerType);

            if (_preConfiguration.TryGetValue(handlerType, out List<Action<IApiConfiguration>> configurations))
            {
                configurations.Add(configuration);
            }
            else
            {
                configurations = new List<Action<IApiConfiguration>> { configuration };

                _preConfiguration.Add(handlerType, configurations);
            }
        }

        public void AddOnScopeInitialization(Type handlerType, Action<IApiOnInitialization> onInitialization)
        {
            if (_onInitialization.TryGetValue(handlerType, out List<Action<IApiOnInitialization>> onInitializations))
            {
                onInitializations.Add(onInitialization);
            }
            else
            {
                onInitializations = new List<Action<IApiOnInitialization>> { onInitialization };

                _onInitialization.Add(handlerType, onInitializations);
            }
        }

        public ApiConfigurationScope CreateScope(Type handlerType)
        {
            ValidateHandler(handlerType);

            ApiConfiguration apiConfiguration = InternalBuildApiConfiguration(handlerType);

            if (_preConfiguration.TryGetValue(handlerType, out List<Action<IApiConfiguration>> preConfigurations))
            {
                foreach (Action<IApiConfiguration> preConfiguration in preConfigurations)
                {
                    preConfiguration.Invoke(apiConfiguration);
                }
            }

            if (_configuration.TryGetValue(handlerType, out List<Action<IApiConfiguration>> configurations))
            {
                foreach (Action<IApiConfiguration> configuration in configurations)
                {
                    configuration.Invoke(apiConfiguration);
                }
            }
            else
            {
                throw new Exception("Handler not registered");
            }

            if (_postConfiguration.TryGetValue(handlerType, out List<Action<IApiConfiguration>> postConfigurations))
            {
                foreach (Action<IApiConfiguration> postConfiguration in postConfigurations)
                {
                    postConfiguration.Invoke(apiConfiguration);
                }
            }

            ApiConfigurationScope scope = apiConfiguration.CreateScope();

            if (!_onInitialization.TryGetValue(handlerType, out List<Action<IApiOnInitialization>> onInitialized))
            {
                return scope;
            }

            using ApiOnInitialization provider = new ApiOnInitialization(scope.Scope.BuildProvider());

            foreach (Action<IApiOnInitialization> action in onInitialized)
            {
                action.Invoke(provider);
            }

            return scope;
        }

        private static void ValidateHandler(Type handlerType)
        {
            UseHandlerConfigurationProviderAttribute useProvider = handlerType.GetCustomAttribute<UseHandlerConfigurationProviderAttribute>();

            if (useProvider == null)
            {
                throw new HandlerNullProviderException(handlerType);
            }
        }

        private ApiConfiguration InternalBuildApiConfiguration(Type handlerType)
        {
            IDependencyCollection dependencies = new DependencyCollection();

            if (!_providerConfiguration.IsEmpty())
            {
                _configurationProvider.ExtendConfiguration(c =>
                {
                    ApiConfiguration configuration = new ApiConfiguration(c, handlerType);

                    foreach (Action<IApiConfiguration> providerConfiguration in _providerConfiguration)
                    {
                        providerConfiguration.Invoke(configuration);
                    }
                });
            }

            _configurationProvider.InternalConfigure(dependencies);

            return new ApiConfiguration(dependencies, handlerType);
        }
    }
}