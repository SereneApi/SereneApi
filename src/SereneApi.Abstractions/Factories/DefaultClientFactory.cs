using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Authorization;
using SereneApi.Abstractions.Authorization.Authorizers;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Request.Content;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Factories
{
    /// <inheritdoc cref="IClientFactory"/>
    internal class DefaultClientFactory: IClientFactory, IDisposable
    {
        private readonly object _clientLock = new object();

        private readonly IDependencyProvider _dependencies;

        private readonly ILogger _logger;

        private HttpClient _cachedClient;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultClientFactory"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies the <see cref="DefaultClientFactory"/> may use when creating clients.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public DefaultClientFactory([NotNull] IDependencyProvider dependencies)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            _dependencies.TryGetDependency(out _logger);
        }

        /// <inheritdoc cref="IClientFactory.BuildClientAsync"/>
        public async Task<HttpClient> BuildClientAsync()
        {
            CheckIfDisposed();

            Monitor.Enter(_clientLock);

            if(_cachedClient != null)
            {
                Monitor.Exit(_clientLock);

                _logger?.LogDebug("Using cached client");

                return _cachedClient;
            }

            _logger?.LogDebug("Building Client");

            bool handlerFound = _dependencies.TryGetDependency(out HttpMessageHandler messageHandler);

            if(!handlerFound)
            {
                _logger?.LogDebug("No Handler found, building new Handler");

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
                Monitor.Exit(_clientLock);

                _logger?.LogError("The timeout value was not greater than 0 seconds");

                throw new ArgumentException("The timeout value must be greater than 0 seconds.");
            }

            client.BaseAddress = connection.BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(connection.Timeout);
            client.DefaultRequestHeaders.Accept.Clear();

            if(_dependencies.TryGetDependency(out IAuthorizer authenticator))
            {
                _logger?.LogDebug("An authorizer was provided for the Handler");

                IAuthorization authorization = await authenticator.AuthorizeAsync();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(authorization.Scheme, authorization.Parameter);
            }
            else if(_dependencies.TryGetDependency(out IAuthorization authentication))
            {
                _logger?.LogDebug("Authentication was specified for the Handler");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);
            }

            if(_dependencies.TryGetDependency(out ContentType contentType))
            {
                _logger?.LogDebug("Content type was specified for the Handler");

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(contentType.ToTypeString()));
            }

            _logger?.LogDebug("The client was successfully built");

            _cachedClient = client;

            Monitor.Exit(_clientLock);

            return _cachedClient;
        }

        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Throws an Object Disposed Exception if the <see cref="DefaultClientFactory"/> has been disposed.
        /// </summary>
        protected void CheckIfDisposed()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException(nameof(GetType));
            }
        }

        /// <summary>
        /// Disposes the current instance of the <see cref="DefaultClientFactory"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override this method to implement <see cref="DefaultClientFactory"/> disposal.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if(_disposed)
            {
                return;
            }

            if(disposing)
            {
                _cachedClient?.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
