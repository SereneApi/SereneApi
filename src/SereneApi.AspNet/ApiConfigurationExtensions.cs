using DeltaWare.Dependencies.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Requests;
using SereneApi.AspNet.Requests;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using SereneApi.Abstractions.Requests.Handler;

namespace SereneApi.AspNet
{
    public static class ApiConfigurationExtensions
    {
        [SupportedOSPlatform("windows")]
        public static IConfigurationExtensions UseWindowsImpersonation(this IConfigurationExtensions extensions)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException("This method is only supported on the Windows platform");
            }

            extensions.AddDependencies(d => d.AddScoped(p => p.GetDependency<IServiceProvider>().GetService<IHttpContextAccessor>()));
            extensions.AddDependencies(d => d.AddScoped<IRequestHandler>(p => new ImpersonatingRequestHandler(p)));

            return extensions;
        }
    }
}
