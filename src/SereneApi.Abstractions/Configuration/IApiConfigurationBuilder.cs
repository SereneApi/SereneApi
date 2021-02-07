using DeltaWare.Dependencies.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SereneApi.Abstractions.Configuration
{
    /// <summary>
    /// Configures <see cref="IApiConfiguration"/>.
    /// </summary>
    public interface IApiConfigurationBuilder
    {
        /// <summary>
        /// Specifies the default resource path if it has not been provided.
        /// </summary>
        string ResourcePath { set; }

        /// <summary>
        /// Specifies the default timeout value if it has not been provided.
        /// </summary>
        int Timeout { set; }

        /// <summary>
        /// Specifies the default retry count if it has not been provided.
        /// </summary>
        int RetryCount { set; }

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

        /// <summary>
        /// Specifies an <see cref="ICredentials"/> which will be used when authenticating.
        /// </summary>
        /// <param name="credentials">The <see cref="ICredentials"/> to be for authentication.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddCredentials([NotNull] ICredentials credentials);
    }
}
