using SereneApi.Abstractions.Connection;
using SereneApi.Requests;
using System;

namespace SereneApi.Tests.Interfaces
{
    public interface IApiHandlerWrapper : IDisposable
    {
        IConnectionSettings Connection { get; }

        IApiRequestBuilder MakeRequest { get; }

    }
}
