using Microsoft.Extensions.Logging;
using SereneApi.Abstraction.Enums;
using SereneApi.Extensions;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace SereneApi
{
    /// <summary>
    /// When Inherited; Provides the methods required for implementing a RESTful Api consumer.
    /// </summary>
    [DebuggerDisplay("Source:{Connection.Source}; Timeout:{Connection.Timeout}")]
    public abstract partial class ApiHandler: IDisposable
    {
        #region Variables

        private readonly IClientFactory _clientFactory;

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

        private readonly IRouteFactory _routeFactory;

        private readonly ISerializer _serializer;

        #endregion
        #region Properties

        public IConnectionSettings Connection { get; }

        #endregion
        #region Conclassors

        /// <summary>
        /// Creates a new instance of the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="options">The <see cref="IApiHandlerOptions"/> the <see cref="ApiHandler"/> will use when making requests.</param>
        protected ApiHandler(IApiHandlerOptions options)
        {
            CheckIfDisposed(options);

            _options = options;

            Connection = _options.Connection;

            #region Configure Dependencies
            #region Required

            _clientFactory = _options.Dependencies.GetDependency<IClientFactory>();
            _queryFactory = _options.Dependencies.GetDependency<IQueryFactory>();
            _routeFactory = _options.Dependencies.GetDependency<IRouteFactory>();

            _serializer = _options.Dependencies.GetDependency<ISerializer>();

            #endregion
            #region Optional

            _options.Dependencies.TryGetDependency(out _logger);

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
        private IApiResponse ProcessResponse(HttpResponseMessage responseMessage)
        {
            if(responseMessage == null)
            {
                _logger?.LogWarning("Received an Empty Http Response");

                return ApiResponse.Failure(Status.None, "Received an Empty Http Response");
            }

            Status status = responseMessage.StatusCode.ToStatus();

            if(status.IsSuccessCode())
            {
                return ApiResponse.Success(status);
            }

            _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

            return ApiResponse.Failure(status, responseMessage.ReasonPhrase);
        }

        #endregion
        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Throws an Object Disposed Exception if the <see cref="ApiHandler"/> has been disposed.
        /// </summary>
        private void CheckIfDisposed(IApiHandlerOptions options)
        {
            CheckIfDisposed();

            if(options is ApiHandlerOptions apiHandlerOptions && apiHandlerOptions.IsDisposed())
            {
                throw new ObjectDisposedException(nameof(apiHandlerOptions.GetType));
            }
        }

        /// <summary>
        /// Throws an Object Disposed Exception if the <see cref="ApiHandler"/> has been disposed.
        /// </summary>
        protected void CheckIfDisposed()
        {
            // TODO: Throw an exception if the HttpClient has been disposed of, at present there is no way to do this.
            if(_disposed)
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

        /// <summary>
        /// Override this method to implement <see cref="ApiHandler"/> disposal.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if(_disposed)
            {
                return;
            }

            if(disposing)
            {
                if(_options is IDisposable disposableOptions)
                {
                    disposableOptions.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
