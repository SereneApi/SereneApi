using SereneApi.Abstractions.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Configuration
{
    /// <inheritdoc cref="IConnectionConfiguration"/>
    public class ConnectionConfiguration: IConnectionConfiguration
    {
        /// <inheritdoc cref="IConnectionConfiguration.BaseAddress"/>
        public Uri BaseAddress { get; }

        /// <inheritdoc cref="IConnectionConfiguration.Source"/>
        public string Source { get; }

        /// <inheritdoc cref="IConnectionConfiguration.Resource"/>
        public string Resource { get; }

        /// <inheritdoc cref="IConnectionConfiguration.ResourcePath"/>
        public string ResourcePath { get; }

        /// <inheritdoc cref="IConnectionConfiguration.Timeout"/>
        public int Timeout { get; set; }

        /// <inheritdoc cref="IConnectionConfiguration.RetryAttempts"/>
        public int RetryAttempts { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ConnectionConfiguration"/>.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <param name="resource">The correlated source of the API.</param>
        /// <param name="resourcePath">The API resource that will be consumed.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="UriFormatException">Thrown when the base address is incorrectly formatted.</exception>
        public ConnectionConfiguration([NotNull] string baseAddress, string resource = default, string resourcePath = default)
        {
            if(string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            baseAddress = SourceHelpers.EnsureSourceSlashTermination(baseAddress);

            BaseAddress = new Uri(baseAddress);
            Resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
            ResourcePath = SourceHelpers.EnsureSourceSlashTermination(resourcePath);
            Source = $"{BaseAddress}{ResourcePath}{Resource}";
        }

        /// <summary>
        /// Creates a new instance of <see cref="ConnectionConfiguration"/>.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <param name="resource">The correlated source of the API.</param>
        /// <param name="resourcePath">The API resource that will be consumed.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="UriFormatException">Thrown when the base address is incorrectly formatted.</exception>
        public ConnectionConfiguration([NotNull] Uri baseAddress, string resource = default, string resourcePath = default)
        {
            BaseAddress = SourceHelpers.EnsureSourceSlashTermination(baseAddress);
            Resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
            ResourcePath = SourceHelpers.EnsureSourceSlashTermination(resourcePath);
            Source = $"{BaseAddress}{ResourcePath}{Resource}";
        }
    }
}
