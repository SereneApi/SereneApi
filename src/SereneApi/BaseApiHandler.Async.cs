using Microsoft.Extensions.Logging;
using SereneApi.Abstractions;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi
{
    // Async Implementation
    public abstract partial class BaseApiHandler
    {
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

                return ApiResponse.Failure(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, Logging.Messages.RequestEncounteredException, request.Method.ToString(), GetRequestRoute(request));

                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse.Failure(request, Status.None,
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

                return ApiResponse<TResponse>.Failure(request, Status.TimedOut, "The Request Timed Out; The retry limit was reached", exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(Logging.EventIds.ExceptionEvent, exception, Logging.Messages.RequestEncounteredException, request.Method.ToString(), GetRequestRoute(request));

                if (Options.ThrowExceptions)
                {
                    throw;
                }

                return ApiResponse<TResponse>.Failure(request, Status.None,
                    $"An Exception occurred whilst performing a HTTP {request.Method} Request",
                    exception);
            }
        }
        #endregion
    }
}
