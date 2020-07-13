using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;
using System;

namespace SereneApi.Abstractions.Options
{
    /// <summary>
    /// The options for a specific API.
    /// </summary>
    public interface IApiOptions: IDisposable
    {
        /// <summary>
        /// The dependencies that can be used when making an API request.
        /// </summary>
        IDependencyProvider Dependencies { get; }

        /// <summary>
        /// The connect settings used when making an API request.
        /// </summary>
        IConnectionSettings Connection { get; }
    }
}
