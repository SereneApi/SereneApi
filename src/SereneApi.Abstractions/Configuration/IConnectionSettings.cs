using System;

namespace SereneApi.Abstractions.Configuration
{
    /// <summary>
    /// The settings used to communicate with an API.
    /// </summary>
    public interface IConnectionSettings
    {
        /// <summary>
        /// The base address.
        /// </summary>
        Uri BaseAddress { get; }

        /// <summary>
        /// The correlated source of the API.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// The API resource that will be consumed.
        /// </summary>
        string Resource { get; }

        /// <summary>
        /// The Path to the resource.
        /// </summary>
        /// <remarks>Example: api/</remarks>
        string ResourcePath { get; }

        /// <summary>
        /// The amount of time in seconds the connection will be kept alive.
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// How many times the request will be re-attempted if it has timed out.
        /// </summary>
        int RetryAttempts { get; }
    }
}
