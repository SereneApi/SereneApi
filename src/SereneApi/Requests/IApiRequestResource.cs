namespace SereneApi.Requests
{
    public interface IApiRequestResource : IApiRequestVersion
    {
        IApiRequestVersion AgainstResource(string resource);
    }
}
