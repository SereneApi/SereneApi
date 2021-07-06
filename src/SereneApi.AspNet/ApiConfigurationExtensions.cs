using DeltaWare.Dependencies.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Requests;
using SereneApi.AspNet.Requests;
using System;
using System.Runtime.Versioning;

namespace SereneApi.AspNet
{
    public static class ApiConfigurationExtensions
    {
        [SupportedOSPlatform("windows")]
        public static IConfigurationExtensions EnableUserImpersonation(this IConfigurationExtensions extensions)
        {
            extensions.AddDependencies(d => d.AddScoped(p => p.GetDependency<IServiceProvider>().GetService<IHttpContextAccessor>()));
            extensions.AddDependencies(d => d.AddScoped<IRequestHandler>(p => new ImpersonatedRequestHandler(p)));

            return extensions;
        }
    }
}
