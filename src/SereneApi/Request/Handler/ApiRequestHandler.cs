using System;
using Microsoft.Extensions.Logging;
using SereneApi.Http;
using SereneApi.Response;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Request.Handler
{
    internal sealed class ApiRequestHandler : IApiRequestHandler
    {
        private readonly IHttpClientProvider _httpClientProvider;
        
        private readonly ILogger? _logger;

        public async Task<IApiResponse> ExecuteAsync(IApiRequest apiRequest, CancellationToken cancellationToken = default)
        {
            HttpClient client = _httpClientProvider.GetHttpClient();

            HttpRequestMessage httpRequest = BuildRequestMessage(apiRequest);

            Stopwatch stopwatch = Stopwatch.StartNew();

            using HttpResponseMessage response = await client.SendAsync(httpRequest, cancellationToken);

            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                _logger?.LogInformation("The [{HttpMethod}] Request to {Url} was successful. Status[{StatusCode}] - Duration[{Duration}]", apiRequest.Method, apiRequest.FullRoute, response.StatusCode, stopwatch.Elapsed);
            }
            else
            {
                _logger?.LogWarning("The [{HttpMethod}] Request to {Url} was not successful. Status[{StatusCode}] - Duration[{Duration}]", apiRequest.Method, apiRequest.FullRoute, response.StatusCode, stopwatch.Elapsed);
            }
        }

        private HttpRequestMessage BuildRequestMessage(IApiRequest apiRequest)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(apiRequest.Method, apiRequest.FullRoute);

            foreach (KeyValuePair<string, string> header in apiRequest.Headers)
            {
                httpRequest.Headers.Add(header.Key, header.Value);
            }

            return httpRequest;
        }
    }
}