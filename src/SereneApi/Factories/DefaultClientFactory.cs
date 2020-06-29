using DeltaWare.Dependencies.Abstractions;
using SereneApi.Interfaces;
using SereneApi.Types.Headers.Accept;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SereneApi.Factories
{
    public class DefaultClientFactory: IClientFactory
    {
        private readonly IDependencyCollection _dependencies;

        public DefaultClientFactory(IDependencyCollection dependencies)
        {
            _dependencies = dependencies;
        }

        public HttpClient BuildClient()
        {
            using IDependencyProvider dependencies = _dependencies.BuildProvider();

            bool handlerFound = dependencies.TryGetDependency(out HttpMessageHandler messageHandler);

            if(!handlerFound)
            {
                ICredentials credentials = dependencies.GetDependency<ICredentials>();

                messageHandler = new HttpClientHandler
                {
                    Credentials = credentials
                };
            }

            // If a handle was found, the handler is not disposed of as the Dependency Collection has ownership.
            HttpClient client = new HttpClient(messageHandler, !handlerFound);

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

            return client;
        }
    }
}
