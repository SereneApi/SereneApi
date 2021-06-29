using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Connection;
using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Requests.Builder;
using SereneApi.Abstractions.Response.Handlers;
using SereneApi.Requests.Builder;
using System;
using System.Diagnostics;

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

        private readonly IDependencyProvider _dependencies;

        private readonly IEventManager _eventManager;

        #endregion
        #region Properties

        /// <summary>
        /// The dependencies that may be used by this API.
        /// </summary>
        protected internal IApiOptions Options { get; }

        protected internal IResponseHandler ResponseHandler { get; }

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

            _dependencies = Options.Dependencies;
            _dependencies.TryGetDependency(out _logger);
            _dependencies.TryGetDependency(out _eventManager);

            ResponseHandler = _dependencies.GetDependency<IResponseHandler>();

            _logger?.LogTrace(Logging.Messages.HandlerInstantiated, nameof(GetType));
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
            if (_disposed)
            {
                _logger?.LogWarning("{ApiHandler} was accessed after being disposed of.", nameof(GetType));

                throw new ObjectDisposedException(nameof(GetType));
            }
        }

        /// <summary>
        /// Disposes the current instance of the <see cref="BaseApiHandler"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);

            _logger?.LogDebug(Logging.Messages.DisposedHandler, nameof(GetType));
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
