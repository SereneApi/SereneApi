using Microsoft.Extensions.Logging;
using SereneApi.Enums;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types.Dependencies;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SereneApi.Types
{
    public class ApiHandlerOptionsBuilder
    {
        #region Variables

        private readonly HttpClient _baseClient;

        private readonly bool _disposeClient = true;

        #endregion
        #region Properties

        protected DependencyCollection DependencyCollection { get; }

        protected bool OverrideUseCredentials { get; private set; }

        protected Uri Source { get; set; }

        protected string Resource { get; set; }

        protected HttpClient ClientOverride { get; set; }

        protected bool DisposeClientOverride { get; set; }

        protected Action<HttpRequestHeaders> RequestHeaderBuilder = ApiHandlerOptionDefaults.RequestHeadersBuilder;

        protected TimeSpan Timeout = ApiHandlerOptionDefaults.TimeoutPeriod;

        protected ICredentials Credentials { get; set; } = ApiHandlerOptionDefaults.Credentials;

        #endregion
        #region Constructors

        public ApiHandlerOptionsBuilder()
        {
            DependencyCollection = new DependencyCollection();

            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.JsonSerializerOptionsBuilder);
            DependencyCollection.AddDependency(RetryDependency.Default);
        }

        internal ApiHandlerOptionsBuilder(HttpClient baseClient, bool disposeClient = true) : this()
        {
            _disposeClient = disposeClient;
            _baseClient = baseClient;
        }

        #endregion

        /// <summary>
        /// The Source the <see cref="ApiHandler"/> will use to make API requests against.
        /// </summary>
        /// <param name="source">The source of the Server, EG: http://someservice.com:8080</param>
        /// <param name="resource">The API resource that the <see cref="ApiHandler"/> will interact with.</param>
        /// <param name="resourcePath">The Path preceding the Resource. By default this is set to "api/".</param>
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
                throw new MethodAccessException("This method cannot be called when creating a ApiHandler from an HttpClient");
            }

            ClientOverride = clientOverride;

            Source = clientOverride.BaseAddress;
            Resource = null;

            DisposeClientOverride = disposeClient;
        }

        /// <summary>
        /// Sets the timeout to be used by the <see cref="ApiHandler"/> when making API requests. By default this value is set to 30 seconds.
        /// </summary>
        /// <param name="timeoutPeriod">The <see cref="TimeSpan"/> to be used as the timeout period by the <see cref="ApiHandler"/>.</param>
        public void SetTimeoutPeriod(TimeSpan timeoutPeriod)
        {
            Timeout = timeoutPeriod;
        }

        /// <summary>
        /// Adds an <see cref="ILogger"/> to the <see cref="ApiHandler"/> allowing built in Logging.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to be used for Logging.</param>
        public void AddLogger(ILogger logger)
        {
            DependencyCollection.AddDependency(logger);
        }

        /// <summary>
        /// When set, upon a timeout the <see cref="ApiHandler"/> will re-attempt the request. By Default this is disabled.
        /// </summary>
        /// <param name="retryCount">How many times the <see cref="ApiHandler"/> will re-attempt the request.</param>
        public void SetRetryOnTimeout(uint retryCount)
        {
            ApiHandlerOptionsRules.ValidateRetryCount(retryCount);

            DependencyCollection.AddDependency(new RetryDependency(retryCount));
        }

        /// <summary>
        /// Overrides the default <see cref="HttpResponseHeaders"/> with the supplied <see cref="HttpResponseHeaders"/>.
        /// </summary>
        public void UseHttpRequestHeaders(Action<HttpRequestHeaders> requestHeaderBuilder)
        {
            RequestHeaderBuilder = requestHeaderBuilder;
        }

        /// <summary>
        /// Overrides the default <see cref="JsonSerializerOptions"/> with the supplied <see cref="JsonSerializerOptions"/>.
        /// </summary>
        public void UseJsonSerializerOptions(Action<JsonSerializerOptions> builder)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();

            builder?.Invoke(options);

            DependencyCollection.AddDependency(builder);
        }

        /// <summary>
        /// Overrides the default <see cref="IQueryFactory"/> with the supplied <see cref="IQueryFactory"/>.
        /// </summary>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            DependencyCollection.AddDependency(queryFactory);
        }

        /// <summary>
        /// Overrides the default <see cref="ICredentials"/> used by the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="credentials">The <see cref="ICredentials"/> to be used when making requests.</param>
        public void UseCredentials(ICredentials credentials)
        {
            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called alongside UseClientOverride");
            }

            Credentials = credentials;
        }

        /// <summary>
        /// Overrides the default <see cref="HttpClientHandler"/> used by the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="overrideUseCredentials">If False the Credentials supplied by the UseCredentials method will be used.</param>
        public void UseHttpMessageHandler(Action<HttpClientHandler> builder, bool overrideUseCredentials = false)
        {
            if (_baseClient != null)
            {
                throw new MethodAccessException("This method cannot be called when creating a ApiHandler from an HttpClient");
            }

            HttpClientHandler handler = new HttpClientHandler();

            builder.Invoke(handler);

            // Unbound as the HttpClient controls its lifetime.
            DependencyCollection.AddDependency(handler, Binding.Unbound);

            OverrideUseCredentials = overrideUseCredentials;
        }

        internal IApiHandlerOptions BuildOptions()
        {
            if (ClientOverride != null)
            {
                DependencyCollection.AddDependency(ClientOverride, DisposeClientOverride ? Binding.Bound : Binding.Unbound);
            }
            else
            {
                HttpClient httpClient;

                if (_baseClient != null)
                {
                    httpClient = _baseClient;
                }
                else
                {
                    bool hasHttpClientHandler =
                        DependencyCollection.TryGetDependency(out HttpClientHandler clientHandler);

                    if (!hasHttpClientHandler)
                    {
                        clientHandler = new HttpClientHandler();
                    }

                    if (!(hasHttpClientHandler && OverrideUseCredentials))
                    {
                        clientHandler.Credentials = Credentials;
                    }

                    httpClient = new HttpClient(clientHandler);
                }

                httpClient.BaseAddress = Source;
                httpClient.Timeout = Timeout;

                RequestHeaderBuilder.Invoke(httpClient.DefaultRequestHeaders);

                DependencyCollection.AddDependency(httpClient, _disposeClient ? Binding.Bound : Binding.Unbound);
            }

            IApiHandlerOptions options = new ApiHandlerOptions(DependencyCollection, Source, Resource);

            return options;
        }
    }
}
