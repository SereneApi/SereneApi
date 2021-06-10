using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Connection;
using System;

namespace SereneApi.Abstractions.Options
{
    public interface IApiOptions : IDisposable
    {
        IDependencyProvider Dependencies { get; }

        /// <summary>
        /// The connect settings used when making an API request.
        /// </summary>
        IConnectionSettings Connection { get; }

        bool ThrowExceptions { get; }
    }
}
