using DeltaWare.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Authenticators;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Request.Content;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SereneApi.Extensions.DependencyInjection.Factories
{
    [DebuggerDisplay("{HandlerName}")]
    internal class ClientFactory<TApiHandler>: IClientFactory
    {
        private readonly IDependencyProvider _dependencies;

        public bool IsConfigured { get; private set; }

        public string HandlerName { get; }

        public ClientFactory(IDependencyProvider dependencies)
        {
            _dependencies = dependencies;

            HandlerName = GenerateHandlerName();
        }

        public Task<HttpClient> BuildClientAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                IHttpClientFactory clientFactory =
                    _dependencies.GetDependency<IServiceProvider>().GetService<IHttpClientFactory>();

                HttpClient client = clientFactory.CreateClient(HandlerName);

                return client;
            });
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
                    IAuthentication authentication = authenticator.AuthenticateAsync().GetAwaiter().GetResult();

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
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.ToTypeString()));
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
