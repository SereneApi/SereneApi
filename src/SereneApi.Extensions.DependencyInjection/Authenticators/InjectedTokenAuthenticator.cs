using DeltaWare.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Authenticators;
using SereneApi.Abstractions.Response;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Extensions.DependencyInjection.Authenticators
{
    /// <summary>
    /// Gets the authentication API using Dependency Injection.
    /// </summary>
    /// <typeparam name="TApi">The API that will be making the authentication request.</typeparam>
    /// <typeparam name="TDto">The DTO returned by the authentication API.</typeparam>
    internal class InjectedTokenAuthenticator<TApi, TDto>: TokenAuthenticator<TApi, TDto> where TApi : class, IDisposable where TDto : class
    {
        /// <summary>
        /// Creates a new instance of <see cref="InjectedTokenAuthenticator{TApi,TDto}"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies that can be used.</param>
        /// <param name="callApi">Perform the authentication request.</param>
        /// <param name="extractToken">Extract the token information from the response.</param>
        public InjectedTokenAuthenticator([NotNull] IDependencyProvider dependencies, [NotNull] Func<TApi, Task<IApiResponse<TDto>>> apiCall, [NotNull] Func<TDto, TokenInfo> extractToken) : base(dependencies, apiCall, extractToken)
        {
        }

        /// <inheritdoc cref="IAuthenticator.AuthenticateAsync"/>
        public override async Task<IAuthentication> AuthenticateAsync()
        {
            if(Authentication != null)
            {
                return Authentication;
            }

            IServiceProvider provider = Dependencies.GetDependency<IServiceProvider>();

            TApi apiHandler = provider.GetRequiredService<TApi>();

            IApiResponse<TDto> response = await GetTokenAsync(apiHandler);

            ProcessResponse(response);

            return Authentication;
        }
    }
}
