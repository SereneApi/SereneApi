using Newtonsoft.Json;
using SereneApi.Core.Options.Factories;
using SereneApi.Serializers.Newtonsoft.Json.Serializers;
using System;

namespace SereneApi.Serializers.Newtonsoft.Json
{
    public static class ApiOptionsFactoryExtensions
    {
        /// <summary>
        /// Uses <seealso cref="NewtonsoftSerializer"/> for serialization.
        /// </summary>
        public static void UseNewtonsoftSerializer(this IApiOptionsFactory factory)
        {
            factory.UseSerializer(new NewtonsoftSerializer());
        }

        /// <summary>
        /// Uses <seealso cref="NewtonsoftSerializer"/> for serialization.
        /// </summary>
        /// <param name="settings">The settings to be used by <see cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static void UseNewtonsoftSerializer(this IApiOptionsFactory factory, JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            factory.UseSerializer(new NewtonsoftSerializer(settings));
        }

        /// <summary>
        /// Uses <seealso cref="NewtonsoftSerializer"/> for serialization.
        /// </summary>
        /// <param name="builder">Builds to the settings to be used by <see cref="NewtonsoftSerializer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static void UseNewtonsoftSerializer(this IApiOptionsFactory factory, Action<JsonSerializerSettings> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();

            builder.Invoke(settings);

            UseNewtonsoftSerializer(factory, settings);
        }
    }
}