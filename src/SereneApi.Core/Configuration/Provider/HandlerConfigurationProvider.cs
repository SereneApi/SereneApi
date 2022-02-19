using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Events;
using SereneApi.Core.Http.Client;
using SereneApi.Core.Http.Client.Handler;
using SereneApi.Core.Http.Requests.Handler;
using SereneApi.Core.Transformation;
using SereneApi.Core.Transformation.Transformers;
using System.Net;

namespace SereneApi.Core.Configuration.Provider
{
    public abstract class HandlerConfigurationProvider : IApiConfiguration
    {
        public IDependencyCollection Dependencies { get; } = new DependencyCollection();

        protected HandlerConfigurationProvider()
        {
            Dependencies.AddSingleton<HandlerConfiguration>();

            Dependencies.Configure<HandlerConfiguration>(c =>
            {
                c.SetResourcePath("");
                c.SetRetryAttempts(0);
                c.SetTimeout(30);
                c.SetThrowExceptions(false);
                c.SetThrowOnFailure(false);
            });

            Dependencies.AddSingleton<IEventManager, EventManager>();

            Dependencies.AddSingleton(() => CredentialCache.DefaultCredentials);
            Dependencies.AddSingleton<IClientFactory, ClientFactory>();
            Dependencies.AddSingleton<IHandlerFactory, HandlerFactory>();
            Dependencies.AddSingleton<IHandlerBuilder>(p => (HandlerFactory)p.GetRequiredDependency<IHandlerFactory>());
            Dependencies.AddSingleton<ITransformationService, TransformationService>();
            Dependencies.AddSingleton<IObjectToStringTransformer, BasicObjectToStringTransformer>();
            Dependencies.AddSingleton<IStringToObjectTransformer, BasicStringToObjectTransformer>();

            Dependencies.AddScoped<IRequestHandler, RetryingRequestHandler>();
        }
    }
}