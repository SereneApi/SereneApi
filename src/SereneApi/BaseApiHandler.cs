using Microsoft.Extensions.Logging;
using SereneApi.Abstractions;
using SereneApi.Abstractions.Connection;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Requests.Builder;
using SereneApi.Requests.Builder;
using System;
using System.Diagnostics;
using SereneApi.Abstractions.Requests.Handler;

namespace SereneApi
{
    /// <summary>
    /// When Inherited; Provides the methods required for implementing a RESTful Api consumer.
    /// </summary>
    [DebuggerDisplay("Source:{Connection.Source}; Timeout:{Connection.Timeout}")]
    public abstract partial class BaseApiHandler : IApiHandler
    {
        #region Variables

        private readonly ILogger _logger;

        private readonly IRequestHandler _requestHandler;

        #endregion
        #region Properties

        /// <summary>
        /// The dependencies that may be used by this API.
        /// </summary>
        protected internal IApiOptions Options { get; }

        protected IApiRequestBuilder MakeRequest => new RequestBuilder(this);

        /// <inheritdoc cref="IApiHandler.Connection"/>
        public IConnectionSettings Connection { get; }

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="BaseApiHandler"/>.
        /// </summary>
        /// <param name="options">The options to be used when making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected BaseApiHandler(IApiOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Connection = options.Connection;

            Options = options;

            Options.Dependencies.TryGetDependency(out _logger);

            _requestHandler = Options.Dependencies.GetDependency<IRequestHandler>();

            _logger?.LogTrace(Logging.EventIds.InstantiatedEvent, Logging.Messages.HandlerInstantiated, GetType().Name);
        }

        #endregion
        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Occurs when the component is disposed by a call to the Dispose() method
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Throws an Object Disposed Exception if the <see cref="BaseApiHandler"/> has been disposed.
        /// </summary>
        protected void CheckIfDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            _logger?.LogError(Logging.EventIds.ExceptionEvent, Logging.Messages.AccessOfDisposedHandler, GetType().Name);

            throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Disposes the current instance of the <see cref="BaseApiHandler"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);

            _logger?.LogDebug(Logging.EventIds.DisposedEvent, Logging.Messages.DisposedHandler, GetType().Name);
        }

        /// <summary>
        /// Override this method to implement <see cref="BaseApiHandler"/> disposal.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {

                Options.Dispose();
            }

            Disposed?.Invoke(this, new EventArgs());

            _disposed = true;
        }

        #endregion

        private string GetRequestRoute(IApiRequest request)
        {
            return $"{Options.Connection.BaseAddress}{request.Route}";
        }
    }
}
