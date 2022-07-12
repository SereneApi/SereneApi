using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Http.Authorization.Types;
using SereneApi.Core.Http.Responses;
using System;
using System.Threading.Tasks;

namespace SereneApi.Core.Http.Authentication
{
    /// <summary>
    /// Authorizes using the specified API call returning a <see cref="BearerAuthentication"/> result.
    /// </summary>
    /// <typeparam name="TApi">The API that will be used to authorize with.</typeparam>
    /// <typeparam name="TDto">The httpMethod in which the token will be retrieved.</typeparam>
    public class BearerAuthenticator<TApi, TDto> : ApiHandlerAuthenticator<TApi, TDto>, IAuthenticator where TApi : class, IDisposable where TDto : class
    {
        private BearerAuthentication _cachedAuthentication;
        private TokenAuthResult _lastToken;

        /// <summary>
        /// Creates a new instance of <seealso cref="BearerAuthenticator{TApi,TDto}"/>.
        /// </summary>
        /// <param name="dependencies">
        /// The dependencies the <see cref="BearerAuthenticator{TApi,TDto}"/> can use to authorize with.
        /// </param>
        /// <param name="apiCall">The API call to authorize with.</param>
        /// <param name="retrieveToken">The httpMethod in which the token will be retrieved.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public BearerAuthenticator(IDependencyProvider dependencies, Func<TApi, Task<IApiResponse<TDto>>> apiCall, Func<TDto, TokenAuthResult> retrieveToken) : base(dependencies, apiCall, retrieveToken)
        {
        }

        /// <summary>
        /// Performs the specified authentication request, retrieving the <seealso cref="TokenAuthResult"/>.
        /// </summary>
        /// <returns>The authentication result as a <see cref="BearerAuthentication"/>.</returns>
        /// <exception cref="NullReferenceException">Thrown we no response was received.</exception>
        public async Task<IAuthentication> AuthorizeAsync()
        {
            if (_lastToken == null || _lastToken.Expires != null && DateTimeOffset.Now >= _lastToken.Expires?.AddMinutes(-5.0))
            {
                _lastToken = await RequestAccessTokenAsync();

                _cachedAuthentication = new BearerAuthentication(_lastToken.Token);
            }

            return _cachedAuthentication;
        }
    }
}