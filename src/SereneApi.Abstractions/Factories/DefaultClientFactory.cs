using DeltaWare.Dependencies;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Authenticators;
using SereneApi.Abstractions.Requests.Content;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Factories
{
    internal class DefaultClientFactory: IClientFactory
    {
        private readonly IDependencyProvider _dependencies;

        public DefaultClientFactory(IDependencyProvider dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<HttpClient> BuildClientAsync()
        {
            bool handlerFound = _dependencies.TryGetDependency(out HttpMessageHandler messageHandler);

            if(!handlerFound)
            {
                ICredentials credentials = _dependencies.GetDependency<ICredentials>();

                messageHandler = new HttpClientHandler
                {
                    Credentials = credentials
                };
            }

            // If a handle was found, the handler is not disposed of as the Dependency Collection has ownership.
            HttpClient client = new HttpClient(messageHandler, !handlerFound);

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
                IAuthentication authentication = await authenticator.AuthenticateAsync();

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

            return client;
        }
    }
}
