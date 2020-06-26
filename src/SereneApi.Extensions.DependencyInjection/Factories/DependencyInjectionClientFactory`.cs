using Microsoft.Extensions.DependencyInjection;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace SereneApi.Extensions.DependencyInjection.Factories
{
    [DebuggerDisplay("{HandlerName}")]
    public class DependencyInjectionClientFactory<TApiHandler>: IClientFactory
    {
        public string HandlerName { get; }

        private readonly DependencyCollection _dependencies;

        public DependencyInjectionClientFactory(IDependencyCollection dependencies)
        {
            _dependencies = (DependencyCollection)dependencies;

            HandlerName = GenerateHandlerName();
        }

        public HttpClient BuildClient()
        {
            IHttpClientFactory clientFactory = _dependencies.GetDependency<IHttpClientFactory>();

            HttpClient client = clientFactory.CreateClient(HandlerName);

            return client;
        }

        public static void Configure(IDependencyCollection dependencies, IServiceCollection services)
        {
            string handlerName = GenerateHandlerName();

            IConnectionSettings connection = dependencies.GetDependency<IConnectionSettings>();

            if(connection.Timeout == default || connection.Timeout < 0)
            {
                throw new ArgumentException("The timeout value must be greater than 0 seconds.");
            }
            
            services.AddHttpClient(handlerName, client =>
            {
                HttpClientHelper.ConfigureHttpClient(client, dependencies);
                HttpClientHelper.BuildRequestHeaders(client.DefaultRequestHeaders, dependencies);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                if(!dependencies.TryGetDependency(out HttpMessageHandler messageHandler))
                {
                    messageHandler = HttpClientHelper.BuildMessageHandler(dependencies);
                }

                return messageHandler;
            });
        }

        private static string GenerateHandlerName()
        {
            return $"SereneApi.{typeof(TApiHandler).FullName}";
        }
    }
}
