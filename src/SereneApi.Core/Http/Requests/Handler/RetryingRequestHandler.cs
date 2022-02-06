using SereneApi.Core.Events;
using SereneApi.Core.Events.Types;
using SereneApi.Core.Handler;
using SereneApi.Core.Http.Client;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Http.Responses.Handlers;
using SereneApi.Core.Requests;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Core.Http.Requests.Handler
{
    public class RetryingRequestHandler : IRequestHandler
    {
        private readonly IClientFactory _clientFactory;
        private readonly IConnectionSettings _connection;
        private readonly IEventManager _eventManager;
        private readonly ILogger _logger;
        private readonly IResponseHandler _responseHandler;

        public RetryingRequestHandler(IClientFactory clientFactory, IConnectionSettings connection, IResponseHandler responseHandler, IEventManager eventManager = null, ILogger logger = null)
        {
            _clientFactory = clientFactory;
            _connection = connection;
            _responseHandler = responseHandler;

            _logger = logger;
            _eventManager = eventManager;
        }

        public async Task<IApiResponse> PerformAsync(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = null;

            try
            {
                Stopwatch requestTimer = Stopwatch.StartNew();

                response = await InternalPerformAsync(request, caller, cancellationToken);

                requestTimer.Stop();

                IApiResponse apiResponse = await _responseHandler.ProcessResponseAsync(request, requestTimer.Elapsed, response);

                _eventManager?.PublishAsync(new ResponseEvent(caller, apiResponse)).FireAndForget();

                return apiResponse;
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();

                    _logger?.LogDebug(Logging.EventIds.DisposedEvent, Logging.Messages.DisposedHttpResponseMessage, request.Method.ToString(), GetRequestRoute(request));
                }
            }
        }

        public async Task<IApiResponse<TResponse>> PerformAsync<TResponse>(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = null;

            try
            {
                Stopwatch requestTimer = Stopwatch.StartNew();

                response = await InternalPerformAsync(request, caller, cancellationToken);

                requestTimer.Stop();

                IApiResponse<TResponse> apiResponse =
                    await _responseHandler.ProcessResponseAsync<TResponse>(request, requestTimer.Elapsed, response);

                _eventManager?.PublishAsync(new ResponseEvent(caller, apiResponse)).FireAndForget();

                return apiResponse;
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();

                    _logger?.LogDebug(Logging.EventIds.DisposedEvent, Logging.Messages.DisposedHttpResponseMessage, request.Method.ToString(), GetRequestRoute(request));
                }
            }
        }

        protected virtual async Task<HttpResponseMessage> HandleRequestAsync(HttpClient client, Uri route, Method method, IRequestContent content, CancellationToken cancellationToken = default)
        {
            HttpContent httpContent = (HttpContent)content.GetContent();

            return method switch
            {
                Method.Post => await client.PostAsync(route, httpContent, cancellationToken),
                Method.Get => throw new ArgumentException("A GET request may not have in body content"),
                Method.Put => await client.PutAsync(route, httpContent, cancellationToken),
                Method.Patch => await client.PatchAsync(route, httpContent, cancellationToken),
                Method.Delete => throw new ArgumentException("A DELETE request may not have in body content"),
                Method.None => throw new ArgumentException("None is an invalid method for a request"),
                _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                    "An unknown Method Value was supplied provided")
            };
        }

        protected virtual async Task<HttpResponseMessage> HandleRequestAsync(HttpClient client, Uri route, Method method, CancellationToken cancellationToken = default)
        {
            return method switch
            {
                Method.Post => await client.PostAsync(route, null, cancellationToken),
                Method.Get => await client.GetAsync(route, cancellationToken),
                Method.Put => await client.PutAsync(route, null, cancellationToken),
                Method.Patch => await client.PatchAsync(route, null, cancellationToken),
                Method.Delete => await client.DeleteAsync(route, cancellationToken),
                Method.None => throw new ArgumentException("None is an invalid method for a request"),
                _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                    "An unknown Method Value was supplied provided")
            };
        }

        protected virtual async Task<HttpResponseMessage> InternalPerformAsync(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default)
        {
            _eventManager?.PublishAsync(new RequestEvent(caller, request)).FireAndForget();

            HttpClient client = await _clientFactory.BuildClientAsync(out bool disposeClient);

            try
            {
                return await PerformRetryingRequestAsync(client, request, caller, cancellationToken);
            }
            finally
            {
                if (disposeClient)
                {
                    client?.Dispose();
                }
            }
        }

        private string GetRequestRoute(IApiRequest request)
        {
            return $"{_connection.BaseAddress}{request.Route}";
        }

        private async Task<HttpResponseMessage> PerformRetryingRequestAsync(HttpClient client, IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default)
        {
            bool retryingRequest;
            int requestsAttempted = 0;

            do
            {
                try
                {
                    HttpResponseMessage response;

                    if (request.Content == null)
                    {
                        _logger?.LogInformation(Logging.EventIds.PerformRequestEvent,
                            Logging.Messages.PerformingRequest, request.Method.ToString(),
                            GetRequestRoute(request));

                        response = await HandleRequestAsync(client, request.Route, request.Method,
                            cancellationToken);
                    }
                    else
                    {
                        if (request.Method is Method.Get or Method.Delete or Method.None)
                        {
                            _logger?.LogError(Logging.EventIds.InvalidMethodForRequestEvent,
                                Logging.Messages.InvalidMethodForInBodyContent, request.Method.ToString());
                        }
                        else
                        {
                            _logger?.LogDebug(Logging.EventIds.PerformRequestEvent,
                                Logging.Messages.PerformingRequestWithContent, request.Method.ToString(),
                                GetRequestRoute(request), request.Content.GetContent());
                        }

                        response = await HandleRequestAsync(client, request.Route, request.Method, request.Content,
                            cancellationToken);
                    }

                    _logger?.LogInformation(Logging.EventIds.ResponseReceivedEvent,
                        Logging.Messages.ReceivedResponse,
                        request.Method.ToString(), GetRequestRoute(request), response.StatusCode);

                    return response;
                }
                catch (TaskCanceledException canceledException)
                {
                    // TODO: This may be thrown if a task is cancelled by the CancellationToken.

                    requestsAttempted++;

                    if (_connection.RetryAttempts == 0 || requestsAttempted == _connection.RetryAttempts)
                    {
                        _logger?.LogWarning(Logging.EventIds.RetryEvent, canceledException,
                            Logging.Messages.TimeoutNoRetry, request.Method, GetRequestRoute(request),
                            requestsAttempted);

                        retryingRequest = false;
                    }
                    else
                    {
                        _logger?.LogWarning(Logging.EventIds.RetryEvent, Logging.Messages.TimeoutRetry,
                            request.Method,
                            GetRequestRoute(request), _connection.RetryAttempts - requestsAttempted);

                        _eventManager?.PublishAsync(new RetryEvent(caller, request)).FireAndForget();

                        retryingRequest = true;
                    }
                }
            } while (retryingRequest);

            throw new TimeoutException($"The [{request.Method}] request to \"{GetRequestRoute(request)}\" has Timed out; The retry limit has been reached after attempting {requestsAttempted} times");
        }
    }
}