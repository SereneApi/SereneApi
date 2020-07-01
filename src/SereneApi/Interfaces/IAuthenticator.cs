using System.Threading.Tasks;

namespace SereneApi.Interfaces
{
    public interface IAuthenticator
    {
        Task<IAuthentication> AuthenticateAsync();
    }
}
