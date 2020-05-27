using DeltaWare.SereneApi.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DeltaWare.SereneApi
{
    public class ApiHandlerOptionsBuilder : IApiHandlerOptionsBuilder
    {
        private Uri _source;

        private string _resource;

        private string _resourcePrecursor;

        private readonly uint _retryCount;

        private readonly ILogger _logger;

        private readonly IQueryFactory _queryFactory;

        private HttpClient _httpClientOverride;

        /// <summary>
        /// The Source the <see cref="ApiHandler"/> will use to make API requests against
        /// </summary>
        /// <param name="source">The source of the Server, EG: http://someservice.com:8080</param>
        /// <param name="resource">The API resource that the <see cref="ApiHandler"/> will interact with</param>
        /// <param name="resourcePrecursor">The Resource Precursor this applied before the Resource. By default this is set to "api/"</param>
        public ApiHandlerOptionsBuilder UseSource(Uri source, string resource, string resourcePrecursor = null)
        {
            if (_httpClientOverride != null)
            {
                throw new ArgumentException("This method cannot be called alongside UseClientOverride");
            }

            if (_source != null || _resource != null)
            {
                throw new ArgumentException("This method cannot be called twice");
            }

            _source = source;
            _resource = resource;

            if (resource != null)
            {
                _resourcePrecursor = resourcePrecursor;
            }

            return this;
        }

        /// <summary>
        /// Overrides the Client with the supplied <see cref="HttpClient"/> this will disable the supplied Source, Timeout and <see cref="HttpRequestHeaders"/>.
        /// This should only be used for Unit Testing
        /// </summary>
        /// <param name="clientOverride">The <see cref="HttpClient"/> to be used when making API requests.</param>
        public ApiHandlerOptionsBuilder UseClientOverride(HttpClient clientOverride)
        {
            if (_source != null || _resource != null)
            {
                throw new ArgumentException("This method cannot be called alongside UseSource");
            }

            _httpClientOverride = clientOverride;

            return this;
        }

        public IApiHandlerOptions BuildOptions(bool disposeClient = true)
        {
            HttpClient httpClient;

            if (_httpClientOverride != null)
            {
                httpClient = _httpClientOverride;
            }
            else
            {
                httpClient = new HttpClient();
            }

            IApiHandlerOptions options = new ApiHandlerOptions(httpClient, _logger, _queryFactory, _retryCount, disposeClient);

            return options;
        }
    }
}
