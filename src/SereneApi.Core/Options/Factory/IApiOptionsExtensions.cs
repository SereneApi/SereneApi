using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Core.Options.Factory
{
    public interface IApiOptionsExtensions
    {
        IDependencyCollection Dependencies { get; }
    }
}
