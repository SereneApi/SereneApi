using SereneApi.Abstractions.Response;
using System.Threading.Tasks;

namespace SereneApi.Requests
{
    public interface IApiRequestPerformer<TContent>
    {
        IApiResponse<TContent> Execute();

        Task<IApiResponse<TContent>> ExecuteAsync();
    }
}
