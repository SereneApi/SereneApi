namespace SereneApi.Requests
{
    public interface IApiRequestEndpoint : IApiRequestParameters
    {
        IApiRequestParameters WithEndpoint(string endpoint);
    }
}
