using SereneApi.Core.Connection;
using System;

namespace SereneApi.Core.Handler
{
    public interface IApiHandler : IDisposable
    {
        IConnectionSettings Connection { get; }
    }
}