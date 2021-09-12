using SereneApi.Core.Content;
using SereneApi.Core.Content.Types;
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
        /// The default options used for deserialization by <seealso cref="JsonSerializer"/>.
        /// </summary>
        public static JsonSerializerOptions DefaultDeserializerOptions { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// The default options used for serialization by <seealso cref="JsonSerializer"/>.
        /// </summary>
        public static JsonSerializerOptions DefaultSerializerOptions { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializer"/> using the default options.
        /// </summary>
        public JsonSerializer()
        {
            _deserializerOptions = DefaultDeserializerOptions;
            _serializerOptions = DefaultSerializerOptions;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializer"/> using the specified options.
        /// </summary>
        /// <param name="options">the options to be used for serialization and deserialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public JsonSerializer(JsonSerializerOptions options)
        {
            _deserializerOptions = options ?? throw new ArgumentNullException(nameof(options));
            _serializerOptions = options;
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializer"/> using the specified options.
        /// </summary>
        /// <param name="deserializerOptions">the options to be used for deserialization.</param>
        /// <param name="serializerOptions">the options to be used for serialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public JsonSerializer(JsonSerializerOptions deserializerOptions, JsonSerializerOptions serializerOptions)
        {
            _deserializerOptions = deserializerOptions ?? throw new ArgumentNullException(nameof(deserializerOptions));
            _serializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));
        }

        public TObject Deserialize<TObject>(IResponseContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            string contentString = content.GetContentString();

            return System.Text.Json.JsonSerializer.Deserialize<TObject>(contentString, _deserializerOptions);
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