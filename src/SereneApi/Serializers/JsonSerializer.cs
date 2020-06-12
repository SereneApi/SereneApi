using SereneApi.Interfaces;
using SereneApi.Types;
using SereneApi.Types.Content;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SereneApi.Serializers
{
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerOptions _deserializerOptions = DefaultDeserializerOptions;

        private readonly JsonSerializerOptions _serializerOptions = DefaultSerializerOptions;

        public JsonSerializer()
        {
        }

        public JsonSerializer(JsonSerializerOptions sharedOptions)
        {
            _deserializerOptions = sharedOptions;
            _serializerOptions = sharedOptions;
        }

        public JsonSerializer(JsonSerializerOptions deserializerOptions, JsonSerializerOptions serializerOptions)
        {
            _deserializerOptions = deserializerOptions;
            _serializerOptions = serializerOptions;
        }

        public TObject Deserialize<TObject>(HttpContent content)
        {
            string contentString = content.ReadAsStringAsync().Result;

            return System.Text.Json.JsonSerializer.Deserialize<TObject>(contentString, _deserializerOptions);
        }

        public async Task<TObject> DeserializeAsync<TObject>(HttpContent content)
        {
            await using Stream contentStream = await content.ReadAsStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<TObject>(contentStream, _deserializerOptions);
        }

        public IApiRequestContent Serialize<TObject>(TObject value)
        {
            string jsonContent = System.Text.Json.JsonSerializer.Serialize(value, _serializerOptions);

            return new JsonContent(jsonContent, Encoding.UTF8, MediaType.ApplicationJson);
        }

        public Task<IApiRequestContent> SerializeAsync<TObject>(TObject value)
        {
            return Task.Factory.StartNew(() =>
            {
                string jsonContent = System.Text.Json.JsonSerializer.Serialize(value, _serializerOptions);

                return (IApiRequestContent)new JsonContent(jsonContent, Encoding.UTF8, MediaType.ApplicationJson);
            });
        }

        public static JsonSerializerOptions DefaultDeserializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static JsonSerializerOptions DefaultSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        internal static ISerializer Default => new JsonSerializer();
    }
}
