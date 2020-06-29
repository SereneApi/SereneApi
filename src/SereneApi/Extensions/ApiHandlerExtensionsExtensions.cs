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
        public static IApiHandlerExtensions AddAuthenticator<TApi, TDto>(this IApiHandlerExtensions registrationExtensions, Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenInfo> getTokenInfo) where TApi : class where TDto : class
        {
            CoreOptions coreOptions = GetCoreOptions(registrationExtensions);

            TokenAuthenticator<TApi, TDto> authenticator = new TokenAuthenticator<TApi, TDto>(coreOptions.Dependencies, callApiFunction, getTokenInfo);

            coreOptions.Dependencies.AddDependency<IAuthenticator>(() => authenticator);

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
