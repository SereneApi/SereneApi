using SereneApi.Extensions.Mocking.Rest.Responses;
using SereneApi.Extensions.Mocking.Rest.Responses.Factories;

namespace SereneApi.Extensions.Mocking.Rest.Configuration
{
    public interface IMockingConfiguration
    {
        IMockResponseFactory RegisterMockResponse();

        void RegisterMockResponse(IMockResponseDescriptor mockResponse);
    }
}