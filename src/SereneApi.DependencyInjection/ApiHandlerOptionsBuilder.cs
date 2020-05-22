using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using DeltaWare.SereneApi.Interfaces;

namespace DeltaWare.SereneApi.DependencyInjection
{
    /// <summary>
    /// The <see cref="ApiHandlerOptionsBuilder"/> is used to build new instances of the <see cref="ApiHandlerOptions"/> class
    /// </summary>
    public class ApiHandlerOptionsBuilder
    {
        /// <summary>
        /// The <see cref="ApiHandlerOptions"/> that will be used by the <see cref="ApiHandler"/>
        /// </summary>
        protected ApiHandlerOptions Options { get; }

        /// <summary>
        /// Instantiates a new instance of the <see cref="ApiHandlerOptionsBuilder"/> class
        /// </summary>
        /// <param name="options">The <see cref="ApiHandlerOptions"/> to be configured by the <see cref="ApiHandlerOptionsBuilder"/></param>
        public ApiHandlerOptionsBuilder(ApiHandlerOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// Gets the Source, Resource, ResourcePrecursor and Timeout period from the <see cref="IConfiguration"/>.
        /// Note: Source and Resource MUST be supplied if this is used, ResourcePrecursor and Timeout are optional
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> the values will be retrieved from</param>
        public ApiHandlerOptionsBuilder UseConfiguration(IConfiguration configuration)
        {
            Options.UseConfiguration(configuration);

            return this;
        }

        /// <summary>
        /// The Source the <see cref="ApiHandler"/> will use to make API requests against
        /// </summary>
        /// <param name="source">The source of the Server, EG: http://someservice.com:8080</param>
        /// <param name="resource">The API resource that the <see cref="ApiHandler"/> will interact with</param>
        /// <param name="resourcePrecursor">The Resource Precursor this applied before the Resource. By default this is set to "api/"</param>
        public ApiHandlerOptionsBuilder UseSource(Uri source, string resource, string resourcePrecursor = null)
        {
            Options.UseSource(source, resource, resourcePrecursor);

            return this;
        }

        /// <summary>
        /// Sets the timeout to be used by the <see cref="ApiHandler"/> when making API requests. By default this value is set to 30 seconds
        /// </summary>
        /// <param name="timeoutPeriod">The <see cref="TimeSpan"/> to be used as the timeout period by the <see cref="ApiHandler"/></param>
        public ApiHandlerOptionsBuilder SetTimeoutPeriod(TimeSpan timeoutPeriod)
        {
            Options.SetTimeoutPeriod(timeoutPeriod);

            return this;
        }

        /// <summary>
        /// Overrides the Client with the supplied <see cref="HttpClient"/> this will disable the supplied Source, Timeout and <see cref="HttpRequestHeaders"/>.
        /// This should only be used for Unit Testing
        /// </summary>
        /// <param name="clientOverride">The <see cref="HttpClient"/> to be used when making API requests.</param>
        public ApiHandlerOptionsBuilder UseClientOverride(HttpClient clientOverride)
        {
            Options.UseClientOverride(clientOverride);

            return this;
        }

        /// <summary>
        /// Adds an <see cref="ILoggerFactory"/> to the <see cref="ApiHandler"/> allowing built in Logging
        /// </summary>
        /// <param name="loggerFactory">The <see cref="IQueryFactory"/> to be used for Logging</param>
        public ApiHandlerOptionsBuilder AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            Options.AddLoggerFactory(loggerFactory);

            return this;
        }

        /// <summary>
        /// Overrides the default <see cref="QueryFactory"/> with the supplied <see cref="IQueryFactory"/>
        /// </summary>
        /// <param name="queryFactory">The <see cref="IQueryFactory"/> to be used when building Queries</param>
        public ApiHandlerOptionsBuilder UseQueryFactory(IQueryFactory queryFactory)
        {
            Options.UseQueryFactory(queryFactory);

            return this;
        }

        /// <summary>
        /// Overrides the default <see cref="HttpResponseHeaders"/> with the supplied <see cref="HttpResponseHeaders"/>
        /// </summary>
        /// <param name="requestHeaderBuilder">Builds the <see cref="HttpResponseHeaders"/></param>
        public ApiHandlerOptionsBuilder UseHttpRequestHeaders(Action<HttpRequestHeaders> requestHeaderBuilder)
        {
            Options.UseRequestHeaders(requestHeaderBuilder);

            return this;
        }

        /// <summary>
        /// When set, upon a timeout the <see cref="ApiHandler"/> will re-attempt the request. By Default this is disabled
        /// </summary>
        /// <param name="retryCount">How many times the <see cref="ApiHandler"/> will re-attempt the request</param>
        public ApiHandlerOptionsBuilder RetryOnTimeout(in uint retryCount)
        {
            Options.RetryOnTimeout(retryCount);

            return this;
        }

        public ApiHandlerOptions BuildApiOptions(IServiceCollection services)
        {
            bool usingClientFactory = false;

            if (Options.HttpClient == null)
            {
                services.AddHttpClient(Options.HandlerType.ToString(), client =>
                {
                    client.BaseAddress = new Uri($"{Options.Source}/{Options.ResourcePrecursor}{Options.Resource}");
                    client.Timeout = Options.Timeout;

                    Options.RequestHeaderBuilder.Invoke(client.DefaultRequestHeaders);
                });

                usingClientFactory = true;
            }

            IServiceProvider internalServiceProvider = services.BuildServiceProvider();

            Options.AddServiceProvider(internalServiceProvider);

            if (usingClientFactory)
            {
                Options.AddHttpClientFactory(internalServiceProvider.GetRequiredService<IHttpClientFactory>());
            }

            return Options;
        }
    }
}
