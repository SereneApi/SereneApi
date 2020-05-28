using Microsoft.Extensions.Logging;
using SereneApi.Enums;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types.Dependencies;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SereneApi.Types
{
    public class ApiHandlerOptionsBuilder : IApiHandlerOptionsBuilder
    {
        #region Variables

        private readonly HttpClient _baseClient;

        private readonly bool _disposeClient = true;

        #endregion
        #region Properties

        protected DependencyCollection DependencyCollection { get; }

        protected Uri Source { get; set; }

        protected string Resource { get; set; }

        protected HttpClient ClientOverride;

        protected bool DisposeClientOverride;

        protected Action<HttpRequestHeaders> RequestHeaderBuilder = ApiHandlerOptionDefaults.DefaultRequestHeadersBuilder;

        protected TimeSpan Timeout = ApiHandlerOptionDefaults.TimeoutPeriod;

        protected ICredentials Credentials { get; set; } = CredentialCache.DefaultCredentials;

        #endregion
        #region Constructors

        public ApiHandlerOptionsBuilder()
        {
            DependencyCollection = new DependencyCollection();

            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            DependencyCollection.AddDependency(RetryDependency.Default);
        }

        internal ApiHandlerOptionsBuilder(HttpClient baseClient, bool disposeClient = true) : this()
        {
            _disposeClient = disposeClient;
            _baseClient = baseClient;
        }

        #endregion

        /// <summary>
        /// The Source the <see cref="ApiHandler"/> will use to make API requests against
        /// </summary>
        /// <param name="source">The source of the Server, EG: http://someservice.com:8080</param>
        /// <param name="resource">The API resource that the <see cref="ApiHandler"/> will interact with</param>
        /// <param name="resourcePath">The Path preceding the Resource. By default this is set to "api/"</param>
        public void UseSource(string source, string resource, string resourcePath = null)
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
            Source = ApiHandlerOptionsHelper.FormatSource(source, resource, resourcePath);
            Resource = resource;
        }

        /// <summary>
        /// Overrides the Client with the supplied <see cref="HttpClient"/> this will disable the supplied Source, Timeout and <see cref="HttpRequestHeaders"/>.
        /// This should only be used for Unit Testing
        /// </summary>
        /// <param name="clientOverride">The <see cref="HttpClient"/> to be used when making API requests.</param>
        public void UseClientOverride(HttpClient clientOverride, bool disposeClient = true)
        {
            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            if (Source != null)
            {
                throw new MethodAccessException("This method cannot be called alongside UseSource");
            }

            if (_baseClient != null)
            {
                throw new MethodAccessException("This method cannot be called when using the ApiHandlerFactory");
            }

            ClientOverride = clientOverride;

            Source = clientOverride.BaseAddress;
            Resource = ApiHandlerOptionsHelper.GetResource(Source);

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
            ApiHandlerOptionsRules.ValidateRetryCount(retryCount);

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
        /// Overrides the default <see cref="DefaultQueryFactory"/> with the supplied <see cref="IQueryFactory"/>
        /// </summary>
        /// <param name="queryFactory">The <see cref="IQueryFactory"/> to be used when building Queries</param>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            DependencyCollection.AddDependency(queryFactory);
        }

        /// <summary>
        /// Overrides the default <see cref="ICredentials"/> used by the <see cref="ApiHandler"/>
        /// </summary>
        /// <param name="credentials">The <see cref="ICredentials"/> to be used when making requests.</param>
        public void AddCredentials(ICredentials credentials)
        {
            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called alongside UseClientOverride");
            }

            Credentials = credentials;
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.BuildOptions"/>
        public virtual IApiHandlerOptions BuildOptions()
        {
            if (ClientOverride != null)
            {
                DependencyCollection.AddDependency(ClientOverride, DisposeClientOverride ? DependencyBinding.Bound : DependencyBinding.Unbound);
            }
            else
            {
                HttpClientHandler clientHandler = new HttpClientHandler
                {
                    Credentials = Credentials
                };

                HttpClient httpClient = _baseClient ?? new HttpClient(clientHandler);

                httpClient.BaseAddress = Source;
                httpClient.Timeout = Timeout;

                RequestHeaderBuilder.Invoke(httpClient.DefaultRequestHeaders);

                DependencyCollection.AddDependency(httpClient, _disposeClient ? DependencyBinding.Bound : DependencyBinding.Unbound);
            }

            IApiHandlerOptions options = new ApiHandlerOptions(DependencyCollection, Source, Resource);

            return options;
        }
    }
}
