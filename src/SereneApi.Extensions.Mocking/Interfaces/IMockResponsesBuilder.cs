using SereneApi.Abstraction.Enums;
using SereneApi.Interfaces;

namespace SereneApi.Extensions.Mocking.Interfaces
{
    public interface IMockResponsesBuilder
    {
        void UseSerializer(ISerializer serializer);

        IMockResponseExtensions AddMockResponse(Status status, string message = null);
        IMockResponseExtensions AddMockResponse<TContent>(TContent content);
        IMockResponseExtensions AddMockResponse<TContent>(TContent content, Status status);


    }
}
