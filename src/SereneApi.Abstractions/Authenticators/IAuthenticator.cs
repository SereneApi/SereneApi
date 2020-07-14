using SereneApi.Abstractions.Authentication;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Authenticators
{
    /// <summary>
    /// Dynamically authenticates an API request.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Authenticates a request asynchronously.
        /// </summary>
        Task<IAuthentication> AuthenticateAsync();
    }
}
