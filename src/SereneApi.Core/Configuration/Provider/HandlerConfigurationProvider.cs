using DeltaWare.Dependencies.Abstractions;
using DeltaWare.SDK.Core.Serialization;
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
            dependencies.AddSingleton<HandlerConfiguration>();

            dependencies.Configure<HandlerConfiguration>(c =>
            {
                c.SetResourcePath("");
                c.SetRetryAttempts(0);
                c.SetTimeout(30);
                c.SetThrowExceptions(false);
                c.SetThrowOnFailure(false);
            });

            dependencies.AddSingleton<IEventManager, EventManager>();

            dependencies.AddSingleton(() => CredentialCache.DefaultCredentials);
            dependencies.AddSingleton<IClientFactory, ClientFactory>();
            dependencies.AddSingleton<IHandlerFactory, HandlerFactory>();
            dependencies.AddSingleton<IHandlerBuilder>(p => (HandlerFactory)p.GetRequiredDependency<IHandlerFactory>());
            dependencies.AddSingleton<IObjectSerializer>(() => new ObjectSerializer());

            dependencies.AddScoped<IRequestHandler, RetryingRequestHandler>();

            foreach (Action<IDependencyCollection> configurationExtension in _configurationExtensions)
            {
                configurationExtension.Invoke(dependencies);
            }

            Configure(dependencies);
        }

        protected abstract void Configure(IDependencyCollection dependencies);
    }
}