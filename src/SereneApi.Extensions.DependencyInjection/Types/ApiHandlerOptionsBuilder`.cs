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
        public ApiHandlerOptionsBuilder(DependencyCollection dependencies) : base(dependencies)
        {
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder{TApiHandler}.UseConfiguration"/>
        public void UseConfiguration(IConfiguration configuration)
        {
            if(Dependencies.HasDependency<IConnectionSettings>())
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            string source = configuration.Get<string>(ConfigurationConstants.SourceKey, ConfigurationConstants.SourceIsRequired);
            string resource = configuration.Get<string>(ConfigurationConstants.ResourceKey, ConfigurationConstants.ResourceIsRequired);
            string resourcePath = configuration.Get<string>(ConfigurationConstants.ResourcePathKey, ConfigurationConstants.ResourcePathIsRequired);

            IConnectionSettings connection = new Connection(source, resource, resourcePath);

            Dependencies.AddDependency(connection);
            Dependencies.AddDependency<IRouteFactory>(new RouteFactory(connection));

            #region Timeout

            int timeout = configuration.Get<int>(ConfigurationConstants.TimeoutKey, ConfigurationConstants.TimeoutIsRequired);

            if(timeout < 0)
            {
                throw new ArgumentException("The Timeout value must be equal to or greater than 0");
            }

            if(timeout != default)
            {
                connection.SetTimeout(timeout);
            }

            #endregion
            #region Retry Count

            if(configuration.ContainsKey(ConfigurationConstants.RetryCountKey))
            {
                int retryCount = configuration.Get<int>(ConfigurationConstants.RetryCountKey, ConfigurationConstants.RetryIsRequired);

                if(retryCount != default)
                {
                    connection.SetRetryAttempts(retryCount);
                }
            }

            Dependencies.AddDependency(connection);

            #endregion
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder{TApiHandler}.AddLoggerFactory"/>
        public void AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            ExceptionHelper.EnsureParameterIsNotNull(loggerFactory, nameof(loggerFactory));

            Dependencies.AddDependency(loggerFactory);
        }

        /// <summary>
        /// Builds the <see cref="IApiHandlerOptions"/> for the specified <see cref="ApiHandler"/>.
        /// </summary>
        public IApiHandlerOptions<TApiHandler> BuildOptions(IServiceCollection services)
        {
            IConnectionSettings connection = Dependencies.GetDependency<IConnectionSettings>();

            DependencyCollection dependencies = (DependencyCollection)Dependencies.Clone();

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
