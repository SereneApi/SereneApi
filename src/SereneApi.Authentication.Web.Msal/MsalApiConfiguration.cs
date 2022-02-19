using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using SereneApi.Authentication.Web.Msal;
using SereneApi.Authentication.Web.Msal.Options;
using SereneApi.Core.Http.Authentication;
using SereneApi.Core.Http.Requests;
using System;
using DeltaWare.SDK.Core.Validators;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Configuration
{
    public static class MsalApiConfiguration
    {
        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> to be authenticated by Microsoft Azure.
        /// </summary>
        /// <param name="configuration">The <see cref="IApiConfiguration"/> to be configured.</param>
        /// <param name="clientId">The Client Id of the API.</param>
        public static IMsalClientAuthentication UseMsalAuthentication(this IApiConfiguration configuration, string clientId)
        {
            StringValidator.ThrowOnNullOrWhitespace(clientId, nameof(clientId));

            MsalAuthenticationOptions options = new(clientId);

            UseMsalAuthentication(configuration, options);

            return options;
        }

        public static void UseMsalAuthentication(this IApiConfiguration configuration, bool enableClientAuthentication = false)
        {
            using IDependencyProvider provider = configuration.Dependencies.BuildProvider();

            if (provider.TryGetDependency(out IApiAuthorization authorization))
            {
                MsalAuthenticationOptions options = new(authorization.ClientId);

                if (enableClientAuthentication)
                {
                    options.EnableClientAuthentication();
                }

                options.RegisterScopes(authorization.Scopes);

                UseMsalAuthentication(configuration, options);
            }
            else
            {
                throw new ArgumentException($"Could not find dependency {nameof(IApiAuthorization)}");
            }
        }

        public static void UseMsalAuthentication(IApiConfiguration configuration, IMsalAuthenticationOptions authenticationOptions)
        {
            if (!configuration.Dependencies.HasDependency<IServiceProvider>())
            {
                throw new NotSupportedException("SereneApi must be attached to Microsoft Dependency Injection for this method to be supported.");
            }

            if (authenticationOptions == null)
            {
                throw new ArgumentNullException(nameof(authenticationOptions));
            }

            configuration.Dependencies.TryAddScoped(p => p
                .GetRequiredDependency<IServiceProvider>()
                .GetRequiredService<ITokenAcquisition>());

            configuration.Dependencies.TryAddScoped<IAuthenticator, MsalAuthenticator>();

            configuration.Dependencies.AddSingleton(() => authenticationOptions);
        }
    }
}