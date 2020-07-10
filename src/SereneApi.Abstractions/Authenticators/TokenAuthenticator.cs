using DeltaWare.Dependencies;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Response;
using System;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Authenticators
{
    public class TokenAuthenticator<TApi, TDto>: IAuthenticator where TApi : class, IDisposable where TDto : class
    {
        //private readonly TimerCallback _tokenRefresher;

        private readonly Func<TApi, Task<IApiResponse<TDto>>> _apiCall;

        private readonly Func<TDto, TokenInfo> _extractTokenFunction;

        protected IDependencyProvider Dependencies { get; }

        protected int TokenExpiryTime { get; private set; }

        protected BearerAuthentication Authentication { get; private set; }

        public TokenAuthenticator(IDependencyProvider dependencies, Func<TApi, Task<IApiResponse<TDto>>> apiCall, Func<TDto, TokenInfo> extractToken = null)
        {
            Dependencies = dependencies;

            _apiCall = apiCall;
            _extractTokenFunction = extractToken;
        }

        public virtual async Task<IAuthentication> AuthenticateAsync()
        {
            if(Authentication != null)
            {
                return Authentication;
            }

            TApi apiHandler = null;

            if(Dependencies.TryGetDependency(out IApiHandlerFactory handlerFactory))
            {
                apiHandler = handlerFactory.Build<TApi>();
            }

            if(apiHandler == null)
            {
                throw new ArgumentException($"Could not find any registered instances of {typeof(TApi).Name}");
            }

            IApiResponse<TDto> response = await GetTokenAsync(apiHandler);

            apiHandler.Dispose();

            ProcessResponse(response);

            return Authentication;
        }

        protected Task<IApiResponse<TDto>> GetTokenAsync(TApi handler)
        {
            return _apiCall.Invoke(handler);
        }

        protected void ProcessResponse(IApiResponse<TDto> response)
        {
            if(!response.WasSuccessful || response.HasNullResult())
            {
                if(response.HasException)
                {
                    throw response.Exception;
                }

                throw new Exception(response.Message);
            }

            TokenInfo tokenInfo = _extractTokenFunction.Invoke(response.Result);

            if(tokenInfo == null)
            {
                throw new ArgumentNullException("getTokenInfo", "No token was retrieved.");
            }

            TokenExpiryTime = tokenInfo.ExpiryTime;

            Authentication = new BearerAuthentication(tokenInfo.Token);
        }
    }
}
