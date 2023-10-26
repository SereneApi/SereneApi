using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Resource.Settings
{
    internal interface IResourceSettings
    {
        IDependencyProvider Dependencies { get; }
    }
}
