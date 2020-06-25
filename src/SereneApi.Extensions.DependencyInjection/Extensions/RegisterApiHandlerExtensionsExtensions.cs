using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Types;
using SereneApi.Types.Authenticators;
using System;
using System.Threading.Tasks;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Interfaces
{
    public static class RegisterApiHandlerExtensionsExtensions
    {
        /// <summary>
        /// FOR DI
        /// </summary>
        public static IApiHandlerDiRegistrationExtensions AddAuthenticator<TApi, TDto>(this IApiHandlerDiRegistrationExtensions registrationExtensions, Func<TApi, Task<IApiResponse<TDto>>> callApiFunction, Func<TDto, TokenInfo> getTokenInfo) where TApi : class where TDto : class
        {
            CoreOptions coreOptions = GetCoreOptions(registrationExtensions);

            DITokenAuthenticator<TApi, TDto> authenticator = new DITokenAuthenticator<TApi, TDto>(/*coreOptions.Dependencies, */callApiFunction, getTokenInfo);

            coreOptions.Dependencies.AddDependency<IAuthenticator>(authenticator);

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
