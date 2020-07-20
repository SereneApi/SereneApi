using SereneApi.Abstractions.Configuration;
using System;

namespace SereneApi.Abstractions.Options
{
    /// <summary>
    /// The options for a specific API.
    /// </summary>
    public interface IApiOptions: IDisposable
    {
        TDependency RetrieveRequiredDependency<TDependency>();

        bool RetrieveDependency<TDependency>(out TDependency dependency);

        /// <summary>
        /// The connect settings used when making an API request.
        /// </summary>
        IConnectionSettings Connection { get; }
    }
}
