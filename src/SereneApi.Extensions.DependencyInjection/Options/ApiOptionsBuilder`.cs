using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Configuration;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Helpers;
using SereneApi.Abstractions.Options;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.DependencyInjection.Options
{
    /// <inheritdoc cref="IApiOptionsConfigurator{TApi}"/>
    internal class ApiOptionsBuilder<TApi>: ApiOptionsBuilder, IApiOptionsBuilder<TApi>, IApiOptionsConfigurator<TApi> where TApi : class
    {
        /// <inheritdoc cref="IApiOptionsConfigurator{TApi}.AddConfiguration(Microsoft.Extensions.Configuration.IConfiguration)"/>
        public void AddConfiguration([NotNull] IConfiguration configuration)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if(Dependencies.HasDependency<IConnectionConfiguration>())
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            string source = configuration.Get<string>(ConfigurationExtensions.SourceKey, ConfigurationExtensions.SourceIsRequired);
            string resource = configuration.Get<string>(ConfigurationExtensions.ResourceKey, ConfigurationExtensions.ResourceIsRequired);
            string resourcePath = configuration.Get<string>(ConfigurationExtensions.ResourcePathKey, ConfigurationExtensions.ResourcePathIsRequired);

            using IDependencyProvider provider = Dependencies.BuildProvider();

            IApiConfiguration apiConfiguration = provider.GetDependency<IApiConfiguration>();

            if(string.IsNullOrWhiteSpace(resourcePath))
            {
                if(resourcePath != string.Empty)
                {
                    resourcePath = apiConfiguration.ResourcePath;
                }
            }

            ConnectionConfiguration = new ConnectionConfiguration(source, resource, resourcePath)
            {
                Timeout = apiConfiguration.Timeout,
                RetryAttempts = apiConfiguration.RetryCount
            };

            #region Timeout

            if(configuration.ContainsKey(ConfigurationExtensions.TimeoutKey))
            {
                int timeout = configuration.Get<int>(ConfigurationExtensions.TimeoutKey, ConfigurationExtensions.TimeoutIsRequired);

                if(timeout < 0)
                {
                    throw new ArgumentException("The Timeout value must be greater than 0");
                }

                if(timeout != default)
                {
                    ConnectionConfiguration.Timeout = timeout;
                }
            }

            #endregion
            #region Retry Count

            if(!configuration.ContainsKey(ConfigurationExtensions.RetryCountKey))
            {
                return;
            }

            int retryCount = configuration.Get<int>(ConfigurationExtensions.RetryCountKey, ConfigurationExtensions.RetryIsRequired);

            if(retryCount == default)
            {
                return;
            }

            Rules.ValidateRetryAttempts(retryCount);

            ConnectionConfiguration.RetryAttempts = retryCount;

            #endregion
        }

        /// <inheritdoc cref="IApiOptionsBuilder{TApi}.BuildOptions"/>
        public new IApiOptions<TApi> BuildOptions()
        {
            Dependencies.AddScoped<IConnectionConfiguration>(() => ConnectionConfiguration);

            IApiOptions<TApi> apiOptions = new ApiOptions<TApi>(Dependencies.BuildProvider(), ConnectionConfiguration);

            return apiOptions;
        }
    }
}
