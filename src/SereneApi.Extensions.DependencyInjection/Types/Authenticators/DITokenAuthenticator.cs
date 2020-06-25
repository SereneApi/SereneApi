using Microsoft.Extensions.DependencyInjection;
using SereneApi.Interfaces;
using System;
using System.Threading.Tasks;

namespace SereneApi.Types.Authenticators
{
    public class DITokenAuthenticator<TApi, TDto>: TokenAuthenticator<TApi, TDto> where TApi : class where TDto : class
    {
        public DITokenAuthenticator(Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenInfo> getTokenInfo) : base(callApiFunction, getTokenInfo)
        {
        }

        public override IAuthentication GetAuthentication(IDependencyCollection dependencies)
        {
            if(Authentication != null)
            {
                return Authentication;
            }

            IServiceCollection services = dependencies.GetDependency<IServiceCollection>();

            using ServiceProvider provider = services.BuildServiceProvider();

            TApi apiHandler = provider.GetRequiredService<TApi>();

            IApiResponse<TDto> response = CallApiFunction.Invoke(apiHandler).GetAwaiter().GetResult();

            ProcessResponse(response);

            return Authentication;
        }
    }
}
