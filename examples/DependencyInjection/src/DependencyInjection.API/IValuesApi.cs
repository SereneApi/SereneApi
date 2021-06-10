using SereneApi.Abstractions.Response;
using System.Threading.Tasks;

namespace DependencyInjection.API
{
    public interface IValuesApi
    {
        Task<IApiResponse<int>> GetAsync(int value);

        IApiResponse<string> Get(string value);
    }
}
