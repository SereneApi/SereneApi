namespace SereneApi.Interfaces
{
    public interface IAuthenticator
    {
        IAuthentication GetAuthentication(IDependencyCollection dependencies);
    }
}
