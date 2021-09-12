using SereneApi.Core.Responses;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestPerformer<TContent>
    {
        IApiResponse<TContent> Execute();

        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse<TContent>> ExecuteAsync();
    }
}