using Microsoft.Extensions.DependencyInjection;
using SereneApi.Interfaces;
using SereneApi.Types;
using SereneApi.Types.Headers.Accept;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SereneApi.Extensions.DependencyInjection.Factories
{
    public class DependencyInjectionClientFactory<TApiHandler>: IClientFactory
    {
        private readonly DependencyCollection _dependencyCollection;

        public DependencyInjectionClientFactory(IDependencyCollection dependencyCollection)
        {
            _dependencyCollection = (DependencyCollection)dependencyCollection;
        }

        public void Initiate()
        {
            if(!_dependencyCollection.TryGetDependency(out HttpMessageHandler messageHandler))
            {
                ICredentials credentials = _dependencyCollection.GetDependency<ICredentials>();

                messageHandler = new HttpClientHandler
                {
                    Credentials = credentials
                };
            }

            if(_dependencyCollection.TryGetDependency(out IServiceCollection serviceCollection))
            {
                IConnectionInfo connection = _dependencyCollection.GetDependency<IConnectionInfo>();

                serviceCollection.AddHttpClient(typeof(TApiHandler).ToString(), client =>
                {
                    client.BaseAddress = connection.Source;
                    client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
                    client.DefaultRequestHeaders.Accept.Clear();

                    if(_dependencyCollection.TryGetDependency(out IAuthentication authentication))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
                    }

                    if(_dependencyCollection.TryGetDependency(out ContentType contentType))
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.Value));
                    }
                })
                .ConfigurePrimaryHttpMessageHandler(() => messageHandler);
            }
        }

        public HttpClient BuildClient()
        {
            IHttpClientFactory clientFactory = _dependencyCollection.GetDependency<IHttpClientFactory>();

            return clientFactory.CreateClient(typeof(TApiHandler).ToString());
        }
    }
}
