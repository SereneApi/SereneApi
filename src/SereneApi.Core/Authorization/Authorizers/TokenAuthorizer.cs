using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Authorization.Types;
using SereneApi.Core.Handler.Factories;
using SereneApi.Core.Response;
using SereneApi.Core.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SereneApi.Core.Authorization.Authorizers
{
    /// <summary>
    /// Authorizes using the specified API call returning a <see cref="BearerAuthorization"/> result.
    /// </summary>
    /// <typeparam name="TApi">The API that will be used to authorize with.</typeparam>
    /// <typeparam name="TDto">The method in which the token will be retrieved.</typeparam>
    public class TokenAuthorizer<TApi, TDto> : IAuthorizer, IDisposable where TApi : class, IDisposable where TDto : class
    {
        private const int _expiryLeewaySeconds = -60;
        private readonly Func<TApi, Task<IApiResponse<TDto>>> _apiCall;
        private readonly object _authorizationLock = new object();
        private readonly Func<TDto, TokenAuthResult> _extractTokenFunction;
        private readonly ILogger _logger;
        private readonly Timer _tokenRenew = new Timer();
        private bool _tokenExpired = false;

        /// <summary>
        /// The authorization result.
        /// </summary>
        protected BearerAuthorization Authorization { get; private set; }

        /// <summary>
        /// The dependencies that can be used to authorize with.
        /// </summary>
        protected IDependencyProvider Dependencies { get; }

        /// <summary>
        /// The time when the token expires.
        /// </summary>
        protected TimeSpan TokenExpiryTime { get; private set; }

        protected bool WillAutoRenew { get; set; }

        /// <summary>
        /// Creates a new instance of <seealso cref="TokenAuthorizer{TApi,TDto}"/>.
        /// </summary>
        /// <param name="dependencies">
        /// The dependencies the <see cref="TokenAuthorizer{TApi,TDto}"/> can use to authorize with.
        /// </param>
        /// <param name="apiCall">The API call to authorize with.</param>
        /// <param name="retrieveToken">The method in which the token will be retrieved.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public TokenAuthorizer(IDependencyProvider dependencies, Func<TApi, Task<IApiResponse<TDto>>> apiCall, Func<TDto, TokenAuthResult> retrieveToken = null, bool autoRenew = true)
        {
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            Dependencies.TryGetDependency(out _logger);

            _apiCall = apiCall ?? throw new ArgumentNullException(nameof(apiCall));
            _extractTokenFunction = retrieveToken ?? throw new ArgumentNullException(nameof(retrieveToken));

            WillAutoRenew = autoRenew;

            // This may not be needed, but we're adding a default time so it doesn't continue to
            // throw events. This value will changed after a token is retrieved.
            _tokenRenew.Elapsed += async (s, e) => await OnTimedEventAsync(s, e);
        }

        public IAuthorization Authorize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs the specified authorization request, retrieving the <seealso cref="TokenAuthResult"/>.
        /// </summary>
        /// <returns>The authorization result as a <see cref="BearerAuthorization"/>.</returns>
        /// <exception cref="NullReferenceException">Thrown we no response was received.</exception>
        public virtual async Task<IAuthorization> AuthorizeAsync()
        {
            Authorization = await RetrieveTokenAsync();

            return Authorization;
        }

        /// <summary>
        /// Get ths specified API.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// Thrown when the specified API could not be found.
        /// </exception>
        protected virtual TApi GetApi()
        {
            if (Dependencies.TryGetDependency(out IApiFactory handlerFactory))
            {
                return handlerFactory.Build<TApi>();
            }

            _logger?.LogError(Logging.EventIds.DependencyNotFound, Logging.Messages.DependencyNotFound, nameof(IApiFactory));

            throw new NullReferenceException($"Could not retrieve an instance of {nameof(IApiFactory)}");
        }

        /// <summary>
        /// Retrieves the token from the API
        /// </summary>
        /// <param name="disposeApi">Specifies if the API will be disposed of.</param>
        /// <exception cref="Exception">Thrown when the response was not successful.</exception>
        /// <exception cref="NullReferenceException">Thrown when a null token is provided.</exception>
        protected virtual async Task<BearerAuthorization> RetrieveTokenAsync(bool disposeApi = true)
        {
            Monitor.Enter(_authorizationLock);

            if (Authorization != null || !_tokenExpired)
            {
                Monitor.Exit(_authorizationLock);

                _logger?.LogDebug(Logging.EventIds.AuthorizationEvent, Logging.Messages.AuthorizationTokenCached);

                return Authorization;
            }

            try
            {
                TApi api = GetApi();

                IApiResponse<TDto> response = await _apiCall.Invoke(api);

                if (disposeApi)
                {
                    api.Dispose();
                }

                if (!response.WasSuccessful || response.HasNullData())
                {
                    if (response.HasException)
                    {
                        throw response.Exception;
                    }

                    throw new Exception(response.Message);
                }

                TokenAuthResult token = _extractTokenFunction.Invoke(response.Data);

                if (token == null)
                {
                    throw new NullReferenceException("No token was retrieved.");
                }

                // Not sure if this is really the best way to achieve this...
                TokenExpiryTime = TimeSpan.FromSeconds(token.ExpiryTime + _expiryLeewaySeconds);

                _tokenRenew.Interval = TokenExpiryTime.Milliseconds;
                _tokenExpired = false;

                return new BearerAuthorization(token.Token);
            }
            finally
            {
                Monitor.Exit(_authorizationLock);
            }
        }

        private async Task OnTimedEventAsync(object source, ElapsedEventArgs e)
        {
            if (WillAutoRenew)
            {
                _logger?.LogDebug(Logging.EventIds.AuthorizationEvent, Logging.Messages.AuthorizationTokenRenewal);

                Authorization = await RetrieveTokenAsync();

                if (!_tokenRenew.Enabled)
                {
                    _tokenRenew.Start();
                }
            }
            else
            {
                _tokenExpired = true;
            }
        }

        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Disposes the current instance of the <see cref="TokenAuthorizer{TApi,TDto}"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override this method to implement <see cref="TokenAuthorizer{TApi,TDto}"/> disposal.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _tokenRenew.Stop();
                _tokenRenew.Dispose();
            }

            _disposed = true;
        }

        #endregion IDisposable
    }
}