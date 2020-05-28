using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SereneApi.Enums;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Net.Http;

namespace SereneApi.DependencyInjection
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
            if (ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called alongside UseClientOverride");
            }

            if (Source != null)
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            string source = configuration.Get<string>(ConfigurationConstants.SourceKey, ConfigurationConstants.SourceIsRequired);
            string resource = configuration.Get<string>(ConfigurationConstants.ResourceKey, ConfigurationConstants.ResourceIsRequired);
            string resourcePath = configuration.Get<string>(ConfigurationConstants.ResourcePathKey, ConfigurationConstants.ResourcePathIsRequired);

            Source = ApiHandlerOptionsHelper.FormatSource(source, resource, resourcePath);

            TimeSpan timeout = configuration.Get<TimeSpan>(ConfigurationConstants.TimeoutKey, ConfigurationConstants.TimeoutIsRequired);

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

            // Leaving unbound until testing can be done.
            DependencyCollection.AddDependency(_serviceCollection, DependencyBinding.Unbound);

            bool usingClientFactory = Source == null;

            if (usingClientFactory)
            {
                _serviceCollection.AddHttpClient(typeof(TApiHandler).ToString(), client =>
                {
                    client.BaseAddress = Source;
                    client.Timeout = Timeout;

                    RequestHeaderBuilder.Invoke(client.DefaultRequestHeaders);
                });
            }

            ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider();

            DependencyCollection.AddDependency(serviceProvider);

            if (usingClientFactory)
            {
                IHttpClientFactory clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

                // The ClientFactory is Unbound as the Service Provider is controlling its lifetime.
                DependencyCollection.AddDependency(clientFactory, DependencyBinding.Unbound);
                DependencyCollection.AddDependency(clientFactory.CreateClient(typeof(TApiHandler).ToString()));
            }

            ApiHandlerOptions<TApiHandler> options = new ApiHandlerOptions<TApiHandler>(DependencyCollection, Source);

            return options;
        }
    }
}
