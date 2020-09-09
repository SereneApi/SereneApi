using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Serialization;

namespace SereneApi.Adapters.Testing
{
    public static class DefaultApiExtensionsExtensions
    {
        public static IDefaultApiConfigurationExtensions AddTestingAdapter(this IDefaultApiConfigurationExtensions extensions)
        {
            extensions.AddDependencies(d => d.AddScoped<ISerializer>(() => null));

            return extensions;
        }
    }
}
