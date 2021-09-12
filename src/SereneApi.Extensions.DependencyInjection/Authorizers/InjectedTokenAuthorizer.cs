﻿using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Core.Authorization;
using SereneApi.Core.Authorization.Authorizers;
using SereneApi.Core.Responses;
using System;
using System.Threading.Tasks;

namespace SereneApi.Extensions.DependencyInjection.Authorizers
{
    /// <summary>
    /// Gets the specified authorization API using Dependency Injection.
    /// </summary>
    /// <typeparam name="TApi">The API that will be making the authorization request.</typeparam>
    /// <typeparam name="TDto">The DTO returned by the authentication API.</typeparam>
    internal class InjectedTokenAuthorizer<TApi, TDto> : TokenAuthorizer<TApi, TDto> where TApi : class, IDisposable where TDto : class
    {
        /// <summary>
        /// Creates a new instance of <see cref="InjectedTokenAuthorizer{TApi,TDto}"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies that can be used.</param>
        /// <param name="apiCall">Perform the authorization request.</param>
        /// <param name="retrieveToken">Extract the token information from the response.</param>
        public InjectedTokenAuthorizer(IDependencyProvider dependencies, Func<TApi, Task<IApiResponse<TDto>>> apiCall, Func<TDto, TokenAuthResult> retrieveToken) : base(dependencies, apiCall, retrieveToken)
        {
        }

        /// <inheritdoc cref="IAuthorizer.AuthorizeAsync"/>
        public override async Task<IAuthorization> AuthorizeAsync()
        {
            if (Authorization != null)
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