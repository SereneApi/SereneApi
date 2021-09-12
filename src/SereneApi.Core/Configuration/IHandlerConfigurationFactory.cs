using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Core.Configuration
{
    public interface IHandlerConfigurationFactory
    {
        Configuration Configuration { get; }

        IDependencyCollection Dependencies { get; }
    }
}