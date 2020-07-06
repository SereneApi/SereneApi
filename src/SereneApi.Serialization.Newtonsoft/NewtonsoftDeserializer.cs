using Newtonsoft.Json;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Serializers;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Serialization.Newtonsoft
{
    public class NewtonsoftDeserializer: ISerializer
    {
        private readonly JsonSerializerSettings _deserializerOptions;

        private readonly JsonSerializerSettings _serializerOptions;

        public NewtonsoftDeserializer()
        {
            _deserializerOptions = DefaultDeserializerOptions;
            _serializerOptions = DefaultSerializerOptions;
        }

        public NewtonsoftDeserializer(JsonSerializerSettings sharedOptions)
        {
            _deserializerOptions = sharedOptions;
            _serializerOptions = sharedOptions;
        }

        public NewtonsoftDeserializer(JsonSerializerSettings deserializerOptions, JsonSerializerSettings serializerOptions)
        {
            _deserializerOptions = deserializerOptions;
            _serializerOptions = serializerOptions;
        }

        public TObject Deserialize<TObject>(HttpContent content)
        {
            string contentString = content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<TObject>(contentString, _deserializerOptions);
        }

        public async Task<TObject> DeserializeAsync<TObject>(HttpContent content)
        {
            string contentString = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TObject>(contentString, _deserializerOptions);
        }

        public IApiRequestContent Serialize<TObject>(TObject value)
        {
            string jsonContent = JsonConvert.SerializeObject(value, _serializerOptions);

            return new JsonContent(jsonContent);
        }

        public Task<IApiRequestContent> SerializeAsync<TObject>(TObject value)
        {
            return Task.Factory.StartNew(() =>
            {
                string jsonContent = JsonConvert.SerializeObject(value, _serializerOptions);

                return (IApiRequestContent)new JsonContent(jsonContent);
            });
        }

        public static JsonSerializerSettings DefaultDeserializerOptions = new JsonSerializerSettings
        {
        };

        public static JsonSerializerSettings DefaultSerializerOptions = new JsonSerializerSettings
        {
        };
    }
}
