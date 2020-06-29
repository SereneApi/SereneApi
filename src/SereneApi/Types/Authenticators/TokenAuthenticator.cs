using DeltaWare.Dependencies.Abstractions;
using SereneApi.Interfaces;
using SereneApi.Types.Headers.Authentication;
using System;
using System.Threading.Tasks;

namespace SereneApi.Types.Authenticators
{
    public class TokenAuthenticator<TApi, TDto>: IAuthenticator where TApi : class where TDto : class
    {
        //private readonly TimerCallback _tokenRefresher;

        protected IDependencyCollection Dependencies { get; }

        protected Func<TApi, Task<IApiResponse<TDto>>> CallApiFunction { get; }

        protected Func<TDto, TokenInfo> GetTokenFunction { get; }

        protected int TokenExpiryTime { get; private set; }

        protected BearerAuthentication Authentication { get; private set; }

        public TokenAuthenticator(IDependencyCollection dependencies, Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenInfo> getTokenInfo = null)
        {
            Dependencies = dependencies;
            CallApiFunction = callApiFunction;
            GetTokenFunction = getTokenInfo;
        }

        public virtual IAuthentication Authenticate()
        {
            if(Authentication != null)
            {
                return Authentication;
            }

            TApi apiHandler = null;

            using(IDependencyProvider dependencies = Dependencies.BuildProvider())
            {
                if(dependencies.TryGetDependency(out IApiHandlerFactory handlerFactory))
                {
                    apiHandler = handlerFactory.Build<TApi>();
                }
            }

            if(apiHandler == null)
            {
                throw new ArgumentException($"Could not find any registered instances of {typeof(TApi).Name}");
            }

            IApiResponse<TDto> response = CallApiFunction.Invoke(apiHandler).GetAwaiter().GetResult();

            if(apiHandler is IDisposable disposable)
            {
                disposable.Dispose();
            }

            ProcessResponse(response);

            return Authentication;
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

            TokenInfo tokenInfo = GetTokenFunction.Invoke(response.Result);

            if(tokenInfo == null)
            {
                throw new ArgumentNullException("getTokenInfo", "No token was retrieved.");
            }

            TokenExpiryTime = tokenInfo.ExpiryTime;

            Authentication = new BearerAuthentication(tokenInfo.Token);
        }
    }
}
