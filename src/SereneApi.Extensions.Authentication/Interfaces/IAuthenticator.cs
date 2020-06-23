using SereneApi.Interfaces;

namespace SereneApi.Extensions.Authentication.Interfaces
{
    public interface IAuthenticator
    {
        IAuthentication GetAuthentication();
    }
}
