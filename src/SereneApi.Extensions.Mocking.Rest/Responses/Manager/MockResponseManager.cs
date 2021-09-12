using SereneApi.Core.Content;
using SereneApi.Core.Content.Types;
using SereneApi.Core.Serialization;
using SereneApi.Extensions.Mocking.Rest.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Manager
{
    public class MockResponseManager : IMockResponseManager
    {
        private readonly IReadOnlyList<IMockResponseDescriptor> _mockResponses;

        private readonly ISerializer _serializer;

        private int _timesDelayed = 0;

        public MockResponseManager(IReadOnlyList<IMockResponseDescriptor> mockResponses, ISerializer serializer)
        {
            _mockResponses = mockResponses;

            _serializer = serializer;
        }

        #region Asynchronous Methods

        public async Task<HttpResponseMessage> GetMockResponseAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            IMockResponse mockResponse = await GetWeightedResponseAsync(request);

            if (mockResponse == null)
            {
                return null;
            }

            return await GetResponseMessageAsync(mockResponse, cancellationToken);
        }

        protected virtual async Task<HttpResponseMessage> GetResponseMessageAsync(IMockResponse mockResponse, CancellationToken cancellationToken)
        {
            if (mockResponse.Delay != null && (_timesDelayed < mockResponse.Delay.Repeats || mockResponse.Delay.Repeats == 0))
            {
                _timesDelayed++;

                await Task.Factory.StartNew(() =>
                {
                    bool canceled = cancellationToken.WaitHandle.WaitOne(mockResponse.Delay.Time);

                    if (canceled)
                    {
                        throw new TaskCanceledException("The response was canceled as it breached the timeout time.");
                    }
                }, cancellationToken);
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

        protected virtual async Task<IMockResponse> GetWeightedResponseAsync(HttpRequestMessage request)
        {
            int bestWeight = 0;

            IMockResponseDescriptor bestResponse = null;

            foreach (IMockResponseDescriptor mockResponse in _mockResponses)
            {
                int weight = 0;

                if (mockResponse.Methods is { Length: > 0 })
                {
                    if (mockResponse.Methods.Contains(request.Method.ToMethod()))
                    {
                        weight++;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (mockResponse.Endpoints is { Length: > 0 })
                {
                    if (mockResponse.Endpoints.Contains(request.RequestUri))
                    {
                        weight++;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (request.Content != null)
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

                if (weight <= bestWeight)
                {
                    continue;
                }

                bestWeight = weight;
                bestResponse = mockResponse;
            }

            return bestResponse?.Response;
        }

        #endregion Asynchronous Methods

        #region Synchronous Methods

        public HttpResponseMessage GetMockResponse(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            IMockResponse mockResponse = GetWeightedResponse(request);

            if (mockResponse == null)
            {
                return null;
            }

            return GetResponseMessage(mockResponse, cancellationToken);
        }

        protected virtual HttpResponseMessage GetResponseMessage(IMockResponse mockResponse, CancellationToken cancellationToken)
        {
            if (mockResponse.Delay != null && (_timesDelayed < mockResponse.Delay.Repeats || mockResponse.Delay.Repeats == 0))
            {
                _timesDelayed++;

                bool canceled = cancellationToken.WaitHandle.WaitOne(mockResponse.Delay.Time);

                if (canceled)
                {
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

        protected virtual IMockResponse GetWeightedResponse(HttpRequestMessage request)
        {
            int bestWeight = 0;

            IMockResponseDescriptor bestResponse = null;

            foreach (IMockResponseDescriptor mockResponse in _mockResponses)
            {
                int weight = 0;

                if (mockResponse.Methods is { Length: > 0 })
                {
                    if (mockResponse.Methods.Contains(request.Method.ToMethod()))
                    {
                        weight++;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (mockResponse.Endpoints is { Length: > 0 })
                {
                    if (mockResponse.Endpoints.Contains(request.RequestUri))
                    {
                        weight++;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (request.Content != null)
                {
                    string content = request.Content.ReadAsStringAsync().Result;

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

                if (weight <= bestWeight)
                {
                    continue;
                }

                bestWeight = weight;
                bestResponse = mockResponse;
            }

            return bestResponse?.Response;
        }

        #endregion Synchronous Methods
    }
}