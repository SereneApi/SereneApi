using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Request;
using System.Threading.Tasks;

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
