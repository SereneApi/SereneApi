using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;
using SereneApi.Abstractions.Response.Content;
using SereneApi.Abstractions.Serializers;
using SereneApi.Extensions;
using SereneApi.Helpers;
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
        /// <param name="factory">The <see cref="IRequest"/> that will be performed.</param>
        protected IApiResponse PerformRequest(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            CheckIfDisposed();

            RequestBuilder requestBuilder = new RequestBuilder(Dependencies, Connection.Resource);

            requestBuilder.UsingMethod(method);

            factory?.Compile().Invoke(requestBuilder);

            IApiRequest request = requestBuilder.GetRequest();

            return BasePerformRequest(request);
        }

        /// <summary>
        /// Performs an API Request Synchronously.
        /// </summary>
        /// <param name="method">The <see cref="Method"/> that will be used for the request.</param>
        /// <param name="factory">The <see cref="IRequest"/> that will be performed.</param>
        /// <typeparam name="TResponse">The <see cref="Type"/> to be deserialized from the body of the response.</typeparam>
        protected IApiResponse<TResponse> PerformRequest<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            CheckIfDisposed();

            RequestBuilder requestBuilder = new RequestBuilder(Dependencies, Connection.Resource);

            requestBuilder.UsingMethod(method);

            factory?.Compile().Invoke(requestBuilder);

            IApiRequest request = requestBuilder.GetRequest();

            return BasePerformRequest<TResponse>(request);
        }

        #endregion
        #region Base Action Methods

        protected virtual IApiResponse BasePerformRequest(IApiRequest request)
        {
            HttpResponseMessage responseMessage = null;

            Uri endPoint = request.EndPoint;

            Method method = request.Method;

            try
            {
                if(request.Content == null)
                {
                    responseMessage = RetryRequest(async client =>
                    {
                        return method switch
                        {
                            Method.POST => await client.PostAsync(endPoint, null),
                            Method.GET => await client.GetAsync(endPoint),
                            Method.PUT => await client.PutAsync(endPoint, null),
                            Method.PATCH => await client.PatchAsync(endPoint, null),
                            Method.DELETE => await client.DeleteAsync(endPoint),
                            _ => throw new ArgumentOutOfRangeException(nameof(endPoint), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }
                else
                {
                    HttpContent content = (HttpContent)request.Content.GetContent();

                    responseMessage = RetryRequest(async client =>
                    {
                        return method switch
                        {
                            Method.POST => await client.PostAsync(endPoint, content),
                            Method.GET => throw new ArgumentException(
                                "Get cannot be used in conjunction with an InBody Request"),
                            Method.PUT => await client.PutAsync(endPoint, content),
                            Method.PATCH => await client.PatchAsync(endPoint, content),
                            Method.DELETE => throw new ArgumentException(
                                "Delete cannot be used in conjunction with an InBody Request"),
                            _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }

                return ProcessResponse(responseMessage);
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

                return ApiResponse.Failure(Status.None,
                    $"An Exception occured whilst performing a HTTP {method} Request",
                    exception);
            }
            finally
            {
                responseMessage?.Dispose();
            }
        }

        protected virtual IApiResponse<TResponse> BasePerformRequest<TResponse>(IApiRequest request)
        {
            HttpResponseMessage responseMessage = null;

            Uri endPoint = request.EndPoint;

            Method method = request.Method;

            try
            {
                if(request.Content == null)
                {
                    responseMessage = RetryRequest(async client =>
                    {
                        return method switch
                        {
                            Method.POST => await client.PostAsync(endPoint, null),
                            Method.GET => await client.GetAsync(endPoint),
                            Method.PUT => await client.PutAsync(endPoint, null),
                            Method.PATCH => await client.PatchAsync(endPoint, null),
                            Method.DELETE => await client.DeleteAsync(endPoint),
                            _ => throw new ArgumentOutOfRangeException(nameof(endPoint), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }
                else
                {
                    HttpContent content = (HttpContent)request.Content.GetContent();

                    responseMessage = RetryRequest(async client =>
                    {
                        return method switch
                        {
                            Method.POST => await client.PostAsync(endPoint, content),
                            Method.GET => throw new ArgumentException(
                                "Get cannot be used in conjunction with an InBody Request"),
                            Method.PUT => await client.PutAsync(endPoint, content),
                            Method.PATCH => await client.PatchAsync(endPoint, content),
                            Method.DELETE => throw new ArgumentException(
                                "Delete cannot be used in conjunction with an InBody Request"),
                            _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }

                return ProcessResponse<TResponse>(responseMessage);
            }
            catch(ArgumentException)
            {
                // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                throw;
            }
            catch(TimeoutException timeoutException)
            {
                return ApiResponse<TResponse>.Failure(Status.None, "The Request Timed Out; Retry limit reached",
                    timeoutException);
            }
            catch(Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    method.ToString(), endPoint);

                return ApiResponse<TResponse>.Failure(Status.None,
                    $"An Exception occured whilst performing a HTTP {method} Request",
                    exception);
            }
            finally
            {
                responseMessage?.Dispose();
            }
        }

        /// <summary>
        /// Retries the request to the specified retry count.
        /// </summary>
        /// <param name="requestAction">The request to be performed.</param>
        /// <param name="route">The route to be inserted into the log.</param>
        /// <returns></returns>
        private HttpResponseMessage RetryRequest(Func<HttpClient, Task<HttpResponseMessage>> requestAction, Uri route)
        {
            bool retryingRequest;
            int requestsAttempted = 0;

            do
            {
                try
                {
                    IClientFactory clientFactory = Dependencies.GetDependency<IClientFactory>();

                    using HttpClient client = clientFactory.BuildClientAsync().Result;

                    // Using Task.Result bubbles the exception up to the caller.
                    // This means the Try Catch inside of RetryRequest does not catch the TaskCanceledException.
                    // The Try Catch in this method IS REQUIRED for the retry to functionality to work.
                    // To get around this, Task.GetAwaiter().GetResult() is necessary.
                    HttpResponseMessage responseMessage = requestAction.Invoke(client).GetAwaiter().GetResult();

                    return responseMessage;
                }
                catch(TaskCanceledException canceledException)
                {
                    requestsAttempted++;

                    if(Connection.RetryAttempts == 0 || requestsAttempted == Connection.RetryAttempts)
                    {
                        _logger?.LogError(canceledException, "The Request to \"{RequestRoute}\" has Timed Out; Retry limit reached. Retired {count}", route, requestsAttempted);

                        retryingRequest = false;
                    }
                    else
                    {
                        _logger?.LogWarning("Request to \"{RequestRoute}\" has Timed out, retrying. Attempts Remaining {count}", route, Connection.RetryAttempts - requestsAttempted);

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
                ISerializer serializer = Dependencies.GetDependency<ISerializer>();

                TResponse response = serializer.Deserialize<TResponse>(new HttpContentResponse(responseMessage.Content));

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
