using System;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Serialization;
using SereneApi.Adapters.Testing.Profiling;

namespace SereneApi.Adapters.Testing
{
    public static class DefaultApiExtensionsExtensions
    {
        private static IApiProfiler _apiProfiler;

        public static IDefaultApiConfigurationExtensions AddTestingAdapter(this IDefaultApiConfigurationExtensions extensions)
        {
            _apiProfiler ??= new ApiProfiler();

            extensions.AddDependencies(d => d.AddSingleton(() => _apiProfiler));

            return extensions;
        }
    }
}
