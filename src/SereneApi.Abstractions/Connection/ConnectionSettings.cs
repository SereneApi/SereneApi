using SereneApi.Abstractions.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Connection
{
    /// <inheritdoc cref="ConnectionSettings"/>
    public class ConnectionSettings : IConnectionSettings
    {
        /// <inheritdoc cref="IConnectionSettings.BaseAddress"/>
        public Uri BaseAddress { get; }

        /// <inheritdoc cref="IConnectionSettings.Source"/>
        public string Source { get; }

        /// <inheritdoc cref="IConnectionSettings.Resource"/>
        public string Resource { get; }

        /// <inheritdoc cref="IConnectionSettings.ResourcePath"/>
        public string ResourcePath { get; }

        /// <inheritdoc cref="IConnectionSettings.Timeout"/>
        public int Timeout { get; set; }

        /// <inheritdoc cref="IConnectionSettings.RetryAttempts"/>
        public int RetryAttempts { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ConnectionSettings"/>.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <param name="resource">The correlated source of the API.</param>
        /// <param name="resourcePath">The API resource that will be consumed.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="UriFormatException">Thrown when the base address is incorrectly formatted.</exception>
        public ConnectionSettings([NotNull] string baseAddress, string resource = default, string resourcePath = default)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            baseAddress = SourceHelpers.EnsureSlashTermination(baseAddress);

            BaseAddress = new Uri(baseAddress);
            Resource = SourceHelpers.EnsureNoSlashTermination(resource);
            ResourcePath = SourceHelpers.EnsureNoSlashTermination(resourcePath);
            Source = BuildSourceAddress();
        }

        /// <summary>
        /// Creates a new instance of <see cref="ConnectionSettings"/>.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <param name="resource">The correlated source of the API.</param>
        /// <param name="resourcePath">The API resource that will be consumed.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="UriFormatException">Thrown when the base address is incorrectly formatted.</exception>
        public ConnectionSettings([NotNull] Uri baseAddress, string resource = default, string resourcePath = default)
        {
            BaseAddress = SourceHelpers.EnsureSlashTermination(baseAddress);
            Resource = SourceHelpers.EnsureNoSlashTermination(resource);
            ResourcePath = SourceHelpers.EnsureNoSlashTermination(resourcePath);
            Source = BuildSourceAddress();
        }

        private string BuildSourceAddress()
        {
            string source = BaseAddress.ToString();

            if (!string.IsNullOrWhiteSpace(ResourcePath))
            {
                source += ResourcePath;

                if (!string.IsNullOrWhiteSpace(Resource))
                {
                    source += $"/{Resource}";
                }

                return source;
            }

            if (!string.IsNullOrWhiteSpace(Resource))
            {
                source += Resource;
            }

            return source;
        }
    }
}
