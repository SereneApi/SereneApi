using System;
using SereneApi.Abstractions.Configuration;

namespace SereneApi.Abstractions.Handler.Options
{
    public interface IOptionsBuilder: IOptionsConfigurator, ICoreOptions, IDisposable
    {
        IOptions BuildOptions();
    }
}
