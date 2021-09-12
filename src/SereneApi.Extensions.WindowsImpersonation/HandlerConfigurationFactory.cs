using ApiCommon.Core.Configuration;
using ApiCommon.Core.Requests.Handler;
using DeltaWare.Dependencies.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ApiCommon.Extensions.WindowsImpersonation
{
    public static class HandlerConfigurationFactory
    {
        [SupportedOSPlatform("windows")]
        public static void UseWindowsImpersonation(this IHandlerConfigurationFactory factory)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException("This method is only supported on the Windows platform");
            }

            factory.Dependencies.AddScoped(p => p.GetDependency<IServiceProvider>().GetService<IHttpContextAccessor>());
            factory.Dependencies.AddScoped<IRequestHandler, ImpersonatingRequestHandler>();
        }
    }
}