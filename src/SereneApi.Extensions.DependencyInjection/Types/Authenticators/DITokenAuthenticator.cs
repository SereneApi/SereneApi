using DeltaWare.Dependencies.Abstractions;
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
        public DITokenAuthenticator(IDependencyCollection dependencies, Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenInfo> getTokenInfo) : base(dependencies, callApiFunction, getTokenInfo)
        {
        }

        public override IAuthentication Authenticate()
        {
            if(Authentication != null)
            {
                return Authentication;
            }

            using IDependencyProvider provider = Dependencies.BuildProvider();

            TApi apiHandler = provider.GetDependency<IServiceProvider>().GetRequiredService<TApi>();

            IApiResponse<TDto> response = CallApiFunction.Invoke(apiHandler).GetAwaiter().GetResult();

            ProcessResponse(response);

            return Authentication;
        }
    }
}
