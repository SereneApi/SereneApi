using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SereneApi.Extensions.DependencyInjection.Factories;
using SereneApi.Extensions.DependencyInjection.Helpers;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Factories;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Net.Http;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IApiHandlerOptionsBuilder{TApiHandler}"/>
    internal class ApiHandlerOptionsBuilder<TApiHandler>: ApiHandlerOptionsBuilder, IApiHandlerOptionsBuilder<TApiHandler> where TApiHandler : ApiHandler
    {
        public ApiHandlerOptionsBuilder(DependencyCollection dependencyCollection) : base(dependencyCollection)
        {
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

            DependencyCollection.AddDependency(loggerFactory);
        }

        /// <summary>
        /// Builds the <see cref="IApiHandlerOptions"/> for the specified <see cref="ApiHandler"/>.
        /// </summary>
        public IApiHandlerOptions<TApiHandler> BuildOptions(IServiceCollection services)
        {
            IConnectionInfo connection = DependencyCollection.GetDependency<IConnectionInfo>();

            DependencyCollection dependencies = (DependencyCollection)DependencyCollection.Clone();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            DependencyInjectionClientFactory<TApiHandler> factory = new DependencyInjectionClientFactory<TApiHandler>(dependencies);

            dependencies.AddDependency<IClientFactory>(factory);
            dependencies.AddDependency(serviceProvider);
            dependencies.AddDependency(serviceProvider.GetRequiredService<IHttpClientFactory>());

            if(dependencies.TryGetDependency(out ILoggerFactory loggerFactory))
            {
                ILogger logger = loggerFactory.CreateLogger<TApiHandler>();

                dependencies.AddDependency(logger);
            }

            ApiHandlerOptions<TApiHandler> options = new ApiHandlerOptions<TApiHandler>(dependencies, connection);

            return options;
        }
    }
}
