using Microsoft.Extensions.Configuration;
using SereneApi.Core.Connection;
using System;
using System.Collections.Generic;

namespace SereneApi.Extensions.DependencyInjection
{
    public static class ConfigurationExtensions
    {
        public const string ApiConfigKey = "ApiConfig";

        public const bool ResourceIsRequired = false;
        public const string ResourceKey = "Resource";
        public const bool ResourcePathIsRequired = false;
        public const string ResourcePathKey = "ResourcePath";
        public const string RetryCountKey = "Retries";
        public const bool RetryIsRequired = false;
        public const bool SourceIsRequired = true;
        public const string SourceKey = "Source";
        public const bool TimeoutIsRequired = false;
        public const string TimeoutKey = "Timeout";

        /// <summary>
        /// Returns the <see cref="IConnectionSettings"/> from within ApiConfig that matches the
        /// specified API Key.
        /// </summary>
        /// <param name="configuration">The ROOT <see cref="IConfiguration"/> to be searched.</param>
        /// <param name="apiConfigurationKey">
        /// The <see cref="IConfiguration"/> name containing the API Configuration.
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

            if (!configuration.GetSection(ApiConfigKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ApiConfigKey} inside the Configuration");
            }

            IConfiguration configurationSection = configuration.GetSection(ApiConfigKey);

            if (!configurationSection.GetSection(apiConfigurationKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ApiConfigKey}:{apiConfigurationKey} inside the Configuration");
            }

            IConfiguration apiConfiguration = configurationSection.GetSection(apiConfigurationKey);

            string source = apiConfiguration.Get<string>(SourceKey, SourceIsRequired);
            string resource = apiConfiguration.Get<string>(ResourceKey, ResourceIsRequired);
            string resourcePath = apiConfiguration.Get<string>(ResourcePathKey, ResourcePathIsRequired);

            ConnectionSettings connection = new ConnectionSettings(source, resource, resourcePath);

            #region Timeout

            if (apiConfiguration.ContainsKey(TimeoutKey))
            {
                int timeout = apiConfiguration.Get<int>(TimeoutKey, TimeoutIsRequired);

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

            if (!apiConfiguration.ContainsKey(RetryCountKey))
            {
                int retryCount = apiConfiguration.Get<int>(RetryCountKey, RetryIsRequired);

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
        /// The <see cref="IConfiguration"/> name containing the API Configuration.
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

            if (!configuration.GetSection(ApiConfigKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ApiConfigKey} inside the Configuration");
            }

            IConfiguration configurationSection = configuration.GetSection(ApiConfigKey);

            if (!configurationSection.GetSection(apiConfigurationKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ApiConfigKey}:{apiConfigurationKey} inside the Configuration");
            }

            IConfiguration apiConfiguration = configurationSection.GetSection(apiConfigurationKey);

            if (!configurationSection.GetSection(apiSourceKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ApiConfigKey}:{apiSourceKey} inside the Configuration");
            }

            string source = configurationSection.Get<string>(apiSourceKey, SourceIsRequired);
            string resource = apiConfiguration.Get<string>(ResourceKey, ResourceIsRequired);
            string resourcePath = apiConfiguration.Get<string>(ResourcePathKey, ResourcePathIsRequired);

            ConnectionSettings connection = new ConnectionSettings(source, resource, resourcePath);

            #region Timeout

            if (apiConfiguration.ContainsKey(TimeoutKey))
            {
                int timeout = apiConfiguration.Get<int>(TimeoutKey, TimeoutIsRequired);

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

            if (!apiConfiguration.ContainsKey(RetryCountKey))
            {
                int retryCount = apiConfiguration.Get<int>(RetryCountKey, RetryIsRequired);

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
                throw new KeyNotFoundException($"Could not find {key} inside the Configuration");
            }

            return default;
        }
    }
}