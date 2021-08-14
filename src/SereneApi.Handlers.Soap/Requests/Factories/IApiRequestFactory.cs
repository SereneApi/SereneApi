namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IApiRequestFactory
    {
        IApiRequestParameters AgainstService(string serviceName);
    }
}
