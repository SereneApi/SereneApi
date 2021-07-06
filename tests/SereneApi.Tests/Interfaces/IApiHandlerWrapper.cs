using SereneApi.Abstractions.Connection;
using SereneApi.Abstractions.Requests.Builder;
using System;

namespace SereneApi.Tests.Interfaces
{
    public interface IApiHandlerWrapper : IDisposable
    {
        IConnectionSettings Connection { get; }

        IApiRequestBuilder MakeRequest { get; }

    }
}
