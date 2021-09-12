using SereneApi.Core.Options.Factories;
using System;

namespace SereneApi.Core.Handler.Factories
{
    /// <summary>
    /// Builds new instances of APIs using the provided settings.
    /// </summary>
    public interface IApiFactory : IDisposable
    {
        /// <summary>
        /// Builds a new instance of the specified API.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the specified API has not been registered.</exception>
        TApi Build<TApi>() where TApi : class;

        IApiOptionsExtensions ExtendApi<TApi>();

        void ExtendApi<TApi>(Action<IApiOptionsExtensions> builder) where TApi : class;

        IApiOptionsExtensions RegisterApi<TApi, TApiHandler>(Action<IApiOptionsFactory> builder = null) where TApiHandler : IApiHandler, TApi;
    }
}