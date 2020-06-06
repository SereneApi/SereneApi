using Microsoft.Extensions.Logging;
using SereneApi.Interfaces;
using SereneApi.Types;
using SereneApi.Types.Dependencies;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SereneApi
{
    /// <summary>
    /// When Inherited; Provides the methods required for implementing a RESTful Api consumer.
    /// </summary>
    [DebuggerDisplay("Source:{Source}; Timeout:{Timeout}")]
    public abstract class ApiHandler : IDisposable
    {
        #region Variables

        /// <summary>
        /// The <see cref="HttpClient"/> to be used for requests by this <see cref="ApiHandler"/>.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The <see cref="IApiHandlerOptions"/> this <see cref="ApiHandler"/> will use.
        /// </summary>
        private readonly IApiHandlerOptions _options;

        /// <summary>
        /// The <see cref="JsonSerializerOptions"/> that will be used for response deserialization.
        /// </summary>
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// The <see cref="ILogger"/> this <see cref="ApiHandler"/> will use
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="IQueryFactory"/> that will be used for creating queries
        /// </summary>
        private readonly IQueryFactory _queryFactory;

        private readonly RetryDependency _retry;

        #endregion
        #region Properties

        /// <summary>
        /// The <see cref="HttpClient"/> used by the <see cref="ApiHandler"/> for all requests
        /// </summary>
        protected virtual HttpClient Client => _httpClient;

        /// <summary>
        /// <inheritdoc cref="IApiHandlerOptions.Source"/>
        /// </summary>
        public Uri Source => _options.Source;

        /// <summary>
        /// <inheritdoc cref="IApiHandlerOptions.Resource"/>
        /// </summary>
        public string Resource => _options.Resource;

        /// <summary>
        /// How long a request will stay alive before expiring
        /// </summary>
        public TimeSpan Timeout => Client.Timeout;

        /// <summary>
        /// How many times the <see cref="ApiHandler"/> will retry a request after it has timed out
        /// </summary>
        public uint RetryCount => _retry.Count;

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="options">The <see cref="IApiHandlerOptions"/> the <see cref="ApiHandler"/> will use when making requests.</param>
        protected ApiHandler(IApiHandlerOptions options)
        {
            CheckIfDisposed(options);

            _options = options;

            #region Configure Dependencies

            if (!_options.Dependencies.TryGetDependency(out _httpClient))
            {
                throw new ArgumentException("No HttpClient was provided");
            }

            #region Required

            _queryFactory = _options.Dependencies.GetDependency<IQueryFactory>();
            _jsonSerializerOptions = _options.Dependencies.GetDependency<JsonSerializerOptions>();

            #endregion
            #region Optional

            _options.Dependencies.TryGetDependency(out _logger);
            _options.Dependencies.TryGetDependency(out _retry);

            #endregion

            #endregion

            _logger?.LogTrace($"{GetType()} has been instantiated");
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
            CheckIfDisposed();

            Uri route = GenerateRoute(endpoint);

            _logger?.LogTrace("Performing an InPathRequest against {RequestRoute}", route);

            return BaseInPathRequestAsync(method, route);
        }

        /// <summary>
        /// Performs an in Path Request. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse> InPathRequestAsync(Method method, string endpointTemplate, params object[] endpointParameters)
        {
            CheckIfDisposed();

            string endpoint = FormatEndpointTemplate(endpointTemplate, endpointParameters);

            Uri route = GenerateRoute(endpoint);

            _logger?.LogTrace("Performing an InPathRequest against {RequestRoute}", route);

            return BaseInPathRequestAsync(method, route);
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>. The <see cref="endpoint"/> will be appended to the Url
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(Method method, object endpoint = null)
        {
            CheckIfDisposed();

            Uri route = GenerateRoute(endpoint);

            _logger?.LogTrace("Performing an InPathRequest against {RequestRoute}", route);

            return BaseInPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(Method method, string endpointTemplate, params object[] endpointParameters)
        {
            CheckIfDisposed();

            string endpoint = FormatEndpointTemplate(endpointTemplate, endpointParameters);

            Uri route = GenerateRoute(endpoint);

            _logger?.LogTrace("Performing an InPathRequest against {RequestRoute}", route);

            return BaseInPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Performs an in Path Request with query support returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TQueryContent">The type to be sent in the query</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be performed</param>
        /// <param name="queryContent"> <see cref="queryContent"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="queryContent"/> to be converted into a query</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQueryContent>(Method method, TQueryContent queryContent, Expression<Func<TQueryContent, object>> query, object endpoint = null) where TQueryContent : class
        {
            CheckIfDisposed();

            Uri route = GenerateRouteWithQuery(endpoint, queryContent, query);

            _logger?.LogTrace("Performing an InPathRequest against {RequestRoute}", route);

            return BaseInPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Performs an in Path Request with query support returning a <see cref="TResponse"/>. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TQueryContent">The type to be sent in the query</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="queryContent">The <see cref="queryContent"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="queryContent"/> to be converted into a query</param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQueryContent>(Method method, TQueryContent queryContent, Expression<Func<TQueryContent, object>> query, string endpointTemplate, params object[] endpointParameters) where TQueryContent : class
        {
            CheckIfDisposed();

            Uri route = GenerateRouteWithQuery(endpointTemplate, queryContent, query, endpointParameters);

            _logger?.LogTrace("Performing an InPathRequest against {RequestRoute}", route);

            return BaseInPathRequestAsync<TResponse>(method, route);
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
            CheckIfDisposed();

            Uri route = GenerateRoute(endpoint);

            _logger?.LogTrace("Performing an InBodyRequest against {RequestRoute}", route);

            return BaseInBodyRequestAsync<TContent>(method, route, inBodyContent);
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
            CheckIfDisposed();

            string endpoint = FormatEndpointTemplate(endpointTemplate, endpointParameters);

            Uri route = GenerateRoute(endpoint);

            _logger?.LogTrace("Performing an InBodyRequest against {RequestRoute}", route);

            return BaseInBodyRequestAsync<TContent>(method, route, inBodyContent);
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
            CheckIfDisposed();

            Uri route = GenerateRoute(endpoint);

            _logger?.LogTrace("Performing an InBodyRequest against {RequestRoute}", route);

            return BaseInBodyRequestAsync<TContent, TResponse>(method, route, inBodyContent);
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
            CheckIfDisposed();

            string endpoint = FormatEndpointTemplate(endpointTemplate, endpointParameters);

            Uri route = GenerateRoute(endpoint);

            _logger?.LogTrace("Performing an InBodyRequest against {RequestRoute}", route);

            return BaseInBodyRequestAsync<TContent, TResponse>(method, route, inBodyContent);
        }

        #endregion
        #region Base Action Methods

        /// <summary>
        /// Retries the request to the specified retry count.
        /// </summary>
        /// <param name="requestAction">The request to be performed.</param>
        /// <param name="route">The route to be inserted into the log.</param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> RetryRequestAsync(Func<Task<HttpResponseMessage>> requestAction, Uri route)
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

        /// <summary>
        /// Performs an in Path Request
        /// </summary>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="route">The <see cref="Uri"/> to be used for the request</param>
        protected async Task<IApiResponse> BaseInPathRequestAsync(Method method, Uri route)
        {
            CheckIfDisposed();

            HttpResponseMessage responseMessage;

            try
            {
                responseMessage = await RetryRequestAsync(async () =>
                {
                    return method switch
                    {
                        Method.Post => await Client.PostAsJsonAsync(route),
                        Method.Get => await Client.GetAsync(route),
                        Method.Put => await Client.PutAsJsonAsync(route),
                        Method.Patch => await Client.PatchAsJsonAsync(route),
                        Method.Delete => await Client.DeleteAsync(route),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                            "An incorrect Method Value was supplied.")
                    };
                }, route);
            }
            catch (ArgumentOutOfRangeException)
            {
                // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                return ApiResponse.Failure("The Request Timed Out; Retry limit reached", timeoutException);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    method.ToString(), route);

                return ApiResponse.Failure($"An Exception occured whilst performing a HTTP {method} Request",
                    exception);
            }

            return ProcessResponse(responseMessage);
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="route">The <see cref="Uri"/> to be used for the request</param>
        protected async Task<IApiResponse<TResponse>> BaseInPathRequestAsync<TResponse>(Method method, Uri route)
        {
            CheckIfDisposed();

            HttpResponseMessage responseMessage;

            try
            {
                responseMessage = await RetryRequestAsync(async () =>
                {
                    return method switch
                    {
                        Method.Post => await Client.PostAsJsonAsync(route),
                        Method.Get => await Client.GetAsync(route),
                        Method.Put => await Client.PutAsJsonAsync(route),
                        Method.Patch => await Client.PatchAsJsonAsync(route),
                        Method.Delete => await Client.DeleteAsync(route),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                            "An incorrect Method Value was supplied.")
                    };
                }, route);
            }
            catch (ArgumentOutOfRangeException)
            {
                // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                return ApiResponse<TResponse>.Failure("The Request Timed Out; Retry limit reached", timeoutException);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    method.ToString(), route);

                return ApiResponse<TResponse>.Failure($"An Exception occured whilst performing a HTTP {method} Request",
                    exception);
            }

            return await ProcessResponseAsync<TResponse>(responseMessage);
        }

        /// <summary>
        /// Performs an in Body Request
        /// </summary>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="route">The <see cref="Uri"/> to be used for the request</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        protected async Task<IApiResponse> BaseInBodyRequestAsync<TContent>(Method method, Uri route,
            TContent inBodyContent)
        {
            CheckIfDisposed();

            HttpResponseMessage responseMessage;

            try
            {
                responseMessage = await RetryRequestAsync(async () =>
                {
                    return method switch
                    {
                        Method.Post => await Client.PostAsJsonAsync(route, inBodyContent),
                        Method.Get => throw new ArgumentException(
                            "Get cannot be used in conjunction with an InBody Request"),
                        Method.Put => await Client.PutAsJsonAsync(route, inBodyContent),
                        Method.Patch => await Client.PatchAsJsonAsync(route, inBodyContent),
                        Method.Delete => throw new ArgumentException(
                            "Delete cannot be used in conjunction with an InBody Request"),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                            "An incorrect Method Value was supplied.")
                    };
                }, route);
            }
            catch (ArgumentException)
            {
                // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                return ApiResponse.Failure("The Request Timed Out; Retry limit reached", timeoutException);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    method.ToString(), route);

                return ApiResponse.Failure($"An Exception occured whilst performing a HTTP {method} Request",
                    exception);
            }

            return ProcessResponse(responseMessage);
        }

        /// <summary>
        /// Performs an in Body Request returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="route">The <see cref="Uri"/> to be used for the request</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        protected async Task<IApiResponse<TResponse>> BaseInBodyRequestAsync<TContent, TResponse>(Method method,
            Uri route, TContent inBodyContent)
        {
            CheckIfDisposed();

            HttpResponseMessage responseMessage;

            try
            {
                responseMessage = await RetryRequestAsync(async () =>
                {
                    return method switch
                    {
                        Method.Post => await Client.PostAsJsonAsync(route, inBodyContent),
                        Method.Get => throw new ArgumentException(
                            "Get cannot be used in conjunction with an InBody Request"),
                        Method.Put => await Client.PutAsJsonAsync(route, inBodyContent),
                        Method.Patch => await Client.PatchAsJsonAsync(route, inBodyContent),
                        Method.Delete => throw new ArgumentException(
                            "Delete cannot be used in conjunction with an InBody Request"),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method,
                            "An incorrect Method Value was supplied.")
                    };
                }, route);
            }
            catch (ArgumentException)
            {
                // An incorrect Method value was supplied. So we want this exception to bubble up to the caller.
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                return ApiResponse<TResponse>.Failure("The Request Timed Out; Retry limit reached", timeoutException);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception,
                    "An Exception occured whilst performing a HTTP {httpMethod} Request to \"{RequestRoute}\"",
                    method.ToString(), route);

                return ApiResponse<TResponse>.Failure($"An Exception occured whilst performing a HTTP {method} Request",
                    exception);
            }

            return await ProcessResponseAsync<TResponse>(responseMessage);
        }

        #endregion
        #region Response Processing

        /// <summary>
        /// Deserializes the response content into the <see cref="TContent"/>.
        /// </summary>
        /// <typeparam name="TContent">The <see cref="Type"/> contained in the response content.</typeparam>
        /// <param name="content">The content to be deserializes.</param>
        protected virtual async Task<TContent> DeserializeContentAsync<TContent>(HttpContent content)
        {
            await using Stream contentStream = await content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<TContent>(contentStream, _jsonSerializerOptions);
        }

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        protected virtual IApiResponse ProcessResponse(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                _logger?.LogWarning("Received an Empty Http Response");

                return ApiResponse.Failure("Received an Empty Http Response");
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                return ApiResponse.Failure(responseMessage.ReasonPhrase);
            }

            return ApiResponse.Success();
        }

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

                return ApiResponse<TResponse>.Failure("Received an Empty Http Response");
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                return ApiResponse<TResponse>.Failure(responseMessage.ReasonPhrase);
            }

            try
            {
                TResponse response = await DeserializeContentAsync<TResponse>(responseMessage.Content);

                return ApiResponse<TResponse>.Success(response);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "Could not deserialize the returned value");

                return ApiResponse<TResponse>.Failure("Could not deserialize returned value.", exception);
            }
        }

        #endregion
        #region Route Generation

        /// <summary>
        /// Generates the Path to be used by the <see cref="HttpClient"/> does not include the <see cref="IApiHandlerOptions.Source"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint to be used in the route.</param>
        protected virtual Uri GenerateRoute(object endpoint = null)
        {
            Uri route;

            if (endpoint == null)
            {
                route = new Uri(string.Empty, UriKind.Relative);
            }
            else
            {
                route = new Uri($"{endpoint}", UriKind.Relative);
            }

            return route;
        }

        /// <summary>
        /// Generates the Query String to be in the Request.
        /// </summary>
        /// <typeparam name="TContent">The type to be sent in the query.</typeparam>
        /// <param name="content">>The <see cref="content"/> to be used when generating the <see cref="query"/>.</param>
        /// <param name="query">Selects parts of the <see cref="content"/> to be converted into a query.</param>
        /// <returns></returns>
        protected virtual string GenerateQuery<TContent>(TContent content, Expression<Func<TContent, object>> query = null)
        {
            string queryString;

            // If the query is null, the entire TContent object will be used in the query generation.
            if (query is null)
            {
                queryString = _queryFactory.Build(content);
            }
            else
            {
                queryString = _queryFactory.Build(content, query);
            }

            return queryString;
        }

        /// <summary>
        /// Generates the Path and Query to be used by the <see cref="HttpClient"/> does not include the <see cref="IApiHandlerOptions.Source"/>.
        /// </summary>
        /// <typeparam name="TContent">The type to be sent in the query.</typeparam>
        /// <param name="endpoint">The endpoint to be used in the route.</param>
        /// <param name="content">The <see cref="content"/> to be used when generating the <see cref="query"/>.</param>
        /// <param name="query">Selects parts of the <see cref="content"/> to be converted into a query.</param>
        protected virtual Uri GenerateRouteWithQuery<TContent>(object endpoint, TContent content, Expression<Func<TContent, object>> query = null) where TContent : class
        {
            string queryString = GenerateQuery(content, query);

            Uri route = new Uri($"{endpoint}{queryString}", UriKind.Relative);

            return route;
        }

        /// <summary>
        /// Generates the Path and Query to be used by the <see cref="HttpClient"/> does not include the <see cref="IApiHandlerOptions.Source"/>.
        /// </summary>
        /// <typeparam name="TContent">The type to be sent in the query.</typeparam>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with parameters.</param>
        /// <param name="content">The <see cref="content"/> to be used when generating the <see cref="query"/>.</param>
        /// <param name="query">Selects parts of the <see cref="content"/> to be converted into a query.</param>
        /// <param name="endpointParameters">The parameters to be appended to the Url.</param>
        protected virtual Uri GenerateRouteWithQuery<TContent>(string endpointTemplate, TContent content, Expression<Func<TContent, object>> query = null, params object[] endpointParameters) where TContent : class
        {
            string action = FormatEndpointTemplate(endpointTemplate, endpointParameters);

            string queryString = GenerateQuery(content, query);

            Uri route = new Uri($"{action}{queryString}", UriKind.Relative);

            return route;
        }

        /// <summary>
        /// Formats the endpoint template.
        /// </summary>
        /// <param name="template">The template used for formatting.</param>
        /// <param name="parameters">The parameters to be appended to the template.</param>
        protected virtual string FormatEndpointTemplate(string template, params object[] parameters)
        {
            string endpoint = string.Format(template, parameters);

            // If the length is different the endpoint has been formatted correctly.
            if (endpoint.Length != template.Length)
            {
                return $"{endpoint}";
            }

            // If we have more than 1 parameter here it means the formatting was unsuccessful.
            if (parameters.Length > 1)
            {
                throw new ArgumentException("Multiple Parameters must be used with a format-table endpoint template.");
            }

            // Return an endpoint without formatting the template and appending the only parameter to the end.
            return $"{template}/{parameters[0]}";
        }

        #endregion
        #region IDisposable

        private bool _disposed;

        private void CheckIfDisposed(IApiHandlerOptions options)
        {
            if (options is ApiHandlerOptions apiHandlerOptions && apiHandlerOptions.IsDisposed())
            {
                throw new ObjectDisposedException(nameof(apiHandlerOptions.GetType));
            }
        }

        /// <summary>
        /// Throws an Object Disposed Exception if the <see cref="ApiHandler"/> has been disposed.
        /// </summary>
        protected void CheckIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(GetType));
            }
        }

        /// <summary>
        /// Disposes the current instance of the <see cref="ApiHandler"/>.
        /// </summary>
        public void Dispose()
        {
            _logger?.LogTrace($"{GetType()} is being disposed");

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
