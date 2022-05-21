using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Content.Types;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SereneApi.Core.Serialization
{
    /// <summary>
    /// The default <seealso cref="ISerializer"/>.
    /// </summary>
    /// <remarks>Uses System.Text.Json for Serialization.</remarks>
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerOptions _deserializerOptions;

        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// The default settings used for deserialization by <seealso cref="JsonSerializer"/>.
        /// </summary>
        public static JsonSerializerOptions DefaultDeserializerOptions { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// The default settings used for serialization by <seealso cref="JsonSerializer"/>.
        /// </summary>
        public static JsonSerializerOptions DefaultSerializerOptions { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializer"/> using the default settings.
        /// </summary>
        public JsonSerializer()
        {
            _deserializerOptions = DefaultDeserializerOptions;
            _serializerOptions = DefaultSerializerOptions;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializer"/> using the specified settings.
        /// </summary>
        /// <param name="options">the settings to be used for serialization and deserialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public JsonSerializer(JsonSerializerOptions options)
        {
            _deserializerOptions = options ?? throw new ArgumentNullException(nameof(options));
            _serializerOptions = options;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializer"/> using the specified settings.
        /// </summary>
        /// <param name="deserializerOptions">the settings to be used for deserialization.</param>
        /// <param name="serializerOptions">the settings to be used for serialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public JsonSerializer(JsonSerializerOptions deserializerOptions, JsonSerializerOptions serializerOptions)
        {
            _deserializerOptions = deserializerOptions ?? throw new ArgumentNullException(nameof(deserializerOptions));
            _serializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));
        }

        /// <inheritdoc><cref>ISerializer.DeserializeAsync</cref></inheritdoc>
        public async Task<TObject> DeserializeAsync<TObject>(IResponseContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            await using Stream contentStream = await content.GetContentStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<TObject>(contentStream, _deserializerOptions);
        }

        public async Task<object> DeserializeAsync(Type type, IResponseContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            await using Stream contentStream = await content.GetContentStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync(contentStream, type, _deserializerOptions);
        }

        /// <inheritdoc><cref>ISerializer.Serialize</cref></inheritdoc>
        public IRequestContent Serialize(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string jsonContent = System.Text.Json.JsonSerializer.Serialize(value, _serializerOptions);

            return new JsonContent(jsonContent);
        }
    }
}