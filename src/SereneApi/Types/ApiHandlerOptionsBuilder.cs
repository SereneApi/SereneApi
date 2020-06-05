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
    public class ApiHandlerOptionsBuilder : CoreOptions, IApiHandlerOptionsBuilder
    {
        #region Variables

        private readonly HttpClient _baseClient;

        private readonly bool _disposeClient = true;

        #endregion
        #region Properties

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

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseSource"/>
        public void UseSource(string source, string resource, string resourcePath = null)
        {
            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called after UseClientOverride");
            }

            if (Source != null)
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            // The Resource Precursors default value will be used if a null or whitespace value is provided.
            Source = ApiHandlerOptionsHelper.FormatSource(source, resource, resourcePath);
            Resource = resource;
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseClientOverride"/>
        public void UseClientOverride(HttpClient clientOverride, bool disposeClient = true)
        {
            if (_baseClient != null)
            {
                throw new MethodAccessException("This method cannot be called when creating a ApiHandler from an HttpClient");
            }

            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            if (Source != null)
            {
                throw new MethodAccessException("This method cannot be called after UseSource");
            }

            if (DependencyCollection.HasDependency<HttpClientHandler>())
            {
                throw new MethodAccessException("This method cannot be called after UseHttpClientHandler");
            }

            if (DependencyCollection.HasDependency<HttpMessageHandler>())
            {
                throw new MethodAccessException("This method cannot be called after UseHttpMessageHandler");
            }

            ClientOverride = clientOverride;

            Source = clientOverride.BaseAddress;
            Resource = null;

            DisposeClientOverride = disposeClient;
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.SetTimeoutPeriod"/>
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
        public void SetRetryOnTimeout(uint retryCount)
        {
            ApiHandlerOptionsRules.ValidateRetryCount(retryCount);

            DependencyCollection.AddDependency(new RetryDependency(retryCount));
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseHttpRequestHeaders"/>
        public void UseHttpRequestHeaders(Action<HttpRequestHeaders> requestHeaderBuilder)
        {
            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called after UseClientOverride");
            }

            RequestHeaderBuilder = requestHeaderBuilder;
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerOptionsBuilder.UseJsonSerializerOptions</cref>
        /// </inheritdoc>
        public void UseJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
        {
            DependencyCollection.AddDependency(jsonSerializerOptions);
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerOptionsBuilder.UseJsonSerializerOptions</cref>
        /// </inheritdoc>
        public void UseJsonSerializerOptions(Action<JsonSerializerOptions> builder)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();

            builder?.Invoke(options);

            UseJsonSerializerOptions(options);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseQueryFactory"/>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            DependencyCollection.AddDependency(queryFactory);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseCredentials"/>
        public void UseCredentials(ICredentials credentials)
        {
            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called after UseClientOverride");
            }

            Credentials = credentials;
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseHttpMessageHandler"/>
        public void UseHttpMessageHandler(HttpMessageHandler httpMessageHandler)
        {
            if (_baseClient != null)
            {
                throw new MethodAccessException("This method cannot be called when creating a ApiHandler from an HttpClient");
            }

            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called after UseClientOverride");
            }

            if (DependencyCollection.HasDependency<HttpClientHandler>())
            {
                throw new MethodAccessException("This method cannot be called after UseHttpClientHandler");
            }

            DependencyCollection.AddDependency(httpMessageHandler, Binding.Unbound);
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerOptionsBuilder.UseHttpClientHandler</cref>
        /// </inheritdoc>
        public void UseHttpClientHandler(HttpClientHandler httpClientHandler, bool overrideUseCredentials = false)
        {
            if (_baseClient != null)
            {
                throw new MethodAccessException("This method cannot be called when creating a ApiHandler from an HttpClient");
            }

            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called after UseClientOverride");
            }

            if (DependencyCollection.HasDependency<HttpMessageHandler>())
            {
                throw new MethodAccessException("This method cannot be called after UseHttpMessageHandler");
            }

            // Unbound as the HttpClient controls its lifetime.
            DependencyCollection.AddDependency(httpClientHandler, Binding.Unbound);

            OverrideUseCredentials = overrideUseCredentials;
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerOptionsBuilder.UseHttpClientHandler</cref>
        /// </inheritdoc>
        public void UseHttpClientHandler(Action<HttpClientHandler> builder, bool overrideUseCredentials = false)
        {
            HttpClientHandler handler = new HttpClientHandler();

            builder.Invoke(handler);

            UseHttpClientHandler(handler, overrideUseCredentials);
        }

        public IApiHandlerOptions BuildOptions()
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
