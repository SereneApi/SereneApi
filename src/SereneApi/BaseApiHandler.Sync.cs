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
    public abstract partial class BaseApiHandler
    {
        #region Perform Methods

        /// <summary>
        /// Performs an API Request Synchronously.
        /// </summary>
        /// <param name="request">The request to be performed.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected internal virtual IApiResponse PerformRequest(IApiRequest request, CancellationToken cancellationToken = default)
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
                responseMessage = PerformRequestWithRetry(request, cancellationToken);

                _logger?.LogInformation("The {httpMethod} request against {RequestRoute} completed successfully.", request.Method.ToString(), GetRequestRoute(request));

                IApiResponse response = ResponseHandler.ProcessResponse(request, responseMessage);

                _eventManager?.PublishAsync(new ResponseEvent(this, response)).FireAndForget();

                return response;
            }
            catch (ArgumentException exception)
            {
                if (request.Method == Method.Get || request.Method == Method.Delete || request.Method == Method.None)
                {
                    _logger?.LogWarning("An invalid method [{httpMethod}] was provided.", request.Method.ToString());

                    // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                    throw;
                }

                _logger?.LogError(exception,
                    "An Exception occurred whilst performing a HTTP {httpMethod} Request to {RequestRoute}",
                    request.Method.ToString(), GetRequestRoute(request));

                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
            catch (TimeoutException exception)
            {
                _logger?.LogWarning(exception, "The Request Timed Out; Retry limit reach");

                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse.Failure(request, Status.None, "The Request Timed Out; Retry limit reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occurred whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    request.Method.ToString(), GetRequestRoute(request));

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
                    _logger?.LogInformation("Disposing response for the HTTP {httpMethod} Request to {RequestRoute}", request.Method.ToString(), GetRequestRoute(request));

                    responseMessage.Dispose();
                }
            }
        }

        /// <summary>
        /// Performs an API Request Synchronously.
        /// </summary>
        /// <typeparam name="TResponse">The <see cref="Type"/> to be deserialized from the body of the response.</typeparam>
        /// <param name="request">The request to be performed.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected internal virtual IApiResponse<TResponse> PerformRequest<TResponse>(IApiRequest request, CancellationToken cancellationToken = default)
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
                responseMessage = PerformRequestWithRetry(request, cancellationToken);

                _logger?.LogInformation("The {httpMethod} request against {RequestRoute} completed successfully.", request.Method.ToString(), GetRequestRoute(request));

                IApiResponse<TResponse> response = ResponseHandler.ProcessResponse<TResponse>(request, responseMessage);

                _eventManager?.PublishAsync(new ResponseEvent(this, response)).FireAndForget();

                return response;
            }
            catch (ArgumentException exception)
            {
                if (request.Method == Method.Get || request.Method == Method.Delete || request.Method == Method.None)
                {
                    _logger?.LogWarning("An invalid method [{httpMethod}] was provided.", request.Method.ToString());

                    // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                    throw;
                }

                _logger?.LogError(exception,
                    "An Exception occurred whilst performing a HTTP {httpMethod} Request to {RequestRoute}",
                    request.Method.ToString(), GetRequestRoute(request));

                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse<TResponse>.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
            catch (TimeoutException exception)
            {
                _logger?.LogWarning(exception, "The Request Timed Out; Retry limit reach");

                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse<TResponse>.Failure(request, Status.None, "The Request Timed Out; Retry limit reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occurred whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    request.Method.ToString(), GetRequestRoute(request));

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
                    _logger?.LogInformation("Disposing response for the HTTP {httpMethod} Request to {RequestRoute}", request.Method.ToString(), GetRequestRoute(request));

                    responseMessage.Dispose();
                }
            }
        }

        /// <summary>
        /// Retries the request to the specified retry count.
        /// </summary>
        /// <param name="requestAction">The request to be performed.</param>
        /// <param name="request">The request that will be performed.</param>
        private HttpResponseMessage PerformRequestWithRetry(IApiRequest request, CancellationToken cancellationToken = default)
        {
            bool retryingRequest = false;
            int requestsAttempted = 0;

            IClientFactory clientFactory = Options.Dependencies.GetDependency<IClientFactory>();

            HttpClient client = clientFactory.BuildClientAsync().Result;

            do
            {
                try
                {
                    // Using Task.Result bubbles the exception up to the caller.
                    // This means the Try Catch inside of PerformRequestWithRetry does not catch the TaskCanceledException.
                    // The Try Catch in this method IS REQUIRED for the retry to functionality to work.
                    // To get around this, Task.GetAwaiter().GetResult() is necessary.
                    HttpResponseMessage responseMessage;

                    if (request.Content == null)
                    {
                        _logger?.LogInformation("Performing a {httpMethod} request against {RequestRoute}", request.Method.ToString(), GetRequestRoute(request));

                        responseMessage = request.Method switch
                        {
                            Method.Post => client.PostAsync(request.Route, null, cancellationToken).GetAwaiter().GetResult(),
                            Method.Get => client.GetAsync(request.Route, cancellationToken).GetAwaiter().GetResult(),
                            Method.Put => client.PutAsync(request.Route, null, cancellationToken).GetAwaiter().GetResult(),
                            Method.Patch => client.PatchAsync(request.Route, null, cancellationToken).GetAwaiter().GetResult(),
                            Method.Delete => client.DeleteAsync(request.Route, cancellationToken).GetAwaiter().GetResult(),
                            Method.None => throw new ArgumentException("None is not a valid method for a request."),
                            _ => throw new ArgumentOutOfRangeException(nameof(request.Method), request.Method,
                                "An incorrect Method Value was supplied.")
                        };
                    }
                    else
                    {
                        _logger?.LogInformation("Performing a {httpMethod} request with in body content against {RequestRoute}", request.Method.ToString(), GetRequestRoute(request));

                        HttpContent content = (HttpContent)request.Content.GetContent();

                        responseMessage = request.Method switch
                        {
                            Method.Post => client.PostAsync(request.Route, content, cancellationToken).GetAwaiter().GetResult(),
                            Method.Get => throw new ArgumentException(
                                "Get cannot be used in conjunction with an InBody Request"),
                            Method.Put => client.PutAsync(request.Route, content, cancellationToken).GetAwaiter().GetResult(),
                            Method.Patch => client.PatchAsync(request.Route, content, cancellationToken).GetAwaiter().GetResult(),
                            Method.Delete => throw new ArgumentException(
                                "Delete cannot be used in conjunction with an InBody Request"),
                            Method.None => throw new ArgumentException("None is not a valid method for a request."),
                            _ => throw new ArgumentOutOfRangeException(nameof(request.Method), request.Method,
                                "An incorrect Method Value was supplied.")
                        };
                    }

                    retryingRequest = false;

                    return responseMessage ?? throw new NullReferenceException(nameof(responseMessage));
                }
                catch (TaskCanceledException canceledException)
                {
                    requestsAttempted++;

                    if (Connection.RetryAttempts == 0 || requestsAttempted == Connection.RetryAttempts)
                    {
                        _logger?.LogError(canceledException,
                            "The Request to \"{RequestRoute}\" has Timed Out; Retry limit reached. Retired {count}",
                            GetRequestRoute(request), requestsAttempted);

                        retryingRequest = false;
                    }
                    else
                    {
                        _logger?.LogWarning(
                            "Request to \"{RequestRoute}\" has Timed out, retrying. Attempts Remaining {count}",
                            GetRequestRoute(request), Connection.RetryAttempts - requestsAttempted);

                        _eventManager?.PublishAsync(new RetryEvent(this, request)).FireAndForget();

                        retryingRequest = true;
                    }
                }
                finally
                {
                    if (retryingRequest == false)
                    {
                        _logger?.LogDebug("Disposing HttpClient");

                        client.Dispose();
                    }
                }
            } while (retryingRequest);

            throw new TimeoutException($"The Request to \"{GetRequestRoute(request)}\" has Timed Out; Retry limit reached. Retired {requestsAttempted}");
        }

        #endregion
    }
}
