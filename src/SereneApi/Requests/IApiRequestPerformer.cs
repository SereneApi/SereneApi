using SereneApi.Abstractions.Response;
using System.Threading.Tasks;

namespace SereneApi.Requests
{
    public interface IApiRequestPerformer
    {
        IApiResponse Execute();

        Task<IApiResponse> ExecuteAsync();
    }
}
