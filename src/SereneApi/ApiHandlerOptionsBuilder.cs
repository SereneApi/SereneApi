using DeltaWare.SereneApi.Enums;
using DeltaWare.SereneApi.Helpers;
using DeltaWare.SereneApi.Interfaces;
using DeltaWare.SereneApi.Types;
using DeltaWare.SereneApi.Types.Dependencies;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DeltaWare.SereneApi
{
    public class ApiHandlerOptionsBuilder : IApiHandlerOptionsBuilder
    {
        #region Variables

        private readonly HttpClient _baseClient;

        private readonly bool _disposeClient;

        #endregion
        #region Properties

        protected DependencyCollection DependencyCollection { get; } = new DependencyCollection();

        protected Uri Source { get; set; }

        protected HttpClient ClientOverride;

        protected bool DisposeClientOverride;

        protected Action<HttpRequestHeaders> RequestHeaderBuilder = ApiHandlerOptionDefaults.DefaultRequestHeadersBuilder;

        protected TimeSpan Timeout = ApiHandlerOptionDefaults.TimeoutPeriod;

        #endregion
        #region Constructors

        public ApiHandlerOptionsBuilder()
        {
            _disposeClient = true;

            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            DependencyCollection.AddDependency(RetryDependency.Default);
        }

        internal ApiHandlerOptionsBuilder(HttpClient baseClient, bool disposeClient = true)
        {
            _disposeClient = disposeClient;
            _baseClient = baseClient;

            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            DependencyCollection.AddDependency(RetryDependency.Default);
        }

        #endregion

        /// <summary>
        /// The Source the <see cref="ApiHandler"/> will use to make API requests against
        /// </summary>
        /// <param name="source">The source of the Server, EG: http://someservice.com:8080</param>
        /// <param name="resource">The API resource that the <see cref="ApiHandler"/> will interact with</param>
        /// <param name="resourcePrecursor">The Resource Precursor this applied before the Resource. By default this is set to "api/"</param>
        public void UseSource(string source, string resource, string resourcePrecursor = null)
        {
            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called alongside UseClientOverride");
            }

            if (Source != null)
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            // The Resource Precursors default value will be used if a null or whitespace value is provided.
            Source = ApiHandlerOptionsHelper.CreateApiSource(source, resource, resourcePrecursor);
        }

        /// <summary>
        /// Overrides the Client with the supplied <see cref="HttpClient"/> this will disable the supplied Source, Timeout and <see cref="HttpRequestHeaders"/>.
        /// This should only be used for Unit Testing
        /// </summary>
        /// <param name="clientOverride">The <see cref="HttpClient"/> to be used when making API requests.</param>
        public void UseClientOverride(HttpClient clientOverride, bool disposeClient = true)
        {
            if (Source != null)
            {
                throw new MethodAccessException("This method cannot be called alongside UseSource");
            }

            if (_baseClient != null)
            {
                throw new MethodAccessException("This method cannot be called when using the ApiHandlerFactory");
            }

            ClientOverride = clientOverride;

            DisposeClientOverride = disposeClient;
        }

        /// <summary>
        /// Sets the timeout to be used by the <see cref="ApiHandler"/> when making API requests. By default this value is set to 30 seconds
        /// </summary>
        /// <param name="timeoutPeriod">The <see cref="TimeSpan"/> to be used as the timeout period by the <see cref="ApiHandler"/></param>
        public void SetTimeoutPeriod(TimeSpan timeoutPeriod)
        {
            Timeout = timeoutPeriod;
        }

        /// <summary>
        /// Adds an <see cref="ILogger"/> to the <see cref="ApiHandler"/> allowing built in Logging
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to be used for Logging</param>
        public void AddLogger(ILogger logger)
        {
            DependencyCollection.AddDependency(logger);
        }

        /// <summary>
        /// When enabled, upon a timeout the <see cref="ApiHandler"/> will re-attempt the request. By Default this is disabled
        /// </summary>
        /// <param name="retryCount">How many times the <see cref="ApiHandler"/> will re-attempt the request</param>
        public void EnableRetryOnTimeout(uint retryCount)
        {
            if (retryCount < 1)
            {
                throw new ArgumentException("To Enable Retry on Timeout the RetryCount must be greater than 0");
            }

            DependencyCollection.AddDependency(new RetryDependency(retryCount));
        }

        /// <summary>
        /// Overrides the default <see cref="HttpResponseHeaders"/> with the supplied <see cref="HttpResponseHeaders"/>
        /// </summary>
        /// <param name="requestHeaderBuilder">Builds the <see cref="HttpResponseHeaders"/></param>
        public void UseHttpRequestHeaders(Action<HttpRequestHeaders> requestHeaderBuilder)
        {
            RequestHeaderBuilder = requestHeaderBuilder;
        }

        /// <summary>
        /// Overrides the default <see cref="QueryFactory"/> with the supplied <see cref="IQueryFactory"/>
        /// </summary>
        /// <param name="queryFactory">The <see cref="IQueryFactory"/> to be used when building Queries</param>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            DependencyCollection.AddDependency(queryFactory);
        }

        public virtual IApiHandlerOptions BuildOptions()
        {
            if (ClientOverride != null)
            {
                DependencyCollection.AddDependency(ClientOverride, DisposeClientOverride ? DependencyBinding.Bound : DependencyBinding.Unbound);
            }
            else
            {
                HttpClient httpClient = _baseClient ?? new HttpClient();

                httpClient.BaseAddress = Source;
                httpClient.Timeout = Timeout;

                RequestHeaderBuilder.Invoke(httpClient.DefaultRequestHeaders);

                DependencyCollection.AddDependency(httpClient, _disposeClient ? DependencyBinding.Bound : DependencyBinding.Unbound);
            }

            IApiHandlerOptions options = new ApiHandlerOptions(DependencyCollection, Source);

            return options;
        }
    }
}
