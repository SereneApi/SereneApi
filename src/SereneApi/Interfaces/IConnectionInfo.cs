using System;

namespace SereneApi.Interfaces
{
    /// <summary>
    /// The connection information for the <see cref="ApiHandler"/>.
    /// </summary>
    public interface IConnectionInfo
    {
        Uri BaseAddress { get; }

        public string Source { get; }

        /// <summary>
        /// The Resource the <see cref="ApiHandler"/> is accessing.
        /// </summary>
        string Resource { get; }

        /// <summary>
        /// The Path to the resource.
        /// </summary>
        string ResourcePath { get; }

        int Timeout { get; }

        int RetryAttempts { get; }

        IConnectionInfo SetTimeout(int timeout);

        IConnectionInfo SetRetryAttempts(int attemptCount);
    }
}
