using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Authorization.Types;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Response;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Authorization.Authorizers
{
    /// <summary>
    /// Authorizes using the specified API call returning a <see cref="BearerAuthorization"/> result.
    /// </summary>
    /// <typeparam name="TApi">The API that will be used to authorize with.</typeparam>
    /// <typeparam name="TDto">The method in which the token will be retrieved.</typeparam>
    public class TokenAuthorizer<TApi, TDto>: IAuthorizer where TApi : class, IDisposable where TDto : class
    {
        //private readonly TimerCallback _tokenRefresher;

        private readonly Func<TApi, Task<IApiResponse<TDto>>> _apiCall;

        private readonly Func<TDto, TokenAuthResult> _extractTokenFunction;

        /// <summary>
        /// The dependencies that can be used to authorize with.
        /// </summary>
        protected IDependencyProvider Dependencies { get; }

        /// <summary>
        /// The time in seconds before the token expires.
        /// </summary>
        protected int TokenExpiryTime { get; private set; }

        /// <summary> 
        /// The authorization result.
        /// </summary>
        protected BearerAuthorization Authorization { get; private set; }

        /// <summary>
        /// Creates a new instance of <seealso cref="TokenAuthorizer{TApi,TDto}"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies the <see cref="TokenAuthorizer{TApi,TDto}"/> can use to authorize with.</param>
        /// <param name="apiCall">The API call to authorize with.</param>
        /// <param name="retrieveToken">The method in which the token will be retrieved.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public TokenAuthorizer([NotNull] IDependencyProvider dependencies, [NotNull] Func<TApi, Task<IApiResponse<TDto>>> apiCall, [NotNull] Func<TDto, TokenAuthResult> retrieveToken = null)
        {
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));

            _apiCall = apiCall ?? throw new ArgumentNullException(nameof(apiCall));
            _extractTokenFunction = retrieveToken ?? throw new ArgumentNullException(nameof(retrieveToken));
        }

        /// <summary>
        /// Performs the specified authorization request, retrieving the <seealso cref="TokenAuthResult"/>.
        /// </summary>
        /// <returns>The authorization result as a <see cref="BearerAuthorization"/>.</returns>
        /// <exception cref="NullReferenceException">Thrown we no response was received.</exception>
        public virtual async Task<IAuthorization> AuthorizeAsync()
        {
            if(Authorization != null)
            {
                return Authorization;
            }

            using TApi api = GetApi();

            IApiResponse<TDto> response = await PerformAuthenticationRequestAsync(api);

            if(response == null)
            {
                throw new NullReferenceException("No response was received.");
            }

            TokenAuthResult token = RetrieveToken(response);

            TokenExpiryTime = token.ExpiryTime;
            Authorization = new BearerAuthorization(token.Token);

            return Authorization;
        }

        /// <summary>
        /// Get ths specified API.
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown when the specified API could not be found.</exception>
        protected virtual TApi GetApi()
        {
            TApi api = null;

            if(Dependencies.TryGetDependency(out IApiFactory handlerFactory))
            {
                api = handlerFactory.Build<TApi>();
            }

            if(api == null)
            {
                throw new NullReferenceException($"Could not find any registered instances of {typeof(TApi).Name}");
            }

            return api;
        }

        /// <summary>
        /// Gets the token asynchronously using the supplied API.
        /// </summary>
        /// <param name="api">The API ud to mke the request.</param>
        protected virtual Task<IApiResponse<TDto>> PerformAuthenticationRequestAsync(TApi api)
        {
            return _apiCall.Invoke(api);
        }

        /// <summary>
        /// Retrieves the token from the API response.
        /// </summary>
        /// <param name="response">The response to retrieve the token from.</param>
        /// <exception cref="Exception">Thrown when the response was not successful.</exception>
        /// <exception cref="NullReferenceException">Thrown when a null token is provided.</exception>
        protected virtual TokenAuthResult RetrieveToken(IApiResponse<TDto> response)
        {
            if(!response.WasSuccessful || response.HasNullResult())
            {
                if(response.HasException)
                {
                    throw response.Exception;
                }

                throw new Exception(response.Message);
            }

            TokenAuthResult token = _extractTokenFunction.Invoke(response.Result);

            if(token == null)
            {
                throw new NullReferenceException("No token was retrieved.");
            }

            return token;
        }
    }
}
