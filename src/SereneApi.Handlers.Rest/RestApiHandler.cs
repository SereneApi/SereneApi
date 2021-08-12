using Microsoft.Extensions.Logging;
using SereneApi.Core;
using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Connection;
using SereneApi.Core.Handler;
using SereneApi.Core.Options;
using SereneApi.Core.Requests;
using SereneApi.Core.Requests.Handler;
using SereneApi.Core.Responses;
using SereneApi.Handlers.Rest.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using SereneApi.Handlers.Rest.Requests.Factories;
using SereneApi.Handlers.Rest.Responses.Types;

namespace SereneApi.Handlers.Rest
{
    [ConfigurationProvider(typeof(RestConfigurationProvider))]
    public abstract class RestApiHandler : IApiHandler
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

        protected IApiRequestFactory MakeRequest => new RequestFactory(this);

        /// <inheritdoc cref="IApiHandler.Connection"/>
        public IConnectionSettings Connection { get; }

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="RestApiHandler"/>.
        /// </summary>
        /// <param name="options">The options to be used when making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected RestApiHandler(IApiOptions options)
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
        #region Perform Methods

        /// <summary>
        /// Performs an API Request Asynchronously.
        /// </summary>
        /// <param name="request">The request to be performed.</param>
        /// <param name="cancellationToken">Cancels an ongoing request.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected internal virtual async Task<IApiResponse> PerformRequestAsync(IApiRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            CheckIfDisposed();

            try
            {
                IApiResponse response = await _requestHandler.PerformAsync(request, this, cancellationToken);

                return response;
            }
            catch (TimeoutException exception)
            {
                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return RestApiResponse.Failure(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, Logging.Messages.RequestEncounteredException, request.Method.ToString(), GetRequestRoute(request));

                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return RestApiResponse.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
        }

        /// <summary>
        /// Performs an API Request Asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">The <see cref="Type"/> to be deserialized from the body of the response.</typeparam>
        /// <param name="request">The request to be performed.</param>
        /// <param name="cancellationToken">Cancels an ongoing request.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected internal virtual async Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>(IApiRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            CheckIfDisposed();

            try
            {
                IApiResponse<TResponse> response = await _requestHandler.PerformAsync<TResponse>(request, this, cancellationToken);

                return response;
            }
            catch (TimeoutException exception)
            {
                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return RestApiResponse<TResponse>.Failure(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(Logging.EventIds.ExceptionEvent, exception, Logging.Messages.RequestEncounteredException, request.Method.ToString(), GetRequestRoute(request));

                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return RestApiResponse<TResponse>.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
        }
        #endregion
        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Occurs when the component is disposed by a call to the Dispose() method
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Throws an Object Disposed Exception if the <see cref="RestApiHandler"/> has been disposed.
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
        /// Disposes the current instance of the <see cref="RestApiHandler"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);

            _logger?.LogDebug(Logging.EventIds.DisposedEvent, Logging.Messages.DisposedHandler, GetType().Name);
        }

        /// <summary>
        /// Override this method to implement <see cref="RestApiHandler"/> disposal.
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
