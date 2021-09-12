using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Connection;
using System;

namespace SereneApi.Core.Options
{
    public interface IApiOptions : IDisposable
    {
        /// <summary>
        /// The connect settings used when making an API request.
        /// </summary>
        IConnectionSettings Connection { get; }

        IDependencyProvider Dependencies { get; }
    }
}