using Microsoft.Extensions.Logging;
using SereneApi.Enums;
using SereneApi.Factories;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Serializers;
using SereneApi.Types.Dependencies;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SereneApi.Types
{
    public class ApiHandlerOptionsBuilder: CoreOptions, IApiHandlerOptionsBuilder
    {
        #region Variables

        private readonly HttpClient _baseClient;

        private readonly bool _disposeClient;

        #endregion
        #region Properties

        protected Uri Source { get; set; }

        protected string Resource { get; set; }

        protected string ResourcePath { get; set; }

        protected Action<HttpRequestHeaders> RequestHeaderBuilder = ApiHandlerOptionDefaults.RequestHeadersBuilder;

        protected TimeSpan Timeout = ApiHandlerOptionDefaults.TimeoutPeriod;

        protected ICredentials Credentials { get; set; } = ApiHandlerOptionDefaults.Credentials;

        #endregion
        #region Constructors

        public ApiHandlerOptionsBuilder()
        {
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            DependencyCollection.AddDependency(JsonSerializer.Default);
            DependencyCollection.AddDependency(RetryDependency.Default);
            DependencyCollection.AddDependency<IRouteFactory>(new RouteFactory());
        }

        protected ApiHandlerOptionsBuilder(DependencyCollection dependencyCollection) : base(dependencyCollection)
        {
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            DependencyCollection.AddDependency(JsonSerializer.Default);
            DependencyCollection.AddDependency(RetryDependency.Default);
            DependencyCollection.AddDependency<IRouteFactory>(new RouteFactory());
        }

        internal ApiHandlerOptionsBuilder(HttpClient baseClient, bool disposeClient = true) : this()
        {
            _disposeClient = disposeClient;
            _baseClient = baseClient;

            Source = baseClient.BaseAddress;
        }

        internal ApiHandlerOptionsBuilder(DependencyCollection dependencyCollection, HttpClient baseClient, bool disposeClient = true) : this(dependencyCollection)
        {
            _disposeClient = disposeClient;
            _baseClient = baseClient;

            Source = baseClient.BaseAddress;
        }

        #endregion

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseSource"/>
        public void UseSource(string source, string resource = null, string resourcePath = null)
        {
            ExceptionHelper.EnsureParameterIsNotNull(source, nameof(source));

            if(Source != null)
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            Source = new Uri(SourceHelpers.EnsureSourceSlashTermination(source));
            Resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
            ResourcePath = ApiHandlerOptionsHelper.UseOrGetDefaultResourcePath(resourcePath);

            DependencyCollection.AddDependency<IRouteFactory>(new RouteFactory(ResourcePath));
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerOptionsBuilder.SetTimeoutPeriod</cref>
        /// </inheritdoc>
        public void SetTimeoutPeriod(int seconds)
        {
            SetTimeoutPeriod(TimeSpan.FromSeconds(seconds));
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerOptionsBuilder.SetTimeoutPeriod</cref>
        /// </inheritdoc>
        public void SetTimeoutPeriod(TimeSpan timeoutPeriod)
        {
            Timeout = timeoutPeriod;
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddLogger"/>
        public void AddLogger(ILogger logger)
        {
            DependencyCollection.AddDependency(logger);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.SetRetryOnTimeout"/>
        public void SetRetryOnTimeout(int retryCount)
        {
            ApiHandlerOptionsRules.ValidateRetryCount(retryCount);

            DependencyCollection.AddDependency(new RetryDependency(retryCount));
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseHttpRequestHeaders"/>
        public void UseHttpRequestHeaders(Action<HttpRequestHeaders> requestHeaderBuilder)
        {
            RequestHeaderBuilder = requestHeaderBuilder;
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseQueryFactory"/>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            DependencyCollection.AddDependency(queryFactory);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseCredentials"/>
        public void UseCredentials(ICredentials credentials)
        {
            Credentials = credentials;
        }

        public IApiHandlerOptions BuildOptions()
        {
            HttpClient httpClient;

            if(_baseClient != null)
            {
                httpClient = _baseClient;
            }
            else
            {
                if(!DependencyCollection.TryGetDependency(out HttpClientHandler clientHandler))
                {
                    clientHandler = new HttpClientHandler();
                }

                httpClient = new HttpClient(clientHandler);
            }

            httpClient.BaseAddress = Source;
            httpClient.Timeout = Timeout;

            RequestHeaderBuilder.Invoke(httpClient.DefaultRequestHeaders);

            DependencyCollection.AddDependency(httpClient, _disposeClient ? Binding.Bound : Binding.Unbound);

            IApiHandlerOptions options = new ApiHandlerOptions(DependencyCollection, Source, Resource, ResourcePath);

            return options;
        }
    }
}
