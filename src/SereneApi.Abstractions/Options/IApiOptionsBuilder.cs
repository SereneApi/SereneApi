using System;
using SereneApi.Abstractions.Configuration;

namespace SereneApi.Abstractions.Options
{
    public interface IApiOptionsBuilder: IApiOptionsConfigurator, ICoreOptions, IDisposable
    {
        IApiOptions BuildOptions();
    }
}
