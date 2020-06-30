using DeltaWare.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Interfaces;
using SereneApi.Types;
using SereneApi.Types.Authenticators;
using System;
using System.Threading.Tasks;

namespace SereneApi.Extensions.DependencyInjection.Types.Authenticators
{
    public class DITokenAuthenticator<TApi, TDto>: TokenAuthenticator<TApi, TDto> where TApi : class where TDto : class
    {
        public DITokenAuthenticator(IDependencyProvider dependencies, Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenInfo> getTokenInfo) : base(dependencies, callApiFunction, getTokenInfo)
        {
        }

        public override IAuthentication Authenticate()
        {
            if(Authentication != null)
            {
                return Authentication;
            }

            TApi apiHandler = Dependencies.GetDependency<IServiceProvider>().GetRequiredService<TApi>();

            IApiResponse<TDto> response = CallApiFunction.Invoke(apiHandler).GetAwaiter().GetResult();

            ProcessResponse(response);

            return Authentication;
        }
    }
}
