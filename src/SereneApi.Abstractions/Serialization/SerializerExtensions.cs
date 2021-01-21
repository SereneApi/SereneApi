using SereneApi.Abstractions.Response.Content;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Serialization
{
    public static class SerializerExtensions
    {
        public static TResponse Deserialize<TResponse>([NotNull] this ISerializer serializer, [NotNull] HttpContent content)
        {
            if(serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if(content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return serializer.Deserialize<TResponse>(new HttpContentResponse(content));
        }

        public static Task<TResponse> DeserializeAsync<TResponse>([NotNull] this ISerializer serializer, [NotNull] HttpContent content)
        {
            if(serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if(content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return serializer.DeserializeAsync<TResponse>(new HttpContentResponse(content));
        }
    }
}
