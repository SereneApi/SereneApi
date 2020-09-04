using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Authorization;
using SereneApi.Abstractions.Authorization.Authorizers;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Request.Content;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Factories
{
    /// <inheritdoc cref="IClientFactory"/>
    internal class DefaultClientFactory: IClientFactory
    {
        private readonly IDependencyProvider _dependencies;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultClientFactory"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies the <see cref="DefaultClientFactory"/> may use when creating clients.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public DefaultClientFactory([NotNull] IDependencyProvider dependencies)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }

        /// <inheritdoc cref="IClientFactory.BuildClientAsync"/>
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

            IConnectionConfiguration connection = _dependencies.GetDependency<IConnectionConfiguration>();

            if(connection.Timeout == default || connection.Timeout < 0)
            {
                throw new ArgumentException("The timeout value must be greater than 0 seconds.");
            }

            client.BaseAddress = connection.BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
            client.DefaultRequestHeaders.Accept.Clear();

            if(_dependencies.TryGetDependency(out IAuthorizer authenticator))
            {
                IAuthorization authorization = await authenticator.AuthorizeAsync();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(authorization.Scheme, authorization.Parameter);
            }
            else if(_dependencies.TryGetDependency(out IAuthorization authentication))
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
