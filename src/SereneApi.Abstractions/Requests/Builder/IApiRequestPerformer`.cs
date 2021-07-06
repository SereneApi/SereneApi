using SereneApi.Abstractions.Response;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Requests.Builder
{
    public interface IApiRequestPerformer<TContent>
    {
        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse<TContent>> ExecuteAsync();
    }
}
