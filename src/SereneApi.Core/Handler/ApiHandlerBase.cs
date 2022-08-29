using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Configuration.Handler;
using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Http;
using SereneApi.Core.Http.Requests;
using SereneApi.Core.Http.Requests.Handler;
using SereneApi.Core.Http.Requests.Options;
using SereneApi.Core.Http.Responses;
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
        public IApiSettings Settings { get; }

        protected bool ThrowExceptions { get; }

        protected bool ThrowOnFailure { get; }

        protected ApiHandlerBase(IApiSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Connection = settings.Connection;

            Settings = settings;

            Settings.Dependencies.TryGetDependency(out _logger);

            _requestHandler = Settings.Dependencies.GetRequiredDependency<IRequestHandler>();

            ThrowExceptions = Settings.Dependencies.GetRequiredDependency<HandlerConfiguration>().GetThrowExceptions();
            ThrowOnFailure = Settings.Dependencies.GetRequiredDependency<HandlerConfiguration>().GetThrowOnFailure();

            _logger?.LogTrace(Logging.EventIds.InstantiatedEvent, Logging.Messages.HandlerInstantiated, GetType().Name);
        }

        #region Perform Methods

        /// <summary>
        /// Performs an API Request Asynchronously.
        /// </summary>
        /// <param name="request">The request to be performed.</param>
        /// <param name="cancellationToken">Cancels an ongoing request.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public virtual async Task<IApiResponse> PerformRequestAsync(IApiRequest request, IApiRequestOptions options, CancellationToken cancellationToken = default)
        {
            OnRequest(request, options);

            IApiResponse response;

            try
            {
                response = await _requestHandler.PerformAsync(request, this, cancellationToken); ;
            }
            catch (TimeoutException exception)
            {
                OnTimeout(request, options, exception);

                response = GenerateFailureResponse(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, Logging.Messages.RequestEncounteredException, request.HttpMethod.Method, request.Url);

                OnException(request, options, exception);

                response = GenerateFailureResponse(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.HttpMethod} Request",
                    exception);
            }

            OnResponse(response, options);

            return response;
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
        public virtual async Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>(IApiRequest request, IApiRequestOptions options, CancellationToken cancellationToken = default)
        {
            OnRequest(request, options);

            IApiResponse<TResponse> response;

            try
            {
                response = await _requestHandler.PerformAsync<TResponse>(request, this, cancellationToken);
            }
            catch (TimeoutException exception)
            {
                OnTimeout(request, options, exception);

                response = GenerateFailureResponse<TResponse>(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(Logging.EventIds.ExceptionEvent, exception, Logging.Messages.RequestEncounteredException, request.HttpMethod.Method, request.Url);

                OnException(request, options, exception);

                response = GenerateFailureResponse<TResponse>(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.HttpMethod} Request",
                    exception);
            }

            OnResponse(response, options);

            return response;
        }

        #endregion Perform Methods

        #region Generate Failure Response Methods

        protected abstract IApiResponse GenerateFailureResponse(IApiRequest request, Status status, string message, Exception exception);

        protected abstract IApiResponse<TResponse> GenerateFailureResponse<TResponse>(IApiRequest request, Status status, string message, Exception exception);

        #endregion Generate Failure Response Methods

        #region On Methods

        protected virtual void OnException(IApiRequest request, IApiRequestOptions options, Exception exception)
        {
            options.OnException?.Invoke(exception);

            if (ThrowExceptions || options.ThrowExceptions)
            {
                throw exception;
            }
        }

        protected virtual void OnRequest(IApiRequest request, IApiRequestOptions options)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            CheckIfDisposed();
        }

        protected virtual void OnResponse(IApiResponse response, IApiRequestOptions options)
        {
            if (ThrowOnFailure || options.ThrowOnFail)
            {
                response.ThrowOnFail();
            }
        }

        protected virtual void OnResponse<TResponse>(IApiResponse<TResponse> response, IApiRequestOptions options)
        {
            if (ThrowOnFailure || options.ThrowOnFail)
            {
                response.ThrowOnFail();
            }
        }

        protected virtual void OnTimeout(IApiRequest request, IApiRequestOptions options, TimeoutException exception)
        {
            options.OnTimeout?.Invoke(exception);

            if (ThrowExceptions || options.ThrowExceptions)
            {
                throw exception;
            }
        }

        #endregion On Methods

        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Occurs when the component is disposed by a call to the Dispose() httpMethod
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

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Settings.Dispose();
            }

            Disposed?.Invoke(this, EventArgs.Empty);

            _disposed = true;
        }

        #endregion IDisposable
    }
}