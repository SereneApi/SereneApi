using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Events.Types;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi
{
    // Async Implementation
    public abstract partial class BaseApiHandler
    {
        #region Perform Methods

        /// <summary>
        /// Performs an API Request Asynchronously.
        /// </summary>
        /// <param name="request">The request to be performed.</param>
        /// <param name="cancellationToken">Cancels an ongoing request.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected internal virtual async Task<IApiResponse> PerformRequestAsync(IApiRequest request, CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _eventManager?.PublishAsync(new RequestEvent(this, request)).FireAndForget();

            HttpResponseMessage responseMessage = null;

            try
            {
                responseMessage = await PerformRequestWithRetryAsync(request, cancellationToken);

                IApiResponse response = ResponseHandler.ProcessResponse(request, responseMessage);

                _eventManager?.PublishAsync(new ResponseEvent(this, response)).FireAndForget();

                return response;
            }
            catch (TimeoutException exception)
            {
                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse.Failure(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, Logging.Messages.RequestEncounteredException, request.Method.ToString(), GetRequestRoute(request));

                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
            finally
            {
                if (responseMessage != null)
                {
                    responseMessage.Dispose();

                    _logger?.LogDebug(Logging.Messages.DisposedHttpResponseMessage, request.Method.ToString(), GetRequestRoute(request));
                }
            }
        }

        /// <summary>
        /// Performs an API Request Asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">The <see cref="Type"/> to be deserialized from the body of the response.</typeparam>
        /// <param name="request">The request to be performed.</param>
        /// <param name="cancellationToken">Cancels an ongoing request.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected internal virtual async Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>(IApiRequest request, CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _eventManager?.PublishAsync(new RequestEvent(this, request)).FireAndForget();

            HttpResponseMessage responseMessage = null;

            try
            {
                responseMessage = await PerformRequestWithRetryAsync(request, cancellationToken);

                IApiResponse<TResponse> response = ResponseHandler.ProcessResponse<TResponse>(request, responseMessage);

                _eventManager?.PublishAsync(new ResponseEvent(this, response)).FireAndForget();

                return response;
            }
            catch (TimeoutException exception)
            {
                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse<TResponse>.Failure(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, Logging.Messages.RequestEncounteredException, request.Method.ToString(), GetRequestRoute(request));

                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse<TResponse>.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
            finally
            {
                if (responseMessage != null)
                {
                    responseMessage.Dispose();

                    _logger?.LogDebug(Logging.Messages.DisposedHttpResponseMessage, request.Method.ToString(), GetRequestRoute(request));
                }
            }
        }

        /// <summary>
        /// Retries the request to the specified retry count.
        /// </summary>
        /// <param name="request">The request that will be performed.</param>
        /// <param name="cancellationToken">Cancels an ongoing request.</param>
        private async Task<HttpResponseMessage> PerformRequestWithRetryAsync(IApiRequest request, CancellationToken cancellationToken = default)
        {
            bool retryingRequest = false;
            int requestsAttempted = 0;

            IClientFactory clientFactory = _dependencies.GetDependency<IClientFactory>();

            HttpClient client = await clientFactory.BuildClientAsync();

            do
            {
                try
                {
                    HttpResponseMessage responseMessage;

                    if (request.Content == null)
                    {
                        _logger?.LogInformation(Logging.Messages.PerformingRequest, request.Method.ToString(), GetRequestRoute(request));

                        responseMessage = request.Method switch
                        {
                            Method.Post => await client.PostAsync(request.Route, null, cancellationToken),
                            Method.Get => await client.GetAsync(request.Route, cancellationToken),
                            Method.Put => await client.PutAsync(request.Route, null, cancellationToken),
                            Method.Patch => await client.PatchAsync(request.Route, null, cancellationToken),
                            Method.Delete => await client.DeleteAsync(request.Route, cancellationToken),
                            Method.None => throw new ArgumentException("None is an invalid method for a request"),
                            _ => throw new ArgumentOutOfRangeException(nameof(request.Method), request.Method,
                                "An unknown Method Value was supplied provided")
                        };
                    }
                    else
                    {
                        if (request.Method is Method.Get or Method.Delete or Method.None)
                        {
                            _logger?.LogError(Logging.Messages.InvalidMethodForInBodyContent, request.Method.ToString());
                        }
                        else
                        {
                            _logger?.LogDebug(Logging.Messages.PerformingRequestWithContent, request.Method.ToString(), GetRequestRoute(request), request.Content.GetContent());
                        }

                        HttpContent content = (HttpContent)request.Content.GetContent();

                        responseMessage = request.Method switch
                        {
                            Method.Post => await client.PostAsync(request.Route, content, cancellationToken),
                            Method.Get => throw new ArgumentException("A GET request may not have in body content"),
                            Method.Put => await client.PutAsync(request.Route, content, cancellationToken),
                            Method.Patch => await client.PatchAsync(request.Route, content, cancellationToken),
                            Method.Delete => throw new ArgumentException("A DELETE request may not have in body content"),
                            Method.None => throw new ArgumentException("None is an invalid method for a request"),
                            _ => throw new ArgumentOutOfRangeException(nameof(request.Method), request.Method,
                                "An unknown Method Value was supplied provided")
                        };
                    }

                    retryingRequest = false;

                    _logger?.LogInformation(Logging.Messages.ReceivedResponse, request.Method.ToString(), GetRequestRoute(request), responseMessage.StatusCode);

                    return responseMessage;
                }
                catch (TaskCanceledException canceledException)
                {
                    requestsAttempted++;

                    if (Connection.RetryAttempts == 0 || requestsAttempted == Connection.RetryAttempts)
                    {
                        _logger?.LogWarning(canceledException, Logging.Messages.TimeoutNoRetry, request.Method, GetRequestRoute(request), requestsAttempted);

                        retryingRequest = false;
                    }
                    else
                    {
                        _logger?.LogWarning(Logging.Messages.TimeoutRetry, request.Method, GetRequestRoute(request), Connection.RetryAttempts - requestsAttempted);

                        _eventManager?.PublishAsync(new RetryEvent(this, request)).FireAndForget();

                        retryingRequest = true;
                    }
                }
                finally
                {
                    if (retryingRequest == false)
                    {
                        client.Dispose();

                        _logger?.LogDebug(Logging.Messages.DisposedHttpClient, request.Method, GetRequestRoute(request));
                    }
                }
            } while (retryingRequest);

            throw new TimeoutException($"The [{request.Method}] request to \"{GetRequestRoute(request)}\" has Timed out; The retry limit has been reached after attempting {requestsAttempted} times");
        }

        #endregion
    }
}
