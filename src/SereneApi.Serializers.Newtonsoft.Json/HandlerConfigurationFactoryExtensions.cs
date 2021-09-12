using DeltaWare.Dependencies.Abstractions;
using Newtonsoft.Json;
using SereneApi.Core.Configuration;
using SereneApi.Core.Serialization;
using SereneApi.Serializers.Newtonsoft.Json.Serializers;
using System;

namespace SereneApi.Serializers.Newtonsoft.Json
{
    public static class HandlerConfigurationFactoryExtensions
    {
        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        public static IHandlerConfigurationFactory AddNewtonsoft(this IHandlerConfigurationFactory factory)
        {
            factory.Dependencies.AddScoped<ISerializer>(() => new NewtonsoftSerializer());

            return factory;
        }

        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="settings">The settings to be used by <see cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static IHandlerConfigurationFactory AddNewtonsoft(this IHandlerConfigurationFactory factory, JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            factory.Dependencies.AddScoped<ISerializer>(() => new NewtonsoftSerializer(settings));

            return factory;
        }

        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="factory">Builds to the settings to be used by <seealso cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static IHandlerConfigurationFactory AddNewtonsoft(this IHandlerConfigurationFactory factory, Action<JsonSerializerSettings> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();

            builder.Invoke(settings);

            return AddNewtonsoft(factory, settings);
        }
    }
}