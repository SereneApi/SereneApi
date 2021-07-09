using DeltaWare.Dependencies.Abstractions;
using Microsoft.AspNetCore.Http;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Requests;
using SereneApi.AspNet.Requests;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace SereneApi.AspNet
{
    public static class ApiOptionsBuilderExtensions
    {
        [SupportedOSPlatform("windows")]
        public static void UseWindowsImpersonation(this IApiOptionsBuilder builder)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException("This method is only supported on the Windows platform");
            }

            builder.UseRequestHandler(p => new ImpersonatingRequestHandler(p));
        }
    }
}
