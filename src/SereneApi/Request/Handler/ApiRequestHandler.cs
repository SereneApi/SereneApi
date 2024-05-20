using SereneApi.Http;
using SereneApi.Response;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Request.Handler
{
    internal sealed class ApiRequestHandler : IApiRequestHandler
    {
        private readonly IHttpClientProvider _httpClientProvider;

        private readonly IApiResourceConnection _resourceConnection;

        public Task<IApiResponse> ExecuteAsync(IApiRequest apiRequest, CancellationToken cancellationToken = default)
        {
            HttpClient client = _httpClientProvider.GetHttpClient();

            HttpRequestMessage httpRequest = new HttpRequestMessage(apiRequest.Method, apiRequest.Route)

            // Step 1 - Get Http Client
            // Step 2 - Build Http Request with Api Request
            // Step 3 - Append Request via Config
            // Step 4 - Execute Send Http Request
        }
    }
}
