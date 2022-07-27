using DeltaWare.Dependencies.Abstractions;
using DeltaWare.SDK.Serialization.Types;
using SereneApi.Core.Configuration.Handler;
using SereneApi.Core.Events;
using SereneApi.Core.Http.Client;
using SereneApi.Core.Http.Client.Handler;
using SereneApi.Core.Http.Requests.Handler;
using System;
using System.Collections.Generic;
using System.Net;

namespace SereneApi.Core.Configuration.Provider
{
    public abstract class HandlerConfigurationProvider
    {
        private readonly List<Action<IDependencyCollection>> _configurationExtensions = new();

        public void ExtendConfiguration(Action<IDependencyCollection> configuration)
        {
            _configurationExtensions.Add(configuration ?? throw new ArgumentNullException(nameof(configuration)));
        }

        internal void InternalConfigure(IDependencyCollection dependencies)
        {
            dependencies
                .Register<HandlerConfiguration>()
                .AsSingleton();

            dependencies.Configure<HandlerConfiguration>(c =>
            {
                c.SetResourcePath("");
                c.SetRetryAttempts(0);
                c.SetTimeout(30);
                c.SetThrowExceptions(false);
                c.SetThrowOnFailure(false);
            });

            dependencies.Register<EventManager>()
                .DefineAs<IEventManager>()
                .AsSingleton();

            dependencies.Register<ClientFactory>()
                .DefineAs<IClientFactory>()
                .AsSingleton();

            dependencies.Register<HandlerFactory>()
                .DefineAs<IHandlerFactory>()
                .AsSingleton();

            dependencies.Register(() => CredentialCache.DefaultCredentials)
                .AsSingleton();

            dependencies.Register(p => (HandlerFactory)p.GetRequiredDependency<IHandlerFactory>())
                .DefineAs<IHandlerBuilder>()
                .AsSingleton();

            dependencies.Register(() => new ObjectSerializer())
                .DefineAs<IObjectSerializer>()
                .AsSingleton();

            dependencies.Register<RetryingRequestHandler>()
                .DefineAs<IRequestHandler>()
                .AsScoped();

            foreach (Action<IDependencyCollection> configurationExtension in _configurationExtensions)
            {
                configurationExtension.Invoke(dependencies);
            }

            Configure(dependencies);
        }

        protected abstract void Configure(IDependencyCollection dependencies);
    }
}