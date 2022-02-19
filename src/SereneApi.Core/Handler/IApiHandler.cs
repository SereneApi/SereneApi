using SereneApi.Core.Http;
using System;

namespace SereneApi.Core.Handler
{
    public interface IApiHandler : IDisposable
    {
        IConnectionSettings Connection { get; }
    }
}