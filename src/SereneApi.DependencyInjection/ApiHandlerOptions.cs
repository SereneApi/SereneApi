using DeltaWare.SereneApi.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DeltaWare.SereneApi.DependencyInjection
{
    /// <summary>
    /// The <see cref="ApiHandlerOptions"/> to be used by the <see cref="ApiHandler"/> when making API requests
    /// </summary>
    public class ApiHandlerOptions : IApiHandlerOptions, IDisposable
    {
        private IHttpClientFactory _httpClientFactory;

        private HttpClient _clientOverride;

        #region Public Properties

        /// <summary>
        /// The API Source location
        /// EG: "http://SomeHost.com.au"
        /// </summary>
        public Uri Source { get; private set; }

        /// <summary>
        /// The API <see cref="Resource"/> to be used  by the <see cref="ApiHandler"/> for API requests
        /// EG: "Members", by default api/ is appended to the front therefore Members would become api/Members.
        /// To change this, supply a <see cref="ResourcePrecursor"/>
        /// </summary>
        public string Resource { get; private set; }

        /// <summary>
        /// The API <see cref="ResourcePrecursor"/> will be appended to the front of the <see cref="Resource"/> Value.
        /// By default "api/" is being used
        /// </summary>
        public string ResourcePrecursor { get; private set; } = ApiHandlerOptionDefaults.ResourcePrecursor;

        /// <summary>
        /// Supplies the the HttpClients to be used for requests.
        /// </summary>
        public TimeSpan Timeout { get; private set; } = ApiHandlerOptionDefaults.TimeoutPeriod;

        /// <inheritdoc cref="IApiHandlerOptions.Logger"/>
        public ILogger Logger { get; private set; }

        /// <inheritdoc cref="IApiHandlerOptions.QueryFactory"/>
        public IQueryFactory QueryFactory { get; private set; } = ApiHandlerOptionDefaults.QueryFactory;

        public IServiceProvider ServiceProvider { get; private set; }

        /// <inheritdoc cref="IApiHandlerOptions.HttpClient"/>
        public HttpClient HttpClient
        {
            get
            {
                if (_clientOverride != null)
                {
                    return _clientOverride;
                }

                return _httpClientFactory?.CreateClient(HandlerType.ToString());
            }
        }

        /// <inheritdoc cref="IApiHandlerOptions.RetryCount"/>
        public uint RetryCount { get; private set; }

        /// <summary>
        /// The <see cref="Type"/> of <see cref="ApiHandler"/> these <see cref="IApiHandlerOptions"/> will be used for.
        /// </summary>
        public virtual Type HandlerType { get; } = typeof(ApiHandler);

        public Action<HttpRequestHeaders> RequestHeaderBuilder { get; private set; } = ApiHandlerOptionDefaults.DefaultRequestHeadersBuilder;

        #endregion
        #region Public Methods

        /// <summary>
        /// Gets the Source, Resource, ResourcePrecursor and Timeout period from the <see cref="IConfiguration"/>.
        /// Note: Source and Resource are required, ResourcePrecursor and Timeout are optional
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> the values will be retrieved from</param>
        internal void UseConfiguration(IConfiguration configuration)
        {
            #region Configuration Key Values

            const string sourceKey = "Source";
            const string resourceKey = "Resource";
            const string resourcePrecursorKey = "ResourcePrecursor";
            const string timeoutKey = "Timeout";

            #endregion

            if (Source != null || Resource != null)
            {
                throw new ArgumentException("This method cannot be called twice");
            }

            Source = configuration.Get<Uri>(sourceKey);
            Resource = configuration.Get<string>(resourceKey);

            string precursor = configuration.Get<string>(resourcePrecursorKey, false);

            // If the precursor is null, we don't want to set it. But if it is an empty string we use it.
            if (precursor != null)
            {
                ResourcePrecursor = precursor;
            }

            TimeSpan timeout = configuration.Get<TimeSpan>(timeoutKey, false);

            if (timeout != TimeSpan.Zero)
            {
                Timeout = timeout;
            }
            else if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentException("The Timeout value must be equal to or greater than 0");
            }
        }

        /// <summary>
        /// The Source the <see cref="ApiHandler"/> will use to make API requests against
        /// </summary>
        /// <param name="source">The source of the Server, EG: http://someservice.com:8080</param>
        /// <param name="resource">The API resource that the <see cref="ApiHandler"/> will interact with</param>
        /// <param name="resourcePrecursor">The Resource Precursor this applied before the Resource. By default this is set to "api/"</param>
        internal void UseSource(Uri source, string resource, string resourcePrecursor = null)
        {
            if (Source != null || Resource != null)
            {
                throw new ArgumentException("This method cannot be called twice");
            }

            Source = source;
            Resource = resource;

            if (resource != null)
            {
                ResourcePrecursor = resourcePrecursor;
            }
        }

        /// <summary>
        /// Sets the timeout to be used by the <see cref="ApiHandler"/> when making API requests. By default this value is set to 30 seconds
        /// </summary>
        /// <param name="timeoutPeriod">The <see cref="TimeSpan"/> to be used as the timeout period by the <see cref="ApiHandler"/></param>
        internal void SetTimeoutPeriod(TimeSpan timeoutPeriod)
        {
            Timeout = timeoutPeriod;
        }

        /// <summary>
        /// Overrides the Client Builder with the supplied <see cref="HttpClient"/> this will disable the supplied Source and <see cref="HttpRequestHeaders"/>.
        /// This should only be used for Unit Testing
        /// </summary>
        /// <param name="clientOverride">The <see cref="HttpClient"/> to be used when making API requests.</param>
        internal void UseClientOverride(HttpClient clientOverride)
        {
            if (_httpClientFactory != null)
            {
                throw new ArgumentException("A Client Override cannot be used alongside an HttpClientFactory");
            }

            _clientOverride = clientOverride;
        }

        /// <summary>
        /// Adds an <see cref="ILoggerFactory"/> to the <see cref="ApiHandler"/> allowing built in Logging
        /// </summary>
        /// <param name="loggerFactory">The <see cref="IQueryFactory"/> to be used for Logging</param>
        internal void AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(HandlerType);
        }

        /// <summary>
        /// Overrides the default <see cref="SereneApi.QueryFactory"/> with the supplied <see cref="IQueryFactory"/>
        /// </summary>
        /// <param name="queryFactory">The <see cref="IQueryFactory"/> to be used when building Queries</param>
        internal void UseQueryFactory(IQueryFactory queryFactory)
        {
            QueryFactory = queryFactory;
        }

        /// <summary>
        /// Overrides the default <see cref="HttpResponseHeaders"/> with the supplied <see cref="HttpResponseHeaders"/>
        /// </summary>
        /// <param name="requestHeaderBuilder">Builds the <see cref="HttpResponseHeaders"/></param>
        internal void UseRequestHeaders(Action<HttpRequestHeaders> requestHeaderBuilder)
        {
            RequestHeaderBuilder = requestHeaderBuilder;
        }

        /// <summary>
        /// When set, upon a timeout the <see cref="ApiHandler"/> will re-attempt the request. By Default this is disabled
        /// </summary>
        /// <param name="retryCount">How many times the <see cref="ApiHandler"/> will re-attempt the request</param>
        internal void RetryOnTimeout(in uint retryCount)
        {
            RetryCount = retryCount;
        }

        internal void AddServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        internal void AddHttpClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        #endregion
        #region IDisposable

        private volatile bool _disposed;

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (ServiceProvider is IDisposable disposableServiceProvider)
                {
                    disposableServiceProvider.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
