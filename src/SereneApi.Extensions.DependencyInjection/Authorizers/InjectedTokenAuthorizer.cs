using SereneApi.Abstractions.Authorization;
using SereneApi.Abstractions.Authorization.Authorizers;
using SereneApi.Abstractions.Response;
using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Extensions.DependencyInjection.Authorizers
{
    /// <summary>
    /// Gets the specified authorization API using Dependency Injection.
    /// </summary>
    /// <typeparam name="TApi">The API that will be making the authorization request.</typeparam>
    /// <typeparam name="TDto">The DTO returned by the authentication API.</typeparam>
    internal class InjectedTokenAuthorizer<TApi, TDto>: TokenAuthorizer<TApi, TDto> where TApi : class, IDisposable where TDto : class
    {
        /// <summary>
        /// Creates a new instance of <see cref="InjectedTokenAuthorizer{TApi,TDto}"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies that can be used.</param>
        /// <param name="apiCall">Perform the authorization request.</param>
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

            return await RetrieveTokenAsync(false);
        }

        protected override TApi GetApi()
        {
            IServiceProvider provider = Dependencies.GetDependency<IServiceProvider>();

            return provider.GetRequiredService<TApi>();
        }
    }
}
