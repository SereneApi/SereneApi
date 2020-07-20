using SereneApi.Abstractions.Authorization;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Authorisation.Authorizers
{
    /// <summary>
    /// Dynamically authorizes an API request.
    /// </summary>
    public interface IAuthorizer
    {
        /// <summary>
        /// Authorizes a request asynchronously.
        /// </summary>
        Task<IAuthorization> AuthorizeAsync();
    }
}
