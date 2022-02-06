using SereneApi.Core.Http.Responses;
using System.IO;
using System.Threading.Tasks;

namespace DependencyInjection.API
{
    public interface IValuesApi
    {
        Task<IApiResponse<int>> GetAsync(int value);

        Task<IApiResponse<string>> GetAsync(string value);

        Task<IApiResponse<MemoryStream>> GetSamplePfgAsync();
    }
}