using SereneApi.Interfaces;
using SereneApi.Types.Headers.Accept;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SereneApi.Factories
{
    internal sealed class DefaultClientFactory: IClientFactory
    {
        private readonly IDependencyCollection _dependencyCollection;

        public DefaultClientFactory(IDependencyCollection dependencyCollection)
        {
            _dependencyCollection = dependencyCollection;
        }

        public HttpClient BuildClient()
        {
            IConnectionInfo connection = _dependencyCollection.GetDependency<IConnectionInfo>();

            if(!_dependencyCollection.TryGetDependency(out HttpClientHandler clientHandler))
            {
                ICredentials credentials = _dependencyCollection.GetDependency<ICredentials>();

                clientHandler = new HttpClientHandler
                {
                    Credentials = credentials
                };
            }

            HttpClient client = new HttpClient(clientHandler)
            {
                BaseAddress = connection.Source,
                Timeout = TimeSpan.FromSeconds(connection.Timeout)
            };

            client.DefaultRequestHeaders.Accept.Clear();

            if(_dependencyCollection.TryGetDependency(out IAuthentication authentication))
            {
                AuthenticationHeaderValue authenticationHeader = new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);

                client.DefaultRequestHeaders.Authorization = authenticationHeader;
            }

            if(_dependencyCollection.TryGetDependency(out ContentType contentType))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.Value));
            }

            return client;
        }
    }
}
