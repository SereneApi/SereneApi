using DeltaWare.Dependencies;
using SereneApi.Interfaces;
using System;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi
{
    public interface IApiHandlerOptions: IDisposable
    {
        /// <summary>
        /// The Dependencies required by the <see cref="ApiHandler"/>.
        /// </summary>
        IDependencyProvider Dependencies { get; }

        IConnectionSettings Connection { get; }
    }
}
