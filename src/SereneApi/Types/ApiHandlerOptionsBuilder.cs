using Microsoft.Extensions.Logging;
using SereneApi.Enums;
using SereneApi.Factories;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Serializers;
using SereneApi.Types.Dependencies;
using SereneApi.Types.Headers.Accept;
using SereneApi.Types.Headers.Authentication;
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

        protected TimeSpan Timeout = ApiHandlerOptionDefaults.TimeoutPeriod;

        #endregion
        #region Constructors

        public ApiHandlerOptionsBuilder()
        {
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            DependencyCollection.AddDependency(JsonSerializer.Default);
            DependencyCollection.AddDependency(RetryDependency.Default);
            DependencyCollection.AddDependency<IRouteFactory>(new RouteFactory());
            DependencyCollection.AddDependency(ContentType.Json);
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.Credentials);
        }

        protected ApiHandlerOptionsBuilder(DependencyCollection dependencyCollection) : base(dependencyCollection)
        {
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            DependencyCollection.AddDependency(JsonSerializer.Default);
            DependencyCollection.AddDependency(RetryDependency.Default);
            DependencyCollection.AddDependency<IRouteFactory>(new RouteFactory());
            DependencyCollection.AddDependency(ContentType.Json);
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.Credentials);
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

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseQueryFactory"/>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            DependencyCollection.AddDependency(queryFactory);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseCredentials"/>
        public void UseCredentials(ICredentials credentials)
        {
            DependencyCollection.AddDependency(credentials);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddAuthentication"/>
        public void AddAuthentication(IAuthentication authentication)
        {
            if(DependencyCollection.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            DependencyCollection.AddDependency(authentication);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddBasicAuthentication"/>
        public void AddBasicAuthentication(string username, string password)
        {
            if(DependencyCollection.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            DependencyCollection.AddDependency<IAuthentication>(new BasicAuthentication(username, password));
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddBearerAuthentication"/>
        public void AddBearerAuthentication(string token)
        {
            if(DependencyCollection.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            DependencyCollection.AddDependency<IAuthentication>(new BearerAuthentication(token));
        }

        public void AcceptContentType(ContentType content)
        {
            DependencyCollection.AddDependency(content);
        }

        public IApiHandlerOptions BuildOptions()
        {
            HttpClient client;

            if(_baseClient != null)
            {
                client = _baseClient;
            }
            else
            {
                if(!DependencyCollection.TryGetDependency(out HttpClientHandler clientHandler))
                {
                    ICredentials credentials = DependencyCollection.GetDependency<ICredentials>();

                    clientHandler = new HttpClientHandler
                    {
                        Credentials = credentials
                    };
                }

                client = new HttpClient(clientHandler);
            }

            client.BaseAddress = Source;
            client.Timeout = Timeout;
            client.DefaultRequestHeaders.Accept.Clear();

            if(DependencyCollection.TryGetDependency(out IAuthentication authentication))
            {
                AuthenticationHeaderValue authenticationHeader = new AuthenticationHeaderValue(authentication.Scheme, authentication.Parameter);

                client.DefaultRequestHeaders.Authorization = authenticationHeader;
            }

            if(DependencyCollection.TryGetDependency(out ContentType contentType))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.Value));
            }

            DependencyCollection.AddDependency(client, _disposeClient ? Binding.Bound : Binding.Unbound);

            IApiHandlerOptions options = new ApiHandlerOptions(DependencyCollection, Source, Resource, ResourcePath);

            return options;
        }
    }
}
