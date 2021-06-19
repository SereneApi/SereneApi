using SereneApi.Abstractions.Response;
using System.Threading.Tasks;

namespace SereneApi.Requests
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
