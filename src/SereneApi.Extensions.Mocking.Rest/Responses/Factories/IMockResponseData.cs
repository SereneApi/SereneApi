using SereneApi.Core.Responses;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Factories
{
    public interface IMockResponseData
    {
        IMockResponseDelay RespondsWith(Status status);

        IMockResponseDelay RespondsWith<TContent>(TContent content, Status status = Status.Ok);
    }
}