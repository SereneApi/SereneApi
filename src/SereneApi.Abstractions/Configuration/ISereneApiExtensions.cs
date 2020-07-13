using DeltaWare.Dependencies;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Configuration
{
    /// <summary>
    /// Extends <see cref="ISereneApiConfiguration"/>.
    /// </summary>
    public interface ISereneApiExtensions
    {
        /// <summary>
        /// Adds the specified dependencies.
        /// </summary>
        /// <param name="factory">Builds the dependencies.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddDependency([NotNull] Action<IDependencyCollection> factory);
    }
}
