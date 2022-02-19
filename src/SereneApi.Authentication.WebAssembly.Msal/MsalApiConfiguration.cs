using DeltaWare.Dependencies.Abstractions;
using DeltaWare.Dependencies.Abstractions.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Authentication.WebAssembly.Msal;
using SereneApi.Authentication.WebAssembly.Msal.Options;
using SereneApi.Core.Http.Authentication;
using SereneApi.Core.Http.Requests;
using System;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Configuration
{
    public static class MsalApiConfiguration
    {
        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> to be authenticated by Microsoft Azure.
        /// </summary>
        public static void UseMsalAuthentication(this IApiConfiguration configuration, string clientId, Action<IMsalAuthenticationOptionsBuilder> optionsBuilder)
        {
            if (!configuration.Dependencies.HasDependency<IServiceProvider>())
            {
                throw new NotSupportedException("SereneApi must be attached to Microsoft Dependency Injection for this method to be supported.");
            }

            if (optionsBuilder != null)
            {
                MsalAuthenticationOptions options = new MsalAuthenticationOptions(clientId);

                optionsBuilder.Invoke(options);

                configuration.Dependencies.AddSingleton(() => options);
            }

            configuration.Dependencies.AddScoped(p => p
                .GetRequiredDependency<IServiceProvider>()
                .GetRequiredService<IAccessTokenProvider>(),
                Binding.Unbound);

            configuration.Dependencies.AddScoped(p => p
                .GetRequiredDependency<IServiceProvider>()
                .GetRequiredService<NavigationManager>(),
                Binding.Unbound);

            configuration.Dependencies.AddScoped<IAuthenticator, MsalTokenAuthenticator>();
        }

        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> to be authenticated by Microsoft Azure.
        /// </summary>
        public static void UseMsalAuthentication(this IApiConfiguration configuration, IMsalAuthenticationOptions authenticationOptions = null)
        {
            if (!configuration.Dependencies.HasDependency<IServiceProvider>())
            {
                throw new NotSupportedException("SereneApi must be attached to Microsoft Dependency Injection for this method to be supported.");
            }

            if (authenticationOptions == null)
            {
                using IDependencyProvider provider = configuration.Dependencies.BuildProvider();

                if (provider.TryGetDependency(out IApiAuthorization authorization))
                {
                    MsalAuthenticationOptions options = new MsalAuthenticationOptions(authorization.ClientId);

                    options.RegisterScopes(authorization.Scopes);

                    authenticationOptions = options;
                }
            }

            if (authenticationOptions != null)
            {
                configuration.Dependencies.AddSingleton(() => authenticationOptions);
            }

            configuration.Dependencies.AddScoped(p => p
                .GetRequiredDependency<IServiceProvider>()
                .GetRequiredService<IAccessTokenProvider>(),
                Binding.Unbound);

            configuration.Dependencies.AddScoped(p => p
                .GetRequiredDependency<IServiceProvider>()
                .GetRequiredService<NavigationManager>(),
                Binding.Unbound);

            configuration.Dependencies.AddScoped<IAuthenticator, MsalTokenAuthenticator>();
        }
    }
}