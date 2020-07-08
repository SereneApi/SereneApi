﻿using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SereneApi.Extensions.DependencyInjection
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Returns the <see cref="IConfiguration"/> from within ApiConfig that matches the specified <see cref="apiKey"/>
        /// </summary>
        /// <param name="configuration">The ROOT <see cref="IConfiguration"/> to be searched in</param>
        /// <param name="apiKey">The <see cref="IConfiguration"/> name containing the API Configuration</param>
        /// <exception cref="KeyNotFoundException">Thrown if the specified api key was not found.</exception>
        public static IConfiguration GetApiConfig(this IConfiguration configuration, string apiKey)
        {
            if(!configuration.GetSection(ConfigurationConstants.ApiConfigKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ConfigurationConstants.ApiConfigKey} inside the Configuration");
            }

            IConfiguration apiConfiguration = configuration.GetSection(ConfigurationConstants.ApiConfigKey);

            if(!apiConfiguration.GetSection(apiKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ConfigurationConstants.ApiConfigKey}:{apiKey} inside the Configuration");
            }

            return apiConfiguration.GetSection(apiKey);
        }

        internal static TValue Get<TValue>(this IConfiguration configuration, string key, bool required = true)
        {
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

        internal static bool ContainsKey(this IConfiguration configuration, string key)
        {
            return configuration.GetSection(key).Exists();
        }
    }
}