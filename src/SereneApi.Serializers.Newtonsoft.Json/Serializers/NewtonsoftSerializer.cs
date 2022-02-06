using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Content.Types;
using SereneApi.Core.Serialization;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace SereneApi.Serializers.Newtonsoft.Json.Serializers
{
    /// <summary>
    /// An <seealso cref="ISerializer"/> that implements Newtonsoft for Serialization.
    /// </summary>
    public class NewtonsoftSerializer : ISerializer
    {
        /// <summary>
        /// The default settings used for Deserialization by <seealso cref="NewtonsoftSerializer"/>.
        /// </summary>
        public static JsonSerializerSettings DefaultDeserializerSettings { get; } = new JsonSerializerSettings();

        /// <summary>
        /// The default settings used for serialization by <seealso cref="NewtonsoftSerializer"/>.
        /// </summary>
        public static JsonSerializerSettings DefaultSerializerSettings { get; } = new JsonSerializerSettings();

        public JsonSerializerSettings DeserializerSettings { get; }

        public JsonSerializerSettings SerializerSettings { get; }

        /// <summary>
        /// Creates a new instance of <see cref="NewtonsoftSerializer"/> using the default settings.
        /// </summary>
        public NewtonsoftSerializer()
        {
            DeserializerSettings = DefaultDeserializerSettings;
            SerializerSettings = DefaultSerializerSettings;
        }

        /// <summary>
        /// Creates a new instance of <see cref="NewtonsoftSerializer"/> using the specified settings.
        /// </summary>
        /// <param name="settings">the settings to be used for serialization and deserialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public NewtonsoftSerializer(JsonSerializerSettings settings)
        {
            DeserializerSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            SerializerSettings = settings;
        }

        /// <summary>
        /// Creates a new instance of <see cref="NewtonsoftSerializer"/> using the specified settings.
        /// </summary>
        /// <param name="deserializerSettings">the settings to be used for deserialization.</param>
        /// <param name="serializerSettings">the settings to be used for serialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public NewtonsoftSerializer(JsonSerializerSettings deserializerSettings, JsonSerializerSettings serializerSettings)
        {
            DeserializerSettings = deserializerSettings ?? throw new ArgumentNullException(nameof(deserializerSettings));
            SerializerSettings = serializerSettings ?? throw new ArgumentNullException(nameof(serializerSettings));
        }

        /// <inheritdoc><cref>ISerializer.Deserialize</cref></inheritdoc>
        public TObject Deserialize<TObject>(IResponseContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            string contentString = content.GetContentString();

            return JsonConvert.DeserializeObject<TObject>(contentString, DeserializerSettings);
        }

        /// <inheritdoc><cref>ISerializer.DeserializeAsync</cref></inheritdoc>
        public async Task<TObject> DeserializeAsync<TObject>(IResponseContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            string contentString = await content.GetContentStringAsync();

            return JsonConvert.DeserializeObject<TObject>(contentString, DeserializerSettings);
        }

        /// <inheritdoc><cref>ISerializer.Serialize</cref></inheritdoc>
        public IRequestContent Serialize(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string jsonContent = JsonConvert.SerializeObject(value, SerializerSettings);

            return new JsonContent(jsonContent);
        }
    }
}