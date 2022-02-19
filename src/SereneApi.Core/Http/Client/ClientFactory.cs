using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Configuration;
using SereneApi.Core.Http.Authentication;
using SereneApi.Core.Http.Authorization;
using SereneApi.Core.Http.Client.Handler;
using SereneApi.Core.Http.Content;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SereneApi.Core.Http.Client
{
    /// <inheritdoc cref="IClientFactory"/>
    public class ClientFactory : IClientFactory
    {
        protected virtual bool DisposeClient => true;
        protected IHandlerBuilder HandlerBuilder { get; }
        protected HandlerConfiguration HandlerConfiguration { get; }
        protected IDependencyScope Scope { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ClientFactory"/>.
        /// </summary>
        /// <param name="scope">
        /// The scope the <see cref="ClientFactory"/> may use when creating clients.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public ClientFactory(IDependencyScope scope, IHandlerBuilder handlerBuilder, HandlerConfiguration handlerConfiguration)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));

            HandlerConfiguration = handlerConfiguration;
            HandlerBuilder = handlerBuilder;
        }

        /// <inheritdoc cref="IClientFactory.BuildClientAsync"/>
        public Task<HttpClient> BuildClientAsync(out bool disposeClient)
        {
            using IDependencyProvider dependencies = Scope.BuildProvider();

            dependencies.TryGetDependency(out ILogger logger);

            logger?.LogDebug("Building Client");

            HttpClient client = InternalBuildClient(dependencies, logger);

            disposeClient = DisposeClient;

            return InternalConfigureClientAsync(client, dependencies, logger);
        }

        protected virtual HttpClient InternalBuildClient(IDependencyProvider dependencies, ILogger logger = null)
        {
            return new HttpClient(HandlerBuilder.BuildHandler(), false);
        }

        protected virtual async Task<HttpClient> InternalConfigureClientAsync(HttpClient client, IDependencyProvider dependencies, ILogger logger = null)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Clear();

            SetConnection(client, dependencies, logger);
            SetHeaders(client, dependencies, logger);

            await OnAuthorizationAsync(client, dependencies, logger);

            return client;
        }

        protected virtual async Task OnAuthorizationAsync(HttpClient client, IDependencyProvider dependencies, ILogger logger = null)
        {
            if (dependencies.TryGetDependency(out IAuthenticator authenticator))
            {
                logger?.LogDebug("Authorizing request.");

                IAuthorization authorization;

                try
                {
                    authorization = await authenticator.AuthorizeAsync();

                    logger?.LogDebug("{scheme} Authorization successful.", authorization.Scheme);
                }
                catch (Exception e)
                {
                    logger?.LogError(e, "An exception was encountered whilst authorizing the request.");

                    throw;
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorization.Scheme, authorization.Parameter);
            }
            else if (dependencies.TryGetDependency(out IAuthorization authentication))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
            }
        }

        protected virtual void SetConnection(HttpClient client, IDependencyProvider dependencies, ILogger logger = null)
        {
            IConnectionSettings connection = dependencies.GetRequiredDependency<IConnectionSettings>();

            if (connection.Timeout < 1)
            {
                throw new ArgumentException("The timeout value must be greater than 0 seconds.");
            }

            client.BaseAddress = connection.BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
        }

        protected virtual void SetHeaders(HttpClient client, IDependencyProvider dependencies, ILogger logger = null)
        {
            if (HandlerConfiguration.TryGetContentType(out ContentType contentType))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.ToTypeString()));
            }

            if (dependencies.TryGetDependency(out HandlerConfiguration configuration) && configuration.Contains(HandlerConfigurationKeys.RequestHeaders))
            {
                foreach ((string key, string value) in configuration.GetRequestHeaders())
                {
                    client.DefaultRequestHeaders.Add(key, value);
                }
            }
        }
    }
}