using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Core.Configuration
{
    internal sealed class ApiConfiguration : IApiConfiguration
    {
        public IDependencyCollection Dependencies { get; }

        public ApiConfiguration(IDependencyCollection dependencies)
        {
            Dependencies = dependencies;
        }

        public ApiConfigurationScope CreateScope()
        {
            return new ApiConfigurationScope(Dependencies.CreateScope());
        }
    }
}