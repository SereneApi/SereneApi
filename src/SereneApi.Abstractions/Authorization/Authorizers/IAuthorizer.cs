using SereneApi.Abstractions.Authorization;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Authorisation.Authorizers
{
    /// <summary>
    /// Dynamically authenticates an API request.
    /// </summary>
    public interface IAuthorizer
    {
        /// <summary>
        /// Authenticates a request asynchronously.
        /// </summary>
        Task<IAuthorization> AuthorizeAsync();
    }
}
