using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Response.Handlers;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi
{
    /// <summary>
    /// When Inherited; Provides the methods required for implementing a RESTful Api consumer.
    /// </summary>
    [DebuggerDisplay("Source:{Connection.Source}; Timeout:{Connection.Timeout}")]
    public abstract partial class BaseApiHandler: IApiHandler
    {
        #region Variables

        private readonly ILogger _logger;

        private readonly IEventManager _eventManager;

        #endregion
        #region Properties

        /// <summary>
        /// The dependencies that may be used by this API.
        /// </summary>
        protected IApiOptions Options { get; }

        protected IResponseHandler ResponseHandler { get; }

        /// <inheritdoc cref="IApiHandler.Connection"/>
        public IConnectionConfiguration Connection { get; }

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="BaseApiHandler"/>.
        /// </summary>
        /// <param name="options">The options to be used when making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected BaseApiHandler([NotNull] IApiOptions options)
        {
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Connection = options.Connection;

            Options = options;
            Options.RetrieveDependency(out _logger);
            Options.RetrieveDependency(out _eventManager);

            ResponseHandler = Options.RetrieveRequiredDependency<IResponseHandler>();

            _logger?.LogInformation($"{GetType()} has been instantiated");
        }

        #endregion
        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Throws an Object Disposed Exception if the <see cref="BaseApiHandler"/> has been disposed.
        /// </summary>
        protected void CheckIfDisposed()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException(nameof(GetType));
            }
        }

        /// <summary>
        /// Disposes the current instance of the <see cref="BaseApiHandler"/>.
        /// </summary>
        public void Dispose()
        {
            _logger?.LogInformation($"{GetType()} is being disposed");

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override this method to implement <see cref="BaseApiHandler"/> disposal.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if(_disposed)
            {
                return;
            }

            if(disposing)
            {
                Options.Dispose();
            }

            _disposed = true;
        }

        #endregion

        private string GetRequestRoute(Uri endpoint)
        {
            return $"{Options.Connection.BaseAddress}{endpoint}";
        }
    }
}
