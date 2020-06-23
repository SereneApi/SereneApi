using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SereneApi.Enums;
using SereneApi.Extensions.DependencyInjection.Helpers;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Factories;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IApiHandlerOptionsBuilder{TApiHandler}"/>
    internal class ApiHandlerOptionsBuilder<TApiHandler>: ApiHandlerOptionsBuilder, IApiHandlerOptionsBuilder<TApiHandler> where TApiHandler : ApiHandler
    {
        private IServiceCollection _serviceCollection;

        public ApiHandlerOptionsBuilder()
        {
        }

        public ApiHandlerOptionsBuilder(DependencyCollection dependencyCollection, IServiceCollection serviceCollection) : base(dependencyCollection)
        {
            DependencyCollection.AddDependency(serviceCollection);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder{TApiHandler}.UseConfiguration"/>
        public void UseConfiguration(IConfiguration configuration)
        {
            if(DependencyCollection.HasDependency<IConnectionInfo>())
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            string source = configuration.Get<string>(ConfigurationConstants.SourceKey, ConfigurationConstants.SourceIsRequired);
            string resource = configuration.Get<string>(ConfigurationConstants.ResourceKey, ConfigurationConstants.ResourceIsRequired);
            string resourcePath = configuration.Get<string>(ConfigurationConstants.ResourcePathKey, ConfigurationConstants.ResourcePathIsRequired);

            IConnectionInfo connectionInfo = new ConnectionInfo(source, resource, resourcePath);

            DependencyCollection.AddDependency(connectionInfo);
            DependencyCollection.AddDependency<IRouteFactory>(new RouteFactory(connectionInfo));

            #region Timeout

            int timeout = configuration.Get<int>(ConfigurationConstants.TimeoutKey, ConfigurationConstants.TimeoutIsRequired);

            if(timeout < 0)
            {
                throw new ArgumentException("The Timeout value must be equal to or greater than 0");
            }

            if(timeout != default)
            {
                connectionInfo.SetTimeout(timeout);
            }

            #endregion
            #region Retry Count

            if(configuration.ContainsKey(ConfigurationConstants.RetryCountKey))
            {
                int retryCount = configuration.Get<int>(ConfigurationConstants.RetryCountKey, ConfigurationConstants.RetryIsRequired);

                if(retryCount != default)
                {
                    connectionInfo.SetRetryAttempts(retryCount);
                }
            }

            DependencyCollection.AddDependency(connectionInfo);

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
            IConnectionInfo connection = DependencyCollection.GetDependency<IConnectionInfo>();

            ApiHandlerOptions<TApiHandler> options = new ApiHandlerOptions<TApiHandler>(DependencyCollection, connection);

            return options;
        }
    }
}
