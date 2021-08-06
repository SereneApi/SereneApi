﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Connection;
using SereneApi.Abstractions.Content;
using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Events.Types;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Response;
using SereneApi.Abstractions.Response.Handlers;

namespace SereneApi.Abstractions.Requests.Handler
{
    public class RetryingRequestHandler : IRequestHandler
    {
        private readonly ILogger _logger;

        private readonly IClientFactory _clientFactory;

        private readonly IResponseHandler _responseHandler;

        private readonly IConnectionSettings _connection;

        private readonly IEventManager _eventManager;

        public RetryingRequestHandler(IDependencyProvider dependencies)
        {
            _clientFactory = dependencies.GetDependency<IClientFactory>();
            _connection = dependencies.GetDependency<IConnectionSettings>();
            _responseHandler = dependencies.GetDependency<IResponseHandler>();

            dependencies.TryGetDependency(out _logger);
            dependencies.TryGetDependency(out _eventManager);
        }

        public async Task<IApiResponse> PerformAsync(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = null;

            try
            {
                response = await InternalPerformAsync(request, caller, cancellationToken);

                IApiResponse apiResponse = await _responseHandler.ProcessResponseAsync(request, response);

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
                response = await InternalPerformAsync(request, caller, cancellationToken);

                IApiResponse<TResponse> apiResponse = await _responseHandler.ProcessResponseAsync<TResponse>(request, response);

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

        private async Task<HttpResponseMessage> InternalPerformAsync(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default)
        {
            _eventManager?.PublishAsync(new RequestEvent(caller, request)).FireAndForget();

            bool retryingRequest = false;
            int requestsAttempted = 0;

            HttpClient client = await _clientFactory.BuildClientAsync();

            do
            {
                try
                {
                    HttpResponseMessage response;

                    if (request.Content == null)
                    {
                        _logger?.LogInformation(Logging.EventIds.PerformRequestEvent, Logging.Messages.PerformingRequest, request.Method.ToString(), GetRequestRoute(request));

                        response = await HandleRequestAsync(client, request.Route, request.Method, cancellationToken);
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

                        response = await HandleRequestAsync(client, request.Route, request.Method, request.Content, cancellationToken);
                    }

                    retryingRequest = false;

                    _logger?.LogInformation(Logging.EventIds.ResponseReceivedEvent, Logging.Messages.ReceivedResponse, request.Method.ToString(), GetRequestRoute(request), response.StatusCode);

                    return response;
                }
                catch (TaskCanceledException canceledException)
                {
                    // TODO: This may be thrown if a task is cancelled by the CancellationToken.

                    requestsAttempted++;

                    if (_connection.RetryAttempts == 0 || requestsAttempted == _connection.RetryAttempts)
                    {
                        _logger?.LogWarning(Logging.EventIds.RetryEvent, canceledException, Logging.Messages.TimeoutNoRetry, request.Method, GetRequestRoute(request), requestsAttempted);

                        retryingRequest = false;
                    }
                    else
                    {
                        _logger?.LogWarning(Logging.EventIds.RetryEvent, Logging.Messages.TimeoutRetry, request.Method, GetRequestRoute(request), _connection.RetryAttempts - requestsAttempted);

                        _eventManager?.PublishAsync(new RetryEvent(caller, request)).FireAndForget();

                        retryingRequest = true;
                    }
                }
                finally
                {
                    if (retryingRequest == false)
                    {
                        client.Dispose();

                        _logger?.LogDebug(Logging.EventIds.DisposedEvent, Logging.Messages.DisposedHttpClient, request.Method, GetRequestRoute(request));
                    }
                }
            } while (retryingRequest);

            throw new TimeoutException($"The [{request.Method}] request to \"{GetRequestRoute(request)}\" has Timed out; The retry limit has been reached after attempting {requestsAttempted} times");
        }

        private string GetRequestRoute(IApiRequest request)
        {
            return $"{_connection.BaseAddress}{request.Route}";
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
    }
}
