using System.Threading.Tasks;

namespace SereneApi.Core.Authorization.Authorizers
{
    /// <summary>
    /// Dynamically authorizes an API request.
    /// </summary>
    public interface IAuthorizer
    {
        IAuthorization Authorize();

        /// <summary>
        /// Authorizes a request asynchronously.
        /// </summary>
        Task<IAuthorization> AuthorizeAsync();
    }
}