using DeltaWare.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Abstractions.Authorisation.Authorizers;
using SereneApi.Abstractions.Authorization;
using SereneApi.Abstractions.Response;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Extensions.DependencyInjection.Authorizers
{
    /// <summary>
    /// Gets the authentication API using Dependency Injection.
    /// </summary>
    /// <typeparam name="TApi">The API that will be making the authentication request.</typeparam>
    /// <typeparam name="TDto">The DTO returned by the authentication API.</typeparam>
    internal class InjectedTokenAuthorizer<TApi, TDto>: TokenAuthorizer<TApi, TDto> where TApi : class, IDisposable where TDto : class
    {
        /// <summary>
        /// Creates a new instance of <see cref="InjectedTokenAuthorizer{TApi,TDto}"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies that can be used.</param>
        /// <param name="callApi">Perform the authentication request.</param>
        /// <param name="retrieveToken">Extract the token information from the response.</param>
        public InjectedTokenAuthorizer([NotNull] IDependencyProvider dependencies, [NotNull] Func<TApi, Task<IApiResponse<TDto>>> apiCall, [NotNull] Func<TDto, TokenAuthResult> retrieveToken) : base(dependencies, apiCall, retrieveToken)
        {
        }

        /// <inheritdoc cref="IAuthorizer.AuthorizeAsync"/>
        public override async Task<IAuthorization> AuthorizeAsync()
        {
            if(Authorization != null)
            {
                return Authorization;
            }

            IServiceProvider provider = Dependencies.GetDependency<IServiceProvider>();

            TApi apiHandler = provider.GetRequiredService<TApi>();

            IApiResponse<TDto> response = await PerformAuthenticationRequestAsync(apiHandler);

            RetrieveToken(response);

            return Authorization;
        }
    }
}
