using System;

namespace SereneApi.Core.Connection
{
    /// <summary>
    /// The settings used to communicate with an API.
    /// </summary>
    public interface IConnectionSettings
    {
        /// <summary>
        /// Specifies the Hosts address.
        /// </summary>
        Uri BaseAddress { get; }

        /// <summary>
        /// Specifies the resource that will be consumed.
        /// </summary>
        /// <remarks>This value is optional and can be overriden when making a request.</remarks>
        string Resource { get; }

        /// <summary>
        /// Specified the intermediate path between the base address and the resource.
        /// </summary>
        /// <remarks>Example: api/</remarks>
        string ResourcePath { get; }

        /// <summary>
        /// Specifies how many times the connection will be retried after it has timed-out.
        /// </summary>
        int RetryAttempts { get; }

        /// <summary>
        /// The correlated source of the API.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Specifies the amount of time in seconds the connection will be kept alive before being timed-out.
        /// </summary>
        int Timeout { get; }
    }
}