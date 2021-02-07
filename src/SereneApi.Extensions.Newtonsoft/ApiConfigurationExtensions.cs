using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Serialization;
using SereneApi.Extensions.Newtonsoft.Serializers;
using DeltaWare.Dependencies.Abstractions;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.Newtonsoft
{
    public static class ApiConfigurationExtensions
    {
        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        public static IApiConfigurationExtensions AddNewtonsoft(this IApiConfigurationExtensions extensions)
        {
            extensions.AddDependencies(d => d.AddScoped<ISerializer>(() => new NewtonsoftSerializer()));

            return extensions;
        }

        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="settings">The settings to be used by <see cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static IApiConfigurationExtensions AddNewtonsoft(this IApiConfigurationExtensions extensions, [NotNull] JsonSerializerSettings settings)
        {
            if(settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            extensions.AddDependencies(d => d.AddScoped<ISerializer>(() => new NewtonsoftSerializer(settings)));

            return extensions;
        }

        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="factory">Builds to the settings to be used by <seealso cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static IApiConfigurationExtensions AddNewtonsoft(this IApiConfigurationExtensions extensions, [NotNull] Action<JsonSerializerSettings> factory)
        {
            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();

            factory.Invoke(settings);

            return AddNewtonsoft(extensions, settings);
        }
    }
}
