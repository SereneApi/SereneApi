using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Configuration.Handler;
using SereneApi.Core.Http.Authentication;
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

        private readonly IDependencyProvider _dependencies;

        private readonly ILogger? _logger;

        public ClientFactory(IDependencyProvider dependencies, IHandlerBuilder handlerBuilder, HandlerConfiguration handlerConfiguration, ILogger? logger = null)
        {
            _dependencies = dependencies;
            _logger = logger;

            HandlerConfiguration = handlerConfiguration;
            HandlerBuilder = handlerBuilder;
        }

        /// <inheritdoc cref="IClientFactory.BuildClientAsync"/>
        public Task<HttpClient> BuildClientAsync(out bool disposeClient)
        {
            _logger?.LogDebug("Building Client");

            HttpClient client = InternalBuildClient();

            disposeClient = DisposeClient;

            return InternalConfigureClientAsync(client);
        }

        protected virtual HttpClient InternalBuildClient()
        {
            return new HttpClient(HandlerBuilder.BuildHandler(), false);
        }

        protected virtual async Task<HttpClient> InternalConfigureClientAsync(HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Clear();

            SetConnection(client);
            SetHeaders(client);

            await OnAuthorizationAsync(client);

            return client;
        }

        protected virtual async Task OnAuthorizationAsync(HttpClient client)
        {
            if (_dependencies.TryGetDependency(out IAuthenticator authenticator))
            {
                _logger?.LogDebug("Authorizing request.");

                IAuthentication authentication;

                try
                {
                    authentication = await authenticator.AuthorizeAsync();

                    _logger?.LogDebug("{scheme} Authorization successful.", authentication.Scheme);
                }
                catch (Exception e)
                {
                    _logger?.LogError(e, "An exception was encountered whilst authorizing the request.");

                    throw;
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
            }
            else if (_dependencies.TryGetDependency(out IAuthentication authentication))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
            }
        }

        protected virtual void SetConnection(HttpClient client)
        {
            IConnectionSettings connection = _dependencies.GetRequiredDependency<IConnectionSettings>();

            if (connection.Timeout < 1)
            {
                throw new ArgumentException("The timeout value must be greater than 0 seconds.");
            }

            client.BaseAddress = connection.BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
        }

        protected virtual void SetHeaders(HttpClient client)
        {
            if (HandlerConfiguration.TryGetContentType(out ContentType contentType))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.ToTypeString()));
            }

            if (_dependencies.TryGetDependency(out HandlerConfiguration configuration) && configuration.Contains(HandlerConfigurationKeys.RequestHeaders))
            {
                foreach ((string key, string value) in configuration.GetRequestHeaders())
                {
                    client.DefaultRequestHeaders.Add(key, value);
                }
            }
        }
    }
}