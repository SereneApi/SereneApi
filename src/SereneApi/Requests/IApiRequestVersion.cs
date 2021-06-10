using SereneApi.Abstractions;

namespace SereneApi.Requests
{
    public interface IApiRequestVersion : IApiRequestEndpoint
    {
        IApiRequestEndpoint AgainstVersion(string version);

        IApiRequestEndpoint AgainstVersion(IApiVersion version);
    }
}
