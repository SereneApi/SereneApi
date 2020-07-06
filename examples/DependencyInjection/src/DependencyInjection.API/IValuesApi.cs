using SereneApi.Abstractions.Responses;
using System.Threading.Tasks;

namespace DependencyInjection.API
{
    public interface IValuesApi
    {
        Task<IApiResponse<int>> GetAsync(int value);

        IApiResponse<string> GetAsync(string value);
    }
}
