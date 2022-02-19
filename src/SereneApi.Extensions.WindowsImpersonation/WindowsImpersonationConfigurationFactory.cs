using DeltaWare.Dependencies.Abstractions;
using DeltaWare.Dependencies.Abstractions.Enums;
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
        [SupportedOSPlatform("windows")]
        public static void UseWindowsImpersonation(this IApiConfiguration factory)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException("This method is only supported on the Windows platform");
            }

            factory.Dependencies.AddScoped(p => p.GetRequiredDependency<IServiceProvider>().GetRequiredService<IHttpContextAccessor>(), Binding.Unbound);
            factory.Dependencies.AddScoped<IRequestHandler, ImpersonatingRequestHandler>();
        }
    }
}