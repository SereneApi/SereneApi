using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.DependencyInjection
{
    public static class ConfigurationExtensions
    {
        public const string ApiConfigKey = "ApiConfig";

        public const string SourceKey = "Source";
        public const string ResourceKey = "Resource";
        public const string ResourcePathKey = "ResourcePath";
        public const string TimeoutKey = "Timeout";
        public const string RetryCountKey = "Retries";

        public const bool SourceIsRequired = true;
        public const bool ResourceIsRequired = false;
        public const bool ResourcePathIsRequired = false;
        public const bool TimeoutIsRequired = false;
        public const bool RetryIsRequired = false;

        /// <summary>
        /// Returns the <see cref="IConfiguration"/> from within ApiConfig that matches the specified API Key.
        /// </summary>
        /// <param name="configuration">The ROOT <see cref="IConfiguration"/> to be searched.</param>
        /// <param name="apiKey">The <see cref="IConfiguration"/> name containing the API Configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the specified api key was not found.</exception>
        public static IConfiguration GetApiConfig([NotNull] this IConfiguration configuration, [NotNull] string apiKey)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if(string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            if(!configuration.GetSection(ApiConfigKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ApiConfigKey} inside the Configuration");
            }

            IConfiguration apiConfiguration = configuration.GetSection(ApiConfigKey);

            if(!apiConfiguration.GetSection(apiKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ApiConfigKey}:{apiKey} inside the Configuration");
            }

            return apiConfiguration.GetSection(apiKey);
        }

        /// <summary>
        /// Gets a value from <see cref="IConfiguration"/>.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type"/> to be returned.</typeparam>
        /// <param name="configuration">The <see cref="IConfiguration"/> the value will be retrieved from.</param>
        /// <param name="key">The key of the value.</param>
        /// <param name="required">Specifies if the value must be present.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the specified value cannot be found and requited is set to true.</exception>
        internal static TValue Get<TValue>([NotNull] this IConfiguration configuration, [NotNull] string key, bool required = true)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if(string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if(configuration.GetSection(key).Exists())
            {
                return configuration.GetSection(key).Get<TValue>();
            }

            if(required)
            {
                throw new KeyNotFoundException($"Could not find {key} inside the Configuration");
            }

            return default;
        }

        /// <summary>
        /// Returns a boolean value specifying if the <see cref="IConfiguration"/> contains the specified key.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> the value will be retrieved from.</param>
        /// <param name="key">The key of the value.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        internal static bool ContainsKey([NotNull] this IConfiguration configuration, [NotNull] string key)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if(string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return configuration.GetSection(key).Exists();
        }
    }
}
