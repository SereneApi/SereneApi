using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Authorization;
using SereneApi.Core.Authorization.Authorizers;
using SereneApi.Core.Connection;
using SereneApi.Core.Content;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SereneApi.Core.Factories
{
    /// <inheritdoc cref="IClientFactory"/>
    public class ClientFactory : IClientFactory
    {
        private readonly IDependencyProvider _dependencies;

        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of <see cref="ClientFactory"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies the <see cref="ClientFactory"/> may use when creating clients.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public ClientFactory(IDependencyProvider dependencies)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            _dependencies.TryGetDependency(out _logger);
        }

        /// <inheritdoc cref="IClientFactory.BuildClientAsync"/>
        public async Task<HttpClient> BuildClientAsync()
        {
            _logger?.LogDebug("Building Client");

            bool handlerFound = _dependencies.TryGetDependency(out HttpMessageHandler messageHandler);

            if (!handlerFound)
            {
                _logger?.LogDebug("No Handler found, building new Handler");

                messageHandler = BuildHttpMessageHandler();
            }

            // If a handle was found, the handler is not disposed of as the Dependency Collection has ownership.
            HttpClient client = new HttpClient(messageHandler, !handlerFound);

            IConnectionSettings connection = _dependencies.GetDependency<IConnectionSettings>();

            if (connection.Timeout == default || connection.Timeout < 0)
            {
                throw new ArgumentException("The timeout value must be greater than 0 seconds.");
            }

            client.BaseAddress = connection.BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
            client.DefaultRequestHeaders.Accept.Clear();

            if (_dependencies.TryGetDependency(out IAuthorizer authenticator))
            {
                IAuthorization authorization = await authenticator.AuthorizeAsync();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(authorization.Scheme, authorization.Parameter);
            }
            else if (_dependencies.TryGetDependency(out IAuthorization authentication))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
            }

            if (_dependencies.TryGetDependency(out ContentType contentType))
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(contentType.ToTypeString()));
            }

            return client;
        }

        public HttpMessageHandler BuildHttpMessageHandler()
        {
            ICredentials credentials = _dependencies.GetDependency<ICredentials>();

            return new HttpClientHandler
            {
                Credentials = credentials
            };
        }
    }
}
