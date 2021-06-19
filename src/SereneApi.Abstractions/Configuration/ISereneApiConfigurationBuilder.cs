using System;
using System.Net;

namespace SereneApi.Abstractions.Configuration
{
    /// <summary>
    /// Configures <see cref="ISereneApiConfiguration"/>.
    /// </summary>
    public interface ISereneApiConfigurationBuilder : IConfigurationExtensions
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
        /// Specifies an <see cref="ICredentials"/> which will be used when authenticating.
        /// </summary>
        /// <param name="credentials">The <see cref="ICredentials"/> to be for authentication.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddCredentials(ICredentials credentials);
    }
}
