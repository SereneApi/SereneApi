using DeltaWare.Dependencies;
using Microsoft.Extensions.Configuration;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Helpers;
using SereneApi.Abstractions.Options;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.DependencyInjection.Options
{
    /// <inheritdoc cref="IApiOptionsConfigurator{TApi}"/>
    internal class ApiApiOptionsBuilder<TApi>: ApiOptionsBuilder, IApiOptionsBuilder<TApi>, IApiOptionsConfigurator<TApi> where TApi : class
    {
        /// <inheritdoc cref="IApiOptionsConfigurator{TApi}.UseConfiguration"/>
        public void UseConfiguration([NotNull] IConfiguration configuration)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if(Dependencies.HasDependency<IConnectionSettings>())
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            string source = configuration.Get<string>(ConfigurationExtensions.SourceKey, ConfigurationExtensions.SourceIsRequired);
            string resource = configuration.Get<string>(ConfigurationExtensions.ResourceKey, ConfigurationExtensions.ResourceIsRequired);
            string resourcePath = configuration.Get<string>(ConfigurationExtensions.ResourcePathKey, ConfigurationExtensions.ResourcePathIsRequired);

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

            if(configuration.ContainsKey(ConfigurationExtensions.TimeoutKey))
            {
                int timeout = configuration.Get<int>(ConfigurationExtensions.TimeoutKey, ConfigurationExtensions.TimeoutIsRequired);

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

            ConnectionSettings.RetryAttempts = retryCount;

            #endregion
        }

        /// <inheritdoc cref="IApiOptionsBuilder{TApi}.BuildOptions"/>
        public new IApiOptions<TApi> BuildOptions()
        {
            Dependencies.AddScoped<IConnectionSettings>(() => ConnectionSettings);

            IApiOptions<TApi> apiOptions = new ApiOptions<TApi>(Dependencies.BuildProvider(), ConnectionSettings);

            return apiOptions;
        }
    }
}
