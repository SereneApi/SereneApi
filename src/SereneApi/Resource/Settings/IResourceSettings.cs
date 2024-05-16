using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Resource.Settings
{
    internal interface IResourceSettings
    {
        IConnectionSettings ConnectionSettings { get; }

        IDependencyProvider Dependencies { get; }
    }
}
