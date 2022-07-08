using DeltaWare.Dependencies.Abstractions;
using DeltaWare.SDK.Core.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using SereneApi.Authentication.Web.Msal;
using SereneApi.Authentication.Web.Msal.Options;
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
        /// <param name="configuration">The <see cref="IApiConfiguration"/> to be configured.</param>
        /// <param name="clientId">The Client Id of the API.</param>
        public static IMsalClientAuthentication UseMsalAuthentication(this IApiConfiguration configuration, string clientId)
        {
            StringValidator.ThrowOnNullOrWhitespace(clientId, nameof(clientId));

            MsalAuthenticationOptions options = new(clientId);

            UseMsalAuthentication(configuration, options);

            return options;
        }

        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> to be authenticated by Microsoft Azure.
        /// </summary>
        /// <remarks>An instance of <see cref="IApiAuthorization"/> must be present for configuration.</remarks>
        /// <param name="enableClientAuthentication">Specifies if client authentication will be enabled.</param>
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

                options.RegisterUserScopes(authorization.Scopes);

                UseMsalAuthentication(configuration, options);
            }
            else
            {
                throw new ArgumentException($"Could not find dependency {nameof(IApiAuthorization)}");
            }
        }

        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> to be authenticated by Microsoft Azure.
        /// </summary>
        /// <param name="authenticationOptions">The authentication options to be used for authentication</param>
        public static void UseMsalAuthentication(this IApiConfiguration configuration, IMsalAuthenticationOptions authenticationOptions)
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