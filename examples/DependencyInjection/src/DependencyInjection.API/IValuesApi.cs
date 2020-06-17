using SereneApi;
using System.Threading.Tasks;

namespace DependencyInjection.API
{
    public interface IValuesApi
    {
        Task<IApiResponse<int>> GetIntAsync(int value);
    }
}
