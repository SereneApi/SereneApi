using System;

namespace SereneApi.Abstractions.Options
{
    public interface IApiOptionsFactory : IApiOptionsExtensions, IApiOptionsBuilder, IDisposable
    {
        IApiOptions BuildOptions();
    }
}
