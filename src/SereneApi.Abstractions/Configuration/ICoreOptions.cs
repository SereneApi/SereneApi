using DeltaWare.Dependencies;

namespace SereneApi.Abstractions.Configuration
{
    public interface ICoreOptions
    {
        IDependencyCollection Dependencies { get; }
    }
}
