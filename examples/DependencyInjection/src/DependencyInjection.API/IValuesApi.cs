using SereneApi.Abstractions;
using System.Threading.Tasks;
using SereneApi.Abstractions.Responses;

namespace DependencyInjection.API
{
    public interface IValuesApi
    {
        Task<IApiResponse<int>> GetAsync(int value);

        IApiResponse<string> GetAsync(string value);
    }
}
