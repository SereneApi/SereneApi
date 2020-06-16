namespace SereneApi.Interfaces
{
    public interface IRequest: IRequestEndPoint
    {
        IRequestEndPoint AgainstResource(string resource);
    }
}
