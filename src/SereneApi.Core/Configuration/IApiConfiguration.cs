using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Core.Configuration
{
    public interface IApiConfiguration
    {
        IDependencyCollection Dependencies { get; }
    }
}