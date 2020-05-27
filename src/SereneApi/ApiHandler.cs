using DeltaWare.SereneApi.Enums;
using DeltaWare.SereneApi.Interfaces;
using DeltaWare.SereneApi.Types;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeltaWare.SereneApi
{
    /// <summary>
    /// When Inherited; Provides tools and methods required for implementing a RESTful Api consumer.
    /// </summary>
    public abstract class ApiHandler : IDisposable
    {
        #region Variables

        /// <summary>
        /// The <see cref="HttpClient"/> to be used for requests by this <see cref="ApiHandler"/>
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The <see cref="ILogger"/> this <see cref="ApiHandler"/> will use.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The options this <see cref="ApiHandler"/> will use.
        /// </summary>
        private readonly IApiHandlerOptions _options;

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new instance of the ApiHandler
        /// </summary>
        /// <param name="options">The options this ApiHandler will inherit</param>
        protected ApiHandler(IApiHandlerOptions options)
        {
            _options = options;

            _httpClient = _options.HttpClient;

            if (_httpClient == null)
            {
                throw new ArgumentException("No HttpClient was provided");
            }

            // Create our logger and set the category to our HandlerType
            _logger = _options.Logger;
        }

        #endregion
        #region Action Methods

        /// <summary>
        /// Performs an in Path Request. The <see cref="endpoint"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse> InPathRequestAsync(ApiMethod method, object endpoint = null)
        {
            Uri route = GenerateRoute(endpoint);

            return InPathRequestAsync(method, route);
        }

        /// <summary>
        /// Performs an in Path Request. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse> InPathRequestAsync(ApiMethod method, string endpointTemplate, params object[] endpointParameters)
        {
            string endpoint = FormatEndpointTemplate(endpointTemplate, endpointParameters);

            Uri route = GenerateRoute(endpoint);

            return InPathRequestAsync(method, route);
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>. The <see cref="endpoint"/> will be appended to the Url
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(ApiMethod method, object endpoint = null)
        {
            Uri route = GenerateRoute(endpoint);

            return InPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(ApiMethod method, string endpointTemplate, params object[] endpointParameters)
        {
            string endpoint = FormatEndpointTemplate(endpointTemplate, endpointParameters);

            Uri route = GenerateRoute(endpoint);

            return InPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Performs an in Path Request with query support returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TQueryContent">The type to be sent in the query</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be performed</param>
        /// <param name="queryContent"> <see cref="queryContent"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="queryContent"/> to be converted into a query</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQueryContent>(ApiMethod method, TQueryContent queryContent, Expression<Func<TQueryContent, object>> query, object endpoint = null) where TQueryContent : class
        {
            Uri route = GenerateRouteWithQuery(endpoint, queryContent, query);

            return InPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Performs an in Path Request with query support returning a <see cref="TResponse"/>. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TQueryContent">The type to be sent in the query</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="queryContent">The <see cref="queryContent"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="queryContent"/> to be converted into a query</param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQueryContent>(ApiMethod method, TQueryContent queryContent, Expression<Func<TQueryContent, object>> query, string endpointTemplate, params object[] endpointParameters) where TQueryContent : class
        {
            Uri route = GenerateRouteWithQuery(endpointTemplate, queryContent, query, endpointParameters);

            return InPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful <see cref="ApiMethod"/> to be used</param>
        /// <param name="bodyContent">The object serialized and sent in the body of the request</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be appended to the end of the Url</param>
        protected virtual Task<IApiResponse> InBodyRequestAsync<TContent>(ApiMethod method, TContent bodyContent, object endpoint = null)
        {
            Uri route = GenerateRoute(endpoint);

            return InBodyRequestAsync<TContent>(method, route, bodyContent);
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="method"></param>
        /// <param name="bodyContent"></param>
        /// <param name="endpointTemplate"></param>
        /// <param name="endpointParameters"></param>
        protected virtual Task<IApiResponse> InBodyRequestAsync<TContent>(ApiMethod method, TContent bodyContent, string endpointTemplate, params object[] endpointParameters)
        {
            string endpoint = FormatEndpointTemplate(endpointTemplate, endpointParameters);

            Uri route = GenerateRoute(endpoint);

            return InBodyRequestAsync<TContent>(method, route, bodyContent);
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="bodyContent"></param>
        /// <param name="endpoint"></param>
        protected virtual Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(ApiMethod method, TContent bodyContent, object endpoint = null)
        {
            Uri route = GenerateRoute(endpoint);

            return InBodyRequestAsync<TContent, TResponse>(method, route, bodyContent);
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="bodyContent"></param>
        /// <param name="endpointTemplate"></param>
        /// <param name="endpointParameters"></param>
        protected virtual Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(ApiMethod method, TContent bodyContent, string endpointTemplate, params object[] endpointParameters)
        {
            string endpoint = FormatEndpointTemplate(endpointTemplate, endpointParameters);

            Uri route = GenerateRoute(endpoint);

            return InBodyRequestAsync<TContent, TResponse>(method, route, bodyContent);
        }

        #endregion
        #region Base Action Methods

        /// <summary>
        /// Performs an in Path Request
        /// </summary>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="route">The <see cref="Uri"/> to be used for the request</param>
        protected async Task<IApiResponse> InPathRequestAsync(ApiMethod method, Uri route)
        {
            HttpResponseMessage responseMessage = null;

            bool retryingRequest;
            int requestsAttempted = 0;

            do
            {
                requestsAttempted++;

                try
                {
                    responseMessage = method switch
                    {
                        ApiMethod.Post => await _httpClient.PostAsJsonAsync(route),
                        ApiMethod.Get => await _httpClient.GetAsync(route),
                        ApiMethod.Put => await _httpClient.PutAsJsonAsync(route),
                        ApiMethod.Patch => await _httpClient.PatchAsJsonAsync(route),
                        ApiMethod.Delete => await _httpClient.DeleteAsync(route),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method, "An incorrect ApiMethod Value was supplied.")
                    };

                    retryingRequest = false;
                }
                catch (ArgumentException)
                {
                    // An incorrect ApiMethod value was supplied. So we want this exception to bubble up to the caller.
                    throw;
                }
                catch (TaskCanceledException canceledException)
                {
                    if (requestsAttempted == _options.RetryCount)
                    {
                        _logger.LogError(canceledException, "The Request to \"{RequestRoute}\" has Timed Out; Retry limit reached. Retired {count}",
                            route, requestsAttempted);

                        return ApiResponse.Failure("The Request Timed Out; Retry limit reached");
                    }

                    _logger.LogWarning("Request to \"{RequestRoute}\" has Timed out, retrying. Attempts Remaining {count}",
                        route, _options.RetryCount - requestsAttempted);

                    retryingRequest = true;
                }
                catch (Exception exception)
                {

                    _logger?.LogError(exception,
                        "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                        method.ToString(), route);

                    return ApiResponse.Failure($"An Exception occured whilst performing a HTTP {method} Request", exception);
                }
            } while (retryingRequest);

            return ProcessResponse(responseMessage);
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="route">The <see cref="Uri"/> to be used for the request</param>
        protected async Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(ApiMethod method, Uri route)
        {
            HttpResponseMessage responseMessage = null;

            bool retryingRequest;
            int requestsAttempted = 0;

            do
            {
                requestsAttempted++;

                try
                {
                    responseMessage = method switch
                    {
                        ApiMethod.Post => await _httpClient.PostAsJsonAsync(route),
                        ApiMethod.Get => await _httpClient.GetAsync(route),
                        ApiMethod.Put => await _httpClient.PutAsJsonAsync(route),
                        ApiMethod.Patch => await _httpClient.PatchAsJsonAsync(route),
                        ApiMethod.Delete => await _httpClient.DeleteAsync(route),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method, "An incorrect ApiMethod Value was supplied.")
                    };

                    retryingRequest = false;
                }
                catch (ArgumentException)
                {
                    // An incorrect ApiMethod value was supplied. So we want this exception to bubble up to the caller.
                    throw;
                }
                catch (TaskCanceledException canceledException)
                {
                    if (requestsAttempted == _options.RetryCount)
                    {
                        _logger.LogError(canceledException, "The Request to \"{RequestRoute}\" has Timed Out; Retry limit reached. Retired {count}",
                            route, requestsAttempted);

                        return ApiResponse<TResponse>.Failure("The Request Timed Out; Retry limit reached");
                    }

                    _logger.LogWarning("Request to \"{RequestRoute}\" has Timed out, retrying. Attempts Remaining {count}",
                        route, _options.RetryCount - requestsAttempted);

                    retryingRequest = true;
                }
                catch (Exception exception)
                {

                    _logger?.LogError(exception,
                        "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                        method.ToString(), route);

                    return ApiResponse<TResponse>.Failure($"An Exception occured whilst performing a HTTP {method} Request", exception);
                }
            } while (retryingRequest);

            return await ProcessResponseAsync<TResponse>(responseMessage);
        }

        /// <summary>
        /// Performs an in Body Request
        /// </summary>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="route">The <see cref="Uri"/> to be used for the request</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        protected async Task<IApiResponse> InBodyRequestAsync<TContent>(ApiMethod method, Uri route, TContent inBodyContent)
        {
            HttpResponseMessage responseMessage = null;

            bool retryingRequest;
            int requestsAttempted = 0;

            do
            {
                requestsAttempted++;

                try
                {
                    responseMessage = method switch
                    {
                        ApiMethod.Post => await _httpClient.PostAsJsonAsync(route, inBodyContent),
                        ApiMethod.Get => throw new ArgumentException("Get cannot be used in conjunction with an InBody Request"),
                        ApiMethod.Put => await _httpClient.PutAsJsonAsync(route, inBodyContent),
                        ApiMethod.Patch => await _httpClient.PatchAsJsonAsync(route, inBodyContent),
                        ApiMethod.Delete => throw new ArgumentException("Delete cannot be used in conjunction with an InBody Request"),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                            "An incorrect ApiMethod Value was supplied.")
                    };

                    retryingRequest = false;
                }
                catch (ArgumentException)
                {
                    // An incorrect ApiMethod value was supplied. So we want this exception to bubble up to the caller.
                    throw;
                }
                catch (TaskCanceledException canceledException)
                {
                    if (requestsAttempted == _options.RetryCount)
                    {
                        _logger.LogError(canceledException, "The Request to \"{RequestRoute}\" has Timed Out; Retry limit reached. Retired {count}",
                            route, requestsAttempted);

                        return ApiResponse.Failure("The Request Timed Out; Retry limit reached");
                    }

                    _logger.LogWarning("Request to \"{RequestRoute}\" has Timed out, retrying. Attempts Remaining {count}",
                        route, _options.RetryCount - requestsAttempted);

                    retryingRequest = true;
                }
                catch (Exception exception)
                {

                    _logger?.LogError(exception,
                        "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                        method.ToString(), route);

                    return ApiResponse.Failure($"An Exception occured whilst performing a HTTP {method} Request", exception);
                }
            } while (retryingRequest);

            return ProcessResponse(responseMessage);
        }

        /// <summary>
        /// Performs an in Body Request returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="route">The <see cref="Uri"/> to be used for the request</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        protected async Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(ApiMethod method, Uri route, TContent inBodyContent)
        {
            HttpResponseMessage responseMessage = null;

            bool retryingRequest;
            int requestsAttempted = 0;

            do
            {
                requestsAttempted++;

                try
                {
                    responseMessage = method switch
                    {
                        ApiMethod.Post => await _httpClient.PostAsJsonAsync(route, inBodyContent),
                        ApiMethod.Get => throw new ArgumentException("Get cannot be used in conjunction with an InBody Request"),
                        ApiMethod.Put => await _httpClient.PutAsJsonAsync(route, inBodyContent),
                        ApiMethod.Patch => await _httpClient.PatchAsJsonAsync(route, inBodyContent),
                        ApiMethod.Delete => throw new ArgumentException("Delete cannot be used in conjunction with an InBody Request"),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method, "An incorrect ApiMethod Value was supplied.")
                    };

                    retryingRequest = false;
                }
                catch (ArgumentException)
                {
                    // An incorrect ApiMethod value was supplied. So we want this exception to bubble up to the caller.
                    throw;
                }
                catch (TaskCanceledException canceledException)
                {
                    if (requestsAttempted == _options.RetryCount)
                    {
                        _logger.LogError(canceledException, "The Request to \"{RequestRoute}\" has Timed Out; Retry limit reached. Retired {count}",
                            route, requestsAttempted);

                        return ApiResponse<TResponse>.Failure("The Request Timed Out; Retry limit reached");
                    }

                    _logger.LogWarning("Request to \"{RequestRoute}\" has Timed out, retrying. Attempts Remaining {count}",
                        route, _options.RetryCount - requestsAttempted);

                    retryingRequest = true;
                }
                catch (Exception exception)
                {

                    _logger?.LogError(exception,
                        "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                        method.ToString(), route);

                    return ApiResponse<TResponse>.Failure($"An Exception occured whilst performing a HTTP {method} Request", exception);
                }
            } while (retryingRequest);

            return await ProcessResponseAsync<TResponse>(responseMessage);
        }

        #endregion
        #region Helper Methods

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/> deserializing the contained <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized from the response</typeparam>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        protected virtual async Task<IApiResponse<TResponse>> ProcessResponseAsync<TResponse>(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                return ApiResponse<TResponse>.Failure("No Response was returned.");
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                return ApiResponse<TResponse>.Failure(responseMessage.ReasonPhrase);
            }

            try
            {
                await using Stream contentStream = await responseMessage.Content.ReadAsStreamAsync();

                TResponse response = await JsonSerializer.DeserializeAsync<TResponse>(contentStream);

                return ApiResponse<TResponse>.Success(response);
            }
            catch (Exception exception)
            {
                return ApiResponse<TResponse>.Failure("Could not deserialize returned value.", exception);

            }
        }

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        protected virtual IApiResponse ProcessResponse(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                _logger?.LogWarning("Received an Empty Http Response Message");

                return ApiResponse.Failure("Received an Empty Http Response Message");
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                return ApiResponse.Failure(responseMessage.ReasonPhrase);
            }

            return ApiResponse.Success();
        }

        /// <summary>
        /// Generates the Path to be used by the <see cref="HttpClient"/> does not include the <see cref="IApiHandlerOptions.Source"/>
        /// </summary>
        /// <param name="endpoint">The endpoint to be used in the route</param>
        protected virtual Uri GenerateRoute(object endpoint = null)
        {
            Uri route;

            if (endpoint == null)
            {
                route = new Uri("", UriKind.Relative);
            }
            else
            {
                route = new Uri($"/{endpoint}", UriKind.Relative);
            }

            return route;
        }

        /// <summary>
        /// Generates the Path and Query to be used by the <see cref="HttpClient"/> does not include the <see cref="IApiHandlerOptions.Source"/>
        /// </summary>
        /// <typeparam name="TContent">The type to be sent in the query</typeparam>
        /// <param name="endpoint">The endpoint to be used in the route</param>
        /// <param name="content">The <see cref="content"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="content"/> to be converted into a query</param>
        protected virtual Uri GenerateRouteWithQuery<TContent>(object endpoint, TContent content, Expression<Func<TContent, object>> query = null) where TContent : class
        {
            string queryString;

            if (query is null)
            {
                queryString = _options.QueryFactory.Build(content);
            }
            else
            {
                queryString = _options.QueryFactory.Build(content, query);
            }

            Uri route = new Uri($"/{endpoint}{queryString}", UriKind.Relative);

            return route;
        }

        /// <summary>
        /// Generates the Path and Query to be used by the <see cref="HttpClient"/> does not include the <see cref="IApiHandlerOptions.Source"/>
        /// </summary>
        /// <typeparam name="TContent">The type to be sent in the query</typeparam>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="parameters"/></param>
        /// <param name="content">The <see cref="content"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="content"/> to be converted into a query</param>
        /// <param name="parameters">The <see cref="parameters"/> to be appended to the Url</param>
        protected virtual Uri GenerateRouteWithQuery<TContent>(string endpointTemplate, TContent content, Expression<Func<TContent, object>> query = null, params object[] parameters) where TContent : class
        {
            string queryString;

            if (query is null)
            {
                queryString = _options.QueryFactory.Build(content);
            }
            else
            {
                queryString = _options.QueryFactory.Build(content, query);
            }

            string action = FormatEndpointTemplate(endpointTemplate, parameters);

            Uri route = new Uri($"/{action}{queryString}", UriKind.Relative);

            return route;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointTemplate"></param>
        /// <param name="endpointParameters"></param>
        protected virtual string FormatEndpointTemplate(string endpointTemplate, params object[] endpointParameters)
        {
            string endpoint = string.Format(endpointTemplate, endpointParameters);

            if (endpoint.Length != endpointTemplate.Length)
            {
                return $"{endpoint}";
            }

            if (endpointParameters.Length > 1)
            {
                throw new ArgumentException("Multiple Parameters must be used with a format-table endpoint template.");
            }

            return $"{endpointTemplate}/{endpointParameters[0]}";
        }



        #endregion
        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_options is IDisposable disposableOptions)
                {
                    disposableOptions.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
