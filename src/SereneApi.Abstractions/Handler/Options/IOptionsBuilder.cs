using SereneApi.Abstractions.Configuration;
using System;

namespace SereneApi.Abstractions.Handler.Options
{
    public interface IOptionsBuilder: IOptionsConfigurator, ICoreOptions, IDisposable
    {
        IOptions BuildOptions();
    }
}
