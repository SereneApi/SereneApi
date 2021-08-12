using System.Threading.Tasks;
using SereneApi.Core.Responses;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestPerformer<TContent>
    {
        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse<TContent>> ExecuteAsync();
    }
}
