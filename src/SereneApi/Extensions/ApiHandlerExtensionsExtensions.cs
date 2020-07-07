using DeltaWare.Dependencies;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Authenticators;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Response;
using System;
using System.Threading.Tasks;
using SereneApi.Abstractions.Extensions;

namespace SereneApi.Extensions
{
    public static class ApiHandlerExtensionsExtensions
    {
        /// <summary>
        /// NOT FOR DI
        /// </summary>
        public static IApiHandlerExtensions AddAuthenticator<TApi, TDto>(this IApiHandlerExtensions extensions, Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenInfo> getTokenInfo) where TApi : class, IDisposable where TDto : class
        {
            IDependencyCollection dependencies = extensions.GetDependencyCollection();

            dependencies.AddSingleton<IAuthenticator>(p => new TokenAuthenticator<TApi, TDto>(p, callApiFunction, getTokenInfo));

            return extensions;
        }

        internal static IDependencyCollection GetDependencyCollection(this IApiHandlerExtensions extensions)
        {
            if(extensions is ICoreOptions options)
            {
                return options.Dependencies;
            }

            throw new InvalidCastException($"Must inherit from {nameof(ICoreOptions)}");
        }
    }
}
