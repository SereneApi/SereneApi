using Microsoft.Extensions.Logging;
using SereneApi.Core.Configuration;
using SereneApi.Core.Connection;
using SereneApi.Core.Options;
using SereneApi.Core.Requests;
using SereneApi.Core.Requests.Handler;
using SereneApi.Core.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Core.Handler
{
    public abstract class ApiHandlerBase : IApiHandler
    {
        private readonly ILogger _logger;

        private readonly IRequestHandler _requestHandler;

        /// <inheritdoc cref="IApiHandler.Connection"/>
        public IConnectionSettings Connection { get; }

        /// <summary>
        /// The dependencies that may be used by this API.
        /// </summary>
        public IApiOptions Options { get; }

        protected bool ThrowExceptions { get; }

        protected ApiHandlerBase(IApiOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Connection = options.Connection;

            Options = options;

            Options.Dependencies.TryGetDependency(out _logger);

            _requestHandler = Options.Dependencies.GetDependency<IRequestHandler>();

            ThrowExceptions = Options.Dependencies.GetDependency<IConfiguration>().Get<bool>("ThrowExceptions");

            _logger?.LogTrace(Logging.EventIds.InstantiatedEvent, Logging.Messages.HandlerInstantiated, GetType().Name);
        }

        #region Asynchrounous Methods

        /// <summary>
        /// Performs an API Request Asynchronously.
        /// </summary>
        /// <param name="request">The request to be performed.</param>
        /// <param name="cancellationToken">Cancels an ongoing request.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public virtual async Task<IApiResponse> PerformRequestAsync(IApiRequest request, CancellationToken cancellationToken = default)
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
                if (ThrowExceptions)
                {
                    throw;
                }

                return BuildFailureResponse(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, Logging.Messages.RequestEncounteredException, request.Method.ToString(), GetRequestRoute(request));

                if (ThrowExceptions)
                {
                    throw;
                }

                return BuildFailureResponse(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
        }

        /// <summary>
        /// Performs an API Request Asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">
        /// The <see cref="Type"/> to be deserialized from the body of the response.
        /// </typeparam>
        /// <param name="request">The request to be performed.</param>
        /// <param name="cancellationToken">Cancels an ongoing request.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public virtual async Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>(IApiRequest request, CancellationToken cancellationToken = default)
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
                if (ThrowExceptions)
                {
                    throw;
                }

                return BuildFailureResponse<TResponse>(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(Logging.EventIds.ExceptionEvent, exception, Logging.Messages.RequestEncounteredException, request.Method.ToString(), GetRequestRoute(request));

                if (ThrowExceptions)
                {
                    throw;
                }

                return BuildFailureResponse<TResponse>(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
        }

        #endregion Asynchrounous Methods

        #region Synchrounous Methods

        public virtual IApiResponse PerformRequest(IApiRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            CheckIfDisposed();

            try
            {
                IApiResponse response = _requestHandler.Perform(request, this);

                return response;
            }
            catch (TimeoutException exception)
            {
                if (ThrowExceptions)
                {
                    throw;
                }

                return BuildFailureResponse(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, Logging.Messages.RequestEncounteredException, request.Method.ToString(), GetRequestRoute(request));

                if (ThrowExceptions)
                {
                    throw;
                }

                return BuildFailureResponse(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
        }

        public virtual IApiResponse<TResponse> PerformRequest<TResponse>(IApiRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            CheckIfDisposed();

            try
            {
                IApiResponse<TResponse> response = _requestHandler.Perform<TResponse>(request, this);

                return response;
            }
            catch (TimeoutException exception)
            {
                if (ThrowExceptions)
                {
                    throw;
                }

                return BuildFailureResponse<TResponse>(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(Logging.EventIds.ExceptionEvent, exception, Logging.Messages.RequestEncounteredException, request.Method.ToString(), GetRequestRoute(request));

                if (ThrowExceptions)
                {
                    throw;
                }

                return BuildFailureResponse<TResponse>(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
        }

        #endregion Synchrounous Methods

        protected abstract IApiResponse BuildFailureResponse(IApiRequest request, Status status, string message, Exception exception);

        protected abstract IApiResponse<TResponse> BuildFailureResponse<TResponse>(IApiRequest request, Status status, string message, Exception exception);

        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Occurs when the component is disposed by a call to the Dispose() method
        /// </summary>
        public event EventHandler Disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);

            _logger?.LogDebug(Logging.EventIds.DisposedEvent, Logging.Messages.DisposedHandler, GetType().Name);
        }

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

        #endregion IDisposable

        private string GetRequestRoute(IApiRequest request)
        {
            return $"{Options.Connection.BaseAddress}{request.Route}";
        }
    }
}