namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IApiRequestService
    {
        IApiRequestParameters AgainstService(string serviceName);
    }
}
