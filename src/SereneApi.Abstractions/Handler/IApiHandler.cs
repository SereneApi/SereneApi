using System;
using SereneApi.Abstractions.Configuration;

namespace SereneApi.Abstractions.Handler
{
    public interface IApiHandler: IDisposable
    {
        IConnectionSettings Connection { get; }
    }
}
