using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Factories;
using SereneApi.Extensions.Caching.Types;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Caching
{
    public class CachedMessageHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        private readonly Cache<Uri, ICachedResponse> _responseCache;


        public CachedMessageHandler(IDependencyProvider dependencies)
        {
            InnerHandler = dependencies.GetDependency<IClientFactory>().BuildHttpMessageHandler();

            dependencies.TryGetDependency(out _logger);

            _responseCache = dependencies.GetDependency<Cache<Uri, ICachedResponse>>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Get)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            if (_responseCache.TryGet(request.RequestUri, out ICachedResponse cachedResponse))
            {
                _logger?.LogInformation("Returning Cached content for request uri: {uri}", request.RequestUri);

                return cachedResponse.GenerateHttpResponse();
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            cachedResponse = CachedResponse.FromHttpResponse(response);

            _responseCache.Store(request.RequestUri, cachedResponse);

            return response;
        }
    }
}
