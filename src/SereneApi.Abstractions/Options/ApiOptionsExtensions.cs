using DeltaWare.Dependencies;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Authenticators;
using SereneApi.Abstractions.Response;
using System;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Options
{
    public class ApiOptionsExtensions: IApiOptionsExtensions, ICoreOptions
    {
        public IDependencyCollection Dependencies { get; }

        public ApiOptionsExtensions(IDependencyCollection dependencies)
        {
            Dependencies = dependencies;
        }

        /// <summary>
        /// NOT FOR DI
        /// </summary>
        public IApiOptionsExtensions AddAuthenticator<TApi, TDto>(Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenInfo> getTokenInfo) where TApi : class, IDisposable where TDto : class
        {
            Dependencies.AddSingleton<IAuthenticator>(p => new TokenAuthenticator<TApi, TDto>(p, callApiFunction, getTokenInfo));

            return this;
        }
    }
}
