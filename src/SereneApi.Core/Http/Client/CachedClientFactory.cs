using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Configuration.Handler;
using SereneApi.Core.Http.Client.Handler;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Core.Http.Client
{
    /// <inheritdoc cref="IClientFactory"/>
    public class CachedClientFactory : ClientFactory, IDisposable
    {
        private readonly object _buildLock = new();
        private HttpClient _cachedClient;

        protected override bool DisposeClient => false;

        /// <summary>
        /// Creates a new instance of <see cref="CachedClientFactory"/>.
        /// </summary>
        /// <param name="scope">
        /// The scope the <see cref="CachedClientFactory"/> may use when creating clients.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public CachedClientFactory(ILifetimeScope scope, IHandlerBuilder builder, HandlerConfiguration handlerConfiguration) : base(scope, builder, handlerConfiguration)
        {
        }

        public void Dispose()
        {
            _cachedClient?.Dispose();
        }

        protected override async Task<HttpClient> InternalConfigureClientAsync(HttpClient client, IDependencyProvider dependencies, ILogger logger = null)
        {
            Monitor.Enter(_buildLock);

            if (_cachedClient != null)
            {
                Monitor.Exit(_buildLock);

                return _cachedClient;
            }

            await base.InternalConfigureClientAsync(client, dependencies, logger);

            _cachedClient = client;

            Monitor.Exit(_buildLock);

            return client;
        }
    }
}