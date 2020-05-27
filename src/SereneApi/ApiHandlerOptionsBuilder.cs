using DeltaWare.SereneApi.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using DeltaWare.SereneApi.Enums;
using DeltaWare.SereneApi.Helpers;
using DeltaWare.SereneApi.Types;
using DeltaWare.SereneApi.Types.Dependencies;

namespace DeltaWare.SereneApi
{
    public class ApiHandlerOptionsBuilder : IApiHandlerOptionsBuilder
    {
        #region Variables

        private readonly DependencyCollection _dependencyCollection = new DependencyCollection();

        private Uri _source;

        private HttpClient _clientOverride;

        private readonly HttpClient _baseClient;

        private bool _disposeClient;

        private TimeSpan _timeout = ApiHandlerOptionDefaults.TimeoutPeriod;

        private readonly Action<HttpRequestHeaders> _requestHeaderBuilder = ApiHandlerOptionDefaults.DefaultRequestHeadersBuilder;

        #endregion

        public ApiHandlerOptionsBuilder()
        {
            _dependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            _dependencyCollection.AddDependency(RetryDependency.Default);
        }

        internal ApiHandlerOptionsBuilder(HttpClient baseClient, bool disposeClient = true) : this()
        {
            _disposeClient = disposeClient;
            _baseClient = baseClient;
        }

        /// <summary>
        /// The Source the <see cref="ApiHandler"/> will use to make API requests against
        /// </summary>
        /// <param name="source">The source of the Server, EG: http://someservice.com:8080</param>
        /// <param name="resource">The API resource that the <see cref="ApiHandler"/> will interact with</param>
        /// <param name="resourcePrecursor">The Resource Precursor this applied before the Resource. By default this is set to "api/"</param>
        public ApiHandlerOptionsBuilder UseSource(string source, string resource, string resourcePrecursor = null)
        {
            if (_clientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called alongside UseClientOverride");
            }

            if (_source != null)
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            // The Resource Precursors default value will be used if a null or whitespace value is provided.
            _source = ApiHandlerOptionsHelper.CreateApiSource(source, resource, resourcePrecursor);

            return this;
        }

        /// <summary>
        /// Overrides the Client with the supplied <see cref="HttpClient"/> this will disable the supplied Source, Timeout and <see cref="HttpRequestHeaders"/>.
        /// This should only be used for Unit Testing
        /// </summary>
        /// <param name="clientOverride">The <see cref="HttpClient"/> to be used when making API requests.</param>
        public ApiHandlerOptionsBuilder UseClientOverride(HttpClient clientOverride, bool disposeClient = true)
        {
            if (_baseClient != null)
            {
                throw new MethodAccessException("This method cannot be called when using the ApiHandlerFactory");
            }

            if (_source != null)
            {
                throw new MethodAccessException("This method cannot be called alongside UseSource");
            }

            _clientOverride = clientOverride;
            _disposeClient = disposeClient;

            return this;
        }

        /// <summary>
        /// Sets the timeout to be used by the <see cref="ApiHandler"/> when making API requests. By default this value is set to 30 seconds
        /// </summary>
        /// <param name="timeoutPeriod">The <see cref="TimeSpan"/> to be used as the timeout period by the <see cref="ApiHandler"/></param>
        public ApiHandlerOptionsBuilder SetTimeoutPeriod(TimeSpan timeoutPeriod)
        {
            _timeout = timeoutPeriod;

            return this;
        }

        /// <summary>
        /// Adds an <see cref="ILogger"/> to the <see cref="ApiHandler"/> allowing built in Logging
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to be used for Logging</param>
        public ApiHandlerOptionsBuilder AddLogger(ILogger logger)
        {
            _dependencyCollection.AddDependency(logger);

            return this;
        }

        public IApiHandlerOptions BuildOptions()
        {
            HttpClient httpClient;

            if (_clientOverride != null)
            {
                httpClient = _clientOverride;
            }
            else
            {
                httpClient = _baseClient;

                httpClient.BaseAddress = _source;
                httpClient.Timeout = _timeout;

                _requestHeaderBuilder.Invoke(httpClient.DefaultRequestHeaders);
            }

            _dependencyCollection.AddDependency(httpClient, _disposeClient ? DependencyBinding.Bound : DependencyBinding.Unbound);

            IApiHandlerOptions options = new ApiHandlerOptions(_dependencyCollection, _source);

            return options;
        }
    }
}
