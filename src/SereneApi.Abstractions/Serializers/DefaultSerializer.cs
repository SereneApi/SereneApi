using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Response.Content;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Serializers
{
    /// <summary>
    /// The default <seealso cref="ISerializer"/>.
    /// </summary>
    /// <remarks>Uses System.Text.Json for Serialization.</remarks>
    public class DefaultSerializer: ISerializer
    {
        private readonly JsonSerializerOptions _deserializerOptions;

        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultSerializer"/> using the default options.
        /// </summary>
        public DefaultSerializer()
        {
            _deserializerOptions = DefaultDeserializerOptions;
            _serializerOptions = DefaultSerializerOptions;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultSerializer"/> using the specified options.
        /// </summary>
        /// <param name="options">the options to be used for serialization and deserialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public DefaultSerializer(JsonSerializerOptions options)
        {
            _deserializerOptions = options ?? throw new ArgumentNullException(nameof(options));
            _serializerOptions = options;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultSerializer"/> using the specified options.
        /// </summary>
        /// <param name="deserializerOptions">the options to be used for deserialization.</param>
        /// <param name="serializerOptions">the options to be used for serialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public DefaultSerializer(JsonSerializerOptions deserializerOptions, JsonSerializerOptions serializerOptions)
        {
            _deserializerOptions = deserializerOptions ?? throw new ArgumentNullException(nameof(deserializerOptions));
            _serializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));
        }

        /// <inheritdoc>
        ///     <cref>ISerializer.Deserialize</cref>
        /// </inheritdoc>
        public TObject Deserialize<TObject>([NotNull] IApiResponseContent content)
        {
            if(content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            string contentString = content.GetContentString();

            return System.Text.Json.JsonSerializer.Deserialize<TObject>(contentString, _deserializerOptions);
        }

        /// <inheritdoc>
        ///     <cref>ISerializer.DeserializeAsync</cref>
        /// </inheritdoc>
        public async Task<TObject> DeserializeAsync<TObject>([NotNull] IApiResponseContent content)
        {
            if(content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            await using Stream contentStream = await content.GetContentStreamAsync();

            return await JsonSerializer.DeserializeAsync<TObject>(contentStream, _deserializerOptions);
        }

        /// <inheritdoc>
        ///     <cref>ISerializer.Serialize</cref>
        /// </inheritdoc>
        public IApiRequestContent Serialize<TObject>([NotNull] TObject value)
        {
            if(value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string jsonContent = JsonSerializer.Serialize(value, _serializerOptions);

            return new JsonContent(jsonContent);
        }

        /// <inheritdoc>
        ///     <cref>ISerializer.SerializeAsync</cref>
        /// </inheritdoc>
        public Task<IApiRequestContent> SerializeAsync<TObject>([NotNull] TObject value)
        {
            if(value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return Task.Factory.StartNew(() =>
            {
                string jsonContent = JsonSerializer.Serialize(value, _serializerOptions);

                return (IApiRequestContent)new JsonContent(jsonContent);
            });
        }

        /// <summary>
        /// The default options used for deserialization by <seealso cref="DefaultSerializer"/>.
        /// </summary>
        public static JsonSerializerOptions DefaultDeserializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// The default options used for serialization by <seealso cref="DefaultSerializer"/>.
        /// </summary>
        public static JsonSerializerOptions DefaultSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
