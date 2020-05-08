using DeltaWare.SereneApi.Enums;
using DeltaWare.SereneApi.Interfaces;
using DeltaWare.SereneApi.Types;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace DeltaWare.SereneApi
{
    /// <summary>
    /// When Inherited; Provides tools and methods required for implementing a RESTful Api consumer.
    /// </summary>
    public abstract class ApiHandler
    {
        #region Variables

        /// <summary>
        /// The <see cref="ILogger"/> this <see cref="ApiHandler"/> will use.
        /// NOTE: This is created using the Logger Factory that is provided in the <see cref="ApiHandlerOptions"/> during construction
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

            // Create our logger and set the category to our HandlerType
            _logger = _options.LoggerFactory?.CreateLogger(_options.HandlerType.ToString());
        }

        #endregion
        #region Action Methods

        /// <summary>
        /// Performs an in Path Request. The <see cref="parameter"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="parameter">The <see cref="parameter"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse> InPathRequestAsync(ApiMethod method, object parameter = null)
        {
            Uri route = GenerateRoute(parameter);

            return InPathRequestAsync(method, route);
        }

        /// <summary>
        /// Performs an in Path Request. The <see cref="parameters"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="actionTemplate">The action to be performed, supports templates for string formatting with <see cref="parameters"/></param>
        /// <param name="parameters">The <see cref="parameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse> InPathRequestAsync(ApiMethod method, string actionTemplate, params object[] parameters)
        {
            string action = FormatActionTemplate(actionTemplate, parameters);

            Uri route = GenerateRoute(action);

            return InPathRequestAsync(method, route);
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>. The <see cref="parameter"/> will be appended to the Url
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="parameter">The <see cref="parameter"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(ApiMethod method, object parameter = null)
        {
            Uri route = GenerateRoute(parameter);

            return InPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>. The <see cref="parameters"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="actionTemplate">The action to be performed, supports templates for string formatting with <see cref="parameters"/></param>
        /// <param name="parameters">The <see cref="parameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(ApiMethod method, string actionTemplate, params object[] parameters)
        {
            string action = FormatActionTemplate(actionTemplate, parameters);

            Uri route = GenerateRoute(action);

            return InPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Performs an in Path Request with query support returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TContent">The type to be sent in the query</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="action">The <see cref="action"/> to be performed</param>
        /// <param name="content">The <see cref="content"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="content"/> to be converted into a query</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TContent>(ApiMethod method, object action, TContent content, Expression<Func<TContent, object>> query = null) where TContent : class
        {
            Uri route = GenerateRouteWithQuery(action, content, query);

            return InPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Performs an in Path Request with query support returning a <see cref="TResponse"/>. The <see cref="parameters"/> will be appended to the Url
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TContent">The type to be sent in the query</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="actionTemplate">The action to be performed, supports templates for string formatting with <see cref="parameters"/></param>
        /// <param name="content">The <see cref="content"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="content"/> to be converted into a query</param>
        /// <param name="parameters">The <see cref="parameters"/> to be appended to the Url</param>
        protected virtual Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TContent>(ApiMethod method, string actionTemplate, TContent content, Expression<Func<TContent, object>> query = null, params object[] parameters) where TContent : class
        {
            Uri route = GenerateRouteWithQuery(actionTemplate, content, query, parameters);

            return InPathRequestAsync<TResponse>(method, route);
        }

        /// <summary>
        /// Performs an in Body Request
        /// </summary>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        /// <returns></returns>
        protected virtual Task<IApiResponse> InBodyRequestAsync<TContent>(ApiMethod method, TContent inBodyContent)
        {
            Uri route = GenerateRoute();

            return InBodyRequestAsync<TContent>(method, route, inBodyContent);
        }

        /// <summary>
        /// Performs an in Body Request
        /// </summary>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The type to be serialized and sent in the body of the request</param>
        /// <param name="action">The <see cref="action"/> to be performed</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        /// <returns></returns>
        protected virtual Task<IApiResponse> InBodyRequestAsync<TContent>(ApiMethod method, object action, TContent inBodyContent)
        {
            Uri route = GenerateRoute(action);

            return InBodyRequestAsync<TContent>(method, route, inBodyContent);
        }

        /// <summary>
        /// Performs an in Body Request returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        /// <returns></returns>
        protected virtual Task<IApiResponse<TResponse>> InBodyRequestAsync<TResponse, TContent>(ApiMethod method, TContent inBodyContent)
        {
            Uri route = GenerateRoute();

            return InBodyRequestAsync<TResponse, TContent>(method, route, inBodyContent);
        }

        /// <summary>
        /// Performs an in Body Request returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="action">The <see cref="action"/> to be performed</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        /// <returns></returns>
        protected virtual Task<IApiResponse<TResponse>> InBodyRequestAsync<TResponse, TContent>(ApiMethod method, object action, TContent inBodyContent)
        {
            Uri route = GenerateRoute(action);

            return InBodyRequestAsync<TResponse, TContent>(method, route, inBodyContent);
        }

        #endregion
        #region Helper Methods

        /// <summary>
        /// Creates a new Instance of a <see cref="HttpClient"/>
        /// </summary>
        protected virtual Task<HttpClient> CreateHttpClientAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                if (_options.ClientOverride != null)
                {
                    _logger?.LogDebug("Using Client Override for Request");

                    return _options.ClientOverride;
                }

                HttpClient client;

                if (_options.HttpClientFactory != null)
                {
                    _logger?.LogDebug("An HttpClientFactory has been provided, creating Client using Factory");

                    client = _options.HttpClientFactory.CreateClient();
                }
                else
                {
                    _logger?.LogDebug("Using default HttpClient as no client factory or override was provided");

                    client = new HttpClient();
                }

                client.BaseAddress = _options.Source;
                client.Timeout = _options.Timeout;

                _options.RequestHeaderBuilder.Invoke(client.DefaultRequestHeaders);

                return client;
            });
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
                return ApiResponse<TResponse>.Failure("No Response was returned.");
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                return ApiResponse<TResponse>.Failure(responseMessage.ReasonPhrase);
            }

            try
            {
                TResponse response = await responseMessage.ReadAsJsonAsync<TResponse>();

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
        /// <param name="action">The action to be used in the route</param>
        protected virtual Uri GenerateRoute(object action = null)
        {
            Uri route;

            if (action == null)
            {
                route = new Uri($"{_options.ResourcePrecursor}{_options.Resource}", UriKind.Relative);
            }
            else
            {
                route = new Uri($"{_options.ResourcePrecursor}{_options.Resource}/{action}", UriKind.Relative);
            }

            return route;
        }

        /// <summary>
        /// Generates the Path and Query to be used by the <see cref="HttpClient"/> does not include the <see cref="IApiHandlerOptions.Source"/>
        /// </summary>
        /// <typeparam name="TContent">The type to be sent in the query</typeparam>
        /// <param name="action">The action to be used in the route</param>
        /// <param name="content">The <see cref="content"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="content"/> to be converted into a query</param>
        protected virtual Uri GenerateRouteWithQuery<TContent>(object action, TContent content, Expression<Func<TContent, object>> query = null) where TContent : class
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

            Uri route = new Uri($"{_options.ResourcePrecursor}{_options.Resource}/{action}{queryString}", UriKind.Relative);

            return route;
        }

        /// <summary>
        /// Generates the Path and Query to be used by the <see cref="HttpClient"/> does not include the <see cref="IApiHandlerOptions.Source"/>
        /// </summary>
        /// <typeparam name="TContent">The type to be sent in the query</typeparam>
        /// <param name="actionTemplate">The action to be performed, supports templates for string formatting with <see cref="parameters"/></param>
        /// <param name="content">The <see cref="content"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="content"/> to be converted into a query</param>
        /// <param name="parameters">The <see cref="parameters"/> to be appended to the Url</param>
        protected virtual Uri GenerateRouteWithQuery<TContent>(string actionTemplate, TContent content, Expression<Func<TContent, object>> query = null, params object[] parameters) where TContent : class
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

            string action = FormatActionTemplate(actionTemplate, parameters);

            Uri route = new Uri($"{_options.ResourcePrecursor}{_options.Resource}/{action}{queryString}", UriKind.Relative);

            return route;
        }

        /// <summary>
        /// Formats the Action Template
        /// </summary>
        /// <param name="actionTemplate">The action to be performed, supports templates for string formatting with <see cref="parameters"/></param>
        /// <param name="parameters">The <see cref="parameters"/> to be appended to the Url</param>
        /// <returns></returns>
        protected virtual string FormatActionTemplate(string actionTemplate, params object[] parameters)
        {
            string action = string.Format(actionTemplate, parameters);

            if (action.Length != actionTemplate.Length)
            {
                return $"{action}";
            }

            if (parameters.Length > 1)
            {
                throw new ArgumentException("Multiple Parameters must be used with a formattable action template.");
            }

            return $"{actionTemplate}/{parameters[0]}";
        }



        #endregion
        #region Private Action Methods

        /// <summary>
        /// Performs an in Path Request
        /// </summary>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="route">The <see cref="route"/> to be used for the request</param>
        private async Task<IApiResponse> InPathRequestAsync(ApiMethod method, Uri route)
        {
            using HttpClient client = await CreateHttpClientAsync();

            if (client is null)
            {
                _logger?.LogWarning("Could not Create an instance of the HttpClient.");

                return ApiResponse.Failure("Could not Create an instance of the HttpClient");
            }

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
                        ApiMethod.Post => await client.PostAsJsonAsync(route),
                        ApiMethod.Get => await client.GetAsync(route),
                        ApiMethod.Put => await client.PutAsJsonAsync(route),
                        ApiMethod.Patch => await client.PatchAsJsonAsync(route),
                        ApiMethod.Delete => await client.DeleteAsync(route),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
                    };

                    retryingRequest = false;
                }
                catch (TaskCanceledException canceledException)
                {
                    if (requestsAttempted == _options.RetryCount)
                    {
                        _logger.LogError(canceledException, "Request Timed Out; Retry limit reached. Retired {count}", requestsAttempted);

                        return ApiResponse.Failure("Request Timed Out; Retry limit reached");
                    }

                    _logger.LogWarning("Request Timed out, retrying. Attempts Remaining {count}", _options.RetryCount - requestsAttempted);

                    retryingRequest = true;
                }
                catch (Exception exception)
                {

                    _logger?.LogError(exception,
                        "An Exception occured whilst performing a HTTP {httpMethod} Request on thr Uri \"{uri}\"",
                        method.ToString(), route);

                    return ApiResponse.Failure("");
                }
            } while (retryingRequest);

            return ProcessResponse(responseMessage);
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="route">The <see cref="route"/> to be used for the request</param>
        private async Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(ApiMethod method, Uri route)
        {
            using HttpClient client = await CreateHttpClientAsync();

            if (client is null)
            {
                _logger?.LogWarning("Could not Create an instance of the HttpClient.");

                return ApiResponse<TResponse>.Failure("Could not Create an instance of the HttpClient");
            }

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
                        ApiMethod.Post => await client.PostAsJsonAsync(route),
                        ApiMethod.Get => await client.GetAsync(route),
                        ApiMethod.Put => await client.PutAsJsonAsync(route),
                        ApiMethod.Patch => await client.PatchAsJsonAsync(route),
                        ApiMethod.Delete => await client.DeleteAsync(route),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
                    };

                    retryingRequest = false;
                }
                catch (TaskCanceledException canceledException)
                {
                    if (requestsAttempted == _options.RetryCount)
                    {
                        _logger.LogError(canceledException, "Request Timed Out; Retry limit reached. Retired {count}",
                            requestsAttempted);

                        return ApiResponse<TResponse>.Failure("Request Timed Out; Retry limit reached");
                    }

                    _logger.LogWarning("Request Timed out, retrying. Attempts Remaining {count}",
                        _options.RetryCount - requestsAttempted);

                    retryingRequest = true;
                }
                catch (Exception exception)
                {

                    _logger?.LogError(exception,
                        "An Exception occured whilst performing a HTTP {httpMethod} Request on thr Uri \"{uri}\"",
                        method.ToString(), route);

                    return ApiResponse<TResponse>.Failure("");
                }
            } while (retryingRequest);

            return await ProcessResponseAsync<TResponse>(responseMessage);
        }

        /// <summary>
        /// Performs an in Body Request returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="route">The <see cref="route"/> to be used for the request</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        /// <returns></returns>
        private async Task<IApiResponse> InBodyRequestAsync<TContent>(ApiMethod method, Uri route, TContent inBodyContent)
        {
            using HttpClient client = await CreateHttpClientAsync();

            if (client is null)
            {
                _logger?.LogWarning("Could not Create an instance of the HttpClient.");

                return ApiResponse.Failure("Could not Create an instance of the HttpClient");
            }

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
                        ApiMethod.Post => await client.PostAsJsonAsync(route, inBodyContent),
                        ApiMethod.Get => throw new ArgumentException("Get cannot be used with InBody Action"),
                        ApiMethod.Put => await client.PutAsJsonAsync(route, inBodyContent),
                        ApiMethod.Patch => await client.PatchAsJsonAsync(route, inBodyContent),
                        ApiMethod.Delete => throw new ArgumentException("Delete cannot be used with InBody Action"),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
                    };

                    retryingRequest = false;
                }
                catch (TaskCanceledException canceledException)
                {
                    if (requestsAttempted == _options.RetryCount)
                    {
                        _logger.LogError(canceledException, "Request Timed Out; Retry limit reached. Retired {count}",
                            requestsAttempted);

                        return ApiResponse.Failure("Request Timed Out; Retry limit reached");
                    }

                    _logger.LogWarning("Request Timed out, retrying. Attempts Remaining {count}",
                        _options.RetryCount - requestsAttempted);

                    retryingRequest = true;
                }
                catch (Exception exception)
                {

                    _logger?.LogError(exception,
                        "An Exception occured whilst performing a HTTP {httpMethod} Request on thr Uri \"{uri}\"",
                        method.ToString(), route);

                    return ApiResponse.Failure("");
                }
            } while (retryingRequest);

            return ProcessResponse(responseMessage);
        }

        /// <summary>
        /// Performs an in Body Request
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful API <see cref="ApiMethod"/> to be used</param>
        /// <param name="route">The <see cref="route"/> to be used for the request</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        /// <returns></returns>
        private async Task<IApiResponse<TResponse>> InBodyRequestAsync<TResponse, TContent>(ApiMethod method, Uri route, TContent inBodyContent)
        {
            using HttpClient client = await CreateHttpClientAsync();

            if (client is null)
            {
                _logger?.LogWarning("Could not Create an instance of the HttpClient.");

                return ApiResponse<TResponse>.Failure("Could not Create an instance of the HttpClient");
            }

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
                        ApiMethod.Post => await client.PostAsJsonAsync(route, inBodyContent),
                        ApiMethod.Get => throw new ArgumentException("Get cannot be used with InBody Action"),
                        ApiMethod.Put => await client.PutAsJsonAsync(route, inBodyContent),
                        ApiMethod.Patch => await client.PatchAsJsonAsync(route, inBodyContent),
                        ApiMethod.Delete => throw new ArgumentException("Delete cannot be used with InBody Action"),
                        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
                    };

                    retryingRequest = false;
                }
                catch (TaskCanceledException canceledException)
                {
                    if (requestsAttempted == _options.RetryCount)
                    {
                        _logger.LogError(canceledException, "Request Timed Out; Retry limit reached. Retired {count}",
                            requestsAttempted);

                        return ApiResponse<TResponse>.Failure("Request Timed Out; Retry limit reached");
                    }

                    _logger.LogWarning("Request Timed out, retrying. Attempts Remaining {count}",
                        _options.RetryCount - requestsAttempted);

                    retryingRequest = true;
                }
                catch (Exception exception)
                {

                    _logger?.LogError(exception,
                        "An Exception occured whilst performing a HTTP {httpMethod} Request on thr Uri \"{uri}\"",
                        method.ToString(), route);

                    return ApiResponse<TResponse>.Failure("");
                }
            } while (retryingRequest);

            return await ProcessResponseAsync<TResponse>(responseMessage);
        }

        #endregion
    }
}
