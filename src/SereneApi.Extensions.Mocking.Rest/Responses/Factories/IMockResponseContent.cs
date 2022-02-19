namespace SereneApi.Extensions.Mocking.Rest.Responses.Factories
{
    public interface IMockResponseContent : IMockResponseData
    {
        IMockResponseData ForContent<TContent>(TContent content);
    }
}