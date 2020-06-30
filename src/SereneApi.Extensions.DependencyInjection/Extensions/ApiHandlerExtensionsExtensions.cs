using DeltaWare.Dependencies;
using SereneApi.Extensions.DependencyInjection.Types.Authenticators;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Threading.Tasks;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Extensions.DependencyInjection
{
    public static class ApiHandlerExtensionsExtensions
    {
        /// <summary>
        /// FOR DI
        /// </summary>
        public static IApiHandlerExtensions AddInjectedAuthenticator<TApi, TDto>(this IApiHandlerExtensions registrationExtensions, Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenInfo> getTokenInfo) where TApi : class where TDto : class
        {
            CoreOptions coreOptions = GetCoreOptions(registrationExtensions);

            coreOptions.Dependencies.AddSingleton<IAuthenticator>(p => new DITokenAuthenticator<TApi, TDto>(p, callApiFunction, getTokenInfo));

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
