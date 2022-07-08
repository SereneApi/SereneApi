using Microsoft.Extensions.Logging;
using SereneApi.Core.Http;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Content.Types;
using SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors;
using SereneApi.Extensions.Mocking.Rest.Extensions;
using SereneApi.Extensions.Mocking.Rest.Handler.Manager;
using SereneApi.Extensions.Mocking.Rest.Helpers;
using SereneApi.Handlers.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Manager
{
    internal class MockResponseManager : IMockResponseManager
    {
        private readonly IReadOnlyList<IMockRequestDescriptor> _mockRequests;
        private readonly IMockHandlerManager _mockHandlerManager;
        private readonly IConnectionSettings _connection;
        private readonly ILogger _logger;

        private int _timesDelayed;

        public MockResponseManager(IReadOnlyList<IMockRequestDescriptor> mockRequests, IConnectionSettings connection, IMockHandlerManager mockHandlerManager, ILogger logger = null)
        {
            _mockRequests = mockRequests;
            _connection = connection;
            _mockHandlerManager = mockHandlerManager;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> GetMockResponseAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            IMockResponse mockResponse = await GetWeightedResponseAsync(request);

            if (mockResponse != null)
            {
                return GetResponseMessage(mockResponse, request, cancellationToken);
            }

            _logger?.LogWarning("No MockResponse was found for request {requestUri}", request.RequestUri);

            return null;
        }

        private HttpResponseMessage GetResponseMessage(IMockResponse mockResponse, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (mockResponse.Delay != null && (_timesDelayed < mockResponse.Delay.Repeats || mockResponse.Delay.Repeats == 0))
            {
                _logger?.LogDebug("Delaying the Mock Response by {time} for request {requestUri}", mockResponse.Delay.Time.ToHumanReadableString(2), request.RequestUri);

                _timesDelayed++;

                bool canceled = cancellationToken.WaitHandle.WaitOne(mockResponse.Delay.Time);

                if (canceled)
                {
                    _logger?.LogDebug("The request for {requestUri} has exceeded the timeout period", request.RequestUri);

                    throw new TaskCanceledException("The response was canceled as it breached the timeout time.");
                }
            }

            if (mockResponse.Content != null)
            {
                return new HttpResponseMessage
                {
                    StatusCode = mockResponse.Status.ToHttpStatusCode(),
                    Content = (HttpContent)mockResponse.Content.GetContent()
                };
            }

            return new HttpResponseMessage
            {
                StatusCode = mockResponse.Status.ToHttpStatusCode()
            };
        }

        private bool TryGetRequestEndpoint(HttpRequestMessage request, out string requestEndpoint)
        {
            requestEndpoint = string.Empty;

            if (request.RequestUri == null)
            {
                throw new ArgumentNullException(nameof(request.RequestUri), "A request Uri must be present.");
            }

            string baseRoute = _connection.GetBaseRoute();
            string requestUri = request.RequestUri.ToString();

            if (!requestUri.StartsWith(baseRoute))
            {
                _logger?.LogDebug("The request Uri \"{requestUri}\" does not match the connection of the {handlerType}", request.RequestUri, typeof(RestApiHandler));

                return false;
            }

            requestEndpoint = requestUri.Remove(0, baseRoute.Length);

            if (requestEndpoint.StartsWith('/'))
            {
                requestEndpoint = requestEndpoint.Substring(1, requestEndpoint.Length - 1);
            }

            return true;
        }

        /// <summary>
        /// Weighs the <see cref="HttpRequestMessage"/> against the registered <see cref="IMockRequestDescriptor"/>s weighing each to find the best <see cref="IMockResponse"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to be matched.</param>
        /// <returns>The matching <see cref="IMockResponse"/> or <see langword="null"/> if no response was found.</returns>
        private async Task<IMockResponse> GetWeightedResponseAsync(HttpRequestMessage request)
        {
            int bestWeight = 0;

            IMockResponse bestResponse = null;

            if (!TryGetRequestEndpoint(request, out string requestEndpoint))
            {
                // Request endpoints don't match.
                return null;
            }

            foreach (IMockRequestDescriptor mockRequest in _mockRequests)
            {
                int weight = 0;

                if (mockRequest.Methods is { Length: > 0 })
                {
                    if (mockRequest.Methods.Contains(request.Method))
                    {
                        weight++;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (mockRequest.Endpoints is { Length: > 0 })
                {
                    int maxEndpointWeight = -1;

                    foreach (string endpoint in mockRequest.Endpoints)
                    {
                        int endpointWeight = CompareEndpoints(endpoint, requestEndpoint);

                        // Skip if the endpoints don't match.
                        if (endpointWeight == -1)
                        {
                            continue;
                        }

                        // If the endpoints match set the value.
                        maxEndpointWeight = endpointWeight;

                        // If the endpoint was a complete match end search.
                        if (maxEndpointWeight == 1)
                        {
                            break;
                        }

                    }

                    // If we did not find a match don't process this mock response.
                    if (maxEndpointWeight == -1)
                    {
                        continue;
                    }

                    weight += maxEndpointWeight;
                }

                IMockResponse response = null;

                if (mockRequest is IMockResponseDescriptor mockResponse)
                {
                    if (request.Content != null && mockResponse.Content != null)
                    {
                        string content = await request.Content.ReadAsStringAsync();

                        IRequestContent requestContent = new JsonContent(content);

                        if (mockResponse.Content.Equals(requestContent))
                        {
                            weight++;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    response = mockResponse.Response;
                }
                else if (mockRequest is IMockHandlerDescriptor mockHandler)
                {
                    try
                    {
                        _logger?.LogDebug("Mock Handler {handler} found for request {requestUri}", mockHandler.HandlerType.Name, request.RequestUri.ToString());

                        response = await _mockHandlerManager.InvokeHandlerAsync(mockHandler, request, requestEndpoint);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "An exception was encountered whilst invoking {handler}.{httpMethod} for request {requestUri}", mockHandler.HandlerType.Name, mockHandler.Method.Name, request.RequestUri.ToString());
                    }
                }

                if (weight <= bestWeight)
                {
                    continue;
                }

                bestWeight = weight;
                bestResponse = response;
            }

            return bestResponse;
        }

        private int CompareEndpoints(string mockEndpoint, string requestEndpoint)
        {
            if (mockEndpoint == null)
            {
                throw new ArgumentNullException(nameof(mockEndpoint), "Mock Endpoint cannot be null");
            }

            if (requestEndpoint == null)
            {
                throw new ArgumentNullException(nameof(requestEndpoint), "Request Endpoint cannot be null");
            }

            if (mockEndpoint.Equals(requestEndpoint))
            {
                // Complete match, return 1.
                return 1;
            }

            string[] endpointSections = mockEndpoint.Split('/');
            string[] requestSections = requestEndpoint.Split('/');

            if (requestSections.Length > mockEndpoint.Length)
            {
                // The request contains more sections than the endpoint, they do not match.
                return -1;
            }

            for (int i = 0; i < endpointSections.Length; i++)
            {
                if (ParameterHelper.IsParameter(endpointSections[i]))
                {
                    if (requestSections.Length - 1 < i)
                    {
                        if (ParameterHelper.IsParameterOptional(endpointSections[i]))
                        {
                            // The section is optional, so the check passes.
                            continue;
                        }

                        // The section is not optional, because it is not present we fail this check.
                        return -1;
                    }

                    continue;
                }

                if (requestSections.Length - 1 < i)
                {
                    // The request doesn't match as it does not contain enough sections.
                    return -1;
                }

                if (!endpointSections[i].Equals(requestSections[i]))
                {
                    // The section is not a parameter and they do not match.
                    return -1;
                }
            }

            // This is not a complete match, but the endpoint can process the parameters.
            return 0;
        }


    }
}