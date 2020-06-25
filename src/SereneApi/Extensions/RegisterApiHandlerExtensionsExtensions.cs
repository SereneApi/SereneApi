using SereneApi.Types;
using System;
using System.Threading.Tasks;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Interfaces
{
    public static class RegisterApiHandlerExtensionsExtensions
    {
        public static IApiHandlerExtensions AddAuthenticator<TApi, TDto>(this IApiHandlerExtensions registrationExtensions, Func<TApi, Task<IApiResponse<TDto>>> function, Func<TDto, TokenInfo> selectTokenInfo = null) where TApi : class where TDto : class
        {
            CoreOptions coreOptions = GetCoreOptions(registrationExtensions);

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
