using SereneApi.Interfaces;

namespace SereneApi.Extensions.Mocking.Interfaces
{
    public interface IMockResponseBuilder
    {
        void UseSerializer(ISerializer serializer);

        IMockResponseExtensions AddMockResponse<TContent>(TContent content);
    }
}
