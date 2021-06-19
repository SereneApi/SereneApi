using SereneApi.Abstractions.Response;
using System.Threading.Tasks;

namespace SereneApi.Requests
{
    public interface IApiRequestPerformer
    {
        /// <summary>
        /// Performs the request Synchronously.
        /// </summary>
        IApiResponse Execute();

        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse> ExecuteAsync();
    }
}
