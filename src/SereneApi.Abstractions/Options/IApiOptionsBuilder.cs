using System;

namespace SereneApi.Abstractions.Options
{
    /// <summary>
    /// Builds API options using the specific configuration.
    /// </summary>
    public interface IApiOptionsBuilder: IApiOptionsConfigurator, ICoreOptions, IDisposable
    {
        /// <summary>
        /// Builds options for an API.
        /// </summary>
        IApiOptions BuildOptions();
    }
}
