using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Configuration;
using SereneApi.Adapters.Testing.Profiling;

namespace SereneApi.Adapters.Testing
{
    public static class DefaultApiExtensionsExtensions
    {
        private static IProfiler _profiler;

        public static IDefaultApiConfigurationExtensions AddTestingAdapter(this IDefaultApiConfigurationExtensions extensions)
        {
            _profiler ??= new Profiler();

            extensions.AddDependencies(d => d.AddSingleton(() => _profiler));

            return extensions;
        }
    }
}
