using Newtonsoft.Json;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<TResponse> ReadAsJsonAsync<TResponse>(this HttpResponseMessage response)
        {
            string contentString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(contentString);
        }
    }
}
