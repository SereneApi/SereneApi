using SereneApi.Extensions.Authentication.Interfaces;
using SereneApi.Types;
using System;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Interfaces
{
    public static class IRegisterApiHandlerExtensionsExtensions
    {
        public static IApiHandlerExtensions AddAuthenticator(this IApiHandlerExtensions registrationExtensions, IAuthenticator authenticator)
        {
            CoreOptions coreOptions = GetCoreOptions(registrationExtensions);

            coreOptions.DependencyCollection.AddDependency(authenticator);

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
