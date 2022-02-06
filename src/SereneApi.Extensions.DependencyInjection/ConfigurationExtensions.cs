using SereneApi.Core.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SereneApi.Extensions.DependencyInjection
{
    public static class ConfigurationExtensions
    {
        private const bool ResourceIsRequired = false;
        private const bool ResourcePathIsRequired = false;
        private const bool RetryIsRequired = false;
        private const bool SourceIsRequired = true;
        private const bool TimeoutIsRequired = false;

        /// <summary>
        /// Returns the <see cref="IConnectionSettings"/> from within ApiConfig that matches the
        /// specified API Key.
        /// </summary>
        /// <param name="configuration">The ROOT <see cref="IConfiguration"/> to be searched.</param>
        /// <param name="apiConfigurationKey">
        /// The <see cref="IConfiguration"/> name containing the API HandlerConfiguration.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the specified api key was not found.
        /// </exception>
        public static IConnectionSettings GetApiConfig(this IConfiguration configuration, string apiConfigurationKey)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (string.IsNullOrWhiteSpace(apiConfigurationKey))
            {
                throw new ArgumentNullException(nameof(apiConfigurationKey));
            }

            if (!configuration.GetSection(ConfigurationKeys.ApiConfig).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ConfigurationKeys.ApiConfig} inside the HandlerConfiguration");
            }

            IConfiguration configurationSection = configuration.GetSection(ConfigurationKeys.ApiConfig);

            if (!configurationSection.GetSection(apiConfigurationKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ConfigurationKeys.ApiConfig}:{apiConfigurationKey} inside the HandlerConfiguration");
            }

            IConfiguration apiConfiguration = configurationSection.GetSection(apiConfigurationKey);

            string source = apiConfiguration.Get<string>(ConfigurationKeys.Source, SourceIsRequired);
            string resource = apiConfiguration.Get<string>(ConfigurationKeys.Resource, ResourceIsRequired);
            string resourcePath = apiConfiguration.Get<string>(ConfigurationKeys.ResourcePath, ResourcePathIsRequired);

            ConnectionSettings connection = new ConnectionSettings(source, resource, resourcePath);

            #region Timeout

            if (apiConfiguration.ContainsKey(ConfigurationKeys.Timeout))
            {
                int timeout = apiConfiguration.Get<int>(ConfigurationKeys.Timeout, TimeoutIsRequired);

                if (timeout < 0)
                {
                    throw new ArgumentException("The Timeout value must be greater than 0");
                }

                if (timeout != default)
                {
                    connection.Timeout = timeout;
                }
            }

            #endregion Timeout

            #region Retry Count

            if (!apiConfiguration.ContainsKey(ConfigurationKeys.RetryCount))
            {
                int retryCount = apiConfiguration.Get<int>(ConfigurationKeys.RetryCount, RetryIsRequired);

                if (retryCount != default)
                {
                    connection.RetryAttempts = retryCount;
                }
            }

            #endregion Retry Count

            return connection;
        }

        /// <summary>
        /// Returns the <see cref="IConnectionSettings"/> from within ApiConfig that matches the
        /// specified API Key.
        /// </summary>
        /// <param name="configuration">The ROOT <see cref="IConfiguration"/> to be searched.</param>
        /// <param name="apiConfigurationKey">
        /// The <see cref="IConfiguration"/> name containing the API HandlerConfiguration.
        /// </param>
        /// <param name="apiSourceKey">
        /// The <see cref="IConfigurationSection"/> containing the source for the API.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the specified api key was not found.
        /// </exception>
        public static IConnectionSettings GetApiConfig(this IConfiguration configuration, string apiConfigurationKey, string apiSourceKey)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (string.IsNullOrWhiteSpace(apiConfigurationKey))
            {
                throw new ArgumentNullException(nameof(apiConfigurationKey));
            }

            if (string.IsNullOrWhiteSpace(apiSourceKey))
            {
                throw new ArgumentNullException(nameof(apiSourceKey));
            }

            if (!configuration.GetSection(ConfigurationKeys.ApiConfig).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ConfigurationKeys.ApiConfig} inside the HandlerConfiguration");
            }

            IConfiguration configurationSection = configuration.GetSection(ConfigurationKeys.ApiConfig);

            if (!configurationSection.GetSection(apiConfigurationKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ConfigurationKeys.ApiConfig}:{apiConfigurationKey} inside the HandlerConfiguration");
            }

            IConfiguration apiConfiguration = configurationSection.GetSection(apiConfigurationKey);

            if (!configurationSection.GetSection(apiSourceKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ConfigurationKeys.ApiConfig}:{apiSourceKey} inside the HandlerConfiguration");
            }

            string source = configurationSection.Get<string>(apiSourceKey, SourceIsRequired);
            string resource = apiConfiguration.Get<string>(ConfigurationKeys.Resource, ResourceIsRequired);
            string resourcePath = apiConfiguration.Get<string>(ConfigurationKeys.ResourcePath, ResourcePathIsRequired);

            ConnectionSettings connection = new ConnectionSettings(source, resource, resourcePath);

            #region Timeout

            if (apiConfiguration.ContainsKey(ConfigurationKeys.Timeout))
            {
                int timeout = apiConfiguration.Get<int>(ConfigurationKeys.Timeout, TimeoutIsRequired);

                if (timeout < 0)
                {
                    throw new ArgumentException("The Timeout value must be greater than 0");
                }

                if (timeout != default)
                {
                    connection.Timeout = timeout;
                }
            }

            #endregion Timeout

            #region Retry Count

            if (!apiConfiguration.ContainsKey(ConfigurationKeys.RetryCount))
            {
                int retryCount = apiConfiguration.Get<int>(ConfigurationKeys.RetryCount, RetryIsRequired);

                if (retryCount != default)
                {
                    connection.RetryAttempts = retryCount;
                }
            }

            #endregion Retry Count

            return connection;
        }

        /// <summary>
        /// Returns a boolean value specifying if the <see cref="IConfiguration"/> contains the
        /// specified key.
        /// </summary>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> the value will be retrieved from.
        /// </param>
        /// <param name="key">The key of the value.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        internal static bool ContainsKey(this IConfiguration configuration, string key)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return configuration.GetSection(key).Exists();
        }

        /// <summary>
        /// Gets a value from <see cref="IConfiguration"/>.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type"/> to be returned.</typeparam>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> the value will be retrieved from.
        /// </param>
        /// <param name="key">The key of the value.</param>
        /// <param name="required">Specifies if the value must be present.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the specified value cannot be found and requited is set to true.
        /// </exception>
        internal static TValue Get<TValue>(this IConfiguration configuration, string key, bool required = true)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (configuration.GetSection(key).Exists())
            {
                return configuration.GetSection(key).Get<TValue>();
            }

            if (required)
            {
                throw new KeyNotFoundException($"Could not find {key} inside the HandlerConfiguration");
            }

            return default;
        }
    }
}