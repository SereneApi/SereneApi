using SereneApi.Abstractions.Configuration;
using System;

namespace SereneApi.Abstractions.Handler
{
    /// <summary>
    /// Handles API requests using the specified connection.
    /// </summary>
    public interface IApiHandler: IDisposable
    {
        /// <summary>
        /// The connection that will be used when performing API requests.
        /// </summary>
        IConnectionSettings Connection { get; }
    }
}
