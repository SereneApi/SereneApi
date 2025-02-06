using Microsoft.Extensions.Logging;
using SereneApi.Http;
using SereneApi.Response;
using SereneApi.Response.Handler;
using System;
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

        private readonly IApiResponseHandler _responseHandler;

        private readonly ILogger? _logger;

        public ApiRequestHandler(IHttpClientProvider httpClientProvider, IApiResponseHandler responseHandler, ILogger<ApiRequestHandler>? logger = null)
        {
            _httpClientProvider = httpClientProvider;
            _responseHandler = responseHandler;
            _logger = logger;
        }

        public async Task<IApiResponse> SendAsync(IApiRequest apiRequest, CancellationToken cancellationToken = default)
        {
            HttpClient client = _httpClientProvider.GetHttpClient();

            using HttpRequestMessage httpRequest = BuildRequestMessage(apiRequest);

            Stopwatch stopwatch = Stopwatch.StartNew();

            HttpResponseMessage httpResponse = await SendAsync(client, httpRequest, stopwatch, cancellationToken);

            return await HandleResponseAsync(httpResponse, apiRequest, stopwatch.Elapsed, cancellationToken);
        }

        private async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage httpRequest, Stopwatch stopwatch, CancellationToken cancellationToken)
        {
            try
            {
                return await client.SendAsync(httpRequest, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }

                throw new TimeoutException();
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private async Task<IApiResponse> HandleResponseAsync(HttpResponseMessage httpResponse, IApiRequest apiRequest, TimeSpan responseTime, CancellationToken cancellationToken)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger?.LogWarning("The [{HttpMethod}] Request to {Url} was not successful. Status[{StatusCode}]", apiRequest.Method, apiRequest.FullRoute, httpResponse.StatusCode);

                return await _responseHandler.HandleFailedResponseAsync(apiRequest, httpResponse, responseTime, cancellationToken);
            }

            _logger?.LogInformation("The [{HttpMethod}] Request to {Url} was successful. Status[{StatusCode}]", apiRequest.Method, apiRequest.FullRoute, httpResponse.StatusCode);

            return await _responseHandler.HandleSuccessfulResponseAsync(apiRequest, httpResponse, responseTime, cancellationToken);
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