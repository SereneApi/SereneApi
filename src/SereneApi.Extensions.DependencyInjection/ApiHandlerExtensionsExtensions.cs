using DeltaWare.Dependencies;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Authenticators;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Response;
using SereneApi.Extensions.DependencyInjection.Authenticators;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Extensions.DependencyInjection
{
    public static class ApiHandlerExtensionsExtensions
    {
        /// <summary>
        /// Adds an authentication API. Before a request is made it will be authenticated.
        /// The extracted token will be re-used until it expires at which point the authentication API will retrieve a new one.
        /// </summary>
        /// <typeparam name="TApi">The API that will be making the authentication request.</typeparam>
        /// <typeparam name="TDto">The DTO returned by the authentication API.</typeparam>
        /// <param name="callApi">Perform the authentication request.</param>
        /// <param name="extractToken">Extract the token information from the response.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public static IApiOptionsExtensions AddDIAuthenticator<TApi, TDto>([NotNull] this IApiOptionsExtensions extensions, [NotNull] Func<TApi, Task<IApiResponse<TDto>>> callApi, [NotNull] Func<TDto, TokenInfo> extractToken) where TApi : class, IDisposable where TDto : class
        {
            if(extensions == null)
            {
                throw new ArgumentNullException(nameof(extensions));
            }

            if(callApi == null)
            {
                throw new ArgumentNullException(nameof(callApi));
            }

            if(extractToken == null)
            {
                throw new ArgumentNullException(nameof(extractToken));
            }

            IDependencyCollection dependencies = extensions.GetDependencyCollection();

            dependencies.AddSingleton<IAuthenticator>(p => new InjectedTokenAuthenticator<TApi, TDto>(p, callApi, extractToken));

            return extensions;
        }

        internal static IDependencyCollection GetDependencyCollection(this IApiOptionsExtensions extensions)
        {
            if(extensions is ICoreOptions options)
            {
                return options.Dependencies;
            }

            throw new InvalidCastException($"Must inherit from {nameof(ICoreOptions)}");
        }
    }
}
