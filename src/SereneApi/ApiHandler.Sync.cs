using Microsoft.Extensions.Logging;
using SereneApi.Abstraction.Enums;
using SereneApi.Extensions;
using SereneApi.Helpers;
using SereneApi.Interfaces.Requests;
using SereneApi.Types;
using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SereneApi
{
    public abstract partial class ApiHandler
    {
        #region Perform Methods

        /// <summary>
        /// Performs an API Request Synchronously.
        /// </summary>
        /// <param name="method">The <see cref="Method"/> that will be used for the request.</param>
        /// <param name="request">The <see cref="IRequest"/> that will be performed.</param>
        protected IApiResponse PerformRequest(Method method, Expression<Func<IRequest, IRequestCreated>> request = null)
        {
            CheckIfDisposed();

            RequestBuilder requestBuilder = new RequestBuilder(_routeFactory, _queryFactory, _serializer, Resource);

            requestBuilder.UsingMethod(method);

            request?.Compile().Invoke(requestBuilder);

            return BasePerformRequest(requestBuilder.GetRequest());
        }

        /// <summary>
        /// Performs an API Request Synchronously.
        /// </summary>
        /// <param name="method">The <see cref="Method"/> that will be used for the request.</param>
        /// <param name="request">The <see cref="IRequest"/> that will be performed.</param>
        /// <typeparam name="TResponse">The <see cref="Type"/> to be deserialized from the body of the response.</typeparam>
        protected IApiResponse<TResponse> PerformRequest<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> request = null)
        {
            CheckIfDisposed();

            RequestBuilder requestBuilder = new RequestBuilder(_routeFactory, _queryFactory, _serializer, Resource);

            requestBuilder.UsingMethod(method);

            request?.Compile().Invoke(requestBuilder);

            return BasePerformRequest<TResponse>(requestBuilder.GetRequest());
        }

        #endregion
        #region Base Action Methods

        protected virtual IApiResponse BasePerformRequest(IApiRequest request)
        {
            HttpResponseMessage responseMessage;

            Uri endPoint = request.EndPoint;

            Method method = request.Method;

            try
            {
                if(request.Content == null)
                {
                    responseMessage = RetryRequest(async () =>
                    {
                        return method switch
                        {
                            Method.Post => await Client.PostAsync(endPoint, null),
                            Method.Get => await Client.GetAsync(endPoint),
                            Method.Put => await Client.PutAsync(endPoint, null),
                            Method.Patch => await Client.PatchAsync(endPoint, null),
                            Method.Delete => await Client.DeleteAsync(endPoint),
                            _ => throw new ArgumentOutOfRangeException(nameof(endPoint), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }
                else
                {
                    HttpContent content = (HttpContent)request.Content.GetContent();

                    responseMessage = RetryRequest(async () =>
                    {
                        return method switch
                        {
                            Method.Post => await Client.PostAsync(endPoint, content),
                            Method.Get => throw new ArgumentException(
                                "Get cannot be used in conjunction with an InBody Request"),
                            Method.Put => await Client.PutAsync(endPoint, content),
                            Method.Patch => await Client.PatchAsync(endPoint, content),
                            Method.Delete => throw new ArgumentException(
                                "Delete cannot be used in conjunction with an InBody Request"),
                            _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }
            }
            catch(ArgumentException)
            {
                // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                throw;
            }
            catch(TimeoutException timeoutException)
            {
                return ApiResponse.Failure(Status.None, "The Request Timed Out; Retry limit reached", timeoutException);
            }
            catch(Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    method.ToString(), endPoint);

                return ApiResponse.Failure(Status.None, $"An Exception occured whilst performing a HTTP {method} Request",
                    exception);
            }

            return ProcessResponse(responseMessage);
        }

        protected virtual IApiResponse<TResponse> BasePerformRequest<TResponse>(IApiRequest request)
        {
            HttpResponseMessage responseMessage;

            Uri endPoint = request.EndPoint;

            Method method = request.Method;

            try
            {
                if(request.Content == null)
                {
                    responseMessage = RetryRequest(async () =>
                    {
                        return method switch
                        {
                            Method.Post => await Client.PostAsync(endPoint, null),
                            Method.Get => await Client.GetAsync(endPoint),
                            Method.Put => await Client.PutAsync(endPoint, null),
                            Method.Patch => await Client.PatchAsync(endPoint, null),
                            Method.Delete => await Client.DeleteAsync(endPoint),
                            _ => throw new ArgumentOutOfRangeException(nameof(endPoint), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }
                else
                {
                    HttpContent content = (HttpContent)request.Content.GetContent();

                    responseMessage = RetryRequest(async () =>
                    {
                        return method switch
                        {
                            Method.Post => await Client.PostAsync(endPoint, content),
                            Method.Get => throw new ArgumentException(
                                "Get cannot be used in conjunction with an InBody Request"),
                            Method.Put => await Client.PutAsync(endPoint, content),
                            Method.Patch => await Client.PatchAsync(endPoint, content),
                            Method.Delete => throw new ArgumentException(
                                "Delete cannot be used in conjunction with an InBody Request"),
                            _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }
            }
            catch(ArgumentException)
            {
                // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                throw;
            }
            catch(TimeoutException timeoutException)
            {
                return ApiResponse<TResponse>.Failure(Status.None, "The Request Timed Out; Retry limit reached", timeoutException);
            }
            catch(Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    method.ToString(), endPoint);

                return ApiResponse<TResponse>.Failure(Status.None, $"An Exception occured whilst performing a HTTP {method} Request",
                    exception);
            }

            return ProcessResponse<TResponse>(responseMessage);
        }

        /// <summary>
        /// Retries the request to the specified retry count.
        /// </summary>
        /// <param name="requestAction">The request to be performed.</param>
        /// <param name="route">The route to be inserted into the log.</param>
        /// <returns></returns>
        private HttpResponseMessage RetryRequest(Func<Task<HttpResponseMessage>> requestAction, Uri route)
        {
            bool retryingRequest;
            int requestsAttempted = 0;

            do
            {
                try
                {
                    // Using Task.Result bubbles the exception up to the caller.
                    // This means the Try Catch inside of RetryRequest does not catch the TaskCanceledException.
                    // The Try Catch in this method IS REQUIRED for the retry to functionality to work.
                    // To get around this, Task.GetAwaiter().GetResult() is necessary.
                    HttpResponseMessage responseMessage = requestAction.Invoke().GetAwaiter().GetResult();

                    return responseMessage;
                }
                catch(TaskCanceledException canceledException)
                {
                    requestsAttempted++;

                    if(_retry.Count == 0 || requestsAttempted == _retry.Count)
                    {
                        _logger?.LogError(canceledException, "The Request to \"{RequestRoute}\" has Timed Out; Retry limit reached. Retired {count}", route, requestsAttempted);

                        retryingRequest = false;
                    }
                    else
                    {
                        _logger?.LogWarning("Request to \"{RequestRoute}\" has Timed out, retrying. Attempts Remaining {count}", route, _retry.Count - requestsAttempted);

                        retryingRequest = true;
                    }
                }
            } while(retryingRequest);

            ExceptionHelper.RequestTimedOut(route, requestsAttempted);

            // This is redundant as ExceptionHelper.TimedOut will throw an exception.
            return null;
        }

        #endregion
        #region Response Processing

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/> deserializing the contained <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized from the response</typeparam>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        private IApiResponse<TResponse> ProcessResponse<TResponse>(HttpResponseMessage responseMessage)
        {
            if(responseMessage == null)
            {
                _logger?.LogWarning("Received an Empty Http Response");

                return ApiResponse<TResponse>.Failure(Status.None, "Received an Empty Http Response");
            }

            Status status = responseMessage.StatusCode.ToStatus();

            if(!responseMessage.IsSuccessStatusCode)
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                return ApiResponse<TResponse>.Failure(status, responseMessage.ReasonPhrase);
            }

            if(responseMessage.Content == null)
            {
                _logger.LogWarning("No content was received in the response.");

                return ApiResponse<TResponse>.Failure(status, "No content was received in the response.");
            }

            try
            {
                TResponse response = _serializer.Deserialize<TResponse>(responseMessage.Content);

                return ApiResponse<TResponse>.Success(status, response);
            }
            catch(JsonException jsonException)
            {
                _logger?.LogError(jsonException, "Could not deserialize the returned value");

                return ApiResponse<TResponse>.Failure(status, "Could not deserialize returned value.", jsonException);
            }
            catch(Exception exception)
            {
                _logger?.LogError(exception, "An Exception occured whilst processing the response.");

                return ApiResponse<TResponse>.Failure(status, "An Exception occured whilst processing the response.", exception);
            }
        }

        #endregion
    }
}
