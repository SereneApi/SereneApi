using System;
using DeltaWare.Dependencies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Helpers;
using SereneApi.Abstractions.Options;
using SereneApi.Extensions.DependencyInjection.Helpers;

namespace SereneApi.Extensions.DependencyInjection.Options
{
    /// <inheritdoc cref="IApiOptionsConfigurator{TApiDefinition}"/>
    internal class ApiApiOptionsBuilder<TApiDefinition>: ApiOptionsBuilder, IApiOptionsBuilder<TApiDefinition>, IApiOptionsConfigurator<TApiDefinition> where TApiDefinition : class
    {
        public Type HandlerType => typeof(TApiDefinition);

        /// <inheritdoc cref="IApiOptionsConfigurator{TApiDefinition}.UseConfiguration"/>
        public void UseConfiguration(IConfiguration configuration)
        {
            if(Dependencies.HasDependency<IConnectionSettings>())
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            string source = configuration.Get<string>(ConfigurationConstants.SourceKey, ConfigurationConstants.SourceIsRequired);
            string resource = configuration.Get<string>(ConfigurationConstants.ResourceKey, ConfigurationConstants.ResourceIsRequired);
            string resourcePath = configuration.Get<string>(ConfigurationConstants.ResourcePathKey, ConfigurationConstants.ResourcePathIsRequired);

            using IDependencyProvider provider = Dependencies.BuildProvider();

            ISereneApiConfiguration sereneApiConfiguration = provider.GetDependency<ISereneApiConfiguration>();

            if(string.IsNullOrWhiteSpace(resourcePath))
            {
                if(resourcePath != string.Empty)
                {
                    resourcePath = sereneApiConfiguration.ResourcePath;
                }
            }

            ConnectionSettings = new ConnectionSettings(source, resource, resourcePath)
            {
                Timeout = sereneApiConfiguration.Timeout,
                RetryAttempts = sereneApiConfiguration.RetryCount
            };

            #region Timeout

            if(configuration.ContainsKey(ConfigurationConstants.TimeoutKey))
            {
                int timeout = configuration.Get<int>(ConfigurationConstants.TimeoutKey, ConfigurationConstants.TimeoutIsRequired);

                if(timeout < 0)
                {
                    throw new ArgumentException("The Timeout value must be greater than 0");
                }

                if(timeout != default)
                {
                    ConnectionSettings.Timeout = timeout;
                }
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

            Rules.ValidateRetryAttempts(retryCount);

            ConnectionSettings.RetryAttempts = retryCount;

            #endregion
        }

        /// <inheritdoc cref="IApiOptionsConfigurator{TApiDefinition}.AddLoggerFactory"/>
        public void AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            ExceptionHelper.EnsureParameterIsNotNull(loggerFactory, nameof(loggerFactory));

            Dependencies.AddScoped(loggerFactory.CreateLogger<TApiDefinition>);
        }

        /// <summary>
        /// Builds the <see cref="IApiOptions"/> for the specified <see cref="ApiHandler"/>.
        /// </summary>
        public new IApiOptions<TApiDefinition> BuildOptions()
        {
            Dependencies.AddScoped<IConnectionSettings>(() => ConnectionSettings);

            IApiOptions<TApiDefinition> apiOptions = new ApiOptions<TApiDefinition>(Dependencies.BuildProvider(), ConnectionSettings);

            return apiOptions;
        }
    }
}
