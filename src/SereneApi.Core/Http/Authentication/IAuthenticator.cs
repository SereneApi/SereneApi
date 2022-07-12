using System.Threading.Tasks;

namespace SereneApi.Core.Http.Authentication
{
    /// <summary>
    /// Dynamically authorizes an API request.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Authorizes a request asynchronously.
        /// </summary>
        Task<IAuthentication> AuthorizeAsync();
    }
}