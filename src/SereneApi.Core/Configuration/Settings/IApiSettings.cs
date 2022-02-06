using SereneApi.Core.Http;
using DeltaWare.Dependencies.Abstractions;
using System;

namespace SereneApi.Core.Configuration.Settings
{
    public interface IApiSettings : IDisposable
    {
        /// <summary>
        /// The connect settings used when making an API request.
        /// </summary>
        IConnectionSettings Connection { get; }

        IDependencyProvider Dependencies { get; }
    }
}