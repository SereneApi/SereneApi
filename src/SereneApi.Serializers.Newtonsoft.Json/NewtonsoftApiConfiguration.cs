﻿using Newtonsoft.Json;
using SereneApi.Core.Configuration;
using SereneApi.Core.Serialization;
using SereneApi.Serializers.Newtonsoft.Json.Serializers;
using System;

namespace SereneApi.Serializers.Newtonsoft.Json
{
    public static class NewtonsoftApiConfiguration
    {
        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        public static void UseNewtonsoftSerializer(this IApiConfiguration configuration)
        {
            configuration.Dependencies.Register(() => new NewtonsoftSerializer())
                .DefineAs<ISerializer>()
                .AsSingleton();
        }

        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="settings">The settings to be used by <see cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static void UseNewtonsoftSerializer(this IApiConfiguration configuration, JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            configuration.Dependencies.Register(() => new NewtonsoftSerializer(settings))
                .DefineAs<ISerializer>()
                .AsSingleton();
        }

        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="configuration">Builds to the settings to be used by <seealso cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static void UseNewtonsoftSerializer(this IApiConfiguration configuration, Action<JsonSerializerSettings> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();

            builder.Invoke(settings);

            configuration.UseNewtonsoftSerializer(settings);
        }
    }
}