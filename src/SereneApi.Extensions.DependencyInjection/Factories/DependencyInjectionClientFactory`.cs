using DeltaWare.Dependencies;
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
        public bool IsConfigured { get; private set; }
        public string HandlerName { get; }

        private readonly IDependencyProvider _dependencies;

        public DependencyInjectionClientFactory(IDependencyProvider dependencies)
        {
            _dependencies = dependencies;

            HandlerName = GenerateHandlerName();
        }

        public HttpClient BuildClient()
        {
            IHttpClientFactory clientFactory = _dependencies.GetDependency<IServiceProvider>().GetService<IHttpClientFactory>();

            HttpClient client = clientFactory.CreateClient(HandlerName);

            return client;
        }

        public void Configure()
        {
            if(IsConfigured)
            {
                return;
            }

            string handlerName = GenerateHandlerName();

            IServiceCollection services = _dependencies.GetDependency<IServiceCollection>();

            services.AddHttpClient(handlerName, client =>
            {
                IConnectionSettings connection = _dependencies.GetDependency<IConnectionSettings>();

                if(connection.Timeout == default || connection.Timeout < 0)
                {
                    throw new ArgumentException("The timeout value must be greater than 0 seconds.");
                }

                client.BaseAddress = connection.BaseAddress;
                client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
                client.DefaultRequestHeaders.Accept.Clear();

                if(_dependencies.TryGetDependency(out IAuthenticator authenticator))
                {
                    IAuthentication authentication = authenticator.Authenticate();

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
                }
                else if(_dependencies.TryGetDependency(out IAuthentication authentication))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
                }

                if(_dependencies.TryGetDependency(out ContentType contentType))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.Value));
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                if(_dependencies.TryGetDependency(out HttpMessageHandler messageHandler))
                {
                    return messageHandler;
                }

                ICredentials credentials = _dependencies.GetDependency<ICredentials>();

                messageHandler = new HttpClientHandler
                {
                    Credentials = credentials
                };

                return messageHandler;
            });

            IsConfigured = true;
        }

        private static string GenerateHandlerName()
        {
            return $"SereneApi.{typeof(TApiHandler).FullName}";
        }
    }
}
