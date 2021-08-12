using System.Threading.Tasks;
using SereneApi.Core.Responses;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestPerformer
    {
        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse> ExecuteAsync();
    }
}
