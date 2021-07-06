using SereneApi.Abstractions.Content;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Serialization
{
    public static class SerializerExtensions
    {
        /// <inheritdoc cref="ISerializer.DeserializeAsync{T}"/>
        public static Task<T> DeserializeAsync<T>(this ISerializer serializer, HttpContent content)
        {
            return serializer.DeserializeAsync<T>(new HttpContentResponse(content));
        }
    }
}
