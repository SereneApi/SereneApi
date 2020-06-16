using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SereneApi.Enums;
using SereneApi.Extensions.DependencyInjection.Helpers;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Factories;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types;
using SereneApi.Types.Dependencies;
using System;
using System.Net.Http;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IApiHandlerOptionsBuilder{TApiHandler}"/>
    public class ApiHandlerOptionsBuilder<TApiHandler>: ApiHandlerOptionsBuilder, IApiHandlerOptionsBuilder<TApiHandler> where TApiHandler : ApiHandler
    {
        private IServiceCollection _serviceCollection;

        internal ApiHandlerOptionsBuilder(DependencyCollection dependencyCollection) : base(dependencyCollection)
        {
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder{TApiHandler}.UseConfiguration"/>
        public void UseConfiguration(IConfiguration configuration)
        {
            if(ClientOverride != null)
            {
                throw new MethodAccessException("This method cannot be called alongside UseClientOverride");
            }

            if(Source != null)
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            string source = configuration.Get<string>(ConfigurationConstants.SourceKey, ConfigurationConstants.SourceIsRequired);
            string resource = configuration.Get<string>(ConfigurationConstants.ResourceKey, ConfigurationConstants.ResourceIsRequired);
            string resourcePath = configuration.Get<string>(ConfigurationConstants.ResourcePathKey, ConfigurationConstants.ResourcePathIsRequired);

            Source = new Uri(SourceHelpers.EnsureSourceSlashTermination(source));
            Resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
            ResourcePath = ApiHandlerOptionsHelper.UseOrGetDefaultResourcePath(resourcePath);

            DependencyCollection.AddDependency<IRouteFactory>(new RouteFactory(ResourcePath));

            #region Timeout

            TimeSpan timeout = configuration.Get<TimeSpan>(ConfigurationConstants.TimeoutKey, ConfigurationConstants.TimeoutIsRequired);

            if(timeout < TimeSpan.Zero)
            {
                throw new ArgumentException("The Timeout value must be equal to or greater than 0");
            }

            if(timeout != TimeSpan.Zero)
            {
                Timeout = timeout;
            }

            #endregion
            #region Retry Count

            if(configuration.ContainsKey(ConfigurationConstants.RetryCountKey))
            {
                int retryCount = configuration.Get<int>(ConfigurationConstants.RetryCountKey, ConfigurationConstants.RetryIsRequired);

                ApiHandlerOptionsRules.ValidateRetryCount(retryCount);

                RetryDependency retryDependency = new RetryDependency(retryCount);

                DependencyCollection.AddDependency(retryDependency);
            }


            #endregion
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder{TApiHandler}.AddLoggerFactory"/>
        public void AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            ExceptionHelper.EnsureParameterIsNotNull(loggerFactory, nameof(loggerFactory));

            ILogger logger = loggerFactory.CreateLogger<TApiHandler>();

            DependencyCollection.AddDependency(logger);
        }

        /// <summary>
        /// Adds a <see cref="IServiceCollection"/> to the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to be added.</param>
        public void AddServicesCollection(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;

            // Leaving unbound until testing can be done.
            DependencyCollection.AddDependency(_serviceCollection, Binding.Unbound);
        }

        /// <summary>
        /// Builds the <see cref="IApiHandlerOptions"/> for the specified <see cref="ApiHandler"/>.
        /// </summary>
        public new IApiHandlerOptions<TApiHandler> BuildOptions()
        {
            if(DependencyCollection.TryGetDependency(out HttpMessageHandler messageHandler))
            {
                _serviceCollection.AddHttpClient(typeof(TApiHandler).ToString(), client =>
                {
                    client.BaseAddress = Source;
                    client.Timeout = Timeout;

                    RequestHeaderBuilder.Invoke(client.DefaultRequestHeaders);
                }).ConfigurePrimaryHttpMessageHandler(() => messageHandler);
            }
            else
            {
                _serviceCollection.AddHttpClient(typeof(TApiHandler).ToString(), client =>
                {
                    client.BaseAddress = Source;
                    client.Timeout = Timeout;

                    RequestHeaderBuilder.Invoke(client.DefaultRequestHeaders);
                }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    Credentials = Credentials
                });
            }

            ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider();

            DependencyCollection.AddDependency(serviceProvider);

            IHttpClientFactory clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            // The ClientFactory is Unbound as the Service Provider is controlling its lifetime.
            DependencyCollection.AddDependency(clientFactory, Binding.Unbound);
            DependencyCollection.AddDependency(clientFactory.CreateClient(typeof(TApiHandler).ToString()));

            ApiHandlerOptions<TApiHandler> options = new ApiHandlerOptions<TApiHandler>(DependencyCollection, Source, Resource, ResourcePath);

            return options;
        }
    }
}
