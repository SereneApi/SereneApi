using Microsoft.Extensions.Logging;
using SereneApi.Abstraction.Enums;
using SereneApi.Extensions;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi
{
    // Async Implementation
    public abstract partial class ApiHandler
    {
        #region Perform Methods

        protected Task<IApiResponse> PerformRequestAsync(Method method, Expression<Func<IRequest, IRequestCreated>> request = null)
        {
            CheckIfDisposed();

            RequestBuilder requestBuilder = new RequestBuilder(_routeFactory, _queryFactory, _serializer, Resource);

            requestBuilder.UsingMethod(method);

            request?.Compile().Invoke(requestBuilder);

            return PerformRequestBaseAsync(requestBuilder.GetRequest());
        }

        protected Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> request = null)
        {
            CheckIfDisposed();

            RequestBuilder requestBuilder = new RequestBuilder(_routeFactory, _queryFactory, _serializer, Resource);

            requestBuilder.UsingMethod(method);

            request?.Compile().Invoke(requestBuilder);

            return PerformRequestBaseAsync<TResponse>(requestBuilder.GetRequest());
        }

        #endregion
        #region Action Methods

        /// <summary>
        /// Performs an in Path Request. The <see cref="endpoint"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse> InPathRequestAsync(Method method, object endpoint = null)
        {
            return PerformRequestAsync(method, request => request
                .WithEndPoint(endpoint));
        }

        /// <summary>
        /// Performs an in Path Request. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse> InPathRequestAsync(Method method, string endpointTemplate, params object[] endpointParameters)
        {
            return PerformRequestAsync(method, request => request
                .WithEndPoint(endpointTemplate, endpointParameters));
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>. The <see cref="endpoint"/> will be appended to the Url
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(Method method, object endpoint = null)
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpoint));
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(Method method, string endpointTemplate, params object[] endpointParameters)
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpointTemplate, endpointParameters));
        }

        /// <summary>
        /// Performs an in Path Request with query support returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TQuery">The type to be sent in the query</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be performed</param>
        /// <param name="queryContent"> <see cref="queryContent"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="queryContent"/> to be converted into a query</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQuery>(Method method, TQuery queryContent, Expression<Func<TQuery, object>> query, object endpoint = null) where TQuery : class
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpoint)
                .WithQuery(queryContent, query));
        }

        /// <summary>
        /// Performs an in Path Request with query support returning a <see cref="TResponse"/>. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TQuery">The type to be sent in the query</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="queryContent">The <see cref="queryContent"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="queryContent"/> to be converted into a query</param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQuery>(Method method, TQuery queryContent, Expression<Func<TQuery, object>> query, string endpointTemplate, params object[] endpointParameters) where TQuery : class
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpointTemplate, endpointParameters)
                .WithQuery(queryContent, query));
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful <see cref="Method"/> to be used</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be appended to the end of the Url</param>
        protected virtual Task<IApiResponse> InBodyRequestAsync<TContent>(Method method, TContent inBodyContent, object endpoint = null)
        {
            return PerformRequestAsync(method, request => request
                .WithEndPoint(endpoint)
                .WithInBodyContent(inBodyContent));
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="method"></param>
        /// <param name="inBodyContent"></param>
        /// <param name="endpointTemplate"></param>
        /// <param name="endpointParameters"></param>
        protected virtual Task<IApiResponse> InBodyRequestAsync<TContent>(Method method, TContent inBodyContent, string endpointTemplate, params object[] endpointParameters)
        {
            return PerformRequestAsync(method, request => request
                .WithEndPoint(endpointTemplate, endpointParameters)
                .WithInBodyContent(inBodyContent));
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="inBodyContent"></param>
        /// <param name="endpoint"></param>
        protected virtual Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(Method method, TContent inBodyContent, object endpoint = null)
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpoint)
                .WithInBodyContent(inBodyContent));
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="inBodyContent"></param>
        /// <param name="endpointTemplate"></param>
        /// <param name="endpointParameters"></param>
        protected virtual Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(Method method, TContent inBodyContent, string endpointTemplate, params object[] endpointParameters)
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpointTemplate, endpointParameters)
                .WithInBodyContent(inBodyContent));
        }

        #endregion
        #region Base Action Methods

        protected virtual async Task<IApiResponse> PerformRequestBaseAsync(IApiRequest request)
        {
            HttpResponseMessage responseMessage;

            Uri endPoint = request.EndPoint;

            Method method = request.Method;

            try
            {
                if (request.Content == null)
                {
                    responseMessage = await RetryRequestAsync(async () =>
                    {
                        return method switch
                        {
                            Method.Post => await Client.PostAsJsonAsync(endPoint),
                            Method.Get => await Client.GetAsync(endPoint),
                            Method.Put => await Client.PutAsJsonAsync(endPoint),
                            Method.Patch => await Client.PatchAsJsonAsync(endPoint),
                            Method.Delete => await Client.DeleteAsync(endPoint),
                            _ => throw new ArgumentOutOfRangeException(nameof(endPoint), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }
                else
                {
                    StringContent content = request.Content.ToStringContent();

                    responseMessage = await RetryRequestAsync(async () =>
                    {
                        return method switch
                        {
                            Method.Post => await Client.PostAsJsonAsync(endPoint, content),
                            Method.Get => throw new ArgumentException(
                                "Get cannot be used in conjunction with an InBody Request"),
                            Method.Put => await Client.PutAsJsonAsync(endPoint, content),
                            Method.Patch => await Client.PatchAsJsonAsync(endPoint, content),
                            Method.Delete => throw new ArgumentException(
                                "Delete cannot be used in conjunction with an InBody Request"),
                            _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }
            }
            catch (ArgumentException)
            {
                // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                return ApiResponse.Failure(Status.None, "The Request Timed Out; Retry limit reached", timeoutException);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    method.ToString(), endPoint);

                return ApiResponse.Failure(Status.None, $"An Exception occured whilst performing a HTTP {method} Request",
                    exception);
            }

            return ProcessResponse(responseMessage);
        }

        protected virtual async Task<IApiResponse<TResponse>> PerformRequestBaseAsync<TResponse>(IApiRequest request)
        {
            HttpResponseMessage responseMessage;

            Uri endPoint = request.EndPoint;

            Method method = request.Method;

            try
            {
                if (request.Content == null)
                {
                    responseMessage = await RetryRequestAsync(async () =>
                    {
                        return method switch
                        {
                            Method.Post => await Client.PostAsJsonAsync(endPoint),
                            Method.Get => await Client.GetAsync(endPoint),
                            Method.Put => await Client.PutAsJsonAsync(endPoint),
                            Method.Patch => await Client.PatchAsJsonAsync(endPoint),
                            Method.Delete => await Client.DeleteAsync(endPoint),
                            _ => throw new ArgumentOutOfRangeException(nameof(endPoint), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }
                else
                {
                    StringContent content = request.Content.ToStringContent();

                    responseMessage = await RetryRequestAsync(async () =>
                    {
                        return method switch
                        {
                            Method.Post => await Client.PostAsJsonAsync(endPoint, content),
                            Method.Get => throw new ArgumentException(
                                "Get cannot be used in conjunction with an InBody Request"),
                            Method.Put => await Client.PutAsJsonAsync(endPoint, content),
                            Method.Patch => await Client.PatchAsJsonAsync(endPoint, content),
                            Method.Delete => throw new ArgumentException(
                                "Delete cannot be used in conjunction with an InBody Request"),
                            _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                                "An incorrect Method Value was supplied.")
                        };
                    }, endPoint);
                }
            }
            catch (ArgumentException)
            {
                // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                return ApiResponse<TResponse>.Failure(Status.None, "The Request Timed Out; Retry limit reached", timeoutException);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    method.ToString(), endPoint);

                return ApiResponse<TResponse>.Failure(Status.None, $"An Exception occured whilst performing a HTTP {method} Request",
                    exception);
            }

            return await ProcessResponseAsync<TResponse>(responseMessage);
        }

        /// <summary>
        /// Retries the request to the specified retry count.
        /// </summary>
        /// <param name="requestAction">The request to be performed.</param>
        /// <param name="route">The route to be inserted into the log.</param>
        /// <returns></returns>
        protected virtual async Task<HttpResponseMessage> RetryRequestAsync(Func<Task<HttpResponseMessage>> requestAction, Uri route)
        {
            bool retryingRequest;
            int requestsAttempted = 0;

            do
            {

                try
                {
                    HttpResponseMessage responseMessage = await requestAction.Invoke();

                    return responseMessage;
                }
                catch (TaskCanceledException canceledException)
                {
                    requestsAttempted++;

                    if (_retry.Count == 0 || requestsAttempted == _retry.Count)
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
            } while (retryingRequest);

            throw new TimeoutException($"The Request to \"{route}\" has Timed Out; Retry limit reached. Retired {requestsAttempted}");
        }

        #endregion

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/> deserializing the contained <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized from the response</typeparam>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        protected virtual async Task<IApiResponse<TResponse>> ProcessResponseAsync<TResponse>(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                _logger?.LogWarning("Received an Empty Http Response");

                return ApiResponse<TResponse>.Failure(Status.None, "Received an Empty Http Response");
            }

            Status status = responseMessage.StatusCode.ToStatus();

            if (!status.IsSuccessCode())
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                return ApiResponse<TResponse>.Failure(status, responseMessage.ReasonPhrase);
            }

            try
            {
                if (responseMessage.Content == null)
                {
                    _logger.LogWarning("No content was received in the response.");

                    return ApiResponse<TResponse>.Failure(status, "No content was received in the response.");
                }

                TResponse response = await _serializer.DeserializeAsync<TResponse>(responseMessage.Content);

                return ApiResponse<TResponse>.Success(status, response);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "Could not deserialize the returned value");

                return ApiResponse<TResponse>.Failure(status, "Could not deserialize returned value.", exception);
            }
        }
    }
}
