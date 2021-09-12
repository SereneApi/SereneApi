using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Core.Options.Factories
{
    public interface IApiOptionsExtensions
    {
        IDependencyCollection Dependencies { get; }
    }
}