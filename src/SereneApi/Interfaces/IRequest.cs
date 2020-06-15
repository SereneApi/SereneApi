namespace SereneApi.Interfaces
{
    public interface IRequest : IRequestEndpoint
    {
        IRequestEndpoint AgainstResource(string resource);
    }
}
