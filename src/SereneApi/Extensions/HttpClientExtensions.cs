using SereneApi;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// Do not change namespace
namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Creates a new <see cref="ApiHandler"/> using the <see cref="HttpClient"/> for the requests.
        /// </summary>
        public static TApiHandler CreateApiHandler<TApiHandler>(this HttpClient client) where TApiHandler : ApiHandler
        {
            return null;
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient client, Uri requestUri)
        {
            return client.PostAsync(requestUri, null);
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<TContent>(this HttpClient client, Uri requestUri, TContent content)
        {
            return client.PostAsync(requestUri, content.ToStringContent());
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync(this HttpClient client, Uri requestUri)
        {
            return client.PutAsync(requestUri, null);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<TContent>(this HttpClient client, Uri requestUri, TContent content)
        {
            return client.PutAsync(requestUri, content.ToStringContent());
        }

        public static Task<HttpResponseMessage> PatchAsJsonAsync(this HttpClient client, Uri requestUri)
        {
            return client.PatchAsync(requestUri, null);
        }

        public static Task<HttpResponseMessage> PatchAsJsonAsync<TContent>(this HttpClient client, Uri requestUri, TContent content)
        {
            return client.PatchAsync(requestUri, content.ToStringContent());
        }

        private static StringContent ToStringContent<TContent>(this TContent content)
        {
            return new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        }
    }
}
