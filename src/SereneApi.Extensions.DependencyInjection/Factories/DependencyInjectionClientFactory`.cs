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

        private readonly DependencyCollection _dependencyCollection;

        public DependencyInjectionClientFactory(IDependencyCollection dependencyCollection)
        {
            _dependencyCollection = (DependencyCollection)dependencyCollection;

            HandlerName = GenerateHandlerName();
        }

        public HttpClient BuildClient()
        {
            IHttpClientFactory clientFactory = _dependencyCollection.GetDependency<IHttpClientFactory>();

            HttpClient client = clientFactory.CreateClient(HandlerName);

            return client;
        }

        public static void Configure(IDependencyCollection dependencies, IServiceCollection services)
        {
            string handlerName = GenerateHandlerName();

            IConnectionInfo connection = dependencies.GetDependency<IConnectionInfo>();

            if(connection.Timeout == default || connection.Timeout < 0)
            {
                throw new ArgumentException("The timeout value must be greater than 0 seconds.");
            }

            HttpMessageHandler messageHandler = HttpClientHelper.BuildMessageHandler(dependencies);

            services.AddHttpClient(handlerName, client =>
            {
                HttpClientHelper.ConfigureHttpClient(client, dependencies);
                HttpClientHelper.BuildRequestHeaders(client.DefaultRequestHeaders, dependencies);
            })
            .ConfigurePrimaryHttpMessageHandler(() => messageHandler);
        }

        private static string GenerateHandlerName()
        {
            return $"SereneApi.{typeof(TApiHandler).FullName}";
        }
    }
}
