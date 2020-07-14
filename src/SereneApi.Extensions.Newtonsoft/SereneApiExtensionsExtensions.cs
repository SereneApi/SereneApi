using DeltaWare.Dependencies;
using Newtonsoft.Json;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Serializers;
using SereneApi.Extensions.Newtonsoft.Serializers;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.Newtonsoft
{
    public static class SereneApiExtensionsExtensions
    {
        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        public static ISereneApiExtensions AddNewtonsoft(this ISereneApiExtensions extensions)
        {
            extensions.AddDependency(d => d.AddScoped<ISerializer>(() => new NewtonsoftSerializer()));

            return extensions;
        }

        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="settings">The settings to be used by <see cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static ISereneApiExtensions AddNewtonsoft(this ISereneApiExtensions extensions, [NotNull] JsonSerializerSettings settings)
        {
            if(settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            extensions.AddDependency(d => d.AddScoped<ISerializer>(() => new NewtonsoftSerializer(settings)));

            return extensions;
        }

        /// <summary>
        /// Adds <seealso cref="NewtonsoftSerializer"/> as the default <see cref="ISerializer"/>.
        /// </summary>
        /// <param name="factory">Builds to the settings to be used by <seealso cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static ISereneApiExtensions AddNewtonsoft(this ISereneApiExtensions extensions, [NotNull] Action<JsonSerializerSettings> factory)
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
