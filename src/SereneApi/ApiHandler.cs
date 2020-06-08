using Microsoft.Extensions.Logging;
using SereneApi.Interfaces;
using SereneApi.Types;
using SereneApi.Types.Dependencies;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http;

namespace SereneApi
{
    /// <summary>
    /// When Inherited; Provides the methods required for implementing a RESTful Api consumer.
    /// </summary>
    [DebuggerDisplay("Source:{Source}; Timeout:{Timeout}")]
    public abstract partial class ApiHandler : IDisposable
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
        /// The <see cref="ILogger"/> this <see cref="ApiHandler"/> will use
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="IQueryFactory"/> that will be used for creating queries
        /// </summary>
        private readonly IQueryFactory _queryFactory;

        private readonly RetryDependency _retry;

        private readonly ISerializer _serializer;

        #endregion
        #region Properties

        /// <summary>
        /// The <see cref="HttpClient"/> used by the <see cref="ApiHandler"/> for all requests
        /// </summary>
        protected virtual HttpClient Client => _httpClient;

        /// <inheritdoc cref="IApiHandlerOptions.Source"/>
        public Uri Source { get; }

        /// <inheritdoc cref="IApiHandlerOptions.Resource"/>
        public string Resource => _options.Resource;

        /// <inheritdoc cref="IApiHandlerOptions.ResourcePath"/>
        public string ResourcePath => _options.ResourcePath;

        /// <summary>
        /// How long a request will stay alive before expiring
        /// </summary>
        public TimeSpan Timeout => Client.Timeout;

        /// <summary>
        /// How many times the <see cref="ApiHandler"/> will retry a request after it has timed out
        /// </summary>
        public int RetryCount => _retry.Count;

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

            Source = new Uri($"{options.Source}{options.ResourcePath}{options.Resource}");

            #region Configure Dependencies

            if (!_options.Dependencies.TryGetDependency(out _httpClient))
            {
                throw new ArgumentException("No HttpClient was provided");
            }

            #region Required

            _queryFactory = _options.Dependencies.GetDependency<IQueryFactory>();
            _serializer = _options.Dependencies.GetDependency<ISerializer>();

            #endregion
            #region Optional

            _options.Dependencies.TryGetDependency(out _logger);
            _options.Dependencies.TryGetDependency(out _retry);

            #endregion

            #endregion

            _logger?.LogTrace($"{GetType()} has been instantiated");
        }

        #endregion
        #region Response Processing

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

            if (responseMessage.IsSuccessStatusCode)
            {
                return ApiResponse.Success();
            }

            _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

            return ApiResponse.Failure(responseMessage.ReasonPhrase);
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
                route = new Uri($"{ResourcePath}{Resource}", UriKind.Relative);
            }
            else
            {
                route = new Uri($"{ResourcePath}{Resource}/{endpoint}", UriKind.Relative);
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

            Uri route = new Uri($"{ResourcePath}{Resource}{endpoint}{queryString}", UriKind.Relative);

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

            Uri route = new Uri($"{ResourcePath}{Resource}/{action}{queryString}", UriKind.Relative);

            return route;
        }

        /// <summary>
        /// Formats the endpoint template.
        /// </summary>
        /// <param name="template">The template used for formatting.</param>
        /// <param name="parameters">The parameters to be appended to the template.</param>
        protected virtual string FormatEndpointTemplate(string template, params object[] parameters)
        {
            #region Format Check Logic

            // This should not need to be done, but if it is not done a format that only support 1 parameter but is supplied more than 1 parameter will not fail.
            int expectedFormatLength = template.Length - parameters.Length * 3;

            for (int i = 0; i < parameters.Length; i++)
            {
                expectedFormatLength += parameters[i].ToString().Length;
            }

            #endregion

            string endpoint = string.Format(template, parameters);

            // If the length is different the endpoint has been formatted correctly.
            if (endpoint != template && expectedFormatLength == endpoint.Length)
            {
                return $"{endpoint}";
            }

            // If we have more than 1 parameter here it means the formatting was unsuccessful.
            if (parameters.Length > 1)
            {
                throw new FormatException("Multiple Parameters must be used with a format-table endpoint template.");
            }

            endpoint = template;

            // Return an endpoint without formatting the template and appending the only parameter to the end.
            return $"{endpoint}/{parameters[0]}";
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
