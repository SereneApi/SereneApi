using DeltaWare.Dependencies;

namespace SereneApi.Abstractions
{
    public interface ICoreOptions
    {
        IDependencyCollection Dependencies { get; }
    }
}
