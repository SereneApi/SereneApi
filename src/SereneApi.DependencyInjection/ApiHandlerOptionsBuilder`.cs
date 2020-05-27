using DeltaWare.SereneApi.Helpers;
using DeltaWare.SereneApi.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace DeltaWare.SereneApi.DependencyInjection
{
    /// <summary>
    /// The <see cref="ApiHandlerOptionsBuilder{TApiHandler}"/> is used to build new instances of the <see cref="SereneApi.DepedencyInjection.ApiHandlerOptions"/> class
    /// </summary>
    public class ApiHandlerOptionsBuilder<TApiHandler> : ApiHandlerOptionsBuilder where TApiHandler : ApiHandler
    {
        private ILoggerFactory _loggerFactory;

        private IServiceCollection _serviceCollection;


        /// <summary>
        /// Gets the Source, Resource, ResourcePrecursor and Timeout period from the <see cref="IConfiguration"/>.
        /// Note: Source and Resource MUST be supplied if this is used, ResourcePrecursor and Timeout are optional
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> the values will be retrieved from</param>
        public ApiHandlerOptionsBuilder UseConfiguration(IConfiguration configuration)
        {
            #region Configuration Key Values

            // TODO: Move these into a separate class
            const string sourceKey = "Source";
            const string resourceKey = "Resource";
            const string resourcePrecursorKey = "ResourcePrecursor";
            const string timeoutKey = "Timeout";

            #endregion

            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called alongside UseClientOverride");
            }

            if (Source != null)
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            string source = configuration.Get<string>(sourceKey);
            string resource = configuration.Get<string>(resourceKey);
            string resourcePrecursor = configuration.Get<string>(resourcePrecursorKey, false);

            Source = ApiHandlerOptionsHelper.CreateApiSource(source, resource, resourcePrecursor);

            TimeSpan timeout = configuration.Get<TimeSpan>(timeoutKey, false);

            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentException("The Timeout value must be equal to or greater than 0");
            }

            if (timeout != TimeSpan.Zero)
            {
                Timeout = timeout;
            }

            return this;
        }

        /// <summary>
        /// Adds an <see cref="ILoggerFactory"/> to the <see cref="ApiHandler"/> allowing built in Logging
        /// </summary>
        /// <param name="loggerFactory">The <see cref="IQueryFactory"/> to be used for Logging</param>
        public void AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        internal void AddServicesCollection(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public new ApiHandlerOptions<TApiHandler> BuildOptions()
        {
            ILogger<TApiHandler> logger = _loggerFactory.CreateLogger<TApiHandler>();

            DependencyCollection.AddDependency(logger);

            if ()

                ApiHandlerOptions<TApiHandler> options = new ApiHandlerOptions<TApiHandler>(DependencyCollection, Source);

            return options;
        }

        public new ApiHandlerOptions<TApiHandler> BuildApiOptions(IServiceCollection services)
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
