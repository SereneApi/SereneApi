using SereneApi.Abstractions.Request.Content;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Serializers
{
    public class DefaultSerializer: ISerializer
    {
        private readonly JsonSerializerOptions _deserializerOptions;

        private readonly JsonSerializerOptions _serializerOptions;

        public DefaultSerializer()
        {
            _deserializerOptions = DefaultDeserializerOptions;
            _serializerOptions = DefaultSerializerOptions;
        }

        public DefaultSerializer(JsonSerializerOptions sharedOptions)
        {
            _deserializerOptions = sharedOptions;
            _serializerOptions = sharedOptions;
        }

        public DefaultSerializer(JsonSerializerOptions deserializerOptions, JsonSerializerOptions serializerOptions)
        {
            _deserializerOptions = deserializerOptions;
            _serializerOptions = serializerOptions;
        }

        public TObject Deserialize<TObject>(HttpContent content)
        {
            string contentstring = content.ReadAsStringAsync().Result;

            return System.Text.Json.JsonSerializer.Deserialize<TObject>(contentstring, _deserializerOptions);
        }

        public async Task<TObject> DeserializeAsync<TObject>(HttpContent content)
        {
            await using Stream contentStream = await content.ReadAsStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<TObject>(contentStream, _deserializerOptions);
        }

        public IApiRequestContent Serialize<TObject>(TObject value)
        {
            string jsonContent = System.Text.Json.JsonSerializer.Serialize(value, _serializerOptions);

            return new JsonContent(jsonContent);
        }

        public Task<IApiRequestContent> SerializeAsync<TObject>(TObject value)
        {
            return Task.Factory.StartNew(() =>
            {
                string jsonContent = System.Text.Json.JsonSerializer.Serialize(value, _serializerOptions);

                return (IApiRequestContent)new JsonContent(jsonContent);
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
    }
}
