using System;
using DeltaWare.Dependencies.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Authentication.Web.IdentityServer;
using SereneApi.Authentication.Web.IdentityServer.Options;
using SereneApi.Core.Http.Authentication;
using SereneApi.Core.Http.Requests;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Configuration
{
    public static class IdentityServerApiConfiguration
    {
        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> authenticated by Identity Server.
        /// </summary>
        public static void UseIdentityServerAuthentication(this IApiConfiguration apiConfiguration, IConfiguration configuration, string configurationKey = "IdentityServerAuthentication")
        {
            UseIdentityServerAuthentication<IdentityServerAuthenticator>(apiConfiguration, configuration, configurationKey);
        }

        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> authenticated by Identity Server.
        /// </summary>
        public static void UseIdentityServerAuthentication<TAuthenticator>(this IApiConfiguration apiConfiguration, IConfiguration configuration, string configurationKey = "IdentityServerAuthentication") where TAuthenticator : IdentityServerAuthenticator
        {
            AuthenticationOptions options = new AuthenticationOptions();

            configuration.Bind(configurationKey, options);

            UseIdentityServerAuthentication(apiConfiguration, options);
        }

        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> authenticated by Identity Server.
        /// </summary>
        public static void UseIdentityServerAuthentication(this IApiConfiguration apiConfiguration, Action<AuthenticationOptions> optionsBuilder)
        {
            UseIdentityServerAuthentication<IdentityServerAuthenticator>(apiConfiguration, optionsBuilder);
        }

        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> authenticated by Identity Server.
        /// </summary>
        public static void UseIdentityServerAuthentication<TAuthenticator>(this IApiConfiguration apiConfiguration, Action<AuthenticationOptions> optionsBuilder) where TAuthenticator : IdentityServerAuthenticator
        {
            AuthenticationOptions options = new AuthenticationOptions();

            optionsBuilder.Invoke(options);

            UseIdentityServerAuthentication<TAuthenticator>(apiConfiguration, options);
        }

        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> authenticated by Identity Server.
        /// </summary>
        public static void UseIdentityServerAuthentication(this IApiConfiguration apiConfiguration, AuthenticationOptions options)
        {
            UseIdentityServerAuthentication<IdentityServerAuthenticator>(apiConfiguration, options);
        }

        /// <summary>
        /// Enables this API to have its <see cref="IApiRequest"/> authenticated by Identity Server.
        /// </summary>
        public static void UseIdentityServerAuthentication<TAuthenticator>(this IApiConfiguration apiConfiguration, AuthenticationOptions options) where TAuthenticator : IdentityServerAuthenticator
        {
            if (!apiConfiguration.Dependencies.HasDependency<IServiceProvider>())
            {
                throw new NotSupportedException("SereneApi must be attached to Microsoft Dependency Injection for Identity Server Authentication to be supported.");
            }

            apiConfiguration.Dependencies.Register<IAuthenticationOptions>(() => options).AsSingleton();

            apiConfiguration.Dependencies.Register<TAuthenticator>()
                .DefineAs<IAuthenticator>()
                .AsScoped();

            apiConfiguration.Dependencies
                .TryRegister(p => p.GetRequiredDependency<IServiceProvider>().GetRequiredService<IHttpContextAccessor>())
                .AsScoped()
                .DoNotBind();

            apiConfiguration.Dependencies
                .TryRegister(p => p.GetRequiredDependency<IServiceProvider>().GetService<IMemoryCache>())
                .AsScoped()
                .DoNotBind();
        }
    }
}