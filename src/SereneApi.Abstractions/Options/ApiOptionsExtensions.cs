using DeltaWare.Dependencies;
using SereneApi.Abstractions.Authorisation.Authorizers;
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
        public IApiOptionsExtensions AddAuthenticator<TApi, TDto>(Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenAuthResult> getTokenInfo) where TApi : class, IDisposable where TDto : class
        {
            Dependencies.AddSingleton<IAuthorizer>(p => new TokenAuthorizer<TApi, TDto>(p, callApiFunction, getTokenInfo));

            return this;
        }
    }
}
