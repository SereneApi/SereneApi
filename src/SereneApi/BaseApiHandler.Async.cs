using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Extensions;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Request.Events;
using SereneApi.Abstractions.Response;
using SereneApi.Abstractions.Response.Events;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net.Http;
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
        /// <param name="method">The <see cref="Method"/> that will be used for the request.</param>
        /// <param name="factory">The <see cref="IRequest"/> that will be performed.</param>
        protected Task<IApiResponse> PerformRequestAsync(Method method, [AllowNull] Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            CheckIfDisposed();

            RequestBuilder requestBuilder = new RequestBuilder(Options, Connection.Resource);

            requestBuilder.UsingMethod(method);

            // The request is optional, so a null check is applied.
            factory?.Compile().Invoke(requestBuilder);

            IApiRequest required = requestBuilder.GetRequest();

            return PerformRequestAsync(required);
        }

        /// <summary>
        /// Performs an API Request Asynchronously.
        /// </summary>
        /// <param name="method">The <see cref="Method"/> that will be used for the request.</param>
        /// <param name="factory">The <see cref="IRequest"/> that will be performed.</param>
        /// <typeparam name="TResponse">The <see cref="Type"/> to be deserialized from the body of the response.</typeparam>
        protected Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>(Method method, [AllowNull] Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            CheckIfDisposed();

            RequestBuilder requestBuilder = new RequestBuilder(Options, Connection.Resource);

            requestBuilder.UsingMethod(method);

            // The request is optional, so a null check is applied.
            factory?.Compile().Invoke(requestBuilder);

            IApiRequest request = requestBuilder.GetRequest();

            return PerformRequestAsync<TResponse>(request);
        }

        /// <summary>
        /// Performs an API Request Asynchronously.
        /// </summary>
        /// <param name="request">The request to be performed.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected virtual async Task<IApiResponse> PerformRequestAsync([NotNull] IApiRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _eventManager?.PublishAsync(new RequestEvent(this, request)).FireAndForget();

            HttpResponseMessage responseMessage = null;

            Uri endpoint = request.Endpoint;

            Method method = request.Method;

            try
            {
                if(request.Content == null)
                {
                    _logger?.LogInformation("Performing a {httpMethod} request against {RequestRoute}", method.ToString(), GetRequestRoute(endpoint));

                    responseMessage = await RetryRequestAsync(async client =>
                    {
                        return method switch
                        {
                            Method.POST => await client.PostAsync(endpoint, null),
                            Method.GET => await client.GetAsync(endpoint),
                            Method.PUT => await client.PutAsync(endpoint, null),
                            Method.PATCH => await client.PatchAsync(endpoint, null),
                            Method.DELETE => await client.DeleteAsync(endpoint),
                            Method.NONE => throw new ArgumentException("None is not a valid method for a request."),
                            _ => throw new ArgumentOutOfRangeException(nameof(endpoint), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, request);
                }
                else
                {
                    _logger?.LogInformation("Performing a {httpMethod} request with in body content against {RequestRoute}", method.ToString(), GetRequestRoute(endpoint));

                    HttpContent content = (HttpContent)request.Content.GetContent();

                    responseMessage = await RetryRequestAsync(async client =>
                    {
                        return method switch
                        {
                            Method.POST => await client.PostAsync(endpoint, content),
                            Method.GET => throw new ArgumentException(
                                "Get cannot be used in conjunction with an InBody Request"),
                            Method.PUT => await client.PutAsync(endpoint, content),
                            Method.PATCH => await client.PatchAsync(endpoint, content),
                            Method.DELETE => throw new ArgumentException(
                                "Delete cannot be used in conjunction with an InBody Request"),
                            Method.NONE => throw new ArgumentException("None is not a valid method for a request."),
                            _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, request);
                }

                _logger?.LogInformation("The {httpMethod} request against {RequestRoute} completed successfully.", method.ToString(), GetRequestRoute(endpoint));

                IApiResponse response = ResponseHandler.ProcessResponse(request, responseMessage);

                _eventManager?.PublishAsync(new ResponseEvent(this, response)).FireAndForget();

                return response;
            }
            catch(ArgumentException exception)
            {
                if(method == Method.GET || method == Method.DELETE || method == Method.NONE)
                {
                    _logger?.LogWarning("An invalid method [{httpMethod}] was provided.", method.ToString());

                    // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                    throw;
                }

                _logger?.LogError(exception,
                    "An Exception occurred whilst performing a HTTP {httpMethod} Request to {RequestRoute}",
                    method.ToString(), GetRequestRoute(endpoint));

                if(Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {method} Request",
                    exception);
            }
            catch(TimeoutException exception)
            {
                _logger?.LogWarning(exception, "The Request Timed Out; Retry limit reach");

                if(Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse.Failure(request, Status.None, "The Request Timed Out; Retry limit reached", exception);
            }
            catch(Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occurred whilst performing a HTTP {httpMethod} Request to {RequestRoute",
                    method.ToString(), GetRequestRoute(endpoint));

                if(Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {method} Request",
                    exception);
            }
            finally
            {
                if(responseMessage != null)
                {
                    _logger?.LogInformation("Disposing response for the HTTP {httpMethod} Request to {RequestRoute}", method.ToString(), GetRequestRoute(endpoint));

                    responseMessage.Dispose();
                }
            }
        }

        /// <summary>
        /// Performs an API Request Asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">The <see cref="Type"/> to be deserialized from the body of the response.</typeparam>
        /// <param name="request">The request to be performed.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected virtual async Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>([NotNull] IApiRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _eventManager?.PublishAsync(new RequestEvent(this, request)).FireAndForget();

            HttpResponseMessage responseMessage = null;

            Uri endpoint = request.Endpoint;

            Method method = request.Method;

            try
            {
                if(request.Content == null)
                {
                    _logger?.LogInformation("Performing a {httpMethod} request against {RequestRoute}", method.ToString(), GetRequestRoute(endpoint));

                    responseMessage = await RetryRequestAsync(async client =>
                    {
                        return method switch
                        {
                            Method.POST => await client.PostAsync(endpoint, null),
                            Method.GET => await client.GetAsync(endpoint),
                            Method.PUT => await client.PutAsync(endpoint, null),
                            Method.PATCH => await client.PatchAsync(endpoint, null),
                            Method.DELETE => await client.DeleteAsync(endpoint),
                            Method.NONE => throw new ArgumentException("None is not a valid method for a request."),
                            _ => throw new ArgumentOutOfRangeException(nameof(endpoint), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, request);
                }
                else
                {
                    _logger?.LogInformation("Performing a {httpMethod} request with in body content against {RequestRoute}", method.ToString(), GetRequestRoute(endpoint));

                    HttpContent content = (HttpContent)request.Content.GetContent();

                    responseMessage = await RetryRequestAsync(async client =>
                    {
                        return method switch
                        {
                            Method.POST => await client.PostAsync(endpoint, content),
                            Method.GET => throw new ArgumentException(
                                "Get cannot be used in conjunction with an InBody Request"),
                            Method.PUT => await client.PutAsync(endpoint, content),
                            Method.PATCH => await client.PatchAsync(endpoint, content),
                            Method.DELETE => throw new ArgumentException(
                                "Delete cannot be used in conjunction with an InBody Request"),
                            Method.NONE => throw new ArgumentException("None is not a valid method for a request."),
                            _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, request);
                }

                _logger?.LogInformation("The {httpMethod} request against {RequestRoute} completed successfully.", method.ToString(), GetRequestRoute(endpoint));

                IApiResponse<TResponse> response = await ResponseHandler.ProcessResponseAsync<TResponse>(request, responseMessage);

                _eventManager?.PublishAsync(new ResponseEvent(this, response)).FireAndForget();

                return response;
            }
            catch(ArgumentException exception)
            {
                if(method == Method.GET || method == Method.DELETE || method == Method.NONE)
                {
                    _logger?.LogWarning("An invalid method [{httpMethod}] was provided.", method.ToString());

                    // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                    throw;
                }

                _logger?.LogError(exception,
                    "An Exception occurred whilst performing a HTTP {httpMethod} Request to {RequestRoute}",
                    method.ToString(), GetRequestRoute(endpoint));

                if(Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse<TResponse>.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {method} Request",
                    exception);
            }
            catch(TimeoutException exception)
            {
                _logger?.LogWarning(exception, "The Request Timed Out; Retry limit reach");

                if(Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse<TResponse>.Failure(request, Status.None, "The Request Timed Out; Retry limit reached", exception);
            }
            catch(Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occurred whilst performing a HTTP {httpMethod} Request to {RequestRoute}",
                    method.ToString(), GetRequestRoute(endpoint));

                if(Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse<TResponse>.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {method} Request",
                    exception);
            }
            finally
            {
                if(responseMessage != null)
                {
                    _logger?.LogInformation("Disposing response for the HTTP {httpMethod} Request to {RequestRoute}", method.ToString(), GetRequestRoute(endpoint));

                    responseMessage.Dispose();
                }
            }
        }

        /// <summary>
        /// Retries the request to the specified retry count.
        /// </summary>
        /// <param name="requestAction">The request to be performed.</param>
        /// <param name="request">The request that will be performed.</param>
        private async Task<HttpResponseMessage> RetryRequestAsync(Func<HttpClient, Task<HttpResponseMessage>> requestAction, IApiRequest request)
        {
            bool retryingRequest;
            int requestsAttempted = 0;

            do
            {
                try
                {
                    IClientFactory clientFactory = Options.RetrieveRequiredDependency<IClientFactory>();

                    HttpClient client = await clientFactory.BuildClientAsync();

                    HttpResponseMessage responseMessage = await requestAction.Invoke(client);

                    return responseMessage;
                }
                catch(TaskCanceledException canceledException)
                {
                    requestsAttempted++;

                    if(Connection.RetryAttempts == 0 || requestsAttempted == Connection.RetryAttempts)
                    {
                        _logger?.LogError(canceledException, "The Request to \"{RequestRoute}\" has Timed Out; Retry limit reached. Retired {count}", GetRequestRoute(request.Endpoint), requestsAttempted);

                        retryingRequest = false;
                    }
                    else
                    {
                        _logger?.LogWarning("Request to \"{RequestRoute}\" has Timed out, retrying. Attempts Remaining {count}", GetRequestRoute(request.Endpoint), Connection.RetryAttempts - requestsAttempted);

                        _eventManager?.PublishAsync(new RetryEvent(this, request)).FireAndForget();

                        retryingRequest = true;
                    }
                }
            } while(retryingRequest);

            throw new TimeoutException($"The Request to \"{GetRequestRoute(request.Endpoint)}\" has Timed Out; Retry limit reached. Retired {requestsAttempted}");
        }

        #endregion
    }
}
