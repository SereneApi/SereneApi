using Microsoft.Extensions.Logging;
using SereneApi.Core.Events;
using SereneApi.Core.Events.Types;
using SereneApi.Core.Handler;
using SereneApi.Core.Http.Client;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Http.Responses.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Core.Http.Requests.Handler
{
    public class RequestHandler : IRequestHandler
    {
        private readonly IConnectionSettings _connection;
        private readonly IEventManager _eventManager;
        private readonly ILogger _logger;
        private readonly IResponseHandler _responseHandler;

        protected IClientFactory ClientFactory { get; }

        public RequestHandler(IClientFactory clientFactory, IConnectionSettings connection, IResponseHandler responseHandler, IEventManager eventManager = null, ILogger logger = null)
        {
            ClientFactory = clientFactory;
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

                    _logger?.LogDebug(Logging.EventIds.DisposedEvent, Logging.Messages.DisposedHttpResponseMessage, request.HttpMethod.ToString(), GetRequestRoute(request));
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

                    _logger?.LogDebug(Logging.EventIds.DisposedEvent, Logging.Messages.DisposedHttpResponseMessage, request.HttpMethod.ToString(), GetRequestRoute(request));
                }
            }
        }

        protected virtual Task<HttpResponseMessage> HandleRequestAsync(HttpClient client, IApiRequest request, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(request.HttpMethod, request.Route);

            foreach (var (key, value) in request.Headers)
            {
                if (value is IEnumerable<string> stringEnumerable)
                {
                    requestMessage.Headers.Add(key, stringEnumerable);
                }
                else if (value is IEnumerable<object> objectEnumerable)
                {
                    requestMessage.Headers.Add(key, objectEnumerable.Select(v => v.ToString()));
                }
                else
                {
                    requestMessage.Headers.Add(key, value.ToString());
                }
            }

            if (request.Content == null)
            {
                _logger?.LogInformation(Logging.EventIds.PerformRequestEvent, Logging.Messages.PerformingRequest, request.HttpMethod.ToString(), GetRequestRoute(request));
            }
            else
            {
                if (request.HttpMethod == HttpMethod.Get || request.HttpMethod == HttpMethod.Delete)
                {
                    _logger?.LogError(Logging.EventIds.InvalidMethodForRequestEvent, Logging.Messages.InvalidMethodForInBodyContent, request.HttpMethod.ToString());
                }
                else
                {
                    _logger?.LogDebug(Logging.EventIds.PerformRequestEvent, Logging.Messages.PerformingRequestWithContent, request.HttpMethod.ToString(), GetRequestRoute(request), request.Content.GetContent());
                }

                requestMessage.Content = (HttpContent)request.Content.GetContent();
            }

            return client.SendAsync(requestMessage, cancellationToken);
        }

        protected virtual async Task<HttpResponseMessage> InternalPerformAsync(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default)
        {
            _eventManager?.PublishAsync(new RequestEvent(caller, request)).FireAndForget();

            HttpClient client = await ClientFactory.BuildClientAsync(out bool disposeClient);

            try
            {
                HttpResponseMessage response = await HandleRequestAsync(client, request, cancellationToken);

                _logger?.LogInformation(Logging.EventIds.ResponseReceivedEvent,
                    Logging.Messages.ReceivedResponse,
                    request.HttpMethod.ToString(), GetRequestRoute(request), response.StatusCode);

                return response;
            }
            catch (TaskCanceledException canceledException)
            {
                _logger?.LogWarning(Logging.EventIds.RetryEvent, canceledException, Logging.Messages.Timeout, request.HttpMethod, GetRequestRoute(request));
            }
            finally
            {
                if (disposeClient)
                {
                    client?.Dispose();
                }
            }

            throw new TimeoutException($"The [{request.HttpMethod}] request to \"{GetRequestRoute(request)}\" has Timed out");
        }

        protected virtual string GetRequestRoute(IApiRequest request)
        {
            return $"{_connection.BaseAddress}{request.Route}";
        }
    }
}