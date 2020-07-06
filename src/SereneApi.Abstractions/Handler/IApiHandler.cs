using SereneApi.Abstractions.Configuration;
using System;

namespace SereneApi.Abstractions.Handler
{
    public interface IApiHandler: IDisposable
    {
        IConnectionSettings Connection { get; }
    }
}
