using DeltaWare.Dependencies;
using SereneApi.Types;
using SereneApi.Types.Authenticators;
using System;
using System.Threading.Tasks;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Interfaces
{
    public static class ApiHandlerExtensionsExtensions
    {
        /// <summary>
        /// NOT FOR DI
        /// </summary>
        public static IApiHandlerExtensions AddAuthenticator<TApi, TDto>(this IApiHandlerExtensions registrationExtensions, Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenInfo> getTokenInfo) where TApi : class, IDisposable where TDto : class
        {
            CoreOptions coreOptions = GetCoreOptions(registrationExtensions);

            coreOptions.Dependencies.AddSingleton<IAuthenticator>(p => new TokenAuthenticator<TApi, TDto>(p, callApiFunction, getTokenInfo));

            return registrationExtensions;
        }

        private static CoreOptions GetCoreOptions(IApiHandlerExtensions extensions)
        {
            if(extensions is CoreOptions coreOptions)
            {
                return coreOptions;
            }

            throw new TypeAccessException($"Must be of type or inherit from {nameof(CoreOptions)}");
        }
    }
}
