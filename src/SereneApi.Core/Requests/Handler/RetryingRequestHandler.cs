using Microsoft.Extensions.Logging;
using SereneApi.Core.Connection;
using SereneApi.Core.Content;
using SereneApi.Core.Events;
using SereneApi.Core.Events.Types;
using SereneApi.Core.Extensions;
using SereneApi.Core.Factories;
using SereneApi.Core.Handler;
using SereneApi.Core.Responses;
using SereneApi.Core.Responses.Handlers;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Core.Requests.Handler
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

        #region Asynchronous Methods

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
            catch (Exception e)
            {
                throw e;
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
            catch (Exception e)
            {
                throw e;
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

        protected virtual Task<HttpResponseMessage> HandleRequestAsync(HttpClient client, Uri route, Method method, IRequestContent content, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage request = BuildRequestMessage(route, method, content);

            return client.SendAsync(request, cancellationToken);
        }

        protected virtual Task<HttpResponseMessage> HandleRequestAsync(HttpClient client, Uri route, Method method, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage request = BuildRequestMessage(route, method);

            return client.SendAsync(request, cancellationToken);
        }

        protected virtual async Task<HttpResponseMessage> InternalPerformAsync(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default)
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
            } while (retryingRequest);

            throw new TimeoutException($"The [{request.Method}] request to \"{GetRequestRoute(request)}\" has Timed out; The retry limit has been reached after attempting {requestsAttempted} times");
        }

        #endregion Asynchronous Methods

        #region Synchronous Methods

        public IApiResponse Perform(IApiRequest request, IApiHandler caller)
        {
            HttpResponseMessage response = null;

            try
            {
                Stopwatch requestTimer = Stopwatch.StartNew();

                response = InternalPerform(request, caller);

                requestTimer.Stop();

                IApiResponse apiResponse = _responseHandler.ProcessResponse(request, requestTimer.Elapsed, response);

                _eventManager?.PublishAsync(new ResponseEvent(caller, apiResponse)).FireAndForget();

                return apiResponse;
            }
            catch (Exception e)
            {
                throw e;
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

        public IApiResponse<TResponse> Perform<TResponse>(IApiRequest request, IApiHandler caller)
        {
            HttpResponseMessage response = null;

            try
            {
                Stopwatch requestTimer = Stopwatch.StartNew();

                response = InternalPerform(request, caller);

                requestTimer.Stop();

                IApiResponse<TResponse> apiResponse =
                    _responseHandler.ProcessResponse<TResponse>(request, requestTimer.Elapsed, response);

                _eventManager?.PublishAsync(new ResponseEvent(caller, apiResponse)).FireAndForget();

                return apiResponse;
            }
            catch (Exception e)
            {
                throw e;
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

        protected virtual HttpResponseMessage HandleRequest(HttpClient client, Uri route, Method method, IRequestContent content)
        {
            HttpRequestMessage request = BuildRequestMessage(route, method, content);

            return client.Send(request);
        }

        protected virtual HttpResponseMessage HandleRequest(HttpClient client, Uri route, Method method)
        {
            HttpRequestMessage request = BuildRequestMessage(route, method);

            return client.Send(request);
        }

        protected virtual HttpResponseMessage InternalPerform(IApiRequest request, IApiHandler caller)
        {
            _eventManager?.PublishAsync(new RequestEvent(caller, request)).FireAndForget();

            bool retryingRequest = false;
            int requestsAttempted = 0;

            HttpClient client = _clientFactory.BuildClient();

            do
            {
                try
                {
                    HttpResponseMessage response;

                    if (request.Content == null)
                    {
                        _logger?.LogInformation(Logging.EventIds.PerformRequestEvent, Logging.Messages.PerformingRequest, request.Method.ToString(), GetRequestRoute(request));

                        response = HandleRequest(client, request.Route, request.Method);
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

                        response = HandleRequest(client, request.Route, request.Method, request.Content);
                    }

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
            } while (retryingRequest);

            throw new TimeoutException($"The [{request.Method}] request to \"{GetRequestRoute(request)}\" has Timed out; The retry limit has been reached after attempting {requestsAttempted} times");
        }

        #endregion Synchronous Methods

        private HttpRequestMessage BuildRequestMessage(Uri route, Method method)
        {
            HttpMethod httpMethod = method switch
            {
                Method.None => throw new ArgumentException("None is an invalid method for a request"),
                Method.Post => HttpMethod.Post,
                Method.Get => HttpMethod.Get,
                Method.Put => HttpMethod.Put,
                Method.Patch => HttpMethod.Patch,
                Method.Delete => HttpMethod.Delete,
                _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                    "An unknown Method Value was supplied provided")
            };

            return new HttpRequestMessage(httpMethod, route);
        }

        private HttpRequestMessage BuildRequestMessage(Uri route, Method method, IRequestContent content)
        {
            HttpMethod httpMethod = method switch
            {
                Method.None => throw new ArgumentException("None is an invalid method for a request"),
                Method.Post => HttpMethod.Post,
                Method.Get => throw new ArgumentException("A GET request may not have in body content"),
                Method.Put => HttpMethod.Put,
                Method.Patch => HttpMethod.Patch,
                Method.Delete => throw new ArgumentException("A DELETE request may not have in body content"),
                _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                    "An unknown Method Value was supplied provided")
            };

            return new HttpRequestMessage(httpMethod, route)
            {
                Content = (HttpContent)content.GetContent()
            };
        }

        private string GetRequestRoute(IApiRequest request)
        {
            return $"{_connection.BaseAddress}{request.Route}";
        }
    }
}