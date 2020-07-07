using SereneApi.Abstractions.Configuration;
using System;

namespace SereneApi.Abstractions.Options
{
    public interface IApiOptionsBuilder: IApiOptionsConfigurator, ICoreOptions, IDisposable
    {
        IApiOptions BuildOptions();
    }
}
