using SereneApi.Abstractions.Authentication;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Authenticators
{
    public interface IAuthenticator
    {
        Task<IAuthentication> AuthenticateAsync();
    }
}
