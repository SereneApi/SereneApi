using SereneApi.Core.Requests;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Factories
{
    public interface IMockResponseFactory : IMockResponseEndpoint
    {
        IMockResponseEndpoint ForMethod(params Method[] methods);
    }
}