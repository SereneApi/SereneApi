using Newtonsoft.Json;
using SereneApi.Abstractions.Options;
using SereneApi.Extensions.Newtonsoft.Serializers;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.Newtonsoft
{
    public static class ApiOptionsConfiguratorExtensions
    {
        /// <summary>
        /// Uses <seealso cref="NewtonsoftSerializer"/> for serialization.
        /// </summary>
        public static void UseNewtonsoftSerializer(this IApiOptionsBuilder builder)
        {
            builder.UseSerializer(new NewtonsoftSerializer());
        }

        /// <summary>
        /// Uses <seealso cref="NewtonsoftSerializer"/> for serialization.
        /// </summary>
        /// <param name="settings">The settings to be used by <see cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static void UseNewtonsoftSerializer(this IApiOptionsBuilder builder, [NotNull] JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            builder.UseSerializer(new NewtonsoftSerializer(settings));
        }

        /// <summary>
        /// Uses <seealso cref="NewtonsoftSerializer"/> for serialization.
        /// </summary>
        /// <param name="factory">Builds to the settings to be used by <see cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static void UseNewtonsoftSerializer(this IApiOptionsBuilder builder, [NotNull] Action<JsonSerializerSettings> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();

            factory.Invoke(settings);

            UseNewtonsoftSerializer(builder, settings);
        }
    }
}
