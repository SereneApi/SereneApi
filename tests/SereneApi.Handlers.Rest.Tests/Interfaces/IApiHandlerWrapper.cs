using System;
using SereneApi.Core.Connection;
using SereneApi.Handlers.Rest.Requests.Factories;

namespace SereneApi.Tests.Interfaces
{
    public interface IApiHandlerWrapper : IDisposable
    {
        IConnectionSettings Connection { get; }

        IApiRequestFactory MakeRequest { get; }

    }
}
