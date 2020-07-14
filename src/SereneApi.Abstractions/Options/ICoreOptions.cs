using DeltaWare.Dependencies;

namespace SereneApi.Abstractions.Options
{
    /// <summary>
    /// Contains core dependencies.
    /// </summary>
    public interface ICoreOptions
    {
        /// <summary>
        /// The dependencies available to the options consumer.
        /// </summary>
        IDependencyCollection Dependencies { get; }
    }
}
