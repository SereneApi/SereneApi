using Microsoft.Extensions.DependencyInjection;
using SereneApi.Interfaces;
using SereneApi.Types;
using SereneApi.Types.Headers.Accept;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

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

            if(!dependencies.TryGetDependency(out HttpMessageHandler messageHandler))
            {
                ICredentials credentials = dependencies.GetDependency<ICredentials>();

                messageHandler = new HttpClientHandler
                {
                    Credentials = credentials
                };
            }

            services.AddHttpClient(handlerName, client =>
            {
                client.BaseAddress = connection.Source;
                client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
                client.DefaultRequestHeaders.Accept.Clear();

                if(dependencies.TryGetDependency(out IAuthentication authentication))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
                }

                if(dependencies.TryGetDependency(out ContentType contentType))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.Value));
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() => messageHandler);
        }

        private static string GenerateHandlerName()
        {
            return $"SereneApi.{typeof(TApiHandler).FullName}";
        }
    }
}
