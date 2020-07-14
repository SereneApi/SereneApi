using DeltaWare.Dependencies;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Configuration
{
    /// <summary>
    /// Configures <see cref="ISereneApiConfiguration"/>.
    /// </summary>
    public interface ISereneApiConfigurationBuilder
    {
        /// <summary>
        /// Specifies the default resource path if it has not been provided.
        /// </summary>
        string ResourcePath { get; set; }

        /// <summary>
        /// Specifies the default timeout value if it has not been provided.
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// Specifies the default retry count if it has not been provided.
        /// </summary>
        int RetryCount { get; set; }

        /// <summary>
        /// Overrides previously set dependencies with the specified dependencies.
        /// </summary>
        /// <param name="factory">Builds the dependencies.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void OverrideDependencies([NotNull] Action<IDependencyCollection> factory);

        /// <summary>
        /// Adds the specified dependencies.
        /// </summary>
        /// <param name="factory">Builds the dependencies.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddDependencies([NotNull] Action<IDependencyCollection> factory);
    }
}
