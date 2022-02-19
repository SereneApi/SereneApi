using System;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Factories
{
    public interface IMockResponseEndpoint : IMockResponseContent
    {
        IMockResponseContent ForEndpoints(params string[] endpoints);

        IMockResponseContent ForEndpoints(params Uri[] endpoints);
    }
}