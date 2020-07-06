using DeltaWare.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Authenticators;
using SereneApi.Abstractions.Response;
using System;
using System.Threading.Tasks;

namespace SereneApi.Extensions.DependencyInjection.Types.Authenticators
{
    internal class DiTokenAuthenticator<TApi, TDto>: TokenAuthenticator<TApi, TDto> where TApi : class, IDisposable where TDto : class
    {
        public DiTokenAuthenticator(IDependencyProvider dependencies, Func<TApi, Task<IApiResponse<TDto>>> apiCallFunction, Func<TDto, TokenInfo> getTokenInfo) : base(dependencies, apiCallFunction, getTokenInfo)
        {
        }

        public override async Task<IAuthentication> AuthenticateAsync()
        {
            if(Authentication != null)
            {
                return Authentication;
            }

            TApi apiHandler = Dependencies.GetDependency<IServiceProvider>().GetRequiredService<TApi>();

            IApiResponse<TDto> response = await GetTokenAsync(apiHandler);

            ProcessResponse(response);

            return Authentication;
        }
    }
}
