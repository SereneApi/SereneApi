namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IRequestService
    {
        IRequestParameters AgainstService(string serviceName);
    }
}