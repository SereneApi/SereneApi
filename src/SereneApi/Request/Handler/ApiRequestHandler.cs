using System;
using Microsoft.Extensions.Logging;
using SereneApi.Http;
using SereneApi.Response;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SereneApi.Response.Handler;

namespace SereneApi.Request.Handler
{
    internal sealed class ApiRequestHandler : IApiRequestHandler
    {
        private readonly IHttpClientProvider _httpClientProvider;
        
        private readonly IApiResponseHandler _responseHandler;

        private readonly ILogger? _logger;

        public ApiRequestHandler(IHttpClientProvider httpClientProvider, IApiResponseHandler responseHandler, ILogger<ApiRequestHandler>? logger = null)
        {
            _httpClientProvider = httpClientProvider;
            _responseHandler = responseHandler;
            _logger = logger;
        }

        public async Task<IApiResponse> ExecuteAsync(IApiRequest apiRequest, CancellationToken cancellationToken = default)
        {
            HttpClient client = _httpClientProvider.GetHttpClient();

            using HttpRequestMessage httpRequest = BuildRequestMessage(apiRequest);

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                using HttpResponseMessage httpResponse = await client.SendAsync(httpRequest, cancellationToken);

                stopwatch.Stop();

                return await HandleResponseAsync(httpResponse, apiRequest, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }

                throw new TimeoutException();
            }
        }
        
        private async Task<IApiResponse> HandleResponseAsync(HttpResponseMessage httpResponse, IApiRequest apiRequest, CancellationToken cancellationToken)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger?.LogWarning("The [{HttpMethod}] Request to {Url} was not successful. Status[{StatusCode}]", apiRequest.Method, apiRequest.FullRoute, httpResponse.StatusCode);

                return await _responseHandler.HandleFailedResponseAsync(apiRequest, httpResponse, cancellationToken);
            }

            _logger?.LogInformation("The [{HttpMethod}] Request to {Url} was successful. Status[{StatusCode}]", apiRequest.Method, apiRequest.FullRoute, httpResponse.StatusCode);

            return await _responseHandler.HandleSuccessfulResponseAsync(apiRequest, httpResponse, cancellationToken);
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