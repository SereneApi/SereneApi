using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Interfaces;
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

        private readonly IDependencyCollection _dependencies;

        public DependencyInjectionClientFactory(IDependencyCollection dependencies)
        {
            _dependencies = dependencies;

            HandlerName = GenerateHandlerName();
        }

        public HttpClient BuildClient()
        {
            using IDependencyProvider provider = _dependencies.BuildProvider();

            IHttpClientFactory clientFactory = provider.GetDependency<IServiceProvider>().GetService<IHttpClientFactory>();

            HttpClient client = clientFactory.CreateClient(HandlerName);

            return client;
        }

        public void Configure()
        {
            string handlerName = GenerateHandlerName();

            using IDependencyProvider provider = _dependencies.BuildProvider();

            IServiceCollection services = provider.GetDependency<IServiceCollection>();

            services.AddHttpClient(handlerName, client =>
            {
                using IDependencyProvider dependencies = _dependencies.BuildProvider();

                IConnectionSettings connection = dependencies.GetDependency<IConnectionSettings>();

                if(connection.Timeout == default || connection.Timeout < 0)
                {
                    throw new ArgumentException("The timeout value must be greater than 0 seconds.");
                }

                client.BaseAddress = connection.BaseAddress;
                client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
                client.DefaultRequestHeaders.Accept.Clear();

                if(dependencies.TryGetDependency(out IAuthenticator authenticator))
                {
                    IAuthentication authentication = authenticator.Authenticate();

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
                }
                else if(dependencies.TryGetDependency(out IAuthentication authentication))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
                }

                if(dependencies.TryGetDependency(out ContentType contentType))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.Value));
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                using IDependencyProvider dependencies = _dependencies.BuildProvider();

                if(dependencies.TryGetDependency(out HttpMessageHandler messageHandler))
                {
                    return messageHandler;
                }

                ICredentials credentials = dependencies.GetDependency<ICredentials>();

                messageHandler = new HttpClientHandler
                {
                    Credentials = credentials
                };

                return messageHandler;
            });
        }

        private static string GenerateHandlerName()
        {
            return $"SereneApi.{typeof(TApiHandler).FullName}";
        }
    }
}
