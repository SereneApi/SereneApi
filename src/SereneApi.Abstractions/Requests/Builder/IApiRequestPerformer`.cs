using System.Threading.Tasks;
using SereneApi.Abstractions.Response;

namespace SereneApi.Abstractions.Requests.Builder
{
    public interface IApiRequestPerformer<TContent>
    {
        /// <summary>
        /// Performs the request Synchronously.
        /// </summary>
        IApiResponse<TContent> Execute();

        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse<TContent>> ExecuteAsync();
    }
}
