using SereneApi.Abstractions.Authentication;
using System.Threading.Tasks;
using SereneApi.Abstractions.Request;

namespace SereneApi.Abstractions.Authenticators
{
    /// <summary>
    /// Authenticates an <see cref="IApiRequest"/>
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Authenticates asynchronously and returns an <see cref="IAuthentication"/> once authentication has been completed.
        /// </summary>
        Task<IAuthentication> AuthenticateAsync();
    }
}
