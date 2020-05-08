using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

// Do not change the namespace
namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
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
            return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        }
    }
}
