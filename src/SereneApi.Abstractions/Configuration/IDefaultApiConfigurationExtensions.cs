using DeltaWare.Dependencies.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Configuration
{
    /// <summary>
    /// Extends <see cref="IDefaultApiConfiguration"/>.
    /// </summary>
    public interface IDefaultApiConfigurationExtensions
    {
        /// <summary>
        /// Adds the specified dependencies.
        /// </summary>
        /// <param name="factory">Builds the dependencies.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddDependencies([NotNull] Action<IDependencyCollection> factory);
    }
}
