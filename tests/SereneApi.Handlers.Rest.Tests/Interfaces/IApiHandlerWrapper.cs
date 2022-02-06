using SereneApi.Core.Http;
using SereneApi.Handlers.Rest.Requests.Factories;
using System;

namespace SereneApi.Handlers.Rest.Tests.Interfaces
{
    public interface IApiHandlerWrapper : IDisposable
    {
        IConnectionSettings Connection { get; }

        IApiRequestMethod MakeRequest { get; }
    }
}