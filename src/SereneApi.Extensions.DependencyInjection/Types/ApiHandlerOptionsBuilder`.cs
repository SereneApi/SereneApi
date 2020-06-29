using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SereneApi.Extensions.DependencyInjection.Helpers;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Factories;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IApiHandlerOptionsBuilder{TApiHandler}"/>
    internal class ApiHandlerOptionsBuilder<TApiHandler>: ApiHandlerOptionsBuilder, IApiHandlerOptionsBuilder<TApiHandler> where TApiHandler : ApiHandler
    {
        public ApiHandlerOptionsBuilder(IDependencyCollection dependencies) : base(dependencies)
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

            Connection = new Connection(source, resource, resourcePath);

            #region Timeout

            int timeout = configuration.Get<int>(ConfigurationConstants.TimeoutKey, ConfigurationConstants.TimeoutIsRequired);

            if(timeout < 0)
            {
                throw new ArgumentException("The Timeout value must be greater than 0");
            }

            if(timeout != default)
            {
                Connection.Timeout = timeout;
            }

            #endregion
            #region Retry Count

            if(!configuration.ContainsKey(ConfigurationConstants.RetryCountKey))
            {
                return;
            }

            int retryCount = configuration.Get<int>(ConfigurationConstants.RetryCountKey, ConfigurationConstants.RetryIsRequired);

            if(retryCount == default)
            {
                return;
            }

            ApiHandlerOptionsRules.ValidateRetryAttempts(retryCount);

            Connection.RetryAttempts = retryCount;

            #endregion
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder{TApiHandler}.AddLoggerFactory"/>
        public void AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            ExceptionHelper.EnsureParameterIsNotNull(loggerFactory, nameof(loggerFactory));

            Dependencies.AddDependency(loggerFactory.CreateLogger<TApiHandler>);
        }

        /// <summary>
        /// Builds the <see cref="IApiHandlerOptions"/> for the specified <see cref="ApiHandler"/>.
        /// </summary>
        public IApiHandlerOptions<TApiHandler> BuildOptions(IServiceCollection services)
        {
            Dependencies.AddDependency<IConnectionSettings>(() => Connection);
            Dependencies.TryAddDependency<IRouteFactory>(() => new RouteFactory(Connection));
            Dependencies.AddDependency(services.BuildServiceProvider);

            ApiHandlerOptions<TApiHandler> options = new ApiHandlerOptions<TApiHandler>(Dependencies.BuildProvider(), Connection);

            return options;
        }
    }
}
