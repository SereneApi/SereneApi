using SereneApi.Abstractions.Configuration;
using DeltaWare.Dependencies.Abstractions;
using System;

namespace SereneApi.Abstractions.Options
{
    /// <summary>
    /// The options for a specific API.
    /// </summary>
    public interface IApiOptions: IDisposable
    {
        IDependencyProvider Dependencies { get; }

        /// <summary>
        /// The connect settings used when making an API request.
        /// </summary>
        IConnectionConfiguration Connection { get; }

        bool ThrowExceptions { get; }
    }
}
