using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Abstractions.Options
{
    public interface IApiOptionsExtensions
    {
        IDependencyCollection Dependencies { get; }
    }
}
