using DeltaWare.Dependencies.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Core.Configuration;
using SereneApi.Core.Http.Requests.Handler;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace SereneApi.Extensions.WindowsImpersonation
{
    public static class WindowsImpersonationConfigurationFactory
    {
        /// <summary>
        /// Enables a request to be impersonated as the current windows identity.
        /// </summary>
        /// <remarks>This httpMethod can only be used in a windows environment. The <see cref="IHttpContextAccessor"/> must be present for impersonation to work.</remarks>
        /// <exception cref="NotSupportedException">Thrown when executed in a non windows environment.</exception>
        [SupportedOSPlatform("windows")]
        public static void UseWindowsImpersonation(this IApiConfiguration factory)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException("This httpMethod is only supported on the Windows platform");
            }

            if (!factory.Dependencies.HasDependency<IHttpContextAccessor>())
            {
                factory.Dependencies
                    .Register(p => p.GetRequiredDependency<IServiceProvider>().GetRequiredService<IHttpContextAccessor>())
                    .AsScoped()
                    .DoNotBind();
            }

            factory.Dependencies
                .Register<ImpersonatingRequestHandler>()
                .DefineAs<IRequestHandler>()
                .AsScoped();
        }
    }
}